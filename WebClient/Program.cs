using WebClient.Clients;

namespace WebClient {

    public class Program {

        public static async Task Main(string[] args) {
            try {
                var grpcClient = new GrpcClient("http://127.0.0.1:5491");
                var webSocketClient = new WebSocketClient(
                    $"ws://{Environment.GetEnvironmentVariable("KOMPAS_WebServer")}/ws",
                    grpcClient
                );
                await webSocketClient.StartAsync();
                Console.ReadLine();
                await webSocketClient.StopAsync();
            } catch (Exception ex){
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
