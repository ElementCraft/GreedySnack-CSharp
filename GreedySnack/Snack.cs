using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GreedySnack
{
    /// <summary>
    /// 贪吃蛇类
    /// </summary>
    public class Snack : ISnack
    {
        public LinkedList<Node> Body { get; private set; }
        public float Speed { get; set; }
        public Vector2 FaceAngle { get; set; }

        [Obsolete]
        private bool _isFaceAngleChanged = false;


        /// <summary>
        /// 构造方法，初始化蛇实体的一些属性
        /// </summary>
        /// <param name="head_loc">头部节点坐标</param>
        /// <param name="tail_loc">尾巴节点坐标</param>
        /// <param name="speed">移速</param>
        /// <param name="faceAngle">面向方向向量</param>
        public Snack(PointF head_loc, PointF tail_loc, float speed, Vector2 faceAngle)
        {
            this.Body = new LinkedList<Node>();

            this.Body.AddLast(new Node(head_loc.X, head_loc.Y));
            this.Body.AddLast(new Node(tail_loc.X, tail_loc.Y));

            this.Speed = speed;
            this.FaceAngle = faceAngle;
        }

        public void Eat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 改变蛇头方向
        /// </summary>
        /// <param name="faceAngle">表示面向方向的向量</param>
        public void RotateHead(Vector2 faceAngle)
        {
            if (this.FaceAngle != faceAngle && !Vector2.Empty.Equals(this.FaceAngle + faceAngle))
            {
                this.FaceAngle = faceAngle;

                // 拐弯，新增一个头节点
                Node node = this.Body.First.Value;
                this.Body.AddFirst(new Node(node.X, node.Y));
            }
        }

        public void Walk(float tick)
        {
            float walkDistance = this.Speed * tick / 1000.0f;

            // 多线程加锁
            lock (this.Body)
            {
                Node headNode = this.Body.First.Value;

                // 计算蛇头前进之后的位置
                Vector3 headVector = new Vector3(headNode.X, headNode.Y, 0);
                Matrix translate = new Matrix();
                Vector3 walkAngle = Vector3.Normalize(new Vector3(this.FaceAngle.X, this.FaceAngle.Y, 0));
                translate.Translate(Vector3.Multiply(walkAngle, walkDistance));
                headVector.TransformCoordinate(translate);

                //删除当前头节点,再新增节点 ，等价于直接在头节点延长
                this.Body.RemoveFirst();
                this.Body.AddFirst(new Node(headVector.X, headVector.Y));

                
            }

            // 缩尾巴
            TailWalk(walkDistance);
        }

        /// <summary>
        /// 缩尾巴
        /// </summary>
        /// <param name="walkDistance">要缩小的长度</param>
        private void TailWalk(float walkDistance)
        {
            Node tail = this.Body.Last.Value;
            Node lastButOne = this.Body.Last.Previous.Value;

            #region 计算倒数两个节点的距离，大于要移动的距离则缩短尾节点。小于则删除尾节点 重复该过程。

            Vector3 diffVector = new Vector3(lastButOne.X - tail.X, lastButOne.Y - tail.Y, 0);
            float diffDistance = diffVector.Length();

            // 多线程锁
            lock (this.Body)
            { 
                if (diffDistance > walkDistance)
                {
                    Matrix matrix = new Matrix();
                    matrix.Translate(Vector3.Multiply(Vector3.Normalize(diffVector), walkDistance));
                    Vector3 resultVector = new Vector3(tail.X, tail.Y, 0);
                    resultVector.TransformCoordinate(matrix);

                    this.Body.RemoveLast();
                    this.Body.AddLast(new Node(resultVector.X, resultVector.Y));
                    return;
                }
                else
                {
                    walkDistance -= diffDistance;
                    this.Body.RemoveLast();
                }
            }

            TailWalk(walkDistance);
            #endregion
        }

        public void Render(Device device)
        {
            Vector2[] vector2s;

            // 多线程加锁
            lock (this.Body)
            {
                int nodesCount = this.Body.Count;

                // 汇总所有蛇身的点
                vector2s = new Vector2[nodesCount];

                int i = 0;
                foreach (var node in this.Body)
                {
                    vector2s[i] = new Vector2(node.X, node.Y);
                    ++i;
                }
            }

            Line line = new Line(device);
            line.Width = 4;
            line.Antialias = true;
            line.Draw(vector2s, Color.SkyBlue);
        }

        /// <summary>
        /// 蛇身节点结构体
        /// </summary>
        public struct Node
        {
            public float X;
            public float Y;

            public Node(float x, float y)
            {
                this.X = x;
                this.Y = y;
            }
        }
    }
}
