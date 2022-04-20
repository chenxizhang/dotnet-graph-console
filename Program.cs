using Microsoft.Identity.Client;
using Microsoft.Graph;

var clientId = args.Length > 0 ? args[0] : "fc341661-31f0-4a4f-8f18-0afb8a56a98a";
var scopes = new[] { "User.Read", "Mail.Read" };

var client = PublicClientApplicationBuilder.Create(clientId).WithRedirectUri("http://localhost").Build();
var token = await client.AcquireTokenInteractive(scopes).ExecuteAsync();

var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((request) =>
{
    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
    return Task.FromResult(0);
}));

var me = await graphClient.Me.Request().GetAsync();
Console.WriteLine($"用户基本信息\n\t显示名称:{me.DisplayName}\n\t邮箱地址:{me.UserPrincipalName}\n");

var messages = await graphClient.Me.Messages.Request().GetAsync();
Console.WriteLine($"用户邮件信息\n\t{string.Join('\t', messages.Select(x => x.Subject + '\n').ToArray())}");
