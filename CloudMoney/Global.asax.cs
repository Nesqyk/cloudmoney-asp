using System;
using System.Web;
using System.Web.UI;
using CloudMoney.Logic;

namespace CloudMoney
{
    public partial class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            DatabaseHelper.EnsureDatabase();

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "jquery",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/jquery-3.7.0.min.js",
                    DebugPath = "~/Scripts/jquery-3.7.0.js",
                    CdnPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.0.min.js",
                    CdnDebugPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.0.js",
                    CdnSupportsSecureConnection = true
                }
            );
        }
    }
}
