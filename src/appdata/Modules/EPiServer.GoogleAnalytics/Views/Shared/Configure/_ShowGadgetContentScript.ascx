<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AnalyticsViewModel>" %>
<%@ Import Namespace="EPiServer.GoogleAnalytics.Models" %>
<script type="text/javascript">
    $(document).ready(function () {
        if (!epi.googleAnalytics || typeof epi.googleAnalytics.showGadgetContent !== 'function') {
            return;
        }

        epi.googleAnalytics.showGadgetContent();
    });
</script>