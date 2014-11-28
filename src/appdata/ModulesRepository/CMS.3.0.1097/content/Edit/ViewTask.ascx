<%@ Control language="c#" Codebehind="ViewTask.ascx.cs" AutoEventWireup="False" Inherits="EPiServer.UI.Util.PlugIns.ViewTask" %>
<%@ Register TagPrefix="EPiServer" Namespace="EPiServer.Web.WebControls" Assembly="EPiServer" %>
	<asp:Panel runat="server" CssClass="EP-systemMessage" ID="TaskSavedPanel">
	    <asp:Literal runat="server" Text="<%$ Resources: EPiServer, personalization.task.tasksaved %>" />
	</asp:Panel>
	<table cellpadding="2" cellspacing="2" width="100%">
		<tr>
			<td>
				<b><EPiServer:translate text="/personalization/task/subject" runat="server" ID="Translate1"/></b>
				<br />
				<%# HttpUtility.HtmlEncode(CurrentTask.Subject)%></td>
		</tr>
		<tr>
			<td>
				<b><EPiServer:translate text="/personalization/task/duedate" runat="server" ID="Translate2" NAME="Translate1"/></b>
				<br />
				<%#CurrentTask.DueDate==DateTime.MaxValue ? String.Empty : CurrentTask.DueDate.ToString()%></td>
		</tr>
		<tr>
			<td>
				<b><EPiServer:translate text="/personalization/task/description" runat="server" ID="Translate4"/></b>
				<br />
				<%# HttpUtility.HtmlEncode(CurrentTask.Description) %></td>
		</tr>
		<tr>
			<td>
				<b><EPiServer:translate text="/personalization/task/status" runat="server" ID="Translate3"/></b>
				<br>
				<asp:DropDownList ID="statusList" Runat="server" /></td>
		</tr>
				<tr>
			<td colspan="2">&nbsp;</td>
		</tr>
		<tr>
			<td>
				<asp:Button Translate="/button/save" Runat="server" OnClick="SaveTask" ID="saveTaskButton"/>
				<asp:Button Translate="/button/settings" Runat="server" OnClick="EditSettings" ID="settingsButton"/>
			</td>
		</tr>
		<tr>
			<td><hr /></td>
		</tr>
		<tr>
			<td valign="top">
			<b><EPiServer:translate text="/personalization/task/comments" runat="server" ID="Translate5"/></b>
			<br />
				<asp:TextBox ID="comments" TextMode="MultiLine" Runat="server"/>
				<asp:Button OnClick="AddActivity" Translate="/button/add" Runat="server" ID="Button1"/>
			</td>
		</tr>
		<asp:Repeater ID="commentHistory" Runat="server">
		<ItemTemplate>
			<tr>
			<td>
				<%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem,"Text").ToString())%><br>
				<i><%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem,"Name").ToString())%> <%#DataBinder.Eval(Container.DataItem,"Created")%></i>
			</td>
		</tr>
		</ItemTemplate>
		</asp:Repeater>
	</table>