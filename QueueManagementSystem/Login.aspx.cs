using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DBHelper.InitializeDatabase();
                
                if (Session["SelectedBusinessType"] != null)
                    lblBusinessType.Text = "Sign in to your " + Session["SelectedBusinessType"].ToString() + " account";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"SELECT u.UserId, u.BusinessId, u.FullName, u.Role, 
                                       b.BusinessName, b.BusinessTypeId 
                                FROM Users u 
                                INNER JOIN Businesses b ON u.BusinessId = b.BusinessId 
                                WHERE u.Username = @Username AND u.Password = @Password";
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Username", txtUsername.Text.Trim()),
                    new SQLiteParameter("@Password", txtPassword.Text.Trim())
                };
                DataTable dt = DBHelper.ExecuteReader(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    int userBusinessTypeId = Convert.ToInt32(row["BusinessTypeId"]);
                    if (Session["SelectedBusinessTypeId"] != null)
                    {
                        int selectedTypeId = Convert.ToInt32(Session["SelectedBusinessTypeId"]);
                        if (userBusinessTypeId != selectedTypeId)
                        {
                            lblMessage.Text = "This account belongs to a different business type.";
                            lblMessage.CssClass = "msg-error";
                            lblMessage.Visible = true;
                            return;
                        }
                    }
                    Session["UserId"] = row["UserId"];
                    Session["BusinessId"] = row["BusinessId"];
                    Session["BusinessTypeId"] = row["BusinessTypeId"];
                    Session["BusinessName"] = row["BusinessName"];
                    Session["FullName"] = row["FullName"];
                    Session["Role"] = row["Role"];
                    switch (userBusinessTypeId)
                    {
                        case 1: Response.Redirect("~/Restaurant/Dashboard.aspx"); break;
                        case 2: Response.Redirect("~/Clinic/Dashboard.aspx"); break;
                        case 3: Response.Redirect("~/Bank/Dashboard.aspx"); break;
                        default: Response.Redirect("~/Default.aspx"); break;
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid username or password.";
                    lblMessage.CssClass = "msg-error";
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Login error: " + ex.Message;
                lblMessage.CssClass = "msg-error";
                lblMessage.Visible = true;
            }
        }
    }
}
