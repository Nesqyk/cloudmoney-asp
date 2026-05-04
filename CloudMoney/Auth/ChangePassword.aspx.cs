using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney.Auth
{
    public partial class ChangePassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.RequireLogin(this))
            {
                return;
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            var master = (Site)Master;
            master.ClearMessage();

            var userId = SessionHelper.GetUserId(Session);
            var result = AuthManager.ChangePassword(userId, txtCurrentPassword.Text, txtNewPassword.Text, txtConfirmNewPassword.Text);

            if (result.Success)
            {
                master.ShowMessage(result.Message);
                txtCurrentPassword.Text = string.Empty;
                txtNewPassword.Text = string.Empty;
                txtConfirmNewPassword.Text = string.Empty;
                return;
            }

            master.ShowError(result.Message);
        }
    }
}
