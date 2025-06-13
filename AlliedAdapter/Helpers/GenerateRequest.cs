//using AlliedAdapter.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AlliedAdapter.Helpers
//{
//    public static class GenerateRequest
//    {
//        public static TransactionRequest FundTransferRequest(FundTransferRequest fundTransferRequest, out string signatureDate)
//        {
//            var transactionString = $"action={fundTransferRequest.Action},credit_account={fundTransferRequest.CreditAccount},debit_account={fundTransferRequest.DebitAccount},amount={fundTransferRequest.Amount},currency={fundTransferRequest.Currency},transaction_id={fundTransferRequest.TransactionId},description={fundTransferRequest.Description},sender_name={fundTransferRequest.SenderName},sender_mobile={fundTransferRequest.SenderMobile}";
//            signatureDate = $"action:{fundTransferRequest.Action} ,credit_account: {fundTransferRequest.CreditAccount} ,debit_account: {fundTransferRequest.DebitAccount} ,amount: {fundTransferRequest.Amount} ,currency: {fundTransferRequest.Currency},transaction_id:{fundTransferRequest.TransactionId},description:{fundTransferRequest.Description},sender_name:{fundTransferRequest.SenderName},sender_mobile:{fundTransferRequest.SenderMobile}";

//            return new TransactionRequest
//            {
//                transaction = transactionString
//            };
//        }

//        public static TransactionRequest InterBankFundTransferRequest(InterBankFundTransferRequest interBankFundTransferRequest, out string signatureDate)
//        {
//            var transactionString = $"transaction_id={interBankFundTransferRequest.transaction_id},bank_code={interBankFundTransferRequest.bank_code},amount={interBankFundTransferRequest.amount},debit_account={interBankFundTransferRequest.debit_account},receiver_account={interBankFundTransferRequest.receiver_account},receiver_name={interBankFundTransferRequest.receiver_name},receiver_mobile={interBankFundTransferRequest.receiver_mobile},narration={interBankFundTransferRequest.narration}";
//            signatureDate = $"transaction_id:{interBankFundTransferRequest.transaction_id},bank_code:{interBankFundTransferRequest.bank_code},amount:{interBankFundTransferRequest.amount},debit_account:{interBankFundTransferRequest.debit_account},receiver_account:{interBankFundTransferRequest.receiver_account},receiver_name:{interBankFundTransferRequest.receiver_name},receiver_mobile:{interBankFundTransferRequest.receiver_mobile},narration:{interBankFundTransferRequest.narration}";

//            return new TransactionRequest
//            {
//                transaction = transactionString
//            };
//        }

//        public static TransactionRequest BillPaymentRequest(PayBillRequest payBillRequest, out string signatureDate)
//        {
//            var transactionString = $"transaction_id={payBillRequest.transaction_id},provider_id={payBillRequest.provider_id},account={payBillRequest.account},amount={payBillRequest.amount},debit_account={payBillRequest.debit_account},credit_account={payBillRequest.credit_account},sender_name={payBillRequest.sender_name},sender_mobile={payBillRequest.sender_mobile}";
//            signatureDate = $"transaction_id:{payBillRequest.transaction_id},provider_id:{payBillRequest.provider_id},account:{payBillRequest.account},amount:{payBillRequest.amount},debit_account:{payBillRequest.debit_account},credit_account:{payBillRequest.credit_account},sender_name:{payBillRequest.sender_name},sender_mobile:{payBillRequest.sender_mobile}";

//            return new TransactionRequest
//            {
//                transaction = transactionString
//            };
//        }

//        public static TransactionRequest MobileTransferRequest(MobileTransferRequest mobileTransferRequest, out string signatureDate)
//        {
//            var transactionString = $"transaction_id={mobileTransferRequest.transaction_id},provider={mobileTransferRequest.provider},amount={mobileTransferRequest.amount},debit_account={mobileTransferRequest.debit_account},receiver_account={mobileTransferRequest.receiver_account},receiver_name={mobileTransferRequest.receiver_name},receiver_mobile={mobileTransferRequest.receiver_mobile},narration={mobileTransferRequest.narration}";
//            signatureDate = $"transaction_id:{mobileTransferRequest.transaction_id},provider:{mobileTransferRequest.provider},amount:{mobileTransferRequest.amount},debit_account:{mobileTransferRequest.debit_account},receiver_account:{mobileTransferRequest.receiver_account},receiver_name:{mobileTransferRequest.receiver_name},receiver_mobile:{mobileTransferRequest.receiver_mobile},narration:{mobileTransferRequest.narration}";

//            return new TransactionRequest
//            {
//                transaction = transactionString
//            };
//        }
//        public static TransactionRequest WalletTransferRequest(PushToWalletRequest pushToWalletRequest, out string signatureDate)
//        {
//            var transactionString = $"transaction_id={pushToWalletRequest.transaction_id},amount={pushToWalletRequest.amount},debit_account={pushToWalletRequest.debit_account},wallet={pushToWalletRequest.wallet},sender_name={pushToWalletRequest.sender_name},sender_mobile={pushToWalletRequest.sender_mobile}";

//                    signatureDate = $"transaction_id:{pushToWalletRequest.transaction_id},amount:{pushToWalletRequest.amount},debit_account:{pushToWalletRequest.debit_account},wallet:{pushToWalletRequest.wallet},sender_name:{pushToWalletRequest.sender_name},sender_mobile:{pushToWalletRequest.sender_mobile}";

//            return new TransactionRequest
//            {
//                transaction = transactionString
//            };
//        }


//    }
//}
