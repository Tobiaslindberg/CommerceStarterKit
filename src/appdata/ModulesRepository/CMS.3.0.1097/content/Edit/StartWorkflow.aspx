<%@ Page Language="c#" CodeBehind="StartWorkflow.aspx.cs" AutoEventWireup="False" Inherits="EPiServer.UI.Edit.StartWorkflow"  %>

<%@ Register TagPrefix="EPiServer" Namespace="EPiServer.Web.WebControls" Assembly="EPiServer" %>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="FullRegion">
    <script type='text/javascript'>
        //<![CDATA[

        function SetDefinitionId(id) {
            var definitionId = window.document.getElementById('<%=DefinitionId.ClientID%>');
            definitionId.value = id;
        }

    //]]>
    </script>
    <div style="padding: 0.8em">

        <asp:HiddenField ID="PageLink" runat="server" />
        <asp:Button ID="ClientPostback" OnCommand="ClientPostback_Command" Visible="false" runat="server" />
        <asp:Panel runat="server" ID="SelectDefinition">
            <h2>
                <episerver:translate runat="server" text="/workflow/edit/workflowinstance/start/selectdefinition" />
            </h2>
            <asp:Label runat="server" ID="NoDefinitions" />
            <br />
            <br />
            <asp:Repeater ID="rptDefinitions" runat="server" OnItemCommand="definitions_ItemCommand">
                <ItemTemplate>
                    <div class="listheadingcontainer">
                        <%# HttpUtility.HtmlEncode((string)DataBinder.Eval(Container.DataItem, "Description")) %>
                    </div>
                    <div style="padding-bottom: 1.2em;">
                        <span onclick="SetDefinitionId('<%# DataBinder.Eval(Container.DataItem, "DefinitionId") %>');">
                            <asp:LinkButton ID="LinkButton1"
                                ToolTip='<%# HttpUtility.HtmlAttributeEncode((string)DataBinder.Eval(Container.DataItem, "Description")) %>'
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DefinitionId") %>'
                                runat="server"
                                Text='<%# HttpUtility.HtmlEncode((string)DataBinder.Eval(Container.DataItem, "DefinitionName"))%>'
                                CssClass="epi-visibleLink"
                                CausesValidation="false" />
                        </span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <episerverui:toolbutton id="Cancel" skinid="Cancel" onclick="Cancel_Click" text="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>" tooltip="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>" runat="server" />
        </asp:Panel>
        <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="EP-validationSummary" ForeColor="Black" />
        <asp:Panel runat="server" ID="StartParameters" Visible="false">
            <asp:PlaceHolder ID="plcStartParams" runat="server" />
            <br clear="all" />
            <div class="epi-buttonContainer">
                <episerverui:toolbutton id="Start" onclick="Save_OnClick" text="<%$ Resources: EPiServer, workflow.edit.workflowinstance.start.start %>" tooltip="<%$ Resources: EPiServer, workflow.edit.workflowinstance.start.start %>" runat="server" />
                <episerverui:toolbutton id="Cancel2" onclick="Cancel_Click" text="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>" tooltip="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>" runat="server" skinid="Cancel" />
            </div>
        </asp:Panel>
        <asp:HiddenField runat="server" ID="DefinitionId" />
    </div>
</asp:Content>