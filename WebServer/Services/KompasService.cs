using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KOMPAS;

namespace WebServer.Services {

    public class KompasService : Kompas.KompasBase {

        public static Dictionary<string, Queue<Request>> ClientRequestQueue {
            get; private set;
        } = new Dictionary<string, Queue<Request>>();
        public static Dictionary<string, Dictionary<string, object?>?> Cache {
            get; private set;
        } = new Dictionary<string, Dictionary<string, object?>?>();

        private readonly ILogger<KompasService> _logger;

        public KompasService(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<KompasService>();
        }

        public override async Task SendRequest(
            Empty _,
            IServerStreamWriter<Request> responseStream,
            ServerCallContext context
        ) {
            string ip = context.Peer.Split(':')[1];
            if (ClientRequestQueue.ContainsKey(ip) || Cache.ContainsKey(ip)) {
                return;
            }
            ClientRequestQueue.Add(ip, new Queue<Request>());
            Cache.Add(ip, new Dictionary<string, object?>());
            _logger.LogInformation("Client with IP {IP} connected.", ip);

            await responseStream.WriteAsync(new Request());
            while (!context.CancellationToken.IsCancellationRequested) {
                if (!ClientRequestQueue[ip].TryDequeue(out var request)) {
                    continue;
                }
                _logger.LogInformation("Request {IP} | {Action}", ip, request.Action);
                await responseStream.WriteAsync(request);
            }

            ClientRequestQueue.Remove(ip);
            Cache.Remove(ip);
            _logger.LogInformation("Client with IP {IP} disconnected.", ip);
        }

        public override Task<Empty> SayHello(Hello request, ServerCallContext context) {
            _logger.LogInformation("SayHello {Message}", request.Message);
            SetCacheData(context.Peer.Split(':')[1], "response", request.Message);
            return Task.FromResult(new Empty());
        }

        public static void AddRequest(string ip, Request request) {
            if (!ClientRequestQueue.ContainsKey(ip)) {
                return;
            }
            ClientRequestQueue[ip].Enqueue(request);
        }

        public static object? GetCacheData(string ip, string data) {
            if (!Cache.ContainsKey(ip) || !Cache[ip]!.ContainsKey(data)) {
                return null;
            }
            return Cache[ip]![data];
        }

        public static void SetCacheData(string ip, string data, object? value) {
            if (!Cache.ContainsKey(ip)) {
                return;
            }
            Cache[ip]![data] = value;
        }
    }
}
