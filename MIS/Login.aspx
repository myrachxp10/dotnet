<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 131px;
            height: 99px;
        }
    </style>
</head>
<body>
    <script type="text/javascript" src='https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js'></script>
<script type="text/javascript" src='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/js/bootstrap.min.js'></script>
<link rel="stylesheet" href='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/css/bootstrap.min.css'
    media="screen" />
<form id="form1" runat="server">
   
<div style="margin-left: 300px;margin-right: 300px;">
    <div style="text-align:center">
        <img alt="Raksha" class="auto-style1" src="SafeyAppLogo.png" /></div>
    <h2 class="form-signin-heading">
        MIS
        Login</h2>
    <label for="txtUsername">
        Username</label>
    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"  placeholder="Enter KEC Email Address."   required />example: xyz@kecrpg.com
    <br />
    <label for="txtPassword">
        Password</label>
    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"
        placeholder="Enter Password" required />
    <div class="checkbox">
        <asp:CheckBox ID="chkRememberMe" Text="Remember Me" runat="server" visible="false"/>
    </div>
    <asp:Button ID="btnLogin" Text="Login" Width="100%" runat="server" OnClick="ValidateUser" Class="btn btn-primary" />
    <br />
    <br />
    <div id="dvMessage" runat="server" visible="false" class="alert alert-danger">
        
        <asp:Label ID="lblMessage" runat="server" />
    </div>
</div>
    </form>
</body>
</html>
