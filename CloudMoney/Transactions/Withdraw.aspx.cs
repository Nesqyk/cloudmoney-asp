using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney.Transactions
{
    public partial class Withdraw : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.RequireLogin(this))
            {
                return;
            }

            if (!IsPostBack)
            {
                LoadBalance();
            }
        }

        protected void btnWithdraw_Click(object sender, EventArgs e)
        {
            var master = (Site)Master;
            master.ClearMessage();

            if (!ValidationHelper.TryParseAmount(txtAmount.Text, out var amount, out var amountError))
            {
                master.ShowError(amountError);
                LoadBalance();
                return;
            }

            var userId = SessionHelper.GetUserId(Session);
            var result = TransactionManager.Withdraw(userId, amount);

            if (result.Success)
            {
                master.ShowMessage(result.Message);
                txtAmount.Text = string.Empty;
            }
            else
            {
                master.ShowError(result.Message);
            }

            LoadBalance();
        }

        private void LoadBalance()
        {
            var userId = SessionHelper.GetUserId(Session);
            var balance = TransactionManager.GetBalanceSnapshot(userId);
            lblCurrentBalance.Text = "P" + balance.CurrentBalance.ToString("N2");
        }
    }
}
