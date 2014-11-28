<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% using(Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/analyze/")) {%>
<div class="epi-googleanalytics epi-googleanalytics-settings epi-googleanalytics-signedout" 
	data-dojoPath="<%= EPiServer.Shell.Paths.ToShellClientResource("ClientResources/") %>"
	data-account-shared="<%= ViewData["SelectedAccount"] %>">
    
    <!-- view Shared/Unauthorized -->
	<% Html.RenderPartial("Analyze/SignedOut"); %>
</div>
<% } %>

