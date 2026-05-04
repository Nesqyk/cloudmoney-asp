# CloudMoney — ASP.NET WebForms Digital Wallet Application

CloudMoney is a full-stack digital wallet web application built with **ASP.NET WebForms (.NET Framework 4.7.2)** and **SQL Server LocalDB**. It allows users to register an account, log in, deposit and withdraw funds, send money to other users, view transaction history reports, and manage their password — all within a consistent, master-page-driven UI.

This README serves a dual purpose:

1. **Functional documentation** — covers every feature and every file in the project.
2. **Learning resource** — explains the WebForms-specific patterns (page lifecycle, server controls, validation, code-behind, ViewState, postback model) in context so that a developer new to either this codebase or to WebForms can ramp up quickly.

---

## Table of Contents

| Section |
|---|
| [1. High-Level Architecture](#1-high-level-architecture) |
| [2. Technology Stack](#2-technology-stack) |
| [3. Directory Structure](#3-directory-structure) |
| [4. The WebForms Page Model: Concepts You Must Know](#4-the-webforms-page-model-concepts-you-must-know) |
| [5. Master Page: Site.Master](#5-master-page-sitemaster) |
| [6. Global.asax and Application Startup](#6-globalasax-and-application-startup) |
| [7. Database Design](#7-database-design) |
| [8. Data Access Layer: DatabaseHelper](#8-data-access-layer-databasehelper) |
| [9. Authentication & Session Management](#9-authentication--session-management) |
| [10. Result Data Classes](#10-result-data-classes) |
| [11. Validation Layer](#11-validation-layer) |
| [12. Module Walkthroughs](#12-module-walkthroughs) |
| [13. Business Logic Layer Deep Dive](#13-business-logic-layer-deep-dive) |
| [14. CSS & Styling Architecture](#14-css--styling-architecture) |
| [15. JavaScript Strategy](#15-javascript-strategy) |
| [16. Build, Run & Verify](#16-build-run--verify) |
| [17. Security Considerations](#17-security-considerations) |
| [18. Learning Path: How to Read This Codebase](#18-learning-path-how-to-read-this-codebase) |

---

## 1. High-Level Architecture

CloudMoney follows a clean three-tier layered architecture, implemented entirely within a single Visual Studio project:

```
┌─────────────────────────────────────────────────┐
│  Presentation Layer (.aspx / code-behind)       │
│  ├── Site.Master       (shared layout & nav)    │
│  ├── Auth/*            (Login, Register, etc.)  │
│  ├── Dashboard.aspx    (home screen)            │
│  ├── Transactions/*    (Deposit, Withdraw, Send)│
│  └── Reports/*         (GridView-based reports) │
└──────────────┬──────────────────────────────────┘
               │ calls static methods in
┌──────────────▼──────────────────────────────────┐
│  Business Logic Layer (Logic/*.cs)              │
│  ├── AuthManager         (registration, login)  │
│  ├── TransactionManager  (balance, transfers)   │
│  ├── ReportManager       (history queries)      │
│  ├── ValidationHelper    (amount/date rules)    │
│  └── SessionHelper       (session abstraction)  │
└──────────────┬──────────────────────────────────┘
               │ calls static methods in
┌──────────────▼──────────────────────────────────┐
│  Data Access Layer                              │
│  └── DatabaseHelper      (SQL + ADO.NET)        │
└──────────────┬──────────────────────────────────┘
               │ connects to
┌──────────────▼──────────────────────────────────┐
│  SQL Server LocalDB (.mdf file in App_Data)     │
│  ├── dbo.Users                                  │
│  └── dbo.Transactions                           │
└─────────────────────────────────────────────────┘
```

**Key architectural decisions and why they were made:**

- **All Logic-layer classes are `static`.** In a real-world app you might use dependency injection with interfaces, but for a project of this size, static classes eliminate the need for object instantiation and keep call sites terse. Each code-behind method is typically a one-liner: `AuthManager.Login(email, password)`.
- **No ORM (Entity Framework, Dapper, etc.).** The data layer uses raw parameterized ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`). This keeps the project dependency-light and means you can see exactly what SQL is being executed at every point — valuable for learning.
- **No stored procedures.** All SQL is inline in C# strings. While stored procedures offer better security and performance isolation, inline SQL keeps the logic co-located with the code that calls it, simplifying debugging for a small application.
- **No admin panel or role system.** Every authenticated user has the same privileges. The application models a peer-to-peer digital wallet, not a multi-tenant system with administrative oversight.
- **Forms Authentication is configured in `web.config` but never actually used.** The application uses manual session-based authentication instead (see [Section 9](#9-authentication--session-management)).

---

## 2. Technology Stack

| Layer | Technology |
|---|---|
| **Framework** | .NET Framework 4.7.2 |
| **Web framework** | ASP.NET WebForms |
| **Language** | C# 7.x (Roslyn compiler via `Microsoft.CodeDom.Providers.DotNetCompilerPlatform`) |
| **Database** | SQL Server LocalDB (`.mdf` file-based, Integrated Security) |
| **Data access** | ADO.NET (`System.Data.SqlClient`) |
| **Frontend** | Server-rendered HTML + CSS (no SPA framework) |
| **CSS** | 473-line hand-written stylesheet (`Content/Style.css`) |
| **JavaScript** | jQuery 3.7.0 (mapped via `ScriptManager`, served from CDN with local fallback) |
| **IDE** | Visual Studio 2022 (or 2017+), IIS Express |
| **Source control** | Git (`.sln` and `.csproj` in repo) |

---

## 3. Directory Structure

```
CloudMoney/
├── CloudMoney.sln                          # VS solution file
├── CloudMoney.csproj                       # Project file (WebForms, .NET 4.7.2)
├── Web.config                              # App configuration (DB conn, auth, etc.)
├── Global.asax / Global.asax.cs            # Application lifecycle events
│
├── Site.Master / .cs / .designer.cs        # Master page (shared layout)
│
├── Default.aspx / .cs                      # Entry router → Dashboard or Login
├── Dashboard.aspx / .cs                    # Main user dashboard
│
├── Auth/
│   ├── Auth.aspx / .cs                     # Sub-router for /Auth/
│   ├── Login.aspx / .cs                    # Login form
│   ├── Register.aspx / .cs                 # Registration form
│   ├── ChangePassword.aspx / .cs           # Change password form
│   └── Logout.aspx / .cs                   # Logout action (no UI)
│
├── Transactions/
│   ├── Deposit.aspx / .cs                  # Deposit money form
│   ├── Withdraw.aspx / .cs                 # Withdraw money form
│   └── SendMoney.aspx / .cs                # Two-step money transfer
│
├── Reports/
│   ├── StatementOfAccount.aspx / .cs       # Full transaction history
│   ├── MyDepositsWithdrawals.aspx / .cs    # Deposit/Withdrawal filter report
│   └── SentReceivedHistory.aspx / .cs      # Sent/Received transfers report
│
├── Logic/
│   ├── DatabaseHelper.cs                   # ADO.NET connection + schema init
│   ├── AuthManager.cs                      # User registration, login, password
│   ├── TransactionManager.cs               # Balance, deposit, withdraw, transfer
│   ├── ReportManager.cs                    # Report queries
│   ├── SessionHelper.cs                    # Session state accessors
│   ├── ValidationHelper.cs                 # Amount/date business rules
│   ├── OperationResult.cs                  # Base result class (Success + Message)
│   ├── LoginResult.cs                      # Login result (extends OperationResult)
│   ├── RegisterResult.cs                   # Registration result
│   ├── RecipientLookupResult.cs            # Recipient search result
│   └── BalanceSnapshot.cs                  # Balance + total sent DTO
│
├── Content/
│   └── Style.css                           # All application styles
│
├── Scripts/                                # Empty (jQuery served via CDN)
│
├── App_Data/
│   ├── CloudMoneyDB.mdf                    # LocalDB database (data file)
│   └── CloudMoneyDB_log.ldf                # LocalDB transaction log
│
├── Properties/
│   └── AssemblyInfo.cs                     # Assembly metadata
│
├── bin/                                    # Compiled output
└── packages/                               # NuGet cache (Roslyn compiler)
```

**The `.designer.cs` files** are auto-generated by Visual Studio. Each `.aspx` file has a corresponding `.designer.cs` that declares protected fields for every server control with `runat="server"`. You should never edit these files manually — Visual Studio regenerates them when you add, remove, or rename controls.

---

## 4. The WebForms Page Model: Concepts You Must Know

Before diving into individual pages, here are the foundational ASP.NET WebForms concepts that every file in this project uses.

### 4.1 The `.aspx` + Code-Behind Split

Every page consists of two parts:

- **`.aspx` file** — declarative markup (HTML mixed with `<asp:*>` server controls and directives).
- **`.aspx.cs` file** — the "code-behind" class that contains event handlers and logic.

The `@Page` directive at the top of each `.aspx` file links them together:

```aspx
<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="CloudMoney.Dashboard" %>
```

Key attributes:
- **`MasterPageFile`** — attaches a master page for consistent layout.
- **`CodeBehind`** — names the C# file.
- **`Inherits`** — the fully-qualified class name. ASP.NET runtime instantiates this class to handle the request.
- **`AutoEventWireup="true"`** — automatically wires `Page_Load`, `Page_Init`, etc. by convention (method name matching) without needing explicit delegate binding.

### 4.2 The Page Lifecycle

Every WebForms page goes through a deterministic lifecycle. Understanding this is critical because it dictates *when* your code runs relative to control initialization, ViewState restoration, and event processing.

```
1.  PreInit          — Master page set, themes applied. Too early for ViewState.
2.  Init             — Controls initialized. Postback data NOT yet loaded.
3.  InitComplete     — All controls initialized.
4.  LoadViewState    — ViewState (hidden __VIEWSTATE field) deserialized into controls.
5.  LoadPostData     — Form values from POST written into control properties.
6.  Load (Page_Load) — *** YOUR CODE USUALLY GOES HERE ***
7.  Control Events   — Button clicks, SelectedIndexChanged, etc. fire AFTER Load.
8.  LoadComplete     — All events processed.
9.  PreRender        — Last chance to modify controls before rendering.
10. SaveViewState    — Control state serialized into __VIEWSTATE.
11. Render           — HTML generated and sent to client.
12. Unload           — Cleanup/dispose.
```

**Critical pattern used throughout this project:** The `IsPostBack` guard.

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadDashboard();  // Only on first load, NOT on button clicks
    }
}
```

- `IsPostBack` returns `true` when the page is responding to a form submission (POST).
- `IsPostBack` returns `false` when the page is loaded for the first time (GET).
- Why this matters: If you bind a GridView on every `Page_Load` (without the guard), postback events like button clicks will rebind the grid before the click handler runs, potentially resetting user input. The guard ensures data binding only happens once, and button-click handlers update controls afterward.

**Event execution order example (Deposit page):**

```
User visits page (GET):
  Page_Load fires → IsPostBack = false → LoadBalance() binds current balance to label.

User clicks "Deposit" button (POST):
  Page_Load fires → IsPostBack = true → skip LoadBalance()
  btnDeposit_Click fires → validate amount → call TransactionManager.Deposit() → LoadBalance() again to refresh label.
```

### 4.3 Server Controls

ASP.NET WebForms wraps HTML elements in server-side objects. The `runat="server"` attribute makes a control accessible in code-behind.

| Server Control | Purpose | Example in this project |
|---|---|---|
| **`<asp:TextBox>`** | Input field. `TextMode="Email"`, `"Password"`, `"Date"` for HTML5 types. | Every form. |
| **`<asp:Label>`** | Display text. Renders as `<span>`. | Dashboard balance labels. |
| **`<asp:Button>`** | Submit button. `OnClick` wires to code-behind method. Causes postback. | Every form's submit. |
| **`<asp:RequiredFieldValidator>`** | Client+server validation: ensures field is not empty. | Every form. |
| **`<asp:CompareValidator>`** | Client+server validation: compares two fields (e.g. password match). | Registration confirm password. |
| **`<asp:Panel>`** | Container `<div>`. Can be toggled `Visible`. | Recipient details panel in SendMoney. |
| **`<asp:HiddenField>`** | Stores value between postbacks without showing it. | Stores recipient UserId in SendMoney. |
| **`<asp:Repeater>`** | Template-driven list. No built-in styling — full control over HTML. | Dashboard "recent transfers" list. |
| **`<asp:GridView>`** | Auto-generated HTML table with built-in paging/sorting. | All report pages. |
| **`<asp:DropDownList>`** | Select dropdown. | Report type filters (All/Deposit/Withdrawal). |
| **`<asp:ContentPlaceHolder>`** | In master pages: defines where content pages inject their markup. | `MainContent` in Site.Master. |
| **`<asp:Content>`** | In content pages: wraps markup placed into a ContentPlaceHolder. | Every content page. |

**How code-behind accesses controls:**

In `Dashboard.aspx.cs`:
```csharp
lblAccountNumber.Text = accountNumber;
repNotifications.DataSource = notifications;
repNotifications.DataBind();
```

The controls (`lblAccountNumber`, `repNotifications`) are declared in the `.designer.cs` as `protected global::System.Web.UI.WebControls.Label lblAccountNumber;`. They are automatically instantiated by the ASP.NET runtime during the `Init` phase. You never call `new Label()` yourself.

### 4.4 ViewState

ViewState is WebForms' mechanism for preserving control state across postbacks. It serializes control property values into a hidden `<input type="hidden" name="__VIEWSTATE">` field in the HTML form.

**How this project uses ViewState:**

- **Implicitly:** Most server controls use it by default. When you set `lblBalance.Text = "P1,500.00"`, that value is stored in ViewState and restored on the next postback — even though labels aren't form fields.
- **HiddenField (`hfRecipientUserId`)** is explicitly used in `SendMoney.aspx` to persist the recipient's UserId between the "Verify Recipient" and "Send Money" button clicks. This is a deliberate ViewState-dependent pattern.

**ViewState tradeoffs:** It makes state management trivial but bloats page size (the hidden field can grow large). This project keeps it minimal by binding data only on non-postback loads.

### 4.5 Validation Architecture

The project uses a **dual-layer validation** strategy:

**Layer 1: Client-side (ASP.NET Validation Controls)**

```aspx
<asp:TextBox ID="txtEmail" runat="server" TextMode="Email"
    placeholder="Enter your email address" CssClass="form-input" />
<asp:RequiredFieldValidator ID="rfvEmail" runat="server"
    ControlToValidate="txtEmail" ErrorMessage="Email address is required."
    Display="Dynamic" ForeColor="#d93025" />
```

- Validators render JavaScript that runs in the browser *before* the form submits.
- `Display="Dynamic"` means the error message takes no layout space until visible (prevents layout shift).
- `ForeColor="#d93025"` styles the error text red.
- `Page.IsValid` in code-behind indicates whether all client-side validators passed (also checked server-side for security).

**The `CompareValidator` on registration:**
```aspx
<asp:CompareValidator ID="cvPasswordMatch" runat="server"
    ControlToValidate="txtConfirmPassword"
    ControlToCompare="txtPassword"
    ErrorMessage="Passwords do not match."
    Display="Dynamic" ForeColor="#d93025" />
```

This compares two TextBox values client-side and server-side. It's the standard WebForms approach for password confirmation.

**Layer 2: Server-side (`ValidationHelper` + per-method validation)**

Client validation can be bypassed (disabled JavaScript, crafted HTTP requests), so every operation re-validates server-side:

- `ValidationHelper.TryParseAmount()` — strips peso symbol, parses `decimal`, enforces range (P100–P2,000) and divisibility by 100.
- `ValidationHelper.ValidateDateRange()` — start < end, dates not in the future.
- `AuthManager.Register()` — verifies password length >= 6, confirms password match, checks duplicate email.
- `TransactionManager.Deposit()` — balance + amount <= 10,000 max.
- `TransactionManager.Withdraw()` — balance >= amount.

### 4.6 Message Display Pattern

Rather than using `<asp:Label>` for status messages on each page, the project centralizes messages through the master page:

```csharp
// In Login.aspx.cs:
var master = (Site)this.Master;
master.ShowError("Invalid email or password.");
master.ShowMessage("Login successful.");
master.ShowInfo("Showing transactions from the last 30 days.");
master.ClearMessage();
```

The `Site` master class exposes four methods that set the CSS class and text of a shared `pnlMessage` Panel. This ensures consistent message styling across all pages.

---

## 5. Master Page: Site.Master

The master page (`Site.Master` / `Site.Master.cs`) provides the shared HTML shell for all content pages. It is the WebForms equivalent of a layout file in other frameworks.

### 5.1 Structure

```
┌──────────────────────────────────────────────┐
│  HEADER (Site.Master)                        │
│  ├── Logo: "CloudMoney — Digital Wallet"     │
│  └── Navigation bar                          │
│      ├── [Logged Out]: Login | Register      │
│      └── [Logged In]:  Dashboard | Deposit   │
│                        | Withdraw | Send ... │
│                        | Reports ▼ | ...     │
├──────────────────────────────────────────────┤
│  MESSAGE PANEL (pnlMessage / lblMessage)     │
│  Visibility toggled by code-behind methods   │
├──────────────────────────────────────────────┤
│  MAIN CONTENT (ContentPlaceHolder)           │
│  ┌────────────────────────────────────────┐  │
│  │  Content injected by each .aspx page   │  │
│  │  via <asp:Content> tags                │  │
│  └────────────────────────────────────────┘  │
├──────────────────────────────────────────────┤
│  FOOTER: Copyright © {year} CloudMoney       │
└──────────────────────────────────────────────┘
```

### 5.2 Navigation Toggle

The master page's `Page_Load` calls `UpdateNavigation()`, which checks if `Session["UserId"]` is set and toggles `phLoggedIn` / `phLoggedOut` visibility. This is the **only** mechanism that controls what navigation links appear — there is no per-page permission logic in the master.

**The Reports dropdown** is a pure CSS dropdown (no JavaScript). A `:hover` rule on the dropdown container shows the sub-menu. This is a clean pattern that avoids the complexity of client-side dropdown toggling.

### 5.3 ContentPlaceHolder: How Pages Inject Content

In `Site.Master`:
```aspx
<asp:ContentPlaceHolder ID="MainContent" runat="server">
</asp:ContentPlaceHolder>
```

In `Dashboard.aspx`:
```aspx
<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="CloudMoney.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dashboard-grid">
        <!-- Dashboard-specific markup -->
    </div>
</asp:Content>
```

Every content page uses `<asp:Content>` with `ContentPlaceHolderID="MainContent"` to inject its markup into the master's layout. There are no other `ContentPlaceHolder` regions (like head scripts or sidebars) — the single content region keeps the design simple.

### 5.4 Accessing Master Page Methods from Content Pages

The master page class is named `Site` (from `Inherits="CloudMoney.Site"`). Content pages access it via the `this.Master` property, which is typed as `MasterPage`. A cast is needed:

```csharp
var master = (Site)this.Master;
master.ShowError("Something went wrong.");
```

This is a standard WebForms pattern for master/content communication.

---

## 6. Global.asax and Application Startup

`Global.asax` is the ASP.NET equivalent of `Startup.cs` in ASP.NET Core or `App.xaml.cs` in WPF. It handles application-level lifecycle events.

### 6.1 `Application_Start`

```csharp
protected void Application_Start(object sender, EventArgs e)
{
    DatabaseHelper.EnsureDatabase();

    ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
        new ScriptResourceDefinition
        {
            Path = "~/Scripts/jquery-3.7.0.min.js",
            DebugPath = "~/Scripts/jquery-3.7.0.js",
            CdnPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.0.min.js",
            CdnDebugPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.0.js"
        });
}
```

**What happens on first request:**

1. **`DatabaseHelper.EnsureDatabase()`** executes `CREATE TABLE IF NOT EXISTS` statements. On a fresh deployment with an empty `.mdf` file, this creates the `Users` and `Transactions` tables along with all indexes. On subsequent runs, it detects existing tables and skips creation. This is the "self-provisioning database" pattern — no separate SQL scripts needed.
2. **jQuery mapping** registers jQuery 3.7.0 with the ASP.NET `ScriptManager`. The `CdnPath` takes priority; if the CDN is unreachable, ASP.NET falls back to the local `~/Scripts/` path. (The `Scripts/` directory is empty in this repo because the CDN is expected to work, but you can drop jQuery files there for offline development.)

### 6.2 `Application_Error`

Not defined in this project. Unhandled exceptions will display the default ASP.NET yellow error page (`customErrors mode="Off"` is the default in debug builds).

---

## 7. Database Design

### 7.1 Connection Configuration (`Web.config`)

```xml
<connectionStrings>
  <add name="CloudMoneyDB"
       connectionString="Data Source=(LocalDB)\MSSQLLocalDB;
         AttachDbFilename=|DataDirectory|\CloudMoneyDB.mdf;
         Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

- **`(LocalDB)\MSSQLLocalDB`** — SQL Server LocalDB, a lightweight edition of SQL Server Express designed for development. Runs on-demand, no service configuration.
- **`|DataDirectory|`** — resolves to the `App_Data` folder at runtime. This is an ASP.NET convention that keeps the database file within the project structure.
- **`Integrated Security=True`** — uses the Windows identity of the running process (IIS Express or the developer's user account). No SQL username/password needed.

### 7.2 Table: `dbo.Users`

| Column | Type | Constraints | Purpose |
|---|---|---|---|
| `UserId` | `INT IDENTITY(1,1)` | PRIMARY KEY | Surrogate key, auto-increments |
| `AccountNumber` | `CHAR(10)` | NOT NULL, UNIQUE | User-facing account identifier (e.g., `3827615049`) |
| `FullName` | `NVARCHAR(120)` | NOT NULL | Display name |
| `Email` | `NVARCHAR(150)` | NOT NULL, UNIQUE | Login credential + contact |
| `PasswordPlainText` | `NVARCHAR(200)` | NOT NULL | Password stored in plain text (see §17) |
| `DateRegistered` | `DATETIME2` | NOT NULL | Registration timestamp |

**Design notes:**

- `AccountNumber` is a fixed-length `CHAR(10)`, always 10 digits, generated cryptographically. The first digit is guaranteed non-zero (so it always looks like a proper account number, not a number with a leading zero that could be confused with octal).
- `NVARCHAR` is used for string columns to support Unicode (international names/emails). `VARCHAR` would save space but would corrupt non-ASCII characters.
- `UNIQUE` constraints on `AccountNumber` and `Email` create automatic unique indexes, preventing duplicate registrations.

### 7.3 Table: `dbo.Transactions`

| Column | Type | Constraints | Purpose |
|---|---|---|---|
| `TransactionId` | `INT IDENTITY(1,1)` | PRIMARY KEY | Surrogate key |
| `TransactionType` | `NVARCHAR(20)` | NOT NULL | `'Deposit'`, `'Withdrawal'`, or `'Transfer'` |
| `FromUserId` | `INT NULL` | FK → Users(UserId) | Sender (NULL for deposits) |
| `ToUserId` | `INT NULL` | FK → Users(UserId) | Receiver (NULL for withdrawals) |
| `Amount` | `DECIMAL(18,2)` | NOT NULL | Transaction amount (always positive) |
| `TransactionDate` | `DATETIME2` | NOT NULL, INDEXED | When the transaction occurred |
| `Remarks` | `NVARCHAR(250) NULL` | | Human-readable description |

**Design notes — the single-table transaction ledger model:**

This schema uses a **unified transaction table** rather than separate tables for deposits, withdrawals, and transfers. This is a classic double-entry-adjacent design:

- **Deposit:** `FromUserId = NULL`, `ToUserId = user`, `TransactionType = 'Deposit'`. Money enters the system from "outside."
- **Withdrawal:** `FromUserId = user`, `ToUserId = NULL`, `TransactionType = 'Withdrawal'`. Money leaves the system.
- **Transfer:** Both `FromUserId` and `ToUserId` populated, `TransactionType = 'Transfer'`. Money moves between users.

**This design makes balance computation a single aggregate query** (see `TransactionManager.GetBalanceSnapshot()` in §13.2) and report queries straightforward (see §13.3). The tradeoff is that `Amount` is always stored as a positive number — the direction is determined by which foreign key column is set, not by a sign.

**Indexes:**
- `IX_Transactions_TransactionDate` — supports date-range filtering for reports.
- `IX_Transactions_FromUserId` — supports "sent" queries.
- `IX_Transactions_ToUserId` — supports "received" queries.

**There are no foreign key constraints defined as `ON DELETE CASCADE` or `ON UPDATE CASCADE`** — the FKs are relational only via JOIN queries, not enforced with DRI (Declarative Referential Integrity) actions. This is why `FromUserId` and `ToUserId` are `NULL`able: a user could theoretically be deleted and their transactions would remain with NULL foreign keys.

---

## 8. Data Access Layer: DatabaseHelper

`Logic/DatabaseHelper.cs` is a `public static class` that encapsulates all ADO.NET operations.

### 8.1 Connection Management

```csharp
public static readonly string ConnectionString =
    ConfigurationManager.ConnectionStrings["CloudMoneyDB"].ConnectionString;

public static SqlConnection CreateOpenConnection()
{
    var connection = new SqlConnection(ConnectionString);
    connection.Open();
    return connection;
}
```

Every method that talks to the database calls `CreateOpenConnection()`, wrapped in a `using` block for automatic disposal:

```csharp
using (var connection = DatabaseHelper.CreateOpenConnection())
{
    // use connection
}
```

`using` blocks ensure `SqlConnection.Dispose()` is called (which calls `Close()`), returning the connection to ADO.NET's connection pool. This is the correct pattern — connections are relatively expensive to open but cheap to reuse from the pool.

### 8.2 Query Execution Methods

| Method | Returns | Used for |
|---|---|---|
| `ExecuteNonQuery(sql, params)` | `int` (rows affected) | INSERT, UPDATE, DELETE |
| `ExecuteScalar<T>(sql, params)` | `T` (single value) | `SELECT COUNT(1)`, `SELECT MAX(...)` |
| `GetDataTable(sql, params)` | `DataTable` | SELECT queries for GridView/Repeater binding |
| `CreateCommand(conn, txn, sql, params)` | `SqlCommand` | Internal helper, used directly for transaction-scoped operations |

**Parameterization pattern:**

All SQL uses `@ParameterName` placeholders with values supplied via `Dictionary<string, object>`:

```csharp
var parameters = new Dictionary<string, object>
{
    { "@Email", email },
    { "@Password", password }
};
var result = DatabaseHelper.ExecuteScalar<int>(
    "SELECT COUNT(1) FROM dbo.Users WHERE Email = @Email AND PasswordPlainText = @Password",
    parameters);
```

`DatabaseHelper.CreateCommand()` iterates the dictionary and calls `cmd.Parameters.AddWithValue(key, value ?? DBNull.Value)`. Null values become `DBNull.Value` — critical for the nullable `FromUserId`/`ToUserId` columns.

**Why raw ADO.NET instead of an ORM:**

- Zero dependencies beyond `System.Data.SqlClient` (which is part of the .NET Framework).
- Full visibility into queries — you can read the SQL inline with the code.
- Simpler debugging — stack traces point directly to the query string.
- For two tables with straightforward queries, an ORM would add complexity without benefit.

### 8.3 Schema Self-Provisioning (`EnsureDatabase`)

`EnsureDatabase()` is called once on `Application_Start`. It uses `SELECT OBJECT_ID(...)` checks to determine if tables already exist, then runs `CREATE TABLE` and `CREATE INDEX` statements only if needed. This ensures:

- A fresh clone of the repo works immediately — no separate SQL scripts to run.
- The database schema is defined in code, making it self-documenting.
- Schema changes are applied on the next app restart.

The SQL uses `IF OBJECT_ID(N'dbo.TableName', N'U') IS NULL` (checking for user table existence) before each CREATE statement, which is the standard SQL Server pattern for idempotent schema creation.

---

## 9. Authentication & Session Management

### 9.1 Dual Authentication Configuration

The `Web.config` declares:

```xml
<authentication mode="Forms">
  <forms loginUrl="~/Auth/Login.aspx" timeout="60" slidingExpiration="true" />
</authentication>
<authorization>
  <allow users="*" />
</authorization>
```

**However, Forms Authentication is never actually activated.** The `<authorization>` allows all users (`*`), meaning no directory-level access restrictions exist. The `Login.aspx` code-behind never calls `FormsAuthentication.SetAuthCookie()`. This is an intentional simplification:

> The project uses **manual session-based authentication** via `SessionHelper` rather than ASP.NET Forms Authentication. Both approaches work; the session approach was chosen for this project because it avoids the complexity of FormsAuthentication ticket encryption and makes the auth flow trivially debugable (everything is in `Session`).

### 9.2 Login Flow

```
User fills Login form
     │
     ▼
Login.aspx.cs → btnLogin_Click
     │
     ├── Calls AuthManager.Login(email, password)
     │       │
     │       ├── SELECT UserId, AccountNumber, FullName
     │       │   FROM Users
     │       │   WHERE Email = @Email AND PasswordPlainText = @Password
     │       │
     │       └── Returns LoginResult { Success, UserId, AccountNumber, FullName }
     │
     ├── On failure: master.ShowError(result.Message)
     │
     └── On success:
           SessionHelper.SignIn(Session, result)
               │
               ├── Session["UserId"]        = result.UserId
               ├── Session["AccountNumber"] = result.AccountNumber
               └── Session["FullName"]      = result.FullName
           │
           └── Response.Redirect("~/Dashboard.aspx")
```

### 9.3 SessionHelper: Session State Abstraction

`Logic/SessionHelper.cs` provides typed access to session values:

```csharp
public static class SessionHelper
{
    public static bool IsLoggedIn(HttpSessionStateBase session)
        => session["UserId"] != null;

    public static int GetUserId(HttpSessionStateBase session)
        => Convert.ToInt32(session["UserId"]);

    public static string GetAccountNumber(HttpSessionStateBase session)
        => Convert.ToString(session["AccountNumber"]);

    public static string GetFullName(HttpSessionStateBase session)
        => Convert.ToString(session["FullName"]);

    public static void SignIn(HttpSessionStateBase session, LoginResult loginResult)
    {
        session["UserId"] = loginResult.UserId;
        session["AccountNumber"] = loginResult.AccountNumber;
        session["FullName"] = loginResult.FullName;
    }

    public static void SignOut(HttpSessionState session)
    {
        session.Clear();
        session.Abandon();
    }

    public static bool RequireLogin(Page page)
    {
        if (page.Session["UserId"] == null)
        {
            page.Response.Redirect("~/Auth/Login.aspx");
            return false;
        }
        return true;
    }
}
```

**Why a dedicated helper class instead of accessing `Session["UserId"]` directly:**

- **Type safety:** `GetUserId(session)` returns `int`, catching type errors at compile time.
- **Single point of change:** If the session key name changes from `"UserId"` to `"CurrentUserId"`, only `SessionHelper` needs updating.
- **Readability:** `SessionHelper.GetUserId(Session)` is more self-documenting than `(int)Session["UserId"]`.
- **Overloaded methods:** `IsLoggedIn` and `SignOut` accept both `HttpSessionStateBase` (available in `Page_Load` via `Session`) and `HttpSessionState` (available in `Global.asax`).

### 9.4 Protected Page Pattern

Every page that requires authentication calls `RequireLogin` as the **first statement** in `Page_Load`:

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    SessionHelper.RequireLogin(this);  // ← gates entire page
    if (!IsPostBack)
    {
        // ... load data
    }
}
```

`RequireLogin` checks `Session["UserId"]` and redirects to the login page if absent. It returns `false` on redirect, but `Response.Redirect` throws a `ThreadAbortException` internally to halt execution, so the method never actually returns `false` — the page processing stops at the redirect.

### 9.5 Logout Flow

```
User clicks "Logout" link
     │
     ▼
Auth/Logout.aspx → Page_Load
     │
     ├── SessionHelper.SignOut(Session)
     │       ├── session.Clear()      // removes all keys
     │       └── session.Abandon()    // destroys session cookie
     │
     └── Response.Redirect("~/Auth/Login.aspx")
```

`Logout.aspx` has no UI — it's a pure action page. The markup is minimal (just a `@Page` directive, no `MasterPageFile`). This is a common WebForms pattern for actions that don't need a view.

---

## 10. Result Data Classes

The `Logic/` folder contains five lightweight data classes that serve as return types from business logic methods. They follow a consistent pattern.

### 10.1 Inheritance Hierarchy

```
OperationResult (base)
├── LoginResult
├── RegisterResult
└── RecipientLookupResult

BalanceSnapshot (standalone, not a result)
```

### 10.2 OperationResult (Base Class)

```csharp
public class OperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public static OperationResult Ok(string message)
        => new OperationResult { Success = true, Message = message };

    public static OperationResult Fail(string message)
        => new OperationResult { Success = false, Message = message };
}
```

The static factory methods (`Ok`, `Fail`) provide a concise way to create results:

```csharp
return OperationResult.Fail("Insufficient balance.");
return OperationResult.Ok("Deposit successful.");
```

### 10.3 LoginResult

Adds `UserId`, `AccountNumber`, and `FullName` properties for populating the session on successful login.

### 10.4 RegisterResult

Adds `AccountNumber` so the registration page can display the generated account number to the newly registered user.

### 10.5 RecipientLookupResult

Adds `UserId`, `AccountNumber`, and `FullName` so the SendMoney page can display recipient details and store their UserId for the actual transfer.

### 10.6 BalanceSnapshot

A simple DTO (Data Transfer Object) with `CurrentBalance` and `TotalSent` — not an `OperationResult` because balance computation can't really "fail" (zero is a valid balance).

**Why result classes instead of exceptions for business failures:**

Exceptions are for *unexpected* failures (database unreachable, null reference, etc.). A user entering the wrong password or having insufficient balance is an *expected* business outcome. Returning a result object with `Success = false` avoids the performance cost of exception throwing and keeps control flow linear (no try/catch for normal logic).

---

## 11. Validation Layer

`Logic/ValidationHelper.cs` defines business rules as constants and provides reusable validation methods.

### 11.1 Business Rule Constants

```csharp
public const decimal MinimumTransactionAmount = 100m;  // P100.00
public const decimal MaximumTransactionAmount = 2000m; // P2,000.00
public const decimal MaximumBalance = 10000m;          // P10,000.00
```

These are `const` (compile-time constants) — they can't change at runtime, and the compiler inlines them everywhere they're used. For a demo application with fixed rules this is fine; a production app might load these from configuration.

### 11.2 Amount Validation

```csharp
public static bool TryParseAmount(string rawAmount, out decimal amount, out string message)
```

This method:
1. Strips leading/trailing whitespace.
2. Removes `P` or `p` prefix (users might type "P1,500" or "1500").
3. Tries parsing with both `CultureInfo.InvariantCulture` (period decimal) and `CultureInfo.CurrentCulture` (locale-aware).
4. Delegates to `ValidateAmount()` for range and divisibility checks.

```csharp
public static bool ValidateAmount(decimal amount, out string message)
```

Checks:
- `amount >= 100` — minimum transaction is P100.
- `amount <= 2000` — maximum transaction is P2,000.
- `amount % 100 == 0` — must be in increments of P100 (e.g., P100, P200, P500, P1000, P2000).

**Why the divisibility rule?** It models a real-world constraint: ATMs and cash services typically dispense in certain denominations. It also prevents "dust" transactions (P0.01, P1.37, etc.) that would clutter the ledger.

### 11.3 Date Range Validation

```csharp
public static bool ValidateDateRange(DateTime startDate, DateTime endDate, out string message)
```

Checks:
- `endDate > startDate` — range must be positive.
- Both dates must be on or before today (no future-dated reports).

This is used by all three report pages before executing database queries.

---

## 12. Module Walkthroughs

This section walks through every page's markup, code-behind logic, and user flow.

### 12.1 Entry Routing: Default.aspx and Auth/Auth.aspx

Both files are pure routers — they have no UI, no master page, and no user-visible content.

**Default.aspx.cs:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (SessionHelper.IsLoggedIn(Session))
        Response.Redirect("~/Dashboard.aspx");
    else
        Response.Redirect("~/Auth/Login.aspx");
}
```

**Auth/Auth.aspx.cs** does the same but redirects logged-in users to `~/Dashboard.aspx` (it lives under `/Auth/` so the logic is slightly different — it acts as a sub-router).

**Why two routers?** `Default.aspx` is the root entry point (set as the default document in `Web.config`). `Auth/Auth.aspx` handles the case where a logged-in user navigates to `/Auth/` directly. Both ensure users end up at the right place.

### 12.2 Registration: Auth/Register.aspx

**Flow:**
1. User fills in Full Name, Email, Password, Confirm Password.
2. `CompareValidator` checks that Password and Confirm Password match — client-side, before the form submits.
3. On postback, `btnRegister_Click` calls `AuthManager.Register()`.
4. `AuthManager.Register()`:
   - Validates password length >= 6.
   - Checks `SELECT COUNT(1) FROM Users WHERE Email = @Email` for duplicates.
   - Generates a unique 10-digit account number via `GenerateUniqueAccountNumber()`.
   - Inserts the user record.
5. On success, the page displays the generated account number and clears the form.

**Account number generation algorithm:**
```csharp
private string GenerateUniqueAccountNumber()
{
    using (var rng = RandomNumberGenerator.Create())
    {
        for (int attempt = 0; attempt < 50; attempt++)
        {
            char[] digits = new char[10];
            digits[0] = NextNonZeroDigit(rng);
            for (int i = 1; i < 10; i++)
                digits[i] = NextDigit(rng);
            string candidate = new string(digits);
            // Check uniqueness...
        }
    }
}
```

This uses `System.Security.Cryptography.RandomNumberGenerator` (not `System.Random`) to generate unbiased random digits. `NextDigit` uses rejection sampling (`do { digit = ...; } while (digit > 9)`) to avoid modulo bias from `rng.GetBytes() % 10`. The first digit is guaranteed non-zero. Up to 50 attempts are made to avoid collisions.

**Why 10-digit account numbers?** They're long enough to be unique for a small-to-medium user base, short enough to be memorable, and `CHAR(10)` fixed-length ensures consistent formatting.

### 12.3 Login: Auth/Login.aspx

**Flow:**
1. If already logged in (`Session["UserId"] != null`), redirect to Dashboard.
2. User enters Email and Password, clicks Login.
3. `AuthManager.Login()` queries the database.
4. On success, `SessionHelper.SignIn()` populates session, redirects to Dashboard.
5. On failure, `master.ShowError()` displays "Invalid email or password."

**The login page uses `RequiredFieldValidator` on both inputs.** The `TextMode="Email"` attribute on the email TextBox provides HTML5 browser-level email validation as an additional layer.

### 12.4 Change Password: Auth/ChangePassword.aspx

A straightforward form: Current Password, New Password, Confirm New Password.

`AuthManager.ChangePassword()`:
1. Validates current password is not empty.
2. Validates new password length >= 6 and matches confirmation.
3. Fetches stored password with `SELECT PasswordPlainText FROM Users WHERE UserId = @UserId`.
4. Compares current password (exact match, ordinal comparison).
5. Updates with `UPDATE Users SET PasswordPlainText = @Password WHERE UserId = @UserId`.

**No old-password verification beyond exact match** — see Security Considerations (§17).

### 12.5 Dashboard: Dashboard.aspx

The dashboard is the central hub after login. It displays four pieces of information:

1. **Account number** (from session/DB).
2. **Full name** (from session/DB).
3. **Current balance** — computed by `TransactionManager.GetBalanceSnapshot()`.
4. **Total sent** — sum of all transfers where user is sender.
5. **Registration date** — formatted as `MMMM dd, yyyy`.
6. **Recent received transfers** — the 5 most recent incoming transfers, shown via an `<asp:Repeater>`.

**LoadDashboard() logic:**

```csharp
private void LoadDashboard()
{
    int userId = SessionHelper.GetUserId(Session);
    var user = AuthManager.GetUserById(userId);
    if (user == null)
    {
        SessionHelper.SignOut(Session);
        Response.Redirect("~/Auth/Login.aspx");
        return;
    }
    var snapshot = TransactionManager.GetBalanceSnapshot(userId);

    lblAccountNumber.Text = user["AccountNumber"].ToString();
    lblFullName.Text = user["FullName"].ToString();
    lblCurrentBalance.Text = snapshot.CurrentBalance.ToString("P#,##0.00");
    lblTotalSent.Text = snapshot.TotalSent.ToString("P#,##0.00");
    lblDateRegistered.Text = Convert.ToDateTime(user["DateRegistered"])
        .ToString("MMMM dd, yyyy");

    var transfers = TransactionManager.GetRecentReceivedTransfers(userId, 5);
    repNotifications.DataSource = transfers;
    repNotifications.DataBind();
    pnlNoNotifications.Visible = transfers.Rows.Count == 0;
}
```

**Why refetch user from DB instead of trusting session data?** The session stores user info at login time, but the password or name could have been changed in another session. The database is the source of truth. If the user record is somehow deleted, the dashboard detects it and forces re-login.

**Repeater template:**
```aspx
<asp:Repeater ID="repNotifications" runat="server">
    <ItemTemplate>
        <div class="notification-item">
            You received <span class="amount-received">
                P<%# Eval("Amount", "{0:#,##0.00}") %></span>
            from <%# Eval("SenderFullName") %>
            (Acct: <%# Eval("SenderAccountNumber") %>)
            <span class="notification-date">
                <%# Eval("TransactionDate", "{0:MMM dd, yyyy}") %>
            </span>
        </div>
    </ItemTemplate>
</asp:Repeater>
```

`Eval()` is the WebForms data-binding syntax — it evaluates a property or column name at bind time. `{0:#,##0.00}` and `{0:MMM dd, yyyy}` are .NET format strings.

### 12.6 Deposit: Transactions/Deposit.aspx

**Flow:**
1. Page loads, displays current balance.
2. User enters amount (P100–P2,000, divisible by 100).
3. Clicks "Deposit."

**btnDeposit_Click logic:**
```csharp
protected void btnDeposit_Click(object sender, EventArgs e)
{
    if (!ValidationHelper.TryParseAmount(txtAmount.Text, out decimal amount, out string error))
    {
        master.ShowError(error);
        LoadBalance();
        return;
    }

    int userId = SessionHelper.GetUserId(Session);
    var result = TransactionManager.Deposit(userId, amount);

    if (result.Success)
    {
        master.ShowMessage(result.Message);
        txtAmount.Text = "";
    }
    else
    {
        master.ShowError(result.Message);
    }
    LoadBalance();
}
```

**TransactionManager.Deposit()** performs the balance cap check (new balance <= P10,000) and inserts a transaction row with `TransactionType = 'Deposit'`, `ToUserId = userId`, `FromUserId = NULL`.

### 12.7 Withdraw: Transactions/Withdraw.aspx

Identical structure to Deposit, but calls `TransactionManager.Withdraw()`.

**TransactionManager.Withdraw()** checks sufficient balance (`balance >= amount`) and inserts with `TransactionType = 'Withdrawal'`, `FromUserId = userId`, `ToUserId = NULL`.

Both pages display the current balance before and after the transaction, so the user can immediately see the result.

### 12.8 Send Money: Transactions/SendMoney.aspx

This is the most complex page. It implements a **two-step transfer flow:**

**Step 1 — Verify Recipient:**
1. User enters the recipient's 10-digit account number.
2. Clicks "Verify Recipient."
3. `TransactionManager.FindRecipient(accountNumber, currentUserId)` looks up the account.
4. Validates: account exists, recipient is not the sender.
5. On success: recipient details are displayed in a Panel (`pnlRecipientDetails`), and the recipient's `UserId` is stored in `HiddenField` (`hfRecipientUserId`).
6. The Panel becomes visible, exposing the amount and password fields.

**Step 2 — Send Money:**
1. User enters amount and their password (re-authentication).
2. Clicks "Send CloudMoney."
3. `TransactionManager.SendMoney(senderUserId, recipientUserId, amount, password)` runs inside a **database transaction**:
   - Validates amount, validates password not empty.
   - Calls `AuthManager.ValidatePassword()` to confirm the password.
   - Checks sender's balance >= amount.
   - **Opens a SQL transaction (`connection.BeginTransaction()`).**
   - Within the transaction: re-verifies recipient exists, re-computes balance (to prevent race conditions), inserts the transfer row.
   - Commits the transaction. On exception, rolls back.
4. On success: clears form, hides recipient panel, shows success message.

**Why a database transaction for SendMoney?**

Without a transaction, it's theoretically possible that:
- The balance check passes.
- Between the check and the INSERT, another concurrent withdrawal reduces the balance below the required amount.
- The transfer still goes through, creating an overdraft.

The explicit `SqlTransaction` ensures atomicity: the balance re-check and the INSERT happen as a single unit of work. If anything fails, the rollback prevents partial state.

**Why two steps instead of one form?**

The two-step flow (verify first, then send) is a UX pattern seen in real banking apps. It lets the user confirm the recipient's identity before committing funds, reducing the risk of sending money to the wrong account.

**The HiddenField (`hfRecipientUserId`) stores the recipient's UserId between Step 1 and Step 2.** This is a deliberate use of ViewState — the value survives postbacks without being visible to the user. An alternative would be storing it in session, but ViewState keeps the state scoped to this specific page instance.

### 12.9 Reports: StatementOfAccount.aspx

Displays every transaction for the logged-in user within a date range.

**Controls:**
- Two `TextBox` controls with `TextMode="Date"` (HTML5 date picker).
- A "Generate Report" button.
- A `GridView` with columns: Date, Type, Direction, Amount, Sender Account, Sender Name, Recipient Account, Recipient Name, Remarks.

**Default behavior:** On first page load (non-postback), the start date is set to 30 days ago and the end date to today. The report auto-loads.

**GridView binding pattern:**
```csharp
private void BindReport()
{
    var master = (Site)this.Master;
    master.ClearMessage();

    if (!TryGetDateRange(out DateTime startDate, out DateTime endDate))
        return;

    int userId = SessionHelper.GetUserId(Session);
    var data = ReportManager.GetStatementOfAccount(userId, startDate, endDate);
    gvStatement.DataSource = data;
    gvStatement.DataBind();

    master.ShowInfo($"Showing {data.Rows.Count} transactions from {startDate:d} to {endDate:d}.");
}
```

**GridView configuration:**
```aspx
<asp:GridView ID="gvStatement" runat="server" CssClass="grid-view"
    AutoGenerateColumns="False" EmptyDataText="No transactions found for this range.">
    <Columns>
        <asp:BoundField DataField="TransactionDate" HeaderText="Date"
            DataFormatString="{0:MMM dd, yyyy hh:mm tt}" />
        <asp:BoundField DataField="TransactionType" HeaderText="Type" />
        <asp:BoundField DataField="Direction" HeaderText="Direction" />
        <asp:BoundField DataField="Amount" HeaderText="Amount"
            DataFormatString="{0:#,##0.00}" />
        <!-- ... more columns -->
    </Columns>
</asp:GridView>
```

`AutoGenerateColumns="False"` gives explicit control over which columns appear and their formatting. `DataFormatString` applies .NET format strings to each cell. `EmptyDataText` is shown as a single row when the DataTable has zero rows.

### 12.10 Reports: MyDepositsWithdrawals.aspx

A subset of the statement report, filtered to only Deposits and Withdrawals.

**Key difference from StatementOfAccount:** Includes a `DropDownList` for type filtering:
```aspx
<asp:DropDownList ID="ddlType" runat="server" CssClass="form-input">
    <asp:ListItem Value="All">All Transactions</asp:ListItem>
    <asp:ListItem Value="Deposit">Deposits Only</asp:ListItem>
    <asp:ListItem Value="Withdrawal">Withdrawals Only</asp:ListItem>
</asp:DropDownList>
```

The selected value is passed to `ReportManager.GetMyDepositsWithdrawals()`, which appends `AND TransactionType = @TypeFilter` to the SQL when the value is not "All".

### 12.11 Reports: SentReceivedHistory.aspx

Mirrors MyDepositsWithdrawals but for transfers. The dropdown options are "All", "Sent", "Received".

---

## 13. Business Logic Layer Deep Dive

### 13.1 AuthManager

**Register(email, password, fullName, confirmPassword):**
- Validates inputs server-side (lengths, match, uniqueness).
- Generates unique account number.
- Inserts user row with `DateTime.UtcNow` as registration date.

**Login(email, password):**
- Trims and lowercases email for case-insensitive matching.
- Direct `WHERE Email = @Email AND PasswordPlainText = @Password` comparison.

**ChangePassword(userId, currentPassword, newPassword, confirmPassword):**
- Fetches stored password, compares, updates.

**ValidatePassword(userId, password):**
- `SELECT COUNT(1) FROM Users WHERE UserId = @UserId AND PasswordPlainText = @Password`.
- Returns `true` if count > 0.

**GetUserById(userId):**
- `SELECT UserId, AccountNumber, FullName, Email, DateRegistered FROM Users WHERE UserId = @UserId`.
- Returns `DataRow` or `null`.

### 13.2 TransactionManager — Balance Computation

`GetBalanceSnapshot(userId)` uses a single aggregate query:

```sql
SELECT
    SUM(CASE
        WHEN TransactionType = 'Deposit'  AND ToUserId   = @UserId THEN Amount
        WHEN TransactionType = 'Transfer' AND ToUserId   = @UserId THEN Amount
        WHEN TransactionType = 'Withdrawal' AND FromUserId = @UserId THEN -Amount
        WHEN TransactionType = 'Transfer' AND FromUserId = @UserId THEN -Amount
        ELSE 0
    END) AS CurrentBalance,

    SUM(CASE
        WHEN TransactionType = 'Transfer' AND FromUserId = @UserId THEN Amount
        ELSE 0
    END) AS TotalSent
FROM dbo.Transactions
```

**Why a single aggregate over the entire table instead of maintaining a running balance column?**

A running balance column in the `Users` table (updated on every transaction) would be faster to read but introduces a risk: if the running balance gets out of sync with the transaction history (due to a bug or manual DB edit), there's no way to recover. Computing the balance from the immutable transaction ledger ensures the balance is always a verifiable function of the transaction history.

The tradeoff is performance — as the `Transactions` table grows, this query gets slower. For a demo application with modest transaction volume, this is acceptable. A production system would add a materialized balance column and periodically reconcile it.

### 13.3 TransactionManager — Deposit, Withdraw, SendMoney

Each transaction method follows the same pattern:
1. Validate inputs.
2. Check business rules (balance cap, sufficient funds).
3. Insert a `Transactions` row with appropriate `TransactionType` and nullable FKs.
4. Return `OperationResult`.

**SendMoney is the only method that uses an explicit database transaction.** See §12.8 for details on why.

### 13.4 TransactionManager — Recipient Lookup

`FindRecipient(accountNumber, currentUserId)`:
1. Trims the input.
2. `SELECT UserId, AccountNumber, FullName FROM Users WHERE AccountNumber = @AccountNumber`.
3. Validates account exists.
4. Validates `recipient.UserId != currentUserId` (no self-transfers).
5. Returns `RecipientLookupResult`.

### 13.5 ReportManager — Report Queries

All three report methods (`GetStatementOfAccount`, `GetMyDepositsWithdrawals`, `GetSentReceivedHistory`) share a common pattern:

1. Build a SQL query with JOINs to `Users` (once or twice, depending on whether both sender and receiver names are needed).
2. Filter by `userId` (on `FromUserId`, `ToUserId`, or both depending on the report type).
3. Filter by date range: `WHERE TransactionDate >= @StartDate AND TransactionDate < @EndDateExclusive`.
4. The end date is incremented by 1 day (`endDateExclusive = endDate.Date.AddDays(1)`) to make the range inclusive of the full end date (transactions at 11:59 PM on the end date are included).
5. Optional type filter (`AND TransactionType = @TypeFilter`).
6. Order by `TransactionDate DESC, TransactionId DESC`.
7. Execute via `DatabaseHelper.GetDataTable()`.
8. Return the `DataTable` for GridView binding.

**The date-exclusive bound pattern:** Using `< @EndDateExclusive` (where end exclusive = end date + 1 day) is more reliable than `<= @EndDate` because it correctly handles `DATETIME2` values with time components. A transaction at `2024-01-15 23:59:59.999` would be excluded by `<= 2024-01-15 00:00:00` but included by `< 2024-01-16 00:00:00`.

---

## 14. CSS & Styling Architecture

`Content/Style.css` (473 lines) is a hand-written stylesheet — no CSS framework like Bootstrap is used. This keeps the project dependency-free and the styling self-contained.

### 14.1 Design System

| Element | Style |
|---|---|
| **Background** | `#f0f2f5` (light grey — similar to Facebook's background) |
| **Text** | `#333` on `#f0f2f5`, darker on cards |
| **Font** | `'Segoe UI', Tahoma, Geneva, Verdana, sans-serif` |
| **Header** | Gradient from `#1a73e8` to `#0d47a1` (blue), sticky position |
| **Cards** | White background, `8px` border-radius, subtle `box-shadow` |
| **Form inputs** | Full-width, `8px` border-radius, `#1a73e8` focus border with glow |
| **Buttons** | 4 variants (primary blue, success green, danger red, secondary grey) |
| **Messages** | Left-bordered alerts (green/red/blue) |
| **GridView** | Blue header, alternating row colors, hover highlight |

### 14.2 Layout Approaches

- **Header:** Sticky position with flexbox navigation.
- **Content:** Centered container with `max-width: 1000px` and auto margins.
- **Dashboard:** CSS Grid with 4-column responsive layout for stat cards.
- **Forms:** Stacked layout with labels above inputs.
- **Reports:** Inline filter row with date inputs, dropdown, and button on the same line.

### 14.3 Responsive Behavior

A single `@media (max-width: 768px)` breakpoint:
- Header stacks vertically.
- Navigation links wrap and stack.
- Dashboard grid collapses to 2 columns.
- Form inline rows become stacked.
- Full-width inputs.

---

## 15. JavaScript Strategy

**There are no local JavaScript files in this project.** The `Scripts/` directory exists in the project file but is empty. Here's how client-side behavior is achieved:

### 15.1 jQuery via CDN

In `Global.asax.cs`:
```csharp
ScriptManager.ScriptResourceMapping.AddDefinition("jquery", ...);
```

This tells ASP.NET's `ScriptManager` where to find jQuery. The CDN path is tried first; if it fails, the local path (`~/Scripts/jquery-3.7.0.min.js`) is used as fallback. No pages currently use `<asp:ScriptManager>` or include `<script>` tags, but jQuery is available if needed.

### 15.2 Client-Side Behavior Without JavaScript

Several interactive features work without custom JavaScript:

- **Form validation:** ASP.NET validation controls generate JavaScript automatically. `RequiredFieldValidator`, `CompareValidator`, and `TextMode` attributes all produce client-side behavior.
- **Reports dropdown:** Pure CSS `:hover` dropdown. The Reports navigation item has a nested `<div class="dropdown">` with `<div class="dropdown-content">` that appears on hover.
- **PostBack-based interactivity:** Button clicks trigger full-page postbacks (ASP.NET's default model). The `SendMoney.aspx` panel visibility is toggled server-side — no AJAX.

### 15.3 No AJAX / UpdatePanel

The project deliberately avoids `UpdatePanel` (ASP.NET's partial-postback mechanism), `PageMethods`, or any AJAX. All interactions are traditional full-page postbacks. This keeps the page lifecycle straightforward and avoids the complexity of partial-page rendering.

---

## 16. Build, Run & Verify

### 16.1 Prerequisites

1. **Visual Studio 2022** (or 2017/2019 with .NET 4.7.2 targeting pack).
2. **SQL Server LocalDB** — included with Visual Studio. If missing, install via the Visual Studio Installer under "Data storage and processing" workload.
3. **IIS Express** — included with Visual Studio.

### 16.2 Opening the Project

1. Clone the repository or open the folder.
2. Open `CloudMoney.sln` in Visual Studio.
3. Restore NuGet packages (should happen automatically; if not: `Tools > NuGet Package Manager > Restore`).
4. Set `Default.aspx` as the start page (right-click → "Set as Start Page").

### 16.3 Running

- Press `F5` (Debug) or `Ctrl+F5` (Start Without Debugging).
- IIS Express starts on `https://localhost:44389/`.
- On the first request, `Application_Start` fires, which calls `DatabaseHelper.EnsureDatabase()` to create the database tables if they don't exist.
- The application is ready to use.

### 16.4 Testing the Features

| Step | Action | Expected Result |
|---|---|---|
| 1 | Open `https://localhost:44389/` | Redirects to Login page |
| 2 | Click "Register here" | Registration form appears |
| 3 | Register with name, email, password | Shows success with account number |
| 4 | Log in with the credentials | Redirects to Dashboard |
| 5 | Go to Deposit, enter P500 | Balance updates to P500.00 |
| 6 | Go to Withdraw, enter P200 | Balance updates to P300.00 |
| 7 | Register a second account, send P100 | Transfer succeeds, balances adjust |
| 8 | View Statement of Account | Shows all transactions |
| 9 | Change password, log out, log back in | New password works |

### 16.5 Troubleshooting Database Issues

If you see a connection error:
1. Verify LocalDB is installed: open a command prompt and run `sqllocaldb info MSSQLLocalDB`.
2. If it says "not found," install LocalDB from the Visual Studio Installer.
3. Verify the `.mdf` file exists in `App_Data/`.
4. Ensure the IIS Express process has permission to read/write the `App_Data` folder.

---

## 17. Security Considerations

This project is a **learning/demonstration application** and has several security characteristics worth noting:

### 17.1 Plain-Text Password Storage

**Passwords are stored without hashing or salting** in the `PasswordPlainText` column. This means:
- Anyone with database access can read all user passwords.
- A database breach would expose all credentials immediately.
- Password comparison is simple string equality.

**In a production system, you would:**
1. Hash passwords with a strong algorithm (bcrypt, Argon2, or at minimum PBKDF2/HMAC-SHA256).
2. Use a unique salt per password.
3. Store only the hash and salt, never the plain text.
4. Compare by hashing the input with the stored salt.

The plain-text approach in this project was chosen for simplicity and debuggability — you can verify the database state directly.

### 17.2 HTTPS Configuration

The project is configured to use IIS Express with SSL (port `44389`). The `SSLEnabled=true` setting in the `.csproj.user` file enforces HTTPS in development. In production, you would configure a proper SSL certificate.

### 17.3 SQL Injection Prevention

All SQL queries use parameterized statements (`@ParameterName` syntax) via `DatabaseHelper.CreateCommand()`. No string concatenation or interpolation is used to build queries. This prevents SQL injection:

```csharp
// SAFE — parameterized
var parameters = new Dictionary<string, object> { { "@Email", email } };
DatabaseHelper.ExecuteScalar<int>("SELECT COUNT(1) FROM Users WHERE Email = @Email", parameters);

// UNSAFE — never do this
// DatabaseHelper.ExecuteScalar<int>($"SELECT COUNT(1) FROM Users WHERE Email = '{email}'");
```

### 17.4 Input Validation

Every server-side operation validates inputs before processing. Client-side validators (RequiredFieldValidator, CompareValidator) provide a first line of defense, but server-side validation (in `ValidationHelper`, `AuthManager`, `TransactionManager`) is the authoritative gate.

### 17.5 Session Security

- Session timeout is governed by ASP.NET defaults (20 minutes of inactivity by default, though Forms Auth is configured for 60).
- Session ID cookies are HTTP-only by default in ASP.NET, preventing client-side script access.
- No sensitive data (passwords) is stored in session — only UserId, AccountNumber, and FullName.

### 17.6 No CSRF Protection

WebForms has some built-in CSRF protection via `ViewStateUserKey` and event validation, but this project does not explicitly configure anti-CSRF tokens. For a production application, you would add `ValidateAntiForgeryToken` attributes or configure `ViewStateUserKey`.

---

## 18. Learning Path: How to Read This Codebase

If you're new to ASP.NET WebForms or this codebase, follow this reading order to build understanding incrementally:

### Step 1: Understand the foundation (20 minutes)

1. **`Web.config`** — see the framework version, connection string, and auth config.
2. **`Global.asax.cs`** — understand `Application_Start` (database init + jQuery mapping).
3. **`Logic/DatabaseHelper.cs`** — see how SQL connections are managed and how the schema is created.
4. **`Logic/OperationResult.cs`** — understand the result class pattern used everywhere.

### Step 2: Understand authentication (30 minutes)

5. **`Logic/SessionHelper.cs`** — see how session state is managed.
6. **`Logic/AuthManager.cs`** — see registration, login, password change logic.
7. **`Auth/Login.aspx`** + `.cs` — see the login form markup and code-behind.
8. **`Auth/Register.aspx`** + `.cs` — see validation controls and registration flow.
9. **`Auth/Logout.aspx.cs`** — see session termination.

### Step 3: Understand the master page (15 minutes)

10. **`Site.Master`** — see the layout structure, navigation, and message panel.
11. **`Site.Master.cs`** — see how navigation toggles and message display methods work.

### Step 4: Understand the dashboard (15 minutes)

12. **`Dashboard.aspx`** + `.cs` — see data binding, Repeater usage, and balance display.

### Step 5: Understand transactions (30 minutes)

13. **`Logic/ValidationHelper.cs`** — learn the business rules for amounts.
14. **`Logic/TransactionManager.cs`** — study balance computation, deposit, withdrawal, and transfer logic. Pay special attention to `SendMoney()` and its explicit transaction.
15. **`Transactions/Deposit.aspx`** + `.cs` — simple form pattern.
16. **`Transactions/SendMoney.aspx`** + `.cs` — two-step form with HiddenField and Panel visibility.

### Step 6: Understand reports (20 minutes)

17. **`Logic/ReportManager.cs`** — see how report SQL is built with date ranges and filters.
18. **`Reports/StatementOfAccount.aspx`** + `.cs` — see GridView binding and date range handling.

### Step 7: Understand the rest (15 minutes)

19. **`Content/Style.css`** — browse the CSS to understand the visual design system.
20. **`Logic/BalanceSnapshot.cs`**, **`LoginResult.cs`**, etc. — review the remaining DTO classes.

### Key files to search when debugging

| Symptom | Check |
|---|---|
| Can't log in | `AuthManager.Login()` — check plain-text comparison |
| Balance not updating | `TransactionManager.GetBalanceSnapshot()` — check CASE expressions |
| Report empty | `ReportManager.GetStatementOfAccount()` — check date range bounds |
| Session lost | `SessionHelper.SignIn()` and `SignOut()` — check session configuration |
| Page not loading | `SessionHelper.RequireLogin()` — check redirect logic |
| Database error | `DatabaseHelper.EnsureDatabase()` — check table existence checks |

---

## Appendix: ASP.NET WebForms Cheat Sheet

| Concept | Syntax | Notes |
|---|---|---|
| **Page directive** | `<%@ Page ... %>` | Must be the first line of `.aspx` |
| **Master binding** | `MasterPageFile="~/Site.Master"` | In page directive |
| **Code-behind class** | `Inherits="CloudMoney.Dashboard"` | Must match namespace+class |
| **Content placeholder** | `<asp:Content ContentPlaceHolderID="MainContent">` | In content pages |
| **Server control** | `<asp:TextBox runat="server" ID="txtName" />` | `runat="server"` is required |
| **Data binding expression** | `<%# Eval("ColumnName") %>` | Inside data-bound control templates |
| **Code render block** | `<%= variable %>` | Outputs value at render time (like Response.Write) |
| **PostBack check** | `if (!IsPostBack) { ... }` | Guards against re-binding on postback |
| **Validator** | `<asp:RequiredFieldValidator ControlToValidate="txtX" />` | Client + server validation |
| **Button click handler** | `OnClick="btnSave_Click"` | Wired in markup or `AutoEventWireup` |
| **Response redirect** | `Response.Redirect("~/Path.aspx")` | Server-side redirect, throws ThreadAbortException |
| **Session access** | `Session["Key"] = value;` | Type `object`, must cast on read |
| **Master page access** | `(Site)this.Master` | Cast to specific master class |
| **ViewState** | `ViewState["key"] = value;` | Implicitly managed by server controls |
| **Grid binding** | `gv.DataSource = dataTable; gv.DataBind();` | Must call DataBind() |
| **Repeater binding** | Same as GridView | No built-in styling |
| **Panel visibility** | `pnl.Visible = true/false;` | Removes from rendered HTML when false |
| **HTML encoding** | `Server.HtmlEncode(str)` | Always encode user input before rendering |

---

*CloudMoney is a learning project that demonstrates the core patterns of ASP.NET WebForms: master pages, server controls, validation, postback-driven interaction, code-behind organization, ADO.NET data access, and session-based authentication. It is not intended as a production-ready financial application without significant security hardening (password hashing, CSRF protection, HTTPS enforcement, and audit logging).*
