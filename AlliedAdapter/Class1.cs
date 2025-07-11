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
using static iTextSharp.text.pdf.PRTokeniser;
using Twilio.Jwt;


namespace AlliedAdapter
{
    public class Class1 : IBackendServerAdapter
    {
        #region Variables

        private const string DemoServiceUrlKey = "Url";
        private static List<AccountVariant> C_ASAAN_ACCOUNTS_SELECTION_LIST;
        public List<AccountVariantCacheItem> accountVariantsCache;
        private ApplicantData applicantData;
        bool UETflag = false;
        private IApplicationConfiguration _applicationConfiguration;
        private string _demoServiceUrl;
        private bool _isDemoServiceUrlFound;
        #endregion

        #region Api Response Constants
        public static class ApiResponseConstants
        {
            public const string SuccessStatus = "Success";
            public const string Message_AccountNotExist = "AccountNotExist";
            public const string Message_UnableToProcess = "UnableToProcessRequest";
            public const string ValidationSuccess = "Validation successful";
            public const string BioValidationSuccess = "Bio Validation successful";
            public const string BioValidationFailed = "Bio Validation Failed";
            public const string AccountFound = "Account Found !";
            public const string PrinterNotConnected = "PrinterNotConnected";
            public const string PrinterNotAvailable = "PrinterNotAvailable";
            public const string DoNotMeetCriteria = "DoNotMeetCriteria";
            public const string FreshCardNotAllowed = "FreshCardNotAllowed";
            public const string OtpSendFailed = "OtpSendFailed";
            public const string InvalidCNIC = "Invalid CNIC";
            public const string AccountAlreadyInProcess = "Dear Customer, your Asaan Account request is already in process.";
            public const string PmdFailed = "PmdFailed";
            public const string PleaseProvideValidOTP = "Please provide Valid OTP";
            public const string AccountDirectlyPushToT24 = "Account Directly Push to t24";
            public const string PleaseProceedAccountWithDesk = "please proceed account with desk";
            public const string ApplicationSubmitted = "Dear Customer your application has been submitted and currently under review. Bank will communicate the status of your application within two working days.";
        }
        #endregion ApiResponseConstants

        #region API Base URLs

        string MyPdaUrl = ConfigurationManager.AppSettings["MyPdaUrl"].ToString();
        string T24Url = ConfigurationManager.AppSettings["T24Url"].ToString();
        string IrisUrl = ConfigurationManager.AppSettings["IrisUrl"].ToString();

        #endregion

        #region Initialize
        public void Initialize()
        {
            try
            {
                Logs.WriteLogEntry(LogType.Info, "", "Initializing", nameof(Initialize));
                _applicationConfiguration = SharedObjectsLocator.Instance.Get<IApplicationConfiguration>().First();
                Logs.WriteLogEntry(LogType.Info, "", "_applicationConfiguration received", nameof(Initialize));
                Logs.WriteLogEntry(LogType.Info, "", "_applicationConfiguration: " + JsonConvert.SerializeObject(_applicationConfiguration), nameof(Initialize));

                _isDemoServiceUrlFound = _applicationConfiguration.ConfigurationList.TryGetValue(DemoServiceUrlKey, out _demoServiceUrl);
                Logs.WriteLogEntry(LogType.Info, "", "_demoServiceUrl: " + _demoServiceUrl, nameof(Initialize));
                Logs.WriteLogEntry(LogType.Info, "", "_isDemoServiceUrlFound: " + _isDemoServiceUrlFound, nameof(Initialize));
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, "", "Exception: " + ex.Message, nameof(Initialize));
            }
        }
        #endregion

        #region Call Backned
        public string CallBackEnd(XDocument request, string referenceNumber, RequestContent requestContent)
        {
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                string response = "";
                string requestType = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.RequestType).Value;
                Logs.WriteLogEntry(LogType.Info, KioskId, $"Request Type: {requestType.ToLower()}", nameof(CallBackEnd));

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
                    case "kgscardstatus":
                        response = Task.Run(() => GetCardStatus(request, referenceNumber)).Result;
                        break;
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
                    case "aoabldebitcardissuance":
                        response = Task.Run(() => AOABLDebitCardIssuance(request, referenceNumber)).Result;
                        break;
                    case "aoiriscardissuance":
                        response = Task.Run(() => AOCardIssuance(request, referenceNumber)).Result;
                        break;

