<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<AuthenticationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
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
	<% Html.GaTranslatePrefix("/episerver/googleanalytics/authentication/"); %>
	<div class="epi-contentContainer epi-padding">
		<p><%= Html.GaTranslate("captcharequired")%></p>
		<div>
			<span class="epi-button">
				<span class="epi-button-child">
					<a class="epi-button-child-item" href="<%= Url.Action("Index", "Authentication", new { isShared = Model.IsShared }) %>"><%= Html.GaTranslate("back")%></a>
				</span>
			</span>
		</div>
	</div>
</body>
</html>