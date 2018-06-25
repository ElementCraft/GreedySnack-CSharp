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
                app.Show();

                while (app.Created)
                {
                    Application.DoEvents();
                }
            }
        }
    }
}
