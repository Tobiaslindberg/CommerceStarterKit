<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EPiServer.GoogleAnalytics.Models.Limit>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer.Shell.Web" %>
<% using (Html.GaTranslatePrefix("/episerver/googleanalytics/shared/analyze/daterange/")) { %>
<div class="period clearfix">
    <label class="epi-floatLeft selector-label date-range-label"><%= Html.GaTranslate("period") %>: </label>
    <div class="epi-floatLeft date-range-selector"
            data-mindate="<%= Html.GaGetDateTimeParamString(new DateTime(1970, 1, 1)) %>"
            data-maxdate="<%= Html.GaGetDateTimeParamString(DateTime.Today.AddDays(1)) %>"
            data-startdate="<%= Html.GaGetDateTimeParamString(Model.From) %>"
            data-enddate="<%= Html.GaGetDateTimeParamString(Model.To) %>"
            data-wrongrangemessage="<%= Html.GaTranslate("invalidrange") %>"
            data-selecttext="<%= Html.GaTranslate("select") %>"
            data-canceltext="<%= Html.GaTranslate("cancel") %>">
        <a href="#" class="epi-floatLeft date-range-description"><%= Model.Description %></a>
        <a class="epi-floatLeft epi-iconToolbar-item-link epi-iconToolbar-calendar" href="#" title="<%= Html.GaTranslate("selectrange") %>"></a>
    </div>
</div>
<% } %>