using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Docy.Core
{
	[DebuggerDisplay("{Fullname}")]
	public class Property : CommentsBase
	{
		public TypeBase Parent { get; set; }
		public string Type { get; set; }
		public string TypeFullName { get; set; }
		public string Attributes { get; set; }

		//info.IsStatic;
		//info.IsPublic;
		//info.IsPrivate;
		//info.IsGenericMethod;
		//info.IsVirtual;
		//info.IsFinal;
		//info.IsAbstract;

		public Property() : base() { }
	}
}
