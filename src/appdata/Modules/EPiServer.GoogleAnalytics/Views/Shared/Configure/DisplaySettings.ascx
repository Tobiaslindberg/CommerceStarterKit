<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="System.Linq" %>

<div class="epi-googleanalytics-configuration epi-googleanalytics-settings">
<div class="epi-paddingHorizontal-small epi-formArea">
	<% using (Html.BeginGadgetForm("SaveDisplaySettings")) { %>
	<% using(Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/configure/displaysettings/")) { %>
    <p class="epi-marginVertical-small"><%= Html.GaTranslate("header") %></p>
	<fieldset>
		<legend><%= Html.GaTranslate("heading/legend")%></legend>

		<div>
            <div>
			    <%= Html.CheckBoxFor(m => m.Layout.DisplayHeading, new { id = Html.GaId("displayheading") })%>
			    <label for="<%= Html.GaId("displayheading") %>"><%= Html.GaTranslate("heading/displaylabel")%></label>
            </div>

			<% if (Model.AllowCustomTitle) { %>
			<div class="epi-size10 dependant">
				<label for="<%= Html.GaId("heading") %>"><%= Html.GaTranslate("heading/label")%></label>
				<%= Html.TextBoxFor(m => m.Layout.Heading, new { id = Html.GaId("heading"), Placeholder = Html.GaTranslate("heading/placeholder"), @class = "gadget-heading" })%>
			</div>
			<% } %>
		</div>
	</fieldset>

	<fieldset>
		<legend><%= Html.GaTranslate("graphs/legend")%></legend>

		<div class="SelectedLayout epi-marginVertical-small">
			<h4><%= Html.GaTranslate("graphs/layout/heading")%></h4>
			<%= Html.GaRadio("Layout.SelectedLayout", "tabbed", Model.Layout.SelectedLayout, "graphs/layout/tabbed")%>
			<%= Html.GaRadio("Layout.SelectedLayout", "stacked", Model.Layout.SelectedLayout, "graphs/layout/stacked")%>
		</div>
		
		<div class="ChartType epi-marginVertical-small">
			<h4><%= Html.GaTranslate("graphs/charttype/heading")%></h4>
			<%= Html.GaRadio("Layout.SelectedChart", "line", Model.Layout.SelectedChart, "graphs/charttype/line")%>
			<%= Html.GaRadio("Layout.SelectedChart", "bar", Model.Layout.SelectedChart, "graphs/charttype/bar")%>
		</div>
	</fieldset>
	<fieldset>
		<legend><%= Html.GaTranslate("lists/legend")%></legend>
		
		<div>
			<label for="<%= Html.GaId("rowCount") %>"><%= Html.GaTranslate("lists/rowcount")%></label>
			<%= Html.DropDownList("Statistics.ListCount", new[] { new SelectListItem { Text = "5" }, new SelectListItem { Text = "10" }, new SelectListItem { Text = "25" }, new SelectListItem { Text = "50" }, new SelectListItem { Text = "100" } }, new { id = Html.GaId("rowCount"), @class = "RowCount" })%>
		</div>	
	</fieldset>

	<div class="epi-buttonContainer-simple">
		<%= Html.AcceptButton()%>
		<%= Html.CancelButton()%>
	</div>
	<% } %>
	<% } %>
</div>
</div>
<% Html.RenderPartial("Configure/_ShowGadgetContentScript", Model); %>