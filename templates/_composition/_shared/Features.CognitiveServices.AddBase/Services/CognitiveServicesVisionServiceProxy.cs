using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using  Param_RootNamespace.Models;

using Newtonsoft.Json.Linq;
using System.Linq;

namespace  Param_RootNamespace.Services
{
    public class CognitiveServicesVisionServiceProxy<T> : ICognitiveServicesVisionServiceProxy<T>
    {
        readonly HttpClient client;     
        string Uri;

        public CognitiveServicesVisionServiceProxy(string baseServiceUrl, string apiKey, string requestParameters)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            Uri = baseServiceUrl + "?" + requestParameters;
        }

        public async Task<T> GetImageResponseAsync(byte[] imageArray)
        {
            HttpResponseMessage response;

            T imageResponse;

            using (ByteArrayContent content = new ByteArrayContent(imageArray))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(Uri, content);

                // Get the JSON response.
                string jsonString = await response.Content.ReadAsStringAsync();
                imageResponse = Deserialize(jsonString, token => token["value"]);
               
            }
            return imageResponse;
        }

        private T Deserialize(string json, Func<JToken, JToken> filter)
        {
            if (string.IsNullOrEmpty(json)) return default(T);

            try
            {
                var token = JToken.Parse(json);
                token = filter?.Invoke(token) ?? token;
                var obj = token.ToObject<T>();

                return obj;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
