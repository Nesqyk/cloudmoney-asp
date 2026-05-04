<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Deposit.aspx.cs" Inherits="CloudMoney.Transactions.Deposit" MasterPageFile="~/Site.Master" Title="Deposit - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="centered-form">
        <div class="card">
            <h2>Deposit</h2>

            <div class="balance-box">
                <div class="balance-label">Current Balance</div>
                <div class="balance-value"><asp:Label ID="lblCurrentBalance" runat="server" /></div>
            </div>

            <div class="form-group">
                <label>Deposit Amount</label>
                <asp:TextBox ID="txtAmount" runat="server" placeholder="Enter amount (100 - 2,000)" />
                <small>Amount must be between P100 and P2,000 and divisible by 100.</small>
            </div>

            <asp:Button ID="btnDeposit" runat="server" CssClass="btn btn-success" Text="Deposit" OnClick="btnDeposit_Click" Style="width:100%;" />
        </div>
    </div>
</asp:Content>
