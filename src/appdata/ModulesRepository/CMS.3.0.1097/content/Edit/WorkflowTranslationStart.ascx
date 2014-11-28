<%@ Control language="c#" Codebehind="WorkflowTranslationStart.ascx.cs" AutoEventWireup="true" Inherits="EPiServer.WorkflowFoundation.Workflows.WorkflowTranslationStart" %>
<script type='text/javascript'>
		//<![CDATA[
		function OpenUserBrowser(type, ctrl)
		{
		    var method;
		    if ( ctrl.id == '<%=OwnerGroupAddButton.ClientID%>')
		    {
		        OpenDialogUserAndGroupBrowser( type, '', '<%=EPiServer.UriSupport.ResolveUrlFromUIBySettings("edit")%>', false, SetOwner);
		    }
		    else if( ctrl.id == '<%=FallbackGroupAddButton.ClientID%>')
		    {
		        OpenDialogUserAndGroupBrowser( type, '', '<%=EPiServer.UriSupport.ResolveUrlFromUIBySettings("edit")%>', false, SetFallback);
		    }
		    else if( ctrl.id == '<%=TranslatorGroupAddButton.ClientID%>')
		    {
		        OpenDialogUserAndGroupBrowser( type, '', '<%=EPiServer.UriSupport.ResolveUrlFromUIBySettings("edit")%>', false, SetTranslator);
		    }
		}
		
		function SetOwner( returnValueArray )
		{
		    document.getElementById('<%=PostedTranslator.ClientID%>').value = GetUser(returnValueArray);
		    __doPostBack('UpdateOwner', '');
		}
		function SetFallback( returnValueArray )
		{
		    document.getElementById('<%=PostedTranslator.ClientID%>').value = GetUser(returnValueArray);
		    __doPostBack('UpdateFallback', '');
		}
		function SetTranslator( returnValueArray )
		{
		    var username =  GetUser(returnValueArray);
		    var language = '';
            var select = document.getElementById('<%=Languages.ClientID%>');
            for (var i=0; i < select.length; i++) 
            {
                if (select.options[i].selected) 
                {
                    language = select.options[i].value;
                }
            }
            document.getElementById('<%=PostedTranslator.ClientID%>').value = username;
	        document.getElementById('<%=PostedLanguage.ClientID%>').value = language;
             __doPostBack('UpdateTranslatorList', '');
        }
		
		function GetUser( returnValueArray )
		{
		    if (returnValueArray && returnValueArray.length > 0)
			{
			    return returnValueArray[0].split('|||')[0];   
			}
			else
			{
			    return '';
			}
		}
		//]]>
    </script>  
<h2><EPiServer:Translate runat="server" text="/workflows/translate/heading" />
    <EPiServer:Translate runat="server" text="/workflows/translate/forpage" />&nbsp;<%=Server.HtmlEncode(PageName)%></h2>
