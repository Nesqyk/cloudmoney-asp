using System;
using System.Web.UI;

namespace CloudMoney
{
    public partial class Site : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateNavigation();
        }

        private void UpdateNavigation()
        {
            var isLoggedIn = Session["UserId"] != null;
            phLoggedIn.Visible = isLoggedIn;
            phLoggedOut.Visible = !isLoggedIn;
        }

        public void ShowMessage(string message)
        {
            pnlMessage.CssClass = "message message-success";
            lblMessage.Text = message;
            pnlMessage.Visible = true;
        }

        public void ShowError(string message)
        {
            pnlMessage.CssClass = "message message-error";
            lblMessage.Text = message;
            pnlMessage.Visible = true;
        }

        public void ShowInfo(string message)
        {
            pnlMessage.CssClass = "message message-info";
            lblMessage.Text = message;
            pnlMessage.Visible = true;
        }

        public void ClearMessage()
        {
            pnlMessage.Visible = false;
            lblMessage.Text = string.Empty;
        }
    }
}
