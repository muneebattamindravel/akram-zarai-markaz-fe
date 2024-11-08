using UnityEngine;

public static class Constants
{
    public static string None = "NONE";
    public static string Success = "SUCCESS!";
    public static string Error = "ERROR!";
    public static string PoolError = "POOL LIMIT!";
    public static string Failed = "FAILED!";
    public static string Done = "DONE!";
    public static string Add = "ADD";
    public static string TransferProfit = "TRANSFER PROFIT";
    public static string View = "VIEW";
    public static string Edit = "EDIT";
    public static string Currency = "";
    public static string Percentage = " % ";
    public static string Manual = "MANUAL";
    public static string loggedIn = "LOGGED IN!";
    public static string NotEnoughPermissions = "PERMISSION ERROR!";
    public static string BackupDone = "DATA BACK UP DONE!";
    public static string DataUploaded = "DATA UPLOADED SUCCESSFULLY!";
    public static string FutureDateError = "FUTURE DATE NOT ALLOWED";

    public static Color PositiveColor
    {
        get
        {
            Color color;
            ColorUtility.TryParseHtmlString("#047B00", out color);
            return color;
        }
    }

    public static Color NegativeColor
    {
        get
        {
            Color color;
            ColorUtility.TryParseHtmlString("#C80000", out color);
            return color;
        }
    }

    public static string DateDisplayFormat = "dd-MM-yyyy";
    public static string BackupFileNamePrefix = "dd-MM-yyyy-HH-mm";

    // GENERIC
    public static string YES = "YES";
    public static string NO = "NO";
    public static string DELETE = "DELETE";
    public static string CANCEL = "CANCEL";
    public static string DeleteConfirmation = "DO YOU WANT TO DELETE THIS ";
    public static string ReturnConfirmation = "SURE YOU WANT TO RETURN ITEMS ? ";
    public static string DefaultProductImagePath = "Images/DefaultProductImage";

    // LOGIN
    public static string UsernameEmpty = "USERNAME CAN'T BE EMPTY";
    public static string PasswordEmpty = "PASSWORD CAN'T BE EMPTY";

    // COMPANIES
    public static string Company = "COMPANY";
    public static string CompanyNameEmpty = "COMPANY NAME CAN'T BE EMPTY";
    public static string CompanyAdded = "NEW COMPANY ADDED SUCCESSFULLY";
    public static string CompanyUpdated = "COMPANY UPDATED SUCCESSFULLY";
    public static string CompanyDeleted = "COMPANY DELETED SUCCESSFULLY";

    // CATEGORIES
    public static string Category = "CATEGORY";
    public static string CategoryNameEmpty = "CATEGORY NAME CAN'T BE EMPTY";
    public static string CategoryAdded = "NEW CATEGORY ADDED SUCCESSFULLY";
    public static string CategoryUpdated = "CATEGORY UPDATED SUCCESSFULLY";
    public static string CategoryDeleted = "CATEGORY DELETED SUCCESSFULLY";

    // UNITS
    public static string Unit = "UNIT";
    public static string UnitNameEmpty = "UNIT NAME CAN'T BE EMPTY";
    public static string UnitAdded = "NEW UNIT ADDED SUCCESSFULLY";
    public static string UnitUpdated = "UNIT UPDATED SUCCESSFULLY";
    public static string UnitDeleted = "UNIT DELETED SUCCESSFULLY";

    // CONTACTS
    public static string Contact = "CONTACT";
    public static string ContactNameEmpty = "CONTACT NAME CAN'T BE EMPTY";
    public static string ContactAdded = "NEW CONTACT ADDED SUCCESSFULLY";
    public static string ContactUpdated = "CONTACT UPDATED SUCCESSFULLY";
    public static string ContactDeleted = "CONTACT DELETED SUCCESSFULLY";
    public static string OpeningBalanceEmpty = "PLEASE ENTER OPENING BALANCE FOR CUSTOMER";

    // PRODUCTS
    public static string Product = "PRODUCT";
    public static string ProductNameEmpty = "PRODUCT NAME CAN'T BE EMPTY";
    public static string ProductAdded = "NEW PRODUCT ADDED SUCCESSFULLY";
    public static string ProductUpdated = "PRODUCT UPDATED SUCCESSFULLY";
    public static string ProductDeleted = "PRODUCT DELETED SUCCESSFULLY";
    public static string ProductImage = "ProductImage";

