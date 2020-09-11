<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestUpload.aspx.cs" Inherits="adapi.TestUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form2" runat="server">
        <div>
            FileName with extension:
            <asp:TextBox ID="txtFileName" runat="server"></asp:TextBox>
            <br />
            File Display Name:
            <asp:TextBox ID="txtFileDisplayName" runat="server"></asp:TextBox>
            <br />
            Folder :
            <asp:TextBox ID="txtFolderName" runat="server"></asp:TextBox>
             <br />
            <asp:TextBox ID="txtImageString" runat="server" Height="306px" TextMode="MultiLine" Width="769px"></asp:TextBox>
            <br />
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Upload" />
            <br />
        </div>
    </form>
    
</body>
</html>
