using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;

namespace QCloud.WeApp.SDK.Authorization
{
    /// <summary>
    /// 登录服务，提供登录和检查登录的接口
    /// </summary>
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
            Request = request;
            Response = response;
        }

        /// <summary>
        /// 配合腾讯云鉴权服务器提供微信小程序登录态服务，需要提供 HTTP 的请求和响应对象进行初始化
        /// </summary>
        public LoginService(HttpRequest request, HttpResponse response) : this(new HttpRequestWrapper(request), new HttpResponseWrapper(response)) { }

        /// <summary>
        /// 请求登陆，获得用户信息。<para />
        /// 无论登录成功与否，该方法都会直接进行 HTTP 响应登录结果，使用该方法后无需再进行 HTTP 响应。<para />
        /// </summary>
        /// <returns>登录成功后获得用户信息</returns>
        /// <exception cref="LoginServiceException">
        /// 如果登录失败，将会抛出异常。
        /// </exception>
        public UserInfo Login()
        {
            var code = GetHeader(Constants.WX_HEADER_CODE);
            var encryptedData = GetHeader(Constants.WX_HEADER_ENCRYPTED_DATA);
            var iv = GetHeader(Constants.WX_HEADER_IV);

            LoginResult loginResult = null;

            try
            {
                var api = new AuthorizationAPI();
                loginResult = api.Login(code, encryptedData, iv);
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
        /// 检查当前请求是否包含已登录的会话
        /// </summary>
        /// <returns>如果已登录，返回当前用户信息</returns>
        /// <exception cref="LoginServiceException">
        /// 如果检查登录失败，或者当前用户登录态不正确，将会抛出异常
        /// </exception>
        public UserInfo Check()
        {
            string id = GetHeader(Constants.WX_HEADER_ID);
            string skey = GetHeader(Constants.WX_HEADER_SKEY);

            CheckLoginResult checkLoginResult = null;

            try
            {
                var api = new AuthorizationAPI();
                checkLoginResult = api.CheckLogin(id, skey);
            }
            catch (Exception apiError)
            {
                LoginServiceException error = null;
                if (apiError is AuthorizationAPIException)
                {
                    AuthorizationAPIException authError = (AuthorizationAPIException)apiError;
                    if (authError.Code == 60011 || authError.Code == 60012)
                    {
                        error = new LoginServiceException(Constants.ERR_INVALID_SESSION, authError.Message, authError);
                    }
                }
                if (error == null)
                {
                    error = new LoginServiceException(Constants.ERR_CHECK_LOGIN_FAILED, apiError.Message, apiError);
                }
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
                var error = new LoginServiceException(Constants.ERR_INVALID_REQUEST, $"请求头不包含 {headerName}，请配合客户端 SDK 登陆后再进行请求");
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
