<%@ Control Language="c#" Codebehind="EditTask.ascx.cs" AutoEventWireup="False" Inherits="EPiServer.UI.Util.PlugIns.EditTask" %>
<%@ Register TagPrefix="EPiServer" Namespace="EPiServer.Web.WebControls" Assembly="EPiServer" %>
<asp:ValidationSummary ID="errorsummary" runat="server" CssClass="EP-validationSummary" />
<table>
    <tr>
        <td>
            <EPiServer:Translate Text="/personalization/task/subject" runat="server" />
            <br />
            <asp:TextBox ID="subject" runat="server" />
            <asp:RequiredFieldValidator ID="RequiredCheck" ErrorMessage="<%$ Resources: EPiServer, validation.formrequired %>" Display="Dynamic"
                ControlToValidate="subject" Text="*" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <EPiServer:Translate Text="/personalization/task/duedate" runat="server" ID="Translate1" />
            <br />
            <EPiServer:InputDate ID="duedate" runat="server" />
            <asp:CustomValidator ID="DateValidator" runat="server" Text="*" OnServerValidate="ValidateDueDate" />
        </td>
    </tr>
    <tr>
        <td>
            <EPiServer:Translate Text="/personalization/task/assignto" runat="server" ID="Translate2" />
            <br />
            <asp:DropDownList ID="assignedTo" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <EPiServer:Translate Text="/personalization/task/description" runat="server" />
            <br />
            <asp:TextBox ID="description" TextMode="MultiLine" runat="server" /></td>
    </tr>
    <tr>
        <td colspan="2">
            &nbsp;</td>
    </tr>
    <tr>
        <td>
            <asp:Button Translate="/button/save" OnClick="SaveTask_Click" ID="SaveTask" runat="server" />
            <asp:Button Translate="/button/delete" OnClick="DeleteTask_Click" ID="DeleteTask" runat="server" />
        </td>
    </tr>
</table>
