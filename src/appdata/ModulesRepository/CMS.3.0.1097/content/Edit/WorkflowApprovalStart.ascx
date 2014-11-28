<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowApprovalStart.ascx.cs" Inherits="EPiServer.WorkflowFoundation.Workflows.WorkflowApprovalStart" %>
<%@ Import Namespace="EPiServer.WorkflowFoundation.Workflows" %>
<%@ Register TagPrefix="Workflow" TagName="History" Src="WorkflowHistoryList.ascx" %>
 <%@ Register TagPrefix="Workflow" TagName="VersionControl" Src="WorkflowApprovalVersionControl.ascx" %>
<script type='text/javascript'>
		//<![CDATA[
		function OpenUserBrowser(type)
		{
		    //opens up dialog to select users or groups, SetApprovers is called with result
		    OpenDialogUserAndGroupBrowser( type, '', '<%=EditPath%>', true, SetApprovers);
		}
		
		//picks up selected users/groups and post them to server to update user list
		function SetApprovers( returnValueArray )
	    {
	        var i;
	        var userString ='';
			var typeString ='';
			if (returnValueArray) {
			    for (var i = 0; i < returnValueArray.length; i++) {
			        var userItem = returnValueArray[i].split('|||');
			        if (i == returnValueArray.length - 1) {
			            userString = userString + userItem[0];
			            typeString = typeString + userItem[1];
			        }
			        else {
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
<Workflow:VersionControl Id="VersionControl" runat="server" />

 <h2><EPiServer:Translate runat="server" text="/workflows/approval/approvaltask/pageheading" />&nbsp;<%=Server.HtmlEncode(PageName)%></h2>
 <asp:CustomValidator runat="server" EnableClientScript="false" ID="CustomValidator" OnServerValidate="CustomValidation" Text="*"/>
 <asp:CustomValidator runat="server" ID="ApproversValidator"
            EnableClientScript="false" OnServerValidate="ApproversValidation" Text="*" />
 <asp:CustomValidator runat="server" ID="ApproversVisibleValidator" 
            EnableClientScript="false" OnServerValidate="ApproversVisibleValidation" Text="*" />
 <asp:CustomValidator runat="server" ID="NumberOfApproversValidation" ControlToValidate="NumberOfApprovers"
            EnableClientScript="false" OnServerValidate="NumberValidation" Text="*" />

<asp:PlaceHolder runat="server" ID="SelectApprovers" Visible="<%#SelectApproversVisible%>">
<div style="width:95%;">
    <span class="EP-systemInfo"><EPiServer:Translate runat="server" text="/workflows/approval/startparameters/selectapprovers" /></span>
    <div class="epi-buttonDefault">
        <EPiServerUI:ToolButton id="UserGroupAddButton" runat="server" OnClientClick="OpenUserBrowser(1);" GeneratesPostBack="False" Text="<%$ Resources: EPiServer, button.addusergroupsid %>" ToolTip="<%$ Resources: EPiServer, button.addusergroupsid %>" SkinId="AddUserGroup" />
    </div>
        
    <asp:HiddenField runat="server" ID="Users" />
    <asp:HiddenField runat="server" ID="Types" />
    
    <asp:Panel runat="server" Visible="false" ID="UsersArea">
        <!-- users/groups table -->
         <asp:DataGrid ID="Grid" runat="server" 
                AutoGenerateColumns="false"
                OnItemCommand="HandleButtons"
                OnDeleteCommand="RemoveUser"
                UseAccessibleHeader="true">
                <Columns>
                   <asp:TemplateColumn HeaderStyle-CssClass="epitableheading" HeaderText="<%$ Resources: EPiServer, workflows.approval.startparameters.name %>">
                       <ItemTemplate>
                            <%#Server.HtmlEncode((string)DataBinder.Eval(Container.DataItem, "Approver")) %>
                       </ItemTemplate>
                       </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="<%$ Resources: EPiServer, workflows.approval.startparameters.isrole %>">
                    <ItemTemplate>
                        <%#Translate("/button/" + (((ApproverItem)Container.DataItem).IsRole ? "yes" : "no")) %>
                    </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="<%$ Resources: EPiServer, workflows.approval.startparameters.required %>">
                        <ItemTemplate>
                            <asp:CheckBox
                                runat="server" 
                                AutoPostBack="true" 
                                OnCheckedChanged="IsRequiredChanged" 
                                Checked="<%#((EPiServer.WorkflowFoundation.Workflows.ApproverItem)Container.DataItem).IsRequired%>"/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
        </asp:DataGrid>
    </asp:Panel>
</div>
 
<div style="width:95%;" class="epi-formArea">
    <div class="epi-size30">
        <div>
           <asp:CheckBox runat="server" Checked="true" ID="ApproversVisible" Visible="<%#DefinitionMode%>" />
           <asp:Label runat="server" AssociatedControlID="ApproversVisible" Text="<%$ Resources: EPiServer, workflows.approval.startparameters.approversvisible %>" Visible="<%#DefinitionMode%>" /> 
        </div>
        <div>
            <asp:CheckBox runat="server" Checked="true" ID="NumbersVisible" Visible="<%#DefinitionMode%>" />
            <asp:Label AssociatedControlID="NumbersVisible" runat="server" Text="<%$ Resources: EPiServer, workflows.approval.startparameters.numbersvisible %>" Visible="<%#DefinitionMode%>" />
        </div>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="SelectNumbers" Visible="<%#SelectNumbersVisible%>">
    <div class="epi-size25">
        <div>
            <asp:Label AssociatedControlID="NumberOfApprovers" runat="server" Translate="/workflows/approval/startparameters/numberofapprovers" />
            <asp:TextBox id="NumberOfApprovers" runat="server" Width="25" MaxLength="9" />
        </div>
</asp:PlaceHolder>
        <div><br />
            <asp:Label AssociatedControlID="Description" runat="server" Translate="/workflows/approval/startparameters/description" />
            <asp:TextBox runat="server" ID="Description" TextMode="MultiLine" Rows="5" Width="100%" Columns="19" />
        </div>
    </div>
</div>       
<asp:Panel runat="server" ID="HistoryPanel" Visible="<%#ShowHistory%>">
    <!--ToggleDescription method is defined in WorkflowHistoryList.ascx-->
    <a href="javascript:void(0);" onclick="ToggleDescription('workflowhistory')";>
                    <EPiServer:Translate runat="server" text="/workflow/edit/workflowhistory/history" /></a>
    <div id="workflowhistory" style="display:none;">
        <Workflow:History id="History" runat="server" />
    </div>
</asp:Panel>