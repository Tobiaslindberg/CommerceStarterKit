<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MySettings.aspx.cs" Inherits="EPiServer.UI.Edit.MySettings" %>
<%@ Register TagPrefix="EPiServer" TagName="UserMembership" Src="UserMembership.ascx" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="epi-contentArea">
        <EPiServerUI:SystemPrefix ID="componentsPrefix" runat="server" />
        <asp:validationsummary id="ValidationSummary" runat="server" cssclass="EP-validationSummary" forecolor="Black"/>
    </div>
    <EPiServer:UserMembership runat="server" />
</asp:Content>