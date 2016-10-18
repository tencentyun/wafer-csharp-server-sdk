using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QCloud.WeApp.SDK.Authorization
{
    [JsonObject]
    public class UserInfo
    {
        [JsonProperty("openId")]
        public string OpenId { get; set; }

        [JsonProperty("nickName")]
        public string NickName { get; set; }

        [JsonProperty("avatarUrl")]
        public string AvatarUrl { get; set; }

        [JsonProperty("gender")]
        public int Gender { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        public static UserInfo BuildFromJson(dynamic json)
        {
            if (json == null) return null;
            return new UserInfo()
            {
                OpenId = json.openId,
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
