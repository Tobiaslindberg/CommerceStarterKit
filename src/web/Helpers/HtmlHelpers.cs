/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Business;
using OxxCommerceStarterKit.Web.Models.Files;
using SelectListItem = OxxCommerceStarterKit.Web.Models.ViewModels.SelectListItem;

namespace OxxCommerceStarterKit.Web.Helpers
{
	public static class HtmlHelpers
	{
		/// <summary>
		/// Returns an element for each child page of the rootLink using the itemTemplate.
		/// </summary>
		/// <param name="helper">The html helper in whose context the list should be created</param>
		/// <param name="rootLink">A reference to the root whose children should be listed</param>
		/// <param name="itemTemplate">A template for each page which will be used to produce the return value. Can be either a delegate or a Razor helper.</param>
		/// <param name="includeRoot">Wether an element for the root page should be returned</param>
		/// <param name="requireVisibleInMenu">Wether pages that do not have the "Display in navigation" checkbox checked should be excluded</param>
		/// <param name="requirePageTemplate">Wether page that do not have a template (i.e. container pages) should be excluded</param>
		/// <remarks>
		/// Filter by access rights and publication status.
		/// </remarks>
		public static IHtmlString MenuList(
			this HtmlHelper helper,
			ContentReference rootLink,
			Func<MenuItem, HelperResult> itemTemplate = null,
			int maxItems = 0,
			bool includeRoot = false,
			bool requireVisibleInMenu = true,
			bool requirePageTemplate = true)
		{
			itemTemplate = itemTemplate ?? GetDefaultItemTemplate(helper);
			var currentContentLink = helper.ViewContext.RequestContext.GetContentLink();
			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();


			if (currentContentLink == null)
			{
				currentContentLink = ContentReference.StartPage;
			}

			Func<IEnumerable<PageData>, IEnumerable<PageData>> filter =
				pages => pages.FilterForDisplay(requirePageTemplate, requireVisibleInMenu);

			var pagePath = contentLoader.GetAncestors(currentContentLink)
				.Reverse()
				.Select(x => x.ContentLink)
				.SkipWhile(x => !x.CompareToIgnoreWorkID(rootLink))
				.ToList();


			var menuItems = contentLoader.GetChildren<PageData>(rootLink)
				.FilterForDisplay(requirePageTemplate, requireVisibleInMenu)
				.Select(x => CreateMenuItem(x, currentContentLink, pagePath, contentLoader, filter))
				.ToList();

			if (includeRoot)
			{
				menuItems.Insert(0, CreateMenuItem(contentLoader.Get<PageData>(rootLink), currentContentLink, pagePath, contentLoader, filter));
			}

			var buffer = new StringBuilder();
			var writer = new StringWriter(buffer);

			if (maxItems > 0)
			{
				menuItems = menuItems.Take(maxItems).ToList();
			}
			foreach (var menuItem in menuItems)
			{
				itemTemplate(menuItem).WriteTo(writer);
			}

			return new MvcHtmlString(buffer.ToString());
		}

		private static MenuItem CreateMenuItem(PageData page, ContentReference currentContentLink, List<ContentReference> pagePath, IContentLoader contentLoader, Func<IEnumerable<PageData>, IEnumerable<PageData>> filter)
		{
			var menuItem = new MenuItem(page)
				{
					Selected = page.ContentLink.CompareToIgnoreWorkID(currentContentLink) ||
							   pagePath.Contains(page.ContentLink),
					HasChildren =
						new Lazy<bool>(() => filter(contentLoader.GetChildren<PageData>(page.ContentLink)).Any())
				};
			return menuItem;
		}

		private static Func<MenuItem, HelperResult> GetDefaultItemTemplate(HtmlHelper helper)
		{
			return x => new HelperResult(writer => writer.Write(helper.PageLink(x.Page)));
		}

		public class MenuItem
		{
			public MenuItem(PageData page)
			{
				Page = page;
			}
			public PageData Page { get; set; }
			public bool Selected { get; set; }
			public Lazy<bool> HasChildren { get; set; }
		}

		public static MvcHtmlString LabelForExtended<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
		{
			return LabelForExtended(html, expression, new RouteValueDictionary());
		}

		public static MvcHtmlString LabelForExtended<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
		{
			return LabelForExtended(html, expression, new RouteValueDictionary(htmlAttributes));
		}
		public static MvcHtmlString LabelForExtended<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
			if (String.IsNullOrEmpty(labelText))
			{
				return MvcHtmlString.Empty;
			}

			labelText += ":";

			if (metadata.IsRequired)
			{
				labelText = string.Concat(labelText, " *");
			}

