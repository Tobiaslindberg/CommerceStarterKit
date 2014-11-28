<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowInstanceList.ascx.cs" Inherits="EPiServer.WorkflowFoundation.UI.WorkflowInstanceList" %>
<%@ Register TagPrefix="Workflow" Namespace="EPiServer.WorkflowFoundation.WebControls" Assembly="EPiServer.WorkflowFoundation"  %>
<%@ Register TagPrefix="Workflow" TagName="History" Src="WorkflowHistoryList.ascx" %>
<%@ Register TagPrefix="Workflow" TagName="Task" Src="WorkflowTaskList.ascx" %>

<script type='text/javascript'>
	//<![CDATA[
	
	function OpenImage(id)
	{
	    window.showModalDialog('<%=EPiServer.UriSupport.ResolveUrlFromUIBySettings("edit/WorkflowImage.aspx")%>?InstanceId=' + id, null, 'dialogWidth:800px;dialogHeight:1000px;help=no;resizable:yes;scroll:yes;status:yes');
	    return false;
	}
	
		//]]>
</script>
<EPiServerScript:ScriptSettings runat="server" ConfirmMessage="<%$ Resources: EPiServer, workflow.edit.workflowlist.confirmterminate %>" ID="deleteCommon" />
<EPiServerScript:ScriptSettings runat="server" ConfirmMessage="<%$ Resources: EPiServer, workflow.edit.workflowlist.confirmterminateall %>" ID="deleteAll" />
<div class="epi-buttonDefault">
    <EPiServerUI:ToolButton id="Fetch" CausesValidation="false" OnClick="Fetch_OnClick" Text="<%$ Resources: EPiServer, workflow.edit.workflowlist.fetch %>" ToolTip="<%$ Resources: EPiServer, workflow.edit.workflowlist.fetch %>" runat="server" />
</div>
<Workflow:WorkflowDefinitionInstanceDataSource id="instanceDataSource" runat="server"/>
<Workflow:WorkflowDefinitionInstanceDataSource id="detailsDataSource" runat="server">  
    <SelectParameters>
        <asp:ControlParameter  ControlID="instanceList" Name="InstanceId" 
          PropertyName="SelectedValue"/>
    </SelectParameters>
