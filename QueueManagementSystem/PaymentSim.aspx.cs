using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem
{
    public partial class PaymentSim : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) LoadPendingTokens();
        }

        private void LoadPendingTokens()
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            string query = @"SELECT TokenId, TokenNumber || ' - ' || CustomerName AS DisplayText FROM Tokens 
                           WHERE BusinessId = @BusinessId AND PaymentStatus = 'Pending' AND date(CreatedDate) = date('now','localtime') ORDER BY CreatedDate ASC";
            DataTable dt = DBHelper.ExecuteReader(query, new SQLiteParameter[] { new SQLiteParameter("@BusinessId", businessId) });
            ddlPendingTokens.Items.Clear();
            ddlPendingTokens.Items.Add(new System.Web.UI.WebControls.ListItem("-- Select Token --", "0"));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                    ddlPendingTokens.Items.Add(new System.Web.UI.WebControls.ListItem(row["DisplayText"].ToString(), row["TokenId"].ToString()));
                pnlNoPending.Visible = false;
            }
            else { pnlPayment.Visible = false; pnlNoPending.Visible = true; }
        }

        private string GetLocalIpBaseUrl()
        {
            string publicUrl = System.Configuration.ConfigurationManager.AppSettings["PublicBaseUrl"];
            if (!string.IsNullOrWhiteSpace(publicUrl))
                return publicUrl.TrimEnd('/');
            return Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath.TrimEnd('/');
        }

        // Generates QR that points to TrackStatus.aspx — same as NewToken pages
        private void SetQrCode(string tokenNumber)
        {
            string baseUrl = GetLocalIpBaseUrl();
            string trackUrl = baseUrl + "/TrackStatus.aspx?token=" + Uri.EscapeDataString(tokenNumber);

            // QR image (same api as NewToken pages — works reliably)
            imgQrCode.ImageUrl = "https://api.qrserver.com/v1/create-qr-code/?size=200x200&color=000000&bgcolor=ffffff&data="
                                 + Uri.EscapeDataString(trackUrl);

            // Also show the direct clickable link
            lnkTrackUrl.Text = trackUrl;
            lnkTrackUrl.NavigateUrl = trackUrl;
        }

        protected void ddlPendingTokens_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPendingTokens.SelectedValue != "0")
            {
                int tokenId = Convert.ToInt32(ddlPendingTokens.SelectedValue);
                DataTable dt = DBHelper.ExecuteReader("SELECT TokenNumber, CustomerName, PaymentStatus FROM Tokens WHERE TokenId = @TokenId",
                    new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId) });
                if (dt.Rows.Count > 0)
                {
                    string tokenNumber = dt.Rows[0]["TokenNumber"].ToString();
                    lblPayToken.Text = tokenNumber;
                    lblPayCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                    SetQrCode(tokenNumber);
                    if (dt.Rows[0]["PaymentStatus"].ToString() == "Paid")
                    { lblPaymentStatus.Text = "PAID"; lblPaymentStatus.CssClass = "payment-status-display payment-paid"; btnSimulatePayment.Visible = false; }
                    else
                    { lblPaymentStatus.Text = "PENDING"; lblPaymentStatus.CssClass = "payment-status-display payment-pending"; btnSimulatePayment.Visible = true; }
                    pnlPayment.Visible = true;
                }
            }
            else pnlPayment.Visible = false;
        }

        protected void btnSimulatePayment_Click(object sender, EventArgs e)
        {
            if (ddlPendingTokens.SelectedValue != "0")
            {
                int tokenId = Convert.ToInt32(ddlPendingTokens.SelectedValue);
                int userId = Convert.ToInt32(Session["UserId"]);

                // Fetch token number before reloading dropdown
                DataTable dtToken = DBHelper.ExecuteReader("SELECT TokenNumber FROM Tokens WHERE TokenId = @TokenId",
                    new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId) });
                string tokenNumber = dtToken.Rows.Count > 0 ? dtToken.Rows[0]["TokenNumber"].ToString() : "";

                DBHelper.ExecuteNonQuery("UPDATE Tokens SET PaymentStatus = 'Paid' WHERE TokenId = @TokenId",
                    new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId) });
                DBHelper.ExecuteNonQuery("INSERT INTO TokenStatusLog (TokenId, Status, ChangedBy) VALUES (@TokenId, 'Payment Simulated - Paid', @ChangedBy)",
                    new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId), new SQLiteParameter("@ChangedBy", userId) });

                lblPaymentStatus.Text = "PAID";
                lblPaymentStatus.CssClass = "payment-status-display payment-paid";
                btnSimulatePayment.Visible = false;
                lblMessage.Text = "Payment marked as PAID!";
                lblMessage.CssClass = "msg-success";
                lblMessage.Visible = true;

                if (!string.IsNullOrEmpty(tokenNumber))
                    SetQrCode(tokenNumber);

                LoadPendingTokens();
            }
        }
    }
}
