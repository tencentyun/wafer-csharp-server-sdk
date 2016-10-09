using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;

namespace QCloud.WeApp.SDK
{

    public class LoginService
    {
        private HttpRequestBase Request;
        private HttpResponseBase Response;

        /// <summary>
        /// 配合腾讯云鉴权服务器提供微信小程序登录态服务，需要提供 HTTP 的请求和响应对象进行初始化
        /// </summary>
        public LoginService(HttpRequestBase request, HttpResponseBase response) {
            if (request == null)
            {
                throw new ArgumentNullException("request", "初始化登录服务时，request 不能为 null");
            }
            if (response == null)
            {
                throw new ArgumentNullException("response", "初始化登录服务时，response 不能为 null");
            }
            this.Request = request;
            this.Response = response;
        }

        /// <summary>
        /// 配合腾讯云鉴权服务器提供微信小程序登录态服务，需要提供 HTTP 的请求和响应对象进行初始化
        /// </summary>
        public LoginService(HttpRequest request, HttpResponse response) : this(new HttpRequestWrapper(request), new HttpResponseWrapper(response)) { }

        /// <summary>
        /// 请求登陆，获得用户信息。<para />
        /// 无论登录成功与否，会直接进行 HTTP 响应登录结果，使用该方法后无需再进行 HTTP 响应。<para />
        /// 需要获取登陆结果的，可以直接 await 登录方法的返回值。需要获取登陆错误的，请直接使用 try-catch 捕获。
        /// </summary>
        public async Task<UserInfo> Login()
        {
            var code = GetHeader(Constants.WX_HEADER_CODE);
            var encryptData = GetHeader(Constants.WX_HEADER_ENCRYPT_DATA);

            LoginResult loginResult = null;

            try
            {
                var api = new AuthorizationAPI();
                loginResult = await api.Login(code, encryptData);
            }
            catch (Exception apiError)
            {
                var error = new LoginServiceException(Constants.ERR_LOGIN_FAILED, apiError.Message, apiError);
                Response.WriteJson(JsonForError(error));
                throw error;
            }

            var json = PrepareResponseJsonDictionary();
            json["session"] = new { id = loginResult.Id, skey = loginResult.Skey };
            Response.WriteJson(json);
            return loginResult.UserInfo;
        }

        /// <summary>
        /// 检查当前请求是否包含已登录的会话，如果已登录，会返回用户信息，否则将抛出异常
        /// </summary>
        /// <returns>用户信息</returns>
        public async Task<UserInfo> CheckLogin()
        {

            var id = GetHeader(Constants.WX_HEADER_ID);
            var skey = GetHeader(Constants.WX_HEADER_SKEY);

            CheckLoginResult checkLoginResult = null;

            try
            {
                var api = new AuthorizationAPI();
                checkLoginResult = await api.CheckLogin(id, skey);
            }
            catch (Exception apiError)
            {
                var error = new LoginServiceException(Constants.ERR_CHECK_LOGIN_FAILED, apiError.Message, apiError);
                Response.WriteJson(this.JsonForError(error));
                throw error;
            }
            return checkLoginResult.UserInfo;
        }

        private string GetHeader(string headerName)
        {
            var headerValue = Request.Headers[headerName];

            if (String.IsNullOrEmpty(headerValue))
            {
                var error = new ArgumentNullException(headerName, $"请求头不包含 {headerName}，请配合客户端 SDK 使用");
                Response.WriteJson(JsonForError(error));
                throw error;
            }

            return headerValue;
        }

        private object JsonForError(Exception error)
        {
            var json = PrepareResponseJsonDictionary();
            this.AppendError(json, error);
            return json;
        }

        private IDictionary<string, object> PrepareResponseJsonDictionary()
        {
            var dictionary = new Dictionary<string, object>();
            dictionary[Constants.WX_SESSION_MAGIC_ID] = 1;
            return dictionary;
        }

        private void AppendError(IDictionary<string, object> dictionary, Exception error)
        {
            if (error is LoginServiceException)
            {
                dictionary["error"] = (error as LoginServiceException).Type;
            }
            dictionary["message"] = error.Message;
        }
    }
}
