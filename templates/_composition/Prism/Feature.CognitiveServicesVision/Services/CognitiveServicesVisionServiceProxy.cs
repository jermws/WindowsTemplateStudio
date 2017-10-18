using Param_RootNamespace.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Param_RootNamespace.Services
{
    public class CognitiveServicesVisionServiceProxy : ICognitiveServicesVisionServiceProxy
    {
        readonly HttpClient client;
        string BaseServiceUrl = @"https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze";
        readonly string apiKey = "40769889807c4774a375787fcebf1907";
        string Uri;
        //readonly int count = 10;
        //readonly int searchOffset = 0;
        //readonly string safeSearch = "Moderate";
        public CognitiveServicesVisionServiceProxy()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            string requestParameters = "visualFeatures=Categories,Description,Color&language=en";
            Uri = BaseServiceUrl + "?" + requestParameters;
        }

        public async Task<ImageResponse> GetImageResponse(byte[] imageArray)
        {
            HttpResponseMessage response;

            ImageResponse imageResponse;

            using (ByteArrayContent content = new ByteArrayContent(imageArray))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(Uri, content);

                // Get the JSON response.
                string jsonString = await response.Content.ReadAsStringAsync();

                imageResponse = Deserialize<ImageResponse>(jsonString, token => token["value"]);
            }
            return imageResponse;
        }

        private T Deserialize<T>(string json, Func<JToken, JToken> filter)
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
