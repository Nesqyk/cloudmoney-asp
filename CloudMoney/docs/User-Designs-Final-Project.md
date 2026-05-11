# Final Project Documentation

**Name:** Tyrone Tabornal  
**Section:** BSIT-II  
**Subject Code:** [Insert Subject Code Here]

---

# User Designs of the Final Project

---

## 1. Site Master Page — Navigation Shell & Layout

**[Insert Figure 1: Site Master Page (Navigation Bar & Layout Shell) here]**

```html
<%@ Master Language="C#" AutoEventWireup="true"
    CodeBehind="Site.Master.cs" Inherits="CloudMoney.Site" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />
    <title><%= Page.Title %></title>
    <style>
        *, *::before, *::after {
            box-sizing: border-box; margin: 0; padding: 0;
        }
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana,
                         sans-serif;
            background: #f0f2f5;
            color: #333;
            min-height: 100vh;
            display: flex;
            flex-direction: column;
        }
        .header {
            background: linear-gradient(135deg, #1a73e8, #0d47a1);
            color: white;
            padding: 0 30px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.15);
            position: sticky;
            top: 0;
            z-index: 1000;
        }
        .header-inner {
            max-width: 1200px;
            margin: 0 auto;
            display: flex;
            justify-content: space-between;
            align-items: center;
            height: 60px;
        }
        .logo {
            font-size: 24px;
            font-weight: bold;
            letter-spacing: 0.5px;
        }
        .logo span { color: #64b5f6; }
        .nav {
            display: flex;
            gap: 4px;
            align-items: center;
        }
        .nav a {
            color: white;
            text-decoration: none;
            padding: 8px 14px;
            border-radius: 4px;
            transition: background 0.2s;
            font-size: 14px;
            white-space: nowrap;
        }
        .nav a:hover {
            background: rgba(255,255,255,0.15);
        }
        .nav .dropdown { position: relative; }
        .nav .dropdown-menu {
            display: none;
            position: absolute;
            top: 100%;
            left: 0;
            background: white;
            border-radius: 4px;
            box-shadow: 0 4px 16px rgba(0,0,0,0.15);
            min-width: 220px;
            z-index: 100;
        }
        .nav .dropdown:hover .dropdown-menu {
            display: block;
        }
        .nav .dropdown-menu a {
            color: #333;
            display: block;
            padding: 10px 16px;
            border-bottom: 1px solid #f0f0f0;
        }
        .nav .dropdown-menu a:hover {
            background: #e8f0fe;
            color: #1a73e8;
        }
        .main {
            max-width: 1200px;
            margin: 30px auto;
            padding: 0 20px;
            flex: 1;
            width: 100%;
        }
        .message {
            padding: 12px 16px;
            border-radius: 4px;
            margin-bottom: 16px;
            font-size: 14px;
        }
        .message-success {
            background: #e8f5e9;
            color: #2e7d32;
            border-left: 4px solid #43a047;
        }
        .message-error {
            background: #fdecea;
            color: #c62828;
            border-left: 4px solid #e53935;
        }
        .message-info {
            background: #e3f2fd;
            color: #1565c0;
            border-left: 4px solid #1e88e5;
        }
        .footer {
            text-align: center;
            color: #999;
            font-size: 12px;
            padding: 20px;
            border-top: 1px solid #e8eaed;
            margin-top: 40px;
        }
    </style>
    <asp:ContentPlaceHolder ID="HeadContent" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="header-inner">
                <div class="logo">Cloud<span>Money</span></div>
                <div class="nav">
                    <asp:PlaceHolder ID="phLoggedOut" runat="server">
                        <a href="~/Auth/Login.aspx">Login</a>
                        <a href="~/Auth/Register.aspx">Register</a>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phLoggedIn"
                        runat="server" Visible="false">
                        <a href="~/Dashboard.aspx">Dashboard</a>
                        <a href="~/Transactions/Deposit.aspx">
                            Deposit</a>
                        <a href="~/Transactions/Withdraw.aspx">
                            Withdraw</a>
                        <a href="~/Transactions/SendMoney.aspx">
                            Send Money</a>
                        <div class="dropdown">
                            <a href="#">Reports &#x25BE;</a>
                            <div class="dropdown-menu">
                                <a href="~/Reports/StatementOfAccount.aspx">
                                    Statement of Account</a>
                                <a href="~/Reports/MyDepositsWithdrawals.aspx">
                                    My Deposits / Withdrawals</a>
                                <a href="~/Reports/SentReceivedHistory.aspx">
                                    My Sent / Received</a>
                            </div>
                        </div>
                        <a href="~/Auth/ChangePassword.aspx">
                            Change Password</a>
                        <a href="~/Auth/Logout.aspx">Logout</a>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>

        <div class="main">
            <asp:Panel ID="pnlMessage" runat="server"
                Visible="false">
                <asp:Label ID="lblMessage" runat="server" />
            </asp:Panel>

            <asp:ContentPlaceHolder ID="MainContent"
                runat="server" />
        </div>

        <div class="footer">
            &copy; <%= DateTime.Now.Year %> CloudMoney.
            All rights reserved.
        </div>
    </form>
</body>
</html>
```

