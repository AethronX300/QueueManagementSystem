using System;
using System.Data.SQLite;
using System.Web.UI;

namespace QueueManagementSystem
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DBHelper.InitializeDatabase();
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string insertBusiness = @"INSERT INTO Businesses (BusinessTypeId, BusinessName, Address, ContactNumber) 
                                        VALUES (@BusinessTypeId, @BusinessName, @Address, @ContactNumber)";
                SQLiteParameter[] bizParams = {
                    new SQLiteParameter("@BusinessTypeId", Convert.ToInt32(ddlBusinessType.SelectedValue)),
                    new SQLiteParameter("@BusinessName", txtBusinessName.Text.Trim()),
                    new SQLiteParameter("@Address", txtAddress.Text.Trim()),
                    new SQLiteParameter("@ContactNumber", txtContact.Text.Trim())
                };
                int businessId = DBHelper.ExecuteInsertAndGetId(insertBusiness, bizParams);

                string insertUser = @"INSERT INTO Users (BusinessId, Username, Password, FullName, Role) 
                                    VALUES (@BusinessId, @Username, @Password, @FullName, 'Admin')";
                SQLiteParameter[] userParams = {
                    new SQLiteParameter("@BusinessId", businessId),
                    new SQLiteParameter("@Username", txtUsername.Text.Trim()),
                    new SQLiteParameter("@Password", txtPassword.Text.Trim()),
                    new SQLiteParameter("@FullName", txtFullName.Text.Trim())
                };
                DBHelper.ExecuteNonQuery(insertUser, userParams);

                int businessTypeId = Convert.ToInt32(ddlBusinessType.SelectedValue);
                if (businessTypeId == 3)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        string insertCounter = @"INSERT INTO ServiceCounters (BusinessId, CounterName, IsActive) VALUES (@BusinessId, @CounterName, 1)";
                        SQLiteParameter[] counterParams = {
                            new SQLiteParameter("@BusinessId", businessId),
                            new SQLiteParameter("@CounterName", "Counter " + i)
                        };
                        DBHelper.ExecuteNonQuery(insertCounter, counterParams);
                    }
                }
                if (businessTypeId == 2)
                {
                    string insertRoom = @"INSERT INTO ServiceCounters (BusinessId, CounterName, IsActive) VALUES (@BusinessId, @CounterName, 1)";
                    SQLiteParameter[] roomParams = {
                        new SQLiteParameter("@BusinessId", businessId),
                        new SQLiteParameter("@CounterName", "Consultation Room 1")
                    };
                    DBHelper.ExecuteNonQuery(insertRoom, roomParams);
                }

                lblMessage.Text = "Registration successful! You can now login.";
                lblMessage.CssClass = "msg-success";
                lblMessage.Visible = true;
                txtBusinessName.Text = txtAddress.Text = txtContact.Text = txtFullName.Text = txtUsername.Text = txtPassword.Text = "";
            }
            catch (SQLiteException ex)
            {
                lblMessage.Text = ex.ErrorCode == 19 ? "Username already exists." : "Registration failed: " + ex.Message;
                lblMessage.CssClass = "msg-error";
                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred: " + ex.Message;
                lblMessage.CssClass = "msg-error";
                lblMessage.Visible = true;
            }
        }
    }
}
