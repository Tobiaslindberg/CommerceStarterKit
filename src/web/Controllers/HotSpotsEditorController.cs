/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	[Authorize(Roles = "WebEditors, WebAdmins, Administrators")]
	public class HotSpotsEditorController : Controller
	{

		public ActionResult Index()
		{
			var culture = EPiServer.Globalization.UserInterfaceLanguage.Instance.DetermineEPiServerUserInterfaceCulture();

			var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();

			var model = new HotSpotsEditorViewModel();
			model.Language = culture.Name;
			model.DeleteHotSpotText = localizationService.GetStringByCulture("/hotspots/drop-to-delete", culture);
			model.LinkToProductText = localizationService.GetStringByCulture("/hotspots/link-to-product", culture);
			model.NewHotSpotText = localizationService.GetStringByCulture("/hotspots/new-hotspot", culture);
			

			return PartialView("~/Views/Widgets/HotSpotsEditor/Index.cshtml", model);
		}

	}
}
