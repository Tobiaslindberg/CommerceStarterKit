/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;
using OxxCommerceStarterKit.Core.Attributes.Validation;

namespace OxxCommerceStarterKit.Core.Objects
{
	public class ChangePasswordForm
	{
        [ValidateRequired]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("/common/accountpages/old_password_label")]
        [Placeholder("/common/accountpages/placeholder/old_password")]
        public string OldPassword { get; set; }

        [ValidateRequired]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("/common/accountpages/new_password_label")]
        [Placeholder("/common/accountpages/placeholder/new_password")]
		public string NewPassword { get; set; }

        [ValidateRequired]
        [DataType(DataType.Password)]        
        [ValidateEqualTo("NewPassword")]
        [LocalizedDisplayName("/common/accountpages/repeat_password_label")]
        [Placeholder("/common/accountpages/placeholder/repeat_password")]
        public string RepeatPassword { get; set; }
	}
}
