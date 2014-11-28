<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AdministrationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% int i = (int)ViewContext.TempData["SiteIndex"]; %>
<% var selectedValue = Model.TrackerSettings.Sites[i].TrackingScriptOption.ToString(); %>
    
<div class="script-settings" script-option="<%= Model.TrackerSettings.Sites[i].TrackingScriptOption %>" style="<%= Model.TrackerSettings.Sites[i].RemovedFromConfig ? "background-color:pink" : "" %>">
	<%= Html.HiddenFor(m => m.TrackerSettings.Sites[i].SiteId)%>

	<div class="epi-padding Settings">
		<h2><%= Html.GaTranslate("scriptoption/autoscript/heading") %></h2>
		<p><%= Html.GaTranslate("scriptoption/autoscript/info") %></p>
		<label><%= Html.GaTranslate("scriptoption/autoscript/heading") %></label>
		<%= Html.TextBoxFor(m => m.TrackerSettings.Sites[i].TrackingId, new { placeholder = "UA-######-#" })%>
	</div>

	<div class="epi-padding script-options">
	    <h2><%= Html.GaTranslate("scriptoption/heading") %></h2>
		<div>
			<%= Html.GaRadioFor(m => m.TrackerSettings.Sites[i].TrackingScriptOption, "Classic", selectedValue, "scriptoption/classic")%>
		</div>
		<div>
			<%= Html.GaRadioFor(m => m.TrackerSettings.Sites[i].TrackingScriptOption, "Universal", selectedValue, "scriptoption/universal")%>
		</div>
		<div>
			<%= Html.GaRadioFor(m => m.TrackerSettings.Sites[i].TrackingScriptOption, "Custom",  selectedValue, "scriptoption/custom")%>
		</div>
	</div>

    <div class="auto-script">
	    <div class="epi-gridColumn epi-tabView" id="settings">
		    <ul class="epi-tabView-navigation" role="tablist">
                <li class="epi-tabView-navigation-item epi-tabView-navigation-item-selected">
				    <a href="#" id="domain" class="epi-tabView-tab" role="tab" tabindex="0"><%= Html.GaTranslate("tabs/domain/tab") %></a>
			    </li>
                <li class="epi-tabView-navigation-item">
				    <a href="#" id="roles" class="epi-tabView-tab" role="tab" tabindex="0"><%= Html.GaTranslate("tabs/roles/tab") %></a>
			    </li>
                <li class="epi-tabView-navigation-item">
				    <a href="#" id="links" class="epi-tabView-tab" role="tab" tabindex="0"><%= Html.GaTranslate("tabs/links/tab") %></a>
			    </li>
                <li class="epi-tabView-navigation-item">
				    <a href="#" id="search" class="epi-tabView-tab" role="tab" tabindex="0"><%= Html.GaTranslate("tabs/users/tab") %></a>
			    </li>
		    </ul>
		    <div class="epi-settingsTabPanelContainer epi-scrollable">
			    <% Html.RenderPartial("Tracking/Domains"); %>
			    <% Html.RenderPartial("Tracking/Roles"); %>
			    <% Html.RenderPartial("Tracking/Links"); %>
			    <% Html.RenderPartial("Tracking/Users"); %>
		    </div>
	    </div>
    </div>
    <div class="epi-padding custom-script">
		<p><%= Html.GaTranslate("scriptoption/customscript/info") %></p>
		<%= Html.TextAreaFor(m => m.TrackerSettings.Sites[i].CustomTrackingScript, new { style="width:99%" })%>
    </div>
</div>
