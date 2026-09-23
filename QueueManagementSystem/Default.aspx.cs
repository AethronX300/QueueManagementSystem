using System;
using System.Web.UI;

namespace QueueManagementSystem
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Fallback: Ensure DB is always initialized even if app hasn't fully restarted
                DBHelper.InitializeDatabase();
            }

            if (Session["UserId"] != null)
            {
                RedirectToDashboard();
            }
        }

        protected void btnRestaurant_Click(object sender, EventArgs e)
        {
            Session["SelectedBusinessTypeId"] = 1;
            Session["SelectedBusinessType"] = "Restaurant";
            Response.Redirect("Login.aspx");
        }

        protected void btnClinic_Click(object sender, EventArgs e)
        {
            Session["SelectedBusinessTypeId"] = 2;
            Session["SelectedBusinessType"] = "Clinic";
            Response.Redirect("Login.aspx");
        }

        protected void btnBank_Click(object sender, EventArgs e)
        {
            Session["SelectedBusinessTypeId"] = 3;
            Session["SelectedBusinessType"] = "Bank";
            Response.Redirect("Login.aspx");
        }

        private void RedirectToDashboard()
        {
            int businessTypeId = Convert.ToInt32(Session["BusinessTypeId"]);
            switch (businessTypeId)
            {
                case 1: Response.Redirect("~/Restaurant/Dashboard.aspx"); break;
                case 2: Response.Redirect("~/Clinic/Dashboard.aspx"); break;
                case 3: Response.Redirect("~/Bank/Dashboard.aspx"); break;
            }
        }
    }
}
