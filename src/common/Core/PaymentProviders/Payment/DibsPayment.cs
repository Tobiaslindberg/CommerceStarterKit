/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Runtime.Serialization;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus.Configurator;

namespace OxxCommerceStarterKit.Core.PaymentProviders.Payment
{
    [Serializable]
    public class DibsPayment : Mediachase.Commerce.Orders.Payment
    {
        public static MetaClass DibsPaymentMetaClass
        {
            get
            {
                if (DibsPayment._MetaClass == null)
                    DibsPayment._MetaClass = MetaClass.Load(OrderContext.MetaDataContext, "DibsPayment");
                return DibsPayment._MetaClass;
            }
        }

        public DibsPayment()
            : base(DibsPayment.DibsPaymentMetaClass)
        {

        }

        public DibsPayment(MetaClass metaClass)
            : base(metaClass)
        {
            this.PaymentType = PaymentType.CreditCard;

        }

        public DibsPayment(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.PaymentType = PaymentType.Other;
        }

        private static MetaClass _MetaClass;



        public string CardNumberMasked
        {
            get { return base.GetString("CardNumberMasked"); }
            set { this["CardNumberMasked"] = value; }
        }

        public string CartTypeName
        {
            get { return base.GetString("CardTypeName"); }
            set { this["CardTypeName"] = value; }
        }

      

    }
}
