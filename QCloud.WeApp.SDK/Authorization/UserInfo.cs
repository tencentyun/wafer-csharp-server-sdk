using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QCloud.WeApp.SDK.Authorization
{
    /// <summary>
    /// 表示微信小程序用户信息
    /// </summary>
    [JsonObject]
    public class UserInfo
    {
        /// <summary>
        /// 用户在小程序对应的 openId
        /// </summary>
        [JsonProperty("openId")]
        public string OpenId { get; set; }

        /// <summary>
        /// 用户在小程序对应的 unionId
        /// </summary>
        [JsonProperty("unionId")]
        public string UnionId { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [JsonProperty("nickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 用户头像地址
        /// </summary>
        [JsonProperty("avatarUrl")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 用户性别
        /// </summary>
        [JsonProperty("gender")]
        public int Gender { get; set; }

        /// <summary>
        /// 用户语言
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// 用户城市
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// 用户省份
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        /// 用户国家
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// 从 JSON 对象构建用户信息
        /// </summary>
        /// <param name="json">JSON 对象</param>
        /// <returns></returns>
        internal static UserInfo BuildFromJson(dynamic json)
        {
            if (json == null) return null;
            return new UserInfo()
            {
                OpenId = json.openId,
                UnionId = json.unionId,
                NickName = json.nickName,
                AvatarUrl = json.avatarUrl,
                Gender = json.gender,
                Language = json.language,
                City = json.city,
                Province = json.province,
                Country = json.country
            };
        }
    }
}
