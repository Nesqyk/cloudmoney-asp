using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney.Auth
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionHelper.IsLoggedIn(Session))
            {
                Response.Redirect("~/Dashboard.aspx");
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            var master = (Site)Master;
            master.ClearMessage();

            var result = AuthManager.Register(
                txtFullName.Text,
                txtEmail.Text,
                txtPassword.Text,
                txtConfirmPassword.Text);

            if (!result.Success)
            {
                master.ShowError(result.Message);
                return;
            }

            master.ShowMessage(result.Message + " Your account number is: " + result.AccountNumber);
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
        }
    }
}
