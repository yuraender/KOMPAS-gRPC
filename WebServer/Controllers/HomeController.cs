using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using KOMPAS;
using Grpc.Core;
using WebServer.Pages;

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
        public IActionResult SendTestRequest(Data data) {
            using (var channel = GrpcChannel.ForAddress("http://localhost:5490")) {
                var client = new Greeter.GreeterClient(channel);
                try {
                    var hello = client.SayHello(new HelloRequest { Name = data.Text });
                    if (hello != null) {
                        TempData["answer"] = hello.Message;
                    }
                } catch (RpcException ex) {
                    TempData["answer"] = ex.Status.Detail;
                }
            }
            return RedirectToAction("Index");
        }
    }
}
