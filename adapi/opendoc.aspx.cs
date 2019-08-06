using Microsoft.SharePoint.Client;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security;

namespace adapi
{
    public partial class opendoc : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string dirUrl = "Uploads";
            string dirPath = Server.MapPath(dirUrl);
            string fileName = "";
            ADService dirser = new adapi.ADService();
            try
            {
                string doclink = Request.QueryString["url"];


                if (doclink == null)
                {
                    Response.Write("Incorrect arguments passed");
                }
                else {
                    try
                    {
                        string[] s = doclink.Split('/');
                        fileName = s[s.Length - 1];
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
                            client.DownloadFile(doclink, dirPath + "/" + fileName);
                            client.Dispose();
                            //client.OpenRead(doclink);
                        }
                        Response.Redirect("~/" + dirUrl + "/" + fileName, false);

                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.Message);
                    }

                }
                
            }
            catch (Exception ex) {
                Response.Write(ex.Message);
            }           

        }
    }
}