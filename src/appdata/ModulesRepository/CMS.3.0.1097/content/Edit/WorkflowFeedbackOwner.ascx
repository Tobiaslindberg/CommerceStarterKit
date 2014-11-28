<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowFeedbackOwner.ascx.cs" Inherits="EPiServer.WorkflowFoundation.Workflows.WorkflowFeedbackOwner" %>
<%@ Register TagPrefix="Workflow" TagName="History" Src="WorkflowHistoryList.ascx" %>

<EPiServer:Translate runat="server" text="/workflows/feedbackrequest/ownerheading" />:
<br />
<asp:Label runat="server" ID="RequestDescription" />
<br />
<br />
<EPiServer:Translate runat="server" text="/workflows/feedbackrequest/feedback" />
<Workflow:History Id="History" runat="server" />
