<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Assembly Name="EPiServer.GoogleAnalytics" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Configuration" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<%@ Import Namespace="System.Linq" %>
<%
    string selectedSegment = Model.Statistics.SelectedSegment;
    bool isCustomSegment = selectedSegment == "custom";
    bool isAdvancedSegment = selectedSegment == "advanced";
%>
<div class="epi-paddingHorizontal-small epi-formArea">
<form id="changeFilterBy" class="epi-gadgetform" action="#" >
<% using (Html.GaTranslatePrefix("/episerver/googleanalytics/dashboard/configure/filterby/"))
   { %>
<p class="epi-marginVertical-small"><%= Html.GaTranslate("header")%></p>
<fieldset>
    <legend><%= Html.GaTranslate("legend")%></legend>
    <div class="epi-size10 epi-paddingVertical-small">
        <select name="Statistics.SelectedSegment" class="SegmentFilter">
            <optgroup label="<%= Html.GaTranslate("defaultsegments") %>">
                <% foreach (var segment in Model.DefaultSegments) { %>
                <%= Html.GaOption(segment.Name, segment.Id, Model.Statistics.SelectedSegment)%>
                <% } %>
            </optgroup>
            <optgroup label="<%= Html.GaTranslate("advancedsegments") %>">
                <% foreach (var segment in Model.AdvancedSegments) { %>
                <%= Html.GaOption(segment.Name, segment.Id, Model.Statistics.SelectedSegment)%>
                <% } %>
                <% if (!Model.AdvancedSegments.Any()) { %>
                <option value="advanced">-</option>
                <% } %>
            </optgroup>
            <optgroup label="<%= Html.GaTranslate("other") %>">
                <%= Html.GaOption(Html.GaTranslate("custom") + "...", "custom", Model.Statistics.SelectedSegment)%>
            </optgroup>
        </select>
    </div>
</fieldset>

<fieldset class="custom-segment" style="<%= isCustomSegment ? "" : "display:none" %>">
    <legend><%= Html.GaTranslate("customsegment/settings")%></legend>

    <div class="epi-size10 epi-marginVertical-small">
        <select name="Statistics.CustomSegmentDimension">
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/author"), Dimensions.CustomVarValue(CustomVariables.AuthorVariable), Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/referrer"), "ga:referralPath", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/keyword"), "ga:keyword", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/adgroup"), "ga:adGroup", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/campaign"), "ga:campaign", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/continent"), "ga:continent", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/country"), "ga:country", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/city"), "ga:city", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/landingpage"), "ga:landingPagePath", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/exitpage"), "ga:exitPagePath", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/productname"), "ga:productName", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/productsku"), "ga:productSku", Model.Statistics.CustomSegmentDimension)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/dimension/productcategory"), "ga:productCategory", Model.Statistics.CustomSegmentDimension)%>
        </select>
        <select name="Statistics.CustomSegmentOperator">
            <%= Html.GaOption(Html.GaTranslate("customsegment/operators/startswith"), "startswith", Model.Statistics.CustomSegmentOperator)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/operators/contains"), "contains", Model.Statistics.CustomSegmentOperator)%>
            <%= Html.GaOption(Html.GaTranslate("customsegment/operators/equals"), "equals", Model.Statistics.CustomSegmentOperator)%>
        </select>
        <%= Html.TextBoxFor(m => m.Statistics.CustomSegmentText, new { maxlength = 40, placeholder = Html.GaTranslate("customsegment/placeholder"), @class = "required" })%>
    </div>
</fieldset>

<fieldset class="advanced-segment" style="<%= isAdvancedSegment ? "" : "display:none" %>">
    <legend><%= Html.GaTranslate("googleanalyticscustomsegment/legend")%></legend>
    <p><%= Html.GaTranslate("googleanalyticscustomsegment/description")%></p>
</fieldset>

<div class="epi-buttonContainer-simple">
    <%= Html.AcceptButton()%>
    <%= Html.CancelButton()%>
</div>
<% } %>
</form>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        if (!epi.googleAnalytics || !epi.googleAnalytics.workingInDashboard || typeof epi.googleAnalytics.showGadgetContent !== 'function') {
            return;
        }

        epi.googleAnalytics.showGadgetContent();
    });
</script>