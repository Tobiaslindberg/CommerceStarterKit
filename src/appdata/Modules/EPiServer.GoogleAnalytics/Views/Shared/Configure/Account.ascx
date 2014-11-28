<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="System.Linq" %>

<div class="epi-googleanalytics-configuration epi-googleanalytics-settings">
<div class="epi-paddingHorizontal-small epi-formArea">
	<% using (Html.BeginGadgetForm("SaveAccount")) { %>
	<% using (Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/configure/account/")) { %>
    <p class="epi-marginVertical-small"><%= Html.GaTranslate("header") %></p>
	<fieldset>
		<legend><%= Html.GaTranslate("legend")%></legend>

		<div class="epi-size5 authentication" data-isShared="<%= Model.SelectedAccount.IsShared %>" data-personal-authenticated="<%= Model.PersonalAccount.IsAuthenticated() %>">
			<label for="<%= Html.GaId("user") %>"><%= Html.GaTranslate("account") %></label>
			<select id="<%= Html.GaId("user") %>" name="Account.SelectedAccount" class="account">
				<option></option>
				<% if (Model.SelectedAccount.IsShared) { %>
				<option value="Personal">Personal</option>
				<option selected="selected" value="Shared">Shared</option>
				<% } else { %>
				<option value="Personal" selected="selected">Personal</option>
					<% if(Model.SharedAccount.IsAuthenticated()) { %>
					<option value="Shared">Shared</option>
					<% } %>
				<% } %>
			</select>
        
            <!-- Signout button in Share/Configure/Account.ascx -->
			<a href="#" class="signin"><%= Html.GaTranslate("signin")%></a>
			<a href="#" class="signout"><%= Html.GaTranslate("signout")%></a>
		</div>

		<div class="epi-size5">
			<label for="<%= Html.GaId("profile")%>"><%= Html.GaTranslate("profile")%></label>
			<% if (Model.IsAuthenticated)
	  { %>
			<select id="<%= Html.GaId("profile")%>" name="Account.SelectedTableId" class="webProperty" data-signintoselect-text="<%= Html.GaTranslate("singintoselect")%>">
			<% foreach (var profile in Model.Profiles)
	  {%>
				<optgroup label="<%= profile.Name%>">
				<% foreach (var property in profile.WebProperties)
	   {%>
					<option value="<%= property.TableId%>" title="<%= property.TrackingId%>" <%= Model.Account.SelectedTableId == property.TableId ? "selected='selected'" : ""%>><%= property.DefaultUrl + " - " + property.Name%></option>
				<% } %>
				</optgroup>
			<% } %>
			</select>
			<% }
	  else if (Model.IsForbidden)
	  { %>
			<select id="<%= Html.GaId("profile")%>" name="Account.SelectedTableId" class="webProperty" disabled="disabled" data-signintoselect-text="<%= Html.GaTranslate("singintoselect")%>">
				<option value=""><%= Html.GaTranslate("nowebproperties")%></option>
			</select>
			<% }
	  else
	  { %>
			<select id="<%= Html.GaId("profile")%>" name="Account.SelectedTableId" class="webProperty" disabled="disabled" data-signintoselect-text="<%= Html.GaTranslate("singintoselect")%>">
				<option value=""><%= Html.GaTranslate("singintoselect")%></option>
			</select>
			<% } %>
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