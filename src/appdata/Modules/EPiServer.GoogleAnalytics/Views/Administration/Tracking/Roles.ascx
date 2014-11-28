<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AdministrationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% using(Html.GaTranslatePrefix("/episerver/googleanalytics/administration/tracking/tabs/roles/")){ %>
<% int i = (int)ViewContext.TempData["SiteIndex"]; %>

<div class="epi-padding">
	
	<% using(Html.IdSuffix("Roles")) { %>
	<div class="option">
		<%= Html.GaRadio(ExpressionHelper.GetExpressionText("TrackerSettings.Sites[" + i + "].ExcludeRoles"), "False", Model.TrackerSettings.Sites[i].ExcludeRoles.ToString(), "include") %>
	</div>
	<div class="option">
		<%= Html.GaRadio(ExpressionHelper.GetExpressionText("TrackerSettings.Sites[" + i + "].ExcludeRoles"), "True", Model.TrackerSettings.Sites[i].ExcludeRoles.ToString(), "exclude")%>
	</div>
	<% } %>

	<div class="epi-paddingVertical-small option">
	<ul>
		<% foreach (var role in Model.AvailableRoles) { %>
			<li>
				<%= Html.GaCheckboxFor(m => m.TrackerSettings.Sites[i].Roles, role, Model.TrackerSettings.Sites[i].Roles, role, role) %>
			</li>
		<% } %>
	</ul>
	</div>
</div>
<% } %>
