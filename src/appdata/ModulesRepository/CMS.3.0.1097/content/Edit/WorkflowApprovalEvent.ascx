<%@ Control language="c#" Codebehind="WorkflowApprovalEvent.ascx.cs" AutoEventWireup="false" Inherits="EPiServer.WorkflowFoundation.Workflows.WorkflowApprovalEvent" %>
<%@ Register TagPrefix="Workflow" TagName="History" Src="WorkflowHistoryList.ascx" %>
<%@ Register TagPrefix="Workflow" TagName="VersionControl" Src="WorkflowApprovalVersionControl.ascx" %>
<Workflow:VersionControl Id="VersionControl" runat="server" />
<h2><EPiServer:Translate runat="server" text="/workflows/approval/approvaltask/pageheading" />&nbsp;<%= Server.HtmlEncode(PageName)%></h2>
 
<EPiServer:Translate runat="server" text="/workflows/approval/approvedevent/comment" />
<br />
<asp:TextBox TextMode="MultiLine" runat="server" ID="Message" Rows="5" Width="400" />
<br />
<asp:RadioButton ID="Approved" Checked="true" runat="server" GroupName="ApprovedGroup" Text="<%$ Resources: EPiServer, workflows.approval.approvedevent.approved %>" />
<asp:RadioButton ID="NotApproved" Checked="false" runat="server" GroupName="ApprovedGroup" Text="<%$ Resources: EPiServer, workflows.approval.approvedevent.notapproved %>" />
<br />
<br />
<asp:Panel runat="server" ID="HistoryPanel" Visible="<%#ShowHistory%>">
    <!--ToggleDescription method is defined in WorkflowHistoryList.ascx-->
    <a href="javascript:void(0);" onclick="ToggleDescription('workflowhistory')";>
                    <EPiServer:Translate runat="server" text="/workflow/edit/workflowhistory/history" /></a>
	<div id="workflowhistory" style="display:none;">
        <Workflow:History Id="History" runat="server" />
    </div>
</asp:Panel>
<br />
