//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

//using RestSharp;
//namespace Acme.ShoppingCart.WebApi.IntegrationTests {
//    public static class HttpClientExtensions {
//        public static async Task<IRestResponse<T>> Execute<T>(this HttpClient client, IRestRequest request, HttpStatusCode expected = HttpStatusCode.OK) {
//            string uri = request.GetRequestUrl();
//            HttpMethod requestMethod = CastRequestMethod(request.Method);
//            HttpRequestMessage httpRequest = new HttpRequestMessage(requestMethod, uri);
//            client.DefaultRequestHeaders.Add("Accept", "application/json");
//            string body = request.GetRequestPayload();

//            if (!string.IsNullOrEmpty(body) && (requestMethod == HttpMethod.Put || requestMethod == HttpMethod.Post)) {
//                httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
//            }

//            foreach (var header in request.Parameters.Where(x => x.Type == ParameterType.HttpHeader)) {
//                httpRequest.Headers.Add(header.Name, header.Value.ToString());
//            }

//            HttpResponseMessage httpResponse = await client.SendAsync(httpRequest).ConfigureAwait(false);
//            string content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

//            IRestResponse<T> restResponse = new RestResponse<T> {
//                Request = request,
//                Content = content,
//                StatusCode = httpResponse.StatusCode,
//            };
//            foreach (var header in httpResponse.Headers) {
//                IEnumerable<string> values = null;
//                httpResponse.Headers.TryGetValues(header.Key, out values);
//                restResponse.Headers.Add(new Parameter(header.Key, values, ParameterType.HttpHeader, false));
//            }
//            if (httpResponse.IsSuccessStatusCode) {
//                restResponse.Data = JsonConvert.DeserializeObject<T>(restResponse.Content);
//            } else if (httpResponse.StatusCode != expected) {
//                var error = JsonConvert.DeserializeObject<HttpError>(content);
//                throw new Exception($"{uri}:{restResponse.StatusCode}:{content}:{error?.StackTrace}");
//            }
//            return restResponse;
//        }

//        public static string GetRequestUrl(this IRestRequest request) {
//            var uri = "api/" + request.Resource;
//            static bool filter(Parameter x) => x.Type == ParameterType.QueryString || x.Type == ParameterType.QueryStringWithoutEncode;
//            if (request.Parameters?.Count() > 0 && request.Parameters.Any(filter)) {
//                uri += "?";
//                foreach (var param in request.Parameters.Where(filter)) {
//                    uri += param.Name + "=" + param.Value;
//                }
//            }
//            return uri;
//        }

//        public static string GetRequestPayload(this IRestRequest request) {
//            string payload = string.Empty;

//            var body = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.RequestBody);
//            if (body != null) {
//                payload = JsonConvert.SerializeObject(body.Value, new JsonSerializerSettings() {
//                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
//                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
//                    Formatting = Formatting.Indented,
//                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
//                    NullValueHandling = NullValueHandling.Ignore
//                });
//            }

//            return payload;
//        }


//        public static async Task<ApiResponse<T>> SendAsync<T>(this HttpClient client, IRestRequest request, HttpStatusCode status = HttpStatusCode.OK) {
//            var uri = "api/" + request.Resource;

//            Console.Out.WriteLine($"Executing {request.Method} request to {uri}");
//            var stopwatch = new Stopwatch();
//            stopwatch.Start();
//            ApiResponse<T> result = new ApiResponse<T>();
//            IRestResponse Response = await client.Execute<T>(request, status).ConfigureAwait(false);

//            // retry Forbidden.....to try to make things less flakey
//            if (Response.StatusCode == HttpStatusCode.Forbidden) {
//                Console.Out.WriteLine($"FORBIDDEN: Retrying {request.Method} request to {uri}");
//                Response = await client.Execute<T>(request, status).ConfigureAwait(false);
//            }

//            stopwatch.Stop();
//            Console.Out.WriteLine($"Finished executing {request.Method} request to {uri} with status {Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");
//            if (Response.StatusCode == HttpStatusCode.Forbidden || Response.StatusCode == HttpStatusCode.Unauthorized) {
//                Console.Out.WriteLine($"Authorization header: {JsonConvert.SerializeObject(request.Parameters.FirstOrDefault(x => x.Name == "Authorization"))}");
//            }

//            result.Content = Response.Content;
//            result.ContentEncoding = Response.ContentEncoding;
//            result.ContentLength = Response.ContentLength;
//            result.ContentType = Response.ContentType;
//            result.Cookies = Response.Cookies;
//            result.Data = JsonConvert.DeserializeObject<T>(Response.Content);
//            result.ErrorException = Response.ErrorException;
//            result.ErrorMessage = Response.ErrorMessage;
//            result.Headers = Response.Headers;
//            result.IsSuccessful = Response.IsSuccessful;
//            result.ProtocolVersion = Response.ProtocolVersion;
//            result.RawBytes = Response.RawBytes;
//            result.Request = Response.Request;
//            result.ResponseStatus = Response.ResponseStatus;
//            result.ResponseUri = Response.ResponseUri;
//            result.Server = Response.Server;
//            result.StatusCode = Response.StatusCode;
//            result.StatusDescription = Response.StatusDescription;
//            return result;
//        }
//    }
//}
