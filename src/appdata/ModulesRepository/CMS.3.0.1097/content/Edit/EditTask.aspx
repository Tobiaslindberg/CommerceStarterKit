<%@ Page Language="c#" CodeBehind="EditTask.aspx.cs" AutoEventWireup="True" Inherits="EPiServer.UI.Edit.EditTask"  %>

<%@ Register TagPrefix="EPiServerUI" TagName="EditTask" Src="EditTask.ascx" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="FullRegion">
    <div style="padding: 0.8em">
       <episerverui:EditTask runat="server" />
    </div>
</asp:Content>