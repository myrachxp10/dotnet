using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;

namespace RDLCReport
{
    public partial class Home : System.Web.UI.Page
    {
        public bool checkauth = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    string token = Request.QueryString["key"];
                    string dtpart = DateTime.Now.ToString("ddMMyyyy");
                    var base64EncodedBytes = System.Convert.FromBase64String(token);
                    if (base64EncodedBytes.Length != 0)
                    {
                        string curuser = "kecrpg\\" + System.Text.Encoding.UTF8.GetString(base64EncodedBytes).ToLower().Replace("@kecrpg.com", "").Replace(dtpart,"");
                        ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                        ReportViewer1.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["serverURL"].ToString());
                        ReportViewer1.ServerReport.ReportPath = "/RakshaMIS/Home";
                        ReportParameter[] parameters = new ReportParameter[1];
                        //HttpContext.Current.User.Identity.Name.ToString();
                        parameters[0] = new ReportParameter("LoginID", curuser);
                        ReportViewer1.ServerReport.SetParameters(parameters);
                        ReportViewer1.ServerReport.Refresh();
                    }
                    else {
                        Response.Write("Unauthorised access.");
                    }
                }
                catch (Exception ex)
                {
                    //Response.Write("Error " + ex.ToString());
                    if (ex.Message == "Object reference not set to an instance of an object.")
                        Response.Write("Unknown exception occurred.");
                    else
                        Response.Write("Error=" + ex.Message);
                }
            }
        }
    }
}