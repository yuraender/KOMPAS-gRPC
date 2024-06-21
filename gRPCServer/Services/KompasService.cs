using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KOMPAS;
using Microsoft.Extensions.Logging;

namespace gRPCServer.Services {

    public class KompasService : Kompas.KompasBase {

        private readonly ILogger<KompasService> _logger;

        public KompasService(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<KompasService>();
        }

        public override async Task SendRequest(
            Empty request,
            IServerStreamWriter<Request> responseStream,
            ServerCallContext context
        ) {
            string peer = context.Peer;
            string ip = peer.Substring(peer.IndexOf(':') + 1);
            _logger.LogInformation("Client with IP {IP} connected.", ip);

            await responseStream.WriteAsync(new Request());
            while (!context.CancellationToken.IsCancellationRequested) {
                //Request request;
                //_logger.LogInformation("Request {IP} | {Action} | Data", ip, request.Action, request.Data);
                //await responseStream.WriteAsync(request);
            }

            _logger.LogInformation("Client with IP {IP} disconnected.", ip);
        }

        public override Task<Empty> SayHello(Hello request, ServerCallContext context) {
            _logger.LogInformation("SayHello {Message}", request.Message);
            return Task.FromResult(new Empty());
        }
    }
}
