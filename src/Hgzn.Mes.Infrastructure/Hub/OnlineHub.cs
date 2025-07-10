using System.Net;
using System.Security.Claims;
using Hgzn.Mes.Domain.Entities.Hub;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Monitor;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using IPTools.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using UAParser;
using LoginLog = Hgzn.Mes.Domain.Entities.Audit.LoginLog;
namespace Hgzn.Mes.Infrastructure.Hub;


[Authorize]
public class OnlineHub : Microsoft.AspNetCore.SignalR.Hub
{
    private static readonly List<OnlineUser> OnlineUsers = new();
    private static readonly object ObjLock = new object();
    private readonly ILogger<OnlineHub> _logger;
    private readonly HubConnectionContext _context;
  

    public static List<OnlineUser> OnlineUserInfos
    {
        get
        {
            lock (ObjLock) // 如果需要线程安全，可以加锁
            {
                return OnlineUsers;
            }
        }
    }

    public OnlineHub(ILogger<OnlineHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        lock (ObjLock)
        {
            var ss = this.Context.User.Identity.Name;
            var name = Context.User.Identity?.Name;
            var userId = Context.User.FindUserId();
            var loginUser = GetInfoByHttpContext(Context.GetHttpContext());
            var user = new OnlineUser
            {
                Browser = loginUser.Browser,
                    LoginLocation = loginUser.LoginLocation,
                Ipaddr = loginUser.LoginIp,
                LoginTime = DateTime.Now.ToLocalTime(),
                Os = loginUser.Os,
                UserName = name ?? "Null",
                UserId = userId
            };
            //已登录
            if (Context.UserIdentifier != null)
            {
                //先移除之前的用户id，一个用户只能一个
                OnlineUsers.RemoveAll(u => u.UserId == userId);
                _logger.LogInformation(
                    $"{DateTime.Now.ToLocalTime()}：{name},{Context.ConnectionId}连接服务端success，当前已连接{OnlineUsers.Count}个");
            }

            //全部移除之后，再进行添加
            OnlineUsers.RemoveAll(u => u.ConnnectionId == Context.ConnectionId);

            OnlineUsers.Add(user);
            //当有人加入，向全部客户端发送当前总数
            Clients.All.SendAsync("onlineNum", OnlineUsers.Count);
        }

        return base.OnConnectedAsync();
    }

    public static void RemoveByUserId(Guid userId) {
         OnlineUsers.RemoveAll(u => u.UserId == userId);
      //  _logger.LogInformation($"{DateTime.Now.ToLocalTime()}：{userId},{Context.ConnectionId}已经删除！");
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        lock (ObjLock)
        {
            var userId = Context.User.FindUserId();
            if (Context.UserIdentifier != null)
            {
                OnlineUsers.RemoveAll(u => u.UserId == userId);
                _logger.LogInformation($"用户{Context.User?.Identity?.Name}离开了，当前已连接{OnlineUsers.Count}个");
            }

            OnlineUsers.RemoveAll(u => u.ConnnectionId == Context.ConnectionId);
            Clients.All.SendAsync("onlineNum", OnlineUsers.Count);
        }

        return base.OnDisconnectedAsync(exception);
    }

    private LoginLog GetInfoByHttpContext(HttpContext? httpContext)
    {
        ClientInfo GetClientInfo(HttpContext? context)
        {
            var str = context.GetUserAgent();
            var uaParser = Parser.GetDefault();
            ClientInfo c;
            try
            {
                c = uaParser.Parse(str);
            }
            catch
            {
                c = new ClientInfo("null", new OS("null", "null", "null", "null", "null"),
                    new Device("null", "null", "null"), new UserAgent("null", "null", "null", "null"));
            }

            return c;
        }

        var ipAddr = httpContext.GetClientIp();
        var location = ipAddr == "127.0.0.1" ? new IpInfo() { Province = "本地", City = "本机" } : IpTool.Search(ipAddr);
        var clientInfo = GetClientInfo(httpContext);
        LoginLog entity = new()
        {
            Browser = clientInfo.Device.Family,
            Os = clientInfo.OS.ToString(),
            LoginIp = ipAddr,
            LoginLocation = location.Province + "-" + location.City
        };
        return entity;
    }
}