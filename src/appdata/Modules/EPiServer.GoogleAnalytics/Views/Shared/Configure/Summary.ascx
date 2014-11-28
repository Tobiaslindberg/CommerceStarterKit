<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% using (Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/configure/summary/")) { %>
<fieldset>
	<legend><%= Html.GaTranslate("legend")%></legend>

	<div class="epi-marginVertical-small">
		<%= Html.GaCheckboxFor(m => m.Statistics.DisplaySummary, "label")%>
	</div>
</fieldset>
<% } %>
