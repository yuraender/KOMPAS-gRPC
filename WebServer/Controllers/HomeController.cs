using Grpc.Core;
using Grpc.Net.Client;
using KOMPAS;
using Microsoft.AspNetCore.Mvc;
using WebServer.Pages;

namespace WebServer.Controllers {

    public class HomeController : Controller {

        public IActionResult Index() {
            var response = KompasService.GetCacheData(
                HttpContext.Connection.RemoteIpAddress.ToString(), "response");
            if (response != null) {
                TempData["response"] = response;
            }
            return View();
        }

        [HttpPost]
        public IActionResult StartKompas() {
            return Redirect("kgrpc://start");
        }

        [HttpPost]
        public IActionResult StopKompas() {
            return Redirect("kgrpc://stop");
        }

        [HttpPost]
        public IActionResult SayHello(Data data) {
            if (string.IsNullOrEmpty(data.Text)) {
                return RedirectToAction("Index");
            }
            using (var channel = GrpcChannel.ForAddress("http://127.0.0.1:5491")) {
                var client = new Kompas.KompasClient(channel);
                try {
                    var hello = client.SayHello(new HelloRequest { Name = data.Text });
                    if (hello != null) {
                        TempData["response"] = hello.Message;
                    }
                } catch (RpcException ex) {
                    TempData["response"] = ex.Status.Detail;
                }
            );
            return RedirectToAction("Index");
        }
    }
}
