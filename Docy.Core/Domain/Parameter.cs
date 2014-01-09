using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docy.Core
{
	public class Parameter
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public string TypeFullName { get; set; }
		public string Attributes { get; set; }
		public bool IsOut { get; set; }
		public bool IsRet { get; set; }

		public override string ToString()
		{
			return Type +" "+ Name;
		}
	}
}
