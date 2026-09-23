using System;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem.Clinic
{
    public partial class NewToken : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 2)
            { Response.Redirect("~/Default.aspx"); return; }
        }

        protected void btnGenerateToken_Click(object sender, EventArgs e)
        {
            try
            {
                int businessId = Convert.ToInt32(Session["BusinessId"]);
                int userId = Convert.ToInt32(Session["UserId"]);

                // Token format: C{nn} e.g. C006 = 6th patient today
                string tokenNumber = DBHelper.GetNextTokenNumber(businessId, "C");

                DBHelper.ExecuteNonQuery(
                    @"INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus)
                      VALUES (@BusinessId, @TokenNumber, @CustomerName, @ContactNumber, @ServiceType, 'Waiting', 'Pending')",
                    new SQLiteParameter[] {
                        new SQLiteParameter("@BusinessId", businessId),
                        new SQLiteParameter("@TokenNumber", tokenNumber),
                        new SQLiteParameter("@CustomerName", txtPatientName.Text.Trim()),
                        new SQLiteParameter("@ContactNumber", txtContact.Text.Trim()),
                        new SQLiteParameter("@ServiceType", txtSymptoms.Text.Trim())
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

                lblTokenNumber.Text = tokenNumber;
                lblConfirmName.Text = txtPatientName.Text.Trim()
                    + (string.IsNullOrWhiteSpace(txtSymptoms.Text) ? "" : " &nbsp;|&nbsp; " + txtSymptoms.Text.Trim());
                lblTrackURL.Text = trackUrl;

                pnlTokenGenerated.Visible = true;
                lblMessage.Text = "Token " + tokenNumber + " issued to " + txtPatientName.Text.Trim() + "!";
                lblMessage.CssClass = "msg-success"; lblMessage.Visible = true;
                txtPatientName.Text = txtContact.Text = txtSymptoms.Text = "";
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
        }
    }
}
