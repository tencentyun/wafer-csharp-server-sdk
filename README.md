腾讯云微信小程序服务端 SDK - C#
=============================
[![build](https://ci.appveyor.com/api/projects/status/github/tencentyun/weapp-csharp-server-sdk?svg=true)](https://ci.appveyor.com/project/techird/weapp-csharp-server-sdk)
[![license](https://img.shields.io/github/license/tencentyun/weapp-java-server-sdk.svg)](LICENSE)

本 SDK 是[腾讯云微信小程序一站式解决方案][weapp-solution]（下文简称「解决方案」）的组成部分。业务服务器可通过本 SDK 为小程序客户端提供云端服务支持，包括：

1. 登录态鉴权服务
2. 信道服务

## SDK 获取

本项目遵守 [MIT](LICENSE) 协议，可以直接[下载 SDK 源码][sdk-download]进行修改、编译和发布。

> 如果从[腾讯云微信小程序控制台][la-console]购买解决方案并选择 C# 语言，则分配的业务服务器里已经部署了本 SDK 和 Demo 的发行版本。

## API

请参考项目 [Wiki][api-url]。

## 使用示例（ASP.NET MVC）

获得 SDK 源码后，需要在解决方案中添加 SDK 项目。创建 ASP.NET MVC 网站项目，并且添加对 SDK 项目的引用。

![添加引用](https://cloud.githubusercontent.com/assets/1901286/19793069/6af728f2-9cfd-11e6-928b-486f92c3db75.png)

![添加 SDK 引用](https://cloud.githubusercontent.com/assets/1901286/19793099/a994cdf8-9cfd-11e6-8174-399763714e1e.png)

添加引用后，就可以使用 SDK 提供的服务。

### 配置 SDK

SDK 必须经过初始化配置之后才能使用。可以选择使用代码初始化或者使用配置文件初始化。初始化配置建议在 `Application_Start` 里进行。

使用代码初始化：

```cs
using QCloud.WeApp.SDK;

var configuration = new Configuration() {
    // 业务服务器访问域名
    ServerHost = "199447.qcloud.la",
    // 鉴权服务地址
    AuthServerUrl = "http://10.0.12.135/mina_auth/",
    // 信道服务地址
    TunnelServerUrl = "https://ws.qcloud.com/",
    // 信道服务签名 key
    TunnelSignatureKey = "my$ecretkey",
    // 网络请求超时设置，单位为豪秒
    NetworkTimeout = 30000
};
ConfigurationManager.Setup(configuration);
```

使用配置文件初始化：

```cs
using QCloud.WeApp.SDK;

var configFilePath = "C:\\qcloud\sdk.config";
ConfigurationManager.SetupFromFile(configFilePath);
```

关于 SDK 配置字段的含义以及配置文件格式的更多信息，请参考[服务端 SDK 配置][sdk-config-wiki]。

### 使用 SDK 提供登录服务

#### 登录

业务服务器提供一个路由处理客户端的登录请求，直接把该请求交给 SDK 来处理即可完成登录。登录成功后，可以获取用户信息。

```cs
using QCloud.WeApp.SDK.Authorization;

public class LoginController : Controller
{
    /// GET /login
    /// 实现登录接口，配合客户端 SDK 建立会话
    public ActionResult Index()
    {
        try
        {
            // 使用 Request 和 Response 初始化登录服务
            LoginService loginService = new LoginService(Request, Response);

            // 调用登录接口，如果成功可以获得用户信息。如有需要，可以使用用户信息进行进一步的业务操作
            UserInfo userInfo = loginService.Login();

            Debug.WriteLine(userInfo);
        }
        catch (LoginServiceException ex)
        {
            // 登录失败会抛出登录异常
            Debug.WriteLine(ex);
        }

        // 登录无论成功还是失败，都无需返回响应结果，因为登录服务已经使用 HTTP Response 进行输出
        return null;
    }
}
```

> 如果登录失败，[Login()][login-api] 方法会抛出异常，需要使用 try-catch 来捕获异常。该异常可以不用处理，抛出来是为了方便业务服务器可以进行记录和监控。

#### 获取会话状态

客户端交给业务服务器的请求，业务服务器可以通过 SDK 来检查该请求是否包含合法的微信小程序会话。如果包含，则会返回会话对应的用户信息。

```cs
using QCloud.WeApp.SDK.Authorization;

/// GET /user
/// 利用建立的会话获取用户信息
public class UserController : Controller
{
    // GET: User
    public ActionResult Index()
    {
        try
        {
            // 使用 Request 和 Response 初始化登录服务
            LoginService loginService = new LoginService(Request, Response);

            // 调用检查登录接口，成功后可以获得用户信息，进行正常的业务请求
            UserInfo userInfo = loginService.Check();

            Response.AddHeader("Content-Type", "application/json");
            // 获取会话成功，需要返回 HTTP 视图，这里作为示例返回了获得的用户信息
            return Content(JsonConvert.SerializeObject(new
            {
                code = 0,
                message = "OK",
                data = new { userInfo }
            }));
        }
        catch (Exception error)
        {
            // 可以处理登录失败的情况，但是注意此时无需返回 ActionResult，
            // 因为登录失败的时候，登录服务已经输出登录失败的响应
            Debug.WriteLine(error);
            return null;
        }
    }
}
```

> 如果检查会话失败，或者会话无效，[Check()][check-api] 方法会抛出异常，需要使用 try-catch 来捕获异常。该异常可以不用处理，抛出来是为了方便业务服务器可以进行记录和监控。


阅读解决方案文档中的[鉴权服务][auth-service-wiki]了解更多解决方案中关于鉴权服务的技术资料。

### 使用 SDK 提供信道服务

业务在一个路由上提供信道服务，只需把该路由上的请求都交给 SDK 的信道服务处理即可。

```cs
public class TunnelController : Controller
{

    private TunnelService tunnelService = null;

    /// GET /tunnel
    /// 请求建立隧道连接
    /// 
    /// POST /tunnel
    /// 用于信道服务器推送消息到业务服务器
    public ActionResult Index()
    {

        // 创建信道服务处理信道相关请求
        tunnelService = new TunnelService(Request, Response);

        // 信道服务会自动响应请求，请不要使用 Response 进行二次请求
        tunnelService.Handle(

            // 需要实现信道处理器，ChatTunnelHandler 是一个实现的范例
            handler: new ChatTunnelHandler(), 

            // 配置是可选的，配置 CheckLogin 为 true 的话，会在隧道建立之前获取用户信息，以便业务将隧道和用户关联起来
            options: new TunnelHandleOptions() { CheckLogin = true }
        );

        // 返回 null 确保 MVC 框架不进行输出
        return null;
    }
}
```

使用信道服务需要实现处理器，来获取处理信道的各种事件，具体可参考接口 [ITunnelHandler][tunnel-handler-api] 的 API 文档以及配套 Demo 中的 [ChatTunnelHandler][chat-handler-source] 的实现。

阅读解决方案文档中的[信道服务][tunnel-service-wiki]了解更多解决方案中关于鉴权服务的技术资料。

## 反馈和贡献

如有问题，欢迎使用 [Issues][new-issue] 提出，也欢迎广大开发者给我们提 [Pull Request][pr]。

## LICENSE

[MIT](LICENSE)

[weapp-solution]: https://github.com/tencentyun/weapp-solution "查看腾讯云微信小程序解决方案"
[sdk-download]: https://github.com/tencentyun/weapp-csharp-server-sdk/archive/master.zip "下载 C# SDK 源码"
[la-console]: https://console.qcloud.com/la "打开腾讯云微信小程序一站式解决方案控制台"
[api-url]: https://github.com/tencentyun/weapp-csharp-server-sdk/wiki "查看 C# SDK API 文档"
[sdk-config-wiki]: https://github.com/tencentyun/weapp-solution/wiki/%E6%9C%8D%E5%8A%A1%E7%AB%AF-SDK-%E9%85%8D%E7%BD%AE "查看服务端 SDK 配置"
[auth-service-wiki]: https://github.com/tencentyun/weapp-solution/wiki/%E9%89%B4%E6%9D%83%E6%9C%8D%E5%8A%A1 "查看关于鉴权服务的更多资料"
[tunnel-service-wiki]: https://github.com/tencentyun/weapp-solution/wiki/%E9%89%B4%E6%9D%83%E6%9C%8D%E5%8A%A1 "查看关于信道服务的更多资料"
[login-api]: https://github.com/tencentyun/weapp-csharp-server-sdk/wiki/M_QCloud_WeApp_SDK_Authorization_LoginService_Login "查看 LoginService.Login() 方法 API 文档"
[check-api]: https://github.com/tencentyun/weapp-csharp-server-sdk/wiki/M_QCloud_WeApp_SDK_Authorization_LoginService_Check "查看 LoginService.Check() 方法 API 文档"
[tunnel-handler-api]: https://github.com/tencentyun/weapp-csharp-server-sdk/wiki/T_QCloud_WeApp_SDK_Tunnel_ITunnelHandler "查看 ITunnelHandler 接口 API 文档"
[chat-handler-source]: https://github.com/tencentyun/weapp-csharp-server-sdk/blob/master/QCloud.WeApp.Demo.MVC/Business/ChatTunnelHandler.cs "查看 ChatTunnelHandler 示例代码"
[new-issue]: https://github.com/CFETeam/qcloud-weapp-server-sdk-csharp/issues/new "反馈建议和问题"
[pr]: https://github.com/CFETeam/qcloud-weapp-server-sdk-csharp/pulls "创建 Pull Request"
