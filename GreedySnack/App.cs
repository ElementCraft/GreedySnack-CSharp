using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using GreedySnack.Utils;

namespace GreedySnack
{

    /// <summary>
    /// 游戏窗口和逻辑控制类
    /// </summary>
    public class App : Form
    {
        #region 属性

        public static App Instance { get; private set; }
        public Microsoft.DirectX.Direct3D.Device D3DDevice { get; private set; }

        public Microsoft.DirectX.DirectInput.Device KeyboardDevice { get; private set; }

        /// <summary>
        /// 游戏是否已经结束
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// 游戏是否暂停
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// 每帧间隔时间
        /// </summary>
        public float PerFrameTick { get; private set; }

        /// <summary>
        /// 游戏已经运行过的总帧数
        /// </summary>
        public int TotalFrameCount { get; private set; }

        /// <summary>
        /// 玩家1蛇的实体
        /// </summary>
        public Snack P1 { get; private set; }

        /// <summary>
        /// 玩家2蛇的实体
        /// </summary>
        public Snack P2 { get; private set; }

        // 逻辑线程
        private Thread _logicalThread = null;
        
        // 渲染线程
        private Thread _renderThread = null;

        // 暂存的帧计时
        private float _tempFrameTick = 0.0f;

        

        #endregion

        #region 构造

        public App()
        {
            // 读取窗口属性配置项
            int screenW = Config.Get("Screen.Width", 800);
            int screenH = Config.Get("Screen.Height", 600);
            string gameName = Config.Get("Game.Name", @"Greedy Snack");
            string gameVersion = Config.Get("Game.Version", @"V1.0");

            // 设置窗口属性
            this.Size = new Size(screenW, screenH);
            this.Text = gameName + " " + gameVersion;
            this.Icon = new Icon("icon.ico");
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;

            this.FormClosing += App_FormClosing;

            // 属性初始化赋值
            this.IsFinished = false;
            this.IsPaused = false;
            this.TotalFrameCount = 0;
        }

        /// <summary>
        /// 窗体正在关闭事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.IsFinished = true;
            
            this.D3DDevice.Dispose();
            this.D3DDevice = null;
        }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Init()
        {
            try
            {
                if (Instance != null)
                {
                    Log.Warn(code: 0, desc: @"App实例已存在，不能再次初始化", alsoConsole: true);
                    return true;    //幂等
                }

                //预设参数
                PresentParameters pp = new PresentParameters();
                pp.Windowed = true;
                pp.SwapEffect = SwapEffect.Discard;

                //创建设备
                this.D3DDevice = new Microsoft.DirectX.Direct3D.Device(
                    0,
                    Microsoft.DirectX.Direct3D.DeviceType.Hardware,
                    this,
                    CreateFlags.SoftwareVertexProcessing,
                    pp
                );

                //input设备
                this.KeyboardDevice = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
                this.KeyboardDevice.SetCooperativeLevel(this, CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Background);
                this.KeyboardDevice.Acquire();

                Instance = this;

                //设置每帧时间间隔
                int fps = Config.Get("Game.FPS", 60);
                this.PerFrameTick = 1000.0f / fps;

                return true;
            }
            catch (DirectXException e)
            {
                MessageBox.Show(e.Message, "DirectX初始化失败");
                Log.Error(e.ErrorCode, e.Message, alsoConsole: true);
                return false;
            }
        }

        /// <summary>
        /// 键盘按键逻辑
        /// </summary>
        void KeyboardProcess()
        {
            KeyboardState kbState = KeyboardDevice.GetCurrentKeyboardState();

            // 按住空格暂停游戏
            if (kbState[Key.Space])
            {
                this.IsPaused = true;
            }
            else
            {
                this.IsPaused = false;
            }

            // P1键盘输入处理        
            Vector2 vec = Vector2.Empty;
            if (kbState[Key.W])
            {
                vec += new Vector2(0, -1);
            }
            if (kbState[Key.A])
            {
                vec += new Vector2(-1, 0);
            }
            if (kbState[Key.S])
            {
                vec += new Vector2(0, 1);
            }
            if (kbState[Key.D])
            {
                vec += new Vector2(1, 0);
            }

            if (!vec.Equals(Vector2.Empty)) P1.RotateHead(vec);
        }

        /// <summary>
        /// 游戏主逻辑
        /// </summary>
        void Play()
        {
            while (!this.IsFinished)
            {
                KeyboardProcess();

                // 计算帧
                float passTick = Clock.Tick();
                _tempFrameTick += passTick;

                while (_tempFrameTick > PerFrameTick)
                {
                    _tempFrameTick -= PerFrameTick;

                    if (this.IsPaused) break;   // 游戏暂停则不执行逻辑

                    this.TotalFrameCount++;
                    UpdateFrame(PerFrameTick);
                }
            }


        }

        /// <summary>
        /// 新一帧的逻辑
        /// </summary>
        /// <param name="passTick">距离上一帧经过的时间</param>
        public void UpdateFrame(float passTick)
        {
            P1.Walk(passTick);
        }

        /// <summary>
        /// 游戏渲染、绘制
        /// </summary>
        void Render()
        {
            while (!this.IsFinished)
            {
                if (this.IsPaused) continue;
                if (this.D3DDevice == null) return;

                this.D3DDevice.Clear(ClearFlags.Target, Color.FromArgb(60, 60, 60), 1f, 0);

                this.D3DDevice.BeginScene();

                P1.Render(this.D3DDevice);

                this.D3DDevice.EndScene();
                this.D3DDevice.Present();
            }
            
        }

        /// <summary>
        /// 开始游戏（创建线程）
        /// </summary>
        public void GameStart()
        {
            // 初始化玩家1的蛇
            P1 = new Snack(new PointF(175, 100), new PointF(25, 100), 200, new Vector2(1, 0));

            Clock.Init();

            // 逻辑线程
            _logicalThread = new Thread(new ThreadStart(Play));
            _logicalThread.IsBackground = true;

            // 渲染线程
            _renderThread = new Thread(new ThreadStart(Render));
            _renderThread.IsBackground = true;

            _logicalThread.Start();
            _renderThread.Start();
        }
    }
}
