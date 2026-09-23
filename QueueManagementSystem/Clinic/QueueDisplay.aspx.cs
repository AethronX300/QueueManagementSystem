using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem.Clinic
{
    public partial class QueueDisplay : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 2)
            { Response.Redirect("~/Default.aspx"); return; }

            // Handle per-patient inline action links: ?action=call&tid=5
            string action = Request.QueryString["action"];
            string tidStr = Request.QueryString["tid"];
            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(tidStr))
            {
                int tokenId = Convert.ToInt32(tidStr);
                int userId = Convert.ToInt32(Session["UserId"]);
                string newStatus = action == "call" ? "In Consultation"
                                 : action == "done" ? "Consultation Done"
                                 : action == "complete" ? "Completed"
                                 : null;
                if (newStatus != null)
                {
                    UpdateStatus(tokenId, newStatus, userId);
                    if (newStatus == "Completed")
                        DBHelper.ExecuteNonQuery("UPDATE Tokens SET CompletedDate = datetime('now','localtime') WHERE TokenId = @TokenId",
                            new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId) });
                }
                Response.Redirect("QueueDisplay.aspx"); return;
            }

            if (!IsPostBack) LoadQueueData();
        }

        private void LoadQueueData()
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            SQLiteParameter[] p = { new SQLiteParameter("@BusinessId", businessId) };

            DataTable dtNow = DBHelper.ExecuteReader("SELECT TokenNumber, CustomerName FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'In Consultation' ORDER BY CreatedDate ASC LIMIT 1", p);
            if (dtNow.Rows.Count > 0) { lblNowServing.Text = dtNow.Rows[0]["TokenNumber"].ToString(); lblNowServingName.Text = dtNow.Rows[0]["CustomerName"].ToString(); }
            else { lblNowServing.Text = "---"; lblNowServingName.Text = "No patient in consultation"; }

            DataTable dt = DBHelper.ExecuteReader(
                @"SELECT TokenId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate FROM Tokens 
                  WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime')
                  ORDER BY CASE CurrentStatus WHEN 'In Consultation' THEN 1 WHEN 'Waiting' THEN 2 WHEN 'Consultation Done' THEN 3 WHEN 'Payment Completed' THEN 4 END, CreatedDate ASC", p);

            if (dt.Rows.Count > 0) { rptPatients.DataSource = dt; rptPatients.DataBind(); rptPatients.Visible = true; pnlEmpty.Visible = false; }
            else { rptPatients.Visible = false; pnlEmpty.Visible = true; }
        }

        protected void btnCallNext_Click(object sender, EventArgs e)
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            int userId = Convert.ToInt32(Session["UserId"]);
            SQLiteParameter[] p = { new SQLiteParameter("@BusinessId", businessId) };

            int inConsultation = Convert.ToInt32(DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'In Consultation'", p));
            if (inConsultation > 0) { lblMessage.Text = "Please complete the current consultation first."; lblMessage.CssClass = "msg-error"; lblMessage.Visible = true; LoadQueueData(); return; }

            object result = DBHelper.ExecuteScalar("SELECT TokenId FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'Waiting' ORDER BY CreatedDate ASC LIMIT 1", p);
            if (result != null) { UpdateStatus(Convert.ToInt32(result), "In Consultation", userId); lblMessage.Text = "Next patient called!"; lblMessage.CssClass = "msg-success"; }
            else { lblMessage.Text = "No patients waiting."; lblMessage.CssClass = "msg-error"; }
            lblMessage.Visible = true; LoadQueueData();
        }

        protected void btnCompleteConsultation_Click(object sender, EventArgs e)
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            int userId = Convert.ToInt32(Session["UserId"]);
            object result = DBHelper.ExecuteScalar("SELECT TokenId FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'In Consultation' ORDER BY CreatedDate ASC LIMIT 1", new SQLiteParameter[] { new SQLiteParameter("@BusinessId", businessId) });
            if (result != null) { UpdateStatus(Convert.ToInt32(result), "Consultation Done", userId); lblMessage.Text = "Consultation completed!"; lblMessage.CssClass = "msg-success"; }
            else { lblMessage.Text = "No patient in consultation."; lblMessage.CssClass = "msg-error"; }
            lblMessage.Visible = true; LoadQueueData();
        }

        protected void btnPaymentDone_Click(object sender, EventArgs e)
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            int userId = Convert.ToInt32(Session["UserId"]);
            object result = DBHelper.ExecuteScalar("SELECT TokenId FROM Tokens WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') AND CurrentStatus = 'Consultation Done' ORDER BY CreatedDate ASC LIMIT 1", new SQLiteParameter[] { new SQLiteParameter("@BusinessId", businessId) });
            if (result != null)
            {
                int tokenId = Convert.ToInt32(result);
                DBHelper.ExecuteNonQuery("UPDATE Tokens SET CurrentStatus = 'Payment Completed', PaymentStatus = 'Paid', CompletedDate = datetime('now','localtime') WHERE TokenId = @TokenId", new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId) });
                DBHelper.ExecuteNonQuery("INSERT INTO TokenStatusLog (TokenId, Status, ChangedBy) VALUES (@TokenId, 'Payment Completed', @ChangedBy)", new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId), new SQLiteParameter("@ChangedBy", userId) });
                lblMessage.Text = "Payment completed!"; lblMessage.CssClass = "msg-success";
            }
            else { lblMessage.Text = "No patient pending payment."; lblMessage.CssClass = "msg-error"; }
            lblMessage.Visible = true; LoadQueueData();
        }

        private void UpdateStatus(int tokenId, string status, int userId)
        {
            DBHelper.ExecuteNonQuery("UPDATE Tokens SET CurrentStatus = @Status WHERE TokenId = @TokenId", new SQLiteParameter[] { new SQLiteParameter("@Status", status), new SQLiteParameter("@TokenId", tokenId) });
            DBHelper.ExecuteNonQuery("INSERT INTO TokenStatusLog (TokenId, Status, ChangedBy) VALUES (@TokenId, @Status, @ChangedBy)", new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId), new SQLiteParameter("@Status", status), new SQLiteParameter("@ChangedBy", userId) });
        }

        /// <summary>Returns inline HTML action buttons for each patient card based on their current status.</summary>
        protected string GetActionButtons(int tokenId, string status)
        {
            switch (status)
            {
                case "Waiting":
                    return $"<div class='token-item-actions'>"
                         + $"<a href='QueueDisplay.aspx?action=call&tid={tokenId}' class='btn btn-success btn-action' style='flex:1;'>In Consultation</a>"
                         + "</div>";
                case "In Consultation":
                    return $"<div class='token-item-actions'>"
                         + $"<a href='QueueDisplay.aspx?action=done&tid={tokenId}' class='btn btn-info btn-action' style='flex:1;'>Consultation Done</a>"
                         + "</div>";
                case "Consultation Done":
                    return $"<div class='token-item-actions'>"
                         + $"<a href='QueueDisplay.aspx?action=complete&tid={tokenId}' class='btn btn-primary btn-action' style='flex:1;'>Mark Completed</a>"
                         + "</div>";
                default:
                    return "";
            }
        }

        protected string GetStatusClass(string status)
        {
            switch (status)
            {
                case "Waiting": return "waiting";
                case "In Consultation": return "active";
                case "Consultation Done": return "ready";
                case "Payment Completed": return "completed";
                default: return "waiting";
            }
        }
    }
}
