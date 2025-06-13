using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfalahAdapter.Repositories
{
    public class RapidAccountOpening
    {
        public class CustomerDetails
        {
            public int customerID { get; set; }
            public string customerIDNo { get; set; }
            public string accountType { get; set; }
            public string accountCategory { get; set; }
            public string productID { get; set; }
            public string currency { get; set; }
            public string stage { get; set; }
            public string status { get; set; }
        }

        public class PersonalDetails
        {
            public int customerID { get; set; }
            public string customerName { get; set; }
            public string fatherName { get; set; }
            public string motherOrMaidenName { get; set; }
            public string Stage { get; set; }
            public string gender { get; set; }
            public string dob { get; set; }
            public int country { get; set; }
            public string landline_Number { get; set; }
            public string cityOfBirth { get; set; }

            public customerAddress customerAddress { get; set; }
            public customerFatca customerFatca { get; set; }
            public customerCRS customerCRS { get; set; }
        }

        public class customerAddress
        {
            public int customerID { get; set; }
            public string permanentAddLine1 { get; set; }
            public string permanentAddLine2 { get; set; }
            public string permanentAdd_City { get; set; }
            public int permanentAdd_CountryID { get; set; }
            public string permanentAddPostalCode { get; set; }
            public bool isCurrentAddSameAsPermanent { get; set; }
            public string currAddLine1 { get; set; }
            public string currAddLine2 { get; set; }
            public string currAddPostalCode { get; set; }
            public string currAdd_City { get; set; }
            public int currAdd_CountryID { get; set; }
        }

        public class customerCRS
        {
            public int customerID { get; set; }
            public string jurisdictionTaxResidence { get; set; }
            public string tiN_NTN { get; set; }
            public string no_TIN { get; set; }
        }

        public class customerFatca
        {
            public int customerID { get; set; }
            public int countryofResidence { get; set; }
            public string nationalityOtherCountry { get; set; }
            public bool taxResidencyCertification { get; set; }
            public string usResidence { get; set; }
            public string usBorn { get; set; }
            public string uS_Telephone { get; set; }
            public string uS_Address { get; set; }
            public string uS_Info { get; set; }
            //public int otherCountryofResidence { get; set; }
            public string ssNumber { get; set; }
            public string employerTaxNumber { get; set; }
        }

        public class CustomerFundProvider
        {
            public int customerID { get; set; }

            public string name { get; set; }

            public string iD_No { get; set; }

            public string dob { get; set; }
        }

        public class customerOfficeAddress
        {
            public int customerID { get; set; }

            public string officeAddLine1 { get; set; }
            public string officeAddLine2 { get; set; }

            public string officeAdd_City { get; set; }

            public int officeAdd_CountryID { get; set; }
        }

        public class OccupationDetails
        {
            public int customerID { get; set; }

            public int occupationID { get; set; }

            public int sourceOfFundsID { get; set; }

            public string stage { get; set; }

            public string purposeOfAccount { get; set; }

            public int expected_Monthly_Credit_TRN { get; set; }

            public string avgMonthlySalary { get; set; }


            public string dominant_Mode_Deposit { get; set; }

            public string dominant_Mode_Withdrawal { get; set; }

            public string political_figure { get; set; }

            public string relPolitical { get; set; }

            public string otherSourceFund { get; set; }

            public string employerName { get; set; }

            public string dependentOn { get; set; }
            public customerOfficeAddress customerOfficeAddress { get; set; }
            public CustomerFundProvider CustomerFundProvider { get; set; }
        }

        public class BankingServices
        {
            public int customerID { get; set; }
            public int branchID { get; set; }
            public int branchCity { get; set; }
            public string currency { get; set; }
            public string stage { get; set; }

            public customerProduct customerProduct { get; set; }
            public customerAddressType customerAddressType { get; set; }
        }

        public class customerProduct
        {
            public int customerID { get; set; }
            public string debitCard { get; set; }
            public string name_On_Card { get; set; }
            public string card_Type { get; set; }
            public string chequeBook { get; set; }
            public string payPak_Reason { get; set; }
        }

        public class customerAddressType
        {
            public int preferredAddressTypeID { get; set; }
        }

        public class DocumentUpload
        {
            public int customerID { get; set; }

            public string Stage { get; set; }

            public string idFrontImage { get; set; }

            public string cniC_F_File_Name { get; set; }

            public string cniC_F_File_ContentType { get; set; }

            public string idBackImage { get; set; }

            public string cniC_B_File_Name { get; set; }

            public string cniC_B_File_ContentType { get; set; }

            public string selfieImage { get; set; }

            public string selfie_File_Name { get; set; }

            public string selfie_File_ContentType { get; set; }

            public string digitalSignatureImage { get; set; }

            public string signature_File_Name { get; set; }

            public string signature_File_ContentType { get; set; }

            public string proofOfIncomeImage { get; set; }

            public string proofIncome_File_Name { get; set; }

            public string proofIncome_File_ContentType { get; set; }

            public string zakatDeclaration { get; set; }

            public string zakatFileName { get; set; }

            public string zakatFileContent { get; set; }
        }

        public class CustomerProfile
        {
            public int customerID { get; set; }
            public string customerName { get; set; }
            public string mobileNumber { get; set; }
            public int occupationID { get; set; }
            public int branchID { get; set; }
            public int sourceOfFundsID { get; set; }
            public int maritalStatusID { get; set; }
            public int idType { get; set; }
            public int educationID { get; set; }
            public string customerIDNo { get; set; }
            public int productID { get; set; }
            public string fatherName { get; set; }
            public string motherOrMaidenName { get; set; }
            public DateTime idIssueDate { get; set; }
            public string idFrontImage { get; set; }
            public string idBackImage { get; set; }
            public string selfieImage { get; set; }
            public string digitalSignatureImage { get; set; }
            public string fundProvider { get; set; }
            public string remittingAccountNumber { get; set; }
            public string existingBAFlAccountNumber { get; set; }
            public string annualSalary { get; set; }
            public string avgMonthlySalary { get; set; }
            public bool isConsentProvided { get; set; }
            public string proofOfIncomeImage { get; set; }
            public bool isZakatExzemption { get; set; }
            public int frequencyEStatementID { get; set; }
            public string systemGeneratedID { get; set; }
            public int createdBy { get; set; }
            public DateTime createdDate { get; set; }
            public int modifiedBy { get; set; }
            public string modifiedDate { get; set; }
            public bool isActive { get; set; }
            public string emailAddress { get; set; }
            public string passportNumber { get; set; }
            public string cniC_F_File_Name { get; set; }
            public string cniC_F_File_ContentType { get; set; }
            public string cniC_B_File_Name { get; set; }
            public string cniC_B_File_ContentType { get; set; }
            public string selfie_File_Name { get; set; }
            public string selfie_File_ContentType { get; set; }
            public string signature_File_Name { get; set; }
            public string signature_File_ContentType { get; set; }
            public string proofIncome_File_Name { get; set; }
            public string proofIncome_File_ContentType { get; set; }
            public string nrP_File_Name { get; set; }
            public string nrP_File_ContentType { get; set; }
            public string stage { get; set; }
            public string status { get; set; }
            public string reasonForZakatExempt { get; set; }
            public bool isLetterOfThanks { get; set; }
            public string gender { get; set; }
            public string spouseName { get; set; }
            public string dob { get; set; }
            public string cnicExpiryDate { get; set; }
            public int placeOfBirth { get; set; }
            public string accountTitle { get; set; }
            public string currency { get; set; }
            public string accountType { get; set; }
            public string customerType { get; set; }
            public string operatingInstructions { get; set; }
            public int country { get; set; }
            public string otac { get; set; }
            public DateTime otaC_Date { get; set; }
            public bool isOCRRead { get; set; }
            public bool provideDoc_OverEmail { get; set; }
            public bool isAccountGenerated { get; set; }
            public string accountNumber { get; set; }
            public string accountDate { get; set; }
            public bool isAccountActivated { get; set; }
            public string iban { get; set; }
            public string passportFirstName { get; set; }
            public string passportFirstContentType { get; set; }
            public string passportSecondName { get; set; }
            public string passportSecondContentType { get; set; }
            public string hotScanResponse { get; set; }
            public string customerEnqResponse { get; set; }
            public string customerCreationResponse { get; set; }
            public string accountCreationRespnse { get; set; }
            public string ibUser { get; set; }
            public string ibPass { get; set; }
            public string salaryCurrency { get; set; }
            public bool isNextOfKin { get; set; }
            public string opF_Membership { get; set; }
            public string landline_Number { get; set; }
            public string ref_AccountNumber { get; set; }
            public string network { get; set; }
            public string accountCategory { get; set; }
            public bool localCustomer { get; set; }
            public string smsAlert { get; set; }
            public string zakatDeclaration { get; set; }
            public string zakatFileName { get; set; }
            public string zakatFileContent { get; set; }
            public bool isBioVerified { get; set; }
            public string isBioVerified_Datetime { get; set; }
            public string source_OF_BioVerified { get; set; }
            public bool isDebit { get; set; }
            public string expected_monthly_turnover { get; set; }
            public string nameBusiness { get; set; }
            public string natureBusiness { get; set; }
            public string employerName { get; set; }
            public string position { get; set; }
            public string employedsince { get; set; }
            public string natureType { get; set; }
            public string embossingName { get; set; }
            public string dependentOn { get; set; }
            public bool iB_UserStatus { get; set; }
            public string otherBusiness { get; set; }
            public bool is_Declaration_Read { get; set; }
            public string sanction_Country { get; set; }
            public string purposeOfAccount { get; set; }
            public string account_turn_over_Per_Annum { get; set; }
            public string amount_10M { get; set; }
            public string expected_Monthly_Debit_TRN { get; set; }
            public int expected_Monthly_Credit_TRN { get; set; }
            public string expected_Monthly_Debit_TurnOver { get; set; }
            public string debit_10M_Amount { get; set; }
            public string dominant_Mode_Deposit { get; set; }
            public string dominant_Mode_Withdrawal { get; set; }
            public string local_Geographies_TRN { get; set; }
            public int int_Geographies_TRN { get; set; }
            public string counter_Parties { get; set; }
            public string political_figure { get; set; }
            public bool is_Fund_Provider { get; set; }
            public string cityOfBirth { get; set; }
            public string salary_Other_Income { get; set; }
            public string expected_Monthly_Credit_TurnOver { get; set; }
            public string credit_10M_Amount { get; set; }
            public string relPolitical { get; set; }
            public string status_employment { get; set; }
            public string otherSourceFund { get; set; }
            public string investmentBusiness { get; set; }
            public string businessTurnover { get; set; }
            public string status_of_Ownership { get; set; }
            public int branchCity { get; set; }
            public string ipAddress { get; set; }
            public string accountStatus_Code { get; set; }
            public string accountStatus_Desc { get; set; }
            public string e_Status { get; set; }
            public string e_message { get; set; }
            public string e_Date { get; set; }
            public string comments { get; set; }
            public bool chequeBook { get; set; }
            public int is_Mobile { get; set; }
            public int incmSrclevel { get; set; }
            public int creditTurnOver { get; set; }
            public bool discrepent_Mark { get; set; }
            public bool highRiskCustomer { get; set; }
            public bool callback { get; set; }
            public string discrepentDate { get; set; }
            public string discrepentByBO_User { get; set; }
            public string highRiskByBO_User { get; set; }
            public string highRiskDate { get; set; }
            public string callBackByBO_User { get; set; }
            public string callBackkDate { get; set; }
            public string cB_Status { get; set; }
            public bool otaC_Verified { get; set; }
            public string rectificationDate { get; set; }
            public string request_initiate { get; set; }
            public string ref_Code { get; set; }
            public string rM_Code { get; set; }
            public string highRisk_DocName { get; set; }
            public string highRisk_Doc_ContentType { get; set; }
            public string birtH_INCORP_DATE { get; set; }
            public string sole_CIF_Details { get; set; }
            public string sole_CIF_Selection { get; set; }
            public string bio_Email { get; set; }
            public string bio_Email_Date { get; set; }
            public int bioIntimation { get; set; }
            public string ntn { get; set; }
            public bool premier { get; set; }
            public bool staff { get; set; }
            public bool signatureDiff { get; set; }
            public bool signatureDeclaration { get; set; }
            public string remitterName { get; set; }
            public int remitterRelationShip { get; set; }
            public bool selfDeclaration { get; set; }
        }
        public class branchlist
        {
            public int  branchID { get; set; }
            public String branchCode { get; set; }
            public String branchName { get; set; }
            public String branchAddress { get; set; }
            public String cityID { get; set; }
            public String accountType { get; set; }
            public String fcY_Status { get; set; }
        }
        
        public class countryList
        {
            public int countryID { get; set; }
            
            public string countryName { get; set; }
        }

        public class productType
        {
            public int productID { get; set; }

            public string productCode { get; set; }

            public string productName { get; set; }

            public string accountType { get; set; }

            public string card { get; set; }

            public string currency { get; set; }

            public string accountCategory { get; set; }
        }

        
    }
}
