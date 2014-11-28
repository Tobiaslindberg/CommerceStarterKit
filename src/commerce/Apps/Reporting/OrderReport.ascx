<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderReport.ascx.cs" Inherits="OxxCommerceStarterKit.Commerce.Apps.Reporting.OrderReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Src="~/Apps/Core/Controls/CalendarDatePicker.ascx" TagName="CalendarDatePicker"
    TagPrefix="ecf" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI.HtmlControls" Assembly="System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>
<div class="report-view">
    <div class="report-filter">
        <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:SharedStrings, Market %>"/>:
        <asp:DropDownList runat="server" ID="MarketFilter" AutoPostBack="true" DataValueField="MarketId" DataTextField="MarketName" Width="250"></asp:DropDownList>
        <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:SharedStrings, Currency %>"/>:
        <asp:DropDownList runat="server" ID="CurrencyFilter" AutoPostBack="true" DataValueField="CurrencyCode" DataTextField="Name"></asp:DropDownList>
        <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:SharedStrings, Start_Date %>"/>:
        <ecf:CalendarDatePicker runat="server" ID="StartDate" TimeDisplay="false"/>
        <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:SharedStrings, End_Date %>"/>:
        <ecf:CalendarDatePicker runat="server" ID="EndDate" TimeDisplay="false"/>              
        <asp:Button ID="btnSearch" runat="server" Width="100" Text="<%$ Resources:ReportingStrings, Apply_Filter %>" 
            onclick="btnSearch_Click" />
    </div>
    <div class="report-content">
        <rsweb:ReportViewer OnBookmarkNavigation="MyReportViewer_BookmarkNavigation" ZoomMode="Percent"
            SizeToReportContent="true" AsyncRendering="false" ID="MyReportViewer" runat="server" ShowDocumentMapButton="False"
            ShowExportControls="True" ShowFindControls="False" ShowPageNavigationControls="True"
            ShowPrintButton="True" ShowPromptAreaButton="False" ShowRefreshButton="True"
            ShowZoomControl="True" Font-Names="Verdana" Font-Size="8pt" Width="100%" Height="90%"
            HyperlinkTarget="_blank">
            <LocalReport EnableHyperlinks="True">
            </LocalReport>
        </rsweb:ReportViewer>
    </div>         
</div>