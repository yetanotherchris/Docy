using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docy.Core;
using RazorEngine;
using System.IO;
using RazorEngine.Templating;
using System.Reflection;

namespace Docy.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			DocParser parser = new DocParser();
			IEnumerable<Namespace> namespaces = parser.Parse(@"C:\projects\roadkill\Roadkill.Core\bin\Release\Roadkill.Core.dll");
			Razor.SetTemplateBase(typeof(TemplateBase<>));

			// Namespaces			
			string template = GetTemplate("Index.cshtml");
			string output = Razor.Parse<IEnumerable<Namespace>>(template, namespaces);
			File.WriteAllText(@"Output\index.html", output);

			// All types in all namespaces
			template = GetTemplate("Namespace.cshtml");
			string typeTemplate = GetTemplate("Type.cshtml");
			foreach (Namespace nameSpace in namespaces)
			{
				output = Razor.Parse<Namespace>(template, nameSpace);
				File.WriteAllText(@"Output\" +nameSpace.Name+ ".html", output);

				// Classes
				foreach (TypeBase typeBase in nameSpace.AllTypes)
				{
					output = Razor.Parse<TypeBase>(typeTemplate, typeBase);
					File.WriteAllText(@"Output\" + typeBase.Fullname + ".html", output);
				}
			}
		}

		static string GetTemplate(string name)
		{
			using (StreamReader reader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("Docy.Console.Templates." + name)))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
