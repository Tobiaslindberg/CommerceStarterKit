<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowFeedbackUser.ascx.cs" Inherits="EPiServer.WorkflowFoundation.Workflows.WorkflowFeedbackUser" %>
<%@ Register TagPrefix="Workflow" TagName="History" Src="WorkflowHistoryList.ascx" %>
<h2><EPiServer:Translate runat="server" text="/workflows/feedbackrequest/userheading" /></h2>
<br />
<b><EPiServer:Translate runat="server" text="/workflow/edit/workflowtask/description" /></b>:
<br />
<asp:Label runat="server" ID="RequestDescription" />
<br />
<br />
<EPiServer:Translate runat="server" text="/workflows/feedbackrequest/feedback" />
<br />
<asp:TextBox TextMode="MultiLine" runat="server" ID="Message" Rows="5" Width="400" />
<br />
<!--ToggleDescription method is defined in WorkflowHistoryList.ascx-->
<a href="javascript:void(0);" onclick="ToggleDescription('workflowhistory')";>
    <EPiServer:Translate ID="Translate1" runat="server" text="/workflow/edit/workflowhistory/history" />
</a>
<div id="workflowhistory" style="display: none;">
    <Workflow:History ID="History" runat="server" />
</div>
<br />
