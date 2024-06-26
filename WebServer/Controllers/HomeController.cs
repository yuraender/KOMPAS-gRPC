using KOMPAS;
using Microsoft.AspNetCore.Mvc;
using WebServer.Pages;
using WebServer.Services;

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
            return Redirect("kgrpc://start?server=me.yuraender.ru:5490");
        }

        [HttpPost]
        public IActionResult StopKompas() {
            return Redirect("kgrpc://stop");
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
