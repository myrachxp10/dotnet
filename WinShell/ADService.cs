using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.DirectoryServices;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Collections;
using System.Net.Mail;
using System.Net;
using Json.Net;

namespace adapi
{
    public class ADService
    {

        private static string sPath = "dc=kecrpg,dc=com";//ConfigurationManager.AppSettings["ldap"].ToString();
       

        private DirectoryEntry myDirectory = new DirectoryEntry("LDAP://kecrpg.com", "appdev", "Kec#9102"); // pass the user account and password for your Enterprise admin.

        public static JToken GetList(Uri webUri, ICredentials credentials, string listTitle)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                client.Credentials = credentials;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
                client.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
                var endpointUri = new Uri(webUri, string.Format("/_api/web/lists/getbytitle('{0}')", listTitle));
                var result = client.DownloadString(endpointUri);
                var t = JToken.Parse(result);
                return t["d"];
            }
        }

    }
}