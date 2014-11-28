<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SummaryData>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<div class="<%= Model.Loaded ? "" : "summary-pendingdata" %>">
    <% using (Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/analyze/summary/")) { %>
    <div class="title summary-large">
        <div class="huge">
            <label><%= Html.GaTranslate("visits") %></label>
            <span><%= Model.Visits %></span>
        </div>
        <div class="tiny">
            <span><%= Model.UniqueVisitors %></span>
            <label><%= Html.GaTranslate("uniquevisits") %></label>
        </div>
        <div class="tiny">
            <span><%= Model.Pageviews %></span>
            <label><%= Html.GaTranslate("pageviews") %></label>
        </div>
    </div>
    <div class="title summary-small">
        <label class="heading"><%= Html.GaTranslate("avgtimeonesite") %></label>
        <span><%= TimeSpan.FromSeconds(Math.Round(Model.AverageTimeOnSite)).ToString("hh\\:mm\\:ss") %></span>
    </div>
    <div class="title summary-small">
        <label class="heading"><%= Html.GaTranslate("goalstotal") %></label>
        <span><%= Model.TotalGoals %></span>
    </div>
    <div class="title summary-small">
        <label class="heading"><%= Html.GaTranslate("bouncerate") %></label>
        <span><%= Model.BounceRate.ToString("N2") %>%</span>
    </div>
    <div class="title summary-small">
        <label class="heading"><%= Html.GaTranslate("totalevents") %></label>
        <span><%= Model.TotalEventOccurances %></span>
    </div>
    <% } %>
</div>