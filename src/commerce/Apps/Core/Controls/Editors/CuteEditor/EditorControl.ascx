<%@ Control Language="C#" AutoEventWireup="true" Inherits="Module_Editors_CuteEditor_EditorControl" Codebehind="EditorControl.ascx.cs" %>
<%@ Register TagPrefix="CE" Namespace="CuteEditor" Assembly="CuteEditor" %>
<CE:Editor id="HtmlTextBoxCtrl" FilesPath="~/Apps/Core/Controls/Editors/CuteEditor" Width="700" Height="400" runat="server" AutoConfigure="Simple" ThemeType="Office2003_BlueTheme" EditorWysiwygModeCss="~/Apps/Core/Controls/Editors/CuteEditor/style.css" >
    <FrameStyle BackColor="White" BorderColor="#DDDDDD" BorderStyle="Solid" BorderWidth="1px"
        CssClass="CuteEditorFrame" Height="100%" Width="100%" />
</CE:Editor>