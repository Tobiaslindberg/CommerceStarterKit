<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AdministrationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="System.Linq" %>
<div id="shared">
	<% ViewContext.TempData["SiteIndex"] = 0; %>
	<% Html.RenderPartial("Tracking/Site", Model); %>
</div>
<div id="different">
	<div class="epi-gridColumn epi-tabView" id="siteSettings">
		<ul class="epi-tabView-navigation" role="tablist">
	<% for (int i = 1; i < Model.TrackerSettings.Sites.Count; i++) { %>
		<% var site = Model.TrackerSettings.Sites[i]; %>
			<li class="epi-tabView-navigation-item">
				<a href="#" id="<%= site.SiteId %>" class="epi-tabView-tab" role="tab" tabindex="0"><%= site.SiteName %></a>
			</li>
	<% } %>
		</ul>
		<div class="epi-sitesTabPanelContainer epi-scrollable">
	<% for (int i = 1; i < Model.TrackerSettings.Sites.Count; i++) { %>
		<% if (Model.TrackerSettings.Sites[i].RemovedFromConfig) { continue; } %>
		<% ViewContext.TempData["SiteIndex"] = i; %>
			<div class="epi-padding">
			<%= Html.HiddenFor(m => m.TrackerSettings.Sites[i].SiteId)%>
            <%= Html.HiddenFor(m => m.TrackerSettings.Sites[i].SiteName)%>
			<% Html.RenderPartial("Tracking/Site", Model); %>
			</div>
	<% } %>
		</div>
	</div>
</div>
