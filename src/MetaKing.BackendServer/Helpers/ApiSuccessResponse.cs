using Newtonsoft.Json;

namespace MetaKing.BackendServer.Helpers
{
    public class ApiSuccessResponse : ApiResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; }

        public ApiSuccessResponse(object data, string message = "Request succeeded")
            : base(200, message)
        {
            Data = data;
        }

        public ApiSuccessResponse(string message = "Request succeeded")
            : base(200, message)
        {
        }
    }
}