<asp:HiddenField runat="server" ID="PostedTranslator" />
<asp:HiddenField runat="server" ID="PostedLanguage" />
<table class="epistandardtable">
    <tr>
        <td>
            <EPiServer:Translate runat="server" text="/workflows/translate/initialdelay" />
            &nbsp;(<EPiServer:Translate runat="server" text="/workflows/translate/minutes" />)
        </td>
        <td>
            <asp:TextBox runat="server" ID="InitialDelay" Text="10" Width="30" />
            <asp:RequiredFieldValidator ControlToValidate="InitialDelay" Text="*" EnableClientScript="true" runat="server" ErrorMessage="<%$ Resources: EPiServer, validation.formrequired %>" />
            <asp:RangeValidator id="delayRequired" runat="server" ControlToValidate="InitialDelay"
                MinimumValue="0"
                MaximumValue="2147483647"
                Type="Integer"
                Text="*"
                ErrorMessage="<%$ Resources: EPiServer, workflow.translate.rangenumber %>" Display="dynamic"/>
        </td>
   </tr>
   <tr>
        <td>
            <EPiServer:Translate runat="server" text="/workflows/translate/reminder" />
            &nbsp;(<EPiServer:Translate runat="server" text="/workflows/translate/hours" />)
        </td>
        <td>
            <asp:TextBox runat="server" ID="Reminder" Text="48" Width="30"/>
            <asp:RequiredFieldValidator ControlToValidate="Reminder" Text="*" EnableClientScript="true" runat="server" ErrorMessage="<%$ Resources: EPiServer, validation.formrequired %>" />
            <asp:RangeValidator id="reminderRequired" runat="server" ControlToValidate="Reminder"
                 MinimumValue="0"
                MaximumValue="2147483647"
                Type="Integer"
                Text="*"
                ErrorMessage="<%$ Resources: EPiServer, workflow.translate.rangenumber %>" Display="dynamic"/>
        </td>
    </tr>
    <tr>
        <td>
            <EPiServer:Translate runat="server" text="/workflows/translate/terminate" />
            &nbsp;(<EPiServer:Translate runat="server" text="/workflows/translate/hours" />)
        </td>
        <td>
            <asp:TextBox runat="server" ID="Terminate" Text="120" Width="30"/>
            <asp:RequiredFieldValidator ControlToValidate="Terminate" Text="*" EnableClientScript="true" runat="server" ErrorMessage="<%$ Resources: EPiServer, validation.formrequired %>" />
            <asp:RangeValidator id="terminateRequired" runat="server" ControlToValidate="Terminate"
                MinimumValue="0"
                MaximumValue="2147483647"
                Type="Integer"
                Text="*"
                ErrorMessage="<%$ Resources: EPiServer, workflow.translate.rangenumber %>" Display="dynamic"/>
        </td>
    </tr>
    <tr>
        <td>
            <EPiServer:Translate runat="server" text="/workflows/translate/publishneeded" />
        </td>
        <td>
           <asp:CheckBox ID="NeedsToBePublished" runat="server" Checked="false" />
        </td>
    </tr>
    <tr style="border-bottom: none;">
        <td colspan="2" style="border-bottom: none;">
            <EPiServer:Translate runat="server" text="/workflows/translate/owner" />
        </td>
   </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox runat="server" ID="Owner" Enabled="false" Width="150"/>
            <EPiServerUI:ToolButton id="OwnerGroupAddButton" runat="server" OnClientClick="OpenUserBrowser(1, this);" GeneratesPostBack="False" Text="<%$ Resources: EPiServer, button.addusergroupsid %>" ToolTip="<%$ Resources: EPiServer, button.addusergroupsid %>" SkinID="AddUserGroup" />
        </td>
    </tr>
    <tr style="border-bottom: none;">
        <td colspan="2" style="border-bottom: none;">
            <EPiServer:Translate runat="server" text="/workflows/translate/fallback" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox runat="server" ID="Fallback" Enabled="false" Width="150"/>
            <asp:RequiredFieldValidator ControlToValidate="Fallback" Text="*" EnableClientScript="true" runat="server" ErrorMessage="<%$ Resources: EPiServer, validation.formrequired %>" />
            <EPiServerUI:ToolButton id="FallbackGroupAddButton" runat="server" OnClientClick="OpenUserBrowser(1, this);" GeneratesPostBack="False" Text="<%$ Resources: EPiServer, button.addusergroupsid %>" ToolTip="<%$ Resources: EPiServer, button.addusergroupsid %>" SkinID="AddUserGroup" />
        </td>
    </tr>
    <tr style="border-bottom: none;">
        <td colspan="2" style="border-bottom: none;">
           <EPiServer:Translate runat="server" text="/workflows/translate/addtranslator" />
        </td>
    </tr>
    <tr style="border-bottom: none;">
        <td colspan="2" style="border-bottom: none;">
            <asp:DropDownList runat="server" ID="Languages" Width="155" />
            <EPiServerUI:ToolButton id="TranslatorGroupAddButton" runat="server" OnClientClick="OpenUserBrowser(1, this);" GeneratesPostBack="False" Text="<%$ Resources: EPiServer, button.addusergroupsid %>" ToolTip="<%$ Resources: EPiServer, button.addusergroupsid %>" SkinID="AddUserGroup" />
        </td>
    </tr>
    <tr style="border-bottom: none;">
        <td colspan="2" align="center" style="border-bottom: none;">
        <asp:Panel ID="GridPanel" runat="server">
            <asp:DataGrid ID="Grid" runat="server"   
                AutoGenerateColumns="false"
                OnItemCommand="RemoveTranslator">
                <Columns>
                   <asp:BoundColumn HeaderStyle-CssClass="epitableheading" DataField="Translator" HeaderText="<%$ Resources: EPiServer, workflows.approval.startparameters.name %>" />
                    <asp:BoundColumn HeaderStyle-CssClass="epitableheading" DataField="LanguageName" HeaderText="<%$ Resources: EPiServer, workflows.translate.language %>" />
                </Columns>
             </asp:DataGrid>
        </asp:Panel>
        </td>   
    </tr>
</table>
