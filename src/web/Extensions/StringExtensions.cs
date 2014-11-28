/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Text.RegularExpressions;

namespace OxxCommerceStarterKit.Web.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Remove Html tags
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string StripHtml(this string source)
		{
			string noHTML = Regex.Replace(source, @"<[^>]+>|&nbsp;", "").Trim();
			return Regex.Replace(noHTML, @"\s{2,}", " ");
		}

		/// <summary>
		/// Strips a text to a given length without splitting the last word.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="maxLength">Max length of the text</param>
		/// <returns>A shortened version of the given string</returns>
		/// <remarks>Will return empty string if input is null or empty</remarks>
		public static string StripPreviewText(this string source, int maxLength)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			if (source.Length <= maxLength)
			{
				return source;
			}
			source = source.Substring(0, maxLength);
			// The maximum number of characters to cut from the end of the string.
			var maxCharCut = (source.Length > 15 ? 15 : source.Length - 1);
			var previousWord = source.LastIndexOfAny(new char[] { ' ', '.', ',', '!', '?' }, source.Length - 1, maxCharCut);
			if (previousWord >= 0)
			{
				source = source.Substring(0, previousWord);
			}
			return source + " ...";
		}
	}
}
