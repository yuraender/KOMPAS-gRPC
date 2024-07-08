using Microsoft.AspNetCore.HttpOverrides;
using System.Net.WebSockets;
using System.Text;

namespace WebServer {

    public class Program {

        public static Dictionary<string, WebSocket> Sockets {
            get;
        } = new Dictionary<string, WebSocket>();
        public static Dictionary<string, Dictionary<string, string>> Cache {
            get;
        } = new Dictionary<string, Dictionary<string, string>>();

        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseRouting();
            app.UseWebSockets(new WebSocketOptions {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
            app.Use(async (context, next) => {
                if (context.Request.Path == "/ws") {
                    if (context.WebSockets.IsWebSocketRequest) {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var ip = context.Connection.RemoteIpAddress.ToString();
                        if (Cache.ContainsKey(ip)) {
                            webSocket.Abort();
                            return;
                        }
                        Sockets.Add(ip, webSocket);
                        Cache.Add(ip, new Dictionary<string, string>());
                        try {
                            await EchoWebSocket(context, webSocket);
                        } catch { }
                        Sockets.Remove(ip);
                        Cache.Remove(ip);
                    } else {
                        context.Response.StatusCode = 400;
                    }
                } else {
                    await next(context);
                }
            });
            await app.RunAsync();
        }

        private static async Task EchoWebSocket(HttpContext context, WebSocket webSocket) {
            var buffer = new byte[1024 * 4];
            var ip = context.Connection.RemoteIpAddress.ToString();

            WebSocketReceiveResult result = await webSocket
                .ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue) {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                if (message.StartsWith("IsDocumentOpen")) {
                    Cache[ip]["IsDocumentOpen"] = message.Substring(message.IndexOf(' ') + 1);
                }
                if (message.StartsWith("SayHello")) {
                    Cache[ip]["SayHello"] = message.Substring(message.IndexOf(' ') + 1);
                }
                if (message.StartsWith("GetObjects")) {
                    Cache[ip]["GetObjects"] = message.Substring(message.IndexOf(' ') + 1);
                }
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
