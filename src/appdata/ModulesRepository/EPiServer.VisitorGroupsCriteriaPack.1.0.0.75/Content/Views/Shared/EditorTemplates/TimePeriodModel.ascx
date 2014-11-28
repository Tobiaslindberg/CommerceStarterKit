<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EPiServer.VisitorGroupsCriteriaPack.TimePeriodModel>" %>
<%@ Assembly Name="EPiServer.VisitorGroupsCriteriaPack" %>
<%@ Import Namespace="EPiServer.Personalization.VisitorGroups" %>
<%@ Import Namespace="EPiServer.Cms.Shell" %>

<div id='TimePeriodModel'>
	<div class="epi-critera-block">
		<%=Html.Translate("/visitorgroupscriteriapack/timeperiodcriterion/from")%>
	</div>
    <div class="epi-critera-block">
    	<span class="epi-criteria-inlineblock">
        	<%= Html.DojoEditorFor(p => p.StartDate, null, Html.Translate("/visitorgroupscriteriapack/timeperiodcriterion/date"), "")%>
        </span>
        <span class="epi-criteria-inlineblock">
        	<%= Html.DojoEditorFor(p => p.StartTime, null, Html.Translate("/visitorgroupscriteriapack/timeperiodcriterion/time"), "")%>
        </span>
    </div>
    
    <div class="epi-critera-block">
	    &nbsp;
    </div>

    <div class="epi-critera-block">
	    <%=Html.Translate("/visitorgroupscriteriapack/timeperiodcriterion/to")%>
    </div>
    <div class="epi-critera-block">
        <span class="epi-criteria-inlineblock">
        	<%= Html.DojoEditorFor(p => p.EndDate, null, Html.Translate("/visitorgroupscriteriapack/timeperiodcriterion/date"), "")%>
    	</span>
    	<span class="epi-criteria-inlineblock">
    	    <%= Html.DojoEditorFor(p => p.EndTime, null, Html.Translate("/visitorgroupscriteriapack/timeperiodcriterion/time"), "")%>
	    </span>
    </div>
</div>