**Code Reference:** `Site.Master` — lines 1–55

The Site Master Page (`Site.Master`) serves as the common layout template for the entire CloudMoney application. It defines a sticky header with the **CloudMoney** branding and a responsive navigation bar. The navigation dynamically toggles between logged-out links (Login, Register) and logged-in links (Dashboard, Deposit, Withdraw, Send Money, a CSS-based Reports dropdown with three report options, Change Password, and Logout). A centralized message panel (`pnlMessage`) renders system feedback as success (green), error (red), or informational (blue) banners. The `MainContent` placeholder is where all content pages inject their specific UI. A minimal footer displays the copyright year.

---

## 2. Login Page

**[Insert Figure 2: Login Page here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="CloudMoney.Auth.Login"
    MasterPageFile="~/Site.Master"
    Title="Login - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .centered-form {
            max-width: 450px;
            margin: 60px auto;
        }
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .form-group { margin-bottom: 18px; }
        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .form-group input:focus {
            outline: none;
            border-color: #1a73e8;
            box-shadow: 0 0 0 3px
                        rgba(26,115,232,0.12);
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-primary {
            background: #1a73e8;
            color: white;
        }
        .btn-primary:hover {
            background: #1557b0;
            box-shadow: 0 2px 4px
                        rgba(26,115,232,0.3);
        }
    </style>

    <div class="centered-form">
        <div class="card">
            <h2>Account Login</h2>

            <div class="form-group">
                <label>Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server"
                    TextMode="Email"
                    placeholder="Enter your email address" />
                <asp:RequiredFieldValidator ID="rfvEmail"
                    runat="server"
                    ControlToValidate="txtEmail"
                    ErrorMessage="Email is required."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword"
                    runat="server" TextMode="Password"
                    placeholder="Enter your password" />
                <asp:RequiredFieldValidator
                    ID="rfvPassword" runat="server"
                    ControlToValidate="txtPassword"
                    ErrorMessage="Password is required."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <asp:Button ID="btnLogin" runat="server"
                Text="Login" CssClass="btn btn-primary"
                OnClick="btnLogin_Click"
                Style="width:100%;" />

            <div style="text-align:center; margin-top:18px;
                font-size:14px;">
                No account yet?
                <a href="~/Auth/Register.aspx"
                   style="color:#1a73e8;">
                   Register here</a>
            </div>
        </div>
    </div>
</asp:Content>
```

**Code Reference:** `Auth/Login.aspx` — lines 1–29

The Login Page presents a centered card-style form containing two input fields: **Email Address** (`TextMode="Email"`) and **Password** (`TextMode="Password"`), each guarded by a `RequiredFieldValidator` that displays inline red error text when the field is left blank. The **Login** button spans the full width of the card and triggers the `btnLogin_Click` server-side handler, which validates credentials against the database. A hyperlink at the bottom routes unregistered users to the Registration page.

---

## 3. Registration Page

**[Insert Figure 3: Registration Page here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="CloudMoney.Auth.Register"
    MasterPageFile="~/Site.Master"
    Title="Register - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .centered-form {
            max-width: 450px;
            margin: 60px auto;
        }
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .form-group { margin-bottom: 18px; }
        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .form-group input:focus {
            outline: none;
            border-color: #1a73e8;
            box-shadow: 0 0 0 3px
                        rgba(26,115,232,0.12);
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-success {
            background: #0d904f;
            color: white;
        }
        .btn-success:hover {
            background: #0a6e3c;
            box-shadow: 0 2px 4px
                        rgba(13,144,79,0.3);
        }
    </style>

    <div class="centered-form">
        <div class="card">
            <h2>Create an Account</h2>

            <div class="form-group">
                <label>Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server"
                    placeholder="Enter your full name" />
                <asp:RequiredFieldValidator ID="rfvName"
                    runat="server"
                    ControlToValidate="txtFullName"
                    ErrorMessage="Full name is required."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server"
                    TextMode="Email"
                    placeholder="Enter your email address" />
                <asp:RequiredFieldValidator ID="rfvEmail"
                    runat="server"
                    ControlToValidate="txtEmail"
                    ErrorMessage="Email is required."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Password (minimum 6 characters)</label>
                <asp:TextBox ID="txtPassword"
                    runat="server" TextMode="Password"
                    placeholder="Enter your password" />
                <asp:RequiredFieldValidator
                    ID="rfvPassword" runat="server"
                    ControlToValidate="txtPassword"
                    ErrorMessage="Password is required."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Confirm Password</label>
                <asp:TextBox ID="txtConfirmPassword"
                    runat="server" TextMode="Password"
                    placeholder="Re-enter your password" />
                <asp:RequiredFieldValidator
                    ID="rfvConfirm" runat="server"
                    ControlToValidate="txtConfirmPassword"
                    ErrorMessage="Please confirm your password."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
                <asp:CompareValidator
                    ID="cvPasswordMatch" runat="server"
                    ControlToValidate="txtConfirmPassword"
                    ControlToCompare="txtPassword"
                    ErrorMessage="Passwords do not match."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <asp:Button ID="btnRegister" runat="server"
                Text="Register" CssClass="btn btn-success"
                OnClick="btnRegister_Click"
                style="width:100%;" />

            <div style="text-align:center; margin-top:18px;
                font-size:14px;">
                Already have an account?
                <a href="~/Auth/Login.aspx"
                   style="color:#1a73e8;">
                   Login here</a>
            </div>
        </div>
    </div>
</asp:Content>
```

**Code Reference:** `Auth/Register.aspx` — lines 1–46

The Registration Page collects four pieces of user data: **Full Name**, **Email Address** (with HTML5 email validation), **Password** (minimum 6 characters), and **Confirm Password**. All fields are protected by `RequiredFieldValidator` controls; the password confirmation field additionally uses a `CompareValidator` that ensures the two password entries match before the form can be submitted. The **Register** button, styled with a green accent (`btn-success`), calls the `btnRegister_Click` server method which generates a unique 10-digit account number, inserts the user record, and displays the assigned account number upon success.

---

## 4. Dashboard Page

**[Insert Figure 4: Dashboard Page here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="CloudMoney.Dashboard"
    MasterPageFile="~/Site.Master"
    Title="Dashboard - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
            margin-bottom: 24px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .card h3 {
            color: #333;
            margin-bottom: 12px;
            font-size: 18px;
        }
        .dashboard-grid {
            display: grid;
            grid-template-columns:
                repeat(auto-fit, minmax(260px, 1fr));
            gap: 20px;
            margin-bottom: 24px;
        }
        .stat-card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 24px;
            text-align: center;
        }
        .stat-card .label {
            font-size: 13px;
            color: #666;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-bottom: 8px;
        }
        .stat-card .value {
            font-size: 28px;
            font-weight: bold;
            color: #1a73e8;
        }
        .stat-card .value.balance {
            color: #0d904f;
        }
        .stat-card .value.sent {
            color: #e37400;
        }
        .stat-card.accent-blue {
            border-top: 3px solid #1a73e8;
        }
        .stat-card.accent-purple {
            border-top: 3px solid #7b1fa2;
        }
        .stat-card.accent-green {
            border-top: 3px solid #0d904f;
        }
        .stat-card.accent-orange {
            border-top: 3px solid #e37400;
        }
        .info-row {
            display: flex;
            padding: 10px 0;
            border-bottom: 1px solid #f0f0f0;
        }
        .info-row .info-label {
            font-weight: 600;
            color: #555;
            min-width: 170px;
            font-size: 14px;
        }
        .info-row .info-value {
            color: #333;
            font-size: 14px;
        }
        .notification {
            background: #e8f5e9;
            border-left: 4px solid #43a047;
            padding: 12px 16px;
            margin-bottom: 8px;
            border-radius: 4px;
            font-size: 14px;
        }
        .notification .notif-date {
            color: #666;
            font-size: 12px;
            margin-top: 4px;
        }
        .notification .notif-amount {
            font-weight: bold;
            color: #2e7d32;
        }
        .empty-state {
            text-align: center;
            padding: 30px;
            color: #999;
            font-size: 14px;
        }
    </style>

    <div class="card">
        <h2>Account Dashboard</h2>

        <div class="dashboard-grid">
            <div class="stat-card accent-blue">
                <div class="label">Account Number</div>
                <div class="value">
                    <asp:Label ID="lblAccountNumber"
                        runat="server" />
                </div>
            </div>
            <div class="stat-card accent-purple">
                <div class="label">Full Name</div>
                <div class="value" style="font-size:20px;">
                    <asp:Label ID="lblFullName"
                        runat="server" />
                </div>
            </div>
            <div class="stat-card accent-green">
                <div class="label">Total Current Balance</div>
                <div class="value balance">
                    <asp:Label ID="lblCurrentBalance"
                        runat="server" />
                </div>
            </div>
            <div class="stat-card accent-orange">
                <div class="label">Total Amount Sent</div>
                <div class="value sent">
                    <asp:Label ID="lblTotalSent"
                        runat="server" />
                </div>
            </div>
        </div>

        <div class="info-row">
            <div class="info-label">Date Registered</div>
            <div class="info-value">
                <asp:Label ID="lblDateRegistered"
                    runat="server" />
            </div>
        </div>
    </div>

    <div class="card">
        <h3>Recent Received CloudMoney</h3>
        <asp:Repeater ID="repNotifications"
            runat="server">
            <ItemTemplate>
                <div class="notification">
                    You received
                    <span class="notif-amount">
                        P<%# Eval("Amount", "{0:N2}") %>
                    </span>
                    from <%# Eval("SenderFullName") %>
                    (Acct: <%# Eval("SenderAccountNumber") %>)
                    <div class="notif-date">
                        <%# Eval("TransactionDate",
                            "{0:MMM dd, yyyy hh:mm tt}") %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlNoNotifications" runat="server"
            CssClass="empty-state" Visible="false">
            <p>No recently received CloudMoney.</p>
        </asp:Panel>
    </div>
</asp:Content>
```

**Code Reference:** `Dashboard.aspx` — lines 1–48

The Dashboard is the central hub presented immediately after login. It displays four **stat-card** panels arranged in a responsive CSS Grid: Account Number (blue accent), Full Name (purple accent), Total Current Balance (green accent), and Total Amount Sent (orange accent). Each stat card features an uppercase label and a large bold value. Below the cards, an info-row displays the user's registration date. A second card section renders the five most recent incoming transfers using an `asp:Repeater` control where each notification card shows the sender name, sender account number, amount received, and transaction timestamp. An `asp:Panel` with the `empty-state` CSS class is shown when no recent transfers exist.

---

## 5. Deposit Page

**[Insert Figure 5: Deposit Page here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Deposit.aspx.cs"
    Inherits="CloudMoney.Transactions.Deposit"
    MasterPageFile="~/Site.Master"
    Title="Deposit - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .centered-form {
            max-width: 450px;
            margin: 60px auto;
        }
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .balance-box {
            background: linear-gradient(135deg, #e8f5e9, #c8e6c9);
            border-radius: 8px;
            padding: 18px 24px;
            text-align: center;
            margin-bottom: 20px;
        }
        .balance-box .balance-label {
            font-size: 13px;
            color: #2e7d32;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .balance-box .balance-value {
            font-size: 32px;
            font-weight: bold;
            color: #1b5e20;
            margin-top: 4px;
        }
        .form-group { margin-bottom: 18px; }
        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .form-group input:focus {
            outline: none;
            border-color: #1a73e8;
            box-shadow: 0 0 0 3px
                        rgba(26,115,232,0.12);
        }
        .form-group small {
            color: #666;
            font-size: 12px;
            display: block;
            margin-top: 6px;
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-success {
            background: #0d904f;
            color: white;
        }
        .btn-success:hover {
            background: #0a6e3c;
            box-shadow: 0 2px 4px
                        rgba(13,144,79,0.3);
        }
    </style>

    <div class="centered-form">
        <div class="card">
            <h2>Deposit</h2>

            <div class="balance-box">
                <div class="balance-label">Current Balance</div>
                <div class="balance-value">
                    <asp:Label ID="lblCurrentBalance"
                        runat="server" />
                </div>
            </div>

            <div class="form-group">
                <label>Deposit Amount</label>
                <asp:TextBox ID="txtAmount" runat="server"
                    placeholder="Enter amount (100 - 2,000)" />
                <small>Amount must be between P100 and P2,000
                    and divisible by 100.</small>
            </div>

            <asp:Button ID="btnDeposit" runat="server"
                CssClass="btn btn-success" Text="Deposit"
                OnClick="btnDeposit_Click"
                Style="width:100%;" />
        </div>
    </div>
</asp:Content>
```

**Code Reference:** `Transactions/Deposit.aspx` — lines 1–22

The Deposit Page displays the user's current balance in a green gradient **balance-box** with a large bold monetary value. A single text input accepts the deposit amount, accompanied by a hint (`<small>`) specifying business rules: amounts must fall between P100 and P2,000 and must be divisible by 100. The **Deposit** button (`btn-success`) triggers `btnDeposit_Click`, which calls the `TransactionManager.Deposit()` method. Server-side validation enforces the minimum/maximum amount rules and a total balance cap of P10,000. On success, a new deposit record is inserted into the `Transactions` table and the updated balance is rendered.

---

## 6. Withdraw Page

**[Insert Figure 6: Withdraw Page here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Withdraw.aspx.cs"
    Inherits="CloudMoney.Transactions.Withdraw"
    MasterPageFile="~/Site.Master"
    Title="Withdraw - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .centered-form {
            max-width: 450px;
            margin: 60px auto;
        }
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .balance-box {
            background: linear-gradient(135deg, #e8f5e9, #c8e6c9);
            border-radius: 8px;
            padding: 18px 24px;
            text-align: center;
            margin-bottom: 20px;
        }
        .balance-box .balance-label {
            font-size: 13px;
            color: #2e7d32;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .balance-box .balance-value {
            font-size: 32px;
            font-weight: bold;
            color: #1b5e20;
            margin-top: 4px;
        }
        .form-group { margin-bottom: 18px; }
        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .form-group input:focus {
            outline: none;
            border-color: #1a73e8;
            box-shadow: 0 0 0 3px
                        rgba(26,115,232,0.12);
        }
        .form-group small {
            color: #666;
            font-size: 12px;
            display: block;
            margin-top: 6px;
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-danger {
            background: #d93025;
            color: white;
        }
        .btn-danger:hover {
            background: #b3261e;
        }
    </style>

    <div class="centered-form">
        <div class="card">
            <h2>Withdraw</h2>

            <div class="balance-box">
                <div class="balance-label">Current Balance</div>
                <div class="balance-value">
                    <asp:Label ID="lblCurrentBalance"
                        runat="server" />
                </div>
            </div>

            <div class="form-group">
                <label>Withdrawal Amount</label>
                <asp:TextBox ID="txtAmount" runat="server"
                    placeholder="Enter amount (100 - 2,000)" />
                <small>Amount must be between P100 and P2,000
                    and divisible by 100.</small>
            </div>

            <asp:Button ID="btnWithdraw" runat="server"
                CssClass="btn btn-danger" Text="Withdraw"
                OnClick="btnWithdraw_Click"
                Style="width:100%;" />
        </div>
    </div>
</asp:Content>
```

**Code Reference:** `Transactions/Withdraw.aspx` — lines 1–22

The Withdraw Page mirrors the Deposit Page structurally with the same balance-box displaying the current balance and the same amount-input layout. The key visual distinction is the **Withdraw** button, styled with a red accent (`btn-danger`) to signal the outgoing nature of the transaction. Server-side validation in `btnWithdraw_Click` ensures the withdrawal amount adheres to the P100–P2,000 range and divisibility-by-100 rule, and checks that sufficient balance exists. On success, a withdrawal record is inserted and the balance is recalculated from the ledger.

---

## 7. Send Money Page — Two-Step Transfer

**[Insert Figure 7: Send Money Page (Step 1 — Recipient Verification) here]**

**[Insert Figure 8: Send Money Page (Step 2 — Amount & Confirmation) here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="SendMoney.aspx.cs"
    Inherits="CloudMoney.Transactions.SendMoney"
    MasterPageFile="~/Site.Master"
    Title="Send Money - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .centered-form {
            max-width: 560px;
            margin: 60px auto;
        }
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .form-group { margin-bottom: 18px; }
        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .form-group input:focus {
            outline: none;
            border-color: #1a73e8;
            box-shadow: 0 0 0 3px
                        rgba(26,115,232,0.12);
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-secondary {
            background: #5f6368;
            color: white;
        }
        .btn-secondary:hover { background: #444; }
        .btn-primary {
            background: #1a73e8;
            color: white;
        }
        .btn-primary:hover {
            background: #1557b0;
            box-shadow: 0 2px 4px
                        rgba(26,115,232,0.3);
        }
        .recipient-card {
            background: #e8f0fe;
            border-radius: 8px;
            padding: 16px 20px;
            margin: 16px 0;
            border-left: 4px solid #1a73e8;
        }
        .recipient-card .rec-label {
            font-size: 12px;
            color: #666;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .recipient-card .rec-value {
            font-size: 16px;
            font-weight: 600;
            color: #1a73e8;
            margin-top: 4px;
        }
    </style>

    <div class="centered-form">
        <div class="card">
            <h2>Send CloudMoney</h2>

            <%-- Step 1: Recipient Lookup --%>
            <div class="form-group">
                <label>Recipient Account Number</label>
                <asp:TextBox ID="txtRecipientAccount"
                    runat="server"
                    placeholder="Enter recipient account number" />
            </div>

            <asp:Button ID="btnVerifyRecipient" runat="server"
                Text="Verify Recipient"
                CssClass="btn btn-secondary"
                OnClick="btnVerifyRecipient_Click" />

            <%-- Step 2: Transfer Details (revealed after verification) --%>
            <asp:Panel ID="pnlRecipientDetails" runat="server"
                Visible="false">
                <div class="recipient-card">
                    <div class="rec-label">
                        Recipient Account Number</div>
                    <div class="rec-value">
                        <asp:Label ID="lblRecipientAccount"
                            runat="server" />
                    </div>
                    <div class="rec-label" style="margin-top:10px;">
                        Recipient Full Name</div>
                    <div class="rec-value">
                        <asp:Label ID="lblRecipientName"
                            runat="server" />
                    </div>
                </div>

                <asp:HiddenField ID="hfRecipientUserId"
                    runat="server" />

                <div class="form-group">
                    <label>Amount to Send</label>
                    <asp:TextBox ID="txtAmount" runat="server"
                        placeholder="Enter amount (100 - 2,000)" />
                </div>

                <div class="form-group">
                    <label>Confirm Your Password</label>
                    <asp:TextBox ID="txtPasswordConfirm"
                        runat="server" TextMode="Password"
                        placeholder="Re-enter your password" />
                </div>

                <asp:Button ID="btnSendMoney" runat="server"
                    Text="Send CloudMoney"
                    CssClass="btn btn-primary"
                    OnClick="btnSendMoney_Click"
                    Style="width:100%;" />
            </asp:Panel>
        </div>
    </div>
</asp:Content>
```

**Code Reference:** `Transactions/SendMoney.aspx` — lines 1–39

The Send Money page implements a **two-step transfer workflow**. In Step 1, the user enters a recipient's account number and clicks **Verify Recipient** (`btn-secondary`). The server-side `btnVerifyRecipient_Click` handler looks up the account in the `Users` table and, if found, sets `pnlRecipientDetails.Visible = true` to reveal Step 2 controls. The revealed **recipient-card** (blue left-border, light-blue background) confirms the recipient's account number and full name. A `HiddenField` (`hfRecipientUserId`) carries the recipient's internal user ID between postbacks. Step 2 collects the transfer **Amount** and requires the sender's **Password** for re-authentication via `txtPasswordConfirm`. The final **Send CloudMoney** button (`btn-primary`) executes the transfer inside a `SqlTransaction` to guarantee atomicity: it re-verifies the recipient, re-checks sender balance, inserts a `Transfer` record with both `FromUserId` and `ToUserId` populated, and commits or rolls back as a single unit. Self-transfers are programmatically prevented.

---

## 8. Statement of Account Report

**[Insert Figure 9: Statement of Account Report here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="StatementOfAccount.aspx.cs"
    Inherits="CloudMoney.Reports.StatementOfAccount"
    MasterPageFile="~/Site.Master"
    Title="Statement of Account - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .inline-form {
            display: flex;
            gap: 12px;
            align-items: flex-end;
            flex-wrap: wrap;
        }
        .inline-form .form-group {
            margin-bottom: 0;
        }
        .inline-form label {
            margin-bottom: 4px;
            font-size: 13px;
        }
        .inline-form input,
        .inline-form select {
            width: auto;
            min-width: 130px;
        }
        .form-group { margin-bottom: 18px; }
        .form-group label {
            display: block;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input,
        .form-group select {
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-primary {
            background: #1a73e8;
            color: white;
        }
        .btn-primary:hover {
            background: #1557b0;
            box-shadow: 0 2px 4px
                        rgba(26,115,232,0.3);
        }
        .report {
            width: 100%;
            border-collapse: collapse;
            margin-top: 12px;
            font-size: 14px;
        }
        .report th {
            background: #1a73e8;
            color: white;
            padding: 12px 14px;
            text-align: left;
            font-weight: 600;
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 0.3px;
        }
        .report td {
            padding: 10px 14px;
            border-bottom: 1px solid #e8eaed;
        }
        .report tbody tr:hover {
            background: #f1f5ff;
        }
        .report tbody tr:nth-child(even) {
            background: #fafbfc;
        }
    </style>

    <div class="card">
        <h2>Statement of Account</h2>

        <div class="inline-form">
            <div class="form-group">
                <label>Start Date</label>
                <asp:TextBox ID="txtStartDate" runat="server"
                    TextMode="Date" />
            </div>
            <div class="form-group">
                <label>End Date</label>
                <asp:TextBox ID="txtEndDate" runat="server"
                    TextMode="Date" />
            </div>
            <div class="form-group">
                <asp:Button ID="btnFilter" runat="server"
                    Text="Generate Report"
                    CssClass="btn btn-primary"
                    OnClick="btnFilter_Click" />
            </div>
        </div>

        <asp:GridView ID="gvStatement" runat="server"
            AutoGenerateColumns="false"
            CssClass="report"
            EmptyDataText="No transactions found
                for this range.">
            <Columns>
                <asp:BoundField DataField="TransactionDate"
                    HeaderText="Date"
                    DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="TransactionType"
                    HeaderText="Type" />
                <asp:BoundField DataField="Direction"
                    HeaderText="Direction" />
                <asp:BoundField DataField="Amount"
                    HeaderText="Amount"
                    DataFormatString="P {0:N2}" />
                <asp:BoundField DataField="SenderAccount"
                    HeaderText="Sender Account" />
                <asp:BoundField DataField="SenderName"
                    HeaderText="Sender Name" />
                <asp:BoundField DataField="RecipientAccount"
                    HeaderText="Recipient Account" />
                <asp:BoundField DataField="RecipientName"
                    HeaderText="Recipient Name" />
                <asp:BoundField DataField="Remarks"
                    HeaderText="Remarks" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
```

**Code Reference:** `Reports/StatementOfAccount.aspx` — lines 1–35

The Statement of Account report displays a complete, filterable transaction history. An **inline-form** row at the top provides `TextMode="Date"` inputs for **Start Date** and **End Date**, alongside a **Generate Report** button (`btn-primary`). When clicked, the `btnFilter_Click` handler executes a SQL query with a `LEFT JOIN` on the `Users` table (joined twice — once for the sender, once for the recipient), filtered by date range using an exclusive end-date pattern. The result set binds to an `asp:GridView` with nine columns: Date (formatted as `yyyy-MM-dd HH:mm`), Type (Deposit/Withdrawal/Transfer), Direction (Cash In/Cash Out/Sent/Received), Amount (prefixed with "P"), Sender Account, Sender Name, Recipient Account, Recipient Name, and Remarks. The table header uses a blue background with white uppercase text; alternating rows have a lighter background, and hover highlights rows in a pale blue. An `EmptyDataText` message displays when no records match the filter.

---

## 9. My Deposits & Withdrawals Report

**[Insert Figure 10: My Deposits & Withdrawals Report here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="MyDepositsWithdrawals.aspx.cs"
    Inherits="CloudMoney.Reports.MyDepositsWithdrawals"
    MasterPageFile="~/Site.Master"
    Title="My Deposits / Withdrawals - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .inline-form {
            display: flex;
            gap: 12px;
            align-items: flex-end;
            flex-wrap: wrap;
        }
        .inline-form .form-group {
            margin-bottom: 0;
        }
        .inline-form label {
            margin-bottom: 4px;
            font-size: 13px;
        }
        .inline-form input,
        .inline-form select {
            width: auto;
            min-width: 130px;
        }
        .form-group label {
            display: block;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input,
        .form-group select {
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-primary {
            background: #1a73e8;
            color: white;
        }
        .btn-primary:hover {
            background: #1557b0;
            box-shadow: 0 2px 4px
                        rgba(26,115,232,0.3);
        }
        .report {
            width: 100%;
            border-collapse: collapse;
            margin-top: 12px;
            font-size: 14px;
        }
        .report th {
            background: #1a73e8;
            color: white;
            padding: 12px 14px;
            text-align: left;
            font-weight: 600;
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 0.3px;
        }
        .report td {
            padding: 10px 14px;
            border-bottom: 1px solid #e8eaed;
        }
        .report tbody tr:hover {
            background: #f1f5ff;
        }
        .report tbody tr:nth-child(even) {
            background: #fafbfc;
        }
    </style>

    <div class="card">
        <h2>My Deposits / Withdrawals</h2>

        <div class="inline-form">
            <div class="form-group">
                <label>Start Date</label>
                <asp:TextBox ID="txtStartDate" runat="server"
                    TextMode="Date" />
            </div>
            <div class="form-group">
                <label>End Date</label>
                <asp:TextBox ID="txtEndDate" runat="server"
                    TextMode="Date" />
            </div>
            <div class="form-group">
                <label>Type</label>
                <asp:DropDownList ID="ddlType" runat="server">
                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Deposit">
                        Deposit</asp:ListItem>
                    <asp:ListItem Value="Withdrawal">
                        Withdrawal</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Button ID="btnFilter" runat="server"
                    Text="Generate Report"
                    CssClass="btn btn-primary"
                    OnClick="btnFilter_Click" />
            </div>
        </div>

        <asp:GridView ID="gvTransactions" runat="server"
            AutoGenerateColumns="false"
            CssClass="report"
            EmptyDataText="No deposit or withdrawal
                records found for this range.">
            <Columns>
                <asp:BoundField DataField="TransactionDate"
                    HeaderText="Date"
                    DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="TransactionType"
                    HeaderText="Type" />
                <asp:BoundField DataField="Amount"
                    HeaderText="Amount"
                    DataFormatString="P {0:N2}" />
                <asp:BoundField DataField="Remarks"
                    HeaderText="Remarks" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
```

**Code Reference:** `Reports/MyDepositsWithdrawals.aspx` — lines 1–38

This report filters the transaction ledger to show only **Deposits** and **Withdrawals**, excluding peer-to-peer transfers. The filter bar adds a third control: a `DropDownList` (`ddlType`) with three options — All, Deposit, and Withdrawal — placed alongside the Start Date and End Date inputs. The `btnFilter_Click` handler builds a SQL query filtered by date range and optionally by `TransactionType` (when "All" is not selected). The resulting `GridView` displays four columns: Date, Type, Amount, and Remarks. The table shares the same `.report` styling. When no records are found, the `EmptyDataText` message is displayed.

---

## 10. Sent/Received History Report

**[Insert Figure 11: Sent/Received History Report here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="SentReceivedHistory.aspx.cs"
    Inherits="CloudMoney.Reports.SentReceivedHistory"
    MasterPageFile="~/Site.Master"
    Title="My Sent / Received - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .inline-form {
            display: flex;
            gap: 12px;
            align-items: flex-end;
            flex-wrap: wrap;
        }
        .inline-form .form-group {
            margin-bottom: 0;
        }
        .inline-form label {
            margin-bottom: 4px;
            font-size: 13px;
        }
        .inline-form input,
        .inline-form select {
            width: auto;
            min-width: 130px;
        }
        .form-group label {
            display: block;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input,
        .form-group select {
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-primary {
            background: #1a73e8;
            color: white;
        }
        .btn-primary:hover {
            background: #1557b0;
            box-shadow: 0 2px 4px
                        rgba(26,115,232,0.3);
        }
        .report {
            width: 100%;
            border-collapse: collapse;
            margin-top: 12px;
            font-size: 14px;
        }
        .report th {
            background: #1a73e8;
            color: white;
            padding: 12px 14px;
            text-align: left;
            font-weight: 600;
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 0.3px;
        }
        .report td {
            padding: 10px 14px;
            border-bottom: 1px solid #e8eaed;
        }
        .report tbody tr:hover {
            background: #f1f5ff;
        }
        .report tbody tr:nth-child(even) {
            background: #fafbfc;
        }
    </style>

    <div class="card">
        <h2>My Sent / Received Transactions</h2>

        <div class="inline-form">
            <div class="form-group">
                <label>Start Date</label>
                <asp:TextBox ID="txtStartDate" runat="server"
                    TextMode="Date" />
            </div>
            <div class="form-group">
                <label>End Date</label>
                <asp:TextBox ID="txtEndDate" runat="server"
                    TextMode="Date" />
            </div>
            <div class="form-group">
                <label>Type</label>
                <asp:DropDownList ID="ddlType" runat="server">
                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Sent">Sent</asp:ListItem>
                    <asp:ListItem Value="Received">
                        Received</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Button ID="btnFilter" runat="server"
                    Text="Generate Report"
                    CssClass="btn btn-primary"
                    OnClick="btnFilter_Click" />
            </div>
        </div>

        <asp:GridView ID="gvTransfers" runat="server"
            AutoGenerateColumns="false"
            CssClass="report"
            EmptyDataText="No sent or received transfer
                records found for this range.">
            <Columns>
                <asp:BoundField DataField="TransactionDate"
                    HeaderText="Date"
                    DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="TransferType"
                    HeaderText="Transfer Type" />
                <asp:BoundField DataField="Amount"
                    HeaderText="Amount"
                    DataFormatString="P {0:N2}" />
                <asp:BoundField
                    DataField="CounterpartyAccount"
                    HeaderText="Counterparty Account" />
                <asp:BoundField
                    DataField="CounterpartyName"
                    HeaderText="Counterparty Name" />
                <asp:BoundField DataField="Remarks"
                    HeaderText="Remarks" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
```

**Code Reference:** `Reports/SentReceivedHistory.aspx` — lines 1–40

This report displays only **Transfer**-type transactions with full counterparty information. The `ddlType` dropdown allows filtering by Sent, Received, or All. The underlying SQL query uses an `INNER JOIN` to the `Users` table to retrieve the counterparty's account number and full name for each transfer. The `GridView` presents six columns: Date, Transfer Type (Sent/Received), Amount, Counterparty Account, Counterparty Name, and Remarks.

---

## 11. Change Password Page

**[Insert Figure 12: Change Password Page here]**

```html
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs"
    Inherits="CloudMoney.Auth.ChangePassword"
    MasterPageFile="~/Site.Master"
    Title="Change Password - CloudMoney" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .centered-form {
            max-width: 450px;
            margin: 60px auto;
        }
        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 30px;
        }
        .card h2 {
            color: #1a73e8;
            margin-bottom: 20px;
            font-size: 22px;
            border-bottom: 2px solid #e8f0fe;
            padding-bottom: 12px;
        }
        .form-group { margin-bottom: 18px; }
        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: 600;
            color: #444;
            font-size: 14px;
        }
        .form-group input {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #d0d5dd;
            border-radius: 4px;
            font-size: 14px;
            font-family: inherit;
        }
        .form-group input:focus {
            outline: none;
            border-color: #1a73e8;
            box-shadow: 0 0 0 3px
                        rgba(26,115,232,0.12);
        }
        .btn {
            padding: 10px 24px;
            border: none;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            font-family: inherit;
        }
        .btn-primary {
            background: #1a73e8;
            color: white;
        }
        .btn-primary:hover {
            background: #1557b0;
            box-shadow: 0 2px 4px
                        rgba(26,115,232,0.3);
        }
    </style>

    <div class="centered-form">
        <div class="card">
            <h2>Change Password</h2>

            <div class="form-group">
                <label>Current Password</label>
                <asp:TextBox ID="txtCurrentPassword"
                    runat="server" TextMode="Password"
                    placeholder="Enter current password" />
                <asp:RequiredFieldValidator ID="rfvCurrent"
                    runat="server"
                    ControlToValidate="txtCurrentPassword"
                    ErrorMessage="Current password is required."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>New Password</label>
                <asp:TextBox ID="txtNewPassword"
                    runat="server" TextMode="Password"
                    placeholder="Enter new password" />
                <asp:RequiredFieldValidator ID="rfvNew"
                    runat="server"
                    ControlToValidate="txtNewPassword"
                    ErrorMessage="New password is required."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <div class="form-group">
                <label>Confirm New Password</label>
                <asp:TextBox ID="txtConfirmNewPassword"
                    runat="server" TextMode="Password"
                    placeholder="Confirm new password" />
                <asp:RequiredFieldValidator ID="rfvConfirm"
                    runat="server"
                    ControlToValidate="txtConfirmNewPassword"
                    ErrorMessage="Password confirmation
                        is required."
                    Display="Dynamic" ForeColor="#d93025"
                    Font-Size="12px" />
            </div>

            <asp:Button ID="btnChangePassword" runat="server"
                Text="Update Password"
                CssClass="btn btn-primary"
                OnClick="btnChangePassword_Click"
                Style="width:100%;" />
        </div>
    </div>
</asp:Content>
```

**Code Reference:** `Auth/ChangePassword.aspx` — lines 1–32

The Change Password page requires three password fields: **Current Password** (verified against the stored value), **New Password**, and **Confirm New Password**. All three are protected by `RequiredFieldValidator` controls and rendered as `TextMode="Password"` for input masking. The **Update Password** button (`btn-primary`) triggers `btnChangePassword_Click`, which calls `AuthManager.ChangePassword()`. This method validates the current password against the database, confirms the new password length, and updates the `PasswordPlainText` column. The page requires an active session (authenticated user); unauthenticated access is redirected.

---

## Summary of UI Components

| Figure | Component | File Path |
|--------|-----------|-----------|
| 1 | Site Master Page (Navigation Shell) | `Site.Master` |
| 2 | Login Page | `Auth/Login.aspx` |
| 3 | Registration Page | `Auth/Register.aspx` |
| 4 | Dashboard Page | `Dashboard.aspx` |
| 5 | Deposit Page | `Transactions/Deposit.aspx` |
| 6 | Withdraw Page | `Transactions/Withdraw.aspx` |
| 7–8 | Send Money Page (Two-Step) | `Transactions/SendMoney.aspx` |
| 9 | Statement of Account Report | `Reports/StatementOfAccount.aspx` |
| 10 | My Deposits & Withdrawals Report | `Reports/MyDepositsWithdrawals.aspx` |
| 11 | Sent/Received History Report | `Reports/SentReceivedHistory.aspx` |
| 12 | Change Password Page | `Auth/ChangePassword.aspx` |
