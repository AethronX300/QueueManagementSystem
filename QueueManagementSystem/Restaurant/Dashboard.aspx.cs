using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem.Restaurant
{
    public partial class Dashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 1)
            { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            SQLiteParameter[] p = { new SQLiteParameter("@BusinessId", businessId) };

            lblTotalTokens.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime')", p)?.ToString() ?? "0";
            lblActiveOrders.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus NOT IN ('Payment Completed')", p)?.ToString() ?? "0";
            lblCompleted.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'Payment Completed'", p)?.ToString() ?? "0";
            lblPaymentPending.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'Served' AND PaymentStatus = 'Pending'", p)?.ToString() ?? "0";

            DataTable dt = DBHelper.ExecuteReader(
                "SELECT TokenId, TokenNumber, CustomerName, TableNumber, CurrentStatus, CreatedDate FROM Tokens WHERE BusinessId = @BusinessId AND CurrentStatus != 'Payment Completed' ORDER BY CreatedDate DESC", p);

            if (dt.Rows.Count > 0) { rptActiveOrders.DataSource = dt; rptActiveOrders.DataBind(); pnlNoOrders.Visible = false; }
            else { rptActiveOrders.Visible = false; pnlNoOrders.Visible = true; }
        }

        protected string GetStatusClass(string status)
        {
            switch (status)
            {
                case "Order Placed": return "waiting";
                case "Preparing": return "preparing";
                case "Ready to Serve": return "ready";
                case "Served": return "active";
                case "Payment Completed": return "completed";
                default: return "waiting";
            }
        }
    }
}
