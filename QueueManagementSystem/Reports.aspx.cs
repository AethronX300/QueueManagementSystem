using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueueManagementSystem
{
    public partial class Reports : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack)
            {
                BindStatusDropdown();
                txtFromDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadReports();
            }
        }

        private void BindStatusDropdown()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem("-- All Statuses --", "All"));

            if (Session["BusinessTypeId"] != null)
            {
                int businessTypeId = Convert.ToInt32(Session["BusinessTypeId"]);

                if (businessTypeId == 1) // Restaurant
                {
                    ddlStatus.Items.Add(new ListItem("Order Placed", "Order Placed"));
                    ddlStatus.Items.Add(new ListItem("Preparing", "Preparing"));
                    ddlStatus.Items.Add(new ListItem("Ready to Serve", "Ready to Serve"));
                    ddlStatus.Items.Add(new ListItem("Served", "Served"));
                    ddlStatus.Items.Add(new ListItem("Payment Completed", "Payment Completed"));
                }
                else if (businessTypeId == 2) // Clinic
                {
                    ddlStatus.Items.Add(new ListItem("Waiting", "Waiting"));
                    ddlStatus.Items.Add(new ListItem("In Consultation", "In Consultation"));
                    ddlStatus.Items.Add(new ListItem("Completed", "Completed"));
                }
                else if (businessTypeId == 3) // Bank
                {
                    ddlStatus.Items.Add(new ListItem("Waiting", "Waiting"));
                    ddlStatus.Items.Add(new ListItem("At Counter", "At Counter"));
                    ddlStatus.Items.Add(new ListItem("Completed", "Completed"));
                }
            }
        }

        private void LoadReports(string fromDate = null, string toDate = null,
                                  string nameSearch = null, string statusFilter = null,
                                  string serviceFilter = null)
        {
            int businessId = Convert.ToInt32(Session["BusinessId"]);
            string query = @"SELECT TokenNumber, CustomerName, ContactNumber, ServiceType, 
                                    TableNumber, CurrentStatus, PaymentStatus, CreatedDate, CompletedDate 
                             FROM Tokens WHERE BusinessId = @BusinessId";

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                query += " AND date(CreatedDate) >= @FromDate AND date(CreatedDate) <= @ToDate";
            if (!string.IsNullOrEmpty(nameSearch))
                query += " AND (CustomerName LIKE @Name OR TokenNumber LIKE @Name)";
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
                query += " AND CurrentStatus = @Status";
            if (!string.IsNullOrEmpty(serviceFilter) && serviceFilter != "All")
                query += " AND ServiceType = @Service";

            query += " ORDER BY CreatedDate DESC";

            // Build parameter list dynamically
            var paramList = new System.Collections.Generic.List<SQLiteParameter>
            {
                new SQLiteParameter("@BusinessId", businessId)
            };
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                paramList.Add(new SQLiteParameter("@FromDate", fromDate));
                paramList.Add(new SQLiteParameter("@ToDate", toDate));
            }
            if (!string.IsNullOrEmpty(nameSearch))
                paramList.Add(new SQLiteParameter("@Name", "%" + nameSearch + "%"));
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
                paramList.Add(new SQLiteParameter("@Status", statusFilter));
            if (!string.IsNullOrEmpty(serviceFilter) && serviceFilter != "All")
                paramList.Add(new SQLiteParameter("@Service", serviceFilter));

            DataTable dt = DBHelper.ExecuteReader(query, paramList.ToArray());
            if (dt.Rows.Count > 0)
            {
                gvReports.DataSource = dt; gvReports.DataBind(); gvReports.Visible = true; pnlEmpty.Visible = false;
                lblRecordCount.Text = "Showing " + dt.Rows.Count + " record(s)";
                lblRecordCount.Visible = true;
            }
            else { gvReports.Visible = false; pnlEmpty.Visible = true; lblRecordCount.Visible = false; }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadReports(
                txtFromDate.Text,
                txtToDate.Text,
                txtNameSearch.Text.Trim(),
                ddlStatus.SelectedValue,
                txtServiceType.Text.Trim()
            );
        }

        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = txtToDate.Text = txtNameSearch.Text = txtServiceType.Text = "";
            ddlStatus.SelectedIndex = 0;
            LoadReports();
        }

        protected void gvReports_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReports.PageIndex = e.NewPageIndex;
            LoadReports(
                !string.IsNullOrEmpty(txtFromDate.Text) ? txtFromDate.Text : null,
                !string.IsNullOrEmpty(txtToDate.Text) ? txtToDate.Text : null,
                txtNameSearch.Text.Trim(),
                ddlStatus.SelectedValue,
                txtServiceType.Text.Trim()
            );
        }
    }
}
