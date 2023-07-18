using peer_to_peer_money_transfer.Shared.SmsConfiguration;

namespace peer_to_peer_money_transfer.Shared.Interfaces
{
    public interface ISendSms
    {
        Task<string> SendSmsAsync(SmsModel model);
    }
}
