<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AdministrationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% int i = (int)ViewContext.TempData["SiteIndex"]; %>
<% var selectedValue = Model.TrackerSettings.Sites[i].DomainsMode; %>
<div class="epi-padding domains" selected-mode="<%= Model.TrackerSettings.Sites[i].DomainsMode %>">
	<div class="option">
		<%= Html.GaRadioFor(m => m.TrackerSettings.Sites[i].DomainsMode, "singledomain", selectedValue, "tabs/domain/singledomain/label")%>
		<em><%= Html.GaTranslate("tabs/domain/singledomain/info") %></em>
	</div>
	<div class="option">
		<%= Html.GaRadioFor(m => m.TrackerSettings.Sites[i].DomainsMode, "subdomains", selectedValue, "tabs/domain/subdomains/label")%>
		<em><%= Html.GaTranslate("tabs/domain/subdomains/info") %></em>
	</div>
	<div class="option">
		<%= Html.GaRadioFor(m => m.TrackerSettings.Sites[i].DomainsMode, "multidomains", selectedValue, "tabs/domain/multidomains/label")%>
		<em><%= Html.GaTranslate("tabs/domain/multidomains/info") %></em>
        <div class="epi-indent domains-input">
            <%= Html.TextBoxFor(m => m.TrackerSettings.Sites[i].Domains, new { style="width:99%" })%>
             <%= Html.ValidationMessageFor(m => m.TrackerSettings.Sites[i].Domains, Html.GaTranslate("tabs/domain/multidomains/errormessage"), new { @class="error" })%>
        </div>
	</div>
</div>