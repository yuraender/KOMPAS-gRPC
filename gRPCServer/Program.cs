﻿using Grpc.Core;
using gRPCServer.Services;
using KOMPAS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace gRPCServer {

    public class Program {

        public static ILogger<Program>? Logger {
            get; private set;
        }
        const int port = 5490;

        public static async Task Main(string[] args) {
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
            });
            Logger = loggerFactory.CreateLogger<Program>();

            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureServices((hostContext, services) => {
                Server server = new Server {
                    Services = { Greeter.BindService(new GreeterService(loggerFactory)) },
                    Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
                };
                server.Start();
                services.AddSingleton(server);
                Logger.LogInformation("Server listening on port {Port}", port);
            });
            await hostBuilder.RunConsoleAsync();
        }
    }
}