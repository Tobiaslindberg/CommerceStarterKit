<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<% using (Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/analyze/")) { %>
<div class="graph-by <%= Model.Layout.SelectedLayout %> clearfix">
    <label class="epi-floatLeft selector-label graph-by-label">
        <%= Html.GaTranslate((Model.SelectedGraphs.Count() > 1) ? "graphby/labelgraphs" : "graphby/label") %>: 
    </label>
    <div class="epi-floatLeft graph-by-options-opener">
        <a href="#" class="graph-by-options-placeholder"></a>
        <div class="graph-by-options epi-contextMenu" style="display: none">
            <ul>
                <li><a class="option day" href="#" title="" data-graphby-index="0"><%= Html.GaTranslate("graphby/day") %></a></li>
                <li><a class="option week" href="#" title="" data-graphby-index="1"><%= Html.GaTranslate("graphby/week") %></a></li>
                <li><a class="option month" href="#" title="" data-graphby-index="2"><%= Html.GaTranslate("graphby/month")%></a></li>
            </ul>
        </div>
    </div>
</div>
<% } %>