using System;
using System.Web;
using System.Web.UI;

namespace QueueManagementSystem
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                pnlLoggedIn.Visible = true;
                pnlSidebar.Visible = true;

                lblBusinessName.Text = Session["BusinessName"]?.ToString() ?? "";
                lblUsername.Text = Session["FullName"]?.ToString() ?? "";

                int businessTypeId = Convert.ToInt32(Session["BusinessTypeId"]);
                SetSidebarLinks(businessTypeId);
            }
            else
            {
                pnlLoggedIn.Visible = false;
                pnlSidebar.Visible = false;
            }
        }

        private void SetSidebarLinks(int businessTypeId)
        {
            switch (businessTypeId)
            {
                case 1: // Restaurant
                    lblModuleType.Text = "🍽️ Restaurant";
                    lnkDashboard.NavigateUrl = "~/Restaurant/Dashboard.aspx";
                    lnkNewToken.NavigateUrl = "~/Restaurant/NewToken.aspx";
                    lnkQueueDisplay.NavigateUrl = "~/Restaurant/OrderStatus.aspx";
                    lnkNewToken.Text = "<i class='fas fa-ticket-alt'></i> New Order";
                    lnkQueueDisplay.Text = "<i class='fas fa-list-ol'></i> Order Status";
                    break;

                case 2: // Clinic
                    lblModuleType.Text = "🏥 Clinic";
                    lnkDashboard.NavigateUrl = "~/Clinic/Dashboard.aspx";
                    lnkNewToken.NavigateUrl = "~/Clinic/NewToken.aspx";
                    lnkQueueDisplay.NavigateUrl = "~/Clinic/QueueDisplay.aspx";
                    lnkNewToken.Text = "<i class='fas fa-ticket-alt'></i> New Patient";
                    lnkQueueDisplay.Text = "<i class='fas fa-list-ol'></i> Queue Display";
                    break;

                case 3: // Bank
                    lblModuleType.Text = "🏦 Bank";
                    lnkDashboard.NavigateUrl = "~/Bank/Dashboard.aspx";
                    lnkNewToken.NavigateUrl = "~/Bank/NewToken.aspx";
                    lnkQueueDisplay.NavigateUrl = "~/Bank/QueueDisplay.aspx";
                    lnkNewToken.Text = "<i class='fas fa-ticket-alt'></i> New Token";
                    lnkQueueDisplay.Text = "<i class='fas fa-list-ol'></i> Queue Display";
                    lnkPayment.Visible = false;   // Bank has no payment simulation
                    break;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Default.aspx");
        }
    }
}
