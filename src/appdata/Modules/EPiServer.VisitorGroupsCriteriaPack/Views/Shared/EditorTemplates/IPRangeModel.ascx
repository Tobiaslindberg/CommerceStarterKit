<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EPiServer.VisitorGroupsCriteriaPack.IPRangeModel>" %>
<%@ Assembly Name="EPiServer.VisitorGroupsCriteriaPack" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups.Criteria" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups" %>
 
<div id='IPRangeModel'>
    <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
            <%= Html.LabelFor(model => model.Condition, Html.Translate("/visitorgroupscriteriapack/iprangecriterion/condition"), new { @class = "episize200" })%>
        </span>
        <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.Condition)%>
        </span>
    </div>
       <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
            <%= Html.LabelFor(model => model.IP, Html.Translate("/visitorgroupscriteriapack/iprangecriterion/ip"), new { @class = "episize200" })%>
        </span>
        <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.IP)%>
        </span>
    </div>
</div>