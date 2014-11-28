<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<%  using(Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/configure/lists/")) { %>

<fieldset>
	<legend><%= Html.GaTranslate("legend") %></legend>
	
	<div class="SelectedGraphs epi-marginVertical-small">
		<input type="hidden" name="Statistics.SelectedLists" />
		<% foreach (var metric in Model.ListMetrics) { %>
			<%= Html.GaCheckbox(metric.Name, "Statistics.SelectedLists", metric.Name, Model.Statistics.SelectedLists, metric.TranslationKey, metric.DisplayName)%>
		<% } %>
	</div>
</fieldset>

<% } %>