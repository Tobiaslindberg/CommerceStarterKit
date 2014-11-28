<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% if (Model.SelectedListMetrics.Any()) { %>
    <div id="<%= Html.GaId("lists") %>" class="lists">
        <div class="loading" style="min-height:60px">&nbsp;</div>
    </div>
<% } %>