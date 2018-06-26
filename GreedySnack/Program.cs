using GreedySnack.Utils;
using System;
using System.Windows.Forms;

namespace GreedySnack
{
    class Program
    {
        private static void Main(string[] args)
        {
            using (App app = new App())
            {
                if (app.Init())
                {
                    app.Show();

                    // 游戏开始
                    app.GameStart();
                }
                else
                {
                    return;
                }

                while (app.Created)
                {
                    Application.DoEvents();
                }
            }
        }
    }
}
