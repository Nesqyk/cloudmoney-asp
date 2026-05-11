<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="CloudMoney.Dashboard" MasterPageFile="~/Site.Master" Title="Dashboard - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card">
        <h2>Account Dashboard</h2>

        <div class="dashboard-grid">
            <div class="stat-card accent-blue">
                <div class="label">Account Number</div>
                <div class="value"><asp:Label ID="lblAccountNumber" runat="server" /></div>
            </div>
            <div class="stat-card accent-purple">
                <div class="label">Full Name</div>
                <div class="value" style="font-size:20px;"><asp:Label ID="lblFullName" runat="server" /></div>
            </div>
            <div class="stat-card accent-green">
                <div class="label">Total Current Balance</div>
                <div class="value balance"><asp:Label ID="lblCurrentBalance" runat="server" /></div>
            </div>
            <div class="stat-card accent-orange">
                <div class="label">Total Amount Sent</div>
                <div class="value sent"><asp:Label ID="lblTotalSent" runat="server" /></div>
            </div>
        </div>

        <div class="info-row">
            <div class="info-label">Date Registered</div>
            <div class="info-value"><asp:Label ID="lblDateRegistered" runat="server" /></div>
        </div>
    </div>

    <div class="card">
        <h3>Recent Received CloudMoney</h3>
        <asp:Repeater ID="repNotifications" runat="server">
            <ItemTemplate>
                <div class="notification">
                    You received <span class="notif-amount">P<%# Eval("Amount", "{0:N2}") %></span>
                    from <%# Eval("SenderFullName") %> (Acct: <%# Eval("SenderAccountNumber") %>)
                    <div class="notif-date"><%# Eval("TransactionDate", "{0:MMM dd, yyyy hh:mm tt}") %></div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlNoNotifications" runat="server" CssClass="empty-state" Visible="false">
            <p>No recently received CloudMoney.</p>
        </asp:Panel>
    </div>
</asp:Content>
