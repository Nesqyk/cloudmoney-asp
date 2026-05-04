using System;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace CloudMoney.Logic
{
    public static class SessionHelper
    {
        public static bool IsLoggedIn(HttpSessionStateBase session)
        {
            return session["UserId"] != null;
        }

        public static bool IsLoggedIn(HttpSessionState session)
        {
            return session["UserId"] != null;
        }

        public static int GetUserId(HttpSessionState session)
        {
            return Convert.ToInt32(session["UserId"]);
        }

        public static string GetAccountNumber(HttpSessionState session)
        {
            return Convert.ToString(session["AccountNumber"]);
        }

        public static string GetFullName(HttpSessionState session)
        {
            return Convert.ToString(session["FullName"]);
        }

        public static void SignIn(HttpSessionState session, LoginResult loginResult)
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
            if (page.Session["UserId"] != null)
            {
                return true;
            }

            page.Response.Redirect("~/Auth/Login.aspx");
            return false;
        }
    }
}