    // PRODUCT STOCKS
    public static string ProductStock = "PRODUCT STOCK";
    public static string ProductStockEmpty = "PRODUCT STOCK NAME CAN'T BE EMPTY";
    public static string ProductStockAdded = "PRODUCT STOCK ADDED SUCCESSFULLY";
    public static string ProductStockUpdated = "PRODUCT STOCK UPDATED SUCCESSFULLY";
    public static string ProductStockDeleted = "PRODUCT STOCK DELETED SUCCESSFULLY";
    public static string CostPriceEmpty = "COST PRICE CAN'T BE EMPTY";
    public static string InitialQuantityEmpty = "INITIAL QUANTITY CAN'T BE EMPTY";
    public static string BatchNumberEmpty = "BATCH NUMBER CAN'T BE EMPTY";
    public static string ExpiryDateEmpty = "ENTER EXPIRY DATE";
    public static string TotalAmountZero = "TOTAL AMOUNT CAN'T BE ZERO";
    public static string StockReturned = "STOCK RETURNED";
    public static string ManualStockReturned = "MANUAL STOCK RETURNED";

    // PURCHASES
    public static string Purchase = "PURCHASE";
    public static string SelectCompanyForPurchase = "SELECT COMPANY FOR PURCHASE";
    public static string PurchaseAdded = "NEW PURCHASE ADDED SUCCESSFULLY";
    public static string PurchaseDeleted = "PURCHASE DELETED SUCCESSFULLY";
    public static string TotalPurchaseAmountZero = "TOTAL PURCHASE AMOUNT CAN'T BE ZERO";
    public static string SelectSupplier = "PLEASE SELECT OR ADD SUPPLIER";
    public static string SelectProduct = "PLEASE SELECT OR ADD PRODUCT";
    public static string InvoiceNumberEmpty = "PLEASE ENTER INVOICE NUMBER";
    public static string InvoiceDateEmpty = "PLEASE SELECT INVOICE DATE";

    // ACCOUNTS
    public static string Account = "ACCOUNT";
    public static string AccountNameEmpty = "ACCOUNT NAME CAN'T BE EMPTY";
    public static string AccountAdded = "NEW ACCOUNT ADDED SUCCESSFULLY";
    public static string AccountUpdated = "ACCOUNT UPDATED SUCCESSFULLY";
    public static string AccountDeleted = "ACCOUNT DELETED SUCCESSFULLY";
    public static string SelectAccountType = "PLEASE SELECT ACCOUNT TYPE";
    public static string OpeningBalanceZero = "PLEASE ENTER OPENING BALANCE";
    public static string SelectCompanyForCompanyTypeAccount = "MUST SELECT A COMPANY FOR COMPANY TYPE ACCOUNT";

    // EXPENSES
    public static string Expense = "EXPENSE";
    public static string ExpenseAmountEmpty = "PLEASE ENTER EXPENSE AMOUNT!";
    public static string EnterExpenseDate = "PLEASE ENTER EXPENSE DATE!";
    public static string ExpenseAdded = "NEW EXPENSE ADDED SUCCESSFULLY";
    public static string ExpenseUpdated = "EXPENSE UPDATED SUCCESSFULLY";
    public static string SelectExpenseType = "SELECT EXPENSE TYPE";

    // TRANSFERS
    public static string Transfer = "TRANSFER";
    public static string NotEnoughBalance = "NOT ENOUGH BALANCE";
    public static string WithdrawCapital = "WITHDRAW CAPITAL";
    public static string Transfer_Normal = "TRANSFER_NORMAL";
    public static string Transfer_Capital_Withdrawn = "CAPITAL_WITHDRAWN";
    public static string NotEnoughInCapitalAccount = "NOT ENOUGH IN CAPITAL ACCOUNT";

    public static string TransferAmountEmpty = "PLEASE ENTER TRANSFER AMOUNT!";
    public static string AmountEmpty = "PLEASE ENTER CORRECT AMOUNT!";
    public static string EnterTransferDate = "PLEASE ENTER TRANSFER DATE!";
    public static string TransferAdded = "NEW TRANSFER ADDED SUCCESSFULLY";
    public static string TransferDeleted = "TRANSFER DELETED SUCCESSFULLY";
    public static string TransferUpdated = "TRANSFER UPDATED SUCCESSFULLY";
    public static string SelectTransferType = "SELECT TRANSFER TYPE";

    // BOOKINGS
    public static string BookingAdded = "NEW BOOKING ADDED SUCCESSFULLY";
    public static string AccountMissing = "ACCOUNT MISSING";
    public static string SelectCompanyForBooking = "SELECT COMPANY FOR BOOKING";
    public static string CreateAccountFirst = "FIRST CREATE AN ACCOUNT FOR ";
    public static string SelectFromAccount = "SELECT A FROM ACCOUNT";
    public static string SelectToAccount = "SELECT A TO ACCOUNT";
    public static string BothAccountsAreSame = "BOTH ACCOUNTS ARE SAME";
    public static string PrNumberEmpty = "PLEASE ENTER PR NUMBER";
    public static string BookingDateEmpty = "PLEASE SELECT BOOKING DATE";
    public static string PolicyNameEmpty = "PLEASE ENTER POLICY NAME";
    public static string PolicyPercentageEmpty = "PLEASE ENTER POLICY PERCENTAGE";
    public static string NetRateEmpty = "PLEASE ENTER NET RATE";
    public static string BookingDeleted = "BOOKING DELETED SUCCESSFULLY";

