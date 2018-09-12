using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Atos.AI.Bot.Services
{
    public class WebAPIBaseService
    {
        private static HttpClient HTTPConnection
        {
            get
            {
                return lazyHTTPConnection.Value;
            }
        }

        private static Lazy<HttpClient> lazyHTTPConnection = new Lazy<HttpClient>(() =>
        {
            return new HttpClient();
        });

        public static async Task<WebAPIResult> ExecuteWebAPIRequest(HttpMethod httpMethod, string URL, string headerKeyName, string headerKeyValue, string contentBody = "", bool generateException = true)
        {
            var result = new WebAPIResult();

            // Create the Http Request Message
            using (var request = new HttpRequestMessage(httpMethod, URL))
            {
                // Add the access token and specify that the return value is required in JSON format
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // For API Key based auth
                request.Headers.Add(headerKeyName, headerKeyValue);

                //If the contentBody != "", attach it
                if (contentBody != "")
                {
                    request.Content = new StringContent(contentBody, Encoding.UTF8, "application/json");
                    request.Content.Headers.ContentLength = contentBody.Length;
                }

                using (var response = await HTTPConnection.SendAsync(request))
                {
                    result.ReponseCode = response.StatusCode;
                    result.ReponseContent = await response.Content.ReadAsStringAsync();
                }

                //Check to see if there are exceptions thrown from the Web API, thrown an exception
                //if (result.ReponseCode == HttpStatusCode.OK) return result;

                if (generateException && result.ReponseCode != HttpStatusCode.OK)
                {
                    // Get the error details from the SAP message
                    JObject errorInfo = JObject.Parse(result.ReponseContent);
                    string errorCode = (string)errorInfo["error"]["statusCode"];
                    string errorMessage = (string)errorInfo["error"]["message"];

                    var error = new WebAPIException(errorCode, errorMessage);
                    throw error;
                }
                else
                {
                    return result;
                }

            }

        }

        public static async Task<WebAPIResult> ExecuteOAuthWebAPIRequest(HttpMethod httpMethod, string URL, string authBearerToken, string contentBody = "", bool generateException = true, string headerKeyName = null, string headerKeyValue = null)
        {
            //LoggerService.Debug("Sending request to API for URI: " + URL);

            var result = new WebAPIResult();

            // Create the Http Request Message
            using (var request = new HttpRequestMessage(httpMethod, URL))
            {

                // Add the access token and specify that the return value is required in JSON format
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // For OAuth2, Bearer Token based APIs
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authBearerToken);

                if (!string.IsNullOrEmpty(headerKeyName) && !string.IsNullOrEmpty(headerKeyValue))
                {
                    request.Headers.Add(headerKeyName, headerKeyValue);
                }

                //If the contentBody != "", attach it
                if (contentBody != "")
                {
                    request.Content = new StringContent(contentBody, Encoding.UTF8, "application/json");
                    request.Content.Headers.ContentLength = contentBody.Length;
                }

                using (var response = await HTTPConnection.SendAsync(request))
                {
                    //LoggerService.Debug("Response Status: " + response.StatusCode);
                    result.ReponseCode = response.StatusCode;
                    result.ReponseContent = await response.Content.ReadAsStringAsync();
                }

                //Check to see if there are exceptions thrown from the Web API, thrown an exception
                if (generateException && (result.ReponseCode != HttpStatusCode.OK) && (result.ReponseCode != HttpStatusCode.Created) && (result.ReponseCode != HttpStatusCode.NoContent))
                {
                    if (!string.IsNullOrEmpty(result.ReponseContent))
                    {
                        //Default Values
                        string errorCode = result.ReponseCode.ToString();
                        string errorMessage = result.ReponseContent;

                        //Body may not be a valid json
                        try
                        {
                            // Get the error details from the SAP message
                            JObject errorInfo = JObject.Parse(result.ReponseContent);
                            errorCode = (string)errorInfo["error"]["code"];
                            errorMessage = (string)errorInfo["error"]["message"];
                        }
                        catch
                        {
                        }

                        var error = new WebAPIException(errorCode, errorMessage);
                        throw error;
                    }
                    else
                    {
                        throw new WebAPIException(result.ReponseCode.ToString(), result.ReponseCode.ToString());
                    }
                }
            }

            return result;
        }

        public class WebAPIResult
        {
            public HttpStatusCode ReponseCode { get; set; } = HttpStatusCode.NoContent;
            public string ReponseContent { get; set; } = "";
        }

        public class WebAPIByteResult
        {
            public HttpStatusCode ReponseCode { get; set; } = HttpStatusCode.NoContent;
            public byte[] ReponseContent { get; set; }
        }

        public class WebAPIException : Exception
        {
            public string ErrorCode { get; set; }

            public WebAPIException(string errorCode) : base(errorCode)
            {
                ErrorCode = errorCode;
            }

            public WebAPIException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }
    }
}