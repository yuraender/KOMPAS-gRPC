using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebServer.Pages {

    public class IndexModel : PageModel {

        private readonly ILogger<IndexModel>? _logger;

        public Data? Data {
            get; set;
        }

        public IndexModel(ILogger<IndexModel> logger) {
            _logger = logger;
        }
    }

    public class Data {
        public string? Text {
            get; set;
        }
    }
}
