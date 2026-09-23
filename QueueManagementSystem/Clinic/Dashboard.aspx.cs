using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem.Clinic
{
    public partial class Dashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 2)
            { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            SQLiteParameter[] p = { new SQLiteParameter("@BusinessId", businessId) };

            DataTable dtNow = DBHelper.ExecuteReader("SELECT TokenNumber, CustomerName FROM Tokens WHERE BusinessId = @BusinessId AND CurrentStatus = 'In Consultation' ORDER BY CreatedDate ASC LIMIT 1", p);
            if (dtNow.Rows.Count > 0) { lblNowServing.Text = dtNow.Rows[0]["TokenNumber"].ToString(); lblNowServingName.Text = dtNow.Rows[0]["CustomerName"].ToString(); }

            lblTotalTokens.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime')", p)?.ToString() ?? "0";
            lblWaiting.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'Waiting'", p)?.ToString() ?? "0";
            lblCompleted.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'Payment Completed'", p)?.ToString() ?? "0";
            lblInConsultation.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'In Consultation'", p)?.ToString() ?? "0";

            DataTable dtWait = DBHelper.ExecuteReader("SELECT TokenNumber, CustomerName, CreatedDate FROM Tokens WHERE BusinessId = @BusinessId AND CurrentStatus = 'Waiting' ORDER BY CreatedDate ASC", p);
            if (dtWait.Rows.Count > 0) { rptWaiting.DataSource = dtWait; rptWaiting.DataBind(); pnlNoWaiting.Visible = false; }
            else { pnlNoWaiting.Visible = true; }
        }
    }
}
