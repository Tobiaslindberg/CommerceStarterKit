<%@  Page Language="C#" Inherits="System.Web.Mvc.ViewPage<AuthenticationViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="EPiServer.Shell.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer.Shell.Web" %>
<%@ Import Namespace="EPiServer.Shell.Navigation" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Index</title>
	<% Html.RenderPartial("SharedHead"); %>
	<%= Page.ClientResources("AnalyticsAdministration") %>
</head>
<body>
	<div class="epi-contentContainer epi-padding">
		<%= Html.Partial("Info", "/episerver/googleanalytics/authentication/").ToHtmlString().Replace("~/", ResolveUrl("~/")) %>
		
		<div class="epi-formArea epi-marginVertical">
			<% using(Html.BeginForm("SignIn", null, new { isShared = Model.IsShared }, FormMethod.Post, new { })) { %>
			<% Html.GaTranslatePrefix("/episerver/googleanalytics/authentication/"); %>
				<div class="epi-size10">
					<fieldset>
						<legend>Sign in</legend>
						<div>
							<label for="username"><%= Html.GaTranslate("username") %></label>
							<%= Html.TextBoxFor(m => m.Username, new { autocomplete = "off" }) %>
							<%= Html.ValidationMessageFor(m => m.Username)%>
						</div>
						<div>
							<label for="username"><%= Html.GaTranslate("password") %></label>
							<%= Html.TextBoxFor(m => m.Password, new { type = "password", autocomplete = "off" })%>
							<%= Html.ValidationMessageFor(m => m.Password) %>
						</div>
					</fieldset>
				</div>
				<div class="epi-buttonContainer-simple">
					<span class="epi-button epi-gadgetAccept"><span class="epi-button-child"><input id="subimt" class="epi-button-child-item" name="submit" type="submit" value="<%= Html.GaTranslate("signin") %>"></span></span>
					<span class="epi-button epi-gadgetCancel"><span class="epi-button-child"><input id="reset" class="epi-button-child-item" name="reset" type="reset" value="<%= Html.GaTranslate("cancel") %>"></span></span>
				</div>
			<% } %>

		</div>

		<script>
			$(function () {
				$("#reset").click(function () {
					window.close();
				});
			});
		</script>
	</div>
</body>
</html>
