<%@ Control Language="C#" AutoEventWireup="true" 
            CodeBehind="PluginStyles.ascx.cs" enableviewstate="false"
            Inherits="BVNetwork.EPiSendMail.Plugin.PluginStyles" %>

<%= Page.ClientResources("ShellWidgets")%>
<link rel="stylesheet" type="text/css" href="<%= BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir("/content/css/bootstrap.min.css") %>">
<link rel="stylesheet" type="text/css" href="<%= BVNetwork.EPiSendMail.Configuration.NewsLetterConfiguration.GetModuleBaseDir("/content/css/newsletterstyle.css") %>">
