using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    /// <summary>
    /// 提供快捷的 JSON 拓展方法
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 把对象序列化为 JSON 字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>JSON 字符串</returns>
        /// <example>
        /// <code language="cs">
        /// var userInfoJson = new { name: "techird", age: 18 }.ToJson();
        /// </code>
        /// </example>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 从 JSON 字符串反序列化到指定的匿名类型
        /// </summary>
        /// <typeparam name="T">匿名类型</typeparam>
        /// <param name="shape">匿名类型的实例</param>
        /// <param name="json">需要反序列化的 JSON 字符串</param>
        /// <returns>匿名类型的实例</returns>
        /// <example>
        /// <code language="cs">
        /// var userInfo = new { name: "", age: 0 }.ParseFromJson(userInfoJson);
        /// </code>
        /// </example>
        public static T ParseFromJson<T>(this T shape, string json)
        {
            return JsonConvert.DeserializeAnonymousType(json, shape);
        }
    }
}
