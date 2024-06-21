using KOMPAS;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebServer.Pages;
using WebServer.Services;

namespace WebServer.Controllers {

    public class HomeController : Controller {

        static string exePath =
            @"..\gRPCServer\bin\Release\net8.0\gRPCServer.exe";

        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public IActionResult StartServer() {
            if (System.IO.File.Exists(exePath)) {
                Process.Start(new ProcessStartInfo {
                    FileName = exePath,
                    UseShellExecute = true,
                    CreateNoWindow = false
                });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult StopServer() {
            foreach (var process in Process.GetProcessesByName("gRPCServer")) {
                process.Kill();
                process.WaitForExit();
                process.Dispose();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SayHello(Data data) {
            var ip = HttpContext.Connection.RemoteIpAddress;
            if (ip == null || string.IsNullOrEmpty(data.Text)) {
                return RedirectToAction("Index");
            }
            KompasService.AddRequest(
                ip.ToString(),
                new Request {
                    Action = "SayHello",
                    Data = data.Text
                }
            );
            return RedirectToAction("Index");
        }
    }
}
