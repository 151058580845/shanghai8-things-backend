using Hgzn.Mes.Domain.Entities.Hub;
using Hgzn.Mes.Domain.Entities.System.Log;
using Hgzn.Mes.Domain.Shared.Exceptions;
using IPTools.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using UAParser;

namespace Hgzn.Mes.Infrastructure.Hub;

public class OnlineHub:Microsoft.AspNetCore.SignalR.Hub
{
    public static readonly List<OnlineUser> OnlineUsers = new();
    public static readonly object objLock = new object();
    private readonly HttpContext? _httpContextAccessor;
    private readonly ILogger<OnlineHub> _logger;

    public OnlineHub(IHttpContextAccessor httpContextAccessor, ILogger<OnlineHub> hubContext)
    {
        _httpContextAccessor = httpContextAccessor.HttpContext;
        _logger = hubContext;
    }

    public override Task OnConnectedAsync()
    {
        lock (objLock)
        {
            var name = Context.User.Identity.Name;
            var userId = Context.User.Claims.FirstOrDefault(x => x.Type == "id").Value;
            var loginUser = GetInfoByHttpContext(_httpContextAccessor);
            var user = new OnlineUser
            {
                Browser = loginUser.Browser,
                LoginLocation = loginUser?.LoginLocation,
                Ipaddr = loginUser?.LoginIp,
                LoginTime = DateTime.Now,
                Os = loginUser?.Os,
                UserName = name ?? "Null",
                UserId = Guid.Parse(userId)
            };
            //已登录
            if (Context.UserIdentifier != null)
            {
                //先移除之前的用户id，一个用户只能一个
                OnlineUsers.RemoveAll(u => u.UserId == Guid.Parse(userId));
                _logger.LogInformation($"{DateTime.Now}：{name},{Context.ConnectionId}连接服务端success，当前已连接{OnlineUsers.Count}个");
            }
            //全部移除之后，再进行添加
            OnlineUsers.RemoveAll(u => u.ConnnectionId == Context.ConnectionId);

            OnlineUsers.Add(user);
            //当有人加入，向全部客户端发送当前总数
            Clients.All.SendAsync("onlineNum", OnlineUsers.Count);
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        lock (objLock)
        {
            var userId = Context.User.Claims.FirstOrDefault(x => x.Type == "id").Value;
            if (Context.UserIdentifier != null)
            {
                OnlineUsers.RemoveAll(u => u.UserId == Guid.Parse(userId));
                _logger.LogInformation($"用户{Context.User?.Identity?.Name}离开了，当前已连接{OnlineUsers.Count}个");
            }
            OnlineUsers.RemoveAll(u => u.ConnnectionId == Context.ConnectionId);
            Clients.All.SendAsync("onlineNum", OnlineUsers.Count);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public LoginLog GetInfoByHttpContext(HttpContext? context)
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
                c = new ClientInfo("null",new OS("null", "null", "null", "null", "null"),new Device("null","null","null"), new UserAgent("null", "null", "null", "null"));
            }
            return c;
        }
        var ipAddr = context.GetClientIp();
        IpInfo location;
        if (ipAddr == "127.0.0.1")
        {
            location = new IpInfo() { Province = "本地", City = "本机" };
        }
        else
        {
            location = IpTool.Search(ipAddr);
        }
        ClientInfo clientInfo = GetClientInfo(context);
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