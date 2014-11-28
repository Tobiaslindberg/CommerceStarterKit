<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommerceManager.aspx.cs" 
Inherits="EPiServer.Commerce.AddOns.Manager.Admin.SitePlugin.CommerceManager, EPiServer.Commerce.AddOns.Manager" %>
<%@ Register TagPrefix="sc" Assembly="EPiServer.Shell" Namespace="EPiServer.Shell.Web.UI.WebControls" %>
<%@ Register TagPrefix="EPiServerUI" Assembly="EPiServer.UI" Namespace="EPiServer.UI.WebControls" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="FullRegion" runat="server">
<table cellpadding="0" cellspacing="0" border="0" style="height: 100%; width: 100%; border-collapse: collapse;">
    <tr style="height: 31px;">
    <td>
        <sc:ShellMenu ID="CommerceManagerShellMenu" runat="server"  />
    </td>
    </tr>
    <tr>
    <td>
        <iframe class="episystemiframe" frameborder="0" src="<%= FrameSource %>" ></iframe>
    </td>
    </tr>
</table>
</asp:Content>