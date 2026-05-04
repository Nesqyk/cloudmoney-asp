<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendMoney.aspx.cs" Inherits="CloudMoney.Transactions.SendMoney" MasterPageFile="~/Site.Master" Title="Send Money - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="centered-form" style="max-width:560px;">
        <div class="card">
            <h2>Send CloudMoney</h2>

            <div class="form-group">
                <label>Recipient Account Number</label>
                <asp:TextBox ID="txtRecipientAccount" runat="server" placeholder="Enter recipient account number" />
            </div>

            <asp:Button ID="btnVerifyRecipient" runat="server" Text="Verify Recipient" CssClass="btn btn-secondary" OnClick="btnVerifyRecipient_Click" />

            <asp:Panel ID="pnlRecipientDetails" runat="server" Visible="false">
                <div class="recipient-card">
                    <div class="rec-label">Recipient Account Number</div>
                    <div class="rec-value"><asp:Label ID="lblRecipientAccount" runat="server" /></div>
                    <div class="rec-label" style="margin-top:10px;">Recipient Full Name</div>
                    <div class="rec-value"><asp:Label ID="lblRecipientName" runat="server" /></div>
                </div>

                <asp:HiddenField ID="hfRecipientUserId" runat="server" />

                <div class="form-group">
                    <label>Amount to Send</label>
                    <asp:TextBox ID="txtAmount" runat="server" placeholder="Enter amount (100 - 2,000)" />
                </div>

                <div class="form-group">
                    <label>Confirm Your Password</label>
                    <asp:TextBox ID="txtPasswordConfirm" runat="server" TextMode="Password" placeholder="Re-enter your password" />
                </div>

                <asp:Button ID="btnSendMoney" runat="server" Text="Send CloudMoney" CssClass="btn btn-primary" OnClick="btnSendMoney_Click" Style="width:100%;" />
            </asp:Panel>
        </div>
    </div>
</asp:Content>
