//using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.WebForms;
using System;
using System.Web;

namespace RakshaMIS
{
    public partial class UA_UC : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ReportViewer1.ServerReport.ReportServerCredentials = (Microsoft.Reporting.WebForms.IReportServerCredentials)System.Net.CredentialCache.DefaultCredentials;
            if (!IsPostBack)
            {
                ReportParameter[] parameters = new ReportParameter[1];
                string curuser = HttpContext.Current.User.Identity.Name.ToString();
                parameters[0] = new ReportParameter("LoginID", curuser);
                ReportViewer1.ServerReport.SetParameters(parameters);
                ReportViewer1.ServerReport.Refresh();
            }
        }
       
    }
}