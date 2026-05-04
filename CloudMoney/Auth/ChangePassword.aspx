<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="CloudMoney.Auth.ChangePassword" MasterPageFile="~/Site.Master" Title="Change Password - CloudMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="centered-form">
        <div class="card">
            <h2>Change Password</h2>

            <div class="form-group">
                <label>Current Password</label>
                <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" placeholder="Enter current password" />
                <asp:RequiredFieldValidator ID="rfvCurrent" runat="server" ControlToValidate="txtCurrentPassword"
                    ErrorMessage="Current password is required." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>New Password</label>
                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" placeholder="Enter new password" />
                <asp:RequiredFieldValidator ID="rfvNew" runat="server" ControlToValidate="txtNewPassword"
                    ErrorMessage="New password is required." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Confirm New Password</label>
                <asp:TextBox ID="txtConfirmNewPassword" runat="server" TextMode="Password" placeholder="Confirm new password" />
                <asp:RequiredFieldValidator ID="rfvConfirm" runat="server" ControlToValidate="txtConfirmNewPassword"
                    ErrorMessage="Password confirmation is required." Display="Dynamic" ForeColor="#d93025" Font-Size="12px" />
            </div>

            <asp:Button ID="btnChangePassword" runat="server" Text="Update Password" CssClass="btn btn-primary" OnClick="btnChangePassword_Click" Style="width:100%;" />
        </div>
    </div>
</asp:Content>
