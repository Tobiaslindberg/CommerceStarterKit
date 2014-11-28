<%@ Page Language="c#" CodeBehind="Login.aspx.cs" AutoEventWireup="False" Inherits="EPiServer.UI.Util.Login" Title="Login" %>
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
        <script type="text/javascript">

            function toggleCookieText() {
                var cookieInfoPanel = document.getElementById("cookieInfoPanel");
                cookieInfoPanel.style.display = (cookieInfoPanel.style.display == "block" ? "none" : "block");
                return false;
            }
        </script>

        <div class="epi-loginContainer">
            <episerverui:login id="LoginControl" runat="server" failuretext="<%$ Resources: EPiServer, login.loginfailed %>" enableajaxlogin="true">
            <LayoutTemplate>
                <div class="epi-loginTop">
                </div>
                <div class="epi-loginMiddle">
                    <div class="epi-loginContent">
                        <div class="epi-loginLogo">EPiServer</div>
                        <div class="epi-loginForm">
                            <h1><span style="color:Red;"><asp:Literal runat="server" ID="FailureText" /></span></h1>
                           
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List" EnableClientScript="true" />
                            <div class="epi-credentialsContainer">

                                <div class="epi-float-left">
                                    <asp:Label ID="Label133" AssociatedControlID="UserName" Text="<%$ Resources: EPiServer, login.username %>"
                                        CssClass="episize80" runat="server" /><br />
                                    <asp:TextBox SkinID="Custom" CssClass="epi-inputText" ID="UserName" runat="server" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ErrorMessage="<%$ Resources: EPiServer, login.usernamerequired %>"
                                        Text="&#173;" ControlToValidate="UserName" Display="Dynamic" runat="server" />
                                </div>
                                <div class="epi-float-left">
                                    <asp:Label ID="Label2" AssociatedControlID="Password" Text="<%$ Resources: EPiServer, login.password %>"
                                        CssClass="episize80" runat="server" /><br />
                                    <asp:TextBox SkinID="Custom" CssClass="epi-inputText" ID="Password" Text="password"
                                        runat="server" TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ErrorMessage="<%$ Resources: EPiServer, login.missingpassword %>"
                                        Text="&#173;" ControlToValidate="Password" Display="Dynamic" runat="server" />                                    
                                </div>
                                <div class="epi-button-container epi-float-left">
                                    <span class="epi-button">
                                        <span class="epi-button-child">
                                            <asp:Button ID="Button1" CssClass="epi-button-child-item" CommandName="Login" Text="<%$ Resources: EPiServer, button.login %>"
                                                runat="server" />
                                        </span>
                                    </span>
                                </div>
                                <div class="epi-checkbox-container">
                                    <asp:CheckBox ID="RememberMe" CssClass="epi-checkbox" runat="server" />
                                    <asp:Label ID="Label1" AssociatedControlID="RememberMe" Text="<%$ Resources: EPiServer, login.persistentlogin %>"
                                        runat="server" />
                                </div>
                            </div>
                            <p>
                                <a href="#" onclick="toggleCookieText(); return false;">
                                    <asp:Literal ID="Literal1" Text="<%$ Resources: EPiServer, cookie.logincaption %>"
                                        runat="server" />
                                </a>
                                <div id="cookieInfoPanel" style="display: none; text-align: left;">
                                    <br />
                                    <asp:Literal ID="Literal2" Text="<%$ Resources: EPiServer, cookie.logininfo %>" runat="server" />
                                </div>
                            </p>
                        </div>
                    </div>
                </div>
                <div class="epi-loginBottom">
                </div>
            </LayoutTemplate>
        </episerverui:login>
        </div>
    </form>
</body>
</html>
