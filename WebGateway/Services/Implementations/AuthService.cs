using System.Threading.Channels;
using AutoMapper;
using WebGateway.GRPCInteraction;
using WebGateway.Services.Interfaces;
using WebGateway.ViewModels.Auth;

namespace WebGateway.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IChannelsService _channelsService;
        private readonly IMapper _mapper;

        public AuthService(IChannelsService channelsService, IMapper mapper)
        {
            _channelsService = channelsService;
            _mapper = mapper;
        }
        public async Task<TokenResponse> Login(LoginViewModel loginModel)
        {
            using var chanel = _channelsService.GetChanel(MicroservicesTypes.Auth);
            var client = new AuthGrpcService.AuthGrpcServiceClient(chanel);

            var tokenResult = await client.LoginAsync(_mapper.Map<LoginRequest>(loginModel));

            return tokenResult;
        }

        public async Task<TokenResponse> SignUp(SignUpViewModel signUpViewModel)
        {
            using var chanel = _channelsService.GetChanel(MicroservicesTypes.Auth);
            var client = new AuthGrpcService.AuthGrpcServiceClient(chanel);

            var tokenResult = await client.SignUpAsync(_mapper.Map<SignUpRequest>(signUpViewModel));

            return tokenResult;
        }
    }
}
