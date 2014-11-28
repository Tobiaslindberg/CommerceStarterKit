<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EPiServer.VisitorGroupsCriteriaPack.DownloadModel>" %>
<%@ Assembly Name="EPiServer.VisitorGroupsCriteriaPack" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups.Criteria" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups" %>
 
<div id='DownloadModel'>
    <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
            <%= Html.LabelFor(model => model.VirtualPath, Html.Translate("/visitorgroupscriteriapack/downloadcriterion/virtualpath"), new { @class = "episize200" })%>
        </span>
       <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.VirtualPath)%>
        </span>
    </div>
</div>