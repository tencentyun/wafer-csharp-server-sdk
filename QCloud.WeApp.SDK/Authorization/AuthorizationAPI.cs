using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;

namespace QCloud.WeApp.SDK
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
        public async Task<LoginResult> Login(string code, string encryptData)
        {
            var result = await Request("qcloud.cam.id_skey", new { code, encrypt_data = encryptData });
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
        public async Task<CheckLoginResult> CheckLogin(string id, string skey)
        {
            var result = await Request("qcloud.cam.auth", new { id, skey });
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
        public async Task<dynamic> Request(string apiName, object apiParams)
        {
            var http = Http.CreateClient();

            HttpResponseMessage response = null;
            try
            {
                response = await http.PostAsync(APIEndpoint, BuildRequestBody(apiName, apiParams));

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException("错误的 HTTP 响应：" + response.StatusCode);
                }
            }
            catch (Exception error) {
                throw new HttpRequestException("请求鉴权 API 失败，网络异常或鉴权服务器错误", error);
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("==============Response==============");
            Debug.WriteLine(responseBody);
            Debug.WriteLine("");
            try
            {
                dynamic body = JsonConvert.DeserializeObject(responseBody);

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
        private StringContent BuildRequestBody(string apiName, object apiParams)
        {
            var stringBody = JsonConvert.SerializeObject(new
            {
                @version = 1,
                @componentName = "MA",
                @interface = new
                {
                    @interfaceName = apiName,
                    @para = apiParams
                }
            });
            Debug.WriteLine("==============Request==============");
            Debug.WriteLine(stringBody);
            Debug.WriteLine("");
            return new StringContent(stringBody, new UTF8Encoding(false));
        }
    }
}
