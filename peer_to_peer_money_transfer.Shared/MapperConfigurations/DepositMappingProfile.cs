using AutoMapper;
using PayStack.Net;
using peer_to_peer_money_transfer.DAL.Dtos.Requests;

namespace peer_to_peer_money_transfer.DAL.Configuration
{
    public class DepositMappingProfile : Profile
    {
        public DepositMappingProfile()
        {
            CreateMap<DepositRequest, TransactionInitializeRequest>()
                .ForSourceMember(src => src.CurrentUserId, act => act.DoNotValidate());
        }
    }
}