			TagBuilder tag = new TagBuilder("label");
			tag.MergeAttributes(htmlAttributes);
			tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

			TagBuilder span = new TagBuilder("span");
			span.SetInnerText(labelText);

			// assign <span> to <label> inner html
			tag.InnerHtml = span.ToString(TagRenderMode.Normal);

			return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
		}

		public static string ContentName(this HtmlHelper html, ContentReference contentReference)
		{
			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
			IContent content;
			if (contentLoader.TryGet<IContent>(contentReference, out content))
			{
				return content.Name;
			}
			return string.Empty;
		}

		public static bool IsInEditMode(this HtmlHelper html)
		{
			return EPiServer.Editor.PageEditing.PageIsInEditMode;
		}

		public enum NewsletterColumns
		{
			one = 1,
			two = 2,
			three = 3,
			four = 4,
			five = 5,
			six = 6,
			seven = 7,
			eight = 8,
			nine = 9,
			ten = 10,
			eleven = 11,
			twelve = 12
		}

		public static string GetNewsletterColumns(this HtmlHelper html, ContentAreaItem content, out int columns)
		{
			var newsletterColumn = NewsletterColumns.twelve;

		    DisplayOption displayOption = content.LoadDisplayOption();
		    if (displayOption != null)
			{
                switch (displayOption.Tag)
				{
					case WebGlobal.ContentAreaTags.FullWidth:
						newsletterColumn = NewsletterColumns.twelve;
						break;
					case WebGlobal.ContentAreaTags.TwoThirdsWidth:
						newsletterColumn = NewsletterColumns.eight;
						break;
					case WebGlobal.ContentAreaTags.HalfWidth:
						newsletterColumn = NewsletterColumns.six;
						break;
					case WebGlobal.ContentAreaTags.OneThirdWidth:
						newsletterColumn = NewsletterColumns.four;
						break;
					case WebGlobal.ContentAreaTags.Slider:
						newsletterColumn = NewsletterColumns.twelve;
						break;
				}
			}
			columns = (int)newsletterColumn;
			return newsletterColumn.ToString();
		}
		public static string GetNewsletterImageWidth(this HtmlHelper html, ContentAreaItem content, bool halfWidth = false)
		{
            DisplayOption displayOption = content.LoadDisplayOption();
			if (!halfWidth)
			{
                if (displayOption != null)
				{

                    switch (displayOption.Tag)
					{
						case WebGlobal.ContentAreaTags.FullWidth:
							return ImageFile.NewsletterWidths.FULL;
						case WebGlobal.ContentAreaTags.TwoThirdsWidth:
							return ImageFile.NewsletterWidths.WIDE;
						case WebGlobal.ContentAreaTags.HalfWidth:
							return ImageFile.NewsletterWidths.HALF;
						case WebGlobal.ContentAreaTags.OneThirdWidth:
							return ImageFile.NewsletterWidths.NARROW;
					}
				}
				return ImageFile.NewsletterWidths.FULL;
			}
			else
			{
                if (displayOption != null)
				{
                    switch (displayOption.Tag)
					{
						case WebGlobal.ContentAreaTags.FullWidth:
							return ImageFile.NewsletterWidths.HALF;
						case WebGlobal.ContentAreaTags.TwoThirdsWidth:
							return ImageFile.NewsletterWidths.NARROW;
						case WebGlobal.ContentAreaTags.HalfWidth:
							return ImageFile.NewsletterWidths.NARROW;
						case WebGlobal.ContentAreaTags.OneThirdWidth:
							return ImageFile.NewsletterWidths.NARROW;
					}
				}
				return ImageFile.NewsletterWidths.HALF;
			}
		}
		public static string GetBlockImageWidth(this HtmlHelper html, string tag)
		{
			switch (tag)
			{
				case WebGlobal.ContentAreaTags.OneThirdWidth:
					return ImageFile.BoxSizes.NARROW;
				case WebGlobal.ContentAreaTags.HalfWidth:
					return ImageFile.BoxSizes.HALF;
				case WebGlobal.ContentAreaTags.TwoThirdsWidth:
					return ImageFile.BoxSizes.WIDE;
				case WebGlobal.ContentAreaTags.FullWidth:
					return ImageFile.BoxSizes.FULL;
			}
			return ImageFile.BoxSizes.FULL;
		}
		public static string GetImageWidth(this HtmlHelper html, string tag)
		{
			switch (tag)
			{
				case WebGlobal.ContentAreaTags.OneThirdWidth:
					return ImageFile.ImageWidths.NARROW;
				case WebGlobal.ContentAreaTags.HalfWidth:
					return ImageFile.ImageWidths.HALF;
				case WebGlobal.ContentAreaTags.TwoThirdsWidth:
					return ImageFile.ImageWidths.WIDE;
				case WebGlobal.ContentAreaTags.FullWidth:
					return ImageFile.ImageWidths.FULL;
			}
			return ImageFile.BoxSizes.FULL;
		}


