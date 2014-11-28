<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>

<% foreach(var list in Model.Tables) { %>
<div class="list">
	<h3><%= Html.GaTranslate(list.Dimension.TranslationKey, list.Dimension.DisplayName)%></h3>
	<ol>
		<% int row = 0; %>
		<% foreach (var item in list.Rows) { %>
			<li class="odd<%= row % 2 %> row<%= row %>">
				<span><%= list.Formatter.Format(item.Value) %></span>
				
                <% if(item.HasLink) { %>
					<a target="<%= Model.Frame %>" href="<%= item.Link %>" title="<%= item.Name %>" data-pageLink="<%= item.ContentLink %>"><%= item.Name %></a>
				<% } else { %>
					<label title="<%= item.Name %>"><%= item.Name %></label>
				<% } %>
			</li>
		<% row++; %>
		<% } %>
		<% if (row == 0) { %>
		<li class="nodata"><%= Html.GaTranslate("/episerver/googleanalytics/shared/message/nodata") %></li>
		<% } %>
	</ol>
</div>
<% } %>
