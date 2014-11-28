<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowHistoryList.ascx.cs" Inherits="EPiServer.WorkflowFoundation.UI.WorkflowHistoryList" %>

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

<asp:Repeater ID="HistoryList" runat="server">
    <ItemTemplate>
    <div style="padding-bottom:0.8em;">
        <%#((EPiServer.WorkflowFoundation.HistoryItem)Container.DataItem).Date.ToString()%>&nbsp;&nbsp;
        <%#((EPiServer.WorkflowFoundation.HistoryItem)Container.DataItem).Subject%>
        <br />&nbsp;<a href="javascript:void(0);" onclick="ToggleDescription('<%#Container.UniqueID%>_<%#Container.ItemIndex%>')";>
            <EPiServer:Translate runat="server" text="/workflow/edit/workflowhistory/details" /></a>
        <div class="EP-systemInfo" id="<%#Container.UniqueID%>_<%#Container.ItemIndex%>" style="display:none;">
            &nbsp;<EPiServer:Translate runat="server" text="/workflow/edit/workflowhistory/user" />:&nbsp;
            <%# Server.HtmlEncode(((EPiServer.WorkflowFoundation.HistoryItem)Container.DataItem).User) %><br />
            &nbsp;<%# Server.HtmlEncode(((EPiServer.WorkflowFoundation.HistoryItem)Container.DataItem).Description)%>
        </div>
     </div>
    </ItemTemplate>
</asp:Repeater>
