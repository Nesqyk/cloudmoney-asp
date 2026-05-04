<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SentReceivedHistory.aspx.cs" Inherits="CloudMoney.Reports.SentReceivedHistory" MasterPageFile="~/Site.Master" Title="My Sent / Received - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card">
        <h2>My Sent / Received Transactions</h2>

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
                    <asp:ListItem Value="Sent">Sent</asp:ListItem>
                    <asp:ListItem Value="Received">Received</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Button ID="btnFilter" runat="server" Text="Generate Report" CssClass="btn btn-primary" OnClick="btnFilter_Click" />
            </div>
        </div>

        <asp:GridView ID="gvTransfers" runat="server" AutoGenerateColumns="false" CssClass="report" EmptyDataText="No sent or received transfer records found for this range.">
            <Columns>
                <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="TransferType" HeaderText="Transfer Type" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="P {0:N2}" />
                <asp:BoundField DataField="CounterpartyAccount" HeaderText="Counterparty Account" />
                <asp:BoundField DataField="CounterpartyName" HeaderText="Counterparty Name" />
                <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
