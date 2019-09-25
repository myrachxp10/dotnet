using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try {
            string s = Request.QueryString["error"];
            lblMessage.Text = s;
            dvMessage.Visible = true;
        }
        catch (Exception ex) {

        }
       
    }
    protected void ValidateUser(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();

        byte[] bytes = Encoding.ASCII.GetBytes(username + ":" + password);
        string authheader = Convert.ToBase64String(bytes);
        string html = "";
        string url = "https://digital.kecrpg.com/adapi/v1/auth/" + authheader;
        username = username.Replace("@kecrpg.com", "");
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.AutomaticDecompression = DecompressionMethods.GZip;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            html = reader.ReadToEnd();

        }
        if (html.Equals("true"))
        {
            Session.Add("authstat", true);
            Session.Add("user", username);
            Response.Redirect("./Default.aspx");
        }
        else
        {
            Session.Add("authstat", false);
            lblMessage.Text = "Incorrect UserName or Password.";
            dvMessage.Visible = true;
        }


        //FormsAuthentication.SetAuthCookie(username, chkRememberMe.Checked);


    }
}