<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowFeedbackStart.ascx.cs" Inherits="EPiServer.WorkflowFoundation.Workflows.WorkflowFeedbackStart" %>
<%@ Import Namespace="EPiServer.WorkflowFoundation.Workflows" %>
<script type='text/javascript'>
		//<![CDATA[
		function OpenUserBrowser(type, ctrl)
		{
		    if ( ctrl.id == '<%=OwnerGroupAddButton.ClientID%>')
	        {
	            OpenDialogUserAndGroupBrowser( type, '', '<%=EPiServer.UriSupport.ResolveUrlFromUIBySettings("edit")%>', false, SetOwner);
	        }
	        else if( ctrl.id == '<%=UserGroupAddButton.ClientID%>')
	        {
	            OpenDialogUserAndGroupBrowser( type, '', '<%=EPiServer.UriSupport.ResolveUrlFromUIBySettings("edit")%>', true, SetUsers);
	        }
		}
		function SetOwner( returnValueArray )
		{
			if (returnValueArray && returnValueArray.length > 0)
			{
			    document.getElementById('<%=Users.ClientID%>').value = returnValueArray[0].split('|||')[0];   
			}
			else
			{
			    document.getElementById('<%=Users.ClientID%>').value = '';
			}
			__doPostBack('UpdateOwner', '');
		}
		 
		function SetUsers( returnValueArray )
		{
			var i;
	        var userString ='';
			var typeString ='';
			if (returnValueArray)
			{
	            for(var i=0; i < returnValueArray.length; i++)
                {

	                var userItem = returnValueArray[i].split('|||');
	                if (i==returnValueArray.length-1)
		            {
		                userString = userString + userItem[0];
		                typeString = typeString + userItem[1];
		            }
		            else
		            {
		                userString = userString + userItem[0] + '|||';
		                typeString = typeString + userItem[1] + '|||';
		            }
	            }
	        }
	        document.getElementById('<%=Users.ClientID%>').value = userString;
	        document.getElementById('<%=Types.ClientID%>').value = typeString;
	        __doPostBack('UpdateUserList','');
		}
		//]]>
    </script>  
 
 
 <h2><EPiServer:Translate runat="server" text="/workflows/feedbackrequest/heading" /></h2>
<asp:CustomValidator runat="server" EnableClientScript="false" ID="CustomValidator" OnServerValidate="CustomValidation" Text="*"/>
<br />
<EPiServer:Translate runat="server" text="/workflows/feedbackrequest/description" />
<br />
<asp:TextBox runat="server" ID="Description" TextMode="MultiLine" Rows="5" Width="400" />
<table class="epistandardtable"> 
    <tr>
        <td>
            <EPiServer:Translate runat="server" text="/workflows/feedbackrequest/timetolive" />
        </td>
        <td>
            <asp:TextBox runat="server" ID="TimeToLive" Text="120" Width="30" />
            <asp:RequiredFieldValidator ControlToValidate="TimeToLive" Text="*" EnableClientScript="true" runat="server" ErrorMessage="<%$ Resources: EPiServer, validation.formrequired %>" />
            <asp:CompareValidator id="timeRequired" runat="server" ControlToValidate="TimeToLive"
                            ValueToCompare="0"
                            Type="Integer"
                            Operator="GreaterThan"
                            Text="*"
                            ErrorMessage="<%$ Resources: EPiServer, workflows.translate.positivenumber %>" Display="dynamic"/>
        </td>
    </tr>
    <tr>
        <td>
            <EPiServer:Translate runat="server" text="/workflows/translate/owner" />
        </td>
        <td>
            <asp:TextBox runat="server" ID="Owner" Enabled="false" Width="100"/>
            <EPiServerUI:ToolButton id="OwnerGroupAddButton" runat="server" OnClientClick="OpenUserBrowser(1, this);" GeneratesPostBack="False" Text="<%$ Resources: EPiServer, button.addusergroupsid %>" ToolTip="<%$ Resources: EPiServer, button.addusergroupsid %>" SkinID="AddUserGroup" />
        </td>
    </tr>
</table>
<div style="margin-top:2em;">
    <span class="EP-systemInfo"><EPiServer:Translate runat="server" text="/workflows/feedbackrequest/selectusers" /></span>
    <br />
    <br />
    <EPiServerUI:ToolButton id="UserGroupAddButton" runat="server" OnClientClick="OpenUserBrowser(1, this);" GeneratesPostBack="False" Text="<%$ Resources: EPiServer, button.addusergroupsid %>" ToolTip="<%$ Resources: EPiServer, button.addusergroupsid %>" SkinID="AddUserGroup" />
    <asp:HiddenField runat="server" ID="Users" />
    <asp:HiddenField runat="server" ID="Types" />
    <asp:Panel runat="server" ID="TranslatorsArea" Visible="false">
        <div style="padding-top:0.5em;">
             <asp:DataGrid ID="Grid" runat="server" 
                    AutoGenerateColumns="false"
                    OnDeleteCommand="RemoveUser" >
                    <Columns>
                       <asp:TemplateColumn HeaderStyle-CssClass="epitableheading" HeaderText="<%$ Resources: EPiServer, workflows.approval.startparameters.name %>">
                       <ItemTemplate>
                            <%#Server.HtmlEncode((string)DataBinder.Eval(Container.DataItem, "UserName")) %>
                       </ItemTemplate>
                       </asp:TemplateColumn>
                       <asp:TemplateColumn HeaderStyle-CssClass="epitableheading" HeaderText="<%$ Resources: EPiServer, workflows.approval.startparameters.isrole %>">
                       <ItemTemplate>
                            <%#Translate("/button/" + (((UserItem)Container.DataItem).IsRole ? "yes" : "no")) %>
                       </ItemTemplate>
                       </asp:TemplateColumn>
                    </Columns>
            </asp:DataGrid>
        </div>
    </asp:Panel>
</div>

