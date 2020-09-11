<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Raksha MIS</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="Header">
            <div style="width:90%;float:left;background-color:black;font-family:Arial;font-size:larger;color:whitesmoke">Raksha MIS</div><div style="width:10%;float:Right;background-color:black;font-family:Arial;font-size:larger;color:whitesmoke"><asp:LinkButton ID="lnkbutton" runat="server" OnClick="lnkbutton_Click" >Logout</asp:LinkButton></div>
        </div>
        <asp:ScriptManager ID="scmgr" runat="server"></asp:ScriptManager>      
        <!--<div style="width:100%;align-items:center;margin-left:20px;margin-right:20px"> -->   
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" ZoomMode="PageWidth" Width="100%" Height="600">
                
            </rsweb:ReportViewer>
        <!--</div>-->
         <!--<div style="margin-left:50px;margin-right:50px;font-family:Arial;text-align:center">
            
       </div>-->
    </form>
</body>
</html>