    // NEW SALE
    public static string Sale = "SALE";
    public static string NewSale = "NEW SALE";
    public static string SaleCreated = "SALE CREATED";
    public static string SaleItemsReturned = "SALE ITEMS RETURNED";
    public static string SalePaymentReceived = "SALE PAYMENT RECEIVED, THANK-YOU!";
    public static string EditSale = "EDIT SALE";
    public static string ViewSale = "VIEW SALE";
    public static string QuantityLessThanZero = "QUANTITY CAN'T BE LESS THAN ZERO";
    public static string NotEnoughStock = "NOT ENOUGH STOCK";
    public static string SalePriceWrong = "SALE PRICE SHOULBE BE GREATER OR EQUAL TO 1.00 PKR";
    public static string EnterBookNumber = "ENTER BOOK NUMBER";
    public static string EnterBillNumber = "ENTER BILL NUMBER";
    public static string EnterReturnPrice = "ENTER RETURN PRICE";
    public static string DiscoutGreater = "DISCOUNT CAN NOT BE MORE THAN TOTAL SALE AMOUNT";
    public static string EnterPaymentReceived = "ENTER PAYMENT RECEIVED";

    public static string EnterReturnAmount = "ENTER AMOUNT TO BE RETURNED";

    public static string WrongReturnAmount = "RETURN AMOUNT CAN'T BE NEGATIVE OR ZERO";
    public static string NothingToReturn = "NO ITEM TO RETURN";
    public static string EnterAmount = "ENTER AMOUNT";
    public static string EnterReturnQuantity = "ENTER RETURN QUANTITY";
    public static string ReturnQuantityLarger = "RETURN PRODUCT QUANTITY CAN NOT BE GREATER THAN REMAINING QUANTITY";
    public static string MustSelectCustomerCreditSale = "MUST SELECT CUSTOMER FOR CREDIT SALE";
    public static string MustSelectCustomer = "PLEASE SELECT CUSTOMER";
    public static string MustSelectCustomerOnlineAccount = "MUST SELECT CUSTOMER FOR BANK TRANSFER SALE";
    public static string MustSelectOnlineAccount = "MUST SELECT ONLINE ACCOUNT";
    public static string AddOnlineBankAccount = "ADD ONLINE BANK ACCOUNT FIRST";
    public static string ReceivedDateEmtpy = "ENTER PAYMENT RECEIVED DATE";
    public static string DateEmpty = "ENTER DATE";
    public static string ReceivedAmountGreater = "RECEIVED AMOUNT CAN NOT BE GREATER THAN NET SALE AMOUNT";
    public static string SaleDeleted = "SALE DELETED SUCCESSFULLY";

    // ACCOUNT TRANSACTIONS
    public static string ACCOUNT_CREATED = "ACCOUNT_CREATED";
    public static string SALE = "SALE";
    public static string SALE_PAYMENT = "SALE_PAYMENT";
    public static string BOOKING = "BOOKING";

    // RECOVERIES
    public static string Recovery = "RECOVERY";
    public static string RecoveryAdded = "RECOVERY ADDED";
    public static string ReceivedAmountGreaterThanCustomerRemaining = "RECEIVED AMOUNT GREATER THAN REMAINING AMOUNT";
    public static string RecoveryDeleted = "RECOVERY DELETED SUCCESSFULLY";
    public static string TakeRecovery = "TAKE RECOVERY";
    public static string GiveRecovery = "GIVE RECOVERY";    

    // LOANS
    public static string Loan = "LOAN";
    public static string LoanAdded = "LOAN ADDED";
    public static string LoanDeleted = "LOAN DELETED SUCCESSFULLY";
    public static string TakeLoan = "TAKE LOAN";
    public static string GiveLoan = "GIVE LOAN";

    // INCENTIVES
    public static string Incentive = "INCENTIVE";
    public static string IncentiveAdded = "INCENTIVE ADDED";
    public static string IncentiveDeleted = "INCENTIVE DELETED SUCCESSFULLY";


    // PARTNERS
    public static string SelectParnerAccount = "SELECT A PARTNER ACCOUNT";
    public static string SelectCreditAccount = "SELECT A CREDIT ACCOUNT";
    public static string CapitalAdded = "CAPITAL ADDED";
    public static string CapitalWithdrawn = "CAPITAL WITHDRAWN";
    public static string ProfitPosted = "PROFIT POSTED";
}