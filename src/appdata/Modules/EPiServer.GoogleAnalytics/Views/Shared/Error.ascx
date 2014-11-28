<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ErrorModel>" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<div class="error-area">
    <p class="errorinfo"><%= Model.ErrorSource %></p>
    <p class="errorinfo"><%= Model.Message %></p>
</div>