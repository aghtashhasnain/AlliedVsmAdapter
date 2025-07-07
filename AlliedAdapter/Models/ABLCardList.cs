using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AlliedAdapter.Models
{
    public class ABLCardListResponse
    {
        public List<ABLCardInfo> data { get; set; }

        public ResponseMessage message { get; set; }
    }

    public class ABLCardInfo
    {
        public string IrisCardProductCode { get; set; } //IRIS Card Product Code
        public string name { get; set; } //IRIS Card Product Description (Card Variant)
        public string KgsName { get; set; } // KGS Card Format Name (Card plastic)

        public string description { get; set; }
        public string t24AccountCategoryCode { get; set; } // T24 Account Category Code
        public string Currency { get; set; } //Currency
        public string variant { get; set; } 
        public string scheme { get; set; } 
        public string t24CardCode { get; set; } //T24 Card Code
        public string issuanceCharges { get; set; } //Issuance Charges
        public string replacementCharges { get; set; } //Replacement Charges
        public string perDayFT { get; set; } // Per Day ATM IBF/FT 
        public string billPaymentLimit { get; set; } // Bill Payment Limit/Donation
        public string cashWithdrawalLimit { get; set; } // ATM Cash Withdrawal Limit
        public string eCommerceLimit { get; set; } //POS/eCommerce Limit
        public string ImagePath { get; set; } 
    }

    public class ResponseMessage
    {
        public string status { get; set; }

        public string description { get; set; }
    }

    public class ABLCardListRequest
    {
        public DataRequest data { get; set; }
    }

    public class DataRequest
    {
        public int CodeTypeId { get; set; }
    }
}
