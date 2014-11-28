<%@ Control Language="C#" Inherits="ViewUserControl<string>" %>
<%@ Register TagPrefix="EPiServerUI" Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI" %>
<% 
    var shellModule = EPiServer.GoogleAnalytics.Helpers.ModuleHelper.GetShellModule();
    string ModuleHelpUrl = null != shellModule ? shellModule.GetHelpUrl() : string.Empty;
%>

<div class="epi-contentArea">
	<h1 class="EP-prefix">
		<%= EPiServer.Core.LanguageManager.Instance.Translate(Model + "name")%>
        <a onclick="window.open('<%= ModuleHelpUrl %>','_blank', 'scrollbars=yes, height=500, location=no, menubar=no, resizable=yes, toolbar=no, width=840');return false;" 
            title="Help" href="<%= ModuleHelpUrl %>" target="_blank">
            <img class="EPEdit-CommandTool" align="absmiddle" src="/App_Themes/Default/Images/Tools/Help.png" border="0" alt="Help">
	    </a>
	</h1>
	<span class="EP-systemInfo"><%= EPiServer.Core.LanguageManager.Instance.Translate(Model + "info") %></span>
</div>
