using Grpc.Core;
using KOMPAS;
using System.Threading.Tasks;

namespace gRPCLibrary.Services {

    public class KompasService : Kompas.KompasBase {

        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context) {
            return Task.FromResult(new HelloResponse { Message = "Привет, " + request.Name });
        }
    }
}
