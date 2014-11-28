<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TemporalMetricBase>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models.MetricImplementations" %>

<div class="notenoughdata"><%= Html.GaTranslate("/episerver/googleanalytics/dashboard/analyze/graphs/notenoughdata")%></div>
<div id="<%= Html.GaId(Model.Name) %>" class="metric chart" 
	data-name="<%= Model.Name %>" 
	data-metric-label="<%= Html.GaTranslate(Model.TranslationKey, Model.DisplayName) %>" 
	data-date-label="<%= Html.GaTranslate("/episerver/googleanalytics/dashboard/analyze/graphs/tooltip/datelabel") %>">
	&nbsp;
</div>