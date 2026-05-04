<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyDepositsWithdrawals.aspx.cs" Inherits="CloudMoney.Reports.MyDepositsWithdrawals" MasterPageFile="~/Site.Master" Title="My Deposits / Withdrawals - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card">
        <h2>My Deposits / Withdrawals</h2>

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
                <label>Type</label>
                <asp:DropDownList ID="ddlType" runat="server">
                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Deposit">Deposit</asp:ListItem>
                    <asp:ListItem Value="Withdrawal">Withdrawal</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Button ID="btnFilter" runat="server" Text="Generate Report" CssClass="btn btn-primary" OnClick="btnFilter_Click" />
            </div>
        </div>

        <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="false" CssClass="report" EmptyDataText="No deposit or withdrawal records found for this range.">
            <Columns>
                <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="TransactionType" HeaderText="Type" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="P {0:N2}" />
                <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
