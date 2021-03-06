﻿using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
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
                else
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                   
                    ReportViewer1.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["serverURL"].ToString());
                    ReportViewer1.ServerReport.ReportPath = "/RakshaMIS/Home";
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
                //Response.Write("Error " + ex.ToString());
                if(ex.Message== "Object reference not set to an instance of an object.") 
                    Response.Redirect("./Login.aspx");
                else
                    Response.Redirect("./Login.aspx?error=" + ex.Message);
            }
        }
    }

    protected void lnkbutton_Click(object sender, EventArgs e)
    {
        Session.Remove("authstat");
        Response.Redirect("./Login.aspx?error=logged out.");
    }
}