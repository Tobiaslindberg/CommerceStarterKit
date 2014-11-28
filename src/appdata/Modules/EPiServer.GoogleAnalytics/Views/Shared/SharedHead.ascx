<%@ Control Language="C#" Inherits="ViewUserControl<object>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="EPiServer.Shell.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer.Shell.Web" %>
<%@ Import Namespace="EPiServer.Shell.Navigation" %>

<%= Page.ClientResources("ShellCore")%>
<%= Page.ClientResources("ShellCoreLightTheme")%>
<%--<%= Html.ShellInitializationScript() %>--%>
<%= Html.CssLink(EPiServer.Web.PageExtensions.ThemeUtility.GetCssThemeUrl(Page, "system.css"))%>
<%= Html.CssLink(EPiServer.Web.PageExtensions.ThemeUtility.GetCssThemeUrl(Page, "ToolButton.css"))%>
<!--[if IE 6]>
<%= Html.CssLink(EPiServer.Web.PageExtensions.ThemeUtility.GetCssThemeUrl(Page, "IE6.css"))%>
<%= Html.CssLink(EPiServer.Web.PageExtensions.ThemeUtility.GetCssThemeUrl(Page, "IE.css"))%>
<![endif]-->