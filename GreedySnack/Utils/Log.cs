using System;
using System.IO;

namespace GreedySnack.Utils
{
    /// <summary>
    /// 日志工具类
    /// </summary>
    public class Log
    {
        private static readonly string _path = Config.Get("Logger.Path", @"logs/");
        private static readonly string _debugFileName = Config.Get("Logger.FileName.Debug", @"tmp");
        private static readonly string _errorFileName = Config.Get("Logger.FileName.Error", @"err");
        private static readonly string _warnFileName = Config.Get("Logger.FileName.Warn", @"warn");
        private static readonly string _ext = Config.Get("Logger.FileExtension", @".log");
        private static readonly string _dateFormatter = Config.Get("Logger.DateFormatter", @"yyyy年MM月dd日 HH:mm:ss");


        // 委托 用于控制台的异步write
        private delegate void OutputConsoleDelegate(int code, string desc, ConsoleColor consoleColor);

        /// <summary>
        /// 将日志写入目标文件（异步）
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="code">日志code</param>
        /// <param name="desc">描述</param>
        private async static void Write(string path, int code, string desc)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(fs);
            DateTime now = DateTime.Now;

            await sw.WriteLineAsync();
            await sw.WriteLineAsync("Code:" + code);
            await sw.WriteLineAsync("Time:" + now.ToString(_dateFormatter));
            await sw.WriteLineAsync("Desc:" + desc);
            await sw.WriteLineAsync();

            sw.Dispose();
            fs.Dispose();
            sw.Close();
        }

        /// <summary>
        /// 异步输出到控制台
        /// </summary>
        /// <param name="code">日志code</param>
        /// <param name="desc">描述</param>
        /// <param name="consoleColor">控制台文字颜色</param>
        public static void AsyncOutputToConsole(int code, string desc, ConsoleColor consoleColor)
        {
            string dateInfo = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ");

            Console.ForegroundColor = consoleColor;
            Console.WriteLine(dateInfo + code + " - " + desc + "\r\n");
        }

        /// <summary>
        /// 记录Debug日志
        /// </summary>
        /// <param name="code">日志code</param>
        /// <param name="desc">描述</param>
        /// <param name="alsoConsole">是否也输出到控制台</param>
        public static void Debug(int code, string desc, bool alsoConsole)
        {
            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

            string path = _path + _debugFileName + _ext;

            if (alsoConsole)
            {
                OutputConsoleDelegate consoleDelegate = new OutputConsoleDelegate(AsyncOutputToConsole);
                IAsyncResult result = consoleDelegate.BeginInvoke(code, desc, ConsoleColor.Red, null, null);
            }

            Write(path, code, desc);
        }

        /// <summary>
        /// 记录Error日志
        /// </summary>
        /// <param name="code">日志code</param>
        /// <param name="desc">描述</param>
        /// <param name="alsoConsole">是否也输出到控制台</param>
        public static void Error(int code, string desc, bool alsoConsole)
        {
            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

            string path = _path + _errorFileName + _ext;

            if (alsoConsole)
            {
                OutputConsoleDelegate consoleDelegate = new OutputConsoleDelegate(AsyncOutputToConsole);
                IAsyncResult result = consoleDelegate.BeginInvoke(code, desc, ConsoleColor.White, null, null);
            }

            Write(path, code, desc);
        }

        /// <summary>
        /// 记录Warn日志
        /// </summary>
        /// <param name="code">日志code</param>
        /// <param name="desc">描述</param>
        /// <param name="alsoConsole">是否也输出到控制台</param>
        public static void Warn(int code, string desc, bool alsoConsole)
        {
            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

            string path = _path + _warnFileName + _ext;

            if (alsoConsole)
            {
                OutputConsoleDelegate consoleDelegate = new OutputConsoleDelegate(AsyncOutputToConsole);
                IAsyncResult result = consoleDelegate.BeginInvoke(code, desc, ConsoleColor.DarkYellow, null, null);
            }

            Write(path, code, desc);
        }
    }
}
