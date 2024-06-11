using Grpc.Net.Client;
using KOMPAS;
using Microsoft.Extensions.Logging;

namespace gRPCClient {

    public class Program {

        public static ILogger<Program>? Logger {
            get; private set;
        }
        const int port = 5490;

        public static void Main() {
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
            });
            Logger = loggerFactory.CreateLogger<Program>();

            using (var channel = GrpcChannel.ForAddress("http://localhost:" + port)) {
                var client = new Greeter.GreeterClient(channel);

                var name = Console.ReadLine();
                var answer = client.SayHello(new HelloRequest { Name = name });
                Logger.LogInformation("Ответ сервера: {Answer}", answer.Message);

                Console.ReadKey();
            }
        }
    }
}
