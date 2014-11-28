<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AdministrationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% Html.GaTranslatePrefix("/episerver/googleanalytics/administration/tracking/tabs/links/"); %>
<% int i = (int)ViewContext.TempData["SiteIndex"]; %>

<div class="epi-padding links">
<div class="option">
	<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].TrackExternalLinks, "externallinks/label")%>

	<div class="dependant">
		<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].TrackExternalLinksAsPageViews, "trackexternalaspageviews/label")%>
		<em><%= Html.GaTranslate("trackexternalaspageviews/description")%></em>
	</div>
</div>
<div class="option">
	<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].TrackMailto, "mailto/label")%>
</div>
<div class="option">
	<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].TrackDownloads, "downloads/label")%>
	<div class="dependant">
		<%= Html.TextBoxFor(m => m.TrackerSettings.Sites[i].TrackDownloadExtensions) %>
		<em><%= Html.GaTranslate("downloads/description")%></em>
	</div>
</div>
</div>
