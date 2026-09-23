using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem.Bank
{
    public partial class Dashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 3)
            { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            SQLiteParameter[] p = { new SQLiteParameter("@BusinessId", businessId) };

            lblTotalTokens.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime')", p)?.ToString() ?? "0";
            lblWaiting.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'Waiting'", p)?.ToString() ?? "0";
            lblCompleted.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'Completed'", p)?.ToString() ?? "0";
            lblActiveCounters.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM ServiceCounters WHERE BusinessId = @BusinessId AND IsActive = 1", p)?.ToString() ?? "0";

            DataTable dt = DBHelper.ExecuteReader("SELECT CounterId, CounterName, IsActive FROM ServiceCounters WHERE BusinessId = @BusinessId ORDER BY CounterName", p);
            rptCounters.DataSource = dt;
            rptCounters.DataBind();
        }

        protected string GetCurrentTokenForCounter(object counterIdObj)
        {
            if (counterIdObj == null || counterIdObj == DBNull.Value) return "---";
            int counterId = Convert.ToInt32(counterIdObj);
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            SQLiteParameter[] parameters = { new SQLiteParameter("@BusinessId", businessId), new SQLiteParameter("@CounterId", counterId) };
            object result = DBHelper.ExecuteScalar(
                "SELECT TokenNumber FROM Tokens WHERE BusinessId = @BusinessId AND CounterId = @CounterId AND CurrentStatus = 'At Counter' ORDER BY CreatedDate DESC LIMIT 1",
                parameters);
            return result != null ? result.ToString() : "---";
        }
    }
}
