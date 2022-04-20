# 利用.NET 6.0 开发Microsoft 365应用 - 控制台
> 作者：陈希章 2022-4-20

## 概述

这个范例代码是用来展示如何利用.NET 6.0 快速开发 Microsoft 365应用，我们将展示一个控制台应用，用来访问Microsoft Graph，并显示出来当前用户信息和最近十封邮件。

## 如何使用

有多种方式使用这个范例代码，例如

1. 你可以将其克隆到本地，然后通过 `dotnet run` 命令运行，也可以通过 `dotnet run 51ea05e5-3998-4e1d-b4dc-0090f422da20` 这样的语法运行，这里的参数指的是你自定义的 Azure AD application的ID，如果没有提供，则会使用我默认提供的一个。
1. 如果你本地没有安装 `dotnet` 的 SDK 和 runtime，你可以直接通过 `docker run --rm chenxizhang/dotnet-graph-console` 这样的方式运行，如果需要指定Azure AD application的ID，则按类似这样的语法 `docker run --rm chenxizhang/dotnet-graph-console 51ea05e5-3998-4e1d-b4dc-0090f422da20` 运行即可。
`

## 如何实现

这是一个标准的控制台程序，通过下面的命令

1. `dotnet new console -o graphconsole` 创建项目
1. `dotnet add package Microsoft.Identity.Client` 添加身份验证支持
1. `dotnet add package Microsoft.Graph` 添加对Microsoft Graph的支持

所有的代码就是如下这几行

```csharp
using Microsoft.Identity.Client;
using Microsoft.Graph;

// 如果运行时提供了参数，则读取该参数，否则使用默认的AAD application的ID
var clientId = args.Length > 0 ? args[0] : "fc341661-31f0-4a4f-8f18-0afb8a56a98a";
// 这里声明需要的权限
var scopes = new[] { "User.Read", "Mail.Read" };

// 创建一个客户端对象
var client = PublicClientApplicationBuilder.Create(clientId).WithRedirectUri("http://localhost").Build();
// 使用设备代码方式来获取访问凭据。这个可以在跨平台使用。
var token = await client.AcquireTokenWithDeviceCode(scopes, (result) =>
{
    Console.WriteLine($"{result.Message}");
    return Task.FromResult(0);
}).ExecuteAsync();

// 创建Microsoft Graph服务代理
var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((request) =>
{
    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
    return Task.FromResult(0);
}));

// 读取用户的个人信息
var me = await graphClient.Me.Request().GetAsync();
Console.WriteLine($"用户基本信息\n\t显示名称:{me.DisplayName}\n\t邮箱地址:{me.UserPrincipalName}\n");

// 读取用户前十封邮件
var messages = await graphClient.Me.Messages.Request().GetAsync();
Console.WriteLine($"用户邮件信息\n\t{string.Join('\t', messages.Select(x => x.Subject + '\n').ToArray())}");

```

## 更多参考

更多关于Microsoft Graph 的开发，可以参考 

1. 官方文档 https://docs.microsoft.com/zh-cn/graph/overview 
1. 《三十天学会Microsoft Graph》 https://aka.ms/30DaysMSGraph
1. 《解密和实战Microsoft Identity Platform》 https://identityplatform.xizhang.com 