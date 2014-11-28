<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer.Shell.Web" %>

<% using(Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/analyze/")) {%>
<div class="dashboard-mode epi-googleanalytics epi-googleanalytics-settings <%= Model.IsAuthenticated ? "epi-googleanalytics-signedin" : "epi-googleanalytics-signedout" %>" 
    data-account-shared="<%= Model.Account.UseSharedAccount %>">
    <!-- display Panel for showing GA data, chart -->
    <div class="signedin epi-padding-small">
        <% if(Model.Layout.DisplayHeading) { %>
            <h2 class="gadget-heading"><%= Html.Encode(string.IsNullOrEmpty(Model.Layout.Heading) ? Model.SelectedWebProperty.Name : Model.Layout.Heading) %></h2>
        <% } %>
        <div class="graph-settings">
            <%= Html.Partial("Analyze/DateRange", Model.SelectedRange) %>
            <div class="segment-filter"></div>
        </div>
        <% if (Model.Statistics.DisplaySummary) { %>
        <div class="summary">
            <% Html.RenderPartial("Analyze/Summary", Model.Summary); %>
        </div>
        <% } %>
        <% Html.RenderPartial("Analyze/Graphs", Model); %>
        <div class="lists-container">
            <% Html.RenderPartial("Analyze/Lists", Model); %>
        </div>
    </div>
    <!-- display signout Panel in main view of GA gadget in Dashboard -->
    <% Html.RenderPartial("Analyze/SignedOut"); %>
</div>
<% } %>
<script type="text/javascript">
    $(document).ready(function () {
        epi.googleAnalytics.selectedProfile = <%= ViewData["SelectedProfile"] %>;
        epi.googleAnalytics.isAuthenticated = <%= ViewData["IsAuthenticated"] %>;
    });
</script>