<%@ Page Language="C#"  AutoEventWireup="true"
    CodeBehind="ContentChannelEdit.aspx.cs" Inherits="EPiServer.UI.Admin.ContentChannelEdit"
    Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentRegion" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainRegion" runat="server">
    <div class="epi-formArea epi-paddingVertical-small">
        <div class="epi-size10 epi-paddingVertical-small">
            <div>
                <asp:Label runat="server" AssociatedControlID="ChannelName" Translate="#channelname" />
                <asp:TextBox ID="ChannelName" Columns="25" runat="server" CssClass="EP-requiredField" MaxLength="255" />
                <asp:RequiredFieldValidator ControlToValidate="ChannelName" runat="server" ID="NameValidator"
                    ErrorMessage="<%$ Resources: EPiServer, admin.contentchanneledit.missingname %>">*</asp:RequiredFieldValidator>
                <asp:CustomValidator runat="server" ControlToValidate="ChannelName" ID="ChannelNameValidator" OnServerValidate="ValidateChannelName">*</asp:CustomValidator>
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="PageRoot" Translate="#channelpageroot" />
                <EPiServer:InputPageReference runat="Server" ID="PageRoot" DisableCurrentPageOption="true"
                    Style="display: inline;" />
            </div>
             <div>
                <asp:Label runat="server" AssociatedControlID="BlockRoot" Translate="#blockroot" />
                <asp:TextBox ID="BlockRoot" Columns="25" runat="server" />
                <asp:CustomValidator runat="server" ID="BlockValidator" OnServerValidate="ValidateBlockRoot"
                    ErrorMessage="<%$ Resources: EPiServer, admin.contentchanneledit.notunderblockroot %>">*</asp:CustomValidator>
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="VppRoot" Translate="#channelvpproot" />
                <asp:TextBox ID="VppRoot" Columns="25" runat="server" />
                <asp:CustomValidator runat="server" ID="VppValidator" OnServerValidate="ValidateVppRoot"
                    ErrorMessage="<%$ Resources: EPiServer, admin.contentchanneledit.nofilemapping %>">*</asp:CustomValidator>
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="ChannelSaveAction" Translate="#saveaction" />
                <asp:DropDownList runat="server" ID="ChannelSaveAction" />
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="DefaultContentType" Translate="#defaultcontenttype" />
                <asp:DropDownList runat="server" ID="DefaultContentType" />
            </div>
        </div>
        <fieldset>
            <legend><span><EPiServer:Translate Text="#propertymappingheader" runat="server" /></span></legend>
            <div class="epi-size15">
                <div>
                    <asp:Label runat="server" AssociatedControlID="NoneMappedContentTypes" Translate="#nonemappedcontenttypes" />
                    <asp:DropDownList runat="server" ID="NoneMappedContentTypes" AutoPostBack="true" OnSelectedIndexChanged="NoneMappedContentTypeSelected" />
                </div>
                <div>
                    <asp:Label runat="server" AssociatedControlID="MappedContentTypes" Translate="#mappedcontenttypes" />
                    <asp:DropDownList runat="server" ID="MappedContentTypes" AutoPostBack="true" OnSelectedIndexChanged="MappedContentTypeSelected" />
                </div>
            </div>
            
            <asp:Panel runat="server" ID="PropertyMapping" Visible="false">
                <p>
                    <EPiServer:Translate Text="#currentpropertymapping" runat="server" />&nbsp;<em><asp:Label ID="CurrentPageType" runat="server" /></em>
                </p>
                <asp:Table CssClass="epi-default" runat="server" ID="PropertyMap">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell><episerver:translate text="#contenttypeproperty" runat="server" /></asp:TableHeaderCell>
                        <asp:TableHeaderCell><episerver:translate text="#mappedproperty" runat="server" /></asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
                <div class="epi-buttonDefault">
                    <EPiServerUI:ToolButton ID="UpdatePropertyMapping" DisablePageLeaveCheck="true" CausesValidation="false" runat="server" Text="<%$ Resources: EPiServer, admin.contentchanneledit.updatepropertymapping %>" ToolTip="<%$ Resources: EPiServer, admin.contentchanneledit.updatepropertymapping %>" OnClick="SaveMapping" Visible="false" /><EPiServerUI:ToolButton ID="SavePropertyMapping" DisablePageLeaveCheck="true" CausesValidation="false" runat="server" Text="<%$ Resources: EPiServer, admin.contentchanneledit.savepropertymapping %>" ToolTip="<%$ Resources: EPiServer, admin.contentchanneledit.savepropertymapping %>" OnClick="SaveMapping" /><EPiServerUI:ToolButton ID="DeletePropertyMapping" DisablePageLeaveCheck="true" CausesValidation="false" runat="server" Text="<%$ Resources: EPiServer, admin.contentchanneledit.deletepropertymapping %>" ToolTip="<%$ Resources: EPiServer, admin.contentchanneledit.deletepropertymapping %>" OnClick="DeleteMapping" />
                </div>
            </asp:Panel>
            
        </fieldset>
    </div>
    <div class="epi-buttonContainer">
        <EPiServerUI:ToolButton ID="DeleteButton" EnableClientConfirm="true" DisablePageLeaveCheck="true" CausesValidation="false" runat="server" Text="<%$ Resources: EPiServer, button.delete %>" ToolTip="<%$ Resources: EPiServer, button.delete %>" SkinID="Delete" OnClick="DeleteChannel" Enabled="<%# !IsNew %>" /><EPiServerScript:ScriptSettings runat="server" ConfirmMessage="<%$ Resources: EPiServer, admin.contentchannellist.confirmdelete %>" TargetControlID="DeleteButton" Name='<%# Eval("ChannelId") %>' /><EPiServerUI:ToolButton ID="SaveButton" DisablePageLeaveCheck="true" runat="server" Text="<%$ Resources: EPiServer, button.save %>" ToolTip="<%$ Resources: EPiServer, button.save %>" SkinID="Save" OnClick="SaveChannel" /><EPiServerUI:ToolButton ID="CancelButton" DisablePageLeaveCheck="true" CausesValidation="false" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>" ToolTip="<%$ Resources: EPiServer, button.cancel %>" SkinID="Cancel" OnClick="Cancel" />
    </div>
</asp:Content>
