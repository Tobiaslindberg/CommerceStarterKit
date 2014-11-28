<%@ Page Language="c#" Debug="true" Codebehind="siteinfo.aspx.cs" AutoEventWireup="False" Inherits="EPiServer.UI.Admin.Siteinfo"  Title="SiteInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="FullRegion" runat="server">
<div class="epi-contentContainer epi-padding-large">
    
    <EPiServerUI:RefreshFrame visible="False" id="frameUpdater" framename="AdminMenu" selectedtabname="AdminTab" runat="server" />

    <table id="DatabaseTable" runat="server" class="epi-default">
            <tr>
                <th colspan="2">
                    <episerver:translate text="#dbstatcaption" runat="server" />
                </th>
            </tr>
        </table>
    
        <table id="DataFactoryTable" runat="server" class="epi-default" visible="false">
            <tr>
                <th colspan="2">
                    <episerver:translate text="#dbcachecaption" runat="server" />
                </th>
            </tr>
        </table>
        
        <div class="epi-buttonDefault">
            <EPiServerUI:ToolButton id="ResetDatabaseCountersButton" runat="server" Text="<%$ Resources: EPiServer, admin.siteinfo.buttonresetcounters %>" ToolTip="<%$ Resources: EPiServer, admin.siteinfo.buttonresetcounters %>" SkinID="Check" OnClick="ResetButton_Click" Visible="false" />
        </div>
    
        <asp:Repeater runat="server" ID="PageProviderStatistics" OnItemDataBound="SetupPageProviderStatisticsInfo">
            <ItemTemplate>
            <table class="epi-default">
                    <tr>
                        <th colspan="2">
                            <asp:Label ID="dbcachecaption" runat="server"></asp:Label>
                        </th>
                    </tr>
                <tr>
                    <td width="50%">
                        <asp:Label ID="countersince" runat="server"></asp:Label>
                    </td>
                    <td width="50%">
                        <asp:Label ID="countersince_value" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="countergetitem" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="countergetitem_value" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="counterreadsinglepage" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="counterreadsinglepage_value" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="counterlistchildren" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="counterlistchildren_value" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="counterreadchildpages" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="counterreadchildpages_value" runat="server"></asp:Label>
                    </td>
                </tr>
               <tr>
                    <td>
                        <asp:label id="dbcachehitpercentage" runat="server"></asp:label>
                    </td>
                    <td>
                        <asp:label id="dbcachehitpercentage_value" runat="server"></asp:label>
                    </td>
                </tr>
                 </table>
                 <div class="epi-buttonDefault">
                    <episerverui:toolbutton id="ResetButton" runat="server" text="<%$ Resources: EPiServer, admin.siteinfo.buttonresetcounters %>" tooltip="<%$ Resources: EPiServer, admin.siteinfo.buttonresetcounters %>" skinid="Check" oncommand="ResetButton_Command" />
                 </div>
            </ItemTemplate>
        </asp:Repeater>
</div>
</asp:Content>