                    default:
                        throw new Exception("Unknown request type: " + requestType);
                }
                Logs.WriteLogEntry(LogType.Info, KioskId, "Response: " + response, nameof(CallBackEnd));
                return response;
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Exception: " + ex, nameof(CallBackEnd));
                XDocument responseDoc = request.GetBasicResponseFromRequest();
                responseDoc.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Failed;
                responseDoc.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultDescription).Value = "Exception in backend call: " + ex.Message;
                responseDoc.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.Message).Value = "Technical error happened while calling our servers. Please try again later.";

                return responseDoc.ToString();
            }
        }
        #endregion

        #region Check BackEnd Heartbeat
        public bool CheckBackEndHeartbeat()
        {
            try
            {
                Logs.WriteLogEntry(LogType.Info, "", "Checking backend heartbeat", nameof(CheckBackEndHeartbeat));

                bool isBackendAlive = true;

                Logs.WriteLogEntry(LogType.Info, "", "Backend heartbeat check result: " + isBackendAlive, nameof(CheckBackEndHeartbeat));

                return isBackendAlive;
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, "", "Exception: " + ex.Message, nameof(CheckBackEndHeartbeat));
                throw;
            }
        }
        #endregion

        #region Card Issuance

        #region Customer Verification 
        public async Task<string> CustomerVerification(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "CustomerVerification";
            XDocument response = request.GetBasicResponseFromRequest();
            APIHelper apiService = new APIHelper();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 1]: Request received", _MethodName);

                string tncurl = ConfigurationManager.AppSettings["TNCURL"];
                string TransactionId = GenerateTransactionId();
                string formattedDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                string CnicNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cnic")?.Value ?? string.Empty;
                CnicNumber = CnicNumber.Replace("-", "");

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 2]: CNIC Number: {CnicNumber}", _MethodName);

                string url = T24Url + ConfigurationManager.AppSettings["CustomerVerification"];
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 3]: API URL: {url}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 4]: API Response Content: {ApiResponse.ResponseContent}", _MethodName);

                    var responseData = JsonConvert.DeserializeObject<ABLCustomerVerificationResponse>(ApiResponse.ResponseContent);
                    var verificationResponse = responseData?.ABLCustomerVerificationRsp;

                    if (verificationResponse != null && verificationResponse.StatusDesc == ApiResponseConstants.SuccessStatus)
                    {
                        var hostData = verificationResponse.HostData;
                        Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 5]: Host Data: {hostData}", _MethodName);

                        if (hostData?.HostDesc == null && hostData.CustomerNumber != null)
                        {
                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                            SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.ValidationSuccess);

                            string MobileNumber = ExtractDigitsOnly(hostData.PhoneNumber);
                            Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 6]: Formatted Mobile Number: {MobileNumber}", _MethodName);

                            bodyElement.Add(
                                new XElement("RespMessage", APIResultCodes.Success),
                                new XElement("Name", hostData.Name),
                                new XElement("CustomerNumber", hostData.CustomerNumber),
                                new XElement("DOB", hostData.DOB),
                                new XElement("PhoneNumber", MobileNumber),
                                new XElement("Email", hostData.Email),
                                new XElement("CNIC", hostData.CNIC),
                                new XElement("TransactionId", TransactionId),
                                new XElement("TNCURL", tncurl));
                        }
                        else
                        {
                            Logs.WriteLogEntry(LogType.Warning, KioskId, $"{_MethodName} [Step 7]: Verification Failed - Record Not Found", _MethodName);
                            SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_AccountNotExist);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(
                                new XElement("Message", ApiResponseConstants.Message_AccountNotExist),
                                new XElement("IsAvailable", "Not"),
                                new XElement("TNCURL", tncurl));
                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry(LogType.Warning, KioskId, $"{_MethodName} [Step 8]: API response unsuccessful or status != Success {verificationResponse?.StatusDesc}", _MethodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 9]: API Request Failed - StatusCode: {ApiResponse.StatusCode}, Message: {ApiResponse.Message}", _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 10]: Exception occurred: {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 1]: Received Request", _MethodName);

                string cnicNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cnic")?.Value ?? string.Empty;
                string fingerImage = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("fingerImage")?.Value ?? string.Empty;
                string contactNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("contactnumber")?.Value ?? string.Empty;
                string NumTry = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("NumTry")?.Value ?? string.Empty;

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 2]: Input - CNIC: {cnicNumber}, Contact: {contactNumber}, NumTry: {NumTry}", _MethodName);

                cnicNumber = cnicNumber.Replace("-", "");

                string FinalFingerIndex = NumTry == "1" ? "1" :
                                           NumTry == "2" ? "2" :
                                           NumTry == "3" ? "6" :
                                           NumTry == "4" ? "7" : "1";

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 3]: FingerIndex: {FinalFingerIndex}", _MethodName);

                var soapClient = new BioService.ATMMSGSetSOAP_HTTP_Service();
                var soapRequest = new BioService.complexType
                {
                    CNIC = cnicNumber,
                    FINGER_NO = FinalFingerIndex,
                    CONTACT_NO = contactNumber,
                    ISO_NADRA = fingerImage,
                    ISO_LOCAL = fingerImage,
                    FLAG = "3"
                };

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 4]: Sending request to SOAP Service at {soapClient.Url}", _MethodName);

                var soapResponse = soapClient.Operation1(soapRequest);

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 5]: SOAP Response - Code: {soapResponse.CODE}, Message: {soapResponse.MESSAGE}", _MethodName);

                var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                if (soapResponse.CODE == "100")
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 6]: BioVerification Success", _MethodName);
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.BioValidationSuccess);
                    bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, $"{_MethodName} [Step 6]: BioVerification Failed", _MethodName);
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.BioValidationFailed);
                    bodyElement.Add(new XElement("Message", ApiResponseConstants.BioValidationFailed));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 7]: Exception occurred: {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 8]: Final Response", _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 1]: Validating Input Data", _MethodName);

                string TransactionId = GenerateTransactionId();
                string formattedDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string CustomerNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CustomerNumber")?.Value ?? string.Empty;

                string url = T24Url + ConfigurationManager.AppSettings["ABLCustomerAccountList"];
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 2]: API URL: {url}", _MethodName);

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

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 3]: Request Payload Prepared", _MethodName);

                APIResponse aPIResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, KioskId, "");

                if (aPIResponse.StatusCode == HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 4]: Request Successful", _MethodName);

                    var responseData = JsonConvert.DeserializeObject<ABLCustomerAccountListResponse>(aPIResponse.ResponseContent);
                    var accountListResponse = responseData?.ABLCustomerAccountListRsp;

                    if (accountListResponse != null && accountListResponse.StatusDesc == ApiResponseConstants.SuccessStatus)
                    {
                        if (accountListResponse.HostData?.Account != null)
                        {
                            var idContentList = ExtractAndLogValues(aPIResponse.ResponseContent);
                            string Name = "";

                            foreach (var column in accountListResponse.HostData.Account.Column)
                            {
                                if (column.Id == "CUSTNAME")
                                    Name = column.Content;
                            }

                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                            SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.AccountFound);

                            bodyElement.Add(
                                new XElement("RespMessage", APIResultCodes.Success),
                                new XElement("AccountData", idContentList),
                                new XElement("Name", Name)
                            );

                            Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 5]: Account Data Added", _MethodName);
                        }
                        else
                        {
                            Logs.WriteLogEntry(LogType.Warning, KioskId, $"{_MethodName} [Step 6]: No Account Data Found", _MethodName);
                            SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_AccountNotExist);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_AccountNotExist));
                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry(LogType.Warning, KioskId, $"{_MethodName} [Step 7]: API Response Invalid or Status != Success", _MethodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, accountListResponse?.StatusDesc?.ToString());
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 8]: HTTP Error {aPIResponse.StatusCode} - {aPIResponse.Message}", _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, aPIResponse?.StatusCode.ToString());
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 9]: Exception - {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 10]: Final Response", _MethodName);
            return response.ToString();
        }


        #endregion

        #region ABL ATM CardList
        public async Task<string> ABLCardList(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "ABLCardList";
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;
            XDocument response = request.GetBasicResponseFromRequest();
            List<Dictionary<string, object>> finalATMCardList = new List<Dictionary<string, object>>();

            try
            {
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 1]: Received request", _MethodName);

                string pcNameRaw = ConfigurationManager.AppSettings[KioskId]?.ToString();
                if (string.IsNullOrEmpty(pcNameRaw)) throw new Exception("PC Name not configured for this Kiosk");

                string[] parts = pcNameRaw.Split('|');
                string ComputerName = parts[0].Trim();
                string BranchCode = parts[1].Trim();
                string CardImageBaseUrl = ConfigurationManager.AppSettings["CardImageBaseUrl"].ToString();

                string productcode = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("productcode")?.Value ?? string.Empty;
                string AccountCurrency = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("AccountCurrency")?.Value ?? string.Empty;
                string transactionType = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("TransactionType")?.Value ?? string.Empty;
                string accountType = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("AccountType")?.Value ?? string.Empty;

                var atmCardList = ABLAtmCardList(productcode, AccountCurrency, transactionType, KioskId);
                if (atmCardList == null || atmCardList.Count == 0)
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "NoCardAllowed");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "NoCardAllowed"));
                    return response.ToString();
                }

                var cardFormats = GetCardFormats(ComputerName, KioskId);
                if (cardFormats == null || cardFormats.Count == 0)
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    return response.ToString();
                }

                foreach (var item in atmCardList)
                {
                    bool isValidCard = cardFormats.Any(cf => cf.name == item.KgsName);
                    if (!isValidCard) continue;

                    bool addCard = false;
                    if (transactionType == "AsanAccount")
                    {
                        if ((accountType == "114202" && (item.IrisCardProductCode == "0081" || item.IrisCardProductCode == "0075")) ||
                            (accountType == "114201" && (item.IrisCardProductCode == "0081" || item.IrisCardProductCode == "0070")))
                        {
                            addCard = true;
                        }
                    }
                    else
                    {
                        addCard = true;
                    }

                    if (addCard)
                    {
                        var cardCharges = await ABLDebitCardCharges(item.t24CardCode, KioskId);
                        finalATMCardList.Add(new Dictionary<string, object>
                        {
                            {"id", item.IrisCardProductCode},
                            {"name", item.name},
                            {"kgsName", item.KgsName},
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
                    }
                }
                if (finalATMCardList.Count == 0)
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "No matching card formats found");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
                else
                {
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "ABLCardList Received");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(
                        new XElement("RespMessage", APIResultCodes.Success),
                        new XElement("CardList", finalATMCardList)
                    );
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 9]: Exception - {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            return response.ToString();
        }

        private static List<ABLCardInfo> ABLAtmCardList(string productcode, string accountCurrency, string transactionType, string kioskId)
        {
            string _MethodName = "ABLAtmCardList";
            List<ABLCardInfo> ablCardList = new List<ABLCardInfo>();

            try
            {
                Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 1]: Starting import of Excel data.", _MethodName);

                DataTable cardDataTable = ImportExcel(kioskId);
                Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 2]: Excel data imported successfully.", _MethodName);

                bool matchedPreviously = false;
                string lastAccountCategory = "";

                foreach (DataRow row in cardDataTable.Rows)
                {
                    string accountCategory = row["T24 Account Category Code"]?.ToString().Trim();
                    string currency = row["Currency"]?.ToString().Trim();

                    if (!string.IsNullOrEmpty(accountCategory))
                        lastAccountCategory = accountCategory;
                    else
                        accountCategory = lastAccountCategory;

                    string[] categoryCodes = accountCategory.Split(',')
                                                            .Select(c => c.Trim())
                                                            .Where(c => !string.IsNullOrEmpty(c))
                                                            .ToArray();

                    bool isMatch = categoryCodes.Contains(productcode) || matchedPreviously;

                    if (transactionType == "AsanAccount" && currency == "PKR")
                    {
                        ablCardList.Add(MapToABLCardInfo(row, productcode));
                    }
                    else if (isMatch && accountCurrency != "PKR" && currency == accountCurrency)
                    {
                        ablCardList.Add(MapToABLCardInfo(row, productcode));
                        matchedPreviously = true;
                    }
                    else if (categoryCodes.Contains(productcode) && currency == "PKR")
                    {
                        ablCardList.Add(MapToABLCardInfo(row, productcode));
                    }
                }

                Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 3]: Completed processing. Total Cards Found: {ablCardList.Count}", _MethodName);
                return ablCardList;
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"{_MethodName} [Step 4]: Exception occurred - {ex}", _MethodName);
                return null;
            }
        }



        #endregion

        #region Debit Charges
        public async Task<CardCharges> ABLDebitCardCharges(string productCode, string kioskId)
        {
            const string _MethodName = "ABLDebitCardCharges";
            var apiService = new APIHelper();
            var cardCharges = new CardCharges();

            try
            {
                Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 1]: Input - ProductCode={productCode}", _MethodName);

                string transactionId = GenerateTransactionId();
                string formattedDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 2]: Generated TransactionId={transactionId}, Date={formattedDate}", _MethodName);

                string url = T24Url + ConfigurationManager.AppSettings["DebitCardCharges"];
                Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 3]: API URL={url}", _MethodName);

                string cleanedProductCode = productCode.TrimStart('0');
                Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 4]: Trimmed ProductCode={cleanedProductCode}", _MethodName);

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
                            TransReferenceNo = transactionId,
                            IDNumber = cleanedProductCode
                        }
                    }
                };

                Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 5]: Sending request to API.", _MethodName);
                APIResponse apiResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, kioskId, "");

                if (apiResponse.StatusCode == HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 6]: API response received.", _MethodName);

                    var idContentList = ExtractAndLogValues(apiResponse.ResponseContent);

                    foreach (var (id, content) in idContentList)
                    {
                        if (id == "ISSUANCE.AMOUNT") cardCharges.issuanceamount = content;
                        else if (id == "REPLACEMENT.AMOUNT") cardCharges.replacementamount = content;
                    }

                    Logs.WriteLogEntry(LogType.Info, kioskId, $"{_MethodName} [Step 7]: Parsed Charges - Issuance: {cardCharges.issuanceamount}, Replacement: {cardCharges.replacementamount}", _MethodName);
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, kioskId, $"{_MethodName} [Step 8]: API error - {apiResponse.Message}", _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"{_MethodName} [Step 9]: Exception - {ex}", _MethodName);
            }

            return cardCharges;
        }



        #endregion

        #region PrinterStatus
        public async Task<string> GetPrinterStatus(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "GetPrinterStatus";
            XDocument response = request.GetBasicResponseFromRequest();
            string kioskID = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element("KioskIdentity").Value;

            SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();

            try
            {
                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 1]: Received request for printer status", _MethodName);

                string pcConfig = ConfigurationManager.AppSettings[kioskID];
                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 2]: Retrieved PC Config = {pcConfig}", _MethodName);

                string[] parts = pcConfig.Split('|');
                string computerName = parts[0].Trim();
                string branchCode = parts[1].Trim();

                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 3]: Computer = {computerName}, Branch = {branchCode}", _MethodName);

                string cardName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardName")?.Value ?? string.Empty;
                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 4]: Card Name = {cardName}", _MethodName);

                var getPrinterStatus = deviceOperations.GetPrinterStatus(computerName, cardName);
                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 5]: Printer status code = {getPrinterStatus.code}", _MethodName);

                if (getPrinterStatus.code == 0)
                {
                    string jsonStatus = getPrinterStatus.data.ToString();
                    Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 6]: Raw JSON = {jsonStatus}", _MethodName);

                    var printerStatus = JsonConvert.DeserializeObject<PrinterStatus>(jsonStatus);
                    Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 7]: Deserialized Printer Status = {printerStatus?.status}", _MethodName);

                    if (printerStatus?.status?.ToLower() == "ready")
                    {
                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                        Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 8]: Printer is Ready", _MethodName);
                    }
                    else
                    {
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, $"Printer Not Ready: {printerStatus.status}");
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.PrinterNotConnected));
                        Logs.WriteLogEntry(LogType.Warning, kioskID, $"{_MethodName} [Step 9]: Printer Not Ready - {printerStatus.status}", _MethodName);
                    }
                }
                else
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Printer Not Available");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.PrinterNotAvailable));
                    Logs.WriteLogEntry(LogType.Warning, kioskID, $"{_MethodName} [Step 10]: Printer status failed {getPrinterStatus?.code}", _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskID, $"{_MethodName} [Step 11]: Exception - {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));

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
                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 1]: Received request for hopper status", _MethodName);

                string pcConfig = ConfigurationManager.AppSettings[kioskID];
                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 2]: Retrieved PC Config = {pcConfig}", _MethodName);

                string[] parts = pcConfig.Split('|');
                string computerName = parts[0].Trim();
                string branchCode = parts[1].Trim();

                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 3]: Computer = {computerName}, Branch = {branchCode}", _MethodName);

                string cardName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardName")?.Value ?? string.Empty;
                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 4]: Card Name = {cardName}", _MethodName);

                var getHopperStatus = deviceOperations.IsHopperAvailableForPrinting(computerName, cardName);
                Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 5]: Hopper Status Code = {getHopperStatus.code}", _MethodName);

                if (getHopperStatus.code == 0)
                {
                    string jsonStatus = getHopperStatus.data.ToString();
                    Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 6]: Raw JSON = {jsonStatus}", _MethodName);

                    HopperStatus hopperStatus = JsonConvert.DeserializeObject<HopperStatus>(jsonStatus);
                    Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 7]: Deserialized Hopper Status - ProductAvailable = {hopperStatus?.productAvailable}", _MethodName);

                    if (hopperStatus?.productAvailable == true)
                    {
                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body)
                            .Add(new XElement("RespMessage", APIResultCodes.Success));

                        Logs.WriteLogEntry(LogType.Info, kioskID, $"{_MethodName} [Step 8]: Hopper is Available", _MethodName);
                    }
                    else
                    {
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Hopper is Empty");
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "HopperEmpty"));
                        Logs.WriteLogEntry(LogType.Warning, kioskID, $"{_MethodName} [Step 9]: Hopper is not available", _MethodName);
                    }
                }
                else
                {
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "Hopper Not Available");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "CardNotAvailable"));
                    Logs.WriteLogEntry(LogType.Warning, kioskID, $"{_MethodName} [Step 10]: Hopper status failed", _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskID, $"{_MethodName} [Step 11]: Exception - {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            return response.ToString();
        }


        #endregion

        #region Check Account Balance
        public async Task<string> CheckAccountBalance(XDocument request, string RefrenceNumber)
        {
            string _MethodName = "CheckAccountBalance";
            XDocument response = request.GetBasicResponseFromRequest();
            string KioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 1]: Received request", _MethodName);

                string accountBalanceStr = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountBalance")?.Value ?? string.Empty;
                string issuanceAmountStr = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("IssuanceAmount")?.Value ?? string.Empty;
                string replacementAmountStr = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("ReplacementAmount")?.Value ?? string.Empty;
                string cardGenerationType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardGenerationType")?.Value ?? string.Empty;

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 2]: Extracted values - AccountBalance: {accountBalanceStr}, IssuanceAmount: {issuanceAmountStr}, ReplacementAmount: {replacementAmountStr}, CardGenerationType: {cardGenerationType}", _MethodName);

                if (!double.TryParse(accountBalanceStr, out double accountBalance) ||
                    !double.TryParse(issuanceAmountStr, out double issuanceAmount) ||
                    !double.TryParse(replacementAmountStr, out double replacementAmount))
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Invalid numeric values in request");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));

                    Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 3]: Invalid numeric inputs", _MethodName);
                    return response.ToString();
                }

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 4]: Parsed values - AccountBalance: {accountBalance}, IssuanceAmount: {issuanceAmount}, ReplacementAmount: {replacementAmount}", _MethodName);

                bool isBalanceSufficient = false;

                if (cardGenerationType == "Fresh" || cardGenerationType == "Upgrade")
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 5]: Checking balance for issuance", _MethodName);
                    if (accountBalance >= issuanceAmount)
                    {
                        isBalanceSufficient = true;
                        Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 6]: Sufficient balance for issuance", _MethodName);
                    }
                }
                else if (cardGenerationType == "Replace")
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 7]: Checking balance for replacement", _MethodName);
                    if (accountBalance >= replacementAmount)
                    {
                        isBalanceSufficient = true;
                        Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 8]: Sufficient balance for replacement", _MethodName);
                    }
                }

                if (isBalanceSufficient)
                {
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body)
                        .Add(new XElement("RespMessage", APIResultCodes.Success));

                    Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Step 9]: Balance check passed", _MethodName);
                }
                else
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Insufficient balance");
                    var body = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    body.Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                    body.Add(new XElement("Message", "Insufficient balance for requested transaction"));

                    Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 10]: Balance check failed", _MethodName);
                }
            }
            catch (Exception ex)
            {
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Exception occurred");
                response.Element(TransactionTags.Response).Element(TransactionTags.Body)
                    .Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));

                Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 11]: Exception - {ex}", _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, _MethodName + " Step 1: Validating Input Data: " + request.ToString(), _MethodName);

                string CompanyCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CompanyCode")?.Value ?? "";
                string AccountNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("AccountNumber")?.Value ?? "";
                string ProdCode = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("ProdCode")?.Value ?? "";
                string DpsScheme = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("DpsScheme")?.Value ?? "";
                string CardGenerationType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("CardGenerationType")?.Value ?? "";
                string UpdateType = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("UpdateType")?.Value ?? "";

                string AtmReqType = "";
                if (CardGenerationType == "Fresh" || (CardGenerationType == "Upgrade" && UpdateType == "0"))
                    AtmReqType = "1";
                else if (CardGenerationType == "Replace")
                    AtmReqType = "2";
                else if (CardGenerationType == "Upgrade" && UpdateType == "1")
                    AtmReqType = "5";

                string TransactionId = GenerateTransactionId();
                string formattedDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string url = T24Url + ConfigurationManager.AppSettings["ABLDebitCardIssuance"];

                Logs.WriteLogEntry(LogType.Info, KioskId, _MethodName + " URL: " + url, _MethodName);

                bool flag = await AtmMarkYesForExistingCustomer(AccountNumber, CompanyCode, formattedDate, KioskId);

                if (!flag)
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "ATM Marking Failed");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    return response.ToString();
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "Request Payload: " + JsonConvert.SerializeObject(requestPayload), _MethodName);

                APIResponse apiResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, KioskId, "");

                if (apiResponse.StatusCode != HttpStatusCode.OK)
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "T24 call failed");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    return response.ToString();
                }

                dynamic responseData = JsonConvert.DeserializeObject(apiResponse.ResponseContent);
                dynamic debitCardResponse = responseData != null ? responseData.ABLDebitCardIssuanceRsp : null;

                string hostCode = Convert.ToString(debitCardResponse.HostData.HostCode);
                string hostDesc = Convert.ToString(debitCardResponse.HostData.HostDesc);

                Logs.WriteLogEntry(LogType.Info, KioskId, "HostCode: " + hostCode + " Desc: " + hostDesc, _MethodName);

                if (hostCode == "00")
                {
                    string MotherName = "", FatherName = "", CustomerType = "", AccountType = "", CurrencyCode = "", BranchCode = "";
                    string DefaultAccount = "", AccountStatus = "", BankIMD = "", Email = "", Nationality = "";

                    foreach (var item in debitCardResponse.HostData.field)
                    {
                        string name = Convert.ToString(item.name);
                        string content = Convert.ToString(item.content);

                        if (name == "MOTHER.NAME") MotherName = content;
                        else if (name == "HUSBAND.NAME") FatherName = content;
                        else if (name == "CUSTOMER.NATURE") CustomerType = content;
                        else if (name == "ACCOUNT.NATURE") AccountType = content;
                        else if (name == "CURR.NO") CurrencyCode = content;
                        else if (name == "CO.CODE") BranchCode = content;
                        else if (name == "DEFAULT.ACCOUNT") DefaultAccount = content;
                        else if (name == "STATUS") AccountStatus = content;
                        else if (name == "BANK.IMD") BankIMD = content;
                        else if (name == "CUST.EMAIL") Email = content;
                        else if (name == "NATIONALITY") Nationality = content;
                    }

                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(
                        new XElement("RespMessage", ApiResponseConstants.SuccessStatus),
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
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Host returned error");
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(new XElement("MessageHead", ""));
                    bodyElement.Add(new XElement("Message",
                        errorMessage == "Customer do not meet Basic Eligibility Criteria, Please select Other Criteria."
                        ? ApiResponseConstants.DoNotMeetCriteria
                        : errorMessage));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} : Exception - {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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
                string url = T24Url + ConfigurationManager.AppSettings["ABLAtmFlagUpdate"];

                Logs.WriteLogEntry(LogType.Info, kioskId, methodName + " [URL]: " + url, methodName);

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
                            TransactionId = accountNumber
                        }
                    }
                };

                Logs.WriteLogEntry(LogType.Info, kioskId, "API Request: " + JsonConvert.SerializeObject(requestPayload), methodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestPayload, kioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, kioskId, "API Call Successful! " + apiResponse.Message, methodName);
                    flag = true;
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, kioskId, "API Call Failed. Status Code: " + apiResponse.StatusCode, methodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, "Exception in " + methodName + ": " + ex, methodName);
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

                Logs.WriteLogEntry(LogType.Info, KioskId, $"Request URL: {url}", _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, $"Request XML: {request}", _MethodName);

                wsABLCARDSTATUSCHANGE webService = new wsABLCARDSTATUSCHANGE { Url = url };
                var result = webService.CardListing(CnicNumber);
                string innerXml = XMLHelper.ExtractInnerXml(result);
                string cleanedXml = XMLHelper.FixNestedCardInfo(innerXml);

                Logs.WriteLogEntry(LogType.Info, KioskId, $"Cleaned XML: {cleanedXml}", _MethodName);

                var responseObject = XMLHelper.DeserializeXml<Root>(cleanedXml);
                var bodyElement = response.Element(TransactionTags.Response)?.Element(TransactionTags.Body);
                string CardGenerationType = "", CardNumber = "", CardExpiryDate = "", AccountId = "", CardName = "", ProductDescription = "", CardStatus = "";
                string UpdateType = "0";
                bool CardFoundForReplace = false, CardFoundButFreshCard = false, Flag = false;

                if (responseObject?.Output?.Cards != null && responseObject.WebMethodResponse?.ResponseDescription == "Approved")
                {
                    var allCards = responseObject.Output.Cards;
                    var matchedCards = allCards.Where(c => c.CARDSTATUS != "02" && c.PRODUCTCODE != "0098").ToList();

                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Total non-blocked cards found: {matchedCards.Count}", _MethodName);

                    if (matchedCards.Any())
                    {
                        Card FreshCardList = await FreshCardListing(CnicNumber, finalAccountNumber, KioskId);
                        //   var freshCards = matchedCards.Where(c => c.ACCOUNTID == finalAccountNumber && c.CARDSTATUS == "03").ToList();
                        if (FreshCardList != null)
                        {
                            CardFoundButFreshCard = true;
                            Logs.WriteLogEntry(LogType.Info, KioskId, $"Fresh card found for ProductCode: {FreshCardList.PRODUCTCODE} and Account Number : {FreshCardList.ACCOUNTID}  ", _MethodName);
                        }
                        else
                        {
                            if (matchedCards.Any(c => c.ACCOUNTID == finalAccountNumber)) UpdateType = "1";
                            var relevantCards = matchedCards.Where(c => c.PRODUCTCODE == ProductCode && c.ACCOUNTID == finalAccountNumber).ToList();
                            if (relevantCards.Any())
                            {
                                foreach (var card in relevantCards)
                                {
                                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Card check - Status: {card.CARDSTATUS}, ProductCode: {card.PRODUCTCODE}", _MethodName);
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

                                        Logs.WriteLogEntry(LogType.Info, KioskId, $"Replace card found: {CardNumber}", _MethodName);
                                        break;
                                    }
                                }
                                if (!CardFoundForReplace)
                                {
                                    CardGenerationType = "Fresh";
                                    Logs.WriteLogEntry(LogType.Info, KioskId, $"No active card found, marked as Fresh", _MethodName);
                                }
                            }
                            else
                            {
                                CardGenerationType = "Upgrade";
                                Logs.WriteLogEntry(LogType.Info, KioskId, $"No matching product code card found, marked as Upgrade", _MethodName);
                            }
                        }
                    }
                    else
                    {
                        if (allCards.Any(c => c.ACCOUNTID == finalAccountNumber)) UpdateType = "1";

                        var blockCards = allCards
                        .Where(c => c.CARDSTATUS == "02" && c.PRODUCTCODE != "0098").ToList();

                        Logs.WriteLogEntry(LogType.Info, KioskId, $"Total blocked cards found : {blockCards.Count}", _MethodName);

                        if (blockCards.Any())
                        {
                            var maxExpiry = blockCards.Max(c => c.CARDEXPIRYDATE);
                            Logs.WriteLogEntry(LogType.Info, KioskId, $"Max expiry date among blocked cards: {maxExpiry}", _MethodName);

                            var expiryCard = blockCards.FirstOrDefault(c => c.CARDEXPIRYDATE == maxExpiry);
                            if (expiryCard != null)
                            {
                                Logs.WriteLogEntry(LogType.Info, KioskId, $"Blocked card found with max expiry: {expiryCard.CARDNUMBER}", _MethodName);

                                CardNumber = expiryCard.CARDNUMBER;
                                CardExpiryDate = expiryCard.CARDEXPIRYDATE;
                                AccountId = expiryCard.ACCOUNTID;
                                CardName = expiryCard.CARDNAME;
                                ProductDescription = expiryCard.PRODUCTDESCRIPTION;
                                CardGenerationType = expiryCard.PRODUCTCODE == ProductCode ? "Replace" : "Upgrade";

                            }
                            else
                            {
                                Logs.WriteLogEntry(LogType.Warning, KioskId, $"No card found with the max expiry date", _MethodName);
                            }
                        }
                        else
                        {
                            Logs.WriteLogEntry(LogType.Info, KioskId, $"No blocked cards found", _MethodName);
                        }



                    }
                    if (CardFoundButFreshCard && !CardFoundForReplace)
                    {
                        Logs.WriteLogEntry(LogType.Info, KioskId, $"Card Found But Not For Replace: Card Number: {CardNumber}, Status: {CardStatus}, AccountNumber: {AccountId}", _MethodName);
                        Flag = true;
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "FreshCardNotAllowed");
                        bodyElement.Add(
                            new XElement("MessageHead", "Card Replace Failed !"),
                            new XElement("Message", ApiResponseConstants.FreshCardNotAllowed));
                    }
                    else if (!CardFoundForReplace && !CardFoundButFreshCard)
                    {
                        CardGenerationType = "Upgrade";
                        Logs.WriteLogEntry(LogType.Info, KioskId, $"This is an {CardGenerationType} Card: {ProductCode}", _MethodName);
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

                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "IRIS CardList Response Received");
                    }
                }
                else if (responseObject?.WebMethodResponse?.ResponseDescription == ApiResponseConstants.InvalidCNIC)
                {
                    CardGenerationType = "Fresh";
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Invalid CNIC — defaulting to Fresh card", _MethodName);

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

                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "IRIS CardList Response Received");
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "IRIS card list fetch failed", _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "IRIS card list fetch failed");
                    bodyElement.Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} : Exception - {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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
                Logs.WriteLogEntry(LogType.Info, KioskId, "KIOSK ID: " + kioskID, _MethodName);

                string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, "PC NAME: " + PcName, _MethodName);

                string[] parts = PcName.Split('|');

                string ComputerName = parts[0].Trim();
                string BranchCode = parts[1].Trim();

                Console.WriteLine($"Computer Name: {ComputerName}");
                Console.WriteLine($"Branch Code: {BranchCode}");


                Logs.WriteLogEntry(LogType.Info, KioskId, "IRISCardIssuance Step 1: " + request.ToString(), _MethodName);
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
                string Scheme = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("Scheme")?.Value ?? string.Empty;

                CNIC = CNIC.Replace("-", "");
                string FinalAccountNumber = BranceCode.Substring(BranceCode.Length - 4) + AccountNumber;
                string TrackingId = GenerateTransactionId();
                int IsoCode = GetIsoCode(CurrencyCode) ?? 0;
                string finaCurrenctCode = Convert.ToString(IsoCode).ToString();
                string BankIMD = GetBankIMD(ProductCode);
                string ActivationDate = DateTime.Now.ToString("yyyyMMdd");
                string AccountType = GetAccountType(int.Parse(AccountCategory));

                Logs.WriteLogEntry(LogType.Info, KioskId, $"IRIS Request Details: AccountType={AccountType}, ActivationDate={ActivationDate}, IMD={BankIMD}", _MethodName);

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

                string irisUrl = IrisUrl + ConfigurationManager.AppSettings["IRISCardIssuance"];
                Logs.WriteLogEntry(LogType.Info, KioskId, $"Calling IRIS URL: {irisUrl}", _MethodName);
                InstantCard webService = new InstantCard { Url = irisUrl };

                string requestLog = $@"
                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
                        <soapenv:Header/>
                        <soapenv:Body>
                        <tem:ImportCustomer>
                        <tem:ActionCode>{ActionCode}</tem:ActionCode>
                        <tem:CNIC>{CNIC}</tem:CNIC>
                        <tem:TrackingID>{TrackingId}</tem:TrackingID>
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
                        <tem:AccountNo>{FinalAccountNumber}</tem:AccountNo>
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

                Logs.WriteLogEntry(LogType.Info, KioskId, requestLog, _MethodName);

                string result = webService.ImportCustomer(
                     ActionCode: ActionCode,
                     CNIC: CNIC,
                     TrackingID: TrackingId,
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
                     AccountNo: FinalAccountNumber,
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance API Response responseCode : " + responseCode, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance API Response trackingID : " + trackingID, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance API Response responseDescription : " + responseDescription, _MethodName);

                if (responseCode == "00" && responseDescription == "Success")
                {
                    var cardInfo = DecryptEmbossingFile(BranchCode, ProductCode, KioskId, Scheme);
                    if (cardInfo.CardHolderName != null)
                    {
                        string description;
                        var personalizationResponse = CardPersonalization(cardInfo, ComputerName, SelectedCardName, out description, KioskId);
                        if (personalizationResponse.data.ToString() != "" && personalizationResponse.data != null)
                        {
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(
                                new XElement("RespMessage", APIResultCodes.Success),
                                new XElement("RequestId", personalizationResponse.data)
                            );

                            SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "IRIS Request Successfully Sent");
                        }
                        else
                        {
                            SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, description);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry(LogType.Error, KioskId, "CardInfo is null", _MethodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Embossing file decryption failed");
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, KioskId, $"IRIS Card Issuance Failed: Code={responseCode}, Description={responseDescription}", _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "IRIS Issuance Failed");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (ArgumentNullException argEx)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"ArgumentNullException in {_MethodName}: {argEx}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Missing required data");
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }
            catch (InvalidOperationException invOpEx)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"InvalidOperationException in {_MethodName}: {invOpEx}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Invalid operation");
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"Unhandled Exception in {_MethodName}: {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Internal Server Error");
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            return response.ToString();

        }

        #endregion

        #endregion

        #region Send Sms
        public async Task<string> SendOTP(XDocument request, string RefrenceNumber)
        {
            const string _MethodName = "SendOTP";
            XDocument response = request.GetBasicResponseFromRequest();
            smpp_ws_sendsms service = new smpp_ws_sendsms();

            string kioskId = request.Element(TransactionTags.Request)?.Element(TransactionTags.Header)?.Element(TransactionTags.KioskIdentity)?.Value ?? "Unknown";

            try
            {
                string mobileNumber = request.Element(TransactionTags.Request)?.Element(TransactionTags.Body)?.Element("mobileNumber")?.Value ?? string.Empty;

                Logs.WriteLogEntry(LogType.Info, kioskId, $"Final Request: {request}", _MethodName);

                Random random = new Random();
                //int otp = random.Next(100000, 999999);
                int otp = 111111;
                string message = $"Your OTP for verification is: {otp}. Please enter this code to proceed.";

                string url = ConfigurationManager.AppSettings["SendOtp"]?.ToString();
                Logs.WriteLogEntry(LogType.Info, kioskId, $"Send URL: {url}", _MethodName);
                Logs.WriteLogEntry(LogType.Info, kioskId, $"Attempting to send OTP SMS to {mobileNumber}", _MethodName);

                var serviceResponse = service.QueueSMS("SSK", mobileNumber, message, "3");

                if (!string.IsNullOrEmpty(serviceResponse))
                {
                    Logs.WriteLogEntry(LogType.Info, kioskId, $"SendOTP Response: {serviceResponse}", _MethodName);

                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "OTP Sent Successfully");
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
                    bodyElement.Add(new XElement("OTP", otp));
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, kioskId, "Failed to send OTP. Null response from SMS gateway.", _MethodName);

                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.OtpSendFailed);
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                    bodyElement.Add(new XElement("Message", ApiResponseConstants.OtpSendFailed));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"{_MethodName} : Exception - {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            return response.ToString();
        }

        #endregion

        #region Get Transiction Id
        private static string GenerateTransactionId()
        {
            const string methodName = "GenerateTransactionId";
            string kioskId = "System"; 

            try
            {
                DateTime now = DateTime.Now;
                string dateTimeNow = now.ToString("yyMMddHHmm");
                string randomDigits = new Random().Next(10, 99).ToString();

                string transactionId = $"{dateTimeNow}{randomDigits}";

                Logs.WriteLogEntry(LogType.Info, kioskId, $"Generated Transaction ID: {transactionId}", methodName);

                return transactionId;
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"Exception in GenerateTransactionId: {ex}", methodName);
                throw;
            }
        }
        #endregion

        #region Get Cards Formats
        private List<CardFormats> GetCardFormats(string computerName, string kioskId)
        {
            const string methodName = "GetCardFormats";
            List<CardFormats> cardFormatList = new List<CardFormats>();

            try
            {
                if (string.IsNullOrEmpty(computerName))
                {
                    Logs.WriteLogEntry(LogType.Warning, kioskId, "Computer name is null or empty. Cannot proceed with GetCardFormats.", methodName);
                    return cardFormatList;
                }

                Logs.WriteLogEntry(LogType.Info, kioskId, $"Initiating card format retrieval for computer: {computerName}", methodName);

                SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();
                HardwareResponse getCardFormats = deviceOperations.GetCardFormats(computerName);

                Logs.WriteLogEntry(LogType.Info, kioskId, $"GetCardFormats API Response - Code: {getCardFormats.code}, Description: {getCardFormats.description}, Data: {getCardFormats.data}", methodName);

                if (getCardFormats.code == 0 && getCardFormats.data != null)
                {
                    string jsonCardFormat = getCardFormats.data.ToString();
                    Logs.WriteLogEntry(LogType.Info, kioskId, $"Card Format JSON: {jsonCardFormat}", methodName);

                    cardFormatList = JsonConvert.DeserializeObject<List<CardFormats>>(jsonCardFormat);
                    Logs.WriteLogEntry(LogType.Info, kioskId, $"Card Formats deserialized successfully. Count: {cardFormatList.Count}", methodName);
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, kioskId, "Card format data not available", methodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"Exception in GetCardFormats: {ex}", methodName);
            }

            return cardFormatList;
        }


        #endregion

        #region CardPersonalization
        private HardwareResponse CardPersonalization(CardInfo cardInfo, string computerName, string cardName, out string description, string kioskId)
        {
            const string methodName = "CardPersonalization";
            description = string.Empty;
            HardwareResponse hardwareResponse = new HardwareResponse();

            try
            {
                if (string.IsNullOrEmpty(computerName))
                {
                    Logs.WriteLogEntry(LogType.Error, kioskId, "Computer name is null or empty. Cannot personalize card.", methodName);
                    return hardwareResponse;
                }

                Logs.WriteLogEntry(LogType.Info, kioskId, "Preparing data for card personalization.", methodName);

                List<DataItem> dataItems = new List<DataItem>
                {
                    new DataItem { name = "@PAN@", value = cardInfo.PAN?.Replace(" ", "") ?? string.Empty },
                    new DataItem { name = "@Expiry@", value = cardInfo.Expiry ?? string.Empty },
                    new DataItem { name = "@CardHolderName@", value = cardInfo.CardHolderName ?? string.Empty },
                    new DataItem { name = "@CVV2@", value = cardInfo.CVV2 ?? string.Empty },
                    new DataItem { name = "@iCVV@", value = cardInfo.ICVV ?? string.Empty },
                    new DataItem { name = "@Track1@", value = cardInfo.Track1 ?? string.Empty },
                    new DataItem { name = "@Track2@", value = cardInfo.Track2 ?? string.Empty },
                    new DataItem { name = "@CVV@", value = cardInfo.CVV1 ?? string.Empty },
                    new DataItem { name = "@MemberSince@", value = cardInfo.MemberSince ?? string.Empty }
                };

                CardPersonalizationRequest request = new CardPersonalizationRequest { dataItems = dataItems };
                string jsonRequest = JsonConvert.SerializeObject(request);
                Logs.WriteLogEntry(LogType.Info, kioskId, $"CardPersonalization request JSON: {jsonRequest}", methodName);

                SigmaDS4.DeviceOperations deviceOps = new SigmaDS4.DeviceOperations();
                hardwareResponse = deviceOps.StartCardPersonalization(computerName, cardName, request);

                Logs.WriteLogEntry(LogType.Info,kioskId,$"CardPersonalization Response: Code={hardwareResponse.code}, Description={hardwareResponse.description}, Data={hardwareResponse.data}",methodName
                );

                if (hardwareResponse.code == 0 && hardwareResponse.data != null)
                {
                    Logs.WriteLogEntry(LogType.Info, kioskId, "Card personalization successful. Data found.", methodName);
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, kioskId, "Card personalization failed or returned null data.", methodName);
                    description = hardwareResponse.description ?? "No description returned.";
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"Exception in CardPersonalization: {ex}", methodName);
                description = "Exception during card personalization.";
            }

            return hardwareResponse;
        }

        #endregion

        #region Card Status
        public async Task<string> GetCardStatus(XDocument request, string referenceNumber)
        {
            const string methodName = "GetCardStatus";
            XDocument response = request.GetBasicResponseFromRequest();
            SigmaDS4.DeviceOperations deviceOperations = new SigmaDS4.DeviceOperations();

            string kioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                string pcName = ConfigurationManager.AppSettings[kioskId]?.ToString() ?? string.Empty;

                Logs.WriteLogEntry(LogType.Info, kioskId, $"KIOSK ID: {kioskId}", methodName);
                Logs.WriteLogEntry(LogType.Info, kioskId, $"PC NAME: {pcName}", methodName);

                string requestId = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("RequestId")?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(requestId))
                {
                    Logs.WriteLogEntry(LogType.Warning, kioskId, "RequestId is missing in request.", methodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                    return response.ToString();
                }

                Logs.WriteLogEntry(LogType.Info, kioskId, $"Checking card status for RequestId: {requestId}", methodName);

                HardwareResponse hardwareResponse = deviceOperations.GetPersonalizationRequestStatus(requestId, 3, 15);
                var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                Logs.WriteLogEntry(LogType.Info, kioskId, $"Card Status Data: {hardwareResponse.data}", methodName);
                Logs.WriteLogEntry(LogType.Info, kioskId, $"Card Status Code: {hardwareResponse.code}", methodName);
                Logs.WriteLogEntry(LogType.Info, kioskId, $"Card Status Description: {hardwareResponse.description}", methodName);

                string status = hardwareResponse.data?.ToString();

                switch (hardwareResponse.data.ToString())
                {
                    case "Success":

                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "Card Printed");
                        bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
                        break;
                    case "Failed":
                        Logs.WriteLogEntry(LogType.Error, kioskId, "Card status indicates failure.", methodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                        break;
                    case "AtExit":
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "Card Printed");
                        bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success));
                        break;
                    case "Processing":
                        Logs.WriteLogEntry(LogType.Warning, kioskId, "Card is still processing.", methodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, "Processing");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"{methodName} [Step 9]: Exception - {ex}", methodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            return response.ToString();
        }


        #endregion

        #endregion

        #region Account Opening

        #region Send OTP 
        public async Task<string> SendOtpAsanAccount(XDocument request, string referenceNumber)
        {
            const string methodName = "SendOtpAsanAccount";
            APIHelper apiService = new APIHelper();
            XDocument response = request.GetBasicResponseFromRequest();

            string kioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                string mobileNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("mobileNumber")?.Value ?? string.Empty;
                string cnicNumber = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("cnicNumber")?.Value ?? string.Empty;
                cnicNumber = cnicNumber.Replace("-", "");

                Logs.WriteLogEntry(LogType.Info, kioskId, $"Request XML: {request}", methodName);
                string url = MyPdaUrl + ConfigurationManager.AppSettings["SendOtpPda"];
                Logs.WriteLogEntry(LogType.Info, kioskId, $"[{methodName}] URL: {url}", methodName);

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

                Logs.WriteLogEntry(LogType.Info, kioskId, $"API Request Payload: {JsonConvert.SerializeObject(requestData)}", methodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, kioskId, "");

                Logs.WriteLogEntry(LogType.Info, kioskId, $"API Response: {JsonConvert.SerializeObject(apiResponse)}", methodName);

                if (apiResponse.StatusCode == HttpStatusCode.OK)
                {
                    OTPResponse otpResponse = JsonConvert.DeserializeObject<OTPResponse>(apiResponse.ResponseContent);
                    Logs.WriteLogEntry(LogType.Info, kioskId, $"OTP Send Success: {apiResponse.Message}", methodName);

                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, "Success");
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body)
                            .Add(new XElement("RespMessage", APIResultCodes.Success));
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, kioskId, $"Failed OTP Send. StatusCode: {apiResponse.StatusCode}", methodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"{methodName} [Step 9]: Exception - {ex}", methodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            return response.ToString();
        }

        #endregion

        #region Delete Application
        public async Task<string> DeleteApplication(XDocument request, string referenceNumber)
        {
            const string methodName = "DeleteApplication";
            APIHelper apiService = new APIHelper();
            XDocument response = request.GetBasicResponseFromRequest();

            string kioskId = request.Element(TransactionTags.Request).Element(TransactionTags.Header).Element(TransactionTags.KioskIdentity).Value;

            try
            {
                string nadraResponse = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("nadraResponse")?.Value ?? string.Empty;

                string url = MyPdaUrl + ConfigurationManager.AppSettings["UpdateApplication"];
                Logs.WriteLogEntry(LogType.Info, kioskId, $"[{methodName}] URL: {url}", methodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];

                JObject jsonRequest = null;

                foreach (var consumer in consumerList)
                {
                    jsonRequest = new JObject
                    {
                        ["data"] = new JObject
                        {
                            ["customerProfileId"] = consumer["rdaCustomerProfileId"],
                            ["customerAccountInfoId"] = consumer["accountInformation"]?["rdaCustomerAccInfoId"]
                        }
                    };
                }

                Logs.WriteLogEntry(LogType.Info, kioskId, $"API Request: {jsonRequest}", methodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, jsonRequest, kioskId, "");

                if (apiResponse.StatusCode == HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, kioskId, $"API Call Successful! {apiResponse.Message}", methodName);

                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body)
                            .Add(new XElement("RespMessage", APIResultCodes.Success));
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, kioskId, $"API Call Failed. StatusCode: {apiResponse.StatusCode}", methodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_AccountNotExist);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, $"{methodName} : Exception - {ex}", methodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "Request: " + request.ToString(), _MethodName);

                string url = MyPdaUrl + ConfigurationManager.AppSettings["VerifyOtp"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

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

                Logs.WriteLogEntry(LogType.Info, KioskId, $"API Request bodyElement: {requestData}", _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                Logs.WriteLogEntry(LogType.Info, KioskId, $"API Response bodyElement: {apiResponse.ResponseContent}", _MethodName);

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
                                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Account Status ID: {accStatusId}", _MethodName);
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
                            Logs.WriteLogEntry(LogType.Info, KioskId, $"Customer Application Already in Process:", _MethodName);
                            SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.AccountAlreadyInProcess);
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.AccountAlreadyInProcess));

                        }
                        else
                        {
                            string[] CustomerProfileIds = profileIds.ToArray();
                            string[] CustomerAccInfoIds = accInfoIds.ToArray();

                            profileIdsCsv = string.Join(", ", CustomerProfileIds);
                            accInfoIdsCsv = string.Join(", ", CustomerAccInfoIds);

                            Logs.WriteLogEntry(LogType.Info, KioskId, $"CustomerprofileIds: {profileIdsCsv}", _MethodName);
                            Logs.WriteLogEntry(LogType.Info, KioskId, $"CustomerAccInfoIds: {accInfoIdsCsv}", _MethodName);

                            SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
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
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"API Response: {Description} - {errorDetail} - {Status}", _MethodName);

                    if (Description == ApiResponseConstants.PleaseProvideValidOTP)
                    {
                        Logs.WriteLogEntry(LogType.Warning, KioskId, $"API Call OTP Failed: {Status} - {Description} - {errorDetail}", _MethodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, Description);
                        bodyElement.Add(new XElement("RespMessage", "OtpFailed"),
                        new XElement("OTP", "Failed"));
                    }
                    else
                    {
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, Description);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                        bodyElement.Add(new XElement("RespMessage", ApiResponseConstants.PmdFailed));
                        Logs.WriteLogEntry(LogType.Warning, KioskId, $"API Call PMD Failed: {Status} - {Description} - {errorDetail}", _MethodName);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"{_MethodName} [Step 9]: Exception - {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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
                Logs.WriteLogEntry(LogType.Info, kioskId, $"{methodName} [URL]:  {url}", methodName);

                var deleteRequest = new JObject
                {
                    ["data"] = new JObject
                    {
                        ["customerProfileId"] = profileId,
                        ["customerAccountInfoId"] = accountInfoId
                    }
                };

                Logs.WriteLogEntry(LogType.Info, kioskId, "API Request : " + deleteRequest.ToString(), methodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, deleteRequest, kioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, kioskId, "API Call Successful! " + apiResponse.Message, methodName);
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, kioskId, $"API Call Failed. Status Code: {apiResponse.StatusCode}", methodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, kioskId, "Error while deleting application: " + ex, methodName);
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "AsaanDigitalAccountConventional url: " + AsaanDigitalAccountConventional, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "AsaanDigitalAccountIslamic url: " + AsaanDigitalAccountIslamic, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "AsaanDigitalRemittanceAccountConventional url: " + AsaanDigitalRemittanceAccountConventional, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "AsaanDigitalRemittanceAccountIslamic url: " + AsaanDigitalRemittanceAccountIslamic, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "Declaration url: " + Declaration, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "TnCEnglish url: " + TnCEnglish, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "TnCUrdu url: " + TnCUrdu, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "GetCustomerFromNadra Request !: " + request.ToString(), _MethodName);

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

                Logs.WriteLogEntry(LogType.Info, KioskId, "GetCustomerFromNadra Request!: " + request.ToString(), _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "GetCustomerFromNadra formattedDate!: " + formattedDate, _MethodName);

                #region List Of Variants

                // Occupation List
                List<Dictionary<string, object>> OccupationList = await GetOccupationListAsync(KioskId);
                if (OccupationList.Count > 0)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Occupation List Found !: " + OccupationList, _MethodName);
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Occupation List Not Found !: ", _MethodName);
                }

                // Profession List
                List<Dictionary<string, object>> professionList = await GetProfessionListAsync(KioskId);
                if (professionList.Count > 0)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Profession List Found: " + professionList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Profession List Not Found !: ", _MethodName);
                }

                // Town / Tehsil List
                List<Dictionary<string, object>> townTehsilList = await TownTehsilList(KioskId);
                if (townTehsilList.Count > 0)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Town / Tehsil List Found: " + professionList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Town / Tehsil Not Found !: ", _MethodName);
                }

                // Branches List
                List<Dictionary<string, object>> islamicBranchList = await IslamicBranchList(KioskId);
                if (islamicBranchList.Count > 0)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Islamic Branch List Found: " + islamicBranchList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Islamic Branch Not Found !: ", _MethodName);
                }

                // Conventional List
                List<Dictionary<string, object>> conventionalBranchList = await ConventionalBranchList(KioskId);
                if (conventionalBranchList.Count > 0)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Conventional Branch List Found: " + conventionalBranchList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Conventional Branch Not Found !: ", _MethodName);
                }

                // Gender List
                List<Dictionary<string, object>> genderList = await GenderList(KioskId);
                if (genderList.Count > 0)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Gender List Found: " + genderList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Gender Not Found !: ", _MethodName);
                }
                // Branches List
                List<Dictionary<string, object>> accountPurposeList = await AccountPurpose(KioskId);
                if (accountPurposeList.Count > 0)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "AccountPurpose List Found: " + accountPurposeList, "GetCustomerFromNadra");
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "AccountPurpose Not Found !: ", _MethodName);
                }

                #endregion

                string url = MyPdaUrl + ConfigurationManager.AppSettings["GetCustomerFromNadra"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);


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


                Logs.WriteLogEntry(LogType.Info, KioskId, "GetCustomerFromNadra API Request !: " + JsonConvert.SerializeObject(requestData), _MethodName);
                var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var consumerList = jsonResponse["data"]?["consumerList"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                            MotherName = consumer["motherMaidenName"].ToString();
                            if (string.IsNullOrEmpty(MotherName))
                            {
                                MotherName = Decrypt(consumer["motherMaidenNameEncrypted"].ToString());
                            }
                            FatherName = consumer["fatherHusbandName"].ToString();
                            gender = consumer["gender"].ToString();
                            genderId = consumer["genderId"].ToString();
                            Address1 = consumer["addresses"]?[0]?["customerAddress"].ToString();
                            Address2 = consumer["addresses"]?[0]?["customerAddressLine1"].ToString();
                            Path = consumer["attachments"]?[0]?["path"].ToString();
                            AccessToken = consumer["accessToken"].ToString();
                            IsTranslate = consumer["nadraTranslationInUrduInd"].ToString();
                        }

                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
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
                    Logs.WriteLogEntry(LogType.Error, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, apiResponse.ResponseContent);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message",ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Error in Failed to GetCustomerFromNadra!: " + ex, _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "{PersonalInfo} Step 1: Going to send request basic info personal Data", _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "request" + request, _MethodName);

                string CustomerBasicInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerBasicInfo"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [CustomerBasicInfo URL]:  {CustomerBasicInfoUrl}", _MethodName);
                string CustomerAccountInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerAccountInfo"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [CustomerAccountInfo URL]:  {CustomerAccountInfoUrl}", _MethodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} Step 1", _MethodName);

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
                     ["cityOfBirth"] = consumer["cityOfBirth"].ToString(),
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} Step 2", _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "{personal-basic-info} jsonRequest:" + JsonConvert.SerializeObject(jsonRequest), _MethodName);

                APIResponse aPIResponse = await apiService.SendRestTransaction(CustomerBasicInfoUrl, HttpMethods.POST, jsonRequest, accessToken, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "{personal-basic-info} Response was successful. Step 2:", _MethodName);
                    var responseData = JsonConvert.DeserializeObject<dynamic>(aPIResponse.ResponseContent);
                    Logs.WriteLogEntry(LogType.Info, KioskId, "{personal-basic-info} Response Content  Step 3:" + responseData, _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Going to send request of {personal-account-info}   Step 4:", _MethodName);
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

                    Logs.WriteLogEntry(LogType.Info, KioskId, "{personal-account-info}" + jsonRequest2, _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, "CustomerAccountInfoUrl" + CustomerAccountInfoUrl, _MethodName);
                    APIResponse aPIResponse2 = await apiService.SendRestTransaction(CustomerAccountInfoUrl, HttpMethods.POST, jsonRequest2, accessToken, "");
                    if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Logs.WriteLogEntry(LogType.Info, KioskId, "{personal-basic-info} Response was successful. Step 2:", _MethodName);
                        var responseData2 = JsonConvert.DeserializeObject<dynamic>(aPIResponse2.ResponseContent);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "{personal-basic-info} Response Content  Step 3:" + responseData2, _MethodName);

                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    }
                    else
                    {
                        Logs.WriteLogEntry(LogType.Warning, KioskId, aPIResponse?.StatusCode.ToString(), _MethodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {PersonalInfo} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {PersonalInfo} Error Message: " + aPIResponse.Message, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"Exception in {{PersonalInfo}}: {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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
                string mobileNo = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("mobileNo")?.Value ?? string.Empty;


                Logs.WriteLogEntry(LogType.Info, KioskId, "request" + request, _MethodName);
                string url = MyPdaUrl + ConfigurationManager.AppSettings["CurrentAddress"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

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
                                            ["postalCode"] = consumer["addresses"][0]["postalCode"],
                                            ["phone"] = mobileNo,
                                            ["nearestLandMark"] = consumer["addresses"][0]["nearestLandMark"],
                                            ["mobileNo"] = mobileNo,
                                            ["customerTown"] = town,
                                            ["customerAddress"] = address1,
                                            ["customerAddressLine1"] = address2,
                                            ["countryCodeMobile"] = "+92",
                                            ["city"] = city,
                                            ["countryId"] = 157,
                                            ["country"] = "Pakistan",
                                            ["addressTypeForeignInd"] = consumer["addresses"][0]["addressTypeForeignInd"],
                                            ["addressTypeId"] = consumer["addresses"][0]["addressTypeId"],
                                        },
                                        new JObject
                                        {
                                            ["rdaCustomerProfileAddrId"] = consumer["addresses"][1]["rdaCustomerProfileAddrId"],
                                            ["rdaCustomerId"] = consumer["addresses"][1]["rdaCustomerId"],
                                            ["postalCode"] = consumer["addresses"][1]["postalCode"],
                                            ["phone"] = mobileNo,
                                            ["nearestLandMark"] = consumer["addresses"][1]["nearestLandMark"],
                                            ["mobileNo"] = mobileNo,
                                            ["customerTown"] = town,
                                            ["customerAddress"] = consumer["addresses"][1]["customerAddress"],
                                            ["customerAddressLine1"] = consumer["addresses"][1]["customerAddressLine1"],
                                            ["countryCodeMobile"] = "+92",
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
                Logs.WriteLogEntry(LogType.Info, KioskId, "{CurrentAddress} jsonRequest:" + JsonConvert.SerializeObject(jsonRequest), _MethodName);
                APIResponse aPIResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, jsonRequest, accessToken, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {CurrentAddress} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {CurrentAddress} Error Message: " + aPIResponse.Message, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, aPIResponse.Message);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"Exception in {{CurrentAddress}}: {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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


                Logs.WriteLogEntry(LogType.Info, KioskId, "{OccupationalDetail} Step 1: Going to send request basic info personal Data", _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "request" + request, _MethodName);


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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [CustomerBasicInfo URL]:  {CustomerBasicInfoUrl}", _MethodName);

                string SaveKycUrl = MyPdaUrl + ConfigurationManager.AppSettings["SaveKyc"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [SaveKycUrl URL]:  {SaveKycUrl}", _MethodName);

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
                                 ["cityOfBirth"] = consumer["cityOfBirth"].ToString(),
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
                Logs.WriteLogEntry(LogType.Info, KioskId, "Request :" + JsonConvert.SerializeObject(jsonRequest), _MethodName);
                APIResponse aPIResponse = await apiService.SendRestTransaction(CustomerBasicInfoUrl, HttpMethods.POST, jsonRequest, accessToken, "");

                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "{occupational-basic-info} Response was successful. Step 2:", _MethodName);
                    var responseData = JsonConvert.DeserializeObject<dynamic>(aPIResponse.ResponseContent);

                    Logs.WriteLogEntry(LogType.Info, KioskId, "{occupational-basic-info} Response Content  Step 3:" + responseData, _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Going to send request of {kyc-save}   Step 4:", _MethodName);
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
                    Logs.WriteLogEntry(LogType.Info, KioskId, "kyc-save" + JsonConvert.SerializeObject(jsonRequest2), _MethodName);
                    APIResponse aPIResponse2 = await apiService.SendRestTransaction(SaveKycUrl, HttpMethods.POST, jsonRequest2, accessToken, "");
                    if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Logs.WriteLogEntry(LogType.Info, KioskId, "{kyc-save} Response was successful. Step 2:", _MethodName);
                        var responseData2 = JsonConvert.DeserializeObject<dynamic>(aPIResponse2.ResponseContent);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "{kyc-save} Response Content  Step 3:" + responseData2, _MethodName);

                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Header).Element(TransactionTags.ResultCode).Value = TransactionResultString.Success;
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                        var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    }
                    else
                    {
                        Logs.WriteLogEntry(LogType.Warning, KioskId, " {OccupationalDetail} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                        Logs.WriteLogEntry(LogType.Warning, KioskId, " {OccupationalDetail} Error Message: " + aPIResponse.Message, _MethodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, aPIResponse.Message);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {OccupationalDetail} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {OccupationalDetail} Error Message: " + aPIResponse.Message, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, aPIResponse.Message);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"Exception in {{CustomerAccountList}}: {ex.Message}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "request" + request, _MethodName);
                string CustomerAccountInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerAccountInfo"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [CustomerAccountInfo URL]:  {CustomerAccountInfoUrl}", _MethodName);

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


                Logs.WriteLogEntry(LogType.Info, KioskId, "jsonRequest" + jsonRequest, _MethodName);

                APIResponse aPIResponse = await apiService.SendRestTransaction(CustomerAccountInfoUrl, HttpMethods.POST, jsonRequest, accessToken, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success , ApiResponseConstants.SuccessStatus);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                }
                else
                {

                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {BankingReference} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {BankingReference} Error Message: " + aPIResponse.Message, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, aPIResponse.Message);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));

                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"Exception in {{BankingReference}}: {ex.Message}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "request" + request.ToString(), _MethodName);
                int accountstate = 0;
                if (accountStatement == "true")
                {
                    accountstate = 1;
                }
                string CustomerAccountInfoUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerAccountInfo"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [CustomerAccountInfo URL]:  {CustomerAccountInfoUrl}", _MethodName);

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
                Logs.WriteLogEntry(LogType.Info, KioskId, "Updated Date Of Birth" + date, _MethodName);

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
                Logs.WriteLogEntry(LogType.Info, KioskId, "jsonRequest !: " + JsonConvert.SerializeObject(jsonRequest), _MethodName);
                APIResponse aPIResponse = await apiService.SendRestTransaction(CustomerAccountInfoUrl, HttpMethods.POST, jsonRequest, accessToken, "");
                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, " {AccountsDetails} Response: " + aPIResponse.ResponseContent, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    bodyElement.Add(new XElement("AccountCategory", variantInfo.Name));
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {AccountsDetails} Request failed with status code: " + aPIResponse.StatusCode, _MethodName);
                    Logs.WriteLogEntry(LogType.Warning, KioskId, " {AccountsDetails} Error Message: " + aPIResponse.Message, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, aPIResponse.Message);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Error in Failed to AccountsDetails!: " + ex, _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "request" + request, _MethodName);

                string UpdateKycUrl = MyPdaUrl + ConfigurationManager.AppSettings["UpdateKyc"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [UpdateKyc URL]:  {UpdateKycUrl}", _MethodName);

                string AuthorizerKycUrl = MyPdaUrl + ConfigurationManager.AppSettings["AuthorizerKyc"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [AuthorizerKyc URL]:  {AuthorizerKycUrl}", _MethodName);

                string CustomerProfileStatusUrl = MyPdaUrl + ConfigurationManager.AppSettings["CustomerProfileStatus"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [CustomerProfileStatus URL]:  {CustomerProfileStatusUrl}", _MethodName);

                string ScreeningUrl = MyPdaUrl + ConfigurationManager.AppSettings["Screening"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [ScreeningUrl URL]:  {ScreeningUrl}", _MethodName);

                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];


                //<-- Update KYC --->

                Logs.WriteLogEntry(LogType.Info, KioskId, "Going to send request of {update-kyc}:", _MethodName);

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

                Logs.WriteLogEntry(LogType.Info, KioskId, "update-kyc Request" + JsonConvert.SerializeObject(jsonRequest), _MethodName);

                APIResponse aPIResponse = await apiService.SendRestTransaction(UpdateKycUrl, HttpMethods.POST, jsonRequest, accessToken, "");
                JObject updateKycResponse = JObject.Parse(aPIResponse.ResponseContent);
                var updateKycData = updateKycResponse["data"];
                var updateKycMessage = updateKycResponse["message"];
                string msg = updateKycData["msg"]?.ToString();

                Logs.WriteLogEntry(LogType.Info, KioskId, "Response updateKycData :" + aPIResponse.ResponseContent, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "Response updateKycMessage :" + updateKycMessage["status"] + "-" + updateKycMessage["description"], _MethodName);

                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "updateKycData" + updateKycData + "-" + msg, _MethodName);
                    if (msg == ApiResponseConstants.AccountDirectlyPushToT24)
                    {
                        // <---Authorizer KYC--->
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Going to send request of {AuthorizerKyc}:", _MethodName);
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
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Authorizer-Kyc Request" + JsonConvert.SerializeObject(jsonRequest2), _MethodName);
                        APIResponse aPIResponse2 = await apiService.SendRestTransaction(AuthorizerKycUrl, HttpMethods.POST, jsonRequest2, accessToken, "");
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Response :" + aPIResponse2.ResponseContent, _MethodName);
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

                        Logs.WriteLogEntry(LogType.Info, KioskId, "Response DataResponseMessage :" + DataResponseMessage, _MethodName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Response AuthorizerKycData :" + aPIResponse2.ResponseContent, _MethodName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Response AuthorizerKycMessage :" + AuthorizerKycMessage["status"]?.ToString() + "-" + AuthorizerKycMessage["description"]?.ToString(), _MethodName);

                        if (aPIResponse2.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(AccountNumber))
                        {
                            Logs.WriteLogEntry(LogType.Info, KioskId, "Response Account Number Recieved :" + AccountNumber, _MethodName);

                            // <---Screening--->
                            Logs.WriteLogEntry(LogType.Info, KioskId, "Going to send request of {Screening}:", _MethodName);
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
                            Logs.WriteLogEntry(LogType.Info, KioskId, "Screening Request" + JsonConvert.SerializeObject(jsonRequest3), _MethodName);

                            APIResponse aPIRespons3 = await apiService.SendRestTransaction(ScreeningUrl, HttpMethods.POST, jsonRequest3, accessToken, "");
                            JObject ScreeningResponse = JObject.Parse(aPIRespons3.ResponseContent);
                            var ScreeningData = ScreeningResponse["data"];
                            var ScreeningMessage = ScreeningResponse["message"];

                            Logs.WriteLogEntry(LogType.Info, KioskId, "Response ScreeningData :" + aPIRespons3.ResponseContent, _MethodName);
                            Logs.WriteLogEntry(LogType.Info, KioskId, "Response ScreeningMessage :" + ScreeningMessage["status"] + "-" + ScreeningMessage["description"], _MethodName);

                            if (aPIRespons3.StatusCode == HttpStatusCode.OK)
                            {
                                SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
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
                                Logs.WriteLogEntry(LogType.Warning, KioskId, "Screening " + aPIRespons3.StatusCode.ToString(), _MethodName);
                                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, aPIResponse.Message);
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                            }
                        }
                        else
                        {
                            if (AuthorizerKycMessage["description"].ToString() == "Your Personal Information is not as per Nadra record please correct your information and try again")
                            {
                                Logs.WriteLogEntry(LogType.Info, KioskId, "Error in Authorizer-Kyc " + AuthorizerKycMessage["description"].ToString(), _MethodName);
                                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, AuthorizerKycMessage["description"]?.ToString());
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", "Verification Failed !"));
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", "Your Personal Information is not as per Nadra record please correct your information and try again"));
                                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("ErrorType", "Retry"));
                            }
                            else if (AuthorizerKycMessage["description"].ToString().ToLower() == ApiResponseConstants.PleaseProceedAccountWithDesk)
                            {
                                // <---Customer Profile Status--->
                                Logs.WriteLogEntry(LogType.Info, KioskId, "Going to send request of {CustomerProfileStatus}:", _MethodName);
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

                                Logs.WriteLogEntry(LogType.Info, KioskId, "CustomerProfileStatus Request" + JsonConvert.SerializeObject(jsonRequest4), _MethodName);

                                APIResponse aPIRespons4 = await apiService.SendRestTransaction(CustomerProfileStatusUrl, HttpMethods.POST, jsonRequest4, accessToken, "");
                                JObject CustomerProfileStatusResponse = JObject.Parse(aPIRespons4.ResponseContent);
                                var CustomerProfileStatus = CustomerProfileStatusResponse["data"];
                                var CustomerProfileStatusmessage = CustomerProfileStatusResponse["message"];

                                Logs.WriteLogEntry(LogType.Info, KioskId, "Response CustomerProfileStatus :" + aPIRespons4.ResponseContent, _MethodName);
                                if (aPIRespons4.StatusCode == HttpStatusCode.OK)
                                {
                                    Logs.WriteLogEntry(LogType.Info, KioskId, "CustomerProfileStatus" + CustomerProfileStatusmessage["description"], _MethodName);
                                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, CustomerProfileStatusmessage["description"]?.ToString());
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", "Application Submitted"));
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.ApplicationSubmitted));
                                }
                                else
                                {
                                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Response CustomerProfileStatus :" + aPIRespons4.ResponseContent, _MethodName);
                                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, CustomerProfileStatusmessage["description"]?.ToString());
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));

                                }
                            }
                            else
                            {
                                Logs.WriteLogEntry(LogType.Warning, KioskId, "Response AuthorizerKycMessage :" + AuthorizerKycMessage["description"]?.ToString(), _MethodName);
                                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, AuthorizerKycMessage["description"].ToString());
                                if (!string.IsNullOrEmpty(AuthorizerKycMessage["description"].ToString()))
                                {
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", AuthorizerKycMessage["description"].ToString()));

                                }
                                else
                                {
                                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                                }
                            }
                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry(LogType.Warning, KioskId, "Update KYC Response Data is Null", _MethodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, msg);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, KioskId, " {update-kyc} Request failed  " + updateKycMessage, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, updateKycMessage.ToString());
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"Exception in {{Review Details}}: {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "UserPicture !: " + UserPicture, _MethodName);
                JObject jsonResponse = JObject.Parse(nadraResponse);
                var consumerList = jsonResponse["data"]?["consumerList"];
                string DecryptPath = Decrypt(customerProfileImagePath);

                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [Customer Nadra Image]:  {DecryptPath}", _MethodName);

                string url = MyPdaUrl + ConfigurationManager.AppSettings["Liveliness"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                #region Selfie Image 

                // Selfie Image Processing
                string selfieImageToBase64 = "";
                string _SourcePath = ConfigurationManager.AppSettings["LiveImageBasePath"].ToString();
                string UserFileName = UserPicture.Replace("path=", ""); // Remove "path="
                string selfieImageFileName = Path.Combine(UserFileName, "Photo.Jpeg");
                string selfieImageSourceFile = Path.Combine(_SourcePath, selfieImageFileName);

                Logs.WriteLogEntry(LogType.Info, KioskId, $"Generated Selfie Image Source File Path: {selfieImageSourceFile}", _MethodName);

                selfieImageToBase64 = ConvertImageToBase64(selfieImageSourceFile, _MethodName, KioskId);

                if (selfieImageToBase64 != null)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Selfie Image Base64 conversion successful", _MethodName);
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Selfie Image Base64 conversion failed", _MethodName);
                }

                #endregion

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
                            ["livenessImage"] = selfieImageToBase64,
                            ["livenessFlag"] = true
                        }
                    };

                }

                Logs.WriteLogEntry(LogType.Info, KioskId, "jsonRequest !: " + JsonConvert.SerializeObject(requestData), _MethodName);
                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, accessToken, "");
                JObject LiveImageResponse = JObject.Parse(apiResponse.ResponseContent);
                var LiveImageData = LiveImageResponse["message"];

                var status = LiveImageData["status"]?.ToString();
                var description = LiveImageData["description"]?.ToString();
                var errorDetail = LiveImageData["errorDetail"]?.ToString();

                Logs.WriteLogEntry(LogType.Info, KioskId, "LiveImageData !: " + LiveImageData, _MethodName);


                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Success));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);

                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "LiveImageData " + description, _MethodName);
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, description);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("RespMessage", APIResultCodes.Unsuccessful));
                    var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", description));
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Error in Failed to Get Card Status!: " + ex, _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }

            return response.ToString();
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
                Logs.WriteLogEntry(LogType.Info, KioskId, "ABLDebitCardIssuance Step 1: Validating Input Data" + request.ToString(), _MethodName);
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

                                Logs.WriteLogEntry(LogType.Info, KioskId, $"tBranchCode for '{branchName}' is: {CompanyCode}", _MethodName);
                            }
                            else
                            {

                                Logs.WriteLogEntry(LogType.Warning, KioskId, $"Branch '{branchName}' not found in Islamic branch list.", _MethodName);
                            }
                        }
                        else
                        {
                            Logs.WriteLogEntry(LogType.Warning, KioskId, "Islamic Branch List Not Found!", _MethodName);
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
                                Logs.WriteLogEntry(LogType.Info, KioskId, $"tBranchCode for '{branchName}' is: {CompanyCode}", _MethodName);

                            }
                            else
                            {
                                Logs.WriteLogEntry(LogType.Warning, KioskId, $"Branch '{branchName}' not found in Conventional branch list.", _MethodName);
                            }
                        }
                        else
                        {
                            Logs.WriteLogEntry(LogType.Warning, KioskId, "Conventional Branch List Not Found!", _MethodName);
                        }
                    }
                }
                #endregion

                string TransactionId = GenerateTransactionId();
                DateTime dateTime = DateTime.Now;
                string formattedDate = dateTime.ToString("dd-MM-yyyy HH:mm:ss");

                string url = T24Url + ConfigurationManager.AppSettings["ABLDebitCardIssuance"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

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

                Logs.WriteLogEntry(LogType.Info, KioskId, "Request Code: " + JsonConvert.SerializeObject(requestPayload), _MethodName);

                APIResponse aPIResponse = await apiService.SendTransaction(url, HttpMethods.POST, requestPayload, KioskId, "");

                if (aPIResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    var responseData = JsonConvert.DeserializeObject<dynamic>(aPIResponse.ResponseContent);
                    var debitCardResponse = responseData?.ABLDebitCardIssuanceRsp;
                    Logs.WriteLogEntry(LogType.Info, KioskId, "hostCode Data: " + responseData, _MethodName);

                    string hostCode = responseData?.ABLDebitCardIssuanceRsp?.HostData?.HostCode;
                    var hostDesc = responseData?.ABLDebitCardIssuanceRsp?.HostData?.HostDesc;

                    if (hostCode == "00")
                    {
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Status Code: " + debitCardResponse.StatusCode, _MethodName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "StatusDesc: " + debitCardResponse.StatusDesc, _MethodName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "STAN: " + debitCardResponse.STAN, _MethodName);


                        Logs.WriteLogEntry(LogType.Info, KioskId, "Host Code: " + debitCardResponse.HostData, _MethodName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Host Description: " + debitCardResponse.StatusDesc, _MethodName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Transaction Reference No: " + debitCardResponse.HostData.TransReferenceNo, _MethodName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Transaction Reference No: " + debitCardResponse.HostData.HostCode, _MethodName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Transaction Reference No: " + debitCardResponse.HostData.HostDesc, _MethodName);

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
                            Logs.WriteLogEntry(LogType.Info, KioskId, "Host Code 3: " + item.name + " - " + item.content, _MethodName);
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

                        SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
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
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, errorMessage);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("MessageHead", ""));
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", errorMessage));
                    }
                }
                else
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, aPIResponse.ResponseContent);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));

                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"Exception in AOABLDebitCardIssuance: {ex}", _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
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
                Logs.WriteLogEntry(LogType.Info, KioskId, "KIOSK ID: " + kioskID, _MethodName);

                string PcName = ConfigurationManager.AppSettings[kioskID].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, "PC NAME: " + PcName, _MethodName);

                string[] parts = PcName.Split('|');

                string ComputerName = parts[0].Trim();
                string BranchCode = parts[1].Trim();

                Console.WriteLine($"Computer Name: {ComputerName}");
                Console.WriteLine($"Branch Code: {BranchCode}");

                Logs.WriteLogEntry(LogType.Info, KioskId, "IRISCardIssuance Step 1: " + request.ToString(), _MethodName);
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
                string SelectedCardName = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("SelectedCardName")?.Value ?? string.Empty;
                string Scheme = request.Element(TransactionTags.Request).Element(TransactionTags.Body).Element("Scheme")?.Value ?? string.Empty;

                CNIC = CNIC.Replace("-", "");


                int from = 1000;
                int to = 3015;
                string AccountType = "10";
                Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance AccountType : " + AccountType, _MethodName);

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
                Logs.WriteLogEntry(LogType.Info, KioskId, "ISO Code Found Against Currency Code: " + isoCode, _MethodName);
                string finaCurrenctCode = Convert.ToString(isoCode).ToString();

                string ActivationDate = DateTime.Now.ToString("yyyyMMdd");
                Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance ActivationDate : " + ActivationDate, _MethodName);

                string URL = IrisUrl + ConfigurationManager.AppSettings["IRISCardIssuance"].ToString();
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]: {URL}", _MethodName);
                InstantCard webService = new InstantCard();
                webService.Url = URL;

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

                Logs.WriteLogEntry(LogType.Info, KioskId, requestLog, _MethodName);

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

                Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance API Response responseCode : " + responseCode, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance API Response trackingID : " + trackingID, _MethodName);
                Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance API Response responseDescription : " + responseDescription, _MethodName);

                if (responseDescription == "Success" && responseCode == "00")
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "CardIssuance API Response Description is Success", _MethodName);

                    CardInfo cardInfo = DecryptEmbossingFile(BranchCode, ProductCode, KioskId, Scheme);

                    if (cardInfo != null)
                    {
                        Logs.WriteLogEntry(LogType.Info, KioskId, cardInfo.CardHolderName, _MethodName);

                        string Description = "";
                        HardwareResponse hardwareResponse = CardPersonalization(cardInfo, ComputerName, SelectedCardName, out Description, kioskID);
                        if (hardwareResponse.data.ToString() != "" && hardwareResponse.data != null)
                        {
                            Logs.WriteLogEntry(LogType.Info, KioskId, "Personlization Response : " + hardwareResponse.description, _MethodName);
                            var bodyElement = response.Element(TransactionTags.Response).Element(TransactionTags.Body);
                            bodyElement.Add(new XElement("RespMessage", APIResultCodes.Success),
                                new XElement("RequestId", hardwareResponse.data));
                            SetResponseHeader(response, TransactionResultString.Success, APIResultCodes.Success, ApiResponseConstants.SuccessStatus);
                        }
                        else
                        {
                            Logs.WriteLogEntry(LogType.Warning, KioskId, "Data is Null  " + hardwareResponse.description, _MethodName);
                            SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, hardwareResponse.description.ToString());
                            response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                        }

                    }
                    else
                    {
                        Logs.WriteLogEntry(LogType.Warning, KioskId, "cardInfo", _MethodName);
                        SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ApiResponseConstants.Message_UnableToProcess);
                        response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                    }
                }
                else
                {
                    SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, responseDescription);
                    response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
                }
            }
            catch (ArgumentNullException argEx)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "ArgumentNullException in CardIssuance: " + argEx, _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, argEx.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }
            catch (InvalidOperationException invOpEx)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "InvalidOperationException in CardIssuance: " + invOpEx, _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, invOpEx.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));

            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "General Exception in CardIssuance: " + ex, _MethodName);
                SetResponseHeader(response, TransactionResultString.Failed, APIResultCodes.Unsuccessful, ex.Message);
                response.Element(TransactionTags.Response).Element(TransactionTags.Body).Add(new XElement("Message", ApiResponseConstants.Message_UnableToProcess));
            }
            return response.ToString();
        }

        #endregion

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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1014 } };
                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Occupation List Request: " + requestData.ToString(), _MethodName);
                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var occupations = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "Occupation List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);
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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1016 } };
                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Profession List Request: " + requestData.ToString(), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var professions = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "Profession List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1016 } };

                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Town Tehsil List Request: " + Newtonsoft.Json.JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, KioskId, Newtonsoft.Json.JsonConvert.SerializeObject(requestData), "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var towntehsillist = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "Town / Tehsil List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Islamic Branch List Request: " + JsonConvert.SerializeObject(requestData), _MethodName);
                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject responseJson = JObject.Parse(apiResponse.ResponseContent);
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Islamic Branch List Success Response", _MethodName);
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
                                {"fcyBranch", branch["fcyBranch"]?.ToObject<int?>() ?? 0},
                                { "distance", branch["distance"]?.ToObject<double?>() ?? 0.0 }

                            })
                            .ToList();
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Conventional Branch List Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Conventional Branch List Success Response", _MethodName);
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
                                {"fcyBranch", branch["fcyBranch"]?.ToObject<int?>() ?? 0},
                                { "distance", branch["distance"]?.ToObject<double?>() ?? 0.0 }

                            })
                            .ToList();
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Error, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex.Message, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1006 } };

                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Gender List Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var genderList = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "Gender List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                var requestData = new { data = new { codeTypeId = 1081 } };
                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Account Purpose List Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var AccountList = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "AccountPurposeList List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);

                var requestData = new { data = new { codeTypeId = 1006, codeOrder = 2, codeDescription = "C" } };
                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var ConventionalSavingsAccountVariants = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "ConventionalSavingsAccountVariants List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1082, codeOrder = 1, codeDescription = "C" } };

                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var ConventionalCurrentAccountVariants = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "ConventionalCurrentAccountVariants List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1082, codeOrder = 1, codeDescription = "C" } };

                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var IslamicSavingsAccountVariants = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "IslamicSavingsAccountVariants List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
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
                Logs.WriteLogEntry(LogType.Info, KioskId, $"{_MethodName} [URL]:  {url}", _MethodName);
                var requestData = new { data = new { codeTypeId = 1082, codeOrder = 1, codeDescription = "C" } };

                Logs.WriteLogEntry(LogType.Info, KioskId, "Sending Request: " + JsonConvert.SerializeObject(requestData), _MethodName);

                var apiResponse = await apiService.SendRestTransaction(url, HttpMethods.POST, requestData, KioskId, "");
                if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(apiResponse.ResponseContent);
                    var IslamicCurrentAccountVariants = jsonResponse["data"];

                    Logs.WriteLogEntry(LogType.Info, KioskId, "IslamicCurrentAccountVariants List Retrieved Successfully.", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Message Status: {jsonResponse["message"]?["status"]}", _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, $"Description: {jsonResponse["message"]?["description"]}", _MethodName);

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
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "No response received from the API." + apiResponse.ResponseContent, _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Parsing Error: " + ex, _MethodName);
            }

            return GenderList;
        }


        #endregion



        #endregion

        #region Functions

        #region Extract Error Message
        public static string ExtractErrorMessage(dynamic responseJson, string KioskId)
        {
            string _MethodName = "ExtractErrorMessage";
            try
            {
                Logs.WriteLogEntry(LogType.Info, KioskId, "Step 1: ", _MethodName);

                var rsp = responseJson?.ABLDebitCardIssuanceRsp ?? responseJson;

                if (rsp?["HostData"]?["HostCode"]?.ToString() == "00")
                {
                    return "Success";
                }

                Logs.WriteLogEntry(LogType.Info, KioskId, "Extracting HostDesc", _MethodName);

                JToken hostDesc = rsp?["HostData"]?["HostDesc"];

                if (hostDesc == null)
                    return ApiResponseConstants.Message_UnableToProcess;

                if (hostDesc.Type == JTokenType.String)
                {
                    return hostDesc.ToString();
                }

                if (hostDesc.Type == JTokenType.Object)
                {
                    return hostDesc["content"]?.ToString();
                }

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
                                    Logs.WriteLogEntry(LogType.Info, KioskId, content.ToString(), _MethodName);
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
                return $"Error parsing response: {ex}";
            }
        }
        #endregion

        #region Import Excel
        public static DataTable ImportExcel(string KioskId)
        {
            string filePath = ConfigurationManager.AppSettings["ExcelFilePath"].ToString();
            Logs.WriteLogEntry(LogType.Info, KioskId, "Excel File Path: Step 1" + filePath, "ImportExcel");

            DataTable dataTable = new DataTable();

            if (!System.IO.File.Exists(filePath))
            {
                Logs.WriteLogEntry(LogType.Info, KioskId, "Excel File Path Not Found: Step 2" + filePath, "ImportExcel");
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
                Logs.WriteLogEntry(LogType.Info, KioskId, "Excel Data: Step 2" + dataTable, "ImportExcel");
            }
            return dataTable;
        }

        #endregion

        #region Extract And Log Values
        static List<(string ID, string Content)> ExtractAndLogValues(string jsonResponse)
        {
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
            return idContentList;
        }

        #endregion

        #region Decrypt Embossing Files
        public static CardInfo DecryptEmbossingFile(string BranchCode, string ProductCode, string KioskId, string Scheme)
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

                Logs.WriteLogEntry(LogType.Info, KioskId, "Passphrase Key!: " + passphrase, "DecryptEmbossingFile");
                Logs.WriteLogEntry(LogType.Info, KioskId, "Decrypted File Path!: " + DecryptedFilePath, "DecryptEmbossingFile");
                Logs.WriteLogEntry(LogType.Info, KioskId, "Private Key!: " + privateKey, "DecryptEmbossingFile");
                Logs.WriteLogEntry(LogType.Info, KioskId, "VSMCard Baes Url!: " + VSMCardBaesUrl, "DecryptEmbossingFile");
                Logs.WriteLogEntry(LogType.Info, KioskId, "Drafted Card Files!: " + DraftedCardFiles, "DecryptEmbossingFile");
                DateTime startTime = DateTime.Now;

                bool fileFound = false;
                string expectedFileName = $"EN-{BranchCode}";
                string expectedFileName1 = BranchCode;

                Logs.WriteLogEntry(LogType.Info, KioskId, "InstantCardExportFiles Path!: " + VSMCardBaesUrl, "DecryptEmbossingFile");
                if (Directory.Exists(VSMCardBaesUrl))
                {
                    while ((DateTime.Now - startTime).TotalSeconds < 60)
                    {
                        string[] files = Directory.GetFiles(VSMCardBaesUrl);
                        var targetFile = files.Where(f => Path.GetFileName(f).Contains(ProductCode)).ToList();
                        Logs.WriteLogEntry(LogType.Info, KioskId, "targetFile !: " + targetFile.Count, "DecryptEmbossingFile");
                        if (targetFile.Any())
                        {
                            Thread.Sleep(5000);
                            foreach (string file in targetFile)
                            {
                                Logs.WriteLogEntry(LogType.Info, KioskId, "expectedFileName !: " + file, "DecryptEmbossingFile");
                                string Filename = Path.GetFileName(file);
                                if (Filename.StartsWith(expectedFileName) || Filename.StartsWith(expectedFileName1))
                                {
                                    VSMCardBaesUrl = Path.Combine(VSMCardBaesUrl, Path.GetFileName(file));
                                    Logs.WriteLogEntry(LogType.Info, KioskId, "File Found For Decrypt!: " + VSMCardBaesUrl, "DecryptEmbossingFile");
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
                    Logs.WriteLogEntry(LogType.Info, KioskId, VSMCardBaesUrl + " Directory Not Found!: ", "DecryptEmbossingFile");
                }

                if (fileFound)
                {
                    string outputFile = $"{DecryptedFilePath}{BranchCode}{ProductCode}{DateTime.Now.ToString("ddMMyyyyHHmmss")}.txt";
                    Logs.WriteLogEntry(LogType.Info, KioskId, outputFile + "Decrypt File Path", "DecryptEmbossingFile");

                    Logs.WriteLogEntry(LogType.Info, KioskId, "Decrypt Step 1", "DecryptEmbossingFile");
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

                    if (Scheme.ToLower() == "upi")
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
                    if (Scheme.ToLower() == "upi")
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
                        if (Scheme.ToLower() == "upi")
                        {
                            Logs.WriteLogEntry(LogType.Info, KioskId, "Going to Get Co-Bage Card Data :" + ProductCode, "DecryptEmbossingFile");
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
                            Logs.WriteLogEntry(LogType.Info, KioskId, "Going to Get VISA Card Data :" + ProductCode, "DecryptEmbossingFile");
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


                        Logs.WriteLogEntry(LogType.Info, KioskId,
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
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Decrypt Step 4", "DecryptEmbossingFile");
                    if (System.IO.File.Exists(VSMCardBaesUrl))
                    {
                        string fileName = Path.GetFileName(VSMCardBaesUrl);
                        string destinationPath = Path.Combine(DraftedCardFiles, fileName);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Complete Drafted Card Files Path:" + destinationPath, "DecryptEmbossingFile");
                        System.IO.File.Move(VSMCardBaesUrl, destinationPath);
                        Logs.WriteLogEntry(LogType.Info, KioskId, "File Move on Draft folder", "DecryptEmbossingFile");

                    }

                }

            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(VSMCardBaesUrl))
                {
                    string fileName = Path.GetFileName(VSMCardBaesUrl);
                    string destinationPath = Path.Combine(DraftedCardFiles, fileName);
                    Logs.WriteLogEntry(LogType.Error, KioskId, "Complete Drafted Card Files Path:" + destinationPath, "DecryptEmbossingFile");
                    System.IO.File.Move(VSMCardBaesUrl, destinationPath);
                    Logs.WriteLogEntry(LogType.Error, KioskId, "File Move on Draft folder", "DecryptEmbossingFile");

                }
                Logs.WriteLogEntry(LogType.Error, KioskId, "Failed to Decrypt Embossing File!: " + ex.Message, "DecryptEmbossingFile");
                Logs.WriteLogEntry(LogType.Error, KioskId, "Failed to Decrypt Embossing File!, Inner Exception: " + ex.InnerException, "DecryptEmbossingFile");
            }
            return cardList;
        }
        #endregion

        #region Get Accout List With Account Names

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
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Gender List Found: " + string.Join(", ", genderList), _MethodName);
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
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Gender List Not Found!", _MethodName);
                }

                Occupation occupationData = new Occupation();

                List<Dictionary<string, object>> occupationList = await GetOccupationListAsync(KioskId);

                if (occupationList.Any())
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Occupation List Found: " + string.Join(", ", occupationList), _MethodName);

                    foreach (var occupation in occupationList)
                    {
                        if (occupation.ContainsKey("id") && occupation.ContainsKey("name") && int.TryParse(occupation["id"].ToString(), out int occupationId))
                        {
                            string occupationValue = Convert.ToString(occupation["id"].ToString());

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
                        Logs.WriteLogEntry(LogType.Info, KioskId, "occupationData.DailyWager " + occupationData.DailyWager + " occupationData.SelfEmployed" + occupationData.SelfEmployed, _MethodName);
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Occupation List Not Found!", _MethodName);
                }
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

                variantInfo = GetAsaanAccountVariantID(bankingMode, accountType, accountPurpose, DateOfBirth, accountsSelectionList, KioskId);
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Error :" + ex.Message, _MethodName);
            }

            return variantInfo;
        }

        #endregion

        #region Get Asaan Account Variant ID
        public VariantInfo GetAsaanAccountVariantID(string bankingModeId, int CustomerAccountTypeId, string purposeOfAccountId, DateTime DateOfBirth, List<AccountVariant> accountsSelectionList, string KioskId)
        {
            VariantInfo variantInfo = new VariantInfo();
            string _MethodName = "GetAsaanAccountVariantID";

            try
            {
                int BankingModeId = int.Parse(bankingModeId);
                int PurposeOfAccountId = int.Parse(purposeOfAccountId);
                int consumerAge = GetAgeCountFromDate(DateOfBirth);

                Logs.WriteLogEntry(LogType.Info, KioskId, "consumerAge" + consumerAge, _MethodName);

                List<AccountVariant> variantsFiltered = new List<AccountVariant>();
                foreach (var item in accountsSelectionList)
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "BankingModeId - " + item.BankingModeId + "_User -" + bankingModeId, _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, "CustomerAccountTypeId - " + item.CustomerAccountTypeId + "_User -" + CustomerAccountTypeId, _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, "PurposeOfAccount - " + item.PurposeOfAccount + "_User -" + PurposeOfAccountId, _MethodName);

                }

                Logs.WriteLogEntry(LogType.Info, KioskId, "consumerAge step 1 " + consumerAge, _MethodName);
                variantsFiltered = accountsSelectionList.Where(variant =>
                    variant.BankingModeId == BankingModeId &&
                    variant.CustomerAccountTypeId == CustomerAccountTypeId &&
                    variant.PurposeOfAccount == PurposeOfAccountId &&
                    variant.MinAge >= 18

                ).ToList();


                Logs.WriteLogEntry(LogType.Info, KioskId, "Step 2", _MethodName);
                if (variantsFiltered.Any())
                {
                    Logs.WriteLogEntry(LogType.Info, KioskId, "Step 3", _MethodName);
                    variantInfo.Id = variantsFiltered.First().Id;
                    variantInfo.Name = variantsFiltered.First().Name;

                    Logs.WriteLogEntry(LogType.Info, KioskId, "variantID " + variantInfo.Id, _MethodName);
                    Logs.WriteLogEntry(LogType.Info, KioskId, "variant Name  " + variantInfo.Name, _MethodName);
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, "Variant Not Found", _MethodName);
                }


            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, "Error :" + ex, _MethodName);
            }

            return variantInfo;
        }

        #endregion

        #region Get Age Count From Date
        private int GetAgeCountFromDate(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
        #endregion

        #region FreshCardListing
        public async Task<Card> FreshCardListing(string CnicNumber, string AccountNumber, string KioskId)
        {
            string _MethodName = "FreshCardListing";
            Card freshCardList = null;

            try
            {
                string url = IrisUrl + ConfigurationManager.AppSettings["IRISExistingCardList"];
                Logs.WriteLogEntry(LogType.Info, KioskId, $"Request URL: {url}", _MethodName);

                wsABLCARDSTATUSCHANGE webService = new wsABLCARDSTATUSCHANGE { Url = url };
                var result = webService.FreshCardListing(CnicNumber);
                string innerXml = XMLHelper.ExtractInnerXml(result);
                string cleanedXml = XMLHelper.FixNestedCardInfo(innerXml);

                Logs.WriteLogEntry(LogType.Info, KioskId, $"Cleaned XML: {cleanedXml}", _MethodName);

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
                            Logs.WriteLogEntry(LogType.Info, KioskId, $"Fresh card found — ProductCode: {freshCardList.PRODUCTCODE}, AccountID: {freshCardList.ACCOUNTID}, CardStatus: {freshCardList.CARDSTATUS}, Expiry: {freshCardList.CARDEXPIRYDATE}", _MethodName);
                        }
                    }
                    else
                    {
                        Logs.WriteLogEntry(LogType.Warning, KioskId, $"No fresh cards found Againts {AccountNumber} Account Number ", _MethodName);
                    }
                }
                else
                {
                    Logs.WriteLogEntry(LogType.Warning, KioskId, $"No fresh cards found or response not approved for CNIC: {CnicNumber}", _MethodName);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLogEntry(LogType.Error, KioskId, $"Exception in FreshCardListing: {ex}", _MethodName);
            }

            return freshCardList;
        }

        #endregion

        #region  Convert Image To Base64
        public static string ConvertImageToBase64(string filePath, string methodName, string KioskId)
        {
            if (System.IO.File.Exists(filePath))
            {
                Logs.WriteLogEntry(LogType.Info, KioskId, $"File Found: {filePath}", methodName);

                try
                {
                    using (Image image = Image.FromFile(filePath))
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, image.RawFormat);
                        byte[] imageBytes = memoryStream.ToArray();
                        Logs.WriteLogEntry(LogType.Info, KioskId, "Image processed successfully", methodName);
                        return Convert.ToBase64String(imageBytes);
                    }
                }
                catch (Exception ex)
                {
                    Logs.WriteLogEntry(LogType.Error, KioskId, $"Error processing image: {ex}", methodName);
                    return null;
                }
            }
            else
            {
                Logs.WriteLogEntry(LogType.Info, KioskId, $"File Not Found: {filePath}", methodName);
                return null;
            }
        }
        #endregion

        #region Get Iso Code

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
        #endregion

        #region Decrypt
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
        #endregion

        #region Encrypt Using AES256

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

        #endregion

        #region  Map To ABL Card Info

        private static ABLCardInfo MapToABLCardInfo(DataRow row, string productcode)
        {
            return new ABLCardInfo
            {
                IrisCardProductCode = row["IRIS Card Product Code"]?.ToString(),
                name = row["IRIS Card Product Description (Card Variant)"]?.ToString(),
                KgsName = row["KGS Card Format Name (Card plastic)"]?.ToString(),
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
        }
        #endregion

        #region Get BankIMD

        private string GetBankIMD(string productCode)
        {
            switch (productCode)
            {
                case "0092": return "428638";
                case "0071": return "407572";
                case "0070": return "476215";
                case "0075": return "476215";
                case "0080": return "629240";
                default: return "";
            }
        }
        #endregion

        #region Extract Digits Only
        public static string ExtractDigitsOnly(string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        #endregion

        #region  Get Account Type
        private string GetAccountType(int categoryCode)
        {
            return (categoryCode >= 1000 && categoryCode <= 3015) ? "20" : "10";
        }
        #endregion

        #endregion

        #region Enum & Models

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
                    Logs.WriteLogEntry(LogType.Info, "5", $"LogonUser failed: {Marshal.GetLastWin32Error()}", "Impersonate");

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
                        Logs.WriteLogEntry(LogType.Info, "5", $"ImpersonateLoggedOnUser failed: {Marshal.GetLastWin32Error()}", "Impersonate");

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


        public class VariantInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        #endregion

    }
}
