<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowTaskList.ascx.cs" Inherits="EPiServer.WorkflowFoundation.UI.WorkflowTaskList" %>

    <script type='text/javascript'>
	//<![CDATA[
	
	function ToggleDescription(id)
	{
	    var div = document.getElementById(id);
	    if (div.style.display == 'block')
	    {
	        div.style.display = 'none';
	    }
	    else
	    {
	        div.style.display = 'block';
	    }
	}

	
	//]]>
	</script>

<asp:Repeater ID="TaskList" runat="server">
    <ItemTemplate>
    <div style="padding-bottom:0.8em;">
        <EPiServer:Translate runat="server" text="/workflow/edit/workflowtask/subject" />&nbsp;&nbsp;
        <%# Server.HtmlEncode(CurrentTask.Subject) %>
        <br />&nbsp;<a href="javascript:void(0);" onclick="ToggleDescription('<%#Container.UniqueID%>_<%#Container.ItemIndex%>')";>
            <EPiServer:Translate runat="server" text="/workflow/edit/workflowhistory/details" /></a>
        <div class="EP-systemInfo" id="<%#Container.UniqueID%>_<%#Container.ItemIndex%>" style="display:none;">
            &nbsp;<EPiServer:Translate runat="server" text="/workflow/edit/workflowtask/created" />:&nbsp;
            <%# CurrentTask.Created.ToString()%>
            &nbsp;&nbsp;&nbsp;<EPiServer:Translate runat="server" text="/workflow/edit/workflowtask/duedate" />:&nbsp;
            <%# GetDueDate(CurrentTask)%>
            <br />
            &nbsp;<EPiServer:Translate runat="server" text="/workflow/edit/workflowtask/assignedto" />:&nbsp;
            <%# Server.HtmlEncode(CurrentTask.AssignedTo) %><br />
            &nbsp;<EPiServer:Translate runat="server" text="/workflow/edit/workflowtask/description" />:&nbsp;
            &nbsp;<%# Server.HtmlEncode(CurrentTask.Description) %>
        </div>
     </div>
    </ItemTemplate>
</asp:Repeater>
