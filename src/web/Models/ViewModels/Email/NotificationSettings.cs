/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.ViewModels.Email
{
    [ContentType(DisplayName = "Email Settings", 
        GUID = "05617b03-5ca4-43ee-b162-bfc90d7edf71", 
        Description = "Common email content and settings")]    
    [SiteImageUrl]
    public class NotificationSettings : PageData
    {
        
                [CultureSpecific]
                [Display(
                    Name = "Mail Header",
                    Description = "",
                    GroupName = "Mail Settings",
                    Order = 1)]
                public virtual XhtmlString MailHeader { get; set; }

                [CultureSpecific]
                [Display(
                    Name = "Mail Footer",
                    Description = "",
                    GroupName = "Mail Settings",
                    Order = 3)]
                public virtual XhtmlString MailFooter { get; set; }

                [Display(
                    Name = "Mail sender",
                    Description = "",
                    GroupName = "Mail Settings",
                    Order = 5)]
                public virtual string From { get; set; }
         
    }
}