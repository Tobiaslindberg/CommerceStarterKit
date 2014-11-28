<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer.Shell.Web" %>

<% using(Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/analyze/")) {%>
<div class="edit-mode epi-googleanalytics epi-googleanalytics-settings <%= Model.IsAuthenticated ? "epi-googleanalytics-signedin" : "epi-googleanalytics-signedout" %>" 
    data-account-shared="<%= Model.Account.UseSharedAccount %>">
    <div class="signedin">
        <% if(Model.Layout.DisplayHeading) { %>
            <h2 class="gadget-heading" data-text-template="<%= Html.Encode(Html.GaTranslate("/episerver/googleanalytics/context/heading")) %>"></h2>
        <% } %>
        <div class="graph-settings">
            <%= Html.Partial("Analyze/DateRange", Model.SelectedRange) %>
            <div class="segment-filter"></div>
        </div>
        <% Html.RenderPartial("Analyze/Graphs", Model); %>
        <div class="page-summary"></div>
        <div class="lists-container">
            <% Html.RenderPartial("Analyze/Lists", Model); %>
        </div>
    </div>
    <!-- Signedout partial view in Context/Index, using in EditView widget -->
    <% Html.RenderPartial("Analyze/SignedOut"); %>
</div>
<% } %>
<script type="text/javascript">
    $(document).ready(function () {
        epi.googleAnalytics.notSupportContentTypes = <%= ViewData["NotSupportContentTypes"] %>;
        epi.googleAnalytics.selectedProfile = <%= ViewData["SelectedProfile"] %>;
        epi.googleAnalytics.isAuthenticated = <%= ViewData["IsAuthenticated"] %>;
    });
</script>