<%@ Page Language="c#" Codebehind="WorkflowDefinitionEdit.aspx.cs" AutoEventWireup="False" Inherits="EPiServer.WorkflowFoundation.UI.WorkflowDefinitionEdit"  Title="Workflow admin" %>
<%@ Register TagPrefix="Workflow" Namespace="EPiServer.WorkflowFoundation.WebControls" Assembly="EPiServer.WorkflowFoundation"  %>
<%@ Register TagPrefix="Workflow" TagName="Instances" Src="../edit/WorkflowInstanceList.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContentRegion" runat="server">
	<script type='text/javascript'>
	//<![CDATA[
	
	function OpenDefinitionImage(id)
	{
	    window.showModalDialog('<%=EPiServer.UriSupport.ResolveUrlFromUIBySettings("edit/WorkflowImage.aspx")%>?DefinitionId=' + id, null, 'dialogWidth:800px;dialogHeight:1000px;help=no;resizable:yes;scroll:yes;status:yes');
	    return false;
	}

	//]]>
	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainRegion" runat="server">
    <EPiServerScript:ScriptSettings runat="server" ConfirmMessage="<%$ Resources: EPiServer, admin.workflowdefinitionedit.confirmdelete %>" ID="deleteCommon" />
 
    <EPiServerUI:TabStrip runat="server" id="actionTab" GeneratesPostBack="False" targetid="tabView">
	    <EPiServerUI:Tab Text="/admin/workflowdefinitionedit/basictab" runat="server" ID="Tab1" Sticky="true"/>
	    <EPiServerUI:Tab Text="/admin/workflowdefinitionedit/securitytab" runat="server" ID="Tab2" Sticky="true"/>
		<EPiServerUI:Tab Text="/admin/workflowdefinitionedit/automaticstart" runat="server" ID="Tab3" Sticky="true"/>
		<EPiServerUI:Tab Text="/admin/workflowstartparams/heading" runat="server" ID="StartParamTab" Sticky="true"/>
		<EPiServerUI:Tab Text="/admin/workflowdefinitionedit/runningtab" runat="server" ID="RunningTab" Sticky="true"/>
	</EPiServerUI:TabStrip>
	
    <asp:Panel runat="server" id="tabView" CssClass="epi-padding">
    
        <asp:Panel CssClass="epi-formArea" runat="server" ID="Definitions">
            <div class="epi-size10">
                <div>
                    <asp:Label AssociatedControlID="DefinitionName" runat="server" Translate="/admin/workflowdefinitionedit/defname" />
                    <asp:TextBox runat="server" ID="DefinitionName" />
                    <asp:RequiredFieldValidator ID="DefinitionNameValidator" runat="server" ControlToValidate="DefinitionName" ErrorMessage="<%$ Resources: EPiServer, admin.workflowdefinitionedit.nameerror %>" EnableClientScript="false" Text="*"/>
                </div>
                <div>
                    <asp:Label AssociatedControlID="Description" runat="server" Translate="/admin/workflowdefinitionedit/description" />
                    <asp:TextBox runat="server" ID="Description" TextMode="MultiLine" Rows="5"/>
                </div>
                <div>
                     <asp:Label AssociatedControlID="ClassName" runat="server" Translate="/admin/workflowdefinitionedit/classname" />
                    <asp:TextBox runat="server" ID="ClassName" />
                    <asp:RequiredFieldValidator ID="ClassNameValidator" runat="server" ControlToValidate="ClassName" ErrorMessage="<%$ Resources: EPiServer, admin.workflowdefinitionedit.classerror %>" EnableClientScript="false" Text="*"/>
                    <asp:CustomValidator runat="server" EnableClientScript="false" ID="CustomValidator" OnServerValidate="CustomValidation" Text="*"/>
                </div>
                <div>
                    <asp:Label AssociatedControlID="AssemblyName" runat="server" Translate="/admin/workflowdefinitionedit/assemblyname" />
                    <asp:TextBox runat="server" ID="AssemblyName" />
                    <asp:RequiredFieldValidator ID="AssemblyNameValidator" runat="server" ControlToValidate="AssemblyName" ErrorMessage="<%$ Resources: EPiServer, admin.workflowdefinitionedit.assemblyerror %>" EnableClientScript="false" Text="*"/>
                </div>
                <div>
                    <asp:Label AssociatedControlID="fileLayout" runat="server" Translate="/admin/workflowdefinitionedit/layout" />
                    <asp:FileUpload runat="server" ID="fileLayout" />&nbsp;(<asp:Label runat="server" ID="LayoutText"/>)
                    <asp:CustomValidator runat="server" EnableClientScript="false" ID="LayoutFileValidator" ControlToValidate="fileLayout" OnServerValidate="LayoutFileValidation" Text="*" ErrorMessage="<%$ Resources: EPiServer, admin.workflowdefinitionedit.layoutfileerror %>" />
                </div>
            </div>      
        </asp:Panel>
        
        <asp:Panel runat="server">
            <div class="epi-buttonDefault">
                <EPiServerUI:ToolButton id="UserGroupAddButton" GeneratesPostBack="False" OnClientClick="OpenUserGroupBrowser(0);" runat="server"  Text="<%$ Resources: EPiServer, button.addusergroupsid %>" ToolTip="<%$ Resources: EPiServer, button.addusergroupsid %>" SkinID="AddUserGroup" />
            </div>
            <hr/>
            <div class="epi-buttonDefault">
                <EPiServerUI:ToolButton id="SaveButton" CausesValidation="false"  CssClass="EPEdit-CommandToolDisabled" Enabled="False"  OnClick="AclSaveButton_Click" runat="server"  Text="<%$ Resources: EPiServer, button.save %>" ToolTip="<%$ Resources: EPiServer, button.save %>" SkinID="Save" />
            </div>
            <EPiServerUI:MembershipAccessLevel SaveButtonID="SaveButton" CssClass="EPEdit-sidAccessLevel" id="sidList" runat="server" />
            <EPiServerScript:ScriptEvent runat="server" EventHandler="StoreInitialAccessRights" EventType="Load" EventTargetClientNode="window" />
        </asp:Panel>
        
        <asp:Panel ID="Panel1" runat="server" CssClass="epi-paddingVertical">
        <div class="epi-grid-1-1">
            <div class="epi-gridColumn">
                <p><EPiServer:Translate runat="server" text="/admin/workflowdefinitionedit/startevent" /></p>
                <asp:ListBox style="margin-bottom:1em;" Rows="18" runat="server" 
                    SelectionMode="Multiple" ID="EventList" AutoPostBack="true" 
                    OnSelectedIndexChanged="EventList_SelectedIndexChanged"/>    
                <p><EPiServer:Translate runat="server" text="/admin/workflowdefinitionedit/completedDefinition" /></p>
                <asp:ListBox runat="server" Rows="5" ID="WorkflowDefinitions" SelectionMode="Multiple" AutoPostBack="false" />
            </div>
            <div class="epi-gridColumn">
                <asp:Panel runat="server" id="pnlPageEvent" Visible="false">
                    <p><EPiServer:Translate runat="server" text="/admin/workflowdefinitionedit/pageroot" /></p>
                    <EPiServer:inputpagereference DisableCurrentPageOption="true" style="margin-bottom:1em;" runat="server" id="pageRoot" />
                    <p><EPiServer:Translate runat="server" text="/admin/workflowdefinitionedit/pagetypes" /></p>
                    <asp:ListBox Rows="22" runat="server" ID="PageTypesList" SelectionMode="Multiple" />
                </asp:Panel>         
            </div>
        </div>
        </asp:Panel>
        
        <asp:Panel runat=server ID="StartParams"> 
             <asp:Label runat="server" ID="Message"/>    
             <asp:PlaceHolder runat="server" ID="plcCtrl" />
        </asp:Panel>
        
        <asp:Panel runat=server ID="Running">     
            <Workflow:Instances runat="server" id="InstanceListControl" />
        </asp:Panel>
    
    <div class="epi-buttonContainer">
        <EPiServerUI:ToolButton ID="DeleteDefinition" SkinID="Delete" OnClick="Delete_Click" EnableClientConfirm="true" Text="<%$ Resources: EPiServer, admin.workflowdefinitionedit.delete %>" ToolTip="<%$ Resources: EPiServer, admin.workflowdefinitionedit.delete %>"  runat="server" /><EPiServerUI:ToolButton ID="SaveDefinition" SkinID="Save" OnClick="Save_Click" Text="<%$ Resources: EPiServer, admin.workflowdefinitionedit.save %>" ToolTip="<%$ Resources: EPiServer, admin.workflowdefinitionedit.save %>"  runat="server" /><EPiServerUI:ToolButton ID="ViewDefinition" CausesValidation="false" SkinID="ViewMode" Text="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.view %>" ToolTip="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.view %>"  runat="server" /><EPiServerUI:ToolButton ID="CancelDefinition" CausesValidation="false" SkinID="Cancel" OnClick="Cancel_Click" Text="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>" ToolTip="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>"  runat="server" />
        <EPiServerScript:ScriptSettings ID="ScriptSettings1" runat="server" TargetControlID="DeleteDefinition" CommonSettingsControlID="deleteCommon" Name='<%# Eval("Name") %>' />
    </div>
        
    </asp:Panel>
    
       
    
</asp:Content>