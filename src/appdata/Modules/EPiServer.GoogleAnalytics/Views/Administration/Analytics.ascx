<%@ Control Language="C#" Inherits="ViewUserControl<AdministrationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="EPiServer.Shell.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer.Shell.Web" %>
<%@ Import Namespace="EPiServer.Shell.Navigation" %>

<% using (Html.BeginForm("SaveAnalytics", null, FormMethod.Post, new { id = "SaveForm" })) { %>

<div class="epi-padding">

<div id="disconnected" style="<%= Model.IsAuthenticated ? "display:none" : "" %>">
	<% Html.GaTranslatePrefix("/episerver/googleanalytics/administration/analytics/disconnected/"); %>
	<h2><%= Html.GaTranslate("heading")%></h2>
	<p><%= Html.GaTranslate("description")%></p>
	<span class="epi-button">
		<span class="epi-button-child">
            <!--// Open popup window name=small to authenticate with Google Account-->
			<a class="epi-button-child-item" target="small" href="<%= Url.Action("Index", "Authentication") %>"><%= Html.GaTranslate("signin")%></a>
		</span>
	</span>
</div>

<div id="connected" style="<%= Model.IsAuthenticated ? "" : "display:none" %>">
	<% Html.GaTranslatePrefix("/episerver/googleanalytics/administration/analytics/connected/"); %>
	<h2><%= Html.GaTranslate("heading")%></h2>
	<p><%= Html.GaTranslate("description")%></p>
	
	<%= Html.GaLink("signout", Url.Action("SignOut", "Authentication"), "signout", null)%>

	<div class="roles epi-marginVertical">
		<h2><%= Html.GaTranslate("roles/heading")%></h2>
		<p><%= Html.GaTranslate("roles/description")%></p>
		<%= Html.HiddenFor(avm => avm.Authentication.SharedWith, new { id = "SharedWith" }) %>
		<ul>
			<% foreach (var role in Model.AvailableRoles) { %>
				<li>
					<%= Html.CheckBox(role, Model.Authentication.AllowedRoles.Contains(role), new { @class = "role" })%>
					<label for="<%= role %>"><%= role%></label>
				</li>
			<% } %>
		</ul>
	</div>
</div>

</div>

<div class="epi-buttonContainer">
	<%= Html.GaButton("SaveAnalytics", "submit", "/episerver/googleanalytics/administration/analytics/save")%>
</div>
<% } %>
