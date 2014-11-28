<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditPageTypePropertyControl.ascx.cs"
    Inherits="EPiServer.UI.Admin.EditPageTypePropertyControl" %>
 
<div class="epi-paddingVertical">
    <h4 class="delimiter"><asp:Literal runat="server" ID="ControlName" /></h4>
    <asp:Panel runat="server" ID="SettingsArea" CssClass="epi-paddingHorizontal-small">
        <EPiServerScript:ScriptDisablePageLeaveEvent EventType="Change" EventTargetID="UseGlobalSettings" runat="server" />
        <EPiServerScript:ScriptDisablePageLeaveEvent EventType="Change" EventTargetID="UseCustomSettings" runat="server" />
        <EPiServerScript:ScriptDisablePageLeaveEvent EventType="Change" EventTargetID="GlobalSettingsSelection" runat="server" />
        
        <asp:Panel runat="server" ID="GlobalSettingsPanel">
            <asp:RadioButton runat="server" ID="UseGlobalSettings" Text="<%$ Resources: EPiServer, admin.editpropertydefinition.useglobalsettings %>" GroupName="SettingsType" AutoPostBack="true" CausesValidation="false" OnCheckedChanged="UseGlobalRadioButton_Changed" />
            <asp:DropDownList runat="server" ID="GlobalSettingsSelection" AutoPostBack="true" OnSelectedIndexChanged="GlobalSettingsSelection_Changed" SkinID="Custom" /><a class="epi-paddingHorizontal" href="PageDefinitionTypeEdit.aspx?typeId=<%= PropertyDefinitionData.Type.ID %>&returnUrl=<%= Server.UrlEncode(Request.Url.PathAndQuery) %>"><asp:Literal runat="server" Text="<%$ Resources: EPiServer, admin.editpropertydefinition.manageglobalsettings %>" /></a> 
            <br />
            <asp:RadioButton ID="UseCustomSettings" runat="server" Text="<%$ Resources: EPiServer, admin.editpropertydefinition.usecustomsettings %>" GroupName="SettingsType" AutoPostBack="true" CausesValidation="false" OnCheckedChanged="UseCustomRadioButton_Changed" />
        </asp:Panel>
        
        <div class="epi-paddingVertical-small">
            <h4 class="delimiter"><asp:Literal runat="server" Text="<%$ Resources: EPiServer, admin.editpropertydefinition.typesettingsheader %>" /></h4>
        </div>
            <asp:PlaceHolder runat="server" ID="ControlSettingsEditor" />
        
    </asp:Panel>
    <asp:Label runat="server" ID="NoSettingsInformation" CssClass="epi-paddingHorizontal-small" Text="<%$ Resources: EPiServer, admin.editpropertydefinition.classhasnosettings %>" Visible="false" />
</div>
