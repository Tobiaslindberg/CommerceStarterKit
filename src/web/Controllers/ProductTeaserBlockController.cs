/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using EPiServer.Web.Mvc;
using OxxCommerceStarterKit.Web.Models.Blocks;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class ProductTeaserBlockController : BlockController<ProductTeaserBlock>
	{

		public ProductTeaserBlockController()
        {

        }

		public override ActionResult Index(ProductTeaserBlock currentContent)
		{
			var model = new ProductTeaserBlockViewModel(currentContent);

			// My experiences:  If there is a View with the same name as the Model, then that view is chosen before the controller, 
			// unless an override exists in the database (admin->contenttype->blocktypes->productteaserblock->settings)   

			// The tag is set in the ViewData in the ContentAreaWithDefaultsRenderer.GetContentAreaItemCssClass() method
			model.Tag = ControllerContext.ParentActionViewContext.ViewData["Tag"] as string;

			return View(model);
		}


	}
}
