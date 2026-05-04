<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="CloudMoney.Auth.Register" MasterPageFile="~/Site.Master" Title="Register - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="centered-form">
        <div class="card">
            <h2>Create an Account</h2>

            <div class="form-group">
                <label>Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" placeholder="Enter your full name" />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtFullName"
                    ErrorMessage="Full name is required." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="Enter your email address" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="Email is required." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Password (minimum 6 characters)</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter your password" />
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Confirm Password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder="Re-enter your password" />
                <asp:RequiredFieldValidator ID="rfvConfirm" runat="server" ControlToValidate="txtConfirmPassword"
                    ErrorMessage="Please confirm your password." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
                <asp:CompareValidator ID="cvPasswordMatch" runat="server" ControlToValidate="txtConfirmPassword"
                    ControlToCompare="txtPassword" ErrorMessage="Passwords do not match."
                    Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-success" OnClick="btnRegister_Click" style="width:100%;" />

            <div style="text-align:center; margin-top:18px; font-size:14px;">
                Already have an account? <a href="<%= ResolveUrl("~/Auth/Login.aspx") %>" style="color:#1a73e8;">Login here</a>
            </div>
        </div>
    </div>
</asp:Content>
