<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowTaskControl.ascx.cs" Inherits="EPiServer.WorkflowFoundation.UI.WorkflowTaskControl" %>
<%@ Register TagPrefix="Workflow" Namespace="EPiServer.WorkflowFoundation.WebControls" Assembly="EPiServer.WorkflowFoundation"  %>
<script type='text/javascript'>
	//<![CDATA[
	
	function OpenImage(id)
	{
	    window.showModalDialog('<%=EPiServer.UriSupport.ResolveUrlFromUIBySettings("edit/WorkflowImage.aspx")%>?InstanceId=' + id, null, 'dialogWidth:800px;dialogHeight:1000px;help=no;resizable:yes;scroll:yes;status:yes');
	    return false;
	}
	
	 function GetLoadedUrl()
    {
        if (window.top.latestPageVersion)
        {
            //alert(window.top.latestPageVersion);
            document.getElementById("<%= PageLink.ClientID %>").value = window.top.latestPageVersion;
        }
    }
	
	function onCommand(command)
	{
	    if (command.command == "pageversionchanged")
	    {
	        //alert(command.data);
            document.getElementById("<%= PageLink.ClientID %>").value = command.data;
	    }
	}
	
	//]]>
</script>
<asp:HiddenField ID="PageLink" runat="server" />
<asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="EP-validationSummary" ForeColor="Black" />
<br />
<asp:PlaceHolder ID="plcCtrl" runat="server"></asp:PlaceHolder>
<br style="clear: both;" />

<div>
<EPiServerUI:ToolButton 
    ID="Invoke"
    OnClick="InvokeEvent" 
    Text="<%$ Resources: EPiServer, workflow.edit.taskcontrol.invoke %>"  
    ToolTip ="<%$ Resources: EPiServer, workflow.edit.taskcontrol.invoke %>"  
    runat="server" 
    Visible="false"
    CssClassInnerButton="epitoolbutton"
    CssClassInnerImage="image"/>
<EPiServerUI:ToolButton ID="View" SkinID="ViewMode" Text="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.view %>" ToolTip="<%$ Resources: EPiServer, workflow.edit.workflowlist.details.view %>"  runat="server" />    
<EPiServerUI:ToolButton ID="Cancel" SkinID="Cancel" OnClick="Cancel_Click" Text="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>" ToolTip="<%$ Resources: EPiServer, admin.workflowdefinitionedit.cancel %>"  runat="server" />
</div>
