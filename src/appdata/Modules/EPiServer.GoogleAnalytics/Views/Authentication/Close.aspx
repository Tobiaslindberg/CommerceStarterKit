<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<AuthenticationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Helpers" %>
<%@ Import Namespace="EPiServer.Shell.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer.Shell.Web" %>
<%@ Import Namespace="EPiServer.Shell.Navigation" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Index</title>
        <% Html.RenderPartial("SharedHead"); %>
        <%= Page.ClientResources("AnalyticsAdministration") %>
    </head>
    <body>
        <%= Html.Partial("Info", "/episerver/googleanalytics/authentication/").ToHtmlString().Replace("~/", ResolveUrl("~/")) %>
        <script>
            // This page will be opened in the popup window name=small, so it call the userAuthenticatedCallback() and then close the popup window
            if (window.opener && window.opener.userAuthenticatedCallback) {
                // Will convert the given model object to Json data string.
                // The structure of the converted Json data string can be:
                //      {
                //          username: [string],
                //          isShared: [boolean],
                //          responseResult: {
                //              status: [string],
                //              code: [string],
                //              message: [string],
                //              technicalMessage: [string]
                //          }
                //      }
                window.opener.userAuthenticatedCallback(<%= Model.ToJson() %>);
            }
            window.close();
        </script>
    </body>
</html>