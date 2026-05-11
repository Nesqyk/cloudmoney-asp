<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatementOfAccount.aspx.cs" Inherits="CloudMoney.Reports.StatementOfAccount" MasterPageFile="~/Site.Master" Title="Statement of Account - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card">
        <h2>Statement of Account</h2>

        <div class="inline-form">
            <div class="form-group">
                <label>Start Date</label>
                <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" />
            </div>
            <div class="form-group">
                <label>End Date</label>
                <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" />
            </div>
            <div class="form-group">
                <asp:Button ID="btnFilter" runat="server" Text="Generate Report" CssClass="btn btn-primary" OnClick="btnFilter_Click" />
            </div>
        </div>

        <asp:GridView ID="gvStatement" runat="server" AutoGenerateColumns="false" CssClass="report" EmptyDataText="No transactions found for this range.">
            <Columns>
                <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="TransactionType" HeaderText="Type" />
                <asp:BoundField DataField="Direction" HeaderText="Direction" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="P {0:N2}" />
                <asp:BoundField DataField="SenderAccount" HeaderText="Sender Account" />
                <asp:BoundField DataField="SenderName" HeaderText="Sender Name" />
                <asp:BoundField DataField="RecipientAccount" HeaderText="Recipient Account" />
                <asp:BoundField DataField="RecipientName" HeaderText="Recipient Name" />
                <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
