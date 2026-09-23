using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueueManagementSystem.Bank
{
    public partial class QueueDisplay : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 3)
            { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) LoadQueueData();
        }

        private void LoadQueueData()
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            SQLiteParameter[] p = { new SQLiteParameter("@BusinessId", businessId) };

            DataTable dtCounters = DBHelper.ExecuteReader("SELECT CounterId, CounterName FROM ServiceCounters WHERE BusinessId = @BusinessId AND IsActive = 1 ORDER BY CounterName", p);
            rptCounters.DataSource = dtCounters; rptCounters.DataBind();

            DataTable dtWaiting = DBHelper.ExecuteReader(
                @"SELECT t.TokenNumber, t.CustomerName, t.ServiceType, IFNULL(sc.CounterName,'Unassigned') AS CounterName 
                  FROM Tokens t LEFT JOIN ServiceCounters sc ON t.CounterId = sc.CounterId 
                  WHERE t.BusinessId = @BusinessId AND date(t.CreatedDate) = date('now','localtime') AND t.CurrentStatus = 'Waiting' ORDER BY t.CreatedDate ASC", p);

            if (dtWaiting.Rows.Count > 0) { rptWaitingQueue.DataSource = dtWaiting; rptWaitingQueue.DataBind(); rptWaitingQueue.Visible = true; pnlNoWaiting.Visible = false; }
            else { rptWaitingQueue.Visible = false; pnlNoWaiting.Visible = true; }
        }

        protected void rptCounters_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int counterId = Convert.ToInt32(e.CommandArgument);
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            int userId = Convert.ToInt32(Session["UserId"]);
            SQLiteParameter[] cp = { new SQLiteParameter("@BusinessId", businessId), new SQLiteParameter("@CounterId", counterId) };

            if (e.CommandName == "CallNext")
            {
                int atCounter = Convert.ToInt32(DBHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND CounterId = @CounterId AND CurrentStatus = 'At Counter' AND date(CreatedDate) = date('now','localtime')", cp));
                if (atCounter > 0) { lblMessage.Text = "Complete current customer first."; lblMessage.CssClass = "msg-error"; lblMessage.Visible = true; LoadQueueData(); return; }

                object result = DBHelper.ExecuteScalar(
                    "SELECT TokenId FROM Tokens WHERE BusinessId = @BusinessId AND CounterId = @CounterId AND CurrentStatus = 'Waiting' AND date(CreatedDate) = date('now','localtime') ORDER BY CreatedDate ASC LIMIT 1", cp);
                if (result != null) { UpdateStatus(Convert.ToInt32(result), "At Counter", userId); lblMessage.Text = "Next customer called!"; lblMessage.CssClass = "msg-success"; }
                else { lblMessage.Text = "No customers waiting for this counter."; lblMessage.CssClass = "msg-error"; }
            }
            else if (e.CommandName == "Complete")
            {
                object result = DBHelper.ExecuteScalar(
                    "SELECT TokenId FROM Tokens WHERE BusinessId = @BusinessId AND CounterId = @CounterId AND CurrentStatus = 'At Counter' AND date(CreatedDate) = date('now','localtime') ORDER BY CreatedDate ASC LIMIT 1", cp);
                if (result != null)
                {
                    int tokenId = Convert.ToInt32(result);
                    DBHelper.ExecuteNonQuery("UPDATE Tokens SET CurrentStatus = 'Completed', CompletedDate = datetime('now','localtime') WHERE TokenId = @TokenId", new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId) });
                    DBHelper.ExecuteNonQuery("INSERT INTO TokenStatusLog (TokenId, Status, ChangedBy) VALUES (@TokenId, 'Completed', @ChangedBy)", new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId), new SQLiteParameter("@ChangedBy", userId) });
                    lblMessage.Text = "Service completed!"; lblMessage.CssClass = "msg-success";
                }
                else { lblMessage.Text = "No customer at this counter."; lblMessage.CssClass = "msg-error"; }
            }
            lblMessage.Visible = true;
            LoadQueueData();
        }

        private void UpdateStatus(int tokenId, string status, int userId)
        {
            DBHelper.ExecuteNonQuery("UPDATE Tokens SET CurrentStatus = @Status WHERE TokenId = @TokenId", new SQLiteParameter[] { new SQLiteParameter("@Status", status), new SQLiteParameter("@TokenId", tokenId) });
            DBHelper.ExecuteNonQuery("INSERT INTO TokenStatusLog (TokenId, Status, ChangedBy) VALUES (@TokenId, @Status, @ChangedBy)", new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId), new SQLiteParameter("@Status", status), new SQLiteParameter("@ChangedBy", userId) });
        }

        // Remove date filter for 'At Counter' - it must always show the token currently being served
        protected string GetCurrentToken(object cid)
        {
            if (cid == null) return "---";
            object r = DBHelper.ExecuteScalar(
                "SELECT TokenNumber FROM Tokens WHERE BusinessId = @b AND CounterId = @c AND CurrentStatus = 'At Counter' ORDER BY CreatedDate DESC LIMIT 1",
                new SQLiteParameter[] { new SQLiteParameter("@b", Convert.ToInt32(Session["BusinessId"])), new SQLiteParameter("@c", Convert.ToInt32(cid)) });
            return r != null ? r.ToString() : "---";
        }

        protected string GetCurrentCustomer(object cid)
        {
            if (cid == null) return "";
            object r = DBHelper.ExecuteScalar(
                "SELECT CustomerName FROM Tokens WHERE BusinessId = @b AND CounterId = @c AND CurrentStatus = 'At Counter' ORDER BY CreatedDate DESC LIMIT 1",
                new SQLiteParameter[] { new SQLiteParameter("@b", Convert.ToInt32(Session["BusinessId"])), new SQLiteParameter("@c", Convert.ToInt32(cid)) });
            return r != null ? r.ToString() : "No customer";
        }

        // Keep date filter only for Waiting count (today's queue)
        protected string GetWaitingCount(object cid)
        {
            if (cid == null) return "0";
            object r = DBHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Tokens WHERE BusinessId = @b AND CounterId = @c AND CurrentStatus = 'Waiting' AND date(CreatedDate) = date('now','localtime')",
                new SQLiteParameter[] { new SQLiteParameter("@b", Convert.ToInt32(Session["BusinessId"])), new SQLiteParameter("@c", Convert.ToInt32(cid)) });
            return r?.ToString() ?? "0";
        }
    }
}
