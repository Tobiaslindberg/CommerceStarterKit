<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Logout.aspx.cs" Inherits="EPiServer.Util.Logout" Title="Logout" %>

<%@ Import Namespace="System.Threading" %>
<%@ OutputCache Location="None" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" lang="<%= Thread.CurrentThread.CurrentUICulture.Name %>" xml:lang="<%= Thread.CurrentThread.CurrentUICulture.Name %>">
<head id="Head1" runat="server">
    <title id="Title1" runat="server" />
    <meta name="robots" content="noindex,nofollow" />
    <link type="text/css" rel="stylesheet" href="styles/login.css" />
</head>
<body class="epi-loginBody">
    <form id="aspnetForm" runat="server">
        <div class="epi-loginContainer">
            <div class="epi-loginTop">
            </div>
            <div class="epi-loginMiddle">
                <div class="epi-loginContent">
                    <div class="epi-loginLogo">EPiServer</div>
                    <div class="epi-credentialsContainer-logout">
                        <h1>
                            <asp:Label runat="server" ID="LogOutLabel" /></h1>
                        <p id="DivLogOutText">
                            <asp:Label ID="logouttext" runat="server" Visible="false" /></p>
                        <p id="DivHyperLinkLogin">
                            <asp:HyperLink ID="hyperLinkToLogin" Visible="false" runat="server" /></p>
                    </div>
                </div>
            </div>
            <div class="epi-loginBottom">
            </div>
        </div>
    </form>
</body>
</html>