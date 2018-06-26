using Microsoft.DirectX;
using System;

namespace GreedySnack
{
    /// <summary>
    /// 蛇的行为
    /// </summary>
    interface ISnack
    {
        void Walk(float tick);

        void RotateHead(Vector2 faceAngle);

        void Eat();
    }
}
