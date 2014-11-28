<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AdministrationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% Html.GaTranslatePrefix("/episerver/googleanalytics/administration/tracking/tabs/users/"); %>
<% int i = (int)ViewContext.TempData["SiteIndex"]; %>

<div class="epi-padding">
<div class="option">
	<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].TrackLogins, "logins")%>
</div>
<div class="option">
	<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].TrackVisitorGroups, "visitorgroups")%>
</div>
<div class="option">
	<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].TrackXForms, "xforms")%>
</div>
<div class="option">
	<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].TrackAuthors, "authors")%>
</div>
</div>
