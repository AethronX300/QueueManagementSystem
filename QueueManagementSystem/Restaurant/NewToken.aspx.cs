using System;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem.Restaurant
{
    public partial class NewToken : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["BusinessTypeId"]) != 1)
            { Response.Redirect("~/Default.aspx"); return; }
        }

        protected void btnGenerateToken_Click(object sender, EventArgs e)
        {
            try
            {
                int businessId = Convert.ToInt32(Session["BusinessId"]);
                int userId = Convert.ToInt32(Session["UserId"]);
                string tableNum = txtTableNumber.Text.Trim();

                // New format: T{tableNumber}-{NN} e.g. T2-03 = 3rd order at Table 2 today
                string tokenNumber = DBHelper.GetNextTableTokenNumber(businessId, tableNum);

                DBHelper.ExecuteNonQuery(
                    @"INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, TableNumber, CurrentStatus, PaymentStatus)
                      VALUES (@BusinessId, @TokenNumber, @CustomerName, @ContactNumber, 'Dine-In', @TableNumber, 'Order Placed', 'Pending')",
                    new SQLiteParameter[] {
                        new SQLiteParameter("@BusinessId", businessId),
                        new SQLiteParameter("@TokenNumber", tokenNumber),
                        new SQLiteParameter("@CustomerName", txtCustomerName.Text.Trim()),
                        new SQLiteParameter("@ContactNumber", txtContact.Text.Trim()),
                        new SQLiteParameter("@TableNumber", tableNum)
                    });

                int tokenId = Convert.ToInt32(DBHelper.ExecuteScalar("SELECT last_insert_rowid()"));
                DBHelper.ExecuteNonQuery("INSERT INTO TokenStatusLog (TokenId, Status, ChangedBy) VALUES (@TokenId, 'Order Placed', @ChangedBy)",
                    new SQLiteParameter[] { new SQLiteParameter("@TokenId", tokenId), new SQLiteParameter("@ChangedBy", userId) });

                // Build QR URL using local network IP (so mobile devices on same WiFi can scan it)
                string baseUrl = GetLocalIpBaseUrl();
                string trackUrl = baseUrl + "/TrackStatus.aspx?token=" + Uri.EscapeDataString(tokenNumber);

                // QR code via qrserver.com (200×200, black on white)
                imgQR.ImageUrl = "https://api.qrserver.com/v1/create-qr-code/?size=200x200&color=000000&bgcolor=ffffff&data="
                                 + Uri.EscapeDataString(trackUrl);
                imgQR.AlternateText = "Scan to track order " + tokenNumber;

                lblTokenNumber.Text = tokenNumber;
                lblConfirmName.Text = txtCustomerName.Text.Trim() + " &nbsp;|&nbsp; Table " + tableNum;
                lblTrackURL.Text = trackUrl;

                pnlTokenGenerated.Visible = true;
                lblMessage.Text = "Token " + tokenNumber + " generated for Table " + tableNum + "!";
                lblMessage.CssClass = "msg-success"; lblMessage.Visible = true;
                txtCustomerName.Text = txtTableNumber.Text = txtContact.Text = "";
            }
            catch (Exception ex)
            { lblMessage.Text = "Error: " + ex.Message; lblMessage.CssClass = "msg-error"; lblMessage.Visible = true; }
        }

        /// <summary>
        // Returns the base URL — uses Web.config PublicBaseUrl if set (ngrok etc.),
        // otherwise falls back to localhost so the direct link works in the same browser.
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
