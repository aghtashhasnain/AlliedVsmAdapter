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
                    Logs.WriteLogEntry("info", "", $"Sending POST Request to {url} with payload: {jsonRequest}", "SendTransaction");
             
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




        public string GenerateSignature(string signatureData, string signingKey)
        {
            // Concatenate the transaction message with the signing key using the pipe symbol
            string dataToSign = $"{signatureData}|{signingKey}";

            // Generate the SHA256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));

                // Convert the byte array to a hexadecimal string
                StringBuilder signatureBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    signatureBuilder.Append(b.ToString("x2"));
                }

                return signatureBuilder.ToString();
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        ///// Sample Transaction ////

        public async Task<XDocument> GetCustomerAccountSample(XDocument response, string accountNumber)
        {

            try
            {
                Logs.WriteLogEntry("Info", "", "GetCustomerAccountSample function called ", "GetCustomerAccountSample");
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Successful";


                var body = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                body.Add(
                    new XElement("RespMessage", APIResultCodes.Success),
                    new XElement("AccountNumber", "1001020273"),
                    new XElement("CustomerNumber", "410488"),
                    new XElement("Name", "ST-1001020273"),
                    new XElement("Category", "6004"),
                    new XElement("Currency", "MWK"),
                    new XElement("AccountOfficer", 35),
                    new XElement("Group", 31),
                    new XElement("OpenActualBalance", 156218.71),
                    new XElement("OpenClearedBalance", 156218.71),
                    new XElement("ActualBalance", 146058.71),
                    new XElement("ClearedBalance", 146058.71),
                    new XElement("WorkingBalance", 146058.71),
                    new XElement("LastCreditDate", "2023-08-04"),
                    new XElement("LastCreditAmount", 10000.00),
                    new XElement("BankLastCreditDate", "2022-12-30"),
                    new XElement("BankLastCreditAmount", 6615.82),
                    new XElement("LastDebitDate", "2023-08-11"),
                    new XElement("BankLastDebitDate", "2023-06-01"),
                    new XElement("StartOfYearBalance", "185005.2"),
                    new XElement("OpeningDate", "2013-11-17"),
                    new XElement("OpeningCategory", "6004"),
                    new XElement("Company", "MW0010005")
                );



            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", "", "Error in GetCustomerAccountSample: " + ex, "GetCustomerAccountSample");
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));

            }

            return response;


        }

        public async Task<XDocument> GetCustomerKycSample(XDocument response, string accountNumber, int otp)
        {

            try
            {
                Logs.WriteLogEntry("Info", "", "GetCustomerKycSample function called ", "GetCustomerKycSample");
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Successful";


                string nextKycDateValue = "2025-03-25";

                // Parse the date
                DateTime nextKycDate;
                bool isFutureDate = false;
                string Value = "False";

                if (DateTime.TryParse(nextKycDateValue, out nextKycDate))
                {
                    isFutureDate = nextKycDate > DateTime.Now;
                    if (isFutureDate) {

                        Value = "True";
                        
                    }
                }



                var body = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                body.Add(
                         new XElement("RespMessage", APIResultCodes.Success),
                         new XElement("customerNumber", 410488),
                         new XElement("title", "MR"),
                         new XElement("firstName", "GN-410488"),
                         new XElement("middleName", null),
                         new XElement("lastName", "FN-410488"),
                         new XElement("mobileNumber", "265884577949"),
                         new XElement("emailAddress", null),
                         new XElement("dateOfBirth", "1994-06-12"),
                         new XElement("gender", "MALE"),
                         new XElement("maritalStatus", "SINGLE"),
                         new XElement("nationality", "MW"),
                         new XElement("sector", 1000),
                         new XElement("industry", 1001),
                         new XElement("target", 1005),
                         new XElement("amlRiskGrade", "MEDIUM"),
                         new XElement("dateLastKyc", "2020-03-25"),
                         new XElement("village", "MWIMA"),
                         new XElement("tradAuthority", "KALEMBO"),
                         new XElement("district", "BALAKA"),
                         new XElement("sourceOfFunds", "SALARY"),
                         new XElement("accountOfficer", "35"),
                         new XElement("companyCode", "MW0010005"),
                         new XElement("currency", "MWK"),
                         new XElement("street", "CI NEAR VILLAGE HOUSE"),
                         new XElement("country", "MW"),
                         new XElement("idNumber", "410488"),
                         new XElement("idType", "NATIONAL.ID"),
                         new XElement("idHolder", "LHN-410488"),
                         new XElement("idIssuedBy", "NRB"),
                         new XElement("idIssueDate", "2017-10-31"),
                         new XElement("idExpiryDate", "2021-06-12"),
                         new XElement("idFile", null),
                         new XElement("occupation", "IT SOFTWARE DEVELOPER"),
                         new XElement("signatureFile", null),
                         new XElement("employmentStatus", "EMPLOYED"),
                         new XElement("employer", "TNM"),
                         new XElement("employerBusiness", "Clothing Shop"),
                         new XElement("employerAddress", "P.O BOX 3039 BT"),
                         new XElement("grossIncome", 650000.00),
                         new XElement("netMonthlyIncome", 65000),
                         new XElement("incomeCurrency", "MWK"),
                         new XElement("nextOfKinName", null),
                         new XElement("nextOfKinMobile", null),
                         new XElement("nextOfKinRelationship", null),
                         new XElement("utilityBillFile", null),
                         new XElement("sketchMap", null),
                         new XElement("selfPortrait", null),
                         new XElement("paySlip", null),
                         new XElement("fullName", "GN-410488 FN-410488"),
                         new XElement("customerType", "Individual"),
                         new XElement("nextKycDate", Value)
                     );





            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", "", "Error in GetCustomerKycSample: " + ex, "GetCustomerKycSample");
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));

            }

            return response;


        }


        //public async Task<XDocument> GetBillProductsSample(XDocument response)
        //{
        //    try
        //    {

        //        var fetchBillingProductsResponse = new FetchBillingProductsResponse
        //        {
        //            message = "Products found",
        //            data = new List<Product>
        //            {
        //             new Product { account_number = "1001837978", faculty_id = "MZUNIFES", name = "Faculty of Environmental Sciences" },
        //             new Product { account_number = "386685", faculty_id = "MZUNIBUS", name = "MZUNI Business" },
        //             new Product { account_number = "1006607407", faculty_id = "MZUNIHSS", name = "Faculty of Humanities & Soc. Sciences" },
        //             new Product { account_number = "387401", faculty_id = "MZUNIICT", name = "Mzuni ICT" },
        //             new Product { account_number = "1007578896", faculty_id = "MZUNIDS", name = "MZUNI Data Science" }
        //            }
        //        };


        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Successful";


        //        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
        //        bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));


        //        foreach (var product in fetchBillingProductsResponse.data)
        //        {
        //            bodyElement.Add(new XElement("Product",
        //                new XElement("AccountNumber", product.account_number),
        //                new XElement("FacultyId", product.faculty_id),
        //                new XElement("Name", product.name)
        //            ));
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Logs.WriteLogEntry("Error", "", $"Error in GetBillProductsSample: {ex}", "GetBillProductsSample");

        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));
        //    }

        //    return response;
        //}


        //public async Task<XDocument> MakeBillPaymentSample(XDocument response)
        //{

        //    try
        //    {
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Successful";
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));

        //        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("CommissionCode", "DEBIT PLUS CHARGES"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("DebitAmount", "500.00"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("DebitAccount", "1001020273"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Reference", "FT23290Y4N4P"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("DateTime", "2410011302"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("DebitCurrency", "MWK"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("TransactionId", "47000525d5712"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("DebitCustomer", "410488"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("TransactionType", "ACMP"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("DebitValueDate", "20231017"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("ChargeCode", "DEBIT PLUS CHARGES"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("ChargedCustomer", "410488"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("LocalAmountCredited", "500.00"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("CreditAccount", "MWK1216600100001"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("ProcessingDate", "20231017"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("CreditCurrency", "MWK"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("CreditValueDate", "20231017"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("AmountCredited", "MWK500.00"));
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("AmountDebited", "MWK500.00"));




        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.WriteLogEntry("Error", "", "Error in MakeBillPaymentSample: " + ex, "MakeBillPaymentSample");
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));
        //    }

        //    return response;


        //}


        public async Task<XDocument> OpenAccountSample(XDocument response)
        {

            try
            {
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Validation successful";


                var data = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                data.Add(
                      new XElement("RespMessage", APIResultCodes.Success),
                      new XElement("AccountNumber", "1009994285"),
                      new XElement("CustomerNumber", "151424"),
                      new XElement("Name", "YOYERA MBATATA"),
                      new XElement("Category", "1000"),
                      new XElement("Currency", "MWK"),
                      new XElement("AccountOfficer", 35),
                      new XElement("Group", 5),
                      new XElement("OpenActualBalance", 0.00),
                      new XElement("OpenClearedBalance", 0.00),
                      new XElement("ActualBalance", 0.00),
                      new XElement("ClearedBalance", 0.00),
                      new XElement("WorkingBalance", 0.00),
                      new XElement("StartOfYearBalance", "0.00"),
                      new XElement("OpeningDate", DateTime.Now.ToString("dd-MM-yyyy")),
                      new XElement("OpeningCategory", "1000"),
                      new XElement("Company", "MW0010005")
                  );
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", "", "Error in OpenAccountSample: " + ex, "OpenAccountSample");
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));
            }

            return response;


        }


    }
}
