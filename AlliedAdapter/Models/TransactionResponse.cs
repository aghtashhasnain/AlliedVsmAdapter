using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AlliedAdapter.Models
{
    public class TransactionResponse
    {
    }
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }
        public string Message { get; set; }
    }

    public class ResponseContent
    {
        public string Message { get; set; }
        public object Data { get; set; }
    }


    public class SigningKeyResponse
    {
        public string key { get; set; }
    }

    public class ABLCustomerVerificationResponse
    {
        public ABLCustomerVerificationRsp ABLCustomerVerificationRsp { get; set; }
    }

    public class ABLCustomerVerificationRsp
    {
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string STAN { get; set; }
        public HostResponseData HostData { get; set; }
    }

    public class HostResponseData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("customerNumber")]
        public string CustomerNumber { get; set; }

        [JsonProperty("dob")]
        public string DOB { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("cnic")]
        public string CNIC { get; set; }

        [JsonProperty("hostdec")]
        public string HostDesc { get; set; }
    }


    public class ABLDebitCardChargesResponse
    {
        public ABLDebitCardChargesRsp ABLDebitCardChargesRsp { get; set; }
    }

    public class ABLDebitCardChargesRsp
    {
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string STAN { get; set; }
        public DebitCardHostResponseData HostData { get; set; }
    }

    public class DebitCardHostResponseData
    {
        public string HostCode { get; set; }
        public string HostDesc { get; set; }
        public DebitCardResponseItem Response { get; set; }
    }

    public class DebitCardResponseItem
    {
      public List<Item> Item { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public string Content { get; set; }
    }

    public class FinalATMCardList
    {
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string issuanceamount { get; set; }
        public string replacementamount { get; set; }

    }

    public class CardCharges {

        public string issuanceamount { get; set; }
        public string replacementamount { get; set; }
        public string message { get; set; }
    }

    public class ABLCustomerAccountListResponse
    {
        public ABLCustomerAccountListRsp ABLCustomerAccountListRsp { get; set; }
    }

    public class ABLCustomerAccountListRsp
    {
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string STAN { get; set; }
        public AccountListHostResponseData HostData { get; set; }
    }

    public class AccountListHostResponseData
    {
        public string HostCode { get; set; }
        public string HostDesc { get; set; }
        public string TransReferenceNo { get; set; }
        public AccountData Account { get; set; } // Single Account object
    }
    public class AccountData
    {
        public List<AccountColumn> Column { get; set; } // Column array inside the Account object
    }

    public class AccountColumn
    {
        public string Id { get; set; }
        public string Content { get; set; }
    }

    public class ABLDebitCardIssuanceResponse
    {
        public ABLDebitCardIssuanceRsp ABLDebitCardIssuanceRsp { get; set; }
    }

    public class ABLDebitCardIssuanceRsp
    {   
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string STAN { get; set; }
        public HostDataResponse HostData { get; set; }
    }

    public class HostDataResponse
    {
        public string TransReferenceNo { get; set; }
        public string HostCode { get; set; }
        public string HostDesc { get; set; }
        public List<FieldItem> field { get; set; }
    }

    public class FieldItem
    {
        public string sv { get; set; }
        public string name { get; set; }
        public string mv { get; set; }
        public string content { get; set; }
    }
    public class VerifyOTPResponse
    {
        public string Data { get; set; }
        public VerifyOTPMessage Message { get; set; }
    }
    public class VerifyOTPMessage
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public string ErrorDetail { get; set; }
    }

}
