using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace RakshaMIS
{
    public partial class _Default : Page
    {
        
        public bool checkauth = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    checkauth = (bool)HttpContext.Current.Session["authstat"];
                    if (!checkauth)
                    {
                        Response.Redirect("./Login.aspx");
                    }
                    else {
                        ReportParameter[] parameters = new ReportParameter[1];
                        string curuser = "kecrpg\\" + (string)HttpContext.Current.Session["user"];

                        //HttpContext.Current.User.Identity.Name.ToString();
                        parameters[0] = new ReportParameter("LoginID", curuser);
                        ReportViewer1.ServerReport.SetParameters(parameters);
                        ReportViewer1.ServerReport.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    Response.Redirect("./Login.aspx");
                }
            }
        }
    }
}