/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using OxxCommerceStarterKit.Core.Objects;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class PersonalInformationViewModel : PageViewModel<PersonalInformationPage>
    {
        public PersonalSettingsForm PersonalSettingsForm { get; set; }

        public PersonalInformationViewModel(PersonalInformationPage currentPage) : base(currentPage)
        {
            PersonalSettingsForm = new PersonalSettingsForm();
        }
    }
}
