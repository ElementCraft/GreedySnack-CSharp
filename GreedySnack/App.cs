using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using GreedySnack.Utils;

namespace GreedySnack
{
    public class App : Form
    {
        public App Instance { get; private set; }

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

            
        }
    }
}
