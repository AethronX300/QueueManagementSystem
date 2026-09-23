using System;
using System.Data;
using System.Data.SQLite;
using System.Web;

namespace QueueManagementSystem
{
    /// <summary>
    /// TrackStatus.aspx.cs - Public User Dashboard
    /// - Reads token from URL query string
    /// - Fetches token details from database using ADO.NET
    /// - Displays status, queue position, estimated wait time
    /// - Shows UPI payment section with QR code and payment app links
    /// - Allows user to confirm payment (updates DB)
    /// </summary>
    public partial class TrackStatus : System.Web.UI.Page
    {
        // Demo UPI ID (for college project — replace with real merchant UPI in production)
        private const string UPI_ID   = "queueflow@paytm";
        private const string UPI_NAME = "QueueFlow";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Read token from URL: TrackStatus.aspx?token=R001
                string token = Request.QueryString["token"];

                if (string.IsNullOrEmpty(token))
                {
                    ShowError();
                    return;
                }

                LoadTokenDetails(token);
            }
        }

        // -------------------------------------------------------
        // Load token details from database using ADO.NET
        // -------------------------------------------------------
        private void LoadTokenDetails(string tokenNumber)
        {
            string query = @"
                SELECT t.TokenId, t.TokenNumber, t.CustomerName, t.ServiceType,
                       t.CurrentStatus, t.PaymentStatus,
                       bt.TypeName AS BusinessType
                FROM Tokens t
                INNER JOIN Businesses b  ON t.BusinessId  = b.BusinessId
                INNER JOIN BusinessTypes bt ON b.BusinessTypeId = bt.BusinessTypeId
                WHERE t.TokenNumber = @TokenNumber
                ORDER BY t.TokenId DESC
                LIMIT 1";

            SQLiteParameter[] parameters = {
                new SQLiteParameter("@TokenNumber", tokenNumber)
            };

            DataTable dt = DBHelper.ExecuteReader(query, parameters);

            if (dt.Rows.Count == 0)
            {
                ShowError();
                return;
            }

            DataRow row = dt.Rows[0];

            int    tokenId       = Convert.ToInt32(row["TokenId"]);
            string status        = row["CurrentStatus"].ToString();
            string paymentStatus = row["PaymentStatus"].ToString();
            string businessType  = row["BusinessType"].ToString();
            string serviceType   = row["ServiceType"].ToString();

            // -- Store in ViewState for postback (button click) --
            ViewState["TokenId"]     = tokenId;
            ViewState["TokenNumber"] = tokenNumber;

            // -- Display token info --
            lblTokenNumber.Text   = row["TokenNumber"].ToString();
            lblCustomerName.Text  = row["CustomerName"].ToString();

            // Reason for visiting (Clinic and Bank)
            if (businessType == "Clinic" || businessType == "Bank")
            {
                rowReason.Visible = true;
                lblReason.Text = serviceType;
            }
            else
            {
                rowReason.Visible = false;
            }

            // Business type badge
            lblBusinessType.Text     = businessType;
            lblBusinessType.CssClass = "business-badge";

            // Status badge
            lblStatus.Text     = status;
            lblStatus.CssClass = "sbadge " + GetStatusClass(status);

            // Queue position
            int position       = GetQueuePosition(tokenId);
            lblPosition.Text   = position > 0 ? "#" + position : "—";

            // Estimated wait time
            lblWaitTime.Text   = GetEstimatedWait(position, status);

            // Friendly message
            SetFriendlyMessage(status, businessType);

            // -- Payment section --
            if (businessType == "Bank")
            {
                pnlPaymentSection.Visible = false;
            }
            else
            {
                pnlPaymentSection.Visible = true;
                SetupPaymentSection(tokenNumber, paymentStatus);
            }

            // Show main panel
            pnlTokenInfo.Visible = true;
            pnlError.Visible     = false;
        }

        // -------------------------------------------------------
        // Payment Section Setup
        // -------------------------------------------------------
        private void SetupPaymentSection(string tokenNumber, string paymentStatus)
        {
            if (paymentStatus == "Paid" || paymentStatus == "Payment Completed")
            {
                // Already paid — show confirmation card
                pnlPayment.Visible     = false;
                pnlPaymentDone.Visible = true;
                return;
            }

            // Build UPI deep link
            // Format: upi://pay?pa=<vpa>&pn=<name>&am=0&cu=INR&tn=<note>
            string note    = "Token " + tokenNumber;
            string upiLink = string.Format(
                "upi://pay?pa={0}&pn={1}&am=0&cu=INR&tn={2}",
                UPI_ID, Uri.EscapeDataString(UPI_NAME), Uri.EscapeDataString(note)
            );

            // QR code using free API (qrserver.com)
            string qrApiUrl = "https://api.qrserver.com/v1/create-qr-code/?size=160x160&data="
                              + Uri.EscapeDataString(upiLink);
            imgQR.ImageUrl = qrApiUrl;

            // Payment app deep links (app-specific URI schemes)
            lnkGPay.NavigateUrl    = "gpay://upi/pay?pa="    + UPI_ID + "&pn=" + Uri.EscapeDataString(UPI_NAME) + "&cu=INR&tn=" + Uri.EscapeDataString(note);
            lnkPhonePe.NavigateUrl = "phonepe://pay?pa="     + UPI_ID + "&pn=" + Uri.EscapeDataString(UPI_NAME) + "&cu=INR&tn=" + Uri.EscapeDataString(note);
            lnkPaytm.NavigateUrl   = "paytmmp://pay?pa="     + UPI_ID + "&pn=" + Uri.EscapeDataString(UPI_NAME) + "&cu=INR&tn=" + Uri.EscapeDataString(note);

            // Payment status badge
            lblPaymentStatus.Text     = "Pending";
            lblPaymentStatus.CssClass = "pay-badge pay-pending";

            pnlPayment.Visible     = true;
            pnlPaymentDone.Visible = false;
        }

        // -------------------------------------------------------
        // Confirm Payment Button Click (PostBack)
        // -------------------------------------------------------
        protected void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            if (ViewState["TokenId"] == null) return;

            int tokenId = Convert.ToInt32(ViewState["TokenId"]);

            // Update payment status in database using ADO.NET
            string query = "UPDATE Tokens SET PaymentStatus = 'Paid' WHERE TokenId = @TokenId";
            SQLiteParameter[] parameters = {
                new SQLiteParameter("@TokenId", tokenId)
            };
            DBHelper.ExecuteNonQuery(query, parameters);

            // Reload page to reflect updated status
            string tokenNumber = ViewState["TokenNumber"].ToString();
            Response.Redirect("~/TrackStatus.aspx?token=" + tokenNumber);
        }

        // -------------------------------------------------------
        // Queue Position: count waiting tokens on or before this ID
        // -------------------------------------------------------
        private int GetQueuePosition(int tokenId)
        {
            string query = @"
                SELECT COUNT(*) FROM Tokens
                WHERE TokenId <= @TokenId
                AND CurrentStatus IN ('Waiting', 'Order Placed')
                AND date(CreatedDate) = date('now','localtime')";

            SQLiteParameter[] parameters = {
                new SQLiteParameter("@TokenId", tokenId)
            };

            object result = DBHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        // -------------------------------------------------------
        // Estimated wait: ~3 minutes per person ahead
        // -------------------------------------------------------
        private string GetEstimatedWait(int position, string status)
        {
            if (status == "Serving" || status == "Served" ||
                status == "Ready"   || status == "Ready to Serve" ||
                status == "Completed" || status == "Payment Completed")
                return "No wait";

            if (position <= 0) return "—";

            int minutes = (position - 1) * 3;
            if (minutes == 0)     return "You're next!";
            if (minutes < 60)     return "~" + minutes + " min";
            return "~" + (minutes / 60) + " hr " + (minutes % 60) + " min";
        }

        // -------------------------------------------------------
        // Friendly message + icon based on status & business type
        // -------------------------------------------------------
        private void SetFriendlyMessage(string status, string businessType)
        {
            string icon, message;

            switch (status)
            {
                case "Waiting":
                case "Order Placed":
                    icon = "<i class='fas fa-hourglass-half' style='color:#D4AF37'></i>";
                    if (businessType == "Restaurant")
                        message = "Your order has been placed. Please wait for your turn.";
                    else if (businessType == "Clinic")
                        message = "You are in the queue. Your consultation will begin shortly.";
                    else if (businessType == "Bank")
                        message = "Please wait — you will be called to the counter soon.";
                    else
                        message = "Please wait for your turn.";
                    break;

                case "Preparing":
                    icon    = "<i class='fas fa-fire' style='color:#c39bd3'></i>";
                    message = businessType == "Restaurant"
                        ? "Your order is being prepared in the kitchen!"
                        : "Your request is being processed. Please wait.";
                    break;

                case "Ready":
                case "Ready to Serve":
                    icon = "<i class='fas fa-check-circle' style='color:#2ecc71'></i>";
                    if (businessType == "Restaurant")
                        message = "Your order is ready! Please collect from the counter.";
                    else if (businessType == "Clinic")
                        message = "Please proceed to the doctor's cabin.";
                    else if (businessType == "Bank")
                        message = "Please proceed to your assigned counter.";
                    else
                        message = "You are ready to be served. Please proceed.";
                    break;

                case "Serving":
                case "Served":
                    icon = "<i class='fas fa-concierge-bell' style='color:#5dade2'></i>";
                    if (businessType == "Restaurant")
                        message = "Your food has been served. Enjoy your meal!";
                    else if (businessType == "Clinic")
                        message = "Your consultation is currently in progress.";
                    else if (businessType == "Bank")
                        message = "Your transaction is being processed.";
                    else
                        message = "You are currently being served.";
                    break;

                case "Completed":
                case "Payment Completed":
                    icon    = "<i class='fas fa-star' style='color:#D4AF37'></i>";
                    message = "Service completed. Thank you for your visit!";
                    break;

                default:
                    icon    = "<i class='fas fa-info-circle' style='color:#D4AF37'></i>";
                    message = "Status: " + status;
                    break;
            }

            lblMsgIcon.Text = icon;
            lblMessage.Text = message;
        }

        // -------------------------------------------------------
        // CSS class for status badge
        // -------------------------------------------------------
        private string GetStatusClass(string status)
        {
            switch (status)
            {
                case "Waiting":
                case "Order Placed":   return "st-waiting";
                case "Preparing":      return "st-preparing";
                case "Ready":
                case "Ready to Serve": return "st-ready";
                case "Serving":
                case "Served":         return "st-serving";
                case "Completed":
                case "Payment Completed": return "st-completed";
                default:               return "st-waiting";
            }
        }

        // -------------------------------------------------------
        // Show error panel
        // -------------------------------------------------------
        private void ShowError()
        {
            pnlTokenInfo.Visible = false;
            pnlError.Visible     = true;
        }
    }
}
