<%@ Page Language="C#"  AutoEventWireup="true" CodeBehind="ContentChannelList.aspx.cs" Inherits="EPiServer.UI.Admin.ContentChannelList" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentRegion" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainRegion" runat="server">
    <div class="epi-buttonDefault">
        <EPiServerUI:ToolButton ID="AddButton" SkinID="Add" OnClick="NewChannel" Text="<%$ Resources: EPiServer, button.add %>" ToolTip="<%$ Resources: EPiServer, button.add %>" runat="server" />
    </div>
    <EPiServerScript:ScriptSettings runat="server" ConfirmMessage="<%$ Resources: EPiServer, admin.contentchannellist.confirmdelete %>" ID="deleteCommon" />
    <asp:GridView 
        runat="server"
        id="ContentChannels"
        AutoGenerateColumns="false"
        OnRowCommand="ContentChannels_RowCommand"
        UseAccessibleHeader="true">
        <Columns>
             <asp:TemplateField HeaderText="<%$ Resources: EPiServer, admin.contentchannellist.channelname %>" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <%# HttpUtility.HtmlEncode((string)Eval("ChannelId")) %>
                    </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center"  HeaderText="<%$ Resources: EPiServer, admin.contentchannellist.copy %>">
                 <ItemTemplate> 
                    <EPiServerUI:ToolButton CommandName="Copy" SkinID="Copy" CausesValidation="false" ToolTip="<%$ Resources: EPiServer, admin.contentchannellist.copy %>" CommandArgument='<%# Eval("ChannelId") %>' runat="server" />
                 </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="<%$ Resources: EPiServer, button.edit%>">
                <ItemTemplate> 
                    <EPiServerUI:ToolButton CommandName="Edit" SkinID="Edit" CausesValidation="false" ToolTip="<%$ Resources: EPiServer, button.edit %>" CommandArgument='<%# Eval("ChannelId") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center"  HeaderText="<%$ Resources: EPiServer, button.delete%>">
                <ItemTemplate> 
                    <EPiServerUI:ToolButton ID="deleteTool" CommandName="DeleteChannel" CommandArgument='<%# Eval("ChannelId") %>' SkinID="Delete" EnableClientConfirm="true" ToolTip="<%$ Resources: EPiServer, button.delete %>"  runat="server" /> 
                    <EPiServerScript:ScriptSettings runat="server" TargetControlID="deleteTool" CommonSettingsControlID="deleteCommon" Name='<%# Eval("ChannelId") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
