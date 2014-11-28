 <%@ Page Language="c#" Codebehind="WorkflowAdmin.aspx.cs" AutoEventWireup="False" Inherits="EPiServer.WorkflowFoundation.UI.WorkflowAdmin"  Title="Workflow admin" %>
<%@ Register TagPrefix="Workflow" TagName="Instances" Src="../edit/WorkflowInstanceList.ascx" %>
<%@ Register TagPrefix="Workflow" Namespace="EPiServer.WorkflowFoundation.WebControls" Assembly="EPiServer.WorkflowFoundation"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainRegion" runat="server">

      <EPiServerScript:ScriptSettings runat="server" ConfirmMessage="<%$ Resources: EPiServer, admin.workflowadmin.confirmdelete %>" ID="deleteCommon" />

      <EPiServerUI:TabStrip runat="server" id="actionTab" GeneratesPostBack="False" targetid="tabView">
	        <EPiServerUI:Tab Text="/admin/workflowadmin/deftab" runat="server" ID="Tab1" Sticky="true"/>
			<EPiServerUI:Tab Text="/admin/workflowadmin/runningtab" runat="server" ID="Tab2" Sticky="true"/>
			<%--  for debugging add to the line below runat="server" and remove  Visible="false"--%>
			<EPiServerUI:Tab Text="/admin/workflowadmin/systemtab" Visible="false" ID="Tab3" Sticky="true"/>
	</EPiServerUI:TabStrip>
	
    <asp:Panel runat="server" id="tabView">
        <asp:Panel runat="server" ID="Definitions" CssClass="epi-padding">
            <Workflow:WorkflowDefinitionDataSource id="definitionDataSource" runat="server" />
            <asp:Label runat="server" ID="ErrorMessage" ForeColor="Red" />
                <asp:GridView 
                    runat="server" 
                    ID="definitionList" 
                    DataKeyNames="DefinitionId"
                    DataSourceID="definitionDataSource"
                    AutoGenerateColumns="false"
                    OnRowCommand="definitionList_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="DefinitionName" HeaderStyle-CssClass="epitableheading"  HeaderText="<%$ Resources: EPiServer, admin.workflowadmin.definitionname %>" />
                        <asp:TemplateField HeaderStyle-CssClass="epitableheading" HeaderText="<%$ Resources: EPiServer, admin.workflowadmin.type %>">
                            <ItemTemplate><%# GetWorkflowDefinitionType(((EPiServer.WorkflowFoundation.WorkflowDefinition)Container.DataItem).DefinitionId) %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="epitableheading" HeaderText="<%$ Resources: EPiServer, admin.workflowadmin.startevents %>">
                            <ItemTemplate><%# GetStartEvents(((EPiServer.WorkflowFoundation.WorkflowDefinition)Container.DataItem).DefinitionId) %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="epitableheading" HeaderText="<%$ Resources: EPiServer, admin.workflowdefinitionedit.copy %>">
                         <ItemTemplate> 
                            <EPiServerUI:ToolButton CommandName="Copy" SkinID="Copy" CausesValidation="false" ToolTip="<%$ Resources: EPiServer, admin.workflowdefinitionedit.copy %>" CommandArgument='<%# Eval("DefinitionId") %>' runat="server" />
                         </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="epitableheading" HeaderText="<%$ Resources: EPiServer, button.edit%>">
                         <ItemTemplate> 
                            <EPiServerUI:ToolButton CommandName="Edit" SkinID="Edit" CausesValidation="false" ToolTip="<%$ Resources: EPiServer, button.edit %>" CommandArgument='<%# Eval("DefinitionId") %>' runat="server" />
                         </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="epitableheading" HeaderText="<%$ Resources: EPiServer, button.delete%>">
                         <ItemTemplate> 
                           <EPiServerUI:ToolButton ID="deleteTool" CommandName="DeleteDefinition" CommandArgument='<%# Eval("DefinitionId") %>' SkinID="Delete" EnableClientConfirm="true" ToolTip="<%$ Resources: EPiServer, button.delete %>"  runat="server" /> 
                           <EPiServerScript:ScriptSettings runat="server" TargetControlID="deleteTool" CommonSettingsControlID="deleteCommon" Name='<%# Eval("Name") %>' />
                         </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <EPiServerUI:ToolButton SkinID="Add" Text="<%$ Resources: EPiServer, admin.workflowadmin.createnew %>"  OnClick="CreateNew_Click" ToolTip="<%$ Resources: EPiServer, admin.workflowadmin.createnew %>"  runat="server" />
        </asp:Panel>
        
        <asp:Panel CssClass="epi-formArea epi-padding" runat=server ID="Running">
            <div class="epi-size15">
                <div>
                    <asp:Label AssociatedControlID="FilterList" runat=server Translate="/admin/workflowadmin/filterondefinition" />
                    <asp:DropDownList AutoPostBack=true runat=server ID="FilterList" OnSelectedIndexChanged="FilterOnType"/>
                </div>
            </div>
            <Workflow:Instances runat="server" id="InstanceListControl" />
        </asp:Panel>
        
        <%--  for debugging change the attribute visible to true--%>
        <asp:Panel runat="server" ID="SystemInfo" CssClass="epi-padding" Visible="false">
            <div class="epi-buttonDefault">
            <EPiServerUI:ToolButton id="FetchSystemInfo" OnClick="Fetch_OnClick" Text="<%$ Resources: EPiServer, admin.workflowadmin.fetchsystem %>" ToolTip="<%$ Resources: EPiServer, admin.workflowadmin.fetchsystem %>" runat="server" />
            </div>
            <asp:Panel runat="server" ID="SystemPanel" Visible="false">
                <p><EPiServer:Translate runat=server text="/admin/workflowadmin/eventlist" /></p>
                <asp:GridView runat="server" ID="EventList" AutoGenerateColumns="false">    
                   <Columns>
                    <asp:BoundField DataField="Key" HeaderText="<%$ Resources: EPiServer, admin.workflowadmin.eventname %>" />
                    <asp:BoundField DataField="Value" HeaderText="<%$ Resources: EPiServer, admin.workflowadmin.count %>" />         
                   </Columns>
                </asp:GridView>
            </asp:Panel>
        </asp:Panel>
 
    </asp:Panel>
</asp:Content>



