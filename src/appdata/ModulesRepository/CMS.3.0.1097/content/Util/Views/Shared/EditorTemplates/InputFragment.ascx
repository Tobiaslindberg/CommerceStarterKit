<%@ Import Namespace="EPiServer.XForms.Parsing" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InputFragment>" %>

<% string inputId = "Input_" + new Random(DateTime.Now.Millisecond).Next(Int32.MaxValue);  %>
<label title="<%: Model.Title %>" for="<%: inputId %>">
    <%: Html.DisplayFor(model => model.Label) %>
</label>
<%: Html.TextBox(Model.Reference, Server.HtmlDecode(Model.Value) ?? string.Empty, new { size = Model.Size, id=inputId})%>
<%: Html.ValidationMessage(Model.ValidationReference)%>

