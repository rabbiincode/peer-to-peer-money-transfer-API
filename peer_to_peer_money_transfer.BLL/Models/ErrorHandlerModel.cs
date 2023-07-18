using Newtonsoft.Json;

namespace peer_to_peer_money_transfer.BLL.Models
{
    public class ErrorHandlerModel
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
