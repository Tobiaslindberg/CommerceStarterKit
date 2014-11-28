<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DIBSPayment.aspx.cs" Inherits="OxxCommerceStarterKit.Core.PaymentProviders.DIBS.DIBSPayment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="paymentForm" target="_top" method="post" runat="server" >
        <input type="hidden" name="paymentprovider" value="<%= DIBSSystemName %>" />
        <input type="hidden" name="merchant" value="<%=MerchantID%>" />
        <input type="hidden" name="amount" value="<%=Amount%>" />
        <input type="hidden" name="currency" value="<%=Currency.CurrencyCode%>" />
        <input type="hidden" name="orderid" value="<%=OrderID%>" />
        <input type="hidden" name="uniqueoid" value="<%=OrderID%>" />
        <input type="hidden" name="accepturl" value="<%=CallbackUrl%>" />
        <input type="hidden" name="cancelurl" id="cancelurl" runat="server" clientidmode="Static"/>
        <input type="hidden" name="test" value="foo" />
        <input type="hidden" name="HTTP_COOKIE" value="<%=Request.ServerVariables["HTTP_COOKIE"]%>" />
        <input type="hidden" name="lang" value="<%=Language %>" />
        <input type="hidden" name="md5key" value="<%=MD5Key%>" />
        <input type="hidden" name="voucher" value="yes" />
    </form>
</body>
</html>
