<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<% var visited = ViewData["Visited"] as HashSet<object> ?? (ViewData["visited"] = new HashSet<object>()) as HashSet<object>; %>
<% if (Model == null || Model.GetType() == typeof(object)) { %>
	(null)
<% } else if(Model is string) { %>
	<%= Model %>
<% } else if (Model.GetType().IsValueType) { %>
	<%= Model %>
<% } else if(visited.Contains(Model)) { %>
	(duplicate)
<% } else if (Model is IDictionary) { %>
	<ul>
	<% foreach(var key in (Model as IDictionary).Keys) { %>
	<li>
		<strong><%= key %>:</strong>
		<% Html.RenderPartial("Drill", (Model as IDictionary)[key] ?? new object()); %>
	</li>
	<% } %>
	</ul>
<% } else if (Model is System.Web.Routing.RouteValueDictionary) { %>
	<ul>
	<% foreach(var kvp in (Model as System.Web.Routing.RouteValueDictionary)) { %>
	<li>
		<strong><%= kvp.Key %>:</strong>
		<% Html.RenderPartial("Drill", kvp.Value ?? new object()); %>
	</li>
	<% } %>
	</ul>
<% } else if (Model is IEnumerable) { %>
	<ul>
	<% int i = 0; %>
	<% foreach(var item in (Model as IEnumerable)) { %>
	<li>
		<strong><%= i++ %>:</strong>
		<% Html.RenderPartial("Drill", item); %>
	</li>
	<% } %>
	</ul>
<% } else {%>
	<% visited.Add(Model); %>
	<% Html.RenderPartial("Drill", new System.Web.Routing.RouteValueDictionary(Model ?? new object())); %>
<% } %>
