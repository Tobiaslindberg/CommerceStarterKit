<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% using(Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/configure/graphs/")) { %>

<fieldset>
	<legend><%= Html.GaTranslate("legend") %></legend>

	<div class="SelectedGraphs epi-paddingVertical-small">
		<input type="hidden" name="Statistics.SelectedGraphs" value="" />
		<% foreach (var metric in Model.GraphMetrics) { %>
			<%= Html.GaCheckbox(metric.Name, "Statistics.SelectedGraphs", metric.Name, Model.Statistics.SelectedGraphs, metric.TranslationKey, metric.DisplayName)%>
		<% } %>


	    <% if (Model.GoalsSelectable) { %>
		<div class="SelectedGoals">
            <%= Html.GaCheckboxFor(m => m.Statistics.DisplayGoals, "goals/label") %>
		    <div class="dependant grouped">
				<% var goals = Model.Goals.ToList(); %>
				<% if(goals.Any()) { %>
		        <input type="hidden" name="Statistics.SelectedGoals" value="" />
		        <% foreach (var goal in goals) { %>
                    <%= Html.GaCheckbox(goal.Number.ToString(), "Statistics.SelectedGoals", goal.Number.ToString(), Model.Statistics.SelectedGoals, null, goal.Name)%>
		        <% } %>
		        <% } else { %>
					<%= Html.GaTranslate("goals/needssetup") %>	
		        <% } %>
            </div>
        </div>
		<% } %>
		
		<% if (Model.EventsSelectable) { %>
		<div class="SelectedEvents">
            <%= Html.GaCheckboxFor(m => m.Statistics.DisplayEvents, "events/label")%>
		    <div class="dependant grouped">
				<% if(!string.IsNullOrEmpty(Model.Statistics.SelectedSegment)) { %>
				<p><%= Html.GaTranslate("cannotcombine")%></p>
		        <% } %>
		        <input type="hidden" name="Statistics.SelectedEvents" value="" />
                <% foreach (var eventMetric in Model.EventMetrics) { %>
                    <%= Html.GaCheckbox(eventMetric.EventCategory, "Statistics.SelectedEvents", eventMetric.Name, Model.Statistics.SelectedEvents, eventMetric.TranslationKey, eventMetric.DisplayName)%>
		        <% } %>
            </div>
        </div>
		<% } %>
		
		<% if (Model.VisitorGroupsSelectable) { %>
		<div class="SelectedEvents">
            <%= Html.GaCheckboxFor(m => m.Statistics.DisplayVisitorGroups, "visitorgroups/label")%>
		    <div class="dependant grouped">
				<% if(!string.IsNullOrEmpty(Model.Statistics.SelectedSegment)) { %>
				<p><%= Html.GaTranslate("cannotcombine") %></p>
		        <% } %>
		        <input type="hidden" name="Statistics.SelectedVisitorGroups" value="" />
                <% foreach (var vgMetric in Model.VisitorGroupMetrics) { %>
                   <%= Html.GaCheckbox(vgMetric.VisitorGroupId, "Statistics.SelectedVisitorGroups", vgMetric.VisitorGroupId, Model.Statistics.SelectedVisitorGroups, null, vgMetric.VisitorGroupName)%>
		        <% } %>
            </div>
        </div>
		<% } %>
	</div>
</fieldset>

<% } %>