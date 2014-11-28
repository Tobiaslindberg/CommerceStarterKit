<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowImage.aspx.cs" Inherits="EPiServer.WorkflowFoundation.UI.WorkflowImage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <!-- Mimic Internet Explorer 7 -->
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <title><%= Translate("/workflow/edit/workflowimage/title") %></title>
    <base target="_self">
</head>
<body style="background-color:#E7E7EA">
    <form id="form1" runat="server">
    <div>
       <asp:Image runat="server" ID="WorkflowImageTag" AlternateText="Workflow Image" ImageUrl='<%# "WorkflowImageRenderer.ashx?" + QueryVariable + "=" + Identifier%>' />
    </div>
    </form>
</body>
</html>
