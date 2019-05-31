using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace adapi
{
    public partial class opendoc : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string dirUrl = "Uploads";
            string dirPath = Server.MapPath(dirUrl);
            string fileName = "";
            try
            {
                string doclink = Request.QueryString["url"];               
                string[] s = doclink.Split('/');
                fileName = s[s.Length - 1];
                
                if (doclink.Equals(null))
                {
                    Response.Write("Incorrect arguments passed");
                }
                try
                {

                    string _uid = ConfigurationManager.AppSettings["spUser"].ToString();
                    string _pwd = ConfigurationManager.AppSettings["spPWD"].ToString();
                    

                    // Check for Directory, If not exist, then create it  

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var securedPassword = new SecureString();
                    foreach (var c in _pwd.ToCharArray()) securedPassword.AppendChar(c);
                    var credentials = new SharePointOnlineCredentials(_uid, securedPassword);
                    
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                        client.Headers.Add("User-Agent: Other");
                        client.Credentials = credentials;                        
                        client.DownloadFile(doclink, dirPath +"/"+ fileName);
                        client.Dispose();
                        //client.OpenRead(doclink);
                    }
                    Response.Redirect("~/" + dirUrl + "/" + fileName,false);

                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
            }
            catch (Exception ex) {
                Response.Write(ex.Message);
            }           

        }
    }
}