using System;
using System.Web;

namespace QueueManagementSystem
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // This line is CRITICAL - it creates your database!
            DBHelper.InitializeDatabase();
        }

        protected void Session_Start(object sender, EventArgs e) { }
        protected void Application_Error(object sender, EventArgs e) { }
        protected void Session_End(object sender, EventArgs e) { }
        protected void Application_End(object sender, EventArgs e) { }
    }
}
