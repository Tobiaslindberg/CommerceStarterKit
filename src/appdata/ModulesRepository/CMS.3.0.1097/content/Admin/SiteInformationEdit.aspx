<%@ Page Language="c#" CodeBehind="SiteInformationEdit.aspx.cs" AutoEventWireup="False" Inherits="EPiServer.UI.Admin.SiteInformationEdit"
     Title="SiteInformationEdit" %>
<%@ Import Namespace="EPiServer.Web" %>

<asp:Content ID="ContentForMainRegion" ContentPlaceHolderID="MainRegion" runat="server">
    <EPiServerUI:TabStrip runat="server" ID="actionTab" GeneratesPostBack="False" TargetID="tabView">
	    <EPiServerUI:Tab Sticky="true" Text="<%$ Resources: EPiServer, admin.siteinformationedit.sitetab %>" runat="server" ID="settingsTab" />
</EPiServerUI:TabStrip>

<asp:Panel runat="server" ID="tabView">
    <asp:Panel CssClass="epi-formArea epi-paddingHorizontal" ID="SiteSettings" runat="server">
        <fieldset>
            <legend><EPiServer:Translate Text="<%$ Resources: EPiServer, admin.siteinformationedit.generalgroup %>" runat="server" ID="Translate6"/></legend>
            <div class="epi-size10">
                <div>                
                    <asp:Label runat="server" AssociatedControlID="SiteName" Translate="<%$ Resources: EPiServer, admin.siteinformationlist.sitename%>" />
                    <asp:TextBox runat="server" ID="SiteName"/>
                    <asp:RequiredFieldValidator ID="SiteNameValidator" runat="server" EnableClientScript="true" ControlToValidate="SiteName" Text="*" Display="Dynamic" />
                </div>
                <div>
					<asp:Label ID="LabelSiteUrl" runat="server" AssociatedControlID="SiteUrl" Translate="<%$ Resources: EPiServer, admin.siteinformationlist.siteurl%>" />

                    <asp:TextBox runat="server" ID="SiteUrl" />
                    <asp:RequiredFieldValidator ID="SiteUrlValidator" runat="server" EnableClientScript="true" ControlToValidate="SiteUrl" Text="*" Display="Dynamic" />
                    <asp:CustomValidator OnServerValidate="ValidateSiteUrl" runat="server" Text="*" ErrorMessage="<%$ Resources: EPiServer, admin.siteinformationedit.invalidsiteurl%>"></asp:CustomValidator>
                    <asp:CustomValidator ID="ValidateSiteUrlVDirControl" OnServerValidate="ValidateSiteUrlVDir" Text="*" runat="server" ErrorMessage="<%$ Resources: EPiServer, admin.siteinformationedit.invalidsiteurlpath%>"></asp:CustomValidator>
                    <asp:CustomValidator ID="ValidateSiteUrlUniqueControl" OnServerValidate="ValidateSiteUrlUnique" Text="*" runat="server" ErrorMessage="<%$ Resources: EPiServer, admin.siteinformationedit.siteurlnotunique%>"></asp:CustomValidator>
                </div>
                <div>
                    <asp:Label runat="server" AssociatedControlID="StartPage" Translate="<%$ Resources: EPiServer, admin.siteinformationedit.startpage%>" />
                    <EPiServer:InputPageReference Style="display: inline;" ID="StartPage" DisableCurrentPageOption="true" AutoPostBack="false" runat="server"/>
                    <asp:RequiredFieldValidator ID="StartPageValidator" runat="server" EnableClientScript="false" ControlToValidate="StartPage" Text="*" Display="Dynamic" />
                    <asp:CustomValidator OnServerValidate="ValidateStartPage" runat="server"  Text="*" ErrorMessage="<%$ Resources: EPiServer, admin.siteinformationedit.nestedstartpages%>"></asp:CustomValidator>
                </div>
                <div class="epi-indent epi-size10">
                    <asp:CheckBox ID="EnsureSiteFolder" runat="server"/>
                    <asp:Label runat="server" AssociatedControlID="EnsureSiteFolder" Translate="<%$ Resources: EPiServer, admin.siteinformationedit.createsiteassets%>" />
                </div>
            </div>
        </fieldset>

        <fieldset>
            <legend><EPiServer:Translate Text="<%$ Resources: EPiServer, admin.siteinformationedit.hostgroup %>" runat="server" ID="Translate5"/></legend>
            <asp:GridView ID="AddUrlView" runat="server" AutoGenerateColumns="false" 
                        OnRowCancelingEdit="AddUrlView_CancelEdit" OnRowDeleting="AddUrlView_DeleteRow"
                        OnRowUpdating="AddUrlView_UpdateRow" OnRowEditing="AddUrlView_RowEdit" OnRowDataBound="AddUrlView_DataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="<%$ Resources: EPiServer, admin.siteinformationedit.urlheading %>"
                                ItemStyle-Wrap="false" ItemStyle-Width="260">
                                <ItemTemplate>
                                    <%# HttpUtility.HtmlEncode(((HostDefinition)Container.DataItem).Name) %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="SiteUrlTxt" MaxLength="100" Text='<%# ((HostDefinition)Container.DataItem).Name %>'
                                        runat="server" />
                                    <asp:RequiredFieldValidator ID="ValidatorSiteUrlTxt" ControlToValidate="SiteUrlTxt"
                                        Text="*" ErrorMessage="<%$ Resources: EPiServer, admin.siteinformationedit.missinghostname %>"
                                        EnableClientScript="true" Display="Dynamic" runat="server" />
                                    <EPiServerScript:ScriptSetFocusEvent ID="ScriptSetFocusEvent1" EventType="Load" EventTargetID="<%# Page.ClientID %>"
                                        FocusControlID="SiteUrlTxt" SetFocus="true" SetSelect="true" runat="server" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$ Resources: EPiServer, admin.siteinformationedit.cultureheading %>"
                                ItemStyle-Wrap="false" ItemStyle-Width="260">
                                <ItemTemplate>
                                    <%# ((HostDefinition)Container.DataItem).Language %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="LanguageDropDown" runat="server" OnDataBinding="LanguageDropDown_DataBinding"
                                        SelectedValue="<%# ((HostDefinition)Container.DataItem).Language%>" Width="200">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$ Resources: EPiServer, button.edit %>" ItemStyle-Wrap="false"
                                ItemStyle-HorizontalAlign="Left" ItemStyle-Width="75">
                                <ItemTemplate>
                                    <EPiServerUI:ToolButton DisablePageLeaveCheck="true" ID="ToolButton1" CommandName="Edit"
                                        SkinID="Edit" CausesValidation="false" ToolTip="<%$ Resources: EPiServer, button.edit %>"
                                        Enabled="<%# ButtonEnabled( Container.DataItemIndex )%>" runat="server" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <EPiServerUI:ToolButton DisablePageLeaveCheck="true" ID="ToolButton2" CommandName="Update"
                                        SkinID="Save" CausesValidation="true" ToolTip="<%$ Resources: EPiServer, button.save %>"
                                        Enabled="<%# ButtonEnabled( Container.DataItemIndex )%>" runat="server" /><EPiServerUI:ToolButton DisablePageLeaveCheck="true" ID="ToolButton3" CommandName="Cancel"
                                        SkinID="Cancel" CausesValidation="false" ToolTip="<%$ Resources: EPiServer, button.cancel %>"
                                        Enabled="<%# ButtonEnabled( Container.DataItemIndex )%>" runat="server" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$ Resources: EPiServer, button.delete %>" ItemStyle-HorizontalAlign="Left"
                             ItemStyle-Width="50">
                                <ItemTemplate>
                                    <EPiServerUI:ToolButton EnableClientConfirm="true" ID="deleteTool" CommandName="Delete" SkinID="Delete"
                                        ToolTip="<%$ Resources: EPiServer, button.delete %>" Enabled='<%#ButtonEnabled( Container.DataItemIndex )%>'
                                        runat="server" ConfirmMessage="<%$ Resources: EPiServer, admin.siteinformationedit.confirmdeletehostname %>" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
            </asp:GridView>
            <div class="epi-buttonContainer-small">
                <EPiServerUI:ToolButton ID="AddButton" SkinID="Add" OnClick="AddUrlButton_Click"
                    Text="<%$ Resources: EPiServer, button.add %>" ToolTip="<%$ Resources: EPiServer, button.add %>"
                    runat="server" DisablePageLeaveCheck="true" />
            </div>
        </fieldset>

        <div class="epi-buttonContainer">
        <EPiServerUI:ToolButton ID="DeleteButton" runat="server" Enabled="<%#!IsNew%>" Text="<%$ Resources: EPiServer, admin.siteinformationedit.deletesite %>"
            ToolTip="<%$ Resources: EPiServer, admin.siteinformationedit.deletesite %>" SkinID="Delete" CausesValidation="False"
            OnClick="DeleteButton_Click" ConfirmMessage="<%$ Resources: EPiServer, admin.siteinformationedit.confirmdeletesite %>" EnableClientConfirm="true" />
        <EPiServerUI:ToolButton DisablePageLeaveCheck="true" ID="ApplyButton"
            runat="server" Text="<%$ Resources: EPiServer, button.save %>" ToolTip="<%$ Resources: EPiServer, button.save %>"
            SkinID="Save" OnClick="ApplyButton_Click" />
        <EPiServerUI:ToolButton CausesValidation="false" DisablePageLeaveCheck="true" ID="CancelButton" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>"
            ToolTip="<%$ Resources: EPiServer, button.cancel %>" SkinID="Cancel" OnClick="CancelButton_Click" />
    </div>
  </asp:Panel>
</asp:Panel>
</asp:Content>
