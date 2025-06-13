using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace AlliedAdapter.Helpers
{
    public static class EndPoints
    {
        public readonly static string BASE_URL = ConfigurationManager.AppSettings["BaseURL"];   
        public readonly static string SIGNING_KEY = $"{BASE_URL}/auth/signing-key";

        public readonly static string BALANCE_ENQUIRY = $"{BASE_URL}/bank/balance";
        public readonly static string PROOF_OF_PAYMENT = $"{BASE_URL}/bank/validate";
        public readonly static string ACCOUNT_MINI_STATEMENT = $"{BASE_URL}/bank/statement";
        public readonly static string ACCOUNT_FULL_STATEMENT = $"{BASE_URL}/bank/statement/full";
        public readonly static string FUND_TRANSFER = $"{BASE_URL}/bank/transfer";

        public readonly static string VALIDATE_BILLER_ACCOUNT = $"{BASE_URL}/bills/validate";
        public readonly static string FETCH_BILLING_PRODUCTS = $"{BASE_URL}/bills/products";
        public readonly static string PAY_BILL = $"{BASE_URL}/bills/pay";

        public readonly static string INTERBANK_FUND_TRANSFER = $"{BASE_URL}/interbank/transfer";
        public readonly static string MOBILE_TRANSFER = $"{BASE_URL}/mobile-money/transfer";

        public readonly static string VALIDATE_WALLET = $"{BASE_URL}/wallet/validate";
        public readonly static string WALLET_TRANSFER = $"{BASE_URL}/wallet/transfer";

        public readonly static string CREATE_CUSTOMER = $"{BASE_URL}/accounts/customers";
        public readonly static string CREATE_ACCOUNT = $"{BASE_URL}/accounts/customers/{{0}}/accounts";
        public readonly static string CLOSE_ACCOUNT = $"{BASE_URL}/accounts/close";
        public readonly static string KYC_DETAILS = $"{BASE_URL}/accounts/customers/{{0}}";
        public readonly static string ACOUNT_DETAILS = $"{BASE_URL}/accounts/details";


    }
}
