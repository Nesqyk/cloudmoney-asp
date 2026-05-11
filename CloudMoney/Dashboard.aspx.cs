using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney
{
    public partial class Dashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.RequireLogin(this))
            {
                return;
            }

            if (IsPostBack)
            {
                return;
            }

            LoadDashboard();
        }

        private void LoadDashboard()
        {
            var userId = SessionHelper.GetUserId(Session);
            var user = AuthManager.GetUserById(userId);

            if (user == null)
            {
                SessionHelper.SignOut(Session);
                Response.Redirect("~/Auth/Login.aspx");
                return;
            }

            var balance = TransactionManager.GetBalanceSnapshot(userId);

            lblAccountNumber.Text = Convert.ToString(user["AccountNumber"]);
            lblFullName.Text = Convert.ToString(user["FullName"]);
            lblDateRegistered.Text = Convert.ToDateTime(user["DateRegistered"]).ToString("MMMM dd, yyyy hh:mm tt");
            lblCurrentBalance.Text = "P" + balance.CurrentBalance.ToString("N2");
            lblTotalSent.Text = "P" + balance.TotalSent.ToString("N2");

            var notifications = TransactionManager.GetRecentReceivedTransfers(userId, 5);
            repNotifications.DataSource = notifications;
            repNotifications.DataBind();
            pnlNoNotifications.Visible = notifications.Rows.Count == 0;
        }
    }
}
