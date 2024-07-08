using System.Net.WebSockets;
using System.Text;

namespace WebClient.Clients {

    public class WebSocketClient {

        private readonly Uri _uri;
        private ClientWebSocket _client;
        private readonly GrpcClient _grpcClient;

        public WebSocketClient(string uri, GrpcClient grpcClient) {
            _uri = new Uri(uri);
            _grpcClient = grpcClient;
            _client = new ClientWebSocket();
        }

        public async Task StartAsync() {
            await _client.ConnectAsync(_uri, CancellationToken.None);
            Console.WriteLine("Connected to WebSocket server");
            await Runner();
        }

        private async Task Runner() {
            var buffer = new byte[1024 * 4];
            Task.Run(async () => {
                while (_client.State == WebSocketState.Open) {
                    var request = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, request.Count);
                    Console.WriteLine(message);
                    if (message.StartsWith("SayHello")) {
                        var name = message.Substring(message.IndexOf(' ') + 1);
                        var sayHello = Encoding.UTF8.GetBytes($"SayHello {_grpcClient.SayHello(name)}");
                        await _client.SendAsync(
                            new ArraySegment<byte>(sayHello, 0, sayHello.Length),
                            WebSocketMessageType.Text, true, CancellationToken.None
                        );
                    }
                    if (message.StartsWith("GetObjects")) {
                        var getObjects = Encoding.UTF8.GetBytes($"GetObjects {_grpcClient.GetObjects()}");
                        await _client.SendAsync(
                            new ArraySegment<byte>(getObjects, 0, getObjects.Length),
                            WebSocketMessageType.Text, true, CancellationToken.None
                        );
                    }
                }
            });
            while (_client.State == WebSocketState.Open) {
                var isDocumentOpen = Encoding.UTF8.GetBytes($"IsDocumentOpen {_grpcClient.IsDocumentOpen()}");
                await _client.SendAsync(
                    new ArraySegment<byte>(isDocumentOpen, 0, isDocumentOpen.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None
                );
            }
        }

        public async Task StopAsync() {
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            Console.WriteLine("WebSocket connection closed");
        }
    }
}
