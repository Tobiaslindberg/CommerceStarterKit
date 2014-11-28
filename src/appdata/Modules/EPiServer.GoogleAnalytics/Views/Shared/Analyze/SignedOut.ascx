<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% using(Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/analyze/signedout/")) { %>
<div class="signedout">
	<div class="signedout-inner">
		<p><%= Html.GaTranslate("unauthorized") %></p>
		<p><a href="#" class="signin"><%= Html.GaTranslate("login") %></a> <%= Html.GaTranslate("toaccessdata") %>.</p>
	</div>
</div>
<% } %>