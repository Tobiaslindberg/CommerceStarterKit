/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace OxxCommerceStarterKit.Core.PaymentProviders.DIBS
{
    /// <summary>
    /// This is the result sent back from the DIBS Payment Window after
    /// a purchase has been completed successfully.
    /// </summary>
    public class DibsPaymentResult
    {
        public string AcceptReturnurl { get; set; }
        public string ActionCode { get; set; }
        public int Amount { get; set; }
        public string CardNumberMasked { get; set; }
        public string CardTypeName { get; set; }
        public string Currency { get; set; }
        public string Orderid { get; set; }
        public string Status { get; set; }
        public string Transaction { get; set; }
        public string ValidationErrors { get; set; }
        
        public override string ToString()
        {
            return string.Format("Status: {0}, Transaction: {1}, CardNumberMasked: {2}", Status, Transaction,
                CardNumberMasked);
        }
    }
}
