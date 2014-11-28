<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EPiServer.VisitorGroupsCriteriaPack.SelectedLanguageModel>" %>
<%@ Assembly Name="EPiServer.VisitorGroupsCriteriaPack" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups.Criteria" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups" %>
 
<div id='SelectedLanguageModel'>
    <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
            <%= Html.LabelFor(model => model.Condition, Html.Translate("/visitorgroupscriteriapack/selectedlanguagecriterion/condition"), new { @class = "episize200" })%>
        </span>
        <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.Condition)%>
        </span>
    </div>
      <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
            <%= Html.LabelFor(model => model.SelectedLanguage, Html.Translate("/visitorgroupscriteriapack/selectedlanguagecriterion/selectedlangauge"), new { @class = "episize200" })%>
        </span>
        <span class="epi-criteria-inlineblock">
            <%= Html.DojoEditorFor(model => model.SelectedLanguage)%>
        </span>
    </div>
</div>