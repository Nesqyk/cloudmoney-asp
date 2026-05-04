using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney.Auth
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionHelper.IsLoggedIn(Session))
            {
                Response.Redirect("~/Dashboard.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var master = (Site)Master;
            master.ClearMessage();

            var result = AuthManager.Login(txtEmail.Text, txtPassword.Text);
            if (!result.Success)
            {
                master.ShowError(result.Message);
                return;
            }

            SessionHelper.SignIn(Session, result);
            Response.Redirect("~/Dashboard.aspx");
        }
    }
}
