using System;
using System.Configuration;
using System.Collections.Specialized;

namespace GreedySnack.Utils
{

    /// <summary>
    /// 程序配置项管理类
    /// </summary>
    public class Config
    {
        private static NameValueCollection _configs = ConfigurationManager.AppSettings;

        /// <summary>
        /// 获取指定Key的配置项
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">配置项不存在时的默认值</param>
        /// <returns>配置项值</returns>
        public static string Get(string key, string defaultValue)
        {
            string str = _configs.Get(key);
            if (String.IsNullOrEmpty(str)) return defaultValue;

            return str;
        }

        /// <summary>
        /// 获取指定Key的配置项，带类型转换
        /// </summary>
        /// <typeparam name="T">要转换到的目标数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">配置项不存在时的默认值</param>
        /// <returns>配置项值</returns>
        public static T Get<T> (string key, T defaultValue)
        {
            string str = _configs.Get(key);
            if (String.IsNullOrEmpty(str)) return defaultValue;

            T result = default(T);
            result = (T)Convert.ChangeType(str, typeof(T));
            return result;
        }
    }
}
