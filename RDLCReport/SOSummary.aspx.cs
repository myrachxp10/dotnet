using Microsoft.Reporting.WebForms;
using RDLCReport.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RDLCReport
{
    public partial class SOSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int EhsAssessmentId = Convert.ToInt32(Request.QueryString["EhsAssesmentID"]);
            if (!Page.IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Remote;

                ReportViewer1.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["serverURL"].ToString());
                ReportViewer1.ServerReport.ReportPath = "/RakshaMIS/EHSScore";
                ReportParameter[] parameters = new ReportParameter[1];
                parameters[0] = new ReportParameter("ScoreID", Request.QueryString["EhsAssesmentID"]);
                ReportViewer1.ServerReport.SetParameters(parameters);
                ReportViewer1.ServerReport.Refresh();
            }
        }

    }
}