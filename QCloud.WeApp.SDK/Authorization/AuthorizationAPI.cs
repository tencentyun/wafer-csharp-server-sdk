using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace QCloud.WeApp.SDK.Authorization
{
    /// <summary>
    /// 访问鉴权服务 API
    /// </summary>
    internal class AuthorizationAPI
    {
        /// <summary>
        /// 从配置文件读取 API 访问地址
        /// </summary>
        private string APIEndpoint
        {
            get
            {
                return ConfigurationManager.CurrentConfiguration.AuthServerUrl;
            }
        }

        /// <summary>
        /// 调用登录 API
        /// </summary>
        /// <param name="code">微信登录后得到的 code</param>
        /// <param name="encryptData">微信获取用户数据后得到的加密数据</param>
        /// <returns>返回登录的结果，包含会话信息以及用户信息</returns>
        public LoginResult Login(string code, string encryptData)
        {
            var result = Request("qcloud.cam.id_skey", new { code, encrypt_data = encryptData });
            return new LoginResult()
            {
                Id = result.id,
                Skey = result.skey,
                UserInfo = UserInfo.BuildFromJson(result.user_info)
            };
        }

        /// <summary>
        /// 调用检查登录 API
        /// </summary>
        /// <param name="id">会话 ID</param>
        /// <param name="skey">会话 SKey</param>
        /// <returns>返回检查登录结果，包含用户信息</returns>
        public CheckLoginResult CheckLogin(string id, string skey)
        {
            var result = Request("qcloud.cam.auth", new { id, skey });
            return new CheckLoginResult()
            {
                UserInfo = UserInfo.BuildFromJson(result.user_info)
            };
        }

        /// <summary>
        /// 通用 API 请求方法
        /// </summary>
        /// <param name="apiName">API 名称</param>
        /// <param name="apiParams">API 参数</param>
        /// <returns>API 返回的数据</returns>
        public dynamic Request(string apiName, object apiParams)
        {
            using (SdkDebug.WriteLineAndIndent("> 请求鉴权服务 API"))
            {
                return DoRequest(apiName, apiParams);
            }
        }

        private dynamic DoRequest(string apiName, object apiParams)
        {
            string responseContent;

            // 请求授权服务器，获取返回报文
            try
            {
                string requestContent = BuildRequestBody(apiName, apiParams);

                DateTime start = DateTime.Now;
                responseContent = Http.Post(APIEndpoint, BuildRequestBody(apiName, apiParams));
                DateTime end = DateTime.Now;
                TimeSpan cost = end - start;

                using (SdkDebug.WriteLineAndIndent($"POST {APIEndpoint} (Time: {start.ToString("HH:mm:ss")}, Cost: {cost.TotalMilliseconds}ms)"))
                {
                    using (SdkDebug.WriteLineAndIndent("Requset:"))
                    {
                        SdkDebug.WriteLine(requestContent);
                    }
                    using (SdkDebug.WriteLineAndIndent("Response:"))
                    {
                        SdkDebug.WriteLine(responseContent);
                    }
                }
            }
            catch (Exception error)
            {
                using (SdkDebug.WriteLineAndIndent($"POST {APIEndpoint} (ERROR)"))
                {
                    SdkDebug.WriteLine(error);
                }
                throw new HttpRequestException($"请求鉴权 API 失败，网络异常或鉴权服务器错误：{error.Message}", error);
            }

            // 解析返回报文
            try
            {
                dynamic body = JsonConvert.DeserializeObject(responseContent);

                if (body.returnCode != 0)
                {
                    throw new AuthorizationAPIException($"鉴权服务调用失败：#{body.returnCode} - ${body.returnMessage}") { Code = body.returnCode };
                }

                return body.returnData;
            }
            catch (JsonException e)
            {
                throw new JsonException("鉴权服务器响应格式错误，无法解析 JSON 字符串", e);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 给定 API 名称和参数构建请求报文
        /// </summary>
        /// <param name="apiName">API 名称</param>
        /// <param name="apiParams">API 参数</param>
        /// <returns></returns>
        private string BuildRequestBody(string apiName, object apiParams)
        {
            var body = JsonConvert.SerializeObject(new
            {
                @version = 1,
                @componentName = "MA",
                @interface = new
                {
                    @interfaceName = apiName,
                    @para = apiParams
                }
            });
            return body;
        }
    }
}
