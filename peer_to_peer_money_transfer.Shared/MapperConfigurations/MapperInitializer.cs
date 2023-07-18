using AutoMapper;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.Shared.DataTransferObject;

namespace peer_to_peer_money_transfer.Shared.MapperConfigurations
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<ApplicationUser, RegisterAdminDTO>().ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

            CreateMap<ApplicationUser, RegisterIndividualDTO>().ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

            CreateMap<ApplicationUser, RegisterBusinessDTO>().ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

            CreateMap<ApplicationUser, GetCharacterDTO>().ReverseMap();
        }
    }
}
