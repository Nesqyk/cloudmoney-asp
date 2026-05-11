using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney.Auth
{
    public partial class Auth : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionHelper.IsLoggedIn(Session))
            {
                Response.Redirect("~/Dashboard.aspx");
                return;
            }

            Response.Redirect("~/Auth/Login.aspx");
        }
    }
}
