using Newtonsoft.Json;


namespace PracticeWeb.Common
{
    public class ApiErrorResponse
    {
        [JsonProperty("status")]

        public int StatusCode { get; set; }


        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// overridden to return json serialized object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
