<%@  Page Language="C#" Inherits="System.Web.Mvc.ViewPage<AdministrationViewModel>" %>
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
	<title></title>
	<% Html.RenderPartial("SharedHead"); %>
	<%= Page.ClientResources("AnalyticsAdministration") %>
</head>
<body>
	<div class="epi-contentContainer epi-padding">
		<%= Html.Partial("Info", "/episerver/googleanalytics/administration/").ToHtmlString().Replace("~/", ResolveUrl("~/")) %>
		<% Html.GaTranslatePrefix("/episerver/googleanalytics/administration/"); %>

        <% using(Html.GaTranslatePrefix("/episerver/googleanalytics/administration/summary/")) { %>
        <div id="TrackingEnabled" class="epi-margin <%= Model.TrackingConfigured ? "ok" : "missing" %>">
	        <span class="ok-text"><%= Html.GaTranslate("trackingenabled") %></span>
	        <span class="missing-text"><%= Html.GaTranslate("trackingdisabled") %></span>
        </div>
        <div id="GadgetSetup" class="epi-margin <%= Model.IsAuthenticated ? "ok" : "missing" %>">
	        <span class="ok-text"><%= string.Format(Html.GaTranslate("authenticationconfigured"), Model.AuthenticationSharedWith) %></span>
	        <span class="missing-text"><%= string.Format(Html.GaTranslate("authenticationunconfigured"), Model.AuthenticationSharedWith) %></span>
        </div>
        <% } %>

		<div class="epi-formArea epi-marginVertical">

			<div class="epi-gridColumn epi-tabView" id="tabs">
				<ul class="epi-tabView-navigation" role="tablist">
					<li class="epi-tabView-navigation-item-selected epi-tabView-navigation-item">
						<a href="#" id="tracking" class="epi-tabView-tab" role="tab" tabindex="0"><%= Html.GaTranslate("/episerver/googleanalytics/administration/tracking/tab") %></a>
					</li>
					<li class="epi-tabView-navigation-item">
						<a href="#" id="analytics" class="epi-tabView-tab" role="tab" tabindex="0"><%= Html.GaTranslate("/episerver/googleanalytics/administration/analytics/tab") %></a>
					</li>
				</ul>
				<div class="epi-topTabPanelContainer epi-scrollable">
					<div>
						<% Html.RenderPartial("Tracking", Model); %>
					</div>
					<div>
						<% Html.RenderPartial("Analytics", Model); %>
					</div>
				</div>
			</div>
		</div>
	</div>
</body>
</html>
