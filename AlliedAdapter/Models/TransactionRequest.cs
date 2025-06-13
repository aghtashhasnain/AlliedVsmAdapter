using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlliedAdapter.Models
{
    public class TransactionRequest
    {
        public string transaction { get; set; }
    }

    public class SigningKeyRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }


    public class ABLCustomerVerificationRequest
    {
        public ABLCustomerVerificationReq ABLCustomerVerificationReq { get; set; }
    }

    public class ABLCustomerVerificationReq
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string ChannelType { get; set; }
        public string ChannelSubType { get; set; }
        public string TransactionType { get; set; }
        public string TransactionSubType { get; set; }
        public string TranDateAndTime { get; set; }
        public string Function { get; set; }
        public HostData HostData { get; set; }
    }

    public class ABLDebitCardChargesRequest
    {
        public ABLDebitCardCharges ABLDebitCardCharges { get; set; }
    }

    public class ABLDebitCardCharges
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string ChannelType { get; set; }
        public string ChannelSubType { get; set; }
        public string TransactionType { get; set; }
        public string TransactionSubType { get; set; }
        public string TranDateAndTime { get; set; }
        public string Function { get; set; }
        public DebitCardHostData HostData { get; set; }
    }

    public class DebitCardHostData
    {
        public string TransReferenceNo { get; set; }
        public string IDNumber { get; set; }
    }

    public class ABLCustomerAccountListRequest
    {
        public ABLCustomerAccountListReq ABLCustomerAccountListReq { get; set; }
    }

    public class ABLCustomerAccountListReq
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string ChannelType { get; set; }
        public string ChannelSubType { get; set; }
        public string TransactionType { get; set; }
        public string TransactionSubType { get; set; }
        public string TranDateAndTime { get; set; }
        public string Function { get; set; }
        public AccountListHostData HostData { get; set; }
    }
    public class AccountListHostData
    {
        public string TransReferenceNo { get; set; }
        public string CustomerNumber { get; set; }
    }

    public class ABLDebitCardIssuanceRequest
    {
        public ABLDebitCardIssuanceReq ABLDebitCardIssuanceReq { get; set; }
    }

    public class ABLDebitCardIssuanceReq
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string ChannelType { get; set; }
        public string ChannelSubType { get; set; }
        public string TransactionType { get; set; }
        public string TransactionSubType { get; set; }
        public string TranDateAndTime { get; set; }
        public string Function { get; set; }
        public HostData HostData { get; set; }
    }

    public class HostData
    {
        public string TransReferenceNo { get; set; }
        public string DPS_Scheme { get; set; }
        public string CNIC { get; set; }
        public string Company { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string PackageType { get; set; }
        public string AtmReqType { get; set; }
        public string CustomerNature { get; set; }
        public string AddressFlag { get; set; }
        public string DaoAtmAddr1 { get; set; }
        public string DaoAtmAddr2 { get; set; }
        public string DaoAtmAddr3 { get; set; }
        public string DaoAtmAddr4 { get; set; }
        public string DaoAtmAddr5 { get; set; }
    }
    public class ApiMessage
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
    public class OTPResponse
    {
        [JsonProperty("data")]
        public OTPData Data { get; set; }

        [JsonProperty("message")]
        public ApiMessage Message { get; set; }
    }
    public class OTPData
    {
        [JsonProperty("customerInstructions")]
        public string CustomerInstructions { get; set; }

        [JsonProperty("otpExpiryMinutes")]
        public int OtpExpiryMinutes { get; set; }

        [JsonProperty("mobileNo")]
        public string MobileNo { get; set; }

        [JsonProperty("idNumber")]
        public string IdNumber { get; set; }

        [JsonProperty("alreadyExist")]
        public bool AlreadyExist { get; set; }
    }
}
