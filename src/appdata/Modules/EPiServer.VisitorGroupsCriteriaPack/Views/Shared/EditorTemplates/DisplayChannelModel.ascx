<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EPiServer.VisitorGroupsCriteriaPack.DisplayChannelModel>" %>
<%@ Assembly Name="EPiServer.VisitorGroupsCriteriaPack" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups.Criteria" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups" %>
 
<div id='DisplayChannelModel'>
    <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
            <%= Html.LabelFor(model => model.ChannelName, Html.Translate("/visitorgroupscriteriapack/displaychannelcriterion/channelname"), new { @class = "episize200" })%>
        </span>
        <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.ChannelName)%>
        </span>
    </div>
</div>