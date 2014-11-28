<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowApprovalVersionControl.ascx.cs" Inherits="EPiServer.WorkflowFoundation.Workflows.WorkflowApprovalVersionControl" %>
<%@ Register TagPrefix="Approval" Namespace="EPiServer.WorkflowFoundation.Workflows.WebControls" Assembly="EPiServer.UI" %>

<asp:CustomValidator ID="VersionValidator" runat="server" Display="None" OnServerValidate="ValidatePageVersion" />
<asp:Panel ID="VersionConflict" runat="server" Visible="false">
    <span style="color:Red"><EPiServer:Translate runat="server" text="/workflows/approval/versioncontrol/selectversion" /></span>
    <br />
    <Approval:VersionRadioButton ID="WorkflowVersion" Checked="false" runat="server" GroupName="VersionGroup" Text="<%$ Resources: EPiServer, workflows.approval.versioncontrol.workflowversion %>" />
    <a href="javascript: return void;" onclick="<%=WorkflowVersionLink%>"><%=WorkflowVersionName%></a> 
    <br />
    <Approval:VersionRadioButton ID="LoadedVersion" Checked="true" runat="server" GroupName="VersionGroup" Text="<%$ Resources: EPiServer, workflows.approval.versioncontrol.loadedversion %>" />
    <a href="javascript: return void;" onclick="<%=WorkflowLoadedLink%>"><%=WorkflowLoadedName%></a> 
</asp:Panel>