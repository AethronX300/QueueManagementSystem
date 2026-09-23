using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem.Bank
{
    public partial class NewToken : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 3)
            { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) LoadCounters();
        }

        private void LoadCounters()
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            DataTable dt = DBHelper.ExecuteReader(
                "SELECT CounterId, CounterName FROM ServiceCounters WHERE BusinessId = @BusinessId AND IsActive = 1 ORDER BY CounterName",
                new SQLiteParameter[] { new SQLiteParameter("@BusinessId", businessId) });
            ddlCounter.Items.Clear();
            ddlCounter.Items.Add(new System.Web.UI.WebControls.ListItem("-- Auto Assign (Least Busy) --", "0"));
            foreach (DataRow row in dt.Rows)
                ddlCounter.Items.Add(new System.Web.UI.WebControls.ListItem(row["CounterName"].ToString(), row["CounterId"].ToString()));
        }

        protected void btnGenerateToken_Click(object sender, EventArgs e)
        {
            try
            {
                int businessId = Convert.ToInt32(Session["BusinessId"]);
                int userId = Convert.ToInt32(Session["UserId"]);

                // Token format: B{nn} e.g. B006 = 6th customer today
                string tokenNumber = DBHelper.GetNextTokenNumber(businessId, "B");

                // Counter assignment
                int? counterId = null;
                if (ddlCounter.SelectedValue != "0")
                    counterId = Convert.ToInt32(ddlCounter.SelectedValue);
                else
                {
                    // Auto-assign: find counter with fewest active tokens today
                    string autoQuery = @"SELECT sc.CounterId FROM ServiceCounters sc
                        LEFT JOIN (SELECT CounterId, COUNT(*) AS TokenCount FROM Tokens 
                                   WHERE BusinessId = @BusinessId AND date(CreatedDate) = date('now','localtime') 
                                   AND CurrentStatus IN ('Waiting','At Counter') GROUP BY CounterId) t ON sc.CounterId = t.CounterId
                        WHERE sc.BusinessId = @BusinessId AND sc.IsActive = 1
                        ORDER BY IFNULL(t.TokenCount, 0) ASC LIMIT 1";
                    object autoResult = DBHelper.ExecuteScalar(autoQuery, new SQLiteParameter[] { new SQLiteParameter("@BusinessId", businessId) });
                    if (autoResult != null) counterId = Convert.ToInt32(autoResult);
                }

                DBHelper.ExecuteNonQuery(
                    @"INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus) 
                      VALUES (@BusinessId, @TokenNumber, @CustomerName, @ContactNumber, @ServiceType, @CounterId, 'Waiting', 'N/A')",
                    new SQLiteParameter[] {
                        new SQLiteParameter("@BusinessId", businessId),
                        new SQLiteParameter("@TokenNumber", tokenNumber),
                        new SQLiteParameter("@CustomerName", txtCustomerName.Text.Trim()),
                        new SQLiteParameter("@ContactNumber", txtContact.Text.Trim()),
                        new SQLiteParameter("@ServiceType", ddlServiceType.SelectedValue),
                        new SQLiteParameter("@CounterId", counterId.HasValue ? (object)counterId.Value : DBNull.Value)
                    });

                int tokenId = Convert.ToInt32(DBHelper.ExecuteScalar("SELECT last_insert_rowid()"));
                DBHelper.ExecuteNonQuery("INSERT INTO TokenStatusLog (TokenId, Status, ChangedBy) VALUES (@TokenId, 'Waiting', @ChangedBy)",
                    new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId), new SQLiteParameter("@ChangedBy", userId) });

                // Build QR URL using local network IP (so mobile devices on same WiFi can scan it)
                string baseUrl = GetLocalIpBaseUrl();
                string trackUrl = baseUrl + "/TrackStatus.aspx?token=" + Uri.EscapeDataString(tokenNumber);

                imgQR.ImageUrl = "https://api.qrserver.com/v1/create-qr-code/?size=200x200&color=000000&bgcolor=ffffff&data="
                                 + Uri.EscapeDataString(trackUrl);
                imgQR.AlternateText = "Scan to track queue position for " + tokenNumber;

                string counterText = counterId.HasValue ? ddlCounter.SelectedItem.Text : "Auto Assigned";
                lblTokenNumber.Text = tokenNumber;
                lblConfirmDetails.Text = txtCustomerName.Text.Trim()
                    + " &nbsp;|&nbsp; " + ddlServiceType.SelectedValue
                    + " &nbsp;|&nbsp; " + counterText;
                lblTrackURL.Text = trackUrl;

                pnlTokenGenerated.Visible = true;
                lblMessage.Text = "Token " + tokenNumber + " issued! Counter: " + counterText;
                lblMessage.CssClass = "msg-success"; lblMessage.Visible = true;
                txtCustomerName.Text = txtContact.Text = "";
                ddlServiceType.SelectedIndex = 0;
            }
            catch (Exception ex)
            { lblMessage.Text = "Error: " + ex.Message; lblMessage.CssClass = "msg-error"; lblMessage.Visible = true; }
        }

        private string GetLocalIpBaseUrl()
        {
            string publicUrl = System.Configuration.ConfigurationManager.AppSettings["PublicBaseUrl"];
            if (!string.IsNullOrWhiteSpace(publicUrl))
                return publicUrl.TrimEnd('/');
            return Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath.TrimEnd('/');
        }

        protected void btnNewToken_Click(object sender, EventArgs e)
        {
            pnlTokenGenerated.Visible = false;
            lblMessage.Visible = false;
            LoadCounters();
        }
    }
}
