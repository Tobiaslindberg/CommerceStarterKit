<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" MasterPageFile="../Shared/Site.master" %>

<%@ Import Namespace="EPiServer.Labs.LanguageManager.Helpers" %>
<%@ Assembly Name="EPiServer.Labs.LanguageManager" %>
<%@ Register TagPrefix="sc" Assembly="EPiServer.Shell" Namespace="EPiServer.Shell.Web.UI.WebControls" %>

<asp:Content runat="server" ContentPlaceHolderID="GlobalNavigationContainer">
    <div class="epi-globalNavigationContainer">
        <sc:ShellMenu runat="server" SelectionPath="/global/addons" />
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="epi-contentContainer epi-padding">
        <h1 class="EP-prefix">
            <%= "/languagemanager/settingview/title".Translate() %>
        </h1>
        <p class="EP-systemMessage">
            <%= "/languagemanager/settingview/subtitle".Translate() %>
        </p>

        <form method="post">

        <div class="epi-padding epi-formArea">
            <div class="epi-size15">
                <div>
                    <label><%= "/languagemanager/settingview/name".Translate() %></label><input name="providerName" type="text" readonly="readonly" value="<%: Model.Name %>" />
                </div>
                <div>
                    <label><%= "/languagemanager/settingview/desc".Translate() %></label><input name="desc" type="text" value="<%: Model.Description %>" />
                </div>
                <div>
                    <label><%= "/languagemanager/settingview/consumerkey".Translate() %></label><input name="consumerKey" type="text" value="<%: Model.ConsumerKey %>" />
                </div>
                <div>
                    <label><%= "/languagemanager/settingview/consumersecret".Translate() %></label><input name="consumerSecret" type="text" value="<%: Model.ConsumerSecret %>" />
                </div>
            </div>

            <div class="epi-buttonContainer">
                <%: ViewData["SaveResult"] %>
                <span class="epi-cmsButton">
                    <input type="submit" class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Save" value="<%= "/languagemanager/settingview/savebutton".Translate() %>" />
                </span>
            </div>

        </div>
        </form>
    </div>
</asp:Content>
