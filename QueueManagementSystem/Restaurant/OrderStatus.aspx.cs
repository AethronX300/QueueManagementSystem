using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueueManagementSystem.Restaurant
{
    public partial class OrderStatus : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 1)
            { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) LoadOrders();
        }

        private void LoadOrders()
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            DataTable dt = DBHelper.ExecuteReader(
                @"SELECT TokenId, TokenNumber, CustomerName, TableNumber, CurrentStatus, PaymentStatus, CreatedDate 
                  FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus != 'Payment Completed'
                  ORDER BY CASE CurrentStatus WHEN 'Order Placed' THEN 1 WHEN 'Preparing' THEN 2 WHEN 'Ready to Serve' THEN 3 WHEN 'Served' THEN 4 END, CreatedDate ASC",
                new SQLiteParameter[] { new SQLiteParameter("@BusinessId", businessId) });

            if (dt.Rows.Count > 0) { rptOrders.DataSource = dt; rptOrders.DataBind(); rptOrders.Visible = true; pnlEmpty.Visible = false; }
            else { rptOrders.Visible = false; pnlEmpty.Visible = true; }
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "UpdateStatus")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int tokenId = Convert.ToInt32(args[0]);
                string newStatus = args[1];
                int userId = Convert.ToInt32(Session["UserId"]);

                string updateQuery = "UPDATE Tokens SET CurrentStatus = @Status";
                if (newStatus == "Payment Completed") updateQuery += ", PaymentStatus = 'Paid', CompletedDate = datetime('now','localtime')";
                updateQuery += " WHERE TokenId = @TokenId";

                DBHelper.ExecuteNonQuery(updateQuery, new SQLiteParameter[] { new SQLiteParameter("@Status", newStatus), new SQLiteParameter("@TokenId", tokenId) });
                DBHelper.ExecuteNonQuery("INSERT INTO TokenStatusLog (TokenId, Status, ChangedBy) VALUES (@TokenId, @Status, @ChangedBy)",
                    new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId), new SQLiteParameter("@Status", newStatus), new SQLiteParameter("@ChangedBy", userId) });
                LoadOrders();
            }
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
