using Grpc.Net.Client;

namespace WebGateway.GRPCInteraction
{
    public class ChannelsService : IChannelsService
    {
        private readonly IConfiguration _configuration;
        public ChannelsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GrpcChannel GetChanel(MicroservicesTypes type)
        {
            return type switch
            {
                MicroservicesTypes.Auth => GrpcChannel.ForAddress(_configuration["Microservices:Auth"]),
                _ => throw new ArgumentException("Unsupported MicroservicesType")
            };
        }
    }

}
