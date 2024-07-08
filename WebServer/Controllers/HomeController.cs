using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using WebServer.Pages;

namespace WebServer.Controllers {

    public class HomeController : Controller {

        public IActionResult Index() {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            if (Program.Cache.ContainsKey(ip)) {
                if (Program.Cache[ip].ContainsKey("IsDocumentOpen")) {
                    TempData["status"] = Program.Cache[ip]["IsDocumentOpen"];
                    Program.Cache[ip].Remove("IsDocumentOpen");
                }
                if (Program.Cache.ContainsKey(ip) && Program.Cache[ip].ContainsKey("SayHello")) {
                    TempData["response"] = Program.Cache[ip]["SayHello"];
                    Program.Cache[ip].Remove("SayHello");
                }
                if (Program.Cache.ContainsKey(ip) && Program.Cache[ip].ContainsKey("GetObjects")) {
                    TempData["response"] = Program.Cache[ip]["GetObjects"];
                    Program.Cache[ip].Remove("GetObjects");
                }
            } else {
                TempData["status"] = "Не запущен КОМПАС-3D";
            }
            return View();
        }

        [HttpPost]
        public IActionResult StartKompas() {
            return Redirect("kgrpc://start?server=me.yuraender.ru:5490");
        }

        [HttpPost]
        public IActionResult StopKompas() {
            return Redirect("kgrpc://stop");
        }

        [HttpPost]
        public async Task<IActionResult> SayHello(Data data) {
            if (string.IsNullOrEmpty(data.Text)) {
                return RedirectToAction("Index");
            }
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            if (Program.Sockets.ContainsKey(ip)) {
                var sayHello = Encoding.UTF8.GetBytes($"SayHello {data.Text}");
                await Program.Sockets[ip].SendAsync(
                    new ArraySegment<byte>(sayHello, 0, sayHello.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None
                );
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GetObjects() {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            if (Program.Sockets.ContainsKey(ip)) {
                var getObjects = Encoding.UTF8.GetBytes("GetObjects");
                await Program.Sockets[ip].SendAsync(
                    new ArraySegment<byte>(getObjects, 0, getObjects.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None
                );
            }
            return RedirectToAction("Index");
        }
    }
}
