<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CloudMoney.Auth.Login" MasterPageFile="~/Site.Master" Title="Login - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="centered-form">
        <div class="card">
            <h2>Account Login</h2>

            <div class="form-group">
                <label>Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="Enter your email address" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="Email is required." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter your password" />
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary" OnClick="btnLogin_Click" Style="width:100%;" />

            <div style="text-align:center; margin-top:18px; font-size:14px;">
                No account yet? <a href="<%= ResolveUrl("~/Auth/Register.aspx") %>" style="color:#1a73e8;">Register here</a>
            </div>
        </div>
    </div>
</asp:Content>
