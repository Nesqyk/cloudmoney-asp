using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney.Auth
{
    public partial class Logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionHelper.SignOut(Session);
            Response.Redirect("~/Auth/Login.aspx");
        }
    }
}
