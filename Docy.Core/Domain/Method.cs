using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Docy.Core
{
	[DebuggerDisplay("{Fullname}")]
	public class Method : CommentsBase
	{
		public TypeBase Parent { get; set; }
		public string ReturnType { get; set; }
		public string ReturnTypeFullName { get; set; }
		public IList<Parameter> Parameters { get; set; }

		public Method()
			: base()
		{
			Parameters = new List<Parameter>();
		}

		/// <summary>
		/// Returns the parameters types, comma seperated.
		/// </summary>
		/// <returns></returns>
		public string ParameterTypes()
		{
			List<string> list = new List<string>();
			foreach (Parameter parameter in Parameters)
			{
				list.Add(parameter.Type);
			}

			return string.Join(",", list);
		}
	}
}