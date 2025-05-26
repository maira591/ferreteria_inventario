using Newtonsoft.Json;

namespace Ferreteria.Domain.ViewModel.Notification
{
    public class ResponseEsbNotificationVM
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "statusName")]
        public string StatusName { get; set; }

    }
}
