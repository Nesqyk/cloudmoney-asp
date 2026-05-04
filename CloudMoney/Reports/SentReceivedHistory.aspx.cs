using CloudMoney.Logic;
using System;
using System.Web.UI;

namespace CloudMoney.Reports
{
    public partial class SentReceivedHistory : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.RequireLogin(this))
            {
                return;
            }

            if (!IsPostBack)
            {
                txtStartDate.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
                txtEndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                ddlType.SelectedValue = "All";
                BindReport();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindReport();
        }

        private void BindReport()
        {
            var master = (Site)Master;
            master.ClearMessage();

            if (!TryGetDateRange(out var startDate, out var endDate))
            {
                return;
            }

            var userId = SessionHelper.GetUserId(Session);
            var data = ReportManager.GetSentReceivedHistory(userId, startDate, endDate, ddlType.SelectedValue);
            gvTransfers.DataSource = data;
            gvTransfers.DataBind();

            master.ShowInfo("Report generated successfully.");
        }

        private bool TryGetDateRange(out DateTime startDate, out DateTime endDate)
        {
            var master = (Site)Master;
            startDate = default(DateTime);
            endDate = default(DateTime);

            if (!DateTime.TryParse(txtStartDate.Text, out startDate) || !DateTime.TryParse(txtEndDate.Text, out endDate))
            {
                master.ShowError("Provide valid start and end dates.");
                return false;
            }

            if (!ValidationHelper.ValidateDateRange(startDate, endDate, out var validationMessage))
            {
                master.ShowError(validationMessage);
                return false;
            }

            return true;
        }
    }
}
