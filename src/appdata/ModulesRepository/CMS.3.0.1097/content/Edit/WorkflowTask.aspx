<%@ Page Language="c#" CodeBehind="WorkflowTask.aspx.cs" AutoEventWireup="True" Inherits="EPiServer.UI.Edit.WorkflowTask"  %>
<%@ Register TagPrefix="EPiServerUI" TagName="WorkflowTaskControl" Src="WorkflowTaskControl.ascx" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="FullRegion">
    <div style="padding: 0.8em">
        <EPiServerUI:WorkflowTaskControl runat="server" />
    </div>
</asp:Content>
