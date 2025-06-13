using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
