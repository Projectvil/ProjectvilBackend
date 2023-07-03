using AutoMapper;
using WebGateway.ViewModels.Auth;

namespace WebGateway;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        return new MapperConfiguration(config =>
        {
            config.CreateMap<LoginRequest, LoginViewModel>().ReverseMap();
            config.CreateMap<SignUpRequest, SignUpViewModel>().ForMember(nameof(SignUpRequest.UserName),
                opt => opt
                    .MapFrom(model =>
                        model.UserName)).ReverseMap();


        });
    }
}