using Sedco.SelfService;
using Sedco.SelfService.Server;
using Sedco.SelfService.Server.BackEndAdapters;
using SigmaDS4.Models;
using SigmaDS4.Models.Request;
using SigmaDS4.Models.Response;
using SigmaDS4.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using AlliedAdapter.Helpers;
using AlliedAdapter.Models;
using static AlliedAdapter.Helpers.Constants;
using static AlliedAdapter.Models.TransactionResponse;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Twilio.Rest.Api.V2010.Account;
using System.Reflection;
using System.Security.Policy;
using AlliedAdapter.WebReference1;
using AlliedAdapter.BioService;
using Microsoft.Build.Tasks;
using AlliedAdapter.CardListing;
using System.Web.Util;
using AlliedAdapter.AlliedSMSService;

using System.Data;
using OfficeOpenXml;
using Twilio.Http;
using iTextSharp.xmp.impl;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography.Xml;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Ocsp;
using static Microsoft.Exchange.WebServices.Data.SearchFilter;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using static AlliedAdapter.Class1;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Crypto;
using static OfficeOpenXml.ExcelErrorValue;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Security.Principal;
using AlliedAdapter;





//using iTextSharp.text;
//using iTextSharp.text.pdf;

namespace AlliedAdapter
{
    public class Class1 : IBackendServerAdapter
    {
        private const string DemoServiceUrlKey = "Url";
        private static List<AccountVariant> C_ASAAN_ACCOUNTS_SELECTION_LIST;
        public List<AccountVariantCacheItem> accountVariantsCache;
        private ApplicantData applicantData;
        bool UETflag = false;


        #region API URLs

        string MyPdaUrl = ConfigurationManager.AppSettings["MyPdaUrl"].ToString();
        string T24Url = ConfigurationManager.AppSettings["T24Url"].ToString();
        string IrisUrl = ConfigurationManager.AppSettings["IrisUrl"].ToString();

        #endregion

        private IApplicationConfiguration _applicationConfiguration;
        private string _demoServiceUrl;
        private bool _isDemoServiceUrlFound;

        private const string mobNoTag = "mobileNumber";


        /// <summary>
        /// check interface documentation
        /// </summary>
        /// 

