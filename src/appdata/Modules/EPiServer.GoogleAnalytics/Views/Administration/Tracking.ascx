<%@ Control Language="C#" Inherits="ViewUserControl<AdministrationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="EPiServer.Shell.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer.Shell.Web" %>
<%@ Import Namespace="EPiServer.Shell.Navigation" %>
<div id="trackingPanel" data-shared="<%= Model.TrackerSettings.Share %>">
<% using(Html.GaTranslatePrefix("/episerver/googleanalytics/administration/tracking/")) {%>
<% using(Html.BeginForm("SaveTracking", null)) { %>
	<div class="epi-padding Sharing">
		<div>
			<%= Html.GaRadio("TrackerSettings.Share", "True", Model.TrackerSettings.Share.ToString(), "sharing/share")%>
		</div>
		<div>
			<%= Html.GaRadio("TrackerSettings.Share", "False", Model.TrackerSettings.Share.ToString(), "sharing/different")%>
		</div>
	</div>

	<% Html.RenderPartial("Tracking/Index", Model); %>

	<div class="epi-buttonContainer">
		<%= Html.GaButton("SaveAnalytics", "submit", "save")%>
	</div>
<% } %>
<% } %>
</div>