</Workflow:WorkflowDefinitionInstanceDataSource>
    <asp:Panel ID="Count" Visible="false" runat="server" >
        <p><EPiServer:Translate runat="server" text="/workflow/edit/workflowlist/count" />: <%#InstanceListDataSource.Count%></p>
    </asp:Panel> 
    <asp:GridView 
        runat="server" 
        ID="instanceList" 
        DataKeyNames="InstanceId"
        AutoGenerateColumns="false"
        OnSelectedIndexChanged="RebindView"
        AllowPaging="true"
        UseAccessibleHeaders="true">
        <Columns>
            <asp:BoundField DataField="DefinitionName"  HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.definitionname %>" />
            <asp:BoundField DataField="Initiator"  HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.initiator %>" />
             <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details %>">
             <ItemTemplate> 
                <EPiServerUI:ToolButton CommandName="Select" SkinID="WorkflowInformation" ToolTip="<%$ Resources: EPiServer, workflow.edit.workflowlist.details %>"  runat="server" /> 
             </ItemTemplate>
            </asp:TemplateField>
             <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.view %>">
             <ItemTemplate> 
                <EPiServerUI:ToolButton SkinID="WorkflowView" OnClientClick="<%#CreateImageScript((EPiServer.WorkflowFoundation.WorkflowDefinitionInstance)Container.DataItem)%>" ToolTip="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.view %>"  runat="server" /> 
             </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.terminate %>">
             <ItemTemplate> 
                <EPiServerUI:ToolButton ID="deleteTool" CommandName="Delete" SkinID="Delete" EnableClientConfirm="true" Enabled="<%# HasDeleteAccess((EPiServer.WorkflowFoundation.WorkflowDefinitionInstance)Container.DataItem)%>" ToolTip="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.terminate %>"  runat="server" /> 
                <EPiServerScript:ScriptSettings runat="server" TargetControlID="deleteTool" CommonSettingsControlID="deleteCommon" Name='<%# Eval("Name") %>' />
             </ItemTemplate>
            </asp:TemplateField>
        </Columns>
     </asp:GridView>
     <div class="epi-buttonContainer">
        <EPiServerUI:ToolButton id="TerminateAll" Visible="false" runat="server" EnableClientConfirm="true" OnClick="TerminateAll_OnClick" Text="<%$ Resources: EPiServer, workflow.edit.workflowlist.terminateall %>" ToolTip="<%$ Resources: EPiServer, workflow.edit.workflowlist.terminateall %>" SkinId="Delete" />
        <EPiServerScript:ScriptSettings runat="server" TargetControlID="TerminateAll" CommonSettingsControlID="deleteAll" Name='<%# Eval("Name") %>' />
        <p><asp:Label runat="server" ID="lackTerminateRights" Visible="false" Text="<%$ Resources: EPiServer, workflow.edit.workflowlist.missingdeleteaccess %>" /></p>
    </div>

    <asp:panel ID="pnlDetail" runat="server" Visible="false">
    <div class="epi-contentContainer epi-fullWidth">
         <asp:DetailsView 
            runat="server"
            id="instanceDetail"
            DataKeyNames="InstanceId"
            AutoGenerateRows="false"
            DataSourceID="detailsDataSource"
            UseAccessibleHeaders="true"
            HeaderStyle-HorizontalAlign="Center"
            HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details %>">
            <Fields>
                <asp:BoundField DataField="DefinitionName"  HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.definitionname %>" />
                <asp:BoundField DataField="Initiator"  HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.initiator %>" />
                <asp:TemplateField HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.page %>">
                    <ItemTemplate>
                        <%# Server.HtmlEncode(GetRelatedPage(((EPiServer.WorkflowFoundation.WorkflowDefinitionInstance)Container.DataItem).InstanceId))%>
                    </ItemTemplate>
                </asp:TemplateField> 
                 <asp:TemplateField HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.history %>">
                    <ItemTemplate>
                        <Workflow:History Id="History" runat="server" AccessLevel="<%#detailsDataSource.RequiredAccess%>" WorkflowInstanceId="<%# ((EPiServer.WorkflowFoundation.WorkflowDefinitionInstance)Container.DataItem).InstanceId%>" />
                    </ItemTemplate>
                </asp:TemplateField>  
                 <asp:TemplateField HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.task %>">
                    <ItemTemplate>
                        <Workflow:Task Id="Task" runat="server" WorkflowInstanceId="<%# ((EPiServer.WorkflowFoundation.WorkflowDefinitionInstance)Container.DataItem).InstanceId%>" />
                    </ItemTemplate>
                </asp:TemplateField>                                  
                <asp:BoundField DataField="InstanceId"  HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.instanceid %>" />
                <asp:BoundField DataField="TypeFullName" HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.type %>" />
                 <asp:TemplateField HeaderText="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.waitingevents %>">
                    <ItemTemplate>
                        <%# GetWaitingEvents(((EPiServer.WorkflowFoundation.WorkflowDefinitionInstance)Container.DataItem).InstanceId)%>
                    </ItemTemplate>
                </asp:TemplateField>  
                 <asp:TemplateField ShowHeader="False">
                 <ItemTemplate>
                    <div class="epi-floatRight">
                        <EPiServerUI:ToolButton ID="deleteTool1" CommandName="Delete" SkinID="Delete" EnableClientConfirm="true" Enabled="<%# HasDeleteAccess((EPiServer.WorkflowFoundation.WorkflowDefinitionInstance)Container.DataItem)%>" Text="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.terminate %>" ToolTip="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.terminate %>"  runat="server" /> 
                        <EPiServerScript:ScriptSettings runat="server" TargetControlID="deleteTool1" CommonSettingsControlID="deleteCommon" Name='<%# Eval("Name") %>' />
                        <EPiServerUI:ToolButton ID="Cancel" SkinID="Cancel" OnClick="Cancel_Click" Text="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>" ToolTip="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>"  runat="server" />
                    </div>
                 </ItemTemplate>
                </asp:TemplateField> 
            </Fields>
        </asp:DetailsView>
    </div>
    </asp:panel>


