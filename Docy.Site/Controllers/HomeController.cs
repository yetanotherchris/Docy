using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Docy.Core;

namespace Docy.Site.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {			
			return View(MvcApplication.Namespaces);
        }

		public ActionResult Namespace(string id)
        {
			ViewData["Namespaces"] = MvcApplication.Namespaces;

			return View(MvcApplication.Namespaces.FirstOrDefault(n => n.Name == id));
        }

		public ActionResult Type(string id)
		{
			TypeBase result = MvcApplication.Namespaces.FindType(id);

			ViewData["Namespaces"] = MvcApplication.Namespaces;
			ViewData["Types"] = result.Namespace.AllTypes;
			
			return View(result);
		}

		public ActionResult Constructors(string id)
		{
			TypeBase result = MvcApplication.Namespaces.FindType(id);
			ViewData["Namespaces"] = MvcApplication.Namespaces;

			return View(result);
		}

		public ActionResult Methods(string id)
		{
			TypeBase result = MvcApplication.Namespaces.FindType(id);
			ViewData["Namespaces"] = MvcApplication.Namespaces;

			return View(result);
		}

		public ActionResult Properties(string id)
		{
			TypeBase result = MvcApplication.Namespaces.FindType(id);
			ViewData["Namespaces"] = MvcApplication.Namespaces;

			return View(result);
		}

		[ValidateInput(false)]
		public ActionResult Constructor(string className, string id)
		{
			ViewData["Namespaces"] = MvcApplication.Namespaces;

			TypeBase type = MvcApplication.Namespaces.FindType(className);
			Constructor result = type.FindConstructor(id);
			return View(result);
		}

		[ValidateInput(false)]
		public ActionResult Method(string className,string id)
		{
			ViewData["Namespaces"] = MvcApplication.Namespaces;

			TypeBase type = MvcApplication.Namespaces.FindType(className);
			Method result = type.FindMethod(id);
			return View(result);
		}

		[ValidateInput(false)]
		public ActionResult Property(string className, string id)
		{
			ViewData["Namespaces"] = MvcApplication.Namespaces;

			TypeBase type = MvcApplication.Namespaces.FindType(className);
			Property result = type.FindProperty(id);
			return View(result);
		}
    }
}
