using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace Docy.Site
{
	public static class HtmlExtensions
	{
		public static MvcHtmlString TruncatedActionLink(this HtmlHelper helper,string linkText,string actionName,object routeValues)
		{
			return helper.TruncatedActionLink(linkText, actionName, routeValues, new { title = linkText });
		}

		public static MvcHtmlString TruncatedActionLink(this HtmlHelper helper, string linkText, string actionName, object routeValues,object htmlAttributes)
		{
			if (linkText.Length > MvcApplication.TruncateMaxChars)
			{
				linkText = linkText.Substring(0, MvcApplication.TruncateMaxChars) + "...";
			}

			return helper.ActionLink(linkText, actionName, routeValues, htmlAttributes);
		}

		public static MvcHtmlString IndentHierachy(this HtmlHelper helper, int amount)
		{
			return MvcHtmlString.Create("margin-left:" + (amount * 10) + "px");
		}

		public static MvcHtmlString TypeLink(this HtmlHelper helper, string fullname)
		{
			if (fullname.IndexOf(".") == -1)
			{
				fullname = "System." +fullname;
			}

			// Change this to checking if the namespace is in the Application.Namespaces
			if (fullname.StartsWith("System.") || fullname.StartsWith("Microsoft."))
			{
				string query = fullname;

				// Replace types with a T
				if (fullname.IndexOf("<") != -1)
				{
					// This only matches one generic argument, however that still works with IDictionary<TKey,TValue> + google
					Regex regex = new Regex(@"(.*)\<([a-zA-Z0-9\.]*)\>");
					query = regex.Replace(fullname, "$1<T>");
				}

				return MvcHtmlString.Create("<a href=\"http://www.google.com/search?q=" + query + "&btnI=Im+Feeling+Lucky\">" + helper.Encode(fullname) + "</a>");
			}
			else
			{
				return helper.ActionLink(fullname, "Type", new { id = fullname });
			}
		}
	}
}