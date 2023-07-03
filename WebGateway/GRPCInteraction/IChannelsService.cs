using Grpc.Net.Client;

namespace WebGateway.GRPCInteraction
{
    public interface IChannelsService
    {
        public GrpcChannel GetChanel(MicroservicesTypes type);
    }
}
