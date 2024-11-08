using UnityEngine;
public class MRScreen : MonoBehaviour
{
	public MRScreenName screenName;
	public bool fullScreen;
	public bool firstScreen;
}

public enum MRScreenName
{
	Login,
	MainDashboard,
	Companies_List,
	Companies_View_Add,
	Confirmation,
	Categories_List,
	Categories_View_Add,
	Units_List,
	Units_View_Add,
	Contacts_List,
	Contacts_View_Add,
	Products_List,
	Products_Add,
	Products_View,
	ProductStock_View_Add,
	Purchases_List,
	Purchases_View_Add,
	Accounts_List,
	Accounts_View_Add,
	Bookings_List,
	Bookings_View_Add,
	Sale_View_Add,
	Sales_List,
	ProfitReport,
	ReceiveSalePayment,
	AccountStatement,
	ViewSalePayments,
	StockBook,
	StockReport,
	Expenses_List,
	Expenses_View_Add,
	Recoveries_List,
	Recoveries_View_Add,
	Transfers_List,
	Transfers_View_Add,
	PreLoader,
	Loans_List,
	Loans_View_Add,
	Incentives_List,
	Incentives_View_Add,
	Screen_Sales_ReturnItems,
	Partners_List,
	PartnersCapital_Add,
	PartnersProfit_Add,
	ProductStock_Return,
	ManualStock_Return,
	SaleReturns_List
}
