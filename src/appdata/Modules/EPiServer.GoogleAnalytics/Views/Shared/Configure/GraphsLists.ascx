<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="System.Linq" %>

<div class="epi-googleanalytics-configuration epi-googleanalytics-settings">
<div class="epi-paddingHorizontal-small epi-formArea">
	<% using (Html.BeginGadgetForm("SaveGraphsLists")) { %>
	<% using (Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/configure/graphslists/")) { %>
    
    <p class="epi-marginVertical-small"><%= Html.GaTranslate("header") %></p>

	<% if (Model.AllowCustomTitle) {
		Html.RenderPartial("Configure/Summary", Model);
	} %>
	<% Html.RenderPartial("Configure/Graph", Model); %>
	<% Html.RenderPartial("Configure/List", Model); %>

	<div class="epi-buttonContainer-simple">
		<%= Html.AcceptButton()%>
		<%= Html.CancelButton()%>
	</div>
	<% } %>
	<% } %>
</div>
</div>
<% Html.RenderPartial("Configure/_ShowGadgetContentScript", Model); %>