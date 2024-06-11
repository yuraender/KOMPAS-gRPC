using Grpc.Core;
using KOMPAS;
using Microsoft.Extensions.Logging;

namespace gRPCServer.Services {

    public class GreeterService : Greeter.GreeterBase {

        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<GreeterService>();
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context) {
            _logger.LogInformation("HelloReply " + request.Name);
            return Task.FromResult(new HelloReply {
                Message = "Hello, " + request.Name
            });
        }
    }
}
