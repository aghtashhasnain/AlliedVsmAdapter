using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using AlliedAdapter.Models;
using Newtonsoft.Json;
using Sedco.SelfService;
using static AlliedAdapter.Helpers.Constants;
using static AlliedAdapter.Models.TransactionResponse;

namespace AlliedAdapter.Helpers
{
    public class APIHelper : IDisposable
    {
        private static readonly HttpClient client = new HttpClient();

        public  async Task<APIResponse> SendTransaction(string url, HttpMethods httpMethod, object request, string apiKey, string signature)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();


                HttpResponseMessage response;
                string jsonRequest = "";

                if (httpMethod == HttpMethods.POST)
                {
                    jsonRequest = JsonConvert.SerializeObject(request);
                    StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    Logs.WriteLogEntry(LogType.Info, "", $"Sending POST Request to {url} with payload: {jsonRequest}", "SendTransaction");
             
                    response = await client.PostAsync(url, content);
                }
                else if (httpMethod == HttpMethods.GET)
                {
                    Console.WriteLine($"Sending GET Request to {url}");
                    response = await client.GetAsync(url);
                }
                else
                {
                    throw new NotSupportedException($"HTTP method '{httpMethod}' is not supported.");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response from {url}: {responseBody}");

                var deserializedResponse = JsonConvert.DeserializeObject<AlliedAdapter.Models.ResponseContent>(responseBody);

                return new APIResponse
                {
                    StatusCode = response.StatusCode,
                    ResponseContent = responseBody,
                    Message = deserializedResponse?.Message
                };
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request Exception: {httpEx.Message}");
                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ResponseContent = "A network-related error occurred."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ResponseContent = $"Something went wrong: {ex.Message}"
                };
            }
        }



        public async Task<APIResponse> SendRestTransaction(string url, HttpMethods httpMethod, object request, string apiKey, string signature)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Clear();

                    if (apiKey != "")
                    {
                     
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    
                    }


                    HttpResponseMessage response;
                    if (httpMethod == HttpMethods.POST)
                    {
                        string json = JsonConvert.SerializeObject(request);
                    //    Logs.WriteLogEntry("Info", "", "GetCustomerFromNadra API Request Serilized!: " + json, "SendRestTransaction");
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await client.PostAsync(url, content);
                    }
                    else if (httpMethod == HttpMethods.GET)
                    {
                        response = await client.GetAsync(url);
                    }
                    else
                    {
                        throw new NotSupportedException($"HTTP method '{httpMethod}' is not supported.");
                    }
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return new APIResponse
                    {
                        StatusCode = response.StatusCode,
                        ResponseContent = responseBody
                    };
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request Exception: {httpEx}");

                Console.ReadLine();
                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ResponseContent = "A network-related error occurred."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendTransactionAsync Exception: {ex}");

                Console.ReadLine();
                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ResponseContent = "Something Went Wrong."
                };
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


    }
}
