<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%
    var pageViewsLabel = Html.Encode(Html.GaTranslate("/episerver/googleanalytics/shared/analyze/pagesummary/pageviews"));
    var uniquePageViewsLabel = Html.Encode(Html.GaTranslate("/episerver/googleanalytics/shared/analyze/pagesummary/uniqpageviews"));
    var averageTimeOnPagesLabel = Html.Encode(Html.GaTranslate("/episerver/googleanalytics/shared/analyze/pagesummary/avgtimeonpage"));
    var bounceRateLabel = Html.Encode(Html.GaTranslate("/episerver/googleanalytics/shared/analyze/pagesummary/bouncerate"));
    var exitRateLabel = Html.Encode(Html.GaTranslate("/episerver/googleanalytics/shared/analyze/pagesummary/exitrate"));

    var percentOfTotalLabel = Html.Encode(Html.GaTranslate("/episerver/googleanalytics/shared/analyze/pagesummary/percentoftotal"));
    var siteAverageLabel = Html.Encode(Html.GaTranslate("/episerver/googleanalytics/shared/analyze/pagesummary/siteaverage"));

    var noDataMessage = Html.Encode(Html.GaTranslate("/episerver/googleanalytics/shared/message/nodata"));
%>
<ul class="data-list">
    <% if (Model.PageSummary != null) { %>
    <li>
        <span><%= pageViewsLabel %></span>
        <span><%= Model.PageSummary.Pageviews %></span>
        <span>
            <%= Model.PageviewsPercentOfTotal %>
            <%= percentOfTotalLabel %> (<%= Model.Summary.Pageviews %>)
        </span>
    </li>
    <li>
        <span><%= uniquePageViewsLabel %></span>
        <span><%= Model.PageSummary.UniquePageviews %></span>
        <span>
            <%= Model.UniquePageviewsPercentOfTotal %>
            <%= percentOfTotalLabel %> (<%= Model.Summary.UniquePageviews %>)
        </span>
    </li>
    <li>
        <span><%= averageTimeOnPagesLabel %></span>
        <span><%= TimeSpan.FromSeconds(Math.Round(Model.PageSummary.AverageTimeOnPage)).ToString("mm\\:ss") %></span>
        <span>
            <%= siteAverageLabel %>: <%= TimeSpan.FromSeconds(Math.Round(Model.Summary.AverageTimeOnPage)).ToString("mm\\:ss") %>
            (<%= Model.AverageTimeOnPagePercentOfTotal %> %)
        </span>
    </li>
    <li>
        <span><%= bounceRateLabel %></span>
        <span><%= Model.PageSummary.BounceRate.ToString("N2") %> %</span>
        <span>
            <%= siteAverageLabel %>: <%= Model.Summary.BounceRate.ToString("N2") %> % 
            (<%= Model.BounceRateSiteAveragePercentOfTotal %> %)
        </span>
    </li>
    <li>
        <span><%= exitRateLabel %></span>
        <span><%= Model.PageSummary.ExitRate.ToString("N2") %> %</span>
        <span>
            <%= siteAverageLabel %>: <%= Model.Summary.ExitRate.ToString("N2") %> % 
            (<%= Model.ExitRateSiteAveragePercentOfTotal %> %)
        </span>
    </li>
    <% } else { %>
    <li>
        <span><%= pageViewsLabel %></span>
        <span></span>
        <span><%= noDataMessage %></span>
    </li>
    <li>
        <span><%= uniquePageViewsLabel %></span>
        <span></span>
        <span><%= noDataMessage %></span>
    </li>
    <li>
        <span><%= averageTimeOnPagesLabel %></span>
        <span></span>
        <span><%= noDataMessage %></span>
    </li>
    <li>
        <span><%= bounceRateLabel %></span>
        <span></span>
        <span><%= noDataMessage %></span>
    </li>
    <li>
        <span><%= exitRateLabel %></span>
        <span></span>
        <span><%= noDataMessage %></span>
    </li>
    <% } %>
</ul>