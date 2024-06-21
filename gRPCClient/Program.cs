using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcWebSocketBridge.Client;
using KOMPAS;
using Microsoft.Extensions.Logging;

namespace gRPCClient {

    public class Program {

        public static ILogger<Program>? Logger {
            get; private set;
        }
        const string host = "http://127.0.0.1";
        const int port = 5490;

        public static async Task Main() {
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
            });
            Logger = loggerFactory.CreateLogger<Program>();

            using (var channel = GrpcChannel.ForAddress($"{host}:{port}", new GrpcChannelOptions() {
                HttpHandler = new GrpcWebSocketBridgeHandler()
            })) {
                var client = new Kompas.KompasClient(channel);

                using (var call = client.SendRequest(new Empty())) {
                    var responseStream = call.ResponseStream;
                    await responseStream.MoveNext();
                    while (await responseStream.MoveNext()) {
                        var request = responseStream.Current;
                        Logger.LogInformation("Запрос {Action} | {Data}", request.Action, request.Data);
                        RunAction(client, request);
                    }
                }

                Console.ReadKey();
            }
        }

        private static void RunAction(Kompas.KompasClient client, Request request) {
            if (request.Action == null || request.Data == null) {
                return;
            }
            switch (request.Action) {
                case "SayHello":
                    client.SayHello(new Hello { Message = "Привет, " + request.Data });
                    break;
            }
        }
    }
}