		#region DropDownListFor custom SelectListItem
		/// <summary>Returns an HTML select element for each property in the object that is represented by the specified expression using the specified list items.</summary>
		/// <returns>An HTML select element for each property in the object that is represented by the expression.</returns>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="expression">An expression that identifies the object that contains the properties to display.</param>
		/// <param name="selectList">A collection of <see cref="T:System.Web.Mvc.SelectListItem" /> objects that are used to populate the drop-down list.</param>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TProperty">The type of the value.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
		public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
		{
			return htmlHelper.DropDownListFor<TModel, TProperty>(expression, selectList, null, (IDictionary<string, object>)null);
		}

		/// <summary>Returns an HTML select element for each property in the object that is represented by the specified expression using the specified list items and HTML attributes.</summary>
		/// <returns>An HTML select element for each property in the object that is represented by the expression.</returns>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="expression">An expression that identifies the object that contains the properties to display.</param>
		/// <param name="selectList">A collection of <see cref="T:System.Web.Mvc.SelectListItem" /> objects that are used to populate the drop-down list.</param>
		/// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TProperty">The type of the value.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
		public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
		{
			return htmlHelper.DropDownListFor<TModel, TProperty>(expression, selectList, null, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		/// <summary>Returns an HTML select element for each property in the object that is represented by the specified expression using the specified list items and HTML attributes.</summary>
		/// <returns>An HTML select element for each property in the object that is represented by the expression.</returns>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="expression">An expression that identifies the object that contains the properties to display.</param>
		/// <param name="selectList">A collection of <see cref="T:System.Web.Mvc.SelectListItem" /> objects that are used to populate the drop-down list.</param>
		/// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TProperty">The type of the value.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
		public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
		{
			return htmlHelper.DropDownListFor<TModel, TProperty>(expression, selectList, null, htmlAttributes);
		}

		/// <summary>Returns an HTML select element for each property in the object that is represented by the specified expression using the specified list items and option label.</summary>
		/// <returns>An HTML select element for each property in the object that is represented by the expression.</returns>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="expression">An expression that identifies the object that contains the properties to display.</param>
		/// <param name="selectList">A collection of <see cref="T:System.Web.Mvc.SelectListItem" /> objects that are used to populate the drop-down list.</param>
		/// <param name="optionLabel">The text for a default empty item. This parameter can be null.</param>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TProperty">The type of the value.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
		public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
		{
			return htmlHelper.DropDownListFor<TModel, TProperty>(expression, selectList, optionLabel, (IDictionary<string, object>)null);
		}

		/// <summary>Returns an HTML select element for each property in the object that is represented by the specified expression using the specified list items, option label, and HTML attributes.</summary>
		/// <returns>An HTML select element for each property in the object that is represented by the expression.</returns>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="expression">An expression that identifies the object that contains the properties to display.</param>
		/// <param name="selectList">A collection of <see cref="T:System.Web.Mvc.SelectListItem" /> objects that are used to populate the drop-down list.</param>
		/// <param name="optionLabel">The text for a default empty item. This parameter can be null.</param>
		/// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TProperty">The type of the value.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
		public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
		{
			return htmlHelper.DropDownListFor<TModel, TProperty>(expression, selectList, optionLabel, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		/// <summary>Returns an HTML select element for each property in the object that is represented by the specified expression using the specified list items, option label, and HTML attributes.</summary>
		/// <returns>An HTML select element for each property in the object that is represented by the expression.</returns>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="expression">An expression that identifies the object that contains the properties to display.</param>
		/// <param name="selectList">A collection of <see cref="T:System.Web.Mvc.SelectListItem" /> objects that are used to populate the drop-down list.</param>
		/// <param name="optionLabel">The text for a default empty item. This parameter can be null.</param>
		/// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TProperty">The type of the value.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
		public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			ModelMetadata modelMetadatum = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
			return DropDownListHelper(htmlHelper, modelMetadatum, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes);
		}

		private static MvcHtmlString DropDownListHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
		{
			return htmlHelper.SelectInternal(metadata, optionLabel, expression, selectList, false, htmlAttributes);
		}

		private static IEnumerable<SelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
		{
			object obj = null;
			if (htmlHelper.ViewData != null)
			{
				obj = htmlHelper.ViewData.Eval(name);
			}
			if (obj == null)
			{
				object[] objArray = new object[] { name, "IEnumerable<SelectListItem>" };
				throw new InvalidOperationException("No data");
			}
			IEnumerable<SelectListItem> selectListItems = obj as IEnumerable<SelectListItem>;
			if (selectListItems == null)
			{
				throw new InvalidOperationException("No data 2");
			}
			return selectListItems;
		}
		private static IEnumerable<SelectListItem> GetSelectListWithDefaultValue(IEnumerable<SelectListItem> selectList, object defaultValue, bool allowMultiple)
		{
			IEnumerable enumerable;
			if (!allowMultiple)
			{
				enumerable = new object[] { defaultValue };
			}
			else
			{
				enumerable = defaultValue as IEnumerable;
				if (enumerable == null || enumerable is string)
				{
					throw new InvalidOperationException("Not enumerable");
				}
			}
			IEnumerable<string> str =
				from object value in enumerable
				select Convert.ToString(value);
			IEnumerable<string> strs =
				from Enum value in enumerable.OfType<Enum>()
				select value.ToString("d");
			str = str.Concat<string>(strs);
			HashSet<string> strs1 = new HashSet<string>(str, StringComparer.OrdinalIgnoreCase);
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			foreach (SelectListItem selectListItem in selectList)
			{
				selectListItem.Selected = (selectListItem.Value != null ? strs1.Contains(selectListItem.Value) : strs1.Contains(selectListItem.Text));
				selectListItems.Add(selectListItem);
			}
			return selectListItems;
		}

		private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, ModelMetadata metadata, string optionLabel, string name, IEnumerable<SelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
		{
			ModelState modelState;
			string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			if (string.IsNullOrEmpty(fullHtmlFieldName))
			{
				throw new ArgumentException("Cannot be null", "name");
			}
			bool flag = false;
			if (selectList == null)
			{
				selectList = htmlHelper.GetSelectData(name);
				flag = true;
			}
			object model = (allowMultiple ? htmlHelper.GetModelStateValue(fullHtmlFieldName, typeof(string[])) : htmlHelper.GetModelStateValue(fullHtmlFieldName, typeof(string)));
			if (model == null && !string.IsNullOrEmpty(name))
			{
				if (!flag)
				{
					model = htmlHelper.ViewData.Eval(name);
				}
				else if (metadata != null)
				{
					model = metadata.Model;
				}
			}
			if (model != null)
			{
				selectList = GetSelectListWithDefaultValue(selectList, model, allowMultiple);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (optionLabel != null)
			{
				SelectListItem selectListItem = new SelectListItem()
				{
					Text = optionLabel,
					Value = string.Empty,
					Selected = false
				};
				stringBuilder.AppendLine(ListItemToOption(selectListItem));
			}
			foreach (SelectListItem selectListItem1 in selectList)
			{
				stringBuilder.AppendLine(ListItemToOption(selectListItem1));
			}
			TagBuilder tagBuilder = new TagBuilder("select")
			{
				InnerHtml = stringBuilder.ToString()
			};
			TagBuilder tagBuilder1 = tagBuilder;
			tagBuilder1.MergeAttributes<string, object>(htmlAttributes);
			tagBuilder1.MergeAttribute("name", fullHtmlFieldName, true);
			tagBuilder1.GenerateId(fullHtmlFieldName);
			if (allowMultiple)
			{
				tagBuilder1.MergeAttribute("multiple", "multiple");
			}
			if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
			{
				tagBuilder1.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			}
			tagBuilder1.MergeAttributes<string, object>(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));
			return new MvcHtmlString(tagBuilder1.ToString(TagRenderMode.Normal));
		}
		internal static string ListItemToOption(SelectListItem item)
		{
			TagBuilder tagBuilder = new TagBuilder("option")
			{
				InnerHtml = HttpUtility.HtmlEncode(item.Text)
			};
			TagBuilder value = tagBuilder;
			if (item.Value != null)
			{
				value.Attributes["value"] = item.Value;
			}
			if (item.Selected)
			{
				value.Attributes["selected"] = "selected";
			}
			if (item.Disabled)
			{
				value.Attributes["disabled"] = "disabled";
			}
			if (!string.IsNullOrEmpty(item.DataCode))
			{
				value.Attributes["data-code"] = item.DataCode;
			}
			return value.ToString(TagRenderMode.Normal);
		}
		internal static object GetModelStateValue(this HtmlHelper helper, string key, Type destinationType)
		{
			ModelState modelState;
			if (!helper.ViewData.ModelState.TryGetValue(key, out modelState) || modelState.Value == null)
			{
				return null;
			}
			return modelState.Value.ConvertTo(destinationType, null);
		}

		#endregion

	}
}
