using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney.Transactions
{
    public partial class SendMoney : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.RequireLogin(this))
            {
                return;
            }

            if (!IsPostBack)
            {
                pnlRecipientDetails.Visible = false;
                hfRecipientUserId.Value = string.Empty;
            }
        }

        protected void btnVerifyRecipient_Click(object sender, EventArgs e)
        {
            var master = (Site)Master;
            master.ClearMessage();

            var currentUserId = SessionHelper.GetUserId(Session);
            var result = TransactionManager.FindRecipient(txtRecipientAccount.Text, currentUserId);

            if (!result.Success)
            {
                pnlRecipientDetails.Visible = false;
                hfRecipientUserId.Value = string.Empty;
                master.ShowError(result.Message);
                return;
            }

            lblRecipientAccount.Text = result.AccountNumber;
            lblRecipientName.Text = result.FullName;
            hfRecipientUserId.Value = result.UserId.ToString();
            pnlRecipientDetails.Visible = true;
            master.ShowMessage("Recipient verified. You can now complete the transfer.");
        }

        protected void btnSendMoney_Click(object sender, EventArgs e)
        {
            var master = (Site)Master;
            master.ClearMessage();

            if (!int.TryParse(hfRecipientUserId.Value, out var recipientUserId))
            {
                master.ShowError("Verify the recipient account first.");
                return;
            }

            if (!ValidationHelper.TryParseAmount(txtAmount.Text, out var amount, out var amountError))
            {
                master.ShowError(amountError);
                return;
            }

            var senderUserId = SessionHelper.GetUserId(Session);
            var result = TransactionManager.SendMoney(senderUserId, recipientUserId, amount, txtPasswordConfirm.Text);

            if (!result.Success)
            {
                master.ShowError(result.Message);
                return;
            }

            master.ShowMessage(result.Message);
            txtRecipientAccount.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtPasswordConfirm.Text = string.Empty;
            hfRecipientUserId.Value = string.Empty;
            pnlRecipientDetails.Visible = false;
        }
    }
}
