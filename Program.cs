/*
概述：这个范例代码是用来展示如何利用.NET 6.0 快速开发 Microsoft 365应用，我们将展示一个控制台应用，用来访问Microsoft Graph，并显示出来当前用户信息和最近十封邮件。
作者：陈希章 ares@xizhang.com
参考：更多关于Microsoft Graph 的开发，可以参考 
    1. 官方文档 https://docs.microsoft.com/zh-cn/graph/overview 
    2. 《三十天学会Microsoft Graph》 https://aka.ms/30DaysMSGraph
    3. 《解密和实战Microsoft Identity Platform》 https://identityplatform.xizhang.com 
*/

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
