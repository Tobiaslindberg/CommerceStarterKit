<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EPiServer.VisitorGroupsCriteriaPack.OSBrowserModel>" %>
<%@ Assembly Name="EPiServer.VisitorGroupsCriteriaPack" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups.Criteria" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups" %>
 
<div id='OSBrowserModel'>
    <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
            <%= Html.LabelFor(model => model.OperatingSystem, Html.Translate("/visitorgroupscriteriapack/osbrowsercriterion/operatingsytem"), new { @class = "episize200" })%>
        </span>
        <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.OperatingSystem)%>
        </span>
    </div>
    <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
            <%= Html.LabelFor(model => model.Browser, Html.Translate("/visitorgroupscriteriapack/osbrowsercriterion/browser"), new { @class = "episize200" })%>
        </span>
        <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.BrowserMatchType)%>
        </span>
        <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.Browser)%>
        </span>
    </div>
</div>