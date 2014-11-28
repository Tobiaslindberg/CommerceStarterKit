<%@ Control Language="c#" AutoEventWireup="False" Codebehind="UserGuiSettings.ascx.cs" Inherits="EPiServer.UI.Edit.UserGuiSettings" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div class="epi-formArea epi-paddingHorizontal">
    <fieldset>
        <legend>
            <span>
                <asp:Literal runat="server" Text="<%$ Resources: EPiServer, admin.edituser.userguisettings.settings.legendlanguagesettings %>"></asp:Literal>
            </span>
        </legend>
        <div runat="server" id="languageGui" class="epi-size10">
            <label><asp:Literal runat="server" ID="languageText" Text="<%$ Resources: EPiServer, admin.secedit.editlanguage %>"></asp:Literal></label>
		    <asp:DropDownList runat="server" ID="languageDropDown" SkinID="Size300"></asp:DropDownList>
        </div>
    </fieldset>
    <fieldset>
        <legend>
            <span>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: EPiServer, admin.edituser.userguisettings.settings.viewsettings %>"></asp:Literal>
            </span>
        </legend>
        <div>
            <asp:Literal runat="server" ID="ResetInfoMessage" Text="<%$ Resources: EPiServer, admin.edituser.userguisettings.settings.resetviewsinfo%>" />
            <p><EPiServerUI:ToolButton runat="server" Text="<%$ Resources: EPiServer, admin.edituser.userguisettings.settings.resetviews%>" OnClick="ResetView_Click" /></p>
            <asp:Label CssClass="EP-systemMessage" runat="server" ID="ResetMessage" Visible="false" Text="<%$ Resources: EPiServer, admin.edituser.userguisettings.settings.resetviewsdone%>" />
        </div>
    </fieldset>
</div>