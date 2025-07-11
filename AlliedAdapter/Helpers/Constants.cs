using Sedco.SelfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AlliedAdapter.Helpers
{
    public static class Constants
    {
        public enum HttpMethods
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH


        }
        public enum LogType
        {
            Info,
            Warning,
            Error
        }

        public static class APIResultCodes
        {
            public const string Success = "200";
            public const string Unsuccessful = "500";
            public const string NotFound = "404";
        }

        public static class MobilMoneyProviders
        {
            public const string TNMMpamba = "TNM_MPAMBA";
            public const string AirtelMoney = "AIRTEL_MONEY";
        }

        public static class BankCodes
        {
            public const string FDH = "900006";
            public const string NBS = "900005";
            public const string StandardBank = "900009";
            public const string FCB = "900011";
            public const string Ecobank = "900001";
            public const string CDHInvestmentBank = "900002";
            public const string CentenaryBank = "900113";
            public const string UnayoStandardBank = "900019";
        }

        public static class ApiStatusCodes
        {
            public const string Success = "00";
            public const string Failed = "100";
            public const string ValidationSuccess = "Validation successful";
            public const string AccountNotExist = "AccountNotExist";
            public const string UnableToProcessRequest = "UnableToProcessRequest";
        }
        public static void SetResponseHeader(XDocument response, string resultCode, string apiResultCode, string description)
        {
            var header = response.Element(TransactionTags.Response).Element(TransactionTags.Header);
            header.Element(TransactionTags.ResultCode).Value = resultCode;
            header.Element(TransactionTags.APIResultCode).Value = apiResultCode;
            header.Element(TransactionTags.ResultDescription).Value = description;
        }


    }
}
