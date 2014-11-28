<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<% if (Model.SelectedGraphs.Any()) { %>
    <% if (Model.Layout.SelectedLayout == "stacked") { %>
    <div class="stacked-graph clearfix">
        <%= Html.Partial("Analyze/GraphBy", Model) %>
        <% foreach (var metric in Model.SelectedGraphs) { %>
            <h3><%= Html.GaTranslate(metric.TranslationKey, metric.DisplayName)%></h3>
            <div class="graph loading">
                <%= Html.Partial("Analyze/Graph", metric)%>
            </div>
        <% } %>
    </div>
    <% } else { %>
    <div id="<%= Html.GaId("tabs") %>" class="ga-tabbed-graph clearfix" data-dojo-type="dijit.layout.TabContainer">
        <%= Html.Partial("Analyze/GraphBy", Model) %>
        <%--
        // TECHNOTE:
        //      For special case, the tab name contains XSS code, like: "<script></script>", dojo tab name will not render correctly.
        //      So that, we should bind tab name manually.
        //      The tab name will set as text, not inner HTML.
        --%>
        <% foreach (var metric in Model.SelectedGraphs) { %>
            <div data-dojo-type="dijit.layout.ContentPane" data-tab-name="<%= Html.GaTranslate(metric.TranslationKey, metric.DisplayName) %>" class="tab graph">
                <%= Html.Partial("Analyze/Graph", metric)%>
            </div>
        <% } %>
    </div>
    <% } %>
<% } %>