        public void Initialize()
        {
            try
            {
                //Console.WriteLine("sdnjdnsdn")


                Logs.WriteLogEntry("info", "", "Initializing", "Initialize");

                _applicationConfiguration = SharedObjectsLocator.Instance.Get<IApplicationConfiguration>().First();
                Logs.WriteLogEntry("info", "", "_applicationConfiguration received", "Initialize");
                Logs.WriteLogEntry("info", "", "_applicationConfiguration" + JsonConvert.SerializeObject(_applicationConfiguration), "Initialize");

                //get url from "portal ==>System Adminstartion ==> System Settings ==>check Url key"
                _isDemoServiceUrlFound = _applicationConfiguration.ConfigurationList.TryGetValue(DemoServiceUrlKey, out _demoServiceUrl);

                Logs.WriteLogEntry("info", "", "_demoServiceUrl: " + _demoServiceUrl, "Initialize");

                Logs.WriteLogEntry("info", "", "_isDemoServiceUrlFound: " + _isDemoServiceUrlFound, "Initialize");


            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("info", "", "Exception: " + ex, "Initialize");

            }
        }
        public string CallBackEnd(XDocument request, string referenceNumber, RequestContent requestContent)
        {

            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {

                string response = "";

                // Retrieve the request type
                string requestType = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.RequestType).Value;

                Logs.WriteLogEntry("info", KioskId, "Request Type : " + requestType.ToLower(), "CallBackEnd");


                switch (requestType.ToLower())
                {
                    case "cnicverification":
                        response = Task.Run(() => CustomerVerification(request, referenceNumber)).Result;
                        break;
                    case "bioverification":
                        response = Task.Run(() => BioVerification(request, referenceNumber)).Result;
                        break;
                    case "customeraccountlist":
                        response = Task.Run(() => CustomerAccountList(request, referenceNumber)).Result;
                        break;
                    case "ablcardlist":
                        response = Task.Run(() => ABLCardList(request, referenceNumber)).Result;
                        break;
                    case "kgsprinterstatus":
                        response = Task.Run(() => GetPrinterStatus(request, referenceNumber)).Result;
                        break;
                    case "kgshopperstatus":
                        response = Task.Run(() => GetHopperStatus(request, referenceNumber)).Result;
                        break;
                    case "irisexistingcardlist":
                        response = Task.Run(() => IRISExistingCardList(request, referenceNumber)).Result;
                        break;
                    case "abldebitcardissuance":
                        response = Task.Run(() => ABLDebitCardIssuance(request, referenceNumber)).Result;
                        break;
                    case "sendotp":
                        response = Task.Run(() => SendOTP(request, referenceNumber)).Result;
                        break;
                    case "checkaccountbalance":
                        response = Task.Run(() => CheckAccountBalance(request, referenceNumber)).Result; 
                        break;
                    case "iriscardissuance":
                        response = Task.Run(() => CardIssuance(request, referenceNumber)).Result;
                        break;
                    //case "kgspersonalization":
                    //    response = Task.Run(() => CardPersonalization(request, referenceNumber)).Result;
                    //    break;
                    case "kgscardstatus":
                        response = Task.Run(() => GetCardStatus(request, referenceNumber)).Result;
                        break;
                    /// Account Opening
                    case "sendotpasanaccount":
                        response = Task.Run(() => SendOtpAsanAccount(request, referenceNumber)).Result;
                        break;
                    case "deleteapplication":
                        response = Task.Run(() => DeleteApplication(request, referenceNumber)).Result;
                        break;
                    case "pmdverification":
                        response = Task.Run(() => PmdVerification(request, referenceNumber)).Result;
                        break;
                    case "getcustomerfromnadra":
                        response = Task.Run(() => GetCustomerFromNadra(request, referenceNumber)).Result;
                        break;
                    case "liveliness":
                        response = Task.Run(() => LivelinessCheck(request, referenceNumber)).Result;
                        break;
                    case "getpersonalinformation":
                        response = Task.Run(() => PersonalInformation(request, referenceNumber)).Result;
                        break;
                    case "getcurrentaddress":
                        response = Task.Run(() => CurrentAddress(request, referenceNumber)).Result;
                        break;
                    case "getoccupationdetails":
                        response = Task.Run(() => OccupationalDetail(request, referenceNumber)).Result;
                        break;
                    case "getbankingreferencestage1":
                        response = Task.Run(() => BankingReference(request, referenceNumber)).Result;
                        break;
                    case "getbankingreferencestage2":
                        response = Task.Run(() => AccountsDetails(request, referenceNumber)).Result;
                        break;
                    case "revieweddetails":
                        response = Task.Run(() => ReviewedDetails(request, referenceNumber)).Result;  
                        break;
                    case "aoablcardlist":
                        //  response = Task.Run(() => AOABLCardList(request, referenceNumber)).Result;
                        break;
                    case "aoabldebitcardissuance":
                        response = Task.Run(() => AOABLDebitCardIssuance(request, referenceNumber)).Result;
                        break;
                    case "aoiriscardissuance":
                        response = Task.Run(() => AOCardIssuance(request, referenceNumber)).Result;
                        break;

                    default:
                        throw new Exception("Unknown request type: " + requestType);

                }

                Logs.WriteLogEntry("info", KioskId, "Response: " + response.ToString(), "CallBackEnd");

                return response.ToString();

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("info", KioskId, "Exception: " + ex, "CallBackEnd");

                XDocument responseDoc = request.GetBasicResponseFromRequest();
                responseDoc.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                responseDoc.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Exception in backend call: " + ex.Message;
                responseDoc.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.Message).Value = "Technical error happened while calling our servers. Please try again later.";
                return responseDoc.ToString();
            }
        }

        public bool CheckBackEndHeartbeat()
        {
            try
            {
                Logs.WriteLogEntry("info", "", "Checking backend heartbeat", "CheckBackEndHeartbeat");

                bool isBackendAlive = true;

                Logs.WriteLogEntry("info", "", "Backend heartbeat check result: " + isBackendAlive, "CheckBackEndHeartbeat");

                return isBackendAlive;
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("error", ex.Message, ex.StackTrace, "CheckBackEndHeartbeat");
                throw;
            }
        }


        #region Customer Verification 
        public async Task<string> CustomerVerification(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "CustomerVerification";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 1]: Validating Input Data: {response}", _MethodName);

                string tncurl = ConfigurationManager.AppSettings["TNCURL"].ToString();
                string TransactionId = GenerateTransactionId();
                string formattedDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 1]: Validating Input Data: {response}", _MethodName);

                string CnicNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cnic")?.Value ?? string.Empty;
                CnicNumber = CnicNumber.Replace("-", "");

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 2]: CNIC Number: {CnicNumber}", _MethodName);
                string url = T24Url + ConfigurationManager.AppSettings["CustomerVerification"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                var requestPayload = new ABLCustomerVerificationRequest
                {
                    ABLCustomerVerificationReq = new ABLCustomerVerificationReq
                    {
                        UserID = "XXXXX",
                        Password = "XXXXX",
                        ChannelType = "WEB",
                        ChannelSubType = "SSK",
                        TransactionType = "000",
                        TransactionSubType = "000",
                        TranDateAndTime = formattedDate,
                        Function = "CustomerVerification",
                        HostData = new HostData
                        {
                            TransReferenceNo = TransactionId,
                            CNIC = CnicNumber
                        }
                    }
                };

                APIResponse ApiResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, KioskId, "");

                if (ApiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 3]: API Response Content: {ApiResponse.ResponseContent}", _MethodName);

                    var responseData = JsonConvert.DeserializeObject<ABLCustomerVerificationResponse>(ApiResponse.ResponseContent);
                    var verificationResponse = responseData?.ABLCustomerVerificationRsp;

                    Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 4]: Deserialized Verification Response: {verificationResponse}", _MethodName);

                    if (verificationResponse != null && verificationResponse.StatusDesc == "Success")
                    {
                        var hostData = verificationResponse.HostData;
                        Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 5]: Host Data: {hostData}", _MethodName);

                        if (hostData?.HostDesc == null && hostData.CustomerNumber != null)
                        {
                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Validation successful";

                            string MobileNumber = ExtractDigitsOnly(hostData.PhoneNumber);
                            Logs.WriteLogEntry("Info", KioskId, $"Formated mobile number :{MobileNumber}", _MethodName);

                            bodyElement.Add(
                                new XElement("RespMessage", APIResultCodes.Success),
                                new XElement("Name", hostData.Name),
                                new XElement("CustomerNumber", hostData.CustomerNumber),
                                new XElement("DOB", hostData.DOB),
                                new XElement("PhoneNumber", hostData.PhoneNumber),
                                new XElement("Email", hostData.Email),
                                new XElement("CNIC", hostData.CNIC),
                                new XElement("TransactionId", TransactionId),
                                new XElement("TNCURL", tncurl));
                        }
                        else
                        {
                            Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 6]: Verification Failed - Record Not Found. Status Code: {ApiResponse.StatusCode}", _MethodName);
                            Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 6]: Error Message: {ApiResponse.Message}", _MethodName);

                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(
                                new XElement("Message", "AccountNotExist"),
                                new XElement("IsAvailable", "Not"),
                                new XElement("TNCURL", tncurl));
                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 7]: API Request Failed - Status Code: {ApiResponse.StatusCode}", _MethodName);
                        Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 7]: API Error Message: {ApiResponse.Message}", _MethodName);

                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 8]: API Request Failed - Status Code: {ApiResponse.StatusCode}", _MethodName);
                    Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 8]: API Error Message: {ApiResponse.Message}", _MethodName);

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 9]: Exception occurred: {ex.Message}", _MethodName);

                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));
            }

            return response.ToString();
        }



        #endregion

        #region Bio Verification
        public async Task<string> BioVerification(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "BioVerification";
            XDocument response = request.GetBasicResponseFromRequest();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 1]: Received Request: {request}", _MethodName);

                // Extract values
                string cnicNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cnic")?.Value ?? string.Empty;
                string fingerImage = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("fingerImage")?.Value ?? string.Empty;
                string contactNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("contactnumber")?.Value ?? string.Empty;
                string NumTry = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("NumTry")?.Value ?? string.Empty;

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 2]: Extracted Input - CNIC: {cnicNumber}, Contact: {contactNumber}, NumTry: {NumTry}", _MethodName);

                // Process Finger Index
                cnicNumber = cnicNumber.Replace("-", "");
                string FinalFingerIndex = "";
                if (NumTry == "1")
                {
                    FinalFingerIndex = "1";
                }
                else if (NumTry == "2")
                {
                    FinalFingerIndex = "2";
                }
                else if (NumTry == "3")
                {
                    FinalFingerIndex = "6";
                }
                else if (NumTry == "4")
                {
                    FinalFingerIndex = "7";
                }

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 3]: Processed FingerIndex: {FinalFingerIndex}", _MethodName);

                var soapClient = new BioService.ATMMSGSetSOAP_HTTP_Service();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {soapClient.Url}", _MethodName);
                var soapRequest = new BioService.complexType
                {
                    CNIC = cnicNumber,
                    FINGER_NO = FinalFingerIndex,
                    CONTACT_NO = contactNumber,
                    ISO_NADRA = fingerImage,
                    ISO_LOCAL = fingerImage,
                    FLAG = "3"
                };

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 5]: Sending request to SOAP Service at URL: {soapClient.Url}", _MethodName);
                //   Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 4]: Prepared SOAP Request: {JsonConvert.SerializeObject(soapRequest)}", _MethodName);

                // Call SOAP API
                var soapResponse = soapClient.Operation1(soapRequest);

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 6]: SOAP Response Received - Code: {soapResponse.CODE}, Message: {soapResponse.MESSAGE}", _MethodName);

                var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                if (soapResponse.CODE == "100")
                {
                    Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 7]: BioVerification Success", _MethodName);

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Bio Validation successful";

                    bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 7]: BioVerification Failed", _MethodName);

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Bio Validation Failed";

                    bodyElement.Add(new XElement("Message", "Bio Validation Failed"));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 8]: Exception occurred: {ex}", _MethodName);

                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }

            Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 9]: Final Response: {response}", _MethodName);

            return response.ToString();
        }

        #endregion

        #region Customer Account List
        public async Task<string> CustomerAccountList(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "CustomerAccountList";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 1]: Validating Input Data: {request}", _MethodName);

                string TransactionId = GenerateTransactionId();
                DateTime dateTime = DateTime.Now;
                string formattedDate = dateTime.ToString("dd-MM-yyyy HH:mm:ss");
                string CustomerNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CustomerNumber")?.Value ?? string.Empty;

                string url = T24Url + ConfigurationManager.AppSettings["ABLCustomerAccountList"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]: {url}", _MethodName);

                var requestPayload = new ABLCustomerAccountListRequest
                {
                    ABLCustomerAccountListReq = new ABLCustomerAccountListReq
                    {
                        UserID = "XXXXX",
                        Password = "XXXXX",
                        ChannelType = "WEB",
                        ChannelSubType = "SSK",
                        TransactionType = "000",
                        TransactionSubType = "000",
                        TranDateAndTime = formattedDate,
                        Function = "AccountList",
                        HostData = new AccountListHostData
                        {
                            TransReferenceNo = TransactionId,
                            CustomerNumber = CustomerNumber
                        }
                    }
                };

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 3]: Prepared Request Payload: {JsonConvert.SerializeObject(requestPayload)}", _MethodName);

                APIResponse aPIResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, KioskId, "");

                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 4]: Request successful. Parsing response...", _MethodName);

                    var responseData = JsonConvert.DeserializeObject<ABLCustomerAccountListResponse>(aPIResponse.ResponseContent);
                    var accountListResponse = responseData?.ABLCustomerAccountListRsp;

                    if (accountListResponse != null && accountListResponse.StatusDesc == "Success")
                    {

                        if (accountListResponse.HostData.Account != null)
                        {
                            Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 5]: Parsed response: {JsonConvert.SerializeObject(accountListResponse)}", _MethodName);

                            var idContentList = ExtractAndLogValues(aPIResponse.ResponseContent);
                            Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 6]: Extracted Account Data", _MethodName);
                            string Name = "";
                            foreach (var column in accountListResponse.HostData.Account.Column)
                            {
                                if (column.Id == "CUSTNAME")
                                    Name = column.Content.ToString();
                            }

                            Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 7]: Extracted Customer Name: {Name}", _MethodName);

                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Account Found !";
                            bodyElement.Add(
                                new XElement("RespMessage", APIResultCodes.Success),
                                new XElement("AccountData", idContentList),
                                new XElement("Name", Name)
                            );
                            Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 8]: Account Data Added to Response", _MethodName);
                        }
                        else
                        {
                            Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 6]: No account data found in the response", _MethodName);

                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "AccountNotExist"));

                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 7]: API Request Failed - Status Code: {aPIResponse.StatusCode}", _MethodName);
                        Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 7]: API Error Message: {aPIResponse.Message}", _MethodName);

                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 4]: Request failed with status code: {aPIResponse.StatusCode}", _MethodName);
                    Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 5]: Error Message: {aPIResponse.Message}", _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 9]: Exception occurred: {ex.Message}", _MethodName);

                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));
            }

            Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 10]: Final Response: {response}", _MethodName);

            return response.ToString();
        }

        #endregion

        #region ABL ATM CardList

        public async Task<string> ABLCardList(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "ABLCardList";
            List<Dictionary<string, object>> finalATMCardList = new List<Dictionary<string, object>>();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            XDocument response = request.GetBasicResponseFromRequest();

            try
            {
                Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 1]: Received request: {request}", _MethodName);
                string kioskID = KioskId;
                string PcName = ConfigurationManager.AppSettings[kioskID].ToString();

                Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 2]: Retrieved PcName = {PcName}", _MethodName);

                string[] parts = PcName.Split('|');
                string ComputerName = parts[0].Trim();
                string BranchCode = parts[1].Trim();

                Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 3]: ComputerName = {ComputerName}, BranchCode = {BranchCode}", _MethodName);

                string CardImageBaseUrl = ConfigurationManager.AppSettings["CardImageBaseUrl"].ToString();
                Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 4]: CardImageBaseUrl = {CardImageBaseUrl}", _MethodName);

                string productcode = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("productcode")?.Value ?? string.Empty;
                string AccountCurrency = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("AccountCurrency")?.Value ?? string.Empty;
                string transactionType = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("TransactionType")?.Value ?? string.Empty;
                string accountType = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("AccountType")?.Value ?? string.Empty;

                Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 5]: Extracted inputs - productcode: {productcode}, AccountCurrency: {AccountCurrency}, TransactionType: {transactionType}, AccountType: {accountType}", _MethodName);

                List<ABLCardInfo> atmCardList = ABLAtmCardList(productcode, AccountCurrency, transactionType, KioskId);
                Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 6]: Retrieved ABLCardList with count: {atmCardList?.Count}", _MethodName);

                if (atmCardList == null || atmCardList.Count == 0)
                {
                    Logs.WriteLogEntry("Erro", KioskId, $"{_MethodName} [Step 7]: Failed to retrieve ABL ATM card list.", _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Failed to retrieve ABL ATM card list";
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "NoCardAllowed"));
                    return response.ToString();
                }

                List<CardFormats> cardFormats = GetCardFormats(ComputerName, kioskID);
                Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 8]: Fetching predefined card formats.", _MethodName);

                //string jsonCardFormat = "[{\"name\":\"UPI PAYPAK CLASSIC DEBIT CARD\"},{\"name\":\"VISA CLASSIC DEBIT CARD\"},{\"name\":\"VISA ISLAMIC CLASSIC DEBIT CARD\"},{\"name\":\"VISA PLATINUM DEBIT CARD\"},{\"name\":\"VISA PREMIUM DEBIT CARD\"}]";
                //List<CardFormats> cardFormats = JsonConvert.DeserializeObject<List<CardFormats>>(jsonCardFormat);

                if (cardFormats == null || cardFormats.Count == 0)
                {
                    Logs.WriteLogEntry("Erro", KioskId, $"{_MethodName} [Step 9]: Failed to get card formats.", _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Failed to get card formats";
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                    return response.ToString();
                }

                Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 10]: Filtering cards by transaction type and format.", _MethodName);

                foreach (var item in atmCardList)
                {
                    bool isValidCard = cardFormats.Any(cf => cf.name == item.name);
                    if (!isValidCard) continue;

                    if (transactionType == "AsanAccount")
                    {
                        if ((accountType == "114202" && (item.IrisCardProductCode == "0081" || item.IrisCardProductCode == "0075")) ||
                            (accountType == "114201" && (item.IrisCardProductCode == "0081" || item.IrisCardProductCode == "0070")))
                        {
                            Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 11]: Processing card: {item.name}", _MethodName);
                            CardCharges cardCharges = await ABLDebitCardCharges(item.t24CardCode, KioskId);

                            finalATMCardList.Add(new Dictionary<string, object>
                            {
                                {"id", item.IrisCardProductCode},
                                {"name", item.name},
                                {"replacementCharges", cardCharges.replacementamount},
                                {"issuanceCharges", cardCharges.issuanceamount},
                                {"t24CardCode", item.t24CardCode},
                                {"t24AccountCategoryCode", item.t24AccountCategoryCode},
                                {"variant", item.variant},
                                {"scheme", item.scheme},
                                {"perDayFT", item.perDayFT},
                                {"billPaymentLimit", item.billPaymentLimit},
                                {"cashWithdrawalLimit", item.cashWithdrawalLimit},
                                {"eCommerceLimit", item.eCommerceLimit},
                                {"imagePath", CardImageBaseUrl + item.ImagePath}
                            });

                            Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 12]: Card Processed - ID: {item.IrisCardProductCode}, Name: {item.name}", _MethodName);
                        }
                    }
                    else
                    {

                        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 13]: Processing general card: {item.name}", _MethodName);
                        CardCharges cardCharges = await ABLDebitCardCharges(item.t24CardCode, KioskId);

                        finalATMCardList.Add(new Dictionary<string, object>
                        {
                                {"id", item.IrisCardProductCode},
                                {"name", item.name},
                                {"replacementCharges", cardCharges.replacementamount},
                                {"issuanceCharges", cardCharges.issuanceamount},
                                {"t24CardCode", item.t24CardCode},
                                {"t24AccountCategoryCode", item.t24AccountCategoryCode},
                                {"variant", item.variant},
                                {"scheme", item.scheme},
                                {"perDayFT", item.perDayFT},
                                {"billPaymentLimit", item.billPaymentLimit},
                                {"cashWithdrawalLimit", item.cashWithdrawalLimit},
                                {"eCommerceLimit", item.eCommerceLimit},
                                {"imagePath", CardImageBaseUrl + item.ImagePath}
                        });
                        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 14]: Card Processed - ID: {item.IrisCardProductCode}, Name: {item.name}", _MethodName);
                    }
                }
                if (finalATMCardList.Count == 0)
                {
                    Logs.WriteLogEntry("Erro", KioskId, $"{_MethodName} [Step 15]: No matching card formats found in ABL Card List.", _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "No matching card formats found in ABL Card List";
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }
                else
                {
                    Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 16]: Successfully processed {finalATMCardList.Count} cards.", _MethodName);
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "ABLCardList Received";
                    bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success), new XElement("CardList", finalATMCardList));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 17]: Exception occurred: {ex.Message}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "An error occurred while processing your request.";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Exception occurred: " + ex.Message));
            }
            return response.ToString();
        }

        static List<ABLCardInfo> ABLAtmCardList(string productcode, string AccountCurrency, string transactionType, string KioskId)
        {
            string _MethodName = "ABLAtmCardList";
            try
            {
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 1]: Going to Import Excel", _MethodName);

                DataTable cardDataTable = ImportExcel(KioskId);

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 2]: Excel Imported", _MethodName);

                List<ABLCardInfo> ablCardList = new List<ABLCardInfo>();

                bool Flag = false;
                string lastAccountCategory = "";

                foreach (DataRow row in cardDataTable.Rows)
                {
                    string accountCategory = row["T24 Account Category Code"]?.ToString().Trim();
                    string currency = row["Currency"]?.ToString().Trim();

                    if (!string.IsNullOrEmpty(accountCategory))
                    {
                        lastAccountCategory = accountCategory;
                    }
                    else
                    {
                        accountCategory = lastAccountCategory;
                    }

                    string[] Codes = accountCategory.Split(',')
                          .Select(code => code.Trim())
                          .Where(code => !string.IsNullOrEmpty(code))
                          .ToArray();

                    ABLCardInfo cardInfo;

                    if (transactionType == "AsanAccount")
                    {
                        if (currency == "PKR")
                        {
                            cardInfo = new ABLCardInfo
                            {
                                IrisCardProductCode = row["IRIS Card Product Code"]?.ToString(),
                                name = row["IRIS Card Product Description (Card Variant)"]?.ToString(),
                                t24AccountCategoryCode = productcode,
                                Currency = row["Currency"]?.ToString(),
                                t24CardCode = row["T24 Card Code"]?.ToString(),
                                issuanceCharges = row["Issuance Charges"]?.ToString(),
                                scheme = row["Scheme"]?.ToString(),
                                variant = row["Variant"]?.ToString(),
                                replacementCharges = row["Replacement Charges"]?.ToString(),
                                perDayFT = row["Per Day ATM IBF/FT"]?.ToString(),
                                billPaymentLimit = row["Bill Payment Limit/Donation"]?.ToString(),
                                cashWithdrawalLimit = row["ATM Cash Withdrawal Limit"]?.ToString(),
                                eCommerceLimit = row["POS/eCommerce Limit"]?.ToString(),
                                ImagePath = row["Card Image Name"]?.ToString()
                            };
                            ablCardList.Add(cardInfo);
                        }
                    }
                    else
                    {
                        if ((Codes.Contains(productcode) || Flag) && AccountCurrency != "PKR")
                        {
                            string ImageName = row["Card Image Name"]?.ToString();

                            if (currency == AccountCurrency)
                            {
                                cardInfo = new ABLCardInfo
                                {
                                    IrisCardProductCode = row["IRIS Card Product Code"]?.ToString(),
                                    name = row["IRIS Card Product Description (Card Variant)"]?.ToString(),
                                    t24AccountCategoryCode = productcode,
                                    Currency = row["Currency"]?.ToString(),
                                    t24CardCode = row["T24 Card Code"]?.ToString(),
                                    issuanceCharges = row["Issuance Charges"]?.ToString(),
                                    scheme = row["Scheme"]?.ToString(),
                                    variant = row["Variant"]?.ToString(),
                                    replacementCharges = row["Replacement Charges"]?.ToString(),
                                    perDayFT = row["Per Day ATM IBF/FT"]?.ToString(),
                                    billPaymentLimit = row["Bill Payment Limit/Donation"]?.ToString(),
                                    cashWithdrawalLimit = row["ATM Cash Withdrawal Limit"]?.ToString(),
                                    eCommerceLimit = row["POS/eCommerce Limit"]?.ToString(),
                                    ImagePath = row["Card Image Name"]?.ToString()
                                };
                                ablCardList.Add(cardInfo);
                                Flag = true;
                            }
                        }
                        else if (Codes.Contains(productcode) && currency == "PKR")
                        {
                            cardInfo = new ABLCardInfo
                            {
                                IrisCardProductCode = row["IRIS Card Product Code"]?.ToString(),
                                name = row["IRIS Card Product Description (Card Variant)"]?.ToString(),
                                t24AccountCategoryCode = productcode,
                                Currency = row["Currency"]?.ToString(),
                                t24CardCode = row["T24 Card Code"]?.ToString(),
                                issuanceCharges = row["Issuance Charges"]?.ToString(),
                                scheme = row["Scheme"]?.ToString(),
                                variant = row["Variant"]?.ToString(),
                                replacementCharges = row["Replacement Charges"]?.ToString(),
                                perDayFT = row["Per Day ATM IBF/FT"]?.ToString(),
                                billPaymentLimit = row["Bill Payment Limit/Donation"]?.ToString(),
                                cashWithdrawalLimit = row["ATM Cash Withdrawal Limit"]?.ToString(),
                                eCommerceLimit = row["POS/eCommerce Limit"]?.ToString(),
                                ImagePath = row["Card Image Name"]?.ToString()
                            };
                            ablCardList.Add(cardInfo);
                        }
                    }
                }

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 3]: Process completed successfully", _MethodName);
                return ablCardList;
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 17]: Exception occurred: {ex.Message}", _MethodName);
            }

            return null;
        }



        #endregion

        #region Debit Charges
        public async Task<CardCharges> ABLDebitCardCharges(string ProductCode, string KioskId)
        {
            string _MethodName = "ABLDebitCardCharges";
            APIHelper apiService = new APIHelper();
            CardCharges cardCharges = new CardCharges();

            try
            {
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 1]: Validating Input Data: ProductCode={ProductCode}, KioskId={KioskId}", _MethodName);

                string TransactionId = GenerateTransactionId();
                DateTime dateTime = DateTime.Now;
                string formattedDate = dateTime.ToString("dd-MM-yyyy HH:mm:ss");

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 2]: Generating TransactionId and Formatting Date: TransactionId={TransactionId}, FormattedDate={formattedDate}", _MethodName);

                string url = T24Url + ConfigurationManager.AppSettings["DebitCardCharges"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]: {url}", _MethodName);


                string updatedNumber = ProductCode.TrimStart('0');

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 3]: Constructed URL: {url}, Updated ProductCode: {updatedNumber}", _MethodName);

                var requestPayload = new ABLDebitCardChargesRequest
                {
                    ABLDebitCardCharges = new ABLDebitCardCharges
                    {
                        UserID = "XXXXX",
                        Password = "XXXXX",
                        ChannelType = "WEB",
                        ChannelSubType = "SSK",
                        TransactionType = "000",
                        TransactionSubType = "000",
                        TranDateAndTime = formattedDate,
                        Function = "DebitCardCharges",
                        HostData = new DebitCardHostData
                        {
                            TransReferenceNo = TransactionId,
                            IDNumber = updatedNumber
                        }
                    }
                };

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 4]: Sending request to API", _MethodName);
                APIResponse aPIResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, KioskId, "");

                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 5]: API Response received: {aPIResponse}", _MethodName);

                    // Deserialize and work with the response content
                    List<(string ID, string Content)> idContentList = ExtractAndLogValues(aPIResponse.ResponseContent);

                    foreach (var pair in idContentList)
                    {
                        string id = pair.ID;
                        string content = pair.Content;

                        if (id == "ISSUANCE.AMOUNT")
                        {
                            cardCharges.issuanceamount = content;
                        }
                        else if (id == "REPLACEMENT.AMOUNT")
                        {
                            cardCharges.replacementamount = content;
                        }
                    }

                    Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Step 6]: Processed Card Charges: IssuanceAmount={cardCharges.issuanceamount}, ReplacementAmount={cardCharges.replacementamount}", _MethodName);
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 7]: API Error Message: {aPIResponse.Message}", _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 8]: Exception occurred: {ex.Message}", _MethodName);
            }

            return cardCharges;
        }


        #endregion

        #region PrinterStatus
        public async Task<string> GetPrinterStatus(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "GetPrinterStatus";
            XDocument response = request.GetBasicResponseFromRequest();
            SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();
            string kioskID = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element("KioskIdentity").Value;
            try
            {
                Logs.WriteLogEntry("info", kioskID, "KIOSK ID: " + kioskID, _MethodName);

                string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
                Logs.WriteLogEntry("info", kioskID, "Pc Name and Branch Code: " + PcName, _MethodName);
                string[] parts = PcName.Split('|');
                string ComputerName = parts[0].Trim();
                string BranchCode = parts[1].Trim();

                Logs.WriteLogEntry("info", kioskID, "Computer Name:" + ComputerName, _MethodName);
                Logs.WriteLogEntry("info", kioskID, "Branch Code:" + BranchCode, _MethodName);

                Logs.WriteLogEntry("Info", kioskID, "Request " + request.ToString(), _MethodName);
                string CardName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardName")?.Value ?? string.Empty;
                var getPrinterStatus = deviceOperations.GetPrinterStatus(ComputerName, CardName);
                Logs.WriteLogEntry("Info", kioskID, "Get Printer Status Response Code:" + getPrinterStatus.code, _MethodName);
                if (getPrinterStatus.code == 0)
                {
                    string jsonPrinterStatus = getPrinterStatus.data.ToString();
                    Logs.WriteLogEntry("Info", kioskID, "Printer Status: " + jsonPrinterStatus, _MethodName);
                    PrinterStatus printerStatus = JsonConvert.DeserializeObject<PrinterStatus>(jsonPrinterStatus);
                    Logs.WriteLogEntry("Info", kioskID, "Printer Status Deserialized: " + printerStatus, _MethodName);
                    if (printerStatus.status.ToLower() == "ready")
                    {
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                        bodyElement.Add(
                             new XElement("RespMessage", APIResultCodes.Success)
                        );
                    }
                    else
                    {
                        Logs.WriteLogEntry("Info", kioskID, "Printer Not Ready:" + printerStatus.status, _MethodName);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Printer Not Ready:" + printerStatus.status;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "PrinterNotConnected"));
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Info", kioskID, "Printer Not Available", _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Printer Not Available";
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "PrinterNotAvailable"));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", kioskID, "Error in Failed to Get Printer Status!: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }

            return response.ToString();
        }

        #endregion

        #region Hopper Status
        public async Task<string> GetHopperStatus(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "GetHopperStatus";
            XDocument response = request.GetBasicResponseFromRequest();
            SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();

            string kioskID = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element("KioskIdentity").Value;

            try
            {

                Logs.WriteLogEntry("info", kioskID, "KIOSK ID: " + kioskID, _MethodName);

                string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
                Logs.WriteLogEntry("info", kioskID, "Pc Name and Branch Code: " + PcName, _MethodName);

                string[] parts = PcName.Split('|');

                string ComputerName = parts[0].Trim();
                string BranchCode = parts[1].Trim();

                Logs.WriteLogEntry("info", kioskID, "Computer Name:" + ComputerName, _MethodName);
                Logs.WriteLogEntry("info", kioskID, "Branch Code:" + BranchCode, _MethodName);


                Logs.WriteLogEntry("Info", kioskID, "Request " + request.ToString(), _MethodName);
                string CardName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardName")?.Value ?? string.Empty;

                var getHopperStatus = deviceOperations.IsHopperAvailableForPrinting(ComputerName, CardName);

                Logs.WriteLogEntry("Info", kioskID, "Get Hopper Status Response :" + getHopperStatus, _MethodName);

                if (getHopperStatus.code == 0)
                {
                    string jsonHopperStatus = getHopperStatus.data.ToString();

                    HopperStatus HopperStatus = JsonConvert.DeserializeObject<HopperStatus>(jsonHopperStatus);

                    Logs.WriteLogEntry("Info", kioskID, "Hopper Status Desrelized: " + HopperStatus, _MethodName);
                    if (HopperStatus.productAvailable)
                    {

                        Logs.WriteLogEntry("Info", kioskID, "Hopper Status: " + jsonHopperStatus, _MethodName);


                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                        bodyElement.Add(
                             new XElement("RespMessage", APIResultCodes.Success)
                        );

                    }
                }
                else
                {
                    Logs.WriteLogEntry("Info", kioskID, "Hopper Not Available", _MethodName);

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Hopper Not Available";
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "CardNotAvailable"));

                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", kioskID, "Error in Failed to Get Hopper Status!: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }

            return response.ToString();
        }

        #endregion


        //#region PrinterStatus
        //public async Task<string> GetPrinterStatus(XDocument request, string RefrenceNumber)
        //{
        //    string _MethodName = "GetPrinterStatus";
        //    XDocument response = request.GetBasicResponseFromRequest();
        //    SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();
        //    string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

        //    try
        //    {

        //        //CardInfo cardInfo = DecryptEmbossingFile("0010", "0070", KioskId);

        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 1]: Validating KioskId: {KioskId}", _MethodName);

        //        string kioskID = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element("KioskIdentity").Value;
        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 2]: Kiosk ID: {kioskID}", _MethodName);

        //        string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 3]: Pc Name and Branch Code: {PcName}", _MethodName);

        //        string[] parts = PcName.Split('|');
        //        string ComputerName = parts[0].Trim();
        //        string BranchCode = parts[1].Trim();

        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 4]: Computer Name: {ComputerName}, Branch Code: {BranchCode}", _MethodName);

        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 5]: Request: {request.ToString()}", _MethodName);

        //        // Call to device operation and logging
        //        // var getPrinterStatus = deviceOperations.GetPrinterStatus(ComputerName, CardName);
        //        // Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 6]: Printer Status Response Code: {getPrinterStatus.code}", _MethodName);

        //        // Continue with processing...

        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
        //        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

        //        bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 7]: Exception occurred: {ex.Message}", _MethodName);
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";

        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Your Request Could Not Be Processed at the Moment."));
        //    }

        //    return response.ToString();
        //}


        //#endregion

        //#region Hopper Status
        //public async Task<string> GetHopperStatus(XDocument request, string RefrenceNumber)
        //{
        //    string _MethodName = "GetHopperStatus";
        //    XDocument response = request.GetBasicResponseFromRequest();
        //    SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();
        //    string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

        //    try
        //    {
        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 1]: Validating KioskId: {KioskId}", _MethodName);

        //        string kioskID = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element("KioskIdentity").Value;
        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 2]: Kiosk ID: {kioskID}", _MethodName);

        //        string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 3]: Pc Name and Branch Code: {PcName}", _MethodName);

        //        string[] parts = PcName.Split('|');
        //        string ComputerName = parts[0].Trim();
        //        string BranchCode = parts[1].Trim();

        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 4]: Computer Name: {ComputerName}, Branch Code: {BranchCode}", _MethodName);

        //        Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 5]: Request: {request.ToString()}", _MethodName);

        //        // Call to device operation and logging
        //        // var getHopperStatus = deviceOperations.IsHopperAvailableForPrinting(ComputerName, CardName);
        //        // Logs.WriteLogEntry("info", KioskId, $"{_MethodName} [Step 6]: Hopper Status Response: {getHopperStatus.code}", _MethodName);

        //        // Continue with processing...

        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
        //        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

        //        bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} [Step 7]: Exception occurred: {ex.Message}", _MethodName);
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";

        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Your Request Could Not Be Processed at the Moment."));
        //    }

        //    return response.ToString();
        //}


        //#endregion

        #region Check Account Balance
        public async Task<string> CheckAccountBalance(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "CheckAccountBalance";
            XDocument response = request.GetBasicResponseFromRequest();
            smpp_ws_sendsms service = new smpp_ws_sendsms();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                string AccountBalance = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountBalance")?.Value ?? string.Empty;
                string IssuanceAmount = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("IssuanceAmount")?.Value ?? string.Empty;
                string ReplacementAmount = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("ReplacementAmount")?.Value ?? string.Empty;
                string CardGenerationType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardGenerationType")?.Value ?? string.Empty;

                Logs.WriteLogEntry("INFO", KioskId, $"Received Request: {request}", _MethodName);


                double FinalIssueAmount = 0;
                double FinalReplaceAmount = 0;
                if (!double.TryParse(AccountBalance, out double FinalBalance) ||
                 !double.TryParse(IssuanceAmount, out FinalIssueAmount) ||
                 !double.TryParse(ReplacementAmount, out FinalReplaceAmount))
                {
                    Logs.WriteLogEntry("ERROR", KioskId, "Invalid numeric values in request.", _MethodName);
                }


                bool BalanceAvailabe = false;

                Logs.WriteLogEntry("info", KioskId, $"CardGenerationType : {CardGenerationType}, Account Balance : {FinalBalance}, Card Issuance Amount : {FinalIssueAmount}, Card Replacement Amount : {FinalReplaceAmount}", _MethodName);

                if (CardGenerationType == "Fresh" || CardGenerationType == "Upgrade")
                {
                    Logs.WriteLogEntry("info", KioskId, "Going to check Account Balance is Avaialble to compare issuance Amount", _MethodName);
                    if (FinalBalance >= FinalIssueAmount)
                    {
                        BalanceAvailabe = true;
                        Logs.WriteLogEntry("INFO", KioskId, "Account balance is sufficient for Card Issuance", _MethodName);
                    }
                    else
                    {
                        Logs.WriteLogEntry("ERROR", KioskId, "Insufficient balance for Card Replacement", _MethodName);
                    }

                }
                else if (CardGenerationType == "Replace")
                {

                    if (FinalBalance >= FinalReplaceAmount)
                    {
                        BalanceAvailabe = true;
                        Logs.WriteLogEntry("INFO", KioskId, "Account balance is sufficient for Replacement", _MethodName);
                    }
                    else
                    {
                        Logs.WriteLogEntry("ERROR", KioskId, "Insufficient balance for Card Replacement", _MethodName);
                    }
                }
                if (BalanceAvailabe)
                {
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    Logs.WriteLogEntry("INFO", KioskId, "Account balance is sufficient.", _MethodName);

                }
                else
                {
                    Logs.WriteLogEntry("ERROR", KioskId, "Insufficient balance for requested transaction.", _MethodName);

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Insufficient balance for requested transaction"));

                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error in CheckAccountBalance: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));
            }

            return response.ToString();
        }
        #endregion
        
        #region ABL Debit Card Issuance
        public async Task<string> ABLDebitCardIssuance(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "ABLDebitCardIssuance";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                Logs.WriteLogEntry("info", KioskId, "ABLDebitCardIssuance Step 1: Validating Input Data" + request.ToString(), _MethodName);
                string CompanyCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CompanyCode")?.Value ?? string.Empty;
                string AccountNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountNumber")?.Value ?? string.Empty;
                string ProdCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("ProdCode")?.Value ?? string.Empty;
                string DpsScheme = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("DpsScheme")?.Value ?? string.Empty;
                string CardGenerationType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardGenerationType")?.Value ?? string.Empty;
                string UpdateType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("UpdateType")?.Value ?? string.Empty;

                string AtmReqType = "";
                if ((CardGenerationType == "Fresh") || (CardGenerationType == "Upgrade" && UpdateType == "0"))
                {
                    AtmReqType = "1";

                }
                else if (CardGenerationType == "Replace")
                {
                    AtmReqType = "2";

                }
                else if (CardGenerationType == "Upgrade" && UpdateType == "1")
                {
                    AtmReqType = "5";
                }


                string TransactionId = GenerateTransactionId();
                DateTime dateTime = DateTime.Now;
                string formattedDate = dateTime.ToString("dd-MM-yyyy HH:mm:ss");
                string url = T24Url + ConfigurationManager.AppSettings["ABLDebitCardIssuance"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                bool flag = await AtmMarkYesForExistingCustomer(AccountNumber, CompanyCode, formattedDate, KioskId);

                if (flag)
                {
                    var requestPayload = new
                    {
                        ABLDebitCardIssuanceReq = new
                        {
                            UserID = "XXXXX",
                            Password = "XXXXX",
                            ChannelType = "WEB",
                            ChannelSubType = "SSK",
                            TransactionType = "000",
                            TransactionSubType = "000",
                            TranDateAndTime = formattedDate,
                            Function = "DebitCardIssuance",
                            HostData = new
                            {
                                TransReferenceNo = TransactionId,
                                Company = CompanyCode,
                                TransactionId = AccountNumber,
                                Status = "20",
                                PackageType = ProdCode,
                                AtmReqType = AtmReqType,
                                DPS_Scheme = DpsScheme,
                                CustomerNature = "ETB",
                                AddressFlag = "NO",
                                DaoAtmAddr1 = "",
                                DaoAtmAddr2 = "",
                                DaoAtmAddr3 = "",
                                DaoAtmAddr4 = "",
                                DaoAtmAddr5 = ""
                            }
                        }
                    };

                    Logs.WriteLogEntry("info", KioskId, "Request Payload 1: " + JsonConvert.SerializeObject(requestPayload), _MethodName);

                    APIResponse aPIResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, KioskId, "");
                    // string aa = "{\r\n  \"ABLDebitCardIssuanceRsp\": {\r\n    \"StatusCode\": \"1000\",\r\n    \"StatusDesc\": \"Success\",\r\n    \"STAN\": \"90e1ebfa-4772-11f0-844e-0ae0141b0000\",\r\n    \"HostData\": {\r\n      \"TransReferenceNo\": \"250612145617\",\r\n      \"HostCode\": \"00\",\r\n      \"HostDesc\": \"Success\",\r\n      \"field\": [\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"CUSTOMER\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"2706670\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"DATE.REQUEST\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"20220305\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"ACT.TITLE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"MY ACCOUNT\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"STATUS\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"20\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"ATM.REQ.TYPE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"1\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"NAME.ON.ATM\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"TEST NAME\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"SHORT.NAME\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"SHORT NAME\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"BIRTH.DATE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"20010101\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"MOTHER.NAME\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"MOM NAME\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"LGL.DOC.NAM\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"ID-N\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"LGL.DOC.ID\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"3520083065479\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"GENDER\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"MALE\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"NATIONALITY\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"Single\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"NATIONALITY.1\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"PK\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"POST.CODE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"12345\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"CUST.EMAIL\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"BANK@EXAMPLE.COM\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"ACCOUNT.NATURE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"SINGLE\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"OPERATING.INSTRUCTIONS\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"Singly\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"PACKAGE.TYPE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"20\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"HUSBAND.NAME\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"19500.0000000000000000000000000000000000\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"DPS.SCHEME\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"2-Frequent Online Shopping\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"ADDRESS.FLAG\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"NO\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"CUSTOMER.NATURE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"ETB\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"CURR.NO\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"1\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"INPUTTER\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"36743_CIBOFSML.1_I_INAU_OFS_OFSML\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"DATE.TIME\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"2506121456\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"DATE.TIME\",\r\n          \"mv\": \"2\",\r\n          \"content\": \"2506121456\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"AUTHORISER\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"36743_CIBOFSML.1_OFS_OFSML\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"CO.CODE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"PK0010722\"\r\n        },\r\n        {\r\n          \"sv\": \"1\",\r\n          \"name\": \"DEPT.CODE\",\r\n          \"mv\": \"1\",\r\n          \"content\": \"1\"\r\n        }\r\n      ]\r\n    }\r\n  }\r\n}";
                    if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseData = JsonConvert.DeserializeObject<dynamic>(aPIResponse.ResponseContent);
                        var debitCardResponse = responseData?.ABLDebitCardIssuanceRsp;
                        Logs.WriteLogEntry("info", KioskId, "hostCode Data: " + responseData, _MethodName);

                        string hostCode = responseData?.ABLDebitCardIssuanceRsp?.HostData?.HostCode;
                        var hostDesc = responseData?.ABLDebitCardIssuanceRsp?.HostData?.HostDesc;

                        if (hostCode == "00")
                        {
                            Logs.WriteLogEntry("info", KioskId, "Host Code: " + debitCardResponse.HostData, _MethodName);
                            Logs.WriteLogEntry("info", KioskId, "Host Description: " + debitCardResponse.StatusDesc, _MethodName);
                            Logs.WriteLogEntry("info", KioskId, "Transaction Reference No: " + debitCardResponse.HostData.TransReferenceNo, _MethodName);
                            Logs.WriteLogEntry("info", KioskId, "Transaction Reference No: " + debitCardResponse.HostData.HostCode, _MethodName);
                            Logs.WriteLogEntry("info", KioskId, "Transaction Reference No: " + debitCardResponse.HostData.HostDesc, _MethodName);

                            // Declare variables outside the loop
                            string MotherName = "";
                            string FatherName = "";
                            string CustomerType = "";
                            string AccountType = "";
                            string CurrencyCode = "";
                            string BranchCode = "";
                            string DefaultAccount = "";
                            string AccountStatus = "";
                            string BankIMD = "";
                            string Email = "";
                            string Nationality = "";

                            foreach (var item in debitCardResponse.HostData.field)
                            {
                                Logs.WriteLogEntry("info", KioskId, "Host Code 3: " + item.name + " - " + item.content, _MethodName);

                                // Assign values based on item name
                                if (item.name == "MOTHER.NAME") MotherName = item.content;
                                if (item.name == "HUSBAND.NAME") FatherName = item.content;
                                if (item.name == "CUSTOMER.NATURE") CustomerType = item.content;
                                if (item.name == "ACCOUNT.NATURE") AccountType = item.content;
                                if (item.name == "CURR.NO") CurrencyCode = item.content;
                                if (item.name == "CO.CODE") BranchCode = item.content;
                                if (item.name == "DEFAULT.ACCOUNT") DefaultAccount = item.content;
                                if (item.name == "STATUS") AccountStatus = item.content;
                                if (item.name == "BANK.IMD") BankIMD = item.content;
                                if (item.name == "CUST.EMAIL") Email = item.conte41nt;
                                if (item.name == "NATIONALITY") Nationality = item.content;
                            }

                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                            bodyElement.Add(
                                new XElement("RespMessage", APIResultCodes.Success),
                                new XElement("MotherName", MotherName),
                                new XElement("FatherName", FatherName),
                                new XElement("CustomerType", CustomerType),
                                new XElement("AccountType", AccountType),
                                new XElement("CurrencyCode", CurrencyCode),
                                new XElement("BranchCode", BranchCode),
                                new XElement("DefaultAccount", DefaultAccount),
                                new XElement("AccountStatus", AccountStatus),
                                new XElement("BankIMD", BankIMD),
                                new XElement("Email", Email),
                                new XElement("Nationality", Nationality)
                            );
                        }
                        else
                        {
                            string errorMessage = ExtractErrorMessage(responseData, KioskId);
                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", ""));
                            if (errorMessage == "Customer do not meet Basic Eligibility Criteria, Please select Other Criteria.")
                            {
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "DoNotMeetCriteria"));
                            }
                            else
                            {
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", errorMessage));
                            }
                        }
                    }
                    else
                    {
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                    }
                }
                else
                {
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"Exception in CustomerVerification: {ex.Message}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();
        }

        #endregion
      
        #region AtmMarkYesForExistingCustomer

        public async Task<bool> AtmMarkYesForExistingCustomer(string accountNumber, string BranchCode, string formattedDate, string kioskId)
        {
            APIHelper apiService = new APIHelper();
            string methodName = "AtmMarkYesForExistingCustomer";
            bool flag = false;
            try
            {
                string TransactionId = GenerateTransactionId();
                string url = T24Url + ConfigurationManager.AppSettings["ABLAtmFlagUpdate"].ToString();
                Logs.WriteLogEntry("Info", kioskId, $"{methodName} [URL]:  {url}", methodName);

                var requestPayload = new
                {
                    ABLAtmFlagUpdateReq = new
                    {
                        UserID = "XXXXX",
                        Password = "XXXXX",
                        ChannelType = "WEB",
                        ChannelSubType = "SSK",
                        TransactionType = "000",
                        TransactionSubType = "000",
                        TranDateAndTime = formattedDate,
                        Function = "ABL_ATM_FLAG_UPDATE",
                        HostData = new
                        {
                            TransReferenceNo = TransactionId,
                            Company = BranchCode,
                            TransactionId = accountNumber,
                        }
                    }
                };


                Logs.WriteLogEntry("info", kioskId, "API Request : " + requestPayload.ToString(), methodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestPayload, kioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry("info", kioskId, "API Call Successful! " + apiResponse.Message, methodName);
                    flag = true;
                }
                else
                {
                    Logs.WriteLogEntry("Error", kioskId, $"API Call Failed. Status Code: {apiResponse.StatusCode}", methodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", kioskId, "Error while deleting application: " + ex, methodName);
            }

            return flag;
        }
        #endregion

        #region IRIS APIs

        #region IRIS Existing Card List 
        public async Task<string> IRISExistingCardList(XDocument request, string RefrenceNumber)
        {
            const string _MethodName = "IRISExistingCardList";
            XDocument response = request.GetBasicResponseFromRequest();
            string KioskId = request.Element(TransactionTags.Request)?.Element(TransactionTags.Header)?.Element(TransactionTags.KioskIdentity)?.Value;

            try
            {
                string CnicNumber = (request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("cnic")?.Value ?? "").Replace("-", "");
                string ProductCode = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("ProductCode")?.Value ?? string.Empty;
                string accountNumber = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("accountNumber")?.Value ?? string.Empty;
                string branchCode = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("branchCode")?.Value ?? string.Empty;

                string finalAccountNumber = branchCode.Length >= 4 ? branchCode.Substring(branchCode.Length - 4) + accountNumber : accountNumber;
                string url = IrisUrl + ConfigurationManager.AppSettings["IRISExistingCardList"];

                Logs.WriteLogEntry("Info", KioskId, $"Request URL: {url}", _MethodName);
                Logs.WriteLogEntry("Info", KioskId, $"Request XML: {request}", _MethodName);

                wsABLCARDSTATUSCHANGE webService = new wsABLCARDSTATUSCHANGE { Url = url };
                var result = webService.CardListing(CnicNumber);
                string innerXml = XMLHelper.ExtractInnerXml(result);
                string cleanedXml = XMLHelper.FixNestedCardInfo(innerXml);

                Logs.WriteLogEntry("Info", KioskId, $"Cleaned XML: {cleanedXml}", _MethodName);

                var responseObject = XMLHelper.DeserializeXml<Root>(cleanedXml);
                var bodyElement = response.Element(TransactionTags.Response)?.Element(TransactionTags.Body);
                string CardGenerationType = "", CardNumber = "", CardExpiryDate = "", AccountId = "", CardName = "", ProductDescription = "", CardStatus = "";
                string UpdateType = "0";
                bool CardFoundForReplace = false, CardFoundButFreshCard = false, Flag = false;

                if (responseObject?.Output?.Cards != null && responseObject.WebMethodResponse?.ResponseDescription == "Approved")
                {
                    var allCards = responseObject.Output.Cards;
                    var matchedCards = allCards.Where(c => c.CARDSTATUS != "02" && c.PRODUCTCODE != "0098").ToList();

                    Logs.WriteLogEntry("Info", KioskId, $"Total non-blocked cards found: {matchedCards.Count}", _MethodName);

                    if (matchedCards.Any())
                    {
                        Card FreshCardList = await FreshCardListing(CnicNumber, finalAccountNumber, KioskId);
                        //   var freshCards = matchedCards.Where(c => c.ACCOUNTID == finalAccountNumber && c.CARDSTATUS == "03").ToList();
                        if (FreshCardList != null)
                        {
                            CardFoundButFreshCard = true;
                            Logs.WriteLogEntry("Info", KioskId, $"Fresh card found for ProductCode: {FreshCardList.PRODUCTCODE} and Account Number : {FreshCardList.ACCOUNTID}  ", _MethodName);
                        }
                        else
                        {
                            if (matchedCards.Any(c => c.ACCOUNTID == finalAccountNumber)) UpdateType = "1";
                            var relevantCards = matchedCards.Where(c => c.PRODUCTCODE == ProductCode && c.ACCOUNTID == finalAccountNumber).ToList();
                            if (relevantCards.Any())
                            {
                                foreach (var card in relevantCards)
                                {
                                    Logs.WriteLogEntry("Info", KioskId, $"Card check - Status: {card.CARDSTATUS}, ProductCode: {card.PRODUCTCODE}", _MethodName);
                                    if (card.CARDSTATUS == "00" || card.CARDSTATUS == "01")
                                    {
                                        CardGenerationType = "Replace";
                                        CardNumber = card.CARDNUMBER;
                                        CardExpiryDate = card.CARDEXPIRYDATE;
                                        AccountId = card.ACCOUNTID;
                                        CardName = card.CARDNAME;
                                        ProductDescription = card.PRODUCTDESCRIPTION;
                                        CardStatus = card.CARDSTATUS;
                                        CardFoundForReplace = true;

                                        Logs.WriteLogEntry("Info", KioskId, $"Replace card found: {CardNumber}", _MethodName);
                                        break;
                                    }
                                }
                                if (!CardFoundForReplace)
                                {
                                    CardGenerationType = "Fresh";
                                    Logs.WriteLogEntry("Info", KioskId, $"No active card found, marked as Fresh", _MethodName);
                                }
                            }
                            else
                            {
                                CardGenerationType = "Upgrade";
                                Logs.WriteLogEntry("Info", KioskId, $"No matching product code card found, marked as Upgrade", _MethodName);
                            }
                        }
                    }
                    else
                    {
                        if (allCards.Any(c => c.ACCOUNTID == finalAccountNumber)) UpdateType = "1";

                        var blockCards = allCards
                        .Where(c => c.CARDSTATUS == "02" && c.PRODUCTCODE != "0098").ToList();

                        Logs.WriteLogEntry("Info", KioskId, $"Total blocked cards found : {blockCards.Count}", _MethodName);

                        if (blockCards.Any())
                        {
                            var maxExpiry = blockCards.Max(c => c.CARDEXPIRYDATE);
                            Logs.WriteLogEntry("Info", KioskId, $"Max expiry date among blocked cards: {maxExpiry}", _MethodName);

                            var expiryCard = blockCards.FirstOrDefault(c => c.CARDEXPIRYDATE == maxExpiry);
                            if (expiryCard != null)
                            {
                                Logs.WriteLogEntry("Info", KioskId, $"Blocked card found with max expiry: {expiryCard.CARDNUMBER}", _MethodName);

                                CardNumber = expiryCard.CARDNUMBER;
                                CardExpiryDate = expiryCard.CARDEXPIRYDATE;
                                AccountId = expiryCard.ACCOUNTID;
                                CardName = expiryCard.CARDNAME;
                                ProductDescription = expiryCard.PRODUCTDESCRIPTION;
                                CardGenerationType = expiryCard.PRODUCTCODE == ProductCode ? "Replace" : "Upgrade";

                            }
                            else
                            {
                                Logs.WriteLogEntry("Warning", KioskId, $"No card found with the max expiry date", _MethodName);
                            }
                        }
                        else
                        {
                            Logs.WriteLogEntry("Info", KioskId, $"No blocked cards found", _MethodName);
                        }



                    }
                    if (CardFoundButFreshCard && !CardFoundForReplace)
                    {
                        Logs.WriteLogEntry("info", KioskId, $"Card Found But Not For Replace: Card Number: {CardNumber}, Status: {CardStatus}, AccountNumber: {AccountId}", _MethodName);
                        Flag = true;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                        bodyElement.Add(
                            new XElement("MessageHead", "Card Replace Failed !"),
                            new XElement("Message", "FreshCardNotAllowed"));
                    }
                    else if (!CardFoundForReplace && !CardFoundButFreshCard)
                    {
                        CardGenerationType = "Upgrade";
                        Logs.WriteLogEntry("info", KioskId, $"This is an {CardGenerationType} Card: {ProductCode}", _MethodName);
                    }
                    if (!Flag)
                    {
                        bodyElement.Add(
                        new XElement("RespMessage", APIResultCodes.Success),
                        new XElement("CardGenerationType", CardGenerationType),
                        new XElement("UpdateType", UpdateType),
                        new XElement("CardNumber", CardNumber),
                        new XElement("CardExpiryDate", CardExpiryDate),
                        new XElement("AccountId", AccountId),
                        new XElement("DefaultAccount", AccountId),
                        new XElement("CardName", CardName),
                        new XElement("ProductDescription", ProductDescription),
                        new XElement("CardProductCode", ProductCode));

                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "IRIS CardList Response Received";
                    }
                }
                else if (responseObject?.WebMethodResponse?.ResponseDescription == "Invalid CNIC")
                {
                    CardGenerationType = "Fresh";
                    Logs.WriteLogEntry("Info", KioskId, $"Invalid CNIC — defaulting to Fresh card", _MethodName);

                    bodyElement.Add(
                        new XElement("RespMessage", APIResultCodes.Success),
                        new XElement("CardGenerationType", CardGenerationType),
                        new XElement("UpdateType", UpdateType),
                        new XElement("CardNumber", CardNumber),
                        new XElement("CardExpiryDate", CardExpiryDate),
                        new XElement("AccountId", AccountId),
                        new XElement("DefaultAccount", AccountId),
                        new XElement("CardName", CardName),
                        new XElement("ProductDescription", ProductDescription),
                        new XElement("CardProductCode", ProductCode));

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "IRIS CardList Response Received";
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "IRIS card list fetch failed", _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    bodyElement.Add(new XElement("Message", "UnableToProcessRequest"));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"{_MethodName} Exception: {ex}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }

            return response.ToString();
        }


        #endregion

        #region IRIS Card Issuance
        public async Task<string> CardIssuance(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "CardIssuance";
            XDocument response = request.GetBasicResponseFromRequest();
            APIResponse accountApiiResponse = null;
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {

                string kioskID = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element("KioskIdentity").Value;
                Logs.WriteLogEntry("info", KioskId, "KIOSK ID: " + kioskID, _MethodName);

                string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
                Logs.WriteLogEntry("info", KioskId, "PC NAME: " + PcName, _MethodName);

                string[] parts = PcName.Split('|');

                string ComputerName = parts[0].Trim();
                string BranchCode = parts[1].Trim();

                Console.WriteLine($"Computer Name: {ComputerName}");
                Console.WriteLine($"Branch Code: {BranchCode}");


                Logs.WriteLogEntry("info", KioskId, "IRISCardIssuance Step 1: " + request.ToString(), _MethodName);
                string CardGenerationType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardGenerationType")?.Value ?? string.Empty;
                string IrisCardNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("IrisCardNumber")?.Value ?? string.Empty;
                string FullName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("FullName")?.Value ?? string.Empty;
                string MotherName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("MotherName")?.Value ?? string.Empty;
                string MobileNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("MobileNumber")?.Value ?? string.Empty;
                string FatherName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("FatherName")?.Value ?? string.Empty;
                string CardName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardName")?.Value ?? string.Empty;
                string CustomerType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CustomerType")?.Value ?? string.Empty;
                string ProductCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("ProductCode")?.Value ?? string.Empty;
                string AccountNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountNumber")?.Value ?? string.Empty;
                // string AccountType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountType")?.Value ?? string.Empty;
                string CurrencyCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CurrencyCode")?.Value ?? string.Empty;
                string AccountTitle = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountTitle")?.Value ?? string.Empty;
                string BranceCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("BranceCode")?.Value ?? string.Empty;
                string DefaultAccount = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("DefaultAccount")?.Value ?? string.Empty;
                string AccountStatus = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountStatus")?.Value ?? string.Empty;
                string CNIC = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CNIC")?.Value ?? string.Empty;
                string DOB = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("DOB")?.Value ?? string.Empty;
                string Email = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("Email")?.Value ?? string.Empty;
                string Nationality = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("Nationality")?.Value ?? string.Empty;
                string AccountCategory = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountCategory")?.Value ?? string.Empty;
                string SelectedCardName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("SelectedCardName")?.Value ?? string.Empty;

                CNIC = CNIC.Replace("-", "");


                string lastFourDigits = BranceCode.Substring(BranceCode.Length - 4);
                string finalAccountNumber = lastFourDigits + AccountNumber;

                int AccountCategoryCode = int.Parse(AccountCategory);
                int from = 1000;
                int to = 3015;
                string AccountType = "";
                if (AccountCategoryCode >= from && AccountCategoryCode <= to)
                {
                    AccountType = "20";
                }
                else
                {
                    AccountType = "10";
                }

                Logs.WriteLogEntry("info", KioskId, "CardIssuance AccountType : " + AccountType, _MethodName);

                string BankIMD = "";

                switch (ProductCode)
                {
                    case "0092":
                        BankIMD = "428638";
                        break;
                    case "0071":
                        BankIMD = "407572";
                        break;
                    case "0070":
                        BankIMD = "476215";
                        break;
                    case "0075":
                        BankIMD = "476215";
                        break;
                    case "0080":
                        BankIMD = "629240";
                        break;
                }

                string TrakingId = GenerateTransactionId();
                int? isoCode = GetIsoCode(CurrencyCode);
                Logs.WriteLogEntry("info", KioskId, "ISO Code Found Against Currency Code: " + isoCode, _MethodName);
                string finaCurrenctCode = Convert.ToString(isoCode).ToString();


                string ActivationDate = DateTime.Now.ToString("yyyyMMdd");
                Logs.WriteLogEntry("info", KioskId, "CardIssuance ActivationDate : " + ActivationDate, _MethodName);

                string ActionCode = "";
                string FinalCardNumber = "";

                if (CardGenerationType == "Upgrade")
                {
                    ActionCode = "A";
                }
                else if (CardGenerationType == "Replace")
                {
                    ActionCode = "R";
                    FinalCardNumber = IrisCardNumber;
                }
                else if (CardGenerationType == "Fresh")
                {
                    ActionCode = "A";
                }

                string URL = IrisUrl + ConfigurationManager.AppSettings["IRISCardIssuance"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]: {URL}", _MethodName);
                InstantCard webService = new InstantCard();
                webService.Url = URL;

                string requestLog = $@"
                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
                        <soapenv:Header/>
                        <soapenv:Body>
                        <tem:ImportCustomer>
                        <tem:ActionCode>{ActionCode}</tem:ActionCode>
                        <tem:CNIC>{CNIC}</tem:CNIC>
                        <tem:TrackingID>{TrakingId}</tem:TrackingID>
                        <tem:FullName>{FullName}</tem:FullName>
                        <tem:DateOfBirth>{DOB}</tem:DateOfBirth>
                        <tem:MothersName>{MotherName}</tem:MothersName>
                        <tem:BillingFlag>H</tem:BillingFlag>
                        <tem:MobileNumber>{MobileNumber}</tem:MobileNumber>
                        <tem:ActivationDate>{ActivationDate}</tem:ActivationDate>
                        <tem:FathersName>Test</tem:FathersName>
                        <tem:CardName>{FullName}</tem:CardName>
                        <tem:CustomerType>1</tem:CustomerType>
                        <tem:ProductCode>{ProductCode}</tem:ProductCode>
                        <tem:AccountNo>{finalAccountNumber}</tem:AccountNo>
                        <tem:AccountType>{AccountType}</tem:AccountType>
                        <tem:AccountCurrency>{finaCurrenctCode}</tem:AccountCurrency>
                        <tem:AccountStatus>00</tem:AccountStatus>
                        <tem:AccountTitle>{AccountTitle}</tem:AccountTitle>
                        <tem:BankIMD>{BankIMD}</tem:BankIMD>
                        <tem:Branchcode>{BranchCode}</tem:Branchcode>
                        <tem:DefaultAccount>1</tem:DefaultAccount>
                        <tem:Title></tem:Title>
                        <tem:HomeAddress1></tem:HomeAddress1>
                        <tem:HomeAddress2></tem:HomeAddress2>
                        <tem:HomeAddress3></tem:HomeAddress3>
                        <tem:HomeAddress4></tem:HomeAddress4>
                        <tem:HomePostalCode></tem:HomePostalCode>
                        <tem:HomePhone></tem:HomePhone>
                        <tem:Email></tem:Email>
                        <tem:Company>Allied Bank Ltd</tem:Company>
                        <tem:OfficeAddress1></tem:OfficeAddress1>
                        <tem:OfficeAddress2></tem:OfficeAddress2>
                        <tem:OfficeAddress3></tem:OfficeAddress3>
                        <tem:OfficeAddress4></tem:OfficeAddress4>
                        <tem:OfficeAddress5></tem:OfficeAddress5>
                        <tem:OfficePhone></tem:OfficePhone>
                        <tem:PassportNo></tem:PassportNo>
                        <tem:Nationality>{Nationality}</tem:Nationality>
                        <tem:OldCardNumber>{FinalCardNumber}</tem:OldCardNumber>
                        </tem:ImportCustomer>
                        </soapenv:Body>
                     </soapenv:Envelope>";

                Logs.WriteLogEntry("info", KioskId, requestLog, _MethodName);


                Logs.WriteLogEntry("info", KioskId, "CardIssuance URL : " + webService.Url, _MethodName);

                string result = webService.ImportCustomer(
                     ActionCode: ActionCode,
                     CNIC: CNIC,
                     TrackingID: TrakingId,
                     FullName: FullName,
                     DateOfBirth: DOB,
                     MothersName: MotherName,
                     BillingFlag: "H",
                     MobileNumber: MobileNumber,
                     ActivationDate: ActivationDate,
                     FathersName: "Test",
                     CardName: CardName,
                     CustomerType: "1",
                     ProductCode: ProductCode,
                     AccountNo: finalAccountNumber,
                     AccountType: AccountType,
                     AccountCurrency: finaCurrenctCode,
                     AccountStatus: "00",
                     AccountTitle: AccountTitle,
                     BankIMD: BankIMD,
                     Branchcode: BranchCode,
                     DefaultAccount: "1",
                     Title: "",
                     Prefered_Address_FLag: "",
                     HomeAddress1: "",
                     HomeAddress2: "",
                     HomeAddress3: "",
                     HomeAddress4: "",
                     HomePostalCode: "",
                     HomePhone: "",
                     Email: Email,
                     Company: "Allied Bank Ltd",
                     OfficeAddress1: "",
                     OfficeAddress2: "",
                     OfficeAddress3: "",
                     OfficeAddress4: "",
                     OfficeAddress5: "",
                     OfficePostalCode: "",
                     OfficePhone: "",
                     PassportNo: "",
                     Nationality: "",
                     OldCardNumber: FinalCardNumber


                );


                XDocument doc = XDocument.Parse(result);
                string trackingID = doc.Root.Element("WebMethodResponse").Element("TrackingID")?.Value;
                string responseCode = doc.Root.Element("WebMethodResponse").Element("ResponseCode")?.Value;
                string responseDescription = doc.Root.Element("WebMethodResponse").Element("ResponseDescription")?.Value;

                Logs.WriteLogEntry("info", KioskId, "CardIssuance API Response responseCode : " + responseCode, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "CardIssuance API Response trackingID : " + trackingID, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "CardIssuance API Response responseDescription : " + responseDescription, _MethodName);

                if (responseDescription == "Success" && responseCode == "00")
                {
                    Logs.WriteLogEntry("info", KioskId, "CardIssuance API Response Description is Success", _MethodName);

                    CardInfo cardInfo = DecryptEmbossingFile(BranchCode, ProductCode, KioskId);

                    if (cardInfo != null)
                    {
                        Logs.WriteLogEntry("info", KioskId, cardInfo.CardHolderName, _MethodName);

                        string Description = "";
                        HardwareResponse hardwareResponse = CardPersonalization(cardInfo, ComputerName, SelectedCardName, out Description, kioskID);
                        if (hardwareResponse.data.ToString() != "" && hardwareResponse.data != null)
                        {
                            Logs.WriteLogEntry("Info", KioskId, "Personlization Response : " + hardwareResponse.description, _MethodName);
                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                            bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success),
                                new XElement("RequestId", hardwareResponse.data));

                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "IRIS Request Successfuly Send";
                        }
                        else
                        {
                            Logs.WriteLogEntry("Error", KioskId, "Data is Null  " + hardwareResponse.description, _MethodName);
                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                            //response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", "Something Went Wrong !"));
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = hardwareResponse.description;
                        }

                    }
                    else
                    {
                        Logs.WriteLogEntry("Error", KioskId, "cardInfo", _MethodName);
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                        //response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", "Something Went Wrong !"));
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    }

                    //var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    ////response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", "Card Request Submited !"));
                    //response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Dear Customer Your Debit Card Request has been processed successfully."));
                    //response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    //response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;


                }
                else
                {
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }



            }
            catch (ArgumentNullException argEx)
            {
                Logs.WriteLogEntry("Error", KioskId, "ArgumentNullException in CardIssuance: " + argEx, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            catch (InvalidOperationException invOpEx)
            {
                Logs.WriteLogEntry("Error", KioskId, "InvalidOperationException in CardIssuance: " + invOpEx, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "General Exception in CardIssuance: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();
        }

        #endregion

        #endregion

        #region Send Sms
        public async Task<string> SendOTP(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "SendOTP";
            XDocument response = request.GetBasicResponseFromRequest();
            smpp_ws_sendsms service = new smpp_ws_sendsms();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                string mobileNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("mobileNumber")?.Value ?? string.Empty; ;

                Logs.WriteLogEntry("info", KioskId, "Final Request" + request.ToString(), _MethodName);

                Random random = new Random();
                int otp = random.Next(100000, 999999);
                //int otp = 111111;


                string url = ConfigurationManager.AppSettings["SendOtp"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                string message = $"Your OTP for verification is: {otp}. Please enter this code to proceed.";

                Logs.WriteLogEntry("info", KioskId, "Going to send otp sms", _MethodName);

                var serviceResponse = service.QueueSMS("SSK", mobileNumber, message, "3");

                if (serviceResponse != null)
                {
                    Logs.WriteLogEntry("info", KioskId, "SendOTP Response: " + serviceResponse, _MethodName);

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(
                      new XElement("OTP", otp));
                }
                else
                {
                    Logs.WriteLogEntry("error", KioskId, "Failed to send OTP. Response: " + serviceResponse, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "OtpSendFailed"));
                }

            }
            catch (Exception ex)
            {
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }

            return response.ToString();
        }
        #endregion

        #region Get Transiction Id
        private static string GenerateTransactionId()
        {
            try
            {
                DateTime now = DateTime.Now;
                string dateTimeNow = now.ToString("yyMMddHHmm");
                string randomDigits = new Random().Next(10, 99).ToString();

                string transactionId = $"{dateTimeNow}{randomDigits}";
                Console.WriteLine($"Transaction ID: {transactionId}");
                return transactionId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Get Cards Formats
        private List<CardFormats> GetCardFormats(string computerName, string KioskId)
        {
            string MethodName = "GetCardFormats";

            SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();
            List<CardFormats> cardFormatList = new List<CardFormats>();

            if (!string.IsNullOrEmpty(computerName))
            {

                Logs.WriteLogEntry("info", $"Going to get card formats with kiosk " + computerName + "", KioskId, MethodName);

                HardwareResponse getCardFormats = deviceOperations.GetCardFormats(computerName);

                Logs.WriteLogEntry("info", KioskId, "GetCardFormats Response Code: " + getCardFormats.code + "Description :" + getCardFormats.description + "Data : " + getCardFormats.data, MethodName);
                if (getCardFormats.code == 0)
                {
                    string jsonCardFormat = getCardFormats.data.ToString();

                    Logs.WriteLogEntry("info", KioskId, " Card Format: " + jsonCardFormat, MethodName);
                    cardFormatList = JsonConvert.DeserializeObject<List<CardFormats>>(jsonCardFormat);
                    Logs.WriteLogEntry("info", KioskId, " Card Format Deserialized : " + cardFormatList, MethodName);
                }
                else
                {
                    Logs.WriteLogEntry("info", KioskId, " Card Formats Data Not Available", MethodName);
                }
            }
            else
            {
                Logs.WriteLogEntry("info", KioskId, "Computer Name Not Available", MethodName);
            }
            return cardFormatList;
        }

        #endregion

        #region CardPersonalization
        private HardwareResponse CardPersonalization(CardInfo cardInfo, string ComputerName, string CardName, out string Description, string KioskId)
        {
            string _MethodName = "CardPersonalization";
            string Data = "";
            SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();
            Description = "";
            HardwareResponse hardwareResponse = new HardwareResponse();
            try
            {
                if (ComputerName != null)
                {
                    Logs.WriteLogEntry("info", KioskId, "Adding Data for Card Personalization" + "", _MethodName);

                    List<DataItem> dataItems = new List<DataItem>
                      {
                        new DataItem { name = "@PAN@", value = cardInfo.PAN.Replace(" ", "") ?? "" },
                        new DataItem { name = "@Expiry@", value = cardInfo.Expiry ?? "" },
                        new DataItem { name = "@CardHolderName@", value = cardInfo.CardHolderName ?? "" },
                        new DataItem { name = "@CVV2@", value = cardInfo.CVV2 ?? "" },
                        new DataItem { name = "@iCVV@", value = cardInfo.ICVV ?? "" },
                        new DataItem { name = "@Track1@", value = cardInfo.Track1 ?? "" },
                        new DataItem { name = "@Track2@", value = cardInfo.Track2 ?? "" },
                        new DataItem { name = "@CVV@", value = cardInfo.CVV1 ?? "" },
                        new DataItem { name = "@MemberSince@", value = cardInfo.MemberSince ?? "" }
                      };

                    CardPersonalizationRequest personalizationRequest = new CardPersonalizationRequest { dataItems = dataItems };

                    Logs.WriteLogEntry("info", KioskId, "Going to send data for card personlization request" + "", _MethodName);

                    var json = JsonConvert.SerializeObject(personalizationRequest);
                    Logs.WriteLogEntry("info", KioskId, "Personlization Request : " + json, _MethodName);
                    hardwareResponse = deviceOperations.StartCardPersonalization(ComputerName, CardName, personalizationRequest);


                    Logs.WriteLogEntry("info", KioskId, $"{hardwareResponse?.data?.ToString() + "|" + hardwareResponse?.code + "|" + hardwareResponse?.description?.ToString()}" + "", _MethodName);

                    if (hardwareResponse.code == 0 || hardwareResponse.data != null)
                    {
                        Data = hardwareResponse.data.ToString();
                        Logs.WriteLogEntry("error", KioskId, "Response: Data Found : " + Data, _MethodName);
                    }
                    else
                    {
                        Logs.WriteLogEntry("error", KioskId, "Response: Data is Null", _MethodName);
                        Logs.WriteLogEntry("error", KioskId, "Response: Status and Description" + hardwareResponse.code + "-" + hardwareResponse.description, _MethodName);
                        Description = hardwareResponse.description;
                    }
                }
                else
                {
                    Logs.WriteLogEntry("error", KioskId, "Response: Computer name is null", _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error in Personlization!: " + ex, _MethodName);

            }
            return hardwareResponse;
        }
        #endregion

        #region Card Status
        public async Task<string> GetCardStatus(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "GetCardStatus";
            XDocument response = request.GetBasicResponseFromRequest();
            SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                string kioskID = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element("KioskIdentity").Value;
                Logs.WriteLogEntry("info", KioskId, "KIOSK ID: " + kioskID, _MethodName);

                string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
                Logs.WriteLogEntry("info", KioskId, "PC NAME: " + PcName, _MethodName);

                string RequestId = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("RequestId")?.Value ?? string.Empty;
                Logs.WriteLogEntry("info", KioskId, KioskId, _MethodName);

                if (RequestId != null)
                {
                    Logs.WriteLogEntry("info", KioskId, "Going to check Card Status" + "", _MethodName);

                    HardwareResponse hardwareResponse = deviceOperations.GetPersonalizationRequestStatus(RequestId, 3, 15);
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    Logs.WriteLogEntry("info", KioskId, "data " + hardwareResponse.data.ToString(), _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "code " + hardwareResponse.code, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "description " + hardwareResponse.description, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "description " + hardwareResponse, _MethodName);

                    switch (hardwareResponse.data.ToString())
                    {
                        case "Success":

                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                            bodyElement.Add(
                                 new XElement("RespMessage", APIResultCodes.Success)
                            );
                            break;
                        case "Failed":

                            Logs.WriteLogEntry("error", KioskId, "Response: Failed to Get Card Status!", _MethodName);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", hardwareResponse.description));
                            break;

                        case "AtExit":
                            Logs.WriteLogEntry("error", KioskId, "Response: AtExit to Get Card Status!", _MethodName);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", hardwareResponse.description));
                            break;
                        case "Processing":
                            Logs.WriteLogEntry("Processing", KioskId, "Response: Processing!", _MethodName);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Processing"));
                            break;
                        default:
                            break;
                    }
                }
                else
                {

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error in Failed to Get Card Status!: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

            }

            return response.ToString();
        }

        #endregion

        #region Account Opening

        #region Send OTP 

        public async Task<string> SendOtpAsanAccount(XDocument request, string RefrenceNumber)
        {

            string _MethodName = "SendOtpAsanAccount";
            APIHelper apiService = new APIHelper();
            XDocument response = request.GetBasicResponseFromRequest();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                string mobileNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("mobileNumber")?.Value ?? string.Empty;
                string cnicNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cnicNumber")?.Value ?? string.Empty;
                cnicNumber = cnicNumber.Replace("-", "");
                Logs.WriteLogEntry("info", KioskId, "Request : " + request.ToString(), _MethodName);

                string url = MyPdaUrl + ConfigurationManager.AppSettings["SendOtpPda"].ToString();



                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);


                var requestData = new
                {
                    data = new
                    {
                        mobileNo = mobileNumber,
                        idNumber = cnicNumber,
                        generateOtp = true,
                        latitude = 31.4867712,
                        longitude = 74.3276544,
                        customerTypeId = 106501
                    }
                };

                Logs.WriteLogEntry("info", KioskId, "API Request : " + JsonConvert.SerializeObject(requestData.ToString()), _MethodName);

                var aPIResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    OTPResponse otpResponse = JsonConvert.DeserializeObject<OTPResponse>(aPIResponse.ResponseContent);
                    Logs.WriteLogEntry("info", KioskId, "API Call Successful!" + aPIResponse.Message, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "API Call Failed. Status Code: " + aPIResponse.StatusCode, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error in Failed to SendOtpAsanAccount!: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();

        }


        #endregion

        #region Delete Application
        public async Task<string> DeleteApplication(XDocument request, string RefrenceNumber)

        {

            string _MethodName = "DeleteApplication";
            APIHelper apiService = new APIHelper();
            XDocument response = request.GetBasicResponseFromRequest();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;
                string url = MyPdaUrl + ConfigurationManager.AppSettings["UpdateApplication"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                Logs.WriteLogEntry("info", KioskId, "Request url: " + url, _MethodName);
                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];
                var jsonRequest = (dynamic)null;

                foreach (var consumer in consumerList)
                {
                    jsonRequest = new JObject
                    {
                        ["data"] = new JObject
                        {

                            ["customerProfileId"] = consumer["rdaCustomerProfileId"],
                            ["customerAccountInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"]

                        }
                    };
                }

                Logs.WriteLogEntry("info", KioskId, "API Request : " + jsonRequest.ToString(), _MethodName);

                var aPIResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, jsonRequest, KioskId, "");

                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    OTPResponse otpResponse = JsonConvert.DeserializeObject<OTPResponse>(aPIResponse.ResponseContent);
                    Logs.WriteLogEntry("info", KioskId, "API Call Successful!" + aPIResponse.Message, _MethodName);

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "API Call Failed. Status Code: " + aPIResponse.StatusCode, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong";
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

                }
            }
            catch (Exception ex)
            {

                Logs.WriteLogEntry("Error", KioskId, "Error in Failed to DeleteApplication!: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

            }

            return response.ToString();

        }


        #endregion

        #region PMD Verification

        public async Task<string> PmdVerification(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "PmdVerification";
            APIHelper apiService = new APIHelper();
            XDocument response = request.GetBasicResponseFromRequest();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                string mobileNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("mobileNumber")?.Value ?? string.Empty;
                string cnicNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cnic")?.Value ?? string.Empty;
                string otp = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("otp")?.Value ?? string.Empty;
                cnicNumber = cnicNumber.Replace("-", "");

                Logs.WriteLogEntry("info", KioskId, "Request: " + request.ToString(), _MethodName);

                string url = MyPdaUrl + ConfigurationManager.AppSettings["VerifyOtp"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                string OTP = EncryptUsingAES256(otp);

                var requestData = new
                {
                    data = new
                    {
                        mobileNo = mobileNumber,
                        idNumber = cnicNumber,
                        otp = OTP,
                        customerTypeId = 106501,
                        verifyOtp = true
                    },
                    pagination = new
                    {
                        page = 1,
                        size = 10
                    }
                };

                Logs.WriteLogEntry("Info", KioskId, $"API Request bodyElement: {requestData}", _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                Logs.WriteLogEntry("Info", KioskId, $"API Response bodyElement: {apiResponse.ResponseContent}", _MethodName);

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string profileIdsCsv = "";
                    string accInfoIdsCsv = "";
                    JObject VerifyOtpResponse = JObject.Parse(apiResponse.ResponseContent);
                    var appListToken = VerifyOtpResponse["data"]?["appList"];
                    bool flag = false;
                    if (appListToken != null)
                    {
                        List<string> profileIds = new List<string>();
                        List<string> accInfoIds = new List<string>();

                        if (appListToken.Type == JTokenType.Array)
                        {

                            foreach (var item in appListToken)
                            {

                                var accStatusId = Convert.ToString(item["accountStatusId"]?.ToString());

                                if (accStatusId != "100702")
                                {
                                    Logs.WriteLogEntry("Info", KioskId, $"Account Status ID: {accStatusId}", _MethodName);
                                    flag = true;
                                    break;
                                }

                                var profileId = item["rdaCustomerProfileId"]?.ToString();
                                var accInfoId = item["rdaCustomerAccInfoId"]?.ToString();


                                if (!string.IsNullOrEmpty(profileId)) profileIds.Add(profileId);
                                if (!string.IsNullOrEmpty(accInfoId)) accInfoIds.Add(accInfoId);



                            }
                        }

                        if (flag)
                        {
                            Logs.WriteLogEntry("Info", KioskId, $"Customer Application Already in Process:", _MethodName);

                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Dear Customer, your Asaan Account request is already in process."));

                        }
                        else
                        {
                            string[] CustomerProfileIds = profileIds.ToArray();
                            string[] CustomerAccInfoIds = accInfoIds.ToArray();


                            profileIdsCsv = string.Join(", ", CustomerProfileIds);
                            accInfoIdsCsv = string.Join(", ", CustomerAccInfoIds);

                            Logs.WriteLogEntry("Info", KioskId, $"CustomerprofileIds: {profileIdsCsv}", _MethodName);
                            Logs.WriteLogEntry("Info", KioskId, $"CustomerAccInfoIds: {accInfoIdsCsv}", _MethodName);

                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                            response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                            bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success),
                            new XElement("CustomerProfileIds", profileIdsCsv),
                            new XElement("CustomerAccInfoIds", accInfoIdsCsv));
                        }

                    }



                }
                else
                {
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(apiResponse.ResponseContent);
                    string Status = jsonResponse?.message?.status;
                    string Description = jsonResponse?.message?.description;
                    string errorDetail = jsonResponse?.message?.errorDetail;
                    Logs.WriteLogEntry("Info", KioskId, $"API Response: {Description} - {errorDetail} - {Status}", _MethodName);

                    if (Description == "Please provide Valid OTP")
                    {
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = Description;
                        bodyElement.Add(new XElement("RespMessage", "OtpFailed"),
                             new XElement("OTP", "Failed"));

                        Logs.WriteLogEntry("Error", KioskId, $"API Call OTP Failed: {Status} - {Description} - {errorDetail}", _MethodName);
                    }
                    else
                    {
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                        bodyElement.Add(new XElement("RespMessage", "PmdFailed"));

                        Logs.WriteLogEntry("Error", KioskId, $"API Call PMD Failed: {Status} - {Description} - {errorDetail}", _MethodName);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Exception in PmdVerification: " + ex, _MethodName);

                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }

            return response.ToString();
        }


        #endregion

        #region DeleteApplication
        public async Task SendDeleteRequestAsync(string profileId, string accountInfoId, string kioskId)
        {
            APIHelper apiService = new APIHelper();
            string methodName = "SendDeleteRequestAsync";
            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["UpdateApplication"].ToString();
                Logs.WriteLogEntry("Info", kioskId, $"{methodName} [URL]:  {url}", methodName);

                var deleteRequest = new JObject
                {
                    ["data"] = new JObject
                    {
                        ["customerProfileId"] = profileId,
                        ["customerAccountInfoId"] = accountInfoId
                    }
                };

                Logs.WriteLogEntry("info", kioskId, "API Request : " + deleteRequest.ToString(), methodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, deleteRequest, kioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry("info", kioskId, "API Call Successful! " + apiResponse.Message, methodName);
                }
                else
                {
                    Logs.WriteLogEntry("Error", kioskId, $"API Call Failed. Status Code: {apiResponse.StatusCode}", methodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", kioskId, "Error while deleting application: " + ex, methodName);
            }
        }

        #endregion

        #region Get Customer Data From NADRA
        public async Task<string> GetCustomerFromNadra(XDocument request, string RefrenceNumber)
        {

            string _MethodName = "GetCustomerFromNadra";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {

                string AsaanDigitalAccountConventional = ConfigurationManager.AppSettings["AsaanDigitalAccountConventional"].ToString();
                string AsaanDigitalAccountIslamic = ConfigurationManager.AppSettings["AsaanDigitalAccountIslamic"].ToString();
                string AsaanDigitalRemittanceAccountConventional = ConfigurationManager.AppSettings["AsaanDigitalRemittanceAccountConventional"].ToString();
                string AsaanDigitalRemittanceAccountIslamic = ConfigurationManager.AppSettings["AsaanDigitalRemittanceAccountIslamic"].ToString();
                string Declaration = ConfigurationManager.AppSettings["Declaration"].ToString();
                string DeclarationUrdu = ConfigurationManager.AppSettings["DeclarationUrdu"].ToString();
                string TnCEnglish = ConfigurationManager.AppSettings["TnCEnglish"].ToString();
                string TnCUrdu = ConfigurationManager.AppSettings["TnCUrdu"].ToString();

                Logs.WriteLogEntry("info", KioskId, "AsaanDigitalAccountConventional url: " + AsaanDigitalAccountConventional, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "AsaanDigitalAccountIslamic url: " + AsaanDigitalAccountIslamic, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "AsaanDigitalRemittanceAccountConventional url: " + AsaanDigitalRemittanceAccountConventional, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "AsaanDigitalRemittanceAccountIslamic url: " + AsaanDigitalRemittanceAccountIslamic, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "Declaration url: " + Declaration, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "TnCEnglish url: " + TnCEnglish, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "TnCUrdu url: " + TnCUrdu, _MethodName);


                Logs.WriteLogEntry("Info", KioskId, "GetCustomerFromNadra Request !: " + request.ToString(), _MethodName);


                string mobileNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("mobileNumber")?.Value ?? string.Empty;
                string cnicNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cnic")?.Value ?? string.Empty;
                string dateOfIssuance = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("dateOfIssuance")?.Value ?? string.Empty;

                string CustomerProfileIds = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CustomerProfileIds")?.Value ?? string.Empty;
                string CustomerAccInfoIds = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CustomerAccInfoIds")?.Value ?? string.Empty;




                if (CustomerAccInfoIds != "")
                {

                    string profileId = CustomerProfileIds.Split(',').Select(x => x.Trim()).FirstOrDefault();
                    var accountInfoIdList = CustomerAccInfoIds.Split(',').Select(x => x.Trim()).ToList();

                    foreach (string accountId in accountInfoIdList)
                    {
                        await SendDeleteRequestAsync(profileId, accountId, KioskId);
                    }
                }


                DateTime date = DateTime.ParseExact(dateOfIssuance, "yyyy-MM-dd", null);
                string formattedDate = date.ToString("dd/MM/yyyy");

                Logs.WriteLogEntry("Info", KioskId, "GetCustomerFromNadra Request!: " + request.ToString(), _MethodName);
                Logs.WriteLogEntry("Info", KioskId, "GetCustomerFromNadra formattedDate!: " + formattedDate, _MethodName);

                #region List Of Variants

                // Occupation List
                List<Dictionary<string, object>> OccupationList = await GetOccupationListAsync(KioskId);
                if (OccupationList.Count > 0)
                {
                    Logs.WriteLogEntry("Info", KioskId, "Occupation List Found !: " + OccupationList, _MethodName);
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "Occupation List Not Found !: ", _MethodName);
                }

                // Profession List
                List<Dictionary<string, object>> professionList = await GetProfessionListAsync(KioskId);
                if (professionList.Count > 0)
                {
                    Logs.WriteLogEntry("Info", KioskId, "Profession List Found: " + professionList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "Profession List Not Found !: ", _MethodName);
                }

                // Town / Tehsil List
                List<Dictionary<string, object>> townTehsilList = await TownTehsilList(KioskId);
                if (townTehsilList.Count > 0)
                {
                    Logs.WriteLogEntry("Info", KioskId, "Town / Tehsil List Found: " + professionList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "Town / Tehsil Not Found !: ", _MethodName);
                }

                // Branches List
                List<Dictionary<string, object>> islamicBranchList = await IslamicBranchList(KioskId);
                if (islamicBranchList.Count > 0)
                {
                    Logs.WriteLogEntry("Info", KioskId, "Islamic Branch List Found: " + islamicBranchList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "Islamic Branch Not Found !: ", _MethodName);
                }

                // Conventional List
                List<Dictionary<string, object>> conventionalBranchList = await ConventionalBranchList(KioskId);
                if (conventionalBranchList.Count > 0)
                {
                    Logs.WriteLogEntry("Info", KioskId, "Conventional Branch List Found: " + conventionalBranchList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "Conventional Branch Not Found !: ", _MethodName);
                }

                // Gender List
                List<Dictionary<string, object>> genderList = await GenderList(KioskId);
                if (genderList.Count > 0)
                {
                    Logs.WriteLogEntry("Info", KioskId, "Gender List Found: " + genderList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "Gender Not Found !: ", _MethodName);
                }
                // Branches List
                List<Dictionary<string, object>> accountPurposeList = await AccountPurpose(KioskId);
                if (accountPurposeList.Count > 0)
                {
                    Logs.WriteLogEntry("Info", KioskId, "AccountPurpose List Found: " + accountPurposeList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "AccountPurpose Not Found !: ", _MethodName);
                }

                #endregion

                string url = MyPdaUrl + ConfigurationManager.AppSettings["GetCustomerFromNadra"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);


                var requestData = new
                {
                    data = new
                    {
                        consumerList = new[]
                    {
                    new
                    {
                            idNumber = cnicNumber.Replace("-", ""),
                            mobileNo = mobileNumber,
                            dateOfIssue = formattedDate,
                            bankingStyleId = 201501,
                            attachments = new object[] { },
                            isPrimary = true,
                            customerTypeId = 106501

                    }
                        },
                        latitude = 31.4867712,
                        longitude = 74.3276544,
                        noOfJointApplicatns = 0,
                        channelId = 114604,
                        bioMetricVerificationNadra = 1
                    }
                };


                Logs.WriteLogEntry("Info", KioskId, "GetCustomerFromNadra API Request !: " + JsonConvert.SerializeObject(requestData), _MethodName);
                var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");



                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var consumerList = jsonResponse["data"]?["consumerList"];

                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    string FullName = "";
                    string DateOfBirth = "";
                    string rdaCustomerProfileId = "";
                    string MotherName = "";
                    string FatherName = "";
                    string gender = "";
                    string genderId = "";
                    string Address1 = "";
                    string Address2 = "";
                    string Path = "";
                    string AccessToken = "";
                    string IsTranslate = "";





                    if (consumerList != null)
                    {
                        foreach (var consumer in consumerList)
                        {
                            FullName = consumer["fullName"].ToString();
                            DateOfBirth = consumer["dateOfBirth"].ToString();
                            rdaCustomerProfileId = consumer["rdaCustomerProfileId"].ToString();
                            MotherName = Decrypt(consumer["motherMaidenNameEncrypted"].ToString());
                            FatherName = consumer["fatherHusbandName"].ToString();
                            gender = consumer["gender"].ToString();
                            genderId = consumer["genderId"].ToString();
                            Address1 = consumer["addresses"]?[0]?["customerAddress"].ToString();
                            Address2 = consumer["addresses"]?[0]?["customerAddressLine1"].ToString();
                            Path = consumer["attachments"]?[0]?["path"].ToString();
                            AccessToken = consumer["accessToken"].ToString();
                            IsTranslate = consumer["nadraTranslationInUrduInd"].ToString();
                        }


                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                        bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success),

                        new XElement("FullName", FullName),
                        new XElement("DateOfBirth", DateOfBirth),
                        new XElement("MotherName", MotherName),
                        new XElement("FatherName", FatherName),
                        new XElement("CustomerProfileId", rdaCustomerProfileId),
                        new XElement("Gender", gender),
                        new XElement("GenderId", genderId),
                        new XElement("Address1", Address1),
                        new XElement("Address2", Address2),
                        new XElement("IslamicBranchList", islamicBranchList),
                        new XElement("ConventionalBranchList", conventionalBranchList),
                        new XElement("OccupationList", OccupationList),
                        new XElement("ProfessionList", professionList),
                        new XElement("GenderList", genderList),
                        new XElement("TownTehsilList", townTehsilList),
                        new XElement("AccountPurposeList", accountPurposeList),
                        new XElement("Path", Path),
                        new XElement("AccessToken", AccessToken),
                        new XElement("NadraResponse", apiResponse.ResponseContent),
                        new XElement("IsTranslate", IsTranslate),
                        new XElement("TnCEnglish", TnCEnglish),
                        new XElement("TnCUrdu", TnCUrdu),
                        new XElement("Declaration", Declaration),
                        new XElement("DeclarationUrdu", DeclarationUrdu),
                        new XElement("AsaanDigitalAccountConventional", AsaanDigitalAccountConventional),
                        new XElement("AsaanDigitalAccountIslamic", AsaanDigitalAccountIslamic),
                        new XElement("AsaanDigitalRemittanceAccountConventional", AsaanDigitalRemittanceAccountConventional),
                        new XElement("AsaanDigitalRemittanceAccountIslamic", AsaanDigitalRemittanceAccountIslamic));




                    }
                }
                else
                {
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error in Failed to GetCustomerFromNadra!: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();

        }

        #endregion

        #region Personal Information
        public async Task<string> PersonalInformation(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "PostPersonalInformation";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                string name = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("name")?.Value ?? string.Empty;
                string motherName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("motherName")?.Value ?? string.Empty;
                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;
                string accessToken = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accessToken")?.Value ?? string.Empty;
                string genderId = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("gender")?.Value ?? string.Empty;

                Logs.WriteLogEntry("info", KioskId, "{PersonalInfo} Step 1: Going to send request basic info personal Data", _MethodName);
                Logs.WriteLogEntry("info", KioskId, "request" + request, _MethodName);

                string CustomerBasicInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerBasicInfo"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [CustomerBasicInfo URL]:  {CustomerBasicInfoUrl}", _MethodName);
                string CustomerAccountInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerAccountInfo"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [CustomerAccountInfo URL]:  {CustomerAccountInfoUrl}", _MethodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];


                var jsonRequest = (dynamic)null;

                foreach (var consumer in consumerList)
                {
                    jsonRequest = new JObject
                    {


                        ["data"] = new JObject
                        {
                            ["consumerList"] = new JArray
                 {
                     new JObject
                     {
                   ["rdaCustomerProfileId"] = consumer["rdaCustomerProfileId"],
                     ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                     ["fullName"] = name,
                     ["fatherHusbandName"] = consumer["fatherHusbandName"],
                     ["motherMaidenName"] = motherName,
                     ["genderId"] = genderId,
                     ["cityOfBirth"] = Decrypt(consumer["cityOfBirthEncrypted"].ToString()),
                     ["countryOfBirthPlaceId"] = 157,
                     ["isPrimary"] = consumer["isPrimary"],
                     ["nationalityTypeId"] = 100901,
                     ["nationalities"] = new JArray
                     {
                         new JObject
                         {
                             ["rdaCustomerId"] = consumer["rdaCustomerProfileId"],
                             ["nationalityId"] = 157,
                             ["idNumber"] = consumer["idNumber"]
                         }
                     },
                     ["placeOfIssue"] = 157,
                     ["occupationId"] = consumer["occupationId"] ?? JValue.CreateNull(),
                     ["professionId"] = consumer["professionId"] ?? JValue.CreateNull(),
                     ["emailAddress"] = consumer["emailAddress"] ?? JValue.CreateNull(),
                     ["taxResidentInd"] = 0,
                     ["countryOfResidenceId"] = 157,
                     ["customerTitleId"] = 100801,
                     ["nameOfOrganization"] = consumer["nameOfOrganization"] ?? JValue.CreateNull(),
                     ["designation"] = consumer["designation"] ?? JValue.CreateNull(),
                     ["employedSince"] = consumer["employedSince"] ?? JValue.CreateNull(),
                     ["employerAddress"] = consumer["employerAddress"] ?? JValue.CreateNull(),
                     ["employerAddressLine2"] = consumer["employerAddressLine2"] ?? JValue.CreateNull(),
                     ["employerTown"] = consumer["employerTown"] ?? JValue.CreateNull(),
                     ["employerCity"] = consumer["employerCity"] ?? JValue.CreateNull(),
                     ["customerTypeId"] =106501
                     }
                 }
                        }
                    };

                }
                Console.WriteLine(JsonConvert.SerializeObject(jsonRequest));
                Logs.WriteLogEntry("info", KioskId, "{personal-basic-info} jsonRequest:" + JsonConvert.SerializeObject(jsonRequest), _MethodName);




                APIResponse aPIResponse = await apiService.SendRestTransaction(CustomerBasicInfoUrl, HttpMethods.POST, jsonRequest, accessToken, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    Logs.WriteLogEntry("info", KioskId, "{personal-basic-info} Response was successful. Step 2:", _MethodName);

                    var responseData = JsonConvert.DeserializeObject<dynamic>(aPIResponse.ResponseContent);



                    Logs.WriteLogEntry("info", KioskId, "{personal-basic-info} Response Content  Step 3:" + responseData, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "Going to send request of {personal-account-info}   Step 4:", _MethodName);
                    var jsonRequest2 = (dynamic)null;
                    foreach (var consumer in consumerList)
                    {
                        jsonRequest2 = new JObject
                        {
                            ["data"] = new JObject
                            {
                                ["rdaCustomerId"] = consumer["rdaCustomerProfileId"],
                                ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                                ["customerAccountTypeId"] = consumer["accountInformation"]["customerAccountTypeId"],
                                ["purposeOfAccountId"] = consumer["accountInformation"]["purposeOfAccountId"],
                                ["atmTypeId"] = consumer["accountInformation"]["atmTypeId"],
                                ["bankStatementAlertInd"] = consumer["accountInformation"]["bankStatementAlertInd"],
                                ["accountVariantId"] = consumer["accountInformation"]["accountVariantId"],
                                ["noOfJointApplicatns"] = consumer["accountInformation"]["noOfJointApplicatns"],
                                ["currencyType"] = consumer["accountInformation"]["currencyType"],
                                ["currencyTypeId"] = consumer["accountInformation"]["currencyTypeId"],
                                ["natureOfAccountId"] = consumer["accountInformation"]["natureOfAccountId"],
                                ["antiAnnualSalaryId"] = 102603,
                                ["bankingModeId"] = consumer["accountInformation"]["bankingModeId"],
                                ["customerBranch"] = consumer["accountInformation"]["customerBranch"],
                                ["physicalCardInd"] = consumer["accountInformation"]["physicalCardInd"],
                                ["beneficialOwnerAccount"] = name,
                                ["nameOnPhysicalATM"] = name,
                                ["modeOfMajorTransId"] = consumer["accountInformation"]["modeOfMajorTransId"],
                                ["operatingInstId"] = 103001,
                                ["mailingAddrPrefId"] = consumer["accountInformation"]["mailingAddrPrefId"],
                                ["customerTypeId"] = 106501,
                                ["accountTypeId"] = 102201,
                            }
                        };
                    }

                    Logs.WriteLogEntry("info", KioskId, "{personal-account-info}" + jsonRequest2, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "CustomerAccountInfoUrl" + CustomerAccountInfoUrl, _MethodName);
                    APIResponse aPIResponse2 = await apiService.SendRestTransaction(CustomerAccountInfoUrl, HttpMethods.POST, jsonRequest2, accessToken, "");
                    if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {


                        Logs.WriteLogEntry("info", KioskId, "{personal-basic-info} Response was successful. Step 2:", _MethodName);

                        var responseData2 = JsonConvert.DeserializeObject<dynamic>(aPIResponse2.ResponseContent);

                        Logs.WriteLogEntry("info", KioskId, "{personal-basic-info} Response Content  Step 3:" + responseData2, _MethodName);

                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    }
                    else
                    {
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                    }

                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, " {PersonalInfo} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry("Error", KioskId, " {PersonalInfo} Error Message: " + aPIResponse.Message, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"Exception in {{PersonalInfo}}: {ex.Message}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();
        }

        #endregion

        #region CurrentAddress
        public async Task<string> CurrentAddress(XDocument request, string RefrenceNumber)
        {

            string _MethodName = "CurrentAddress";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                string address1 = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("address1")?.Value ?? string.Empty;
                string address2 = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("address2")?.Value ?? string.Empty;
                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;
                string accessToken = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accessToken")?.Value ?? string.Empty;
                string town = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("town")?.Value ?? string.Empty;
                string city = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("city")?.Value ?? string.Empty;


                Logs.WriteLogEntry("info", KioskId, "request" + request, _MethodName);

                string url = MyPdaUrl + ConfigurationManager.AppSettings["CurrentAddress"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];


                var jsonRequest = (dynamic)null;

                foreach (var consumer in consumerList)
                {
                    jsonRequest = new JObject
                    {
                        ["data"] = new JArray
                            {
                                new JObject
                                {
                                    ["addressesList"] = new JArray
                                    {
                                        new JObject
                                        {
                                            ["rdaCustomerProfileAddrId"] = consumer["addresses"][0]["rdaCustomerProfileAddrId"],
                                            ["rdaCustomerId"] = consumer["addresses"][0]["rdaCustomerId"],
                                            ["phone"] = consumer["addresses"][0]["phone"],
                                            ["postalCode"] = consumer["addresses"][0]["postalCode"],
                                            ["nearestLandMark"] = consumer["addresses"][0]["nearestLandMark"],
                                            ["mobileNo"] = consumer["addresses"][0]["mobileNo"],
                                            ["customerTown"] = town,
                                            ["customerAddress"] = address1,
                                            ["customerAddressLine1"] = address2,
                                            ["countryCodeMobile"] = +92,
                                            ["city"] = city,
                                            ["countryId"] = 157,
                                            ["country"] =  "Pakistan",
                                            ["addressTypeForeignInd"] = consumer["addresses"][0]["addressTypeForeignInd"],
                                            ["addressTypeId"] = consumer["addresses"][0]["addressTypeId"],
                                        },
                                        new JObject
                                        {
                                            ["rdaCustomerProfileAddrId"] = consumer["addresses"][1]["rdaCustomerProfileAddrId"],
                                            ["rdaCustomerId"] = consumer["addresses"][1]["rdaCustomerId"],
                                             ["postalCode"] = consumer["addresses"][1]["postalCode"],
                                            ["phone"] = consumer["addresses"][1]["phone"],
                                            ["nearestLandMark"] = consumer["addresses"][1]["nearestLandMark"],
                                            ["mobileNo"] = consumer["addresses"][1]["mobileNo"],
                                            ["customerTown"] = town,
                                            ["customerAddress"] = consumer["addresses"][1]["customerAddress"],
                                            ["customerAddressLine1"] = consumer["addresses"][1]["customerAddressLine1"],
                                            ["countryCodeMobile"] = +92,
                                            ["city"] = city,
                                            ["countryId"] = 157,
                                            ["country"] = "Pakistan",
                                            ["addressTypeForeignInd"] = consumer["addresses"][1]["addressTypeForeignInd"],
                                            ["addressTypeId"] = consumer["addresses"][1]["addressTypeId"],
                                        }
                                    },
                                    ["isPrimary"] = true
                                }
                            }
                    };

                }
                Console.WriteLine(JsonConvert.SerializeObject(jsonRequest));
                Logs.WriteLogEntry("info", KioskId, "{CurrentAddress} jsonRequest:" + JsonConvert.SerializeObject(jsonRequest), _MethodName);

                APIResponse aPIResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, jsonRequest, accessToken, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, " {CurrentAddress} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry("Error", KioskId, " {CurrentAddress} Error Message: " + aPIResponse.Message, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"Exception in {{CurrentAddress}}: {ex.Message}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();

        }
        #endregion

        #region Occupational Details
        public async Task<string> OccupationalDetail(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "OccupationalDetail";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                string name = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("name")?.Value ?? string.Empty;
                string motherName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("motherName")?.Value ?? string.Empty;
                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;
                string accessToken = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accessToken")?.Value ?? string.Empty;
                string occupationId = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("occupation")?.Value ?? string.Empty;
                string professionId = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("profession")?.Value ?? string.Empty;
                string professionName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("professionName")?.Value ?? string.Empty;
                string genderId = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("gender")?.Value ?? string.Empty;
                string address1 = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("address1")?.Value ?? string.Empty;
                string address2 = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("address2")?.Value ?? string.Empty;
                string town = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("town")?.Value ?? string.Empty;
                string city = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("city")?.Value ?? string.Empty;
                string Email = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("email")?.Value ?? string.Empty;


                Logs.WriteLogEntry("info", KioskId, "{OccupationalDetail} Step 1: Going to send request basic info personal Data", _MethodName);
                Logs.WriteLogEntry("info", KioskId, "request" + request, _MethodName);


                if (string.IsNullOrEmpty(professionId) && !string.IsNullOrEmpty(professionName))
                {
                    List<Dictionary<string, object>> professionList = await GetProfessionListAsync(KioskId);


                    var matchingProfession = professionList.FirstOrDefault(p =>
                        p.ContainsKey("name") &&
                        p["name"]?.ToString().Equals(professionName, StringComparison.OrdinalIgnoreCase) == true);

                    if (matchingProfession != null && matchingProfession.ContainsKey("id"))
                    {
                        professionId = matchingProfession["id"]?.ToString();
                    }
                }
                string CustomerBasicInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerBasicInfo"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [CustomerBasicInfo URL]:  {CustomerBasicInfoUrl}", _MethodName);

                string SaveKycUrl = MyPdaUrl + ConfigurationManager.AppSettings["SaveKyc"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [SaveKycUrl URL]:  {SaveKycUrl}", _MethodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];


                var jsonRequest = (dynamic)null;

                foreach (var consumer in consumerList)
                {
                    jsonRequest = new JObject
                    {
                        ["data"] = new JObject
                        {
                            ["consumerList"] = new JArray
                            {
                            new JObject
                            {
                                 ["rdaCustomerProfileId"] = consumer["rdaCustomerProfileId"],
                                 ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                                 ["fullName"] = name,
                                 ["fatherHusbandName"] = consumer["fatherHusbandName"],
                                 ["motherMaidenName"] = motherName,
                                 ["genderId"] = genderId,
                                 ["cityOfBirth"] = Decrypt(consumer["cityOfBirthEncrypted"].ToString()),
                                 ["countryOfBirthPlaceId"] = 157,
                                 ["isPrimary"] = consumer["isPrimary"],
                                 ["nationalityTypeId"] = 100901,
                                 ["nationalities"] = new JArray
                                 {
                                     new JObject
                                     {
                                         ["rdaCustomerId"] = consumer["rdaCustomerProfileId"],
                                         ["nationalityId"] = 157,
                                         ["idNumber"] = consumer["idNumber"]
                                     }
                                 },
                                 ["placeOfIssue"] = 157,
                                 ["occupationId"] = occupationId,
                                 ["professionId"] = professionId,
                                 ["emailAddress"] = Email,
                                 ["taxResidentInd"] = 0,
                                 ["countryOfResidenceId"] = 157,
                                 ["customerTitleId"] = 100801,
                                 ["nameOfOrganization"] = consumer["nameOfOrganization"] ?? JValue.CreateNull(),
                                 ["designation"] = consumer["designation"] ?? JValue.CreateNull(),
                                 ["employedSince"] = consumer["employedSince"] ?? JValue.CreateNull(),
                                 ["employerAddress"] = address1,
                                 ["employerAddressLine2"] = address2,
                                 ["employerTown"] = town,
                                 ["employerCity"] = city,
                                 ["customerTypeId"] = 106501
                                 }
                            }
                        }
                    };

                }

                Logs.WriteLogEntry("info", KioskId, "Request :" + JsonConvert.SerializeObject(jsonRequest), _MethodName);
                APIResponse aPIResponse = await apiService.SendRestTransaction(CustomerBasicInfoUrl, HttpMethods.POST, jsonRequest, accessToken, "");

                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    Logs.WriteLogEntry("info", KioskId, "{occupational-basic-info} Response was successful. Step 2:", _MethodName);
                    var responseData = JsonConvert.DeserializeObject<dynamic>(aPIResponse.ResponseContent);

                    Logs.WriteLogEntry("info", KioskId, "{occupational-basic-info} Response Content  Step 3:" + responseData, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "Going to send request of {kyc-save}   Step 4:", _MethodName);
                    var jsonRequest2 = (dynamic)null;

                    foreach (var consumer in consumerList)
                    {
                        jsonRequest2 = new JObject
                        {
                            ["data"] = new JArray
                            {
                                new JObject
                                {
                                    ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                                    ["rdaCustomerProfileId"] = consumer["rdaCustomerProfileId"],
                                    ["isPrimary"] = true
                                }
                            }
                        };
                    }

                    Logs.WriteLogEntry("info", KioskId, "kyc-save" + JsonConvert.SerializeObject(jsonRequest2), _MethodName);
                    APIResponse aPIResponse2 = await apiService.SendRestTransaction(SaveKycUrl, HttpMethods.POST, jsonRequest2, accessToken, "");
                    if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Logs.WriteLogEntry("info", KioskId, "{kyc-save} Response was successful. Step 2:", _MethodName);

                        var responseData2 = JsonConvert.DeserializeObject<dynamic>(aPIResponse2.ResponseContent);

                        Logs.WriteLogEntry("info", KioskId, "{kyc-save} Response Content  Step 3:" + responseData2, _MethodName);

                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    }

                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, " {OccupationalDetail} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry("Error", KioskId, " {OccupationalDetail} Error Message: " + aPIResponse.Message, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"Exception in {{CustomerAccountList}}: {ex.Message}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();
        }

        #endregion

        #region Banking Reference 
        public async Task<string> BankingReference(XDocument request, string RefrenceNumber)
        {

            string _MethodName = "BankingReference";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                string bankingMode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("bankingMode")?.Value ?? string.Empty;
                string branch = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("branch")?.Value ?? string.Empty;
                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;
                string accessToken = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accessToken")?.Value ?? string.Empty;
                string name = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("name")?.Value ?? string.Empty;

                Logs.WriteLogEntry("info", KioskId, "request" + request, _MethodName);

                string CustomerAccountInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerAccountInfo"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [CustomerAccountInfo URL]:  {CustomerAccountInfoUrl}", _MethodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];


                var jsonRequest = (dynamic)null;

                foreach (var consumer in consumerList)
                {
                    jsonRequest = new JObject
                    {
                        ["data"] = new JObject
                        {
                            ["rdaCustomerId"] = consumer["rdaCustomerProfileId"],
                            ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                            ["customerAccountTypeId"] = consumer["accountInformation"]["customerAccountTypeId"],
                            ["purposeOfAccountId"] = consumer["accountInformation"]["purposeOfAccountId"],
                            ["atmTypeId"] = null,
                            ["bankStatementAlertInd"] = consumer["accountInformation"]["bankStatementAlertInd"],
                            ["accountVariantId"] = consumer["accountInformation"]["accountVariantId"],
                            ["noOfJointApplicatns"] = consumer["accountInformation"]["noOfJointApplicatns"],
                            ["currencyType"] = consumer["accountInformation"]["currencyType"],
                            ["currencyTypeId"] = consumer["accountInformation"]["currencyTypeId"],
                            ["natureOfAccountId"] = consumer["accountInformation"]["natureOfAccountId"],
                            ["antiAnnualSalaryId"] = 102603,
                            ["bankingModeId"] = Convert.ToInt32(bankingMode),
                            ["customerBranch"] = branch,
                            ["physicalCardInd"] = 1,
                            ["beneficialOwnerAccount"] = name,
                            ["nameOnPhysicalATM"] = name,
                            ["modeOfMajorTransId"] = consumer["accountInformation"]["modeOfMajorTransId"],
                            ["operatingInstId"] = 103001,
                            ["mailingAddrPrefId"] = consumer["accountInformation"]["mailingAddrPrefId"],
                            ["customerTypeId"] = 106501,
                            ["accountTypeId"] = 102201,
                        }
                    };
                }


                Logs.WriteLogEntry("info", KioskId, "jsonRequest" + jsonRequest, _MethodName);

                APIResponse aPIResponse = await apiService.SendRestTransaction(CustomerAccountInfoUrl, HttpMethods.POST, jsonRequest, accessToken, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                }
                else
                {

                    Logs.WriteLogEntry("Error", KioskId, " {BankingReference} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry("Error", KioskId, " {BankingReference} Error Message: " + aPIResponse.Message, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"Exception in {{BankingReference}}: {ex.Message}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();


        }


        #endregion

        #region Accounts Details
        public async Task<string> AccountsDetails(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "AccountsDetails";
            APIHelper apiService = new APIHelper();
            XDocument response = request.GetBasicResponseFromRequest();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                string accountType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accountType")?.Value ?? string.Empty;
                string accountPurpose = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accountPurpose")?.Value ?? string.Empty;
                string remiterName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("remiterName")?.Value ?? string.Empty;
                string remiterRelationship = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("remiterRelationship")?.Value ?? string.Empty;
                string emailAddress = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("emailAddress")?.Value ?? string.Empty;
                string accountStatement = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accountStatement")?.Value ?? string.Empty;
                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;
                string accessToken = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accessToken")?.Value ?? string.Empty;
                string name = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("name")?.Value ?? string.Empty;
                string bankingMode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("bankingMode")?.Value ?? string.Empty;
                string branch = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("branch")?.Value ?? string.Empty;
                string GenderID = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("genderId")?.Value ?? string.Empty;
                string occupationID = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("occupationId")?.Value ?? string.Empty;
                string DateOfBirth = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("dateOfBirth")?.Value ?? string.Empty;

                Logs.WriteLogEntry("info", KioskId, "request" + request.ToString(), _MethodName);
                int accountstate = 0;
                // 
                if (accountStatement == "true")
                {
                    accountstate = 1;
                }

                string CustomerAccountInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerAccountInfo"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [CustomerAccountInfo URL]:  {CustomerAccountInfoUrl}", _MethodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];

                int FinalAccountType = 0;

                if (accountType == "Current")
                {
                    FinalAccountType = 114301;
                }
                else
                {
                    FinalAccountType = 114302;
                }

                DateTime date = DateTime.Now;
                foreach (var consumer1 in consumerList)
                {
                    string Date = consumer1["dateOfBirth"].ToString();
                    date = DateTime.ParseExact(Date, "dd/MM/yyyy", null);
                }

                Logs.WriteLogEntry("info", KioskId, "Updated Date Of Birth" + date, _MethodName);

                VariantInfo variantInfo = await GetAccoutListWithAccountNames(FinalAccountType, accountPurpose, bankingMode, GenderID, occupationID, date, KioskId);
                var jsonRequest = (dynamic)null;

                foreach (var consumer in consumerList)
                {
                    jsonRequest = new JObject
                    {
                        ["data"] = new JObject
                        {
                            ["rdaCustomerId"] = consumer["rdaCustomerProfileId"],
                            ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                            ["customerAccountTypeId"] = FinalAccountType,
                            ["purposeOfAccountId"] = int.Parse(accountPurpose),
                            ["atmTypeId"] = null,
                            ["bankStatementAlertInd"] = accountstate,
                            ["accountVariantId"] = variantInfo.Id,
                            ["noOfJointApplicatns"] = 0,
                            ["currencyType"] = "PKR",
                            ["currencyTypeId"] = 108301,
                            ["natureOfAccountId"] = 102101,
                            ["antiAnnualSalaryId"] = 102603,
                            ["bankingModeId"] = Convert.ToInt32(bankingMode),
                            ["customerBranch"] = branch,
                            ["physicalCardInd"] = 1,
                            ["beneficialOwnerAccount"] = name,
                            ["nameOnPhysicalATM"] = name,
                            ["modeOfMajorTransId"] = new JArray
                            {
                                108401,
                                108402,
                                108403,
                                108405,
                                108406,
                                108407
                            },
                            ["operatingInstId"] = 103001,
                            ["remitterName"] = remiterName,
                            ["relationshipWithRemitter"] = remiterRelationship,
                            ["mailingAddrPrefId"] = 103301,
                            ["customerTypeId"] = 106501,
                            ["accountTypeId"] = 102201,
                        }
                    };
                }

                Logs.WriteLogEntry("Info", KioskId, "jsonRequest !: " + JsonConvert.SerializeObject(jsonRequest), _MethodName);

                APIResponse aPIResponse = await apiService.SendRestTransaction(CustomerAccountInfoUrl, HttpMethods.POST, jsonRequest, accessToken, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry("Info", KioskId, " {AccountsDetails} Response: " + aPIResponse.ResponseContent, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(new XElement("AccountCategory", variantInfo.Name));
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, " {AccountsDetails} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry("Error", KioskId, " {AccountsDetails} Error Message: " + aPIResponse.Message, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error in Failed to AccountsDetails!: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();
        }


        #endregion

        #region Post Reviewed Details
        public async Task<string> ReviewedDetails(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "ReviewedDetails";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;
                string accessToken = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accessToken")?.Value ?? string.Empty;

                Logs.WriteLogEntry("info", KioskId, "request" + request, _MethodName);

                string UpdateKycUrl = MyPdaUrl + ConfigurationManager.AppSettings["UpdateKyc"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [UpdateKyc URL]:  {UpdateKycUrl}", _MethodName);

                string AuthorizerKycUrl = MyPdaUrl + ConfigurationManager.AppSettings["AuthorizerKyc"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [AuthorizerKyc URL]:  {AuthorizerKycUrl}", _MethodName);

                string CustomerProfileStatusUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerProfileStatus"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [CustomerProfileStatus URL]:  {CustomerProfileStatusUrl}", _MethodName);

                string ScreeningUrl = MyPdaUrl + ConfigurationManager.AppSettings["Screening"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [ScreeningUrl URL]:  {ScreeningUrl}", _MethodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];


                //// Update KYC

                Logs.WriteLogEntry("info", KioskId, "Going to send request of {update-kyc}:", _MethodName);

                var jsonRequest = (dynamic)null;

                foreach (var consumer in consumerList)
                {
                    jsonRequest = new JObject
                    {
                        ["data"] = new JArray
                        {
                          new JObject
                          {
                            ["rdaCustomerProfileId"] = consumer["rdaCustomerProfileId"],
                            ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                            ["isCapuByPass"] = true
                          }
                        }
                    };
                }

                Logs.WriteLogEntry("info", KioskId, "update-kyc Request" + JsonConvert.SerializeObject(jsonRequest), _MethodName);

                APIResponse aPIResponse = await apiService.SendRestTransaction(UpdateKycUrl, HttpMethods.POST, jsonRequest, accessToken, "");
                JObject updateKycResponse = JObject.Parse(aPIResponse.ResponseContent);
                var updateKycData = updateKycResponse["data"];
                var updateKycMessage = updateKycResponse["message"];
                string msg = updateKycData["msg"]?.ToString();

                Logs.WriteLogEntry("info", KioskId, "Response updateKycData :" + aPIResponse.ResponseContent, _MethodName);
                Logs.WriteLogEntry("info", KioskId, "Response updateKycMessage :" + updateKycMessage["status"] + "-" + updateKycMessage["description"], _MethodName);

                bool flag = false;
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {


                    Logs.WriteLogEntry("info", KioskId, "updateKycData" + updateKycData + "-" + msg, _MethodName);

                    if (msg == "Account Directly Push to t24")
                    {
                        //// Authorizer KYC
                        Logs.WriteLogEntry("info", KioskId, "Going to send request of {AuthorizerKyc}:", _MethodName);
                        var jsonRequest2 = (dynamic)null;
                        foreach (var consumer in consumerList)
                        {
                            jsonRequest2 = new JObject
                            {
                                ["data"] = new JObject
                                {
                                    ["rdaCustomerProfileId"] = consumer["rdaCustomerProfileId"],
                                    ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"]
                                }
                            };
                        }

                        Logs.WriteLogEntry("info", KioskId, "Authorizer-Kyc Request" + JsonConvert.SerializeObject(jsonRequest2), _MethodName);
                        APIResponse aPIResponse2 = await apiService.SendRestTransaction(AuthorizerKycUrl, HttpMethods.POST, jsonRequest2, accessToken, "");
                        Logs.WriteLogEntry("info", KioskId, "Response :" + aPIResponse2.ResponseContent, _MethodName);
                        JObject AuthorizerKyceResponse = JObject.Parse(aPIResponse2.ResponseContent);

                        var AuthorizerKycMessage = AuthorizerKyceResponse["message"];

                        string AccountNumber = "";
                        string DataResponseMessage = "null";
                        string t24StatusCode = "";
                        string responseMessage = "";
                        string acccountStatus = "";
                        string accountStatusCodeId = "";
                        string cpNumber = "";



                        if (AuthorizerKyceResponse["data"] is JObject AuthorizerKycData)
                        {
                            AccountNumber = AuthorizerKycData["accountNumber"]?.ToString();
                            DataResponseMessage = AuthorizerKycData["responseMessage"]?.ToString();
                            t24StatusCode = AuthorizerKycData["t24StatusCode"]?.ToString();
                            acccountStatus = AuthorizerKycData["acccountStatus"]?.ToString();
                            accountStatusCodeId = AuthorizerKycData["accountStatusCodeId"]?.ToString();
                            cpNumber = AuthorizerKycData["cpNumber"]?.ToString();
                        }


                        Logs.WriteLogEntry("info", KioskId, "Response DataResponseMessage :" + DataResponseMessage, _MethodName);
                        Logs.WriteLogEntry("info", KioskId, "Response AuthorizerKycData :" + aPIResponse2.ResponseContent, _MethodName);
                        Logs.WriteLogEntry("info", KioskId, "Response AuthorizerKycMessage :" + AuthorizerKycMessage["status"]?.ToString() + "-" + AuthorizerKycMessage["description"]?.ToString(), _MethodName);

                        if (aPIResponse2.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(AccountNumber))
                        {

                            Logs.WriteLogEntry("info", KioskId, "Response Account Number Recieved :" + AccountNumber, _MethodName);
                            Logs.WriteLogEntry("info", KioskId, "Going to send request of {Screening}:", _MethodName);
                            var jsonRequest3 = (dynamic)null;
                            foreach (var consumer in consumerList)
                            {
                                jsonRequest3 = new JObject
                                {
                                    ["data"] = new JObject
                                    {
                                        ["customerProfileId"] = consumer["rdaCustomerProfileId"],
                                        ["customerAccountInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                                    }
                                };
                            }


                            Logs.WriteLogEntry("info", KioskId, "Screening Request" + JsonConvert.SerializeObject(jsonRequest3), _MethodName);

                            APIResponse aPIRespons3 = await apiService.SendRestTransaction(ScreeningUrl, HttpMethods.POST, jsonRequest3, accessToken, "");
                            JObject ScreeningResponse = JObject.Parse(aPIRespons3.ResponseContent);
                            var ScreeningData = ScreeningResponse["data"];
                            var ScreeningMessage = ScreeningResponse["message"];

                            Logs.WriteLogEntry("info", KioskId, "Response ScreeningData :" + aPIRespons3.ResponseContent, _MethodName);
                            Logs.WriteLogEntry("info", KioskId, "Response ScreeningMessage :" + ScreeningMessage["status"] + "-" + ScreeningMessage["description"], _MethodName);

                            if (aPIRespons3.StatusCode == HttpStatusCode.OK)
                            {


                                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                                var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                                bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success),
                                    new XElement("ResponseMessage", responseMessage),
                                    new XElement("AcccountStatus", acccountStatus),
                                    new XElement("AccountStatusCodeId", accountStatusCodeId),
                                    new XElement("CpNumber", cpNumber),
                                    new XElement("AccountNumber", AccountNumber),
                                    new XElement("T24StatusCode", t24StatusCode));
                            }
                            else
                            {
                                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

                            }


                        }
                        else
                        {


                            if (AuthorizerKycMessage["description"].ToString() == "Your Personal Information is not as per Nadra record please correct your information and try again")
                            {
                                Logs.WriteLogEntry("info", KioskId, "Error in Authorizer-Kyc " + AuthorizerKycMessage["description"].ToString(), _MethodName);
                                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", "Verification Failed !"));
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Your Personal Information is not as per Nadra record please correct your information and try again"));
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("ErrorType", "Retry"));
                            }
                            else if (AuthorizerKycMessage["description"].ToString().ToLower() == "please proceed account with desk")
                            {
                                //// Customer Profile Status
                                Logs.WriteLogEntry("info", KioskId, "Going to send request of {CustomerProfileStatus}:", _MethodName);
                                var jsonRequest4 = (dynamic)null;
                                foreach (var consumer in consumerList)
                                {
                                    jsonRequest4 = new JObject
                                    {
                                        ["data"] = new JObject
                                        {
                                            ["rdaCustomerProfileId"] = consumer["rdaCustomerProfileId"],
                                            ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                                            ["statusId"] = 100703,
                                        }
                                    };
                                }

                                Logs.WriteLogEntry("info", KioskId, "CustomerProfileStatus Request" + JsonConvert.SerializeObject(jsonRequest4), _MethodName);

                                APIResponse aPIRespons4 = await apiService.SendRestTransaction(CustomerProfileStatusUrl, HttpMethods.POST, jsonRequest4, accessToken, "");
                                JObject CustomerProfileStatusResponse = JObject.Parse(aPIRespons4.ResponseContent);
                                var CustomerProfileStatus = CustomerProfileStatusResponse["data"];
                                var CustomerProfileStatusmessage = CustomerProfileStatusResponse["message"];

                                Logs.WriteLogEntry("info", KioskId, "Response CustomerProfileStatus :" + aPIRespons4.ResponseContent, _MethodName);
                                //Logs.WriteLogEntry("info", KioskId , "Response CustomerProfileStatusmessage :" + CustomerProfileStatusmessage["status"] + "-" + CustomerProfileStatusmessage["description"], _MethodName);

                                if (aPIRespons4.StatusCode == HttpStatusCode.OK)
                                {
                                    Logs.WriteLogEntry("info", KioskId, "CustomerProfileStatus" + CustomerProfileStatusmessage["description"], _MethodName);
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", "Application Submitted"));
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Dear Customer your application has been submitted and currently under review. Bank will communicate the status of your application within two working days."));
                                }
                                else
                                {
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                                }

                            }
                            else
                            {
                                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", AuthorizerKycMessage["description"].ToString()));
                            }
                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry("info", KioskId, "Update KYC Response Data is Null", _MethodName);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, " {update-kyc} Request failed  " + updateKycMessage, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"Exception in {{Review Details}}: {ex.Message}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();
        }

        #endregion

        #region Liveliness 

        public async Task<string> LivelinessCheck(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "LivelinessCheck";
            APIHelper apiService = new APIHelper();
            XDocument response = request.GetBasicResponseFromRequest();
            SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {

                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;
                string accessToken = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("accessToken")?.Value ?? string.Empty;
                string customerProfileImagePath = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("customerProfileImagePath")?.Value ?? string.Empty;
                string UserPicture = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("UserPicture")?.Value ?? string.Empty;

                Logs.WriteLogEntry("Info", KioskId, "UserPicture !: " + UserPicture, _MethodName);



                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];
                string DecryptPath = Decrypt(customerProfileImagePath);

                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [Customer Nadra Image]:  {DecryptPath}", _MethodName);

                string url = MyPdaUrl + ConfigurationManager.AppSettings["Liveliness"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                #region Selfie Image 

                // Selfie Image Processing
                string selfieImageToBase64 = "";
                string _SourcePath = "C:\\CommonFiles\\FileStorage\\";
                string UserFileName = UserPicture.Replace("path=", ""); // Remove "path="
                string selfieImageFileName = Path.Combine(UserFileName, "Photo.Jpeg");
                string selfieImageSourceFile = Path.Combine(_SourcePath, selfieImageFileName);

                Logs.WriteLogEntry("info", KioskId, $"Generated Selfie Image Source File Path: {selfieImageSourceFile}", _MethodName);

                selfieImageToBase64 = ConvertImageToBase64(selfieImageSourceFile, _MethodName, KioskId);

                if (selfieImageToBase64 != null)
                {
                    Logs.WriteLogEntry("info", KioskId, "Selfie Image Base64 conversion successful", _MethodName);
                }
                else
                {
                    Logs.WriteLogEntry("info", KioskId, "Selfie Image Base64 conversion failed", _MethodName);
                }

                #endregion

                string finalBaseImage = "";
                if (KioskId == "5" || KioskId == "7")
                {
                    finalBaseImage = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCADIAJYDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD3kk+p/OgE/wCTQetJVEEgOadUQODUgNJopMWiio5p4oE3yyKi+rHFIZJRWNN4ggQZiQye5YKKzpfF5DFY7YOfZ+lPlYrnS/aId+wyqG9CeakBFcNNrN3fNkQIBnp5e7+tSx3mpQndGxKd0Yn9M0+UXMdrmkrBtdaM8R8yMqwx/wDXqyurRFyGkAx6GjlDmNWiq0N7DLj5sZ6Z71ZpDTuFFFFABRRRQAUUCigCM0lBpKogWnqaZXPa7r72shtLMKZQPnc9E/8Ar0WGXr/XI7YvFCheReMnhQa5m61O5uW8yTBGMDPaqM13JI+B+8cnLEetVLmdEz9ouEX2xg00rBuWnilnwTLgdvmFQ+RGJAHuFznsSc1SWZAN8SSunXKLxWvYXEBTMkbK3bzHwRQAIhBRhKxXGNoP+TV1Rc+XmKR8A52tjH5nmrSmBIlkMYkP1LCrUUqTLtAUN/dBFK4mZTz3hjLvEp2j7yP0/Mc1XgfdKWZWZiQ3JrSntlkPPXqAx4H5UltbW0Y+UkODgEc5/E07iLsWGtyqls/qK1NN1Dzf3EvEijAPrWekirHnBOPz/nVV75C3/LSKQfdYrkfpRYEdbRWVpGtRahuhZgLhBlgOhHqK1ag0CiiigAFFFFAiI0lKetJVCI7iZbe3eVyAqKSSa8rubl7q9ZmbahO5m7V3niyYRaKVOcSOFOPz/pXnlztSRY0AHrTAsSXKZCR/KoHP9400yRRDfJEGx/EQMj3pimKBPMkwWJznvVG+1dDGVC5/ur/XNDGXW1HeSQ2FA7DH4Uw6jKBiMnB6jeP8MVhfv7pwAcKK0YbdsKJGLNmpbSKUWX4tVuBnY7Z7KjdPrjirEd/duMt5o2jIZjyT7VHa220jAGeuAOB+PWraRzSSoZDhAc4buKnmQ+Qsx3UskQeVQZMZ/wD11p2ExcYzhgd3Xg5qmtsBaFhjIx+VXNPRApjIGRz9RQpXYpRsjSeUYIkXaf0qjKh3kFMg/wAWSR9asO4yARux2PpVK425IWQYbsc5FaGYsJSxu4LqLAw2Gb1FdurB1DA5BGa86kfhYgDIFzg9mrs9Bmkm0uNpDkg4GfSlIa3NOikoqRi0UCigCI9aSlPWkqhHOeLjI1rbRIODIWJ78Dj+dcDdyTQli0YDng5r0DxEjNIh3EJtxn0Oa4/U3VIyCFfjjco4qgRzN3OFQEbh1xnpn196qQwSTMCc88mrK2sl3djj5R0HpXXaVoKkqZFwPcVlOfKawjcyrDS5JFGBgetbCaIyKM5IrrLbT4YkAVVH4VKbVPWudyudCicounugwFwvarH2cgDJxjtW81ovTdgelILWM4zg/hS5h8pRhg227Aj+Gs+VmilzXSCEBcYqheaeHzt4NClqDjoZqXmOG69qqSzksSOxJxntUc6PFKUkB9M1DI4UgnO44I55+orohO5yzhYsh0RDIgcqeq7uh+ldh4akD2EmOB5hwPwFcH9ti258uRWxg9s103hPUVmuGgTABX7uR1FadDM6+iiipGAooFFAEZ60lKetJVIRy/i9tkMbFd3BwPeuQMf+jR55kkB616Fr9mLnT2IQM6HIJ7Vxl2yb5G4zGuAOucVQIzfDcSz6ncEqCsJ2g+p9a7m3jHHQCuQ8JwmO2mnYYDtkH1qPU/EF5JM0Fou1FPXoTXNJc0jpi+WJ6BlUH3h+dRPOgPDj868qutU1LBDXSj/gRqpHq14GAa6Y/XNL2a7lKb7Hrv2oUwXcS5BOGritE1G6u5tq5lwPugZrX1K/vLKEmW2eNcfebkVnY1TN9LxGb74AqTzYn6uD+NeU3Os3MjELLwT2qGG/neTaboDPYVapozc2eo3dtb3I4Kkj0Ncd4gJs7m3J+4SV47GqVrLeRyeZHcHK9RitLU1fVtEaVl/exfNx7U0uVmb95WGQRieIgc7fnXjqO9dP4Us0W9abYBtTgjsa4+wmP2JWDHIXBFdv4QRkRyQQCMH610dDnZ1dFFFSMBRQKKAIj1ooPWkqxAyhlKsMg9RXnfiWNYRdR2x5BKkKOcn/APXXotcTrVl9mv7ot8wlIkWpbsXFJ3Fs7VbXSIIQOVjAPucVzWpQymUQ2sJyeWbH9a7JBvhUDqRUEmlySHIxg+lc/NZnRa551eaJI0SkSYkH3g2SD9OKh+w7YliQZcdSB1NegSaA8pwp6++BVmy0G1siHkAeUdPQVXtNBqC3KXgrRpLFGuJ8rJIMbSOgrodesxf6XNb5wWHBqxbRjAK96kuFBj61m7vUuy2PGm0+a1u2SZMbT0YcGpYtB+0ziTLBC2doX+ua9FudMtb8+XMgz2buKrroL2z4Dbk7VaqEOmjk49JvLeUSw7ioP3SMkD0+ldZp9oVt23oBvHK4q9FYCP5jjPtTmIUEVLlcLWORt9P+yave2oGYWwyegB616No9ibGyCNgsTurkmtWudRyg5bC13gGFA9q6E9DmklcWiiimSKKKBRSAibqfrSUHqfrSVZItY+uW/mGCQLkglT+Va9Vr+JprR1UZbqKUtioO0jn7U9FY9DitNJFQYFZZVoydqk49qhee42kiNz/wE1yyj1OuErGpdagkKEnArLg1WNFknnB2D7tZrLcTT77lJEhB7qRk1fnWC6i8gMvlkbTj6Z4pxi2XKcUiODxVb3MzRwnaV52sCM/SprnxJFBAXkYKg65rn7nw1MJM2pl3AjqRjv61GdC1JgXnGMDIO4H8KrkI9qrHS2ms2mpx5t2O/txita2vBKmG+8ODXO6TbRWBR3Uxsx29OPrWpP8AKxliyxzhlFRKFi4zTNV2XHIBrOuBtzjoaBLIV5Q1Xd2c7SCDRGLTJlJNaF3RoS18jEfKAWz+VdLWNokJDPKPu42j61qyzxQjMjqvsTzXQtjlluSUVWW/tWYASjJ9QRVkHIyOlMkBRSd6KBEZ6n6mkoPU/WjNUIKZK4jjZz2GafWfqUuFWId+TQ3ZDW5mSbljEgHBJFVTcS5wAn5VqzTQtarAqFtvRjxz61lTxsiF+oFYyutjaFnuZepamhXy5GVQpDHB5/KpNLMP9mjULg7Idu8b+2Rz/UVmXlmmoXeGx1zmrksJWDyXZ5Y8AbWckce1Ckkh8l2VLzxS53NauiLj5QRkn3P6itPw9q76pDMsvLxEZbbjOfUVi+TYmcItgkjtxgDk1votpodiSsSxPIc7FOST/wDWpp6DlBbIpTXyPfvbJKZH87ai56Hqf5VrTXEViqpx5jn/AAGf5VzwtLK5YlYMluTkcGo10WeObbaZK53eXuwAfWlzIUqbR0EF1O7ks2VJJAI7VoeX5ssaDqzAVTt4PJVWlIXAwBmtrSofMlM7D5V4X60K/UUrdC1K8emW3lQg72ORnt70230/zR51yzMzc7c/zrQaGJ23PGjH1Kg06rMr3M2+sIY7YyRjaV985qTS2ZrUg9FbAqG/uPPdbaH5jnnHr6Vft4RBAsY6jqfehAyYUULiimIgbqfqaKCeT9TTasQtQNZxvN5r7mPoTxSXVw0EQZQCScc062mM8Ic43Z5A7UaNj1JgoVdqgADsBWLfokc0gAAXGcD6VryzpCm5z9B61z2oyuYmkYf6wkZqJvQqCuzHgjXfnvWiLSORecgGq9nbEgzTHag5GT19/pUkuooDhB8nr61hyrqdPM27ImtNNtrWQyIvztwXbk/SorvQLO8n82ea4dz0ywwB6AYqvJrGz+IUy31fzrhYskFmA60m22Uko6lxdPitF2x9B0zUZd4txRsFhxkVLeyGALjcQxwTms6SUySBFySR2oSaYuZNGnZTvcRN5gBKnBOOtdTaYFrEAMDbXJbhp1hJLJgMAWI/pW7Z6rHLo1rcpgvLGCF9DW0X3OeSvsakkyRJukYKPes97me+Yx26lY+7Gmw20l24muGO09B6/wCArTjVY0CooVR0AqrEbEVrZx2y8fM56sasUlFAhRRQKKAIG6n6mkob7zfU00nFaISEliWaMo3Q1Q+xTRsfLlAHrkg1daTHWqFxenJWP86TSLSb2HLbAyL50m9icYz/AFrB1nUF/wCEg+w4zHHFkKOOeDW5pqGSd5XOSo7+ted+IboxeNA+eDJtP0xisp7WRrBWep00mr+UP9R0/wBv/wCtUJ14hsNa4Hr5n/1qrlCyhsc+lRSRB024waxU2a+ziXm1zAOLf8d//wBasUY8zzBnfnOc96bJuTg1HGuOc0OTYKKWx0cOrI0W24iJ46jBB/CmNqkEZxbWoDEckgL/AC61lxnOB2qZY+c96HNgqaINbuZW0m5kdssVwPQZ4roPBax6toMflkxeR+7Kn5skd88VzusRF9NeNfvOQAPxrV+GUpjS8tm/vZxV03oRUR1n9kkHmb/x3/69J/ZX/Tb/AMc/+vWyQDwaYYx2rRNGGpXt4hbwiMHOOpx1qXNIwK9RTC1Xa5JKvLfhRTYeWP0oqWBEcknA7mk2c5J/Cntyx+tITWlylEr3QHlnArCDEyHFbl4T5BA6msiKIk4xyTUM2jojX023K2pbPLmvKvGsDQ66ZMEYfNepvrWm2AW3mukR1GCoycflXJeNbGHUY47y1ZXSQcOOmRUSCN76lazlE8CNnqKmMeenWsfQpj5HlseVOOa2zjGR1rmejNzMuR8/SolGBnFXJmUt8wzUQUHoaAHRoMbhVlFwMmmxKMU+QhEJoAp3R82dEHQc1c8CgxeIryPnHJqvbwlpNx5zW74SsDHq95clcAgKK0hvYznsdn3ozSHrQRzxWxzi0xolb2PtQWI6/pSFwOtGoWEWNlPHNFSBge9FFwsU3b52+tItNP32z6ml6Kasoily+72qkUKLJJ0CgnP4VoAcU2aNTAylQQ1Io83lV5ZZJG/iOa3vDkjzJJp8yLJBtLgN/CavT6HG6kxNj2IrNNjcWkm9Q6MP4lqbWNHJSVijq+lnSNRWeIE20p6+jehqdWylbwVdRsmiuBlJBgk/wtWILaWGLY4O5DtPHXHesakbalQbasynJ9/nFKE461aa3Drx1pq2rE+1ZGgQRlgCOnenSIZGA7Cr0dvtjVAOWq5BprSOqqv1JppXJbsVLK0JYADJNdRp0QiUgDgDFRwWiQptH3v71YWu6jLBOttaTOnlj52U4ya6Ix5VqYv33ZHXmg9K4ew1++tX/fO08f8AdY8/nXVWerWl4gKSbWP8LcGqRDg0WjxzTacelNB5qiRyt834UUiH5z9KKQFbY5dvlPU9qGR8fdP5UUUxoVY24+U/lTpYWaPgHNFFAMriGQKfkb8qY0LHgofyoooAjEBVSBGRn2qjOp5S4iYA8K4HT60UVEjWJRltZI5Cu046jA6inxQSk/cP5UUVy9TZGhBCQR8hJ6Ditq3g8pBlfmPU0UVrSMarFuWMMTOBk44471zD2BkkaSRWZmOScUUVsRHYeunJ0EJJ/Gtax05YAGMfzfTpRRTBtmiQcdDTQrf3T+VFFBCFjBDnII4ooopAf//Z";
                }
                else
                {
                    finalBaseImage = selfieImageToBase64;
                }

                var requestData = (dynamic)null;
                foreach (var consumer in consumerList)
                {

                    requestData = new JObject
                    {
                        ["data"] = new JObject
                        {
                            ["rdaCustomerId"] = consumer["rdaCustomerProfileId"],
                            ["rdaCustomerAccInfoId"] = consumer["accountInformation"]["rdaCustomerAccInfoId"],
                            ["cnicFrontURL"] = DecryptPath,
                            ["livenessImage"] = finalBaseImage,
                            ["livenessFlag"] = true
                        }
                    };

                }

                //   Logs.WriteLogEntry("Info", KioskId, "jsonRequest !: " + JsonConvert.SerializeObject(requestData), _MethodName);
                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, accessToken, "");
                JObject LiveImageResponse = JObject.Parse(apiResponse.ResponseContent);
                var LiveImageData = LiveImageResponse["message"];

                var status = LiveImageData["status"]?.ToString();
                var description = LiveImageData["description"]?.ToString();
                var errorDetail = LiveImageData["errorDetail"]?.ToString();

                Logs.WriteLogEntry("Info", KioskId, "LiveImageData !: " + LiveImageData, _MethodName);


                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "LiveImageData " + description, _MethodName);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", description));
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error in Failed to Get Card Status!: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Something Went Wrong. Check Logs";
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));
            }

            return response.ToString();
        }

        #endregion 

        #region Occupation List
        public async Task<List<Dictionary<string, object>>> GetOccupationListAsync(string KioskId)
        {
            string _MethodName = "GetOccupationListAsync";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> occupationList = new List<Dictionary<string, object>>();

            try
            {

                string url = MyPdaUrl + ConfigurationManager.AppSettings["ListOfVariant"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);


                var requestData = new { data = new { codeTypeId = 1014 } };

                Logs.WriteLogEntry("Info", KioskId, "Sending Occupation List Request: " + requestData.ToString(), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var occupations = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "Occupation List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (occupations != null)
                    {
                        foreach (var occupation in occupations)
                        {
                            var occupationEntry = new Dictionary<string, object>
                        {
                            { "id", occupation["id"] },
                            { "name", occupation["name"] },
                            { "description", occupation["description"] }
                        };

                            occupationList.Add(occupationEntry);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return occupationList;
        }

        #endregion

        #region Profession List
        public async Task<List<Dictionary<string, object>>> GetProfessionListAsync(string KioskId)
        {
            string _MethodName = "GetProfessionListAsync";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> professionList = new List<Dictionary<string, object>>();

            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["ListOfVariant"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                var requestData = new { data = new { codeTypeId = 1016 } };

                Logs.WriteLogEntry("Info", KioskId, "Sending Profession List Request: " + requestData.ToString(), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var professions = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "Profession List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (professions != null)
                    {
                        foreach (var profession in professions)
                        {
                            var professionEntry = new Dictionary<string, object>
                    {
                        { "id", profession["id"] },
                        { "name", profession["name"] },
                        { "description", profession["description"] }
                    };

                            professionList.Add(professionEntry);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return professionList;
        }


        #endregion

        #region Town / Tehsil List
        public async Task<List<Dictionary<string, object>>> TownTehsilList(string KioskId)
        {
            string _MethodName = "TownTehsilList";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> tehsilList = new List<Dictionary<string, object>>();

            try
            {

                string url = MyPdaUrl + ConfigurationManager.AppSettings["TownTehsilList"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                var requestData = new { data = new { codeTypeId = 1016 } };

                Logs.WriteLogEntry("Info", KioskId, "Sending Town Tehsil List Request: " + Newtonsoft.Json.JsonConvert.SerializeObject(requestData), _MethodName);

                // Send API request
                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, KioskId, Newtonsoft.Json.JsonConvert.SerializeObject(requestData), "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var towntehsillist = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "Town / Tehsil List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (towntehsillist != null)
                    {
                        foreach (var towntehsil in towntehsillist)
                        {
                            var townTehsilEntry = new Dictionary<string, object>
                    {
                        { "districtName", towntehsil["districtName"] },
                        { "tehsilName", towntehsil["tehsilName"] },
                        { "tehsilId", towntehsil["tehsilId"] }
                    };

                            tehsilList.Add(townTehsilEntry);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return tehsilList;
        }



        #endregion

        #region Branches List

        public async Task<List<Dictionary<string, object>>> IslamicBranchList(string KioskId)
        {
            string _MethodName = "BranchList";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> branchList = new List<Dictionary<string, object>>();

            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["BranchList"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new
                {
                    data = new
                    {
                        branchName = "",
                        categoryType = "I",
                        latitude = (double?)null,
                        longitude = (double?)null,
                        distance = 40
                    }
                };

                Logs.WriteLogEntry("Info", KioskId, "Sending Islamic Branch List Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject responseJson = JObject.Parse(apiResponse.ResponseContent);
                    Logs.WriteLogEntry("Info", KioskId, "Sending Islamic Branch List Success Response", _MethodName);
                    if (responseJson["data"]?["branchList"] != null)
                    {
                        branchList = responseJson["data"]["branchList"]
                            .Select(branch => new Dictionary<string, object>
                            {
                                { "id", (int)(branch["id"] ?? 0) },
                                { "branchName", (string)(branch["branchName"] ?? "") },
                                { "branchCode", (string)(branch["branchCode"] ?? "") },
                                { "tBranchCode", (string)(branch["tBranchCode"] ?? "") },
                                { "cityName", (string)(branch["cityName"] ?? "") },
                                { "latitude", branch["latitude"]?.ToObject<double?>() ?? 0.0 },
                                { "longitude", branch["longitude"]?.ToObject<double?>() ?? 0.0 },
                                { "fcyBranch", (int)(branch["fcyBranch"] ?? 0) },
                                { "distance", branch["distance"]?.ToObject<double?>() ?? 0.0 }

                            })
                            .ToList();
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return branchList;
        }

        public async Task<List<Dictionary<string, object>>> ConventionalBranchList(string KioskId)
        {
            string _MethodName = "BranchList";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> branchList = new List<Dictionary<string, object>>();

            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["BranchList"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new
                {
                    data = new
                    {
                        branchName = "",
                        categoryType = "C",
                        latitude = (double?)null,
                        longitude = (double?)null,
                        distance = 40
                    }
                };

                Logs.WriteLogEntry("Info", KioskId, "Sending Conventional Branch List Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry("Info", KioskId, "Sending Conventional Branch List Success Response", _MethodName);
                    JObject responseJson = JObject.Parse(apiResponse.ResponseContent);
                    if (responseJson["data"]?["branchList"] != null)
                    {
                        branchList = responseJson["data"]?["branchList"]?
                            .Select(branch => new Dictionary<string, object>
                            {
                                { "id", (int)(branch["id"] ?? 0) },
                                { "branchName", (string)(branch["branchName"] ?? "") },
                                { "branchCode", (string)(branch["branchCode"] ?? "") },
                                { "tBranchCode", (string)(branch["tBranchCode"] ?? "") },
                                { "cityName", (string)(branch["cityName"] ?? "") },
                                { "latitude", branch["latitude"]?.ToObject<double?>() ?? 0.0 },
                                { "longitude", branch["longitude"]?.ToObject<double?>() ?? 0.0 },
                                { "fcyBranch", (int)(branch["fcyBranch"] ?? 0) },
                                { "distance", branch["distance"]?.ToObject<double?>() ?? 0.0 }

                            })
                            .ToList();
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return branchList;
        }


        #endregion 

        #region Gender List

        public async Task<List<Dictionary<string, object>>> GenderList(string KioskId)
        {
            string _MethodName = "GenderList";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> GenderList = new List<Dictionary<string, object>>();

            try
            {

                string url = MyPdaUrl + ConfigurationManager.AppSettings["ListOfVariant"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);


                var requestData = new { data = new { codeTypeId = 1006 } };


                Logs.WriteLogEntry("Info", KioskId, "Sending Gender List Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var genderList = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "Gender List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (genderList != null)
                    {
                        foreach (var gender in genderList)
                        {
                            var genderEntry = new Dictionary<string, object>
                        {
                            { "id", gender["id"] },
                            { "name", gender["name"] },
                            { "description", gender["description"] }
                        };

                            GenderList.Add(genderEntry);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return GenderList;
        }


        #endregion #region Gender List

        #region Account Purpose 

        public async Task<List<Dictionary<string, object>>> AccountPurpose(string KioskId)
        {
            string _MethodName = "AccountPurpose";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> AccountPurposeList = new List<Dictionary<string, object>>();

            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["ListOfVariant"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                var requestData = new { data = new { codeTypeId = 1081 } };


                Logs.WriteLogEntry("Info", KioskId, "Sending Account Purpose List Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var AccountList = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "AccountPurposeList List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (AccountList != null)
                    {
                        foreach (var AccountPurpose in AccountList)
                        {
                            var AccountPurposeEntry = new Dictionary<string, object>
                        {
                            { "id", AccountPurpose["id"] },
                            { "name", AccountPurpose["name"] },
                            { "description", AccountPurpose["description"] }
                        };

                            AccountPurposeList.Add(AccountPurposeEntry);

                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return AccountPurposeList;
        }

        #endregion

        #region Conventional Savings Account Variants List

        public async Task<List<Dictionary<string, object>>> ConventionalSavingsAccountVariants(string KioskId)
        {
            string _MethodName = "ConventionalSavingsAccountVariants";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> GenderList = new List<Dictionary<string, object>>();

            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["AccountVariantList"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                var requestData = new { data = new { codeTypeId = 1006, codeOrder = 2, codeDescription = "C" } };


                Logs.WriteLogEntry("Info", KioskId, "Sending Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var ConventionalSavingsAccountVariants = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "ConventionalSavingsAccountVariants List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (ConventionalSavingsAccountVariants != null)
                    {
                        foreach (var Account in ConventionalSavingsAccountVariants)
                        {
                            var genderEntry = new Dictionary<string, object>
                        {
                            { "id", Account["id"] },
                            { "name", Account["name"] },
                            { "description", Account["description"] }
                        };

                            GenderList.Add(genderEntry);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return GenderList;
        }


        #endregion

        #region Conventional Current Account Variants List

        public async Task<List<Dictionary<string, object>>> ConventionalCurrentAccountVariants(string KioskId)
        {
            string _MethodName = "ConventionalCurrentAccountVariants";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> GenderList = new List<Dictionary<string, object>>();

            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["AccountVariantList"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1082, codeOrder = 1, codeDescription = "C" } };


                Logs.WriteLogEntry("Info", KioskId, "Sending Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var ConventionalCurrentAccountVariants = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "ConventionalCurrentAccountVariants List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (ConventionalCurrentAccountVariants != null)
                    {
                        foreach (var Account in ConventionalCurrentAccountVariants)
                        {
                            var genderEntry = new Dictionary<string, object>
                        {
                            { "id", Account["id"] },
                            { "name", Account["name"] },
                            { "description", Account["description"] }
                        };

                            GenderList.Add(genderEntry);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return GenderList;
        }


        #endregion

        #region Islamic Savings Account Variants List

        public async Task<List<Dictionary<string, object>>> IslamicSavingsAccountVariants(string KioskId)
        {
            string _MethodName = "IslamicSavingsAccountVariants";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> GenderList = new List<Dictionary<string, object>>();

            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["AccountVariantList"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1082, codeOrder = 1, codeDescription = "C" } };


                Logs.WriteLogEntry("Info", KioskId, "Sending Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var IslamicSavingsAccountVariants = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "IslamicSavingsAccountVariants List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (IslamicSavingsAccountVariants != null)
                    {
                        foreach (var Account in IslamicSavingsAccountVariants)
                        {
                            var genderEntry = new Dictionary<string, object>
                        {
                            { "id", Account["id"] },
                            { "name", Account["name"] },
                            { "description", Account["description"] }
                        };

                            GenderList.Add(genderEntry);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return GenderList;
        }


        #endregion 

        #region Islamic Current Account Variants List

        public async Task<List<Dictionary<string, object>>> IslamicCurrentAccountVariants(string KioskId)
        {
            string _MethodName = "IslamicCurrentAccountVariants";
            APIHelper apiService = new APIHelper();
            List<Dictionary<string, object>> GenderList = new List<Dictionary<string, object>>();

            try
            {
                string url = MyPdaUrl + ConfigurationManager.AppSettings["AccountVariantList"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1082, codeOrder = 1, codeDescription = "C" } };


                Logs.WriteLogEntry("Info", KioskId, "Sending Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var IslamicCurrentAccountVariants = jsonResponse["data"];

                    Logs.WriteLogEntry("Info", KioskId, "IslamicCurrentAccountVariants List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry("Info", KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

                    if (IslamicCurrentAccountVariants != null)
                    {
                        foreach (var Account in IslamicCurrentAccountVariants)
                        {
                            var genderEntry = new Dictionary<string, object>
                        {
                            { "id", Account["id"] },
                            { "name", Account["name"] },
                            { "description", Account["description"] }
                        };

                            GenderList.Add(genderEntry);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Error", KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Parsing Error: " + ex.Message, _MethodName);
            }

            return GenderList;
        }


        #endregion

        #region Account Opeining Card Issuance

        #region ABL Debit Card Issuance
        public async Task<string> AOABLDebitCardIssuance(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "AOABLDebitCardIssuance";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            try
            {
                Logs.WriteLogEntry("info", KioskId, "ABLDebitCardIssuance Step 1: Validating Input Data" + request.ToString(), _MethodName);
                string CompanyCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CompanyCode")?.Value ?? string.Empty;
                string AccountNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountNumber")?.Value ?? string.Empty;
                string ProdCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("ProdCode")?.Value ?? string.Empty;
                string DpsScheme = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("DpsScheme")?.Value ?? string.Empty;
                string cardRequestType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cardRequestType")?.Value ?? string.Empty;
                string Address1 = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("Address1")?.Value ?? string.Empty;
                string Address2 = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("Address2")?.Value ?? string.Empty;
                string BankingModeId = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("BankingModeId")?.Value ?? string.Empty;
                string branchName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("Branch")?.Value ?? string.Empty;

                #region Find Company Code
                if (string.IsNullOrEmpty(CompanyCode))
                {
                    if (BankingModeId == "114202")
                    {
                        List<Dictionary<string, object>> islamicBranchList = await IslamicBranchList(KioskId);
                        if (islamicBranchList.Count > 0)
                        {
                            var matchedBranch = islamicBranchList
                                .FirstOrDefault(b => b.ContainsKey("branchName") &&
                                                     string.Equals(b["branchName"]?.ToString(), branchName, StringComparison.OrdinalIgnoreCase));

                            CompanyCode = matchedBranch != null && matchedBranch.ContainsKey("tBranchCode")
                                ? matchedBranch["tBranchCode"].ToString()
                                : null;

                            if (CompanyCode != null)
                            {

                                Logs.WriteLogEntry("Info", KioskId, $"tBranchCode for '{branchName}' is: {CompanyCode}", _MethodName);
                            }
                            else
                            {

                                Logs.WriteLogEntry("Info", KioskId, $"Branch '{branchName}' not found in Islamic branch list.", _MethodName);
                            }
                        }
                        else
                        {
                            Logs.WriteLogEntry("Info", KioskId, "Islamic Branch List Not Found!", _MethodName);
                        }
                    }
                    else
                    {
                        List<Dictionary<string, object>> conventionalBranchList = await ConventionalBranchList(KioskId);
                        if (conventionalBranchList.Count > 0)
                        {

                            var matchedBranch = conventionalBranchList
                                .FirstOrDefault(b => b.ContainsKey("branchName") &&
                                                     string.Equals(b["branchName"]?.ToString(), branchName, StringComparison.OrdinalIgnoreCase));

                            CompanyCode = matchedBranch != null && matchedBranch.ContainsKey("tBranchCode")
                                ? matchedBranch["tBranchCode"].ToString()
                                : null;

                            if (CompanyCode != null)
                            {
                                Logs.WriteLogEntry("Info", KioskId, $"tBranchCode for '{branchName}' is: {CompanyCode}", _MethodName);

                            }
                            else
                            {
                                Logs.WriteLogEntry("Info", KioskId, $"Branch '{branchName}' not found in Conventional branch list.", _MethodName);
                            }
                        }
                        else
                        {
                            Logs.WriteLogEntry("Info", KioskId, "Conventional Branch List Not Found!", _MethodName);
                        }
                    }
                }
                #endregion

                string TransactionId = GenerateTransactionId();
                DateTime dateTime = DateTime.Now;
                string formattedDate = dateTime.ToString("dd-MM-yyyy HH:mm:ss");

                string url = T24Url + ConfigurationManager.AppSettings["ABLDebitCardIssuance"].ToString();
                Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                string FinalAddressType = "NO";
                string FinalAddress1 = "";
                string FinalAddress2 = "";
                string FinalStatus = "20";

                if (cardRequestType == "Address")
                {
                    FinalStatus = "10";
                    FinalAddressType = "YES";
                    FinalAddress1 = Address1;
                    FinalAddress2 = Address2;
                }

                var requestPayload = new
                {
                    ABLDebitCardIssuanceReq = new
                    {
                        UserID = "XXXXX",
                        Password = "XXXXX",
                        ChannelType = "WEB",
                        ChannelSubType = "SSK",
                        TransactionType = "000",
                        TransactionSubType = "000",
                        TranDateAndTime = formattedDate,
                        Function = "DebitCardIssuance",
                        HostData = new
                        {
                            TransReferenceNo = TransactionId,
                            Company = CompanyCode,
                            TransactionId = AccountNumber,
                            Status = FinalStatus,
                            PackageType = ProdCode,
                            AtmReqType = "1",
                            DPS_Scheme = DpsScheme,
                            CustomerNature = "NTB",
                            AddressFlag = FinalAddressType,
                            DaoAtmAddr1 = FinalAddress1,
                            DaoAtmAddr2 = FinalAddress2,
                            DaoAtmAddr3 = "",
                            DaoAtmAddr4 = "",
                            DaoAtmAddr5 = ""
                        }
                    }
                };

                Logs.WriteLogEntry("info", KioskId, "Request Code: " + JsonConvert.SerializeObject(requestPayload), _MethodName);

                APIResponse aPIResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, KioskId, "");

                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    var responseData = JsonConvert.DeserializeObject<dynamic>(aPIResponse.ResponseContent);
                    var debitCardResponse = responseData?.ABLDebitCardIssuanceRsp;
                    Logs.WriteLogEntry("info", KioskId, "hostCode Data: " + responseData, _MethodName);

                    string hostCode = responseData?.ABLDebitCardIssuanceRsp?.HostData?.HostCode;
                    var hostDesc = responseData?.ABLDebitCardIssuanceRsp?.HostData?.HostDesc;

                    if (hostCode == "00")
                    {
                        Logs.WriteLogEntry("info", KioskId, "Status Code: " + debitCardResponse.StatusCode, _MethodName);
                        Logs.WriteLogEntry("info", KioskId, "StatusDesc: " + debitCardResponse.StatusDesc, _MethodName);
                        Logs.WriteLogEntry("info", KioskId, "STAN: " + debitCardResponse.STAN, _MethodName);


                        Logs.WriteLogEntry("info", KioskId, "Host Code: " + debitCardResponse.HostData, _MethodName);
                        Logs.WriteLogEntry("info", KioskId, "Host Description: " + debitCardResponse.StatusDesc, _MethodName);
                        Logs.WriteLogEntry("info", KioskId, "Transaction Reference No: " + debitCardResponse.HostData.TransReferenceNo, _MethodName);
                        Logs.WriteLogEntry("info", KioskId, "Transaction Reference No: " + debitCardResponse.HostData.HostCode, _MethodName);
                        Logs.WriteLogEntry("info", KioskId, "Transaction Reference No: " + debitCardResponse.HostData.HostDesc, _MethodName);


                        // Declare variables outside the loop
                        string MotherName = "";
                        string FatherName = "";
                        string CustomerType = "";
                        string AccountType = "";
                        string CurrencyCode = "";
                        string BranchCode = "";
                        string DefaultAccount = "";
                        string AccountStatus = "";
                        string BankIMD = "";
                        string Email = "";
                        string Nationality = "";
                        string DateOfBirth = "";

                        foreach (var item in debitCardResponse.HostData.field)
                        {
                            Logs.WriteLogEntry("info", KioskId, "Host Code 3: " + item.name + " - " + item.content, _MethodName);

                            // Assign values based on item name
                            if (item.name == "MOTHER.NAME") MotherName = item.content;
                            if (item.name == "HUSBAND.NAME") FatherName = item.content;
                            if (item.name == "CUSTOMER.NATURE") CustomerType = item.content;
                            if (item.name == "ACCOUNT.NATURE") AccountType = item.content;
                            if (item.name == "CURR.NO") CurrencyCode = item.content;
                            if (item.name == "CO.CODE") BranchCode = item.content;
                            if (item.name == "DEFAULT.ACCOUNT") DefaultAccount = item.content;
                            if (item.name == "STATUS") AccountStatus = item.content;
                            if (item.name == "BANK.IMD") BankIMD = item.content;
                            if (item.name == "CUST.EMAIL") Email = item.content;
                            if (item.name == "NATIONALITY") Nationality = item.content;
                            if (item.name == "BIRTH.DATE") DateOfBirth = item.content;
                        }

                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;

                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                        bodyElement.Add(
                            new XElement("RespMessage", APIResultCodes.Success),
                            new XElement("MotherName", MotherName),
                            new XElement("FatherName", FatherName),
                            new XElement("CustomerType", CustomerType),
                            new XElement("AccountType", AccountType),
                            new XElement("CurrencyCode", CurrencyCode),
                            new XElement("BranchCode", BranchCode),
                            new XElement("DefaultAccount", DefaultAccount),
                            new XElement("AccountStatus", AccountStatus),
                            new XElement("BankIMD", BankIMD),
                            new XElement("Email", Email),
                            new XElement("DateOfBirth", DateOfBirth),
                            new XElement("Nationality", Nationality),
                            new XElement("CardPrint", "Address")
                        );

                    }
                    else
                    {
                        string errorMessage = ExtractErrorMessage(responseData, KioskId);
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", ""));
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", errorMessage));
                    }
                }
                else
                {

                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));

                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"Exception in AOABLDebitCardIssuance: {ex.Message}", _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;

                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
            }
            return response.ToString();
        }

        #endregion

        #region Account Opening - Card Issuance
        public async Task<string> AOCardIssuance(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "CardIssuance";
            XDocument response = request.GetBasicResponseFromRequest();
            APIResponse accountApiiResponse = null;
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {

                string kioskID = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element("KioskIdentity").Value;
                Logs.WriteLogEntry("info", KioskId, "KIOSK ID: " + kioskID, _MethodName);

                string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
                Logs.WriteLogEntry("info", KioskId, "PC NAME: " + PcName, _MethodName);

                string[] parts = PcName.Split('|');

                string ComputerName = parts[0].Trim();
                string BranchCode = parts[1].Trim();

                Console.WriteLine($"Computer Name: {ComputerName}");
                Console.WriteLine($"Branch Code: {BranchCode}");




                Logs.WriteLogEntry("info", KioskId, "IRISCardIssuance Step 1: " + request.ToString(), _MethodName);
                string FullName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("FullName")?.Value ?? string.Empty;
                string MotherName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("MotherName")?.Value ?? string.Empty;
                string MobileNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("MobileNumber")?.Value ?? string.Empty;
                string FatherName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("FatherName")?.Value ?? string.Empty;
                string CardName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardName")?.Value ?? string.Empty;
                string ProductCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("ProductCode")?.Value ?? string.Empty;
                string AccountNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountNumber")?.Value ?? string.Empty;
                string AccountStatus = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountStatus")?.Value ?? string.Empty;
                string CNIC = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CNIC")?.Value ?? string.Empty;
                string DOB = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("DOB")?.Value ?? string.Empty;
                string Nationality = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("Nationality")?.Value ?? string.Empty;

                CNIC = CNIC.Replace("-", "");

                if (UETflag)
                {
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                    response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(
                       new XElement("RespMessage", APIResultCodes.Success));
                }
                else
                {

                    //int AccountCategoryCode = int.Parse(AccountCategory);
                    int from = 1000;
                    int to = 3015;
                    string AccountType = "10";

                    //if (AccountCategoryCode >= from && AccountCategoryCode <= to)
                    //{
                    //    AccountType = "20";
                    //}
                    //else
                    //{
                    //    AccountType = "10";
                    //}


                    Logs.WriteLogEntry("info", KioskId, "CardIssuance AccountType : " + AccountType, _MethodName);

                    string BankIMD = "";

                    switch (ProductCode)
                    {
                        case "0092":
                            BankIMD = "428638";
                            break;
                        case "0071":
                            BankIMD = "407572";
                            break;
                        case "0070":
                            BankIMD = "476215";
                            break;
                        case "0075":
                            BankIMD = "476215";
                            break;
                        case "0080":
                            BankIMD = "629240";
                            break;
                    }

                    string TrakingId = GenerateTransactionId();
                    int? isoCode = GetIsoCode("PKR");
                    Logs.WriteLogEntry("info", KioskId, "ISO Code Found Against Currency Code: " + isoCode, _MethodName);
                    string finaCurrenctCode = Convert.ToString(isoCode).ToString();

                    string ActivationDate = DateTime.Now.ToString("yyyyMMdd");
                    Logs.WriteLogEntry("info", KioskId, "CardIssuance ActivationDate : " + ActivationDate, _MethodName);



                    string URL = IrisUrl + ConfigurationManager.AppSettings["IRISCardIssuance"].ToString();
                    Logs.WriteLogEntry("Info", KioskId, $"{_MethodName} [URL]: {URL}", _MethodName);
                    InstantCard webService = new InstantCard();
                    webService.Url = URL;

                    // Log input request before service call
                    string requestLog = $@"
                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
                       <soapenv:Header/>
                       <soapenv:Body>
                          <tem:ImportCustomer>
                        <tem:ActionCode>A</tem:ActionCode>
                        <tem:CNIC>{CNIC}</tem:CNIC>
                        <tem:TrackingID>{TrakingId}</tem:TrackingID>
                        <tem:FullName>{FullName}</tem:FullName>
                        <tem:DateOfBirth>{DOB}</tem:DateOfBirth>
                        <tem:MothersName>{MotherName}</tem:MothersName>
                        <tem:BillingFlag>H</tem:BillingFlag>
                        <tem:MobileNumber>{MobileNumber}</tem:MobileNumber>
                        <tem:ActivationDate>{ActivationDate}</tem:ActivationDate>
                        <tem:FathersName>{FatherName}</tem:FathersName>
                        <tem:CardName>{FullName}</tem:CardName>
                        <tem:CustomerType>1</tem:CustomerType>
                        <tem:ProductCode>{ProductCode}</tem:ProductCode>
                        <tem:AccountNo>{AccountNumber}</tem:AccountNo>
                        <tem:AccountType>{AccountType}</tem:AccountType>
                        <tem:AccountCurrency>{finaCurrenctCode}</tem:AccountCurrency>
                        <tem:AccountStatus>00</tem:AccountStatus>
                        <tem:AccountTitle>{FullName}</tem:AccountTitle>
                        <tem:BankIMD>{BankIMD}</tem:BankIMD>
                        <tem:Branchcode>{BranchCode}</tem:Branchcode>
                        <tem:DefaultAccount>1</tem:DefaultAccount>
                        <tem:Title></tem:Title>
                        <tem:HomeAddress1></tem:HomeAddress1>
                        <tem:HomeAddress2></tem:HomeAddress2>
                        <tem:HomeAddress3></tem:HomeAddress3>
                        <tem:HomeAddress4></tem:HomeAddress4>
                        <tem:HomePostalCode></tem:HomePostalCode>
                        <tem:HomePhone></tem:HomePhone>
                        <tem:Email></tem:Email>
                        <tem:Company>Allied Bank Ltd</tem:Company>
                        <tem:OfficeAddress1></tem:OfficeAddress1>
                        <tem:OfficeAddress2></tem:OfficeAddress2>
                        <tem:OfficeAddress3></tem:OfficeAddress3>
                        <tem:OfficeAddress4></tem:OfficeAddress4>
                        <tem:OfficeAddress5></tem:OfficeAddress5>
                        <tem:OfficePhone></tem:OfficePhone>
                        <tem:PassportNo></tem:PassportNo>
                        <tem:Nationality></tem:Nationality>
                        <tem:OldCardNumber></tem:OldCardNumber>
                        </tem:ImportCustomer>
                       </soapenv:Body>
                    </soapenv:Envelope>";

                    Logs.WriteLogEntry("info", KioskId, requestLog, _MethodName);

                    string result = webService.ImportCustomer(
                         ActionCode: "A",
                         CNIC: CNIC,
                         TrackingID: TrakingId,
                         FullName: FullName,
                         DateOfBirth: DOB,
                         MothersName: MotherName,
                         BillingFlag: "H",
                         MobileNumber: MobileNumber,
                         ActivationDate: ActivationDate,
                         FathersName: FatherName,
                         CardName: FullName,
                         CustomerType: "1",
                         ProductCode: ProductCode,
                         AccountNo: AccountNumber,
                         AccountType: AccountType,
                         AccountCurrency: finaCurrenctCode,
                         AccountStatus: "00",
                         AccountTitle: FullName,
                         BankIMD: BankIMD,
                         Branchcode: BranchCode,
                         DefaultAccount: "1",
                         Title: "",
                         Prefered_Address_FLag: "",
                         HomeAddress1: "",
                         HomeAddress2: "",
                         HomeAddress3: "",
                         HomeAddress4: "",
                         HomePostalCode: "",
                         HomePhone: "",
                         Email: "",
                         Company: "Allied Bank Ltd",
                         OfficeAddress1: "",
                         OfficeAddress2: "",
                         OfficeAddress3: "",
                         OfficeAddress4: "",
                         OfficeAddress5: "",
                         OfficePostalCode: "",
                         OfficePhone: "",
                         PassportNo: "",
                         Nationality: Nationality,
                         OldCardNumber: ""
                    );

                    XDocument doc = XDocument.Parse(result);

                    string trackingID = doc.Root.Element("WebMethodResponse").Element("TrackingID")?.Value;
                    string responseCode = doc.Root.Element("WebMethodResponse").Element("ResponseCode")?.Value;
                    string responseDescription = doc.Root.Element("WebMethodResponse").Element("ResponseDescription")?.Value;

                    Logs.WriteLogEntry("info", KioskId, "CardIssuance API Response responseCode : " + responseCode, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "CardIssuance API Response trackingID : " + trackingID, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "CardIssuance API Response responseDescription : " + responseDescription, _MethodName);

                    if (responseDescription == "Success" && responseCode == "00")
                    {
                        Logs.WriteLogEntry("info", KioskId, "CardIssuance API Response Description is Success", _MethodName);

                        //CardInfo cardInfo = DecryptEmbossingFile(BranchCode, ProductCode, KioskId);
                        //Logs.WriteLogEntry("info", KioskId, cardInfo.CardHolderName, _MethodName);

                        //if (cardInfo != null)
                        //{

                        //    string Description = "";
                        //    HardwareResponse hardwareResponse = CardPersonalization(cardInfo, ComputerName, CardName, out Description, KioskId);
                        //    Logs.WriteLogEntry("Info", KioskId, "Personlization Response : " + hardwareResponse.description, _MethodName);
                        //    if (hardwareResponse.data.ToString() != "" && hardwareResponse.data != null)
                        //    {
                        //        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success),
                        //             new XElement("RequestId", hardwareResponse.data));

                        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Success;
                        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "IRIS Request Successfuly Send";
                        //    }
                        //    else
                        //    {
                        //        Logs.WriteLogEntry("Error", KioskId, "Data is Null  " + hardwareResponse.description, _MethodName);
                        //        var bodyElements = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                        //        bodyElements.Add(new XElement("Message", hardwareResponse.description));
                        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                        //        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = hardwareResponse.description;
                        //    }

                        //}

                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                        // response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", "Card Request Submited !"));
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Dear Customer Your Debit Card Request has been processed successfully."));
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                    }
                    else
                    {
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                        bodyElement.Add(new XElement("Message", responseDescription));

                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;

                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "UnableToProcessRequest"));
                    }


                }
            }
            catch (ArgumentNullException argEx)
            {
                Logs.WriteLogEntry("Error", KioskId, "ArgumentNullException in CardIssuance: " + argEx, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Missing required arguments: " + argEx.Message));
            }
            catch (InvalidOperationException invOpEx)
            {
                Logs.WriteLogEntry("Error", KioskId, "InvalidOperationException in CardIssuance: " + invOpEx, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Operation is not valid: " + invOpEx.Message));

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "General Exception in CardIssuance: " + ex, _MethodName);
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.APIResultCode).Value = APIResultCodes.Unsuccessful;
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ex.Message));
            }
            return response.ToString();
        }

        #endregion

        #endregion

        #endregion

        #region Functions
        public static string ExtractErrorMessage(dynamic responseJson, string KioskId)
        {
            string _MethodName = "ExtractErrorMessage";
            try
            {
                Logs.WriteLogEntry("info", KioskId, "Step 1: ", _MethodName);

                var rsp = responseJson?.ABLDebitCardIssuanceRsp ?? responseJson;

                if (rsp?["HostData"]?["HostCode"]?.ToString() == "00")
                {
                    return "Success";
                }

                Logs.WriteLogEntry("info", KioskId, "Extracting HostDesc", _MethodName);

                JToken hostDesc = rsp?["HostData"]?["HostDesc"];

                if (hostDesc == null)
                    return "UnableToProcessRequest";

                // Case 1: HostDesc is a string
                if (hostDesc.Type == JTokenType.String)
                {
                    return hostDesc.ToString();
                }

                // Case 2: HostDesc is an object with 'content'
                if (hostDesc.Type == JTokenType.Object)
                {
                    return hostDesc["content"]?.ToString();
                }

                // Case 3: HostDesc is an array of objects with 'content'
                if (hostDesc.Type == JTokenType.Array)
                {
                    var messages = new List<string>();
                    int count = 0;
                    bool flag = false;
                    string message = "";
                    foreach (var item in hostDesc)
                    {
                        if (count < 2)
                        {
                            var content = item?["content"];
                            if (content != null)
                            {
                                if (content.ToString().ToLower() == "atm required is not marked yes.")
                                {
                                    Logs.WriteLogEntry("info", KioskId, content.ToString(), _MethodName);
                                    message = content.ToString();
                                    return message;
                                }
                                messages.Add(content.ToString());
                            }
                            count++;
                        }
                    }
                    if (messages.Any())
                        return string.Join(" | ", messages);
                }
                return "Unknown error format.";
            }
            catch (Exception ex)
            {
                return $"Error parsing response: {ex.Message}";
            }
        }

        #region Import Excel

        public static DataTable ImportExcel(string KioskId)
        {
            //string filePath = ConfigurationManager.AppSettings["ExcelPath"].ToString();   

            string filePath = "C:\\inetpub\\wwwroot\\CEM\\Excel\\Account Mapping with Card.xlsx";

            Logs.WriteLogEntry("Info", KioskId, "Excel File Path: Step 1" + filePath, "ImportExcel");

            DataTable dataTable = new DataTable();

            // Ensure the file exists
            if (!System.IO.File.Exists(filePath))
            {
                Logs.WriteLogEntry("Info", KioskId, "Excel File Path Not Found: Step 2" + filePath, "ImportExcel");
                throw new FileNotFoundException("Excel file not found.");
            }

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;
                for (int col = 1; col <= colCount; col++)
                {
                    dataTable.Columns.Add(worksheet.Cells[1, col].Text);
                }
                for (int row = 2; row <= rowCount; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dataTable.Rows.Add(dataRow);
                }
                Logs.WriteLogEntry("Info", KioskId, "Excel Data: Step 2" + dataTable, "ImportExcel");
            }
            return dataTable;
        }

        #endregion

        #region ExtractAndLogValues
        static List<(string ID, string Content)> ExtractAndLogValues(string jsonResponse)
        {
            // List to hold the id-content pairs
            var idContentList = new List<(string ID, string Content)>();

            using (var reader = new JsonTextReader(new StringReader(jsonResponse)))
            {
                string currentKey = null;
                string id = null;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        currentKey = (string)reader.Value;
                    }
                    else if (reader.TokenType == JsonToken.String && currentKey != null)
                    {
                        if (currentKey == "id")
                        {
                            id = (string)reader.Value;
                        }
                        else if (currentKey == "content" && id == "ACCNO")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                        else if (currentKey == "content" && id == "PRODCODE")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                        else if (currentKey == "content" && id == "BALANCE")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                        else if (currentKey == "content" && id == "CODBRANCH")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                        else if (currentKey == "content" && id == "DESCRIPTION")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                        else if (currentKey == "content" && id == "ISSUANCE.AMOUNT")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                        else if (currentKey == "content" && id == "REPLACEMENT.AMOUNT")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                        else if (currentKey == "content" && id == "CCYDESC")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                        else if (currentKey == "content" && id == "STATUS")
                        {
                            idContentList.Add((id, (string)reader.Value));
                        }
                    }
                }
            }

            // Return the list of id-content pairs
            return idContentList;
        }

        #endregion

        #region Decrypt Embossing Files
        public static CardInfo DecryptEmbossingFile(string BranchCode, string ProductCode, string KioskId)
        {
            CardInfo cardList = new CardInfo();
            string VSMCardBaesUrl = ConfigurationManager.AppSettings["VSMCardBaesUrl"].ToString();
            string DraftedCardFiles = ConfigurationManager.AppSettings["DraftedCardFiles"].ToString();
            try
            {
                string passphrase = ConfigurationManager.AppSettings["passphrase"].ToString();
                string privateKey = ConfigurationManager.AppSettings["privateKey"].ToString();
                string DecryptedFilePath = ConfigurationManager.AppSettings["DecryptedFilePath"].ToString();

                if (!Directory.Exists(DraftedCardFiles))
                {
                    Directory.CreateDirectory(DraftedCardFiles);
                }

                Logs.WriteLogEntry("Info", KioskId, "Passphrase Key!: " + passphrase, "DecryptEmbossingFile");
                Logs.WriteLogEntry("Info", KioskId, "Decrypted File Path!: " + DecryptedFilePath, "DecryptEmbossingFile");
                Logs.WriteLogEntry("Info", KioskId, "Private Key!: " + privateKey, "DecryptEmbossingFile");
                Logs.WriteLogEntry("Info", KioskId, "VSMCard Baes Url!: " + VSMCardBaesUrl, "DecryptEmbossingFile");
                Logs.WriteLogEntry("Info", KioskId, "Drafted Card Files!: " + DraftedCardFiles, "DecryptEmbossingFile");
                DateTime startTime = DateTime.Now;

                bool fileFound = false;
                string expectedFileName = $"EN-{BranchCode}";
                string expectedFileName1 = BranchCode;

                Logs.WriteLogEntry("Info", KioskId, "InstantCardExportFiles Path!: " + VSMCardBaesUrl, "DecryptEmbossingFile");
                if (Directory.Exists(VSMCardBaesUrl))
                {
                    while ((DateTime.Now - startTime).TotalSeconds < 60)
                    {
                        string[] files = Directory.GetFiles(VSMCardBaesUrl);
                        var targetFile = files.Where(f => Path.GetFileName(f).Contains(ProductCode)).ToList();
                        Logs.WriteLogEntry("Info", KioskId, "targetFile !: " + targetFile.Count, "DecryptEmbossingFile");
                        if (targetFile.Any())
                        {
                            Thread.Sleep(5000);
                            foreach (string file in targetFile)
                            {
                                Logs.WriteLogEntry("Info", KioskId, "expectedFileName !: " + file, "DecryptEmbossingFile");
                                string Filename = Path.GetFileName(file);
                                if (Filename.StartsWith(expectedFileName) || Filename.StartsWith(expectedFileName1))
                                {
                                    VSMCardBaesUrl = Path.Combine(VSMCardBaesUrl, Path.GetFileName(file));
                                    Logs.WriteLogEntry("Info", KioskId, "File Found For Decrypt!: " + VSMCardBaesUrl, "DecryptEmbossingFile");
                                    fileFound = true;
                                }
                                if (fileFound)
                                {
                                    break;
                                }
                            }

                        }

                        if (fileFound)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, VSMCardBaesUrl + " Directory Not Found!: ", "DecryptEmbossingFile");
                }

                if (fileFound)
                {
                    string outputFile = $"{DecryptedFilePath}{BranchCode}{ProductCode}{DateTime.Now.ToString("ddMMyyyyHHmmss")}.txt";
                    Logs.WriteLogEntry("Info", KioskId, outputFile + "Decrypt File Path", "DecryptEmbossingFile");

                    Logs.WriteLogEntry("Info", KioskId, "Decrypt Step 1", "DecryptEmbossingFile");
                    PGPDecryptor.DecryptFile(VSMCardBaesUrl, outputFile, privateKey, passphrase);
                    string filePath = outputFile;
                    string fileContent = System.IO.File.ReadAllText(filePath);
                    string namePattern = "";
                    string cardPattern = "";
                    string cvv1Pattern = "";
                    string cvv2Pattern = "";
                    string iCvvPattern = "";
                    string memberSincePattern = "";
                    string track1Pattern = "";
                    string track2Pattern = "";

                    if (ProductCode == "0080")
                    {
                        namePattern = @"!\s""([^""]+)";
                        cardPattern = @"=(\d{6})(\d{16})=(\d{4})";
                        cvv1Pattern = @";\d{6}(\d{10,13})=(\d{4}\d{8})(\d{3})";
                        cvv2Pattern = @"@@(\d{2}/\d{2})(\d{3})";
                        iCvvPattern = @"@(\d{3})@@";
                        memberSincePattern = @"""(\d{4})";
                        track1Pattern = @"%B(\d{16})\^([^ ^]+(?: [^ ^]+)*)\s*\^(\d{9})";
                        track2Pattern = @";(\d{16})=(\d{7})";
                    }
                    else
                    {
                        namePattern = @"!\s""([^""]+)";
                        cardPattern = @";(\d{16})=(\d{7})(\d{3})";
                        cvv1Pattern = @";\d{6}(\d{10,13})=(\d{4}\d{8})(\d{3})";
                        cvv2Pattern = @"@@(\d{2}/\d{2})(\d{3})";
                        iCvvPattern = @"@(\d{3})@@";
                        memberSincePattern = @"""(\d{4})";
                        track1Pattern = @"%B(\d{16})\^([^ ^]+(?: [^ ^]+)*)\s*\^(\d{9})";
                        track2Pattern = @";(\d{16})=(\d{7})";
                    }
                   
                    MatchCollection nameMatches = Regex.Matches(fileContent, namePattern);
                    MatchCollection cardMatches = Regex.Matches(fileContent, cardPattern);
                    MatchCollection cvv1Matches = Regex.Matches(fileContent, cvv1Pattern);
                    MatchCollection cvv2Matches = Regex.Matches(fileContent, cvv2Pattern);
                    MatchCollection iCVVMatches = Regex.Matches(fileContent, iCvvPattern);
                    MatchCollection memberSinceMatches = Regex.Matches(fileContent, memberSincePattern);
                    MatchCollection track1Matches = Regex.Matches(fileContent, track1Pattern);
                    MatchCollection track2Matches = Regex.Matches(fileContent, track2Pattern);

                    int recordCount;
                    if (ProductCode == "0080")
                    {
                             recordCount = new[] {
                             nameMatches.Count,
                             cardMatches.Count,
                             cvv1Matches.Count,
                             cvv2Matches.Count,
                             iCVVMatches.Count,
                             memberSinceMatches.Count,
                             track1Matches.Count,
                             track2Matches.Count,
                             }.Min();
                    }
                    else
                    {
                             recordCount = new[] {
                             nameMatches.Count,
                             cardMatches.Count,
                             cvv2Matches.Count,
                             iCVVMatches.Count,
                             memberSinceMatches.Count,
                             track1Matches.Count,
                             track2Matches.Count
                             }.Min();
                    }
                        string name = "";
                        string cardNumber = "";
                        string cvv1 = "";
                        string cvv2 = "";
                        string icvv = "";
                        string membersince = "";
                        string track1 = "";
                        string track2 = "";
                        string validFromRaw = "";
                        string validThruRaw = "";
                        string validFrom = "";
                        string validThru = "";
                        string pan = "";

                    for (int i = 0; i < recordCount; i++)
                    {
                        if (ProductCode == "0080")
                        {
                            Logs.WriteLogEntry("Info", KioskId, "Going to Get Co-Bage Card Data :" + ProductCode, "DecryptEmbossingFile");
                            name = nameMatches[i].Groups[1].Value.Trim();
                            cardNumber = cardMatches[i].Groups[2].Value;
                            cvv1 = cvv1Matches[i].Groups[3].Value;
                            cvv2 = cvv2Matches[i].Groups[2].Value;
                            icvv = iCVVMatches[i].Groups[1].Value;
                            membersince = memberSinceMatches[i].Groups[1].Value;
                            track1 = track1Matches[i].Groups[0].Value;
                            track2 = track2Matches[i].Groups[0].Value;
                            validFromRaw = cardMatches[i].Groups[1].Value;
                            validThruRaw = cardMatches[i].Groups[3].Value;
                            validFrom = $"{validFromRaw.Substring(2, 2)}/{validFromRaw.Substring(0, 2)}";
                            validThru = $"{validThruRaw.Substring(2, 2)}/{validThruRaw.Substring(0, 2)}";
                            pan = Regex.Replace(cardNumber, ".{4}", "$0 ");
                        }
                        else
                        {
                            Logs.WriteLogEntry("Info", KioskId, "Going to Get VISA Card Data :" + ProductCode, "DecryptEmbossingFile");
                            name = nameMatches[i].Groups[1].Value.Trim();
                            cardNumber = cardMatches[i].Groups[1].Value;
                            cvv1 = cardMatches[i].Groups[3].Value;
                            cvv2 = cvv2Matches[i].Groups[2].Value;
                            icvv = iCVVMatches[i].Groups[1].Value;
                            membersince = memberSinceMatches[i].Groups[1].Value;
                            track1 = track1Matches[i].Groups[0].Value;
                            track2 = track2Matches[i].Groups[0].Value;
                            validFromRaw = cvv2Matches[i].Groups[1].Value;
                            validThruRaw = cardMatches[i].Groups[2].Value;
                            validThru = $"{validThruRaw.Substring(2, 2)}/{validThruRaw.Substring(0, 2)}";
                            var parts = validFromRaw.Split('/');
                            validFrom = $"{parts[1]}/{parts[0]}";
                            pan = Regex.Replace(cardNumber, ".{4}", "$0 ").Trim();
                        }
                        cardList = new CardInfo
                        {
                            PAN = pan,
                            CardHolderName = name,
                            MemberSince = validFrom,
                            Expiry = validThru,
                            Track1 = track1,
                            Track2 = track2,
                            CVV1 = cvv1,
                            CVV2 = cvv2,
                            ICVV = icvv,
                        };
                        cardList = new CardInfo
                        {
                            PAN = pan,
                            CardHolderName = name,
                            MemberSince = validFrom,
                            Expiry = validThru,
                            Track1 = track1,
                            Track2 = track2,
                            CVV1 = cvv1,
                            CVV2 = cvv2,
                            ICVV = icvv,
                        };

                        Logs.WriteLogEntry("Info", KioskId,
                            $"Decrypted Card Info:" +
                            $"\nCardHolderName: {cardList.CardHolderName}" +
                            $"\nPAN: {cardList.PAN}" +
                            $"\nMemberSince (ValidFrom): {cardList.MemberSince}" +
                            $"\nExpiry (ValidThru): {cardList.Expiry}" +
                            $"\nTrack1: {cardList.Track1}" +
                            $"\nTrack2: {cardList.Track2}" +
                            $"\nCVV1: {cardList.CVV1}" +
                            $"\nCVV2: {cardList.CVV2}" +
                            $"\nICVV: {cardList.ICVV}",
                            "DecryptEmbossingFile");

                    }
                    Logs.WriteLogEntry("Info", KioskId, "Decrypt Step 4", "DecryptEmbossingFile");
                    if (System.IO.File.Exists(VSMCardBaesUrl))
                    {
                        string fileName = Path.GetFileName(VSMCardBaesUrl);
                        string destinationPath = Path.Combine(DraftedCardFiles, fileName);
                        Logs.WriteLogEntry("Info", KioskId, "Complete Drafted Card Files Path:" + destinationPath, "DecryptEmbossingFile");
                        System.IO.File.Move(VSMCardBaesUrl, destinationPath);
                        Logs.WriteLogEntry("Info", KioskId, "File Move on Draft folder", "DecryptEmbossingFile");

                    }

                }

            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(VSMCardBaesUrl))
                {
                    string fileName = Path.GetFileName(VSMCardBaesUrl);
                    string destinationPath = Path.Combine(DraftedCardFiles, fileName);
                    Logs.WriteLogEntry("Error", KioskId, "Complete Drafted Card Files Path:" + destinationPath, "DecryptEmbossingFile");
                    System.IO.File.Move(VSMCardBaesUrl, destinationPath);
                    Logs.WriteLogEntry("Error", KioskId, "File Move on Draft folder", "DecryptEmbossingFile");

                }
                Logs.WriteLogEntry("Error", KioskId, "Failed to Decrypt Embossing File!: " + ex.Message, "DecryptEmbossingFile");
                Logs.WriteLogEntry("Error", KioskId, "Failed to Decrypt Embossing File!, Inner Exception: " + ex.InnerException, "DecryptEmbossingFile");
            }
            return cardList;
        }
        #endregion

        public async Task<VariantInfo> GetAccoutListWithAccountNames(int accountType, string accountPurpose, string bankingMode, string GenderID, string occupationID, DateTime DateOfBirth, string KioskId)
        {
            string _MethodName = "GetAccoutListWithAccountNames";
            APIHelper apiService = new APIHelper();
            VariantInfo variantInfo = new VariantInfo();
            try
            {
                // Fetch Gender List
                List<Dictionary<string, object>> genderList = await GenderList(KioskId);
                List<int> maleIds = new List<int>();
                List<int> femaleIds = new List<int>();

                if (genderList.Any())
                {
                    Logs.WriteLogEntry("Info", KioskId, "Gender List Found: " + string.Join(", ", genderList), _MethodName);

                    foreach (var gender in genderList)
                    {
                        if (gender.ContainsKey("id") && gender.ContainsKey("name") && int.TryParse(gender["id"].ToString(), out int genderId))
                        {
                            string genderValue = gender["name"].ToString().ToLower();
                            if (genderValue == "male") maleIds.Add(genderId);
                            else if (genderValue == "female") femaleIds.Add(genderId);
                        }
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "Gender List Not Found!", _MethodName);
                }

                Occupation occupationData = new Occupation();

                List<Dictionary<string, object>> occupationList = await GetOccupationListAsync(KioskId);

                if (occupationList.Any())
                {
                    Logs.WriteLogEntry("Info", KioskId, "Occupation List Found: " + string.Join(", ", occupationList), _MethodName);

                    foreach (var occupation in occupationList)
                    {
                        if (occupation.ContainsKey("id") && occupation.ContainsKey("name") && int.TryParse(occupation["id"].ToString(), out int occupationId))
                        {
                            string occupationValue = Convert.ToString(occupation["id"].ToString());

                            // Assign the occupation ID based on the occupation value if it's not already set
                            if (occupationValue.Contains("101401"))
                                occupationData.SelfEmployed = occupationId;

                            if (occupationValue.Contains("101402"))
                                occupationData.Salaried = occupationId;

                            if (occupationValue.Contains("101404"))
                                occupationData.Student = occupationId;

                            if (occupationValue.Contains("101405"))
                                occupationData.HouseWife = occupationId;

                            if (occupationValue.Contains("101406"))
                                occupationData.RetierdOrPensioner = occupationId;

                            if (occupationValue.Contains("101407"))
                                occupationData.Unemployed = occupationId;

                            if (occupationValue.Contains("101408"))
                                occupationData.DailyWager = occupationId;

                            if (occupationValue.Contains("101409"))
                                occupationData.SelfEmployedInformalSector = occupationId;
                        }
                        Logs.WriteLogEntry("Info", KioskId, "occupationData.DailyWager " + occupationData.DailyWager + " occupationData.SelfEmployed" + occupationData.SelfEmployed, _MethodName);
                    }
                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, "Occupation List Not Found!", _MethodName);
                }

                // Create ACCOUNTS_SELECTION_LIST dynamically
                var accountsSelectionList = new List<AccountVariant>
                {
                          

                            #region Asaan Digital Account
                            new AccountVariant
                            {
                                Id = 108243,
                                Name = "Asaan Digital Account",
                                BankingModeId = (int)BankingMode.CONVENTIONAL,
                                CustomerAccountTypeId = (int)AccountsTypes.CURRENT,
                                PurposeOfAccount = (int)EnumPurposeOfAccountIdList.SAVINGS,
                                MinAge = 18

                            },
                    #endregion

                            #region Asaan Digital Saving Account
                            new AccountVariant
                            {
                                Id = 108251,
                                Name = "Asaan Digital Saving Account",
                                BankingModeId = (int)BankingMode.CONVENTIONAL,
                                CustomerAccountTypeId = (int)AccountsTypes.SAVINGS,
                                PurposeOfAccount = (int)EnumPurposeOfAccountIdList.SAVINGS,
                                MinAge = 18
                            },
                            #endregion

                            #region Allied Islamic Asaan Savings Account
                            new AccountVariant
                            {
                                Id = 108226,
                                Name = "Allied Islamic Asaan Savings Account",
                                BankingModeId = (int)BankingMode.ISLAMIC,
                                CustomerAccountTypeId = (int)AccountsTypes.SAVINGS,
                                PurposeOfAccount = (int)EnumPurposeOfAccountIdList.SAVINGS,
                                MinAge = 18

                            },

                            #endregion
                   
                            #region Allied Islamic Asaan Digital Account
                            new AccountVariant
                            {
                                Id = 108247,
                                Name = "Allied Islamic Asaan Digital Account",
                                BankingModeId = (int)BankingMode.ISLAMIC,
                                CustomerAccountTypeId = (int)AccountsTypes.CURRENT,
                                PurposeOfAccount = (int)EnumPurposeOfAccountIdList.SAVINGS,
                                MinAge = 18

                            },

                    #endregion

                            #region Asaan Digital Remittance Account
                            new AccountVariant
                            {
                                Id = 108244,
                                Name = "Asaan Digital Remittance Account",
                                BankingModeId = (int)BankingMode.CONVENTIONAL,
                                CustomerAccountTypeId = (int)AccountsTypes.CURRENT,
                                PurposeOfAccount = (int)EnumPurposeOfAccountIdList.FOREIGN_REMITTANCE,
                                MinAge = 18
                            },

                    #endregion

                            #region Asaan Digital Remittance Saving Account
                            new AccountVariant
                            {
                                Id = 108252,
                                Name = "Asaan Digital Remittance Saving Account",
                                BankingModeId = (int)BankingMode.CONVENTIONAL,
                                CustomerAccountTypeId = (int)AccountsTypes.SAVINGS,
                                PurposeOfAccount = (int)EnumPurposeOfAccountIdList.FOREIGN_REMITTANCE,
                                MinAge = 18
                            },

                    #endregion

                            #region Allied Aitebar Asaan Digital Remittance Account (Remunerative Current) - Islamic
                            new AccountVariant
                            {
                                Id = 108248,
                                Name = "Allied Aitebar Asaan Digital Remittance Account (Remunerative Current) - Islamic",
                                BankingModeId = (int)BankingMode.ISLAMIC,
                                CustomerAccountTypeId = (int)AccountsTypes.SAVINGS,
                                PurposeOfAccount = (int)EnumPurposeOfAccountIdList.FOREIGN_REMITTANCE,
                                MinAge = 18
                            },

                            #endregion

                            #region Allied Aitebar Asaan Digital Remittance Account (Remunerative Current) - Islamic
                            new AccountVariant
                            {
                                Id = 108248,
                                Name = "Allied Aitebar Asaan Digital Remittance Account (Remunerative Current) - Islamic",
                                BankingModeId = (int)BankingMode.ISLAMIC,
                                CustomerAccountTypeId = (int)AccountsTypes.CURRENT,
                                PurposeOfAccount = (int)EnumPurposeOfAccountIdList.FOREIGN_REMITTANCE,
                                MinAge = 18
                            },

                            #endregion


                 
                };

                // Get the selected variant ID
                variantInfo = GetAsaanAccountVariantID(bankingMode, accountType, accountPurpose, DateOfBirth, accountsSelectionList, KioskId);


            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error :" + ex.Message, _MethodName);
            }

            return variantInfo;
        }

        public VariantInfo GetAsaanAccountVariantID(string bankingModeId, int CustomerAccountTypeId, string purposeOfAccountId, DateTime DateOfBirth, List<AccountVariant> accountsSelectionList, string KioskId)
        {
            VariantInfo variantInfo = new VariantInfo();
            string _MethodName = "GetAsaanAccountVariantID";

            try
            {
                int BankingModeId = int.Parse(bankingModeId);
                int PurposeOfAccountId = int.Parse(purposeOfAccountId);
                int consumerAge = GetAgeCountFromDate(DateOfBirth);

                Logs.WriteLogEntry("info", KioskId, "consumerAge" + consumerAge, _MethodName);

                List<AccountVariant> variantsFiltered = new List<AccountVariant>();
                foreach (var item in accountsSelectionList)
                {
                    Logs.WriteLogEntry("info", KioskId, "BankingModeId - " + item.BankingModeId + "_User -" + bankingModeId, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "CustomerAccountTypeId - " + item.CustomerAccountTypeId + "_User -" + CustomerAccountTypeId, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "PurposeOfAccount - " + item.PurposeOfAccount + "_User -" + PurposeOfAccountId, _MethodName);

                }

                Logs.WriteLogEntry("info", KioskId, "consumerAge step 1 " + consumerAge, _MethodName);
                variantsFiltered = accountsSelectionList.Where(variant =>
                    variant.BankingModeId == BankingModeId &&
                    variant.CustomerAccountTypeId == CustomerAccountTypeId &&
                    variant.PurposeOfAccount == PurposeOfAccountId &&
                    variant.MinAge >= 18

                ).ToList();


                Logs.WriteLogEntry("info", KioskId, "Step 2", _MethodName);
                if (variantsFiltered.Any())
                {
                    Logs.WriteLogEntry("info", KioskId, "Step 3", _MethodName);
                    variantInfo.Id = variantsFiltered.First().Id;
                    variantInfo.Name = variantsFiltered.First().Name;

                    Logs.WriteLogEntry("info", KioskId, "variantID " + variantInfo.Id, _MethodName);
                    Logs.WriteLogEntry("info", KioskId, "variant Name  " + variantInfo.Name, _MethodName);
                }
                else
                {
                    Logs.WriteLogEntry("info", KioskId, "Variant Not Found", _MethodName);
                }


            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, "Error :" + ex.Message, _MethodName);
            }

            return variantInfo;
        }

        private int GetAgeCountFromDate(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public async Task<Card> FreshCardListing(string CnicNumber, string AccountNumber, string KioskId)
        {
            string _MethodName = "FreshCardListing";
            Card freshCardList = null;

            try
            {
                string url = IrisUrl + ConfigurationManager.AppSettings["IRISExistingCardList"];
                Logs.WriteLogEntry("Info", KioskId, $"Request URL: {url}", _MethodName);

                wsABLCARDSTATUSCHANGE webService = new wsABLCARDSTATUSCHANGE { Url = url };
                var result = webService.FreshCardListing(CnicNumber);
                string innerXml = XMLHelper.ExtractInnerXml(result);
                string cleanedXml = XMLHelper.FixNestedCardInfo(innerXml);

                Logs.WriteLogEntry("Info", KioskId, $"Cleaned XML: {cleanedXml}", _MethodName);

                var responseObject = XMLHelper.DeserializeXml<Root>(cleanedXml);

                if ((responseObject?.Output?.Cards != null && responseObject.WebMethodResponse?.ResponseDescription == "Approved") || responseObject?.WebMethodResponse?.ResponseDescription?.ToLower() == "invalid cnic")
                {
                    var freshCards = responseObject.Output.Cards.Where(c => c.ACCOUNTID == AccountNumber).ToList();

                    if (freshCards.Any())
                    {
                        foreach (var card in freshCards)
                        {
                            freshCardList = new Card()
                            {
                                CARDEXPIRYDATE = card.CARDEXPIRYDATE,
                                ACCOUNTID = card.ACCOUNTID,
                                CARDNAME = card.CARDNAME,
                                PRODUCTCODE = card.PRODUCTCODE,
                                CARDNUMBER = card.CARDNUMBER,
                                CARDSTATUS = card.CARDSTATUS

                            };
                            Logs.WriteLogEntry("Info", KioskId, $"Fresh card found — ProductCode: {freshCardList.PRODUCTCODE}, AccountID: {freshCardList.ACCOUNTID}, CardStatus: {freshCardList.CARDSTATUS}, Expiry: {freshCardList.CARDEXPIRYDATE}", _MethodName);
                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry("Info", KioskId, $"No fresh cards found Againts {AccountNumber} Account Number ", _MethodName);
                    }

                }
                else
                {
                    Logs.WriteLogEntry("Info", KioskId, $"No fresh cards found or response not approved for CNIC: {CnicNumber}", _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry("Error", KioskId, $"Exception in FreshCardListing: {ex}", _MethodName);
            }

            return freshCardList;
        }

        public static string ExtractDigitsOnly(string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        //private VariantInfo GetDefaultAsaanAccountVariantID(int bankingModeId, int customerAccountTypeId, int purposeOfAccountId, List<AccountVariant> accountsSelectionList, string KioskId)
        //{
        //    VariantInfo variantInfo = new VariantInfo();
        //    try
        //    {
        //        var defaultVariant = accountsSelectionList.FirstOrDefault(v =>
        //      v.BankingModeId == bankingModeId &&
        //      v.CustomerAccountTypeId == customerAccountTypeId &&
        //      v.PurposeOfAccount == purposeOfAccountId);

        //        variantInfo.Id = defaultVariant?.Id ?? 0;
        //        variantInfo.Name = defaultVariant?.Name ?? "";
        //    }
        //    catch (Exception ex)
        //    {

        //        Logs.WriteLogEntry("Error", KioskId, "Error :" + ex.Message, "GetDefaultAsaanAccountVariantID");
        //    }



        //    return variantInfo;
        //}

        public VariantInfo GetDefaultAsaanAccountVariantID(int bankingModeId, int customerAccountTypeId, int purposeOfAccountId, string KioskId)
        {
            VariantInfo variantInfo = new VariantInfo();
            int variantID = 0;
            string variantName = "";

            try
            {
                if (bankingModeId == (int)BankingMode.CONVENTIONAL)
                {
                    if (purposeOfAccountId == (int)EnumPurposeOfAccountIdList.FOREIGN_REMITTANCE)
                    {
                        variantID = 108244;
                        variantName = "Asaan Digital Remittance Account";
                    }
                    else if (customerAccountTypeId == (int)AccountsTypes.CURRENT)
                    {
                        variantID = 108243;
                        variantName = "Asaan Digital Account";
                    }
                    else
                    {
                        variantID = 108215;
                        variantName = "Allied Asaan Account - Saving";
                    }
                }
                else if (bankingModeId == (int)BankingMode.ISLAMIC)
                {
                    if (purposeOfAccountId == (int)EnumPurposeOfAccountIdList.FOREIGN_REMITTANCE)
                    {
                        variantID = 108248;
                        variantName = "Allied Aitebar Asaan Digital Remittance Account (Remunerative Current) - Islamic";
                    }
                    else if (customerAccountTypeId == (int)AccountsTypes.CURRENT)
                    {
                        variantID = 108247;
                        variantName = "Allied Islamic Asaan Digital Account";
                    }
                    else
                    {
                        variantID = 108226;
                        variantName = "Allied Islamic Asaan Savings Account";
                    }
                }

                if (variantName != "" && variantID != 0)
                {
                    variantInfo.Id = variantID;
                    variantInfo.Name = variantName;

                }

            }
            catch (Exception ex)
            {

                Logs.WriteLogEntry("Error", KioskId, "Error :" + ex.Message, "GetDefaultAsaanAccountVariantID");
            }
            return variantInfo;
        }


        public static dynamic GetCustomerDetails(string jsonResponse)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                //  var json = JsonConvert.SerializeObject(apiResponse.ResponseContent);
                return data["data"]["consumerList"];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return null;
            }
        }
        public static string ConvertImageToBase64(string filePath, string methodName, string KioskId)
        {
            if (System.IO.File.Exists(filePath))
            {
                Logs.WriteLogEntry("info", KioskId, $"File Found: {filePath}", methodName);

                try
                {
                    using (Image image = Image.FromFile(filePath))
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, image.RawFormat);
                        byte[] imageBytes = memoryStream.ToArray();
                        Logs.WriteLogEntry("info", KioskId, "Image processed successfully", methodName);
                        return Convert.ToBase64String(imageBytes);
                    }
                }
                catch (Exception ex)
                {
                    Logs.WriteLogEntry("error", KioskId, $"Error processing image: {ex.Message}", methodName);
                    return null;
                }
            }
            else
            {
                Logs.WriteLogEntry("info", KioskId, $"File Not Found: {filePath}", methodName);
                return null;
            }
        }
        public static int? GetIsoCode(string currencyCode)
        {
            Dictionary<string, int> currencyMap = new Dictionary<string, int>
        {
            { "AED", 784 }, { "AFN", 971 }, { "ALL", 8 }, { "AMD", 51 }, { "ANG", 532 },
            { "AOA", 973 }, { "ARS", 32 }, { "AUD", 36 }, { "AWG", 533 }, { "AZN", 944 },
            { "BAM", 977 }, { "BBD", 52 }, { "BDT", 50 }, { "BGN", 975 }, { "BHD", 48 },
            { "BIF", 108 }, { "BMD", 60 }, { "BND", 96 }, { "BOB", 68 }, { "BOV", 984 },
            { "BRL", 986 }, { "BSD", 44 }, { "BTN", 64 }, { "BWP", 72 }, { "BYN", 933 },
            { "BZD", 84 }, { "CAD", 124 }, { "CDF", 976 }, { "CHF", 756 }, { "CNY", 156 },
            { "COP", 170 }, { "CRC", 188 }, { "CUC", 931 }, { "CUP", 192 }, { "CVE", 132 },
            { "CZK", 203 }, { "DJF", 262 }, { "DKK", 208 }, { "DOP", 214 }, { "DZD", 12 },
            { "EGP", 818 }, { "ERN", 232 }, { "ETB", 230 }, { "EUR", 978 }, { "FJD", 242 },
            { "GBP", 826 }, { "GEL", 981 }, { "GHS", 936 }, { "GIP", 292 }, { "GMD", 270 },
            { "GNF", 324 }, { "GTQ", 320 }, { "GYD", 328 }, { "HKD", 344 }, { "HNL", 340 },
            { "HRK", 191 }, { "HTG", 332 }, { "HUF", 348 }, { "IDR", 360 }, { "ILS", 376 },
            { "INR", 356 }, { "IQD", 368 }, { "IRR", 364 }, { "ISK", 352 }, { "JMD", 388 },
            { "JOD", 400 }, { "JPY", 392 }, { "KES", 404 }, { "KGS", 417 }, { "KHR", 116 },
            { "KMF", 174 }, { "KPW", 408 }, { "KRW", 410 }, { "KWD", 414 }, { "KYD", 136 },
            { "KZT", 398 }, { "LAK", 418 }, { "LBP", 422 }, { "LKR", 144 }, { "LRD", 430 },
            { "LSL", 426 }, { "LYD", 434 }, { "MAD", 504 }, { "MDL", 498 }, { "MGA", 969 },
            { "MKD", 807 }, { "MMK", 104 }, { "MNT", 496 }, { "MOP", 446 }, { "MRO", 478 },
            { "MUR", 480 }, { "MVR", 462 }, { "MWK", 454 }, { "MXN", 484 }, { "MYR", 458 },
            { "MZN", 943 }, { "NAD", 516 }, { "NGN", 566 }, { "NIO", 558 }, { "NOK", 578 },
            { "NPR", 524 }, { "NZD", 554 }, { "OMR", 512 }, { "PAB", 590 }, { "PEN", 604 },
            { "PGK", 598 }, { "PHP", 608 }, { "PKR", 586 }, { "PLN", 985 }, { "PYG", 600 },
            { "QAR", 634 }, { "RON", 946 }, { "RSD", 941 }, { "RUB", 643 }, { "RWF", 646 },
            { "SAR", 682 }, { "SBD", 90 }, { "SCR", 690 }, { "SDG", 938 }, { "SEK", 752 },
            { "SGD", 702 }, { "SHP", 654 }, { "SLL", 694 }, { "SOS", 706 }, { "SRD", 968 },
            { "SSP", 728 }, { "STD", 678 }, { "SVC", 222 }, { "SYP", 760 }, { "SZL", 748 },
            { "THB", 764 }, { "TJS", 972 }, { "TMT", 934 }, { "TND", 788 }, { "TOP", 776 },
            { "TRY", 949 }, { "TTD", 780 }, { "TWD", 901 }, { "TZS", 834 }, { "UAH", 980 },
            { "UGX", 800 }, { "USD", 840 }, { "UYU", 858 }, { "UZS", 860 }, { "VEF", 937 },
            { "VND", 704 }, { "VUV", 548 }, { "WST", 882 }, { "XAF", 950 }, { "XCD", 951 },
            { "XOF", 952 }, { "XPF", 953 }, { "YER", 886 }, { "ZAR", 710 }, { "ZMW", 967 },
            { "ZWL", 932 }
        };

            return currencyMap.TryGetValue(currencyCode, out int isoCode) ? isoCode : (int?)null;
        }

        public static string Decrypt(string cipherText)
        {
            byte[] Key = Encoding.UTF8.GetBytes("4dweqdxcerfvc3rw");
            byte[] IV = Encoding.UTF8.GetBytes("0000000000000000");
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string EncryptUsingAES256(string message)
        {
            string key = "4dweqdxcerfvc3rw";

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[16];

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(message);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }
        private static void ConnectToNetworkShare(string networkPath, string username, string password, string KioskId)
        {
            var psi = new ProcessStartInfo("net", $"use {networkPath} /user:{username} {password}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Logs.WriteLogEntry("Info", KioskId, "Network share connect output: " + output, "ConnectToNetworkShare");
                if (!string.IsNullOrEmpty(error))
                {
                    Logs.WriteLogEntry("Error", KioskId, "Network share connect error: " + error, "ConnectToNetworkShare");
                }
            }
        }
        public class CardInfo
        {
            public string PAN { get; set; }
            public string CardHolderName { get; set; }
            public string CVV1 { get; set; }
            public string CVV2 { get; set; }
            public string ICVV { get; set; }
            public string Expiry { get; set; }
            public string MemberSince { get; set; }
            public string Track1 { get; set; }
            public string Track2 { get; set; }
        }


        public XDocument SetResponse(XDocument response, string resultCode, string apiResultCode, string message)
        {
            response.Element(TransactionTags.Response)
                    .Element(TransactionTags.Header)
                    .Element(TransactionTags.ResultCode).Value = resultCode;

            response.Element(TransactionTags.Response)
                    .Element(TransactionTags.Header)
                    .Element(TransactionTags.APIResultCode).Value = apiResultCode;

            response.Element(TransactionTags.Response)
                    .Element(TransactionTags.Body)
                    .Add(new XElement("RespMessage", apiResultCode));

            if (!string.IsNullOrEmpty(message))
            {
                response.Element(TransactionTags.Response)
                        .Element(TransactionTags.Body)
                        .Add(new XElement("Message", message));
            }
            return response;
        }

        #endregion



        public class VariantInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

    }
}

public enum BankingMode
{
    CONVENTIONAL = 114201,
    ISLAMIC = 114202
}

public enum AccountsTypes
{
    CURRENT = 114301,
    SAVINGS = 114302
}

public enum EnumPurposeOfAccountIdList
{
    SAVINGS = 108106,

    FOREIGN_REMITTANCE = 108104
}

public enum EnumGenderIdList
{
    MALE = 1,
    FEMALE = 2
}

// Model representing an account variant.
public class AccountVariant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int BankingModeId { get; set; }
    public int CustomerAccountTypeId { get; set; }
    public int PurposeOfAccount { get; set; }
    public int MinAge { get; set; }
    public int? MaxAge { get; set; }
    public List<int> Genders { get; set; }
    public List<int> Occupations { get; set; }
}

public class Occupation
{
    public int SelfEmployed { get; set; }
    public int Salaried { get; set; }
    public int Student { get; set; }
    public int HouseWife { get; set; }
    public int RetierdOrPensioner { get; set; }
    public int Unemployed { get; set; }
    public int DailyWager { get; set; }
    public int SelfEmployedInformalSector { get; set; }

}
public class ImpersonationHelper
{
    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool LogonUser(
        string lpszUsername,
        string lpszDomain,
        string lpszPassword,
        int dwLogonType,
        int dwLogonProvider,
        out IntPtr phToken);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
    private const int LOGON32_PROVIDER_WINNT50 = 3;

    public static bool Impersonate(string domain, string username, string password, Action action)
    {
        IntPtr userToken = IntPtr.Zero;

        bool success = LogonUser(
            username,
            domain,
            password,
            LOGON32_LOGON_NEW_CREDENTIALS,
            LOGON32_PROVIDER_WINNT50,
            out userToken);

        if (!success)
        {
            Logs.WriteLogEntry("Info", "5", $"LogonUser failed: {Marshal.GetLastWin32Error()}", "Impersonate");

            return false;
        }

        try
        {
            if (ImpersonateLoggedOnUser(userToken))
            {
                using (var safeHandle = new SafeAccessTokenHandle(userToken))
                {
                    WindowsIdentity.RunImpersonated(safeHandle, () =>
                    {
                        action.Invoke();
                    });
                }

                return true;
            }
            else
            {
                Logs.WriteLogEntry("Info", "5", $"ImpersonateLoggedOnUser failed: {Marshal.GetLastWin32Error()}", "Impersonate");

                return false;
            }
        }
        finally
        {
            CloseHandle(userToken);
        }
    }



}
public class Card
{
    public string CARDNUMBER { get; set; }
    public string ACCOUNTID { get; set; }
    public string CARDNAME { get; set; }
    public string CARDSTATUS { get; set; }
    public string PRODUCTCODE { get; set; }
    public string CARDEXPIRYDATE { get; set; }
}

public class ApplicantData
{
    public PrimaryData Primary { get; set; }
}

public class PrimaryData
{
    public DateTime DateOfBirth { get; set; }
    public int GenderId { get; set; }
    public int OccupationId { get; set; }
}

public class AccountVariantCacheItem
{
    public int CodeId { get; set; }
}

