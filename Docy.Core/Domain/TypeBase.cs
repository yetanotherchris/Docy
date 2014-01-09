using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Docy.Core
{
	[DebuggerDisplay("{Fullname}")]
	public class TypeBase : CommentsBase
	{
		public Namespace Namespace { get; set; }
		public string ParentClass { get; set; }
		public List<TypeSummary> Parents { get; set; }

		/// <summary>
		/// Returns one of these: Class,Interfact, Structure,Enumeration,Delegate
		/// </summary>
		public string ObjectType { get; set; }

		public bool IsPublic { get; set; }
		public bool IsSealed { get; set; }
		public bool IsAbstract { get; set; }
		public bool IsPrimitive { get; set; }
		public bool IsNested { get; set; }

		public IList<Constructor> Constructors { get; set; }
		public IList<Method> Methods { get; set; }
		public IList<Property> Properties { get; set; }
		public IList<MemberSummary> Members { get; set; }

		public TypeBase() : base()
		{
			Constructors = new List<Constructor>();
			Methods = new List<Method>();
			Properties = new List<Property>();
			Members = new List<MemberSummary>();
			Parents = new List<TypeSummary>();
		}
	}

	[DebuggerDisplay("{Fullname}")]
	public class TypeSummary
	{
		public string Name { get; set; }
		public string Fullname { get; set; }
	}

	[DebuggerDisplay("{Name}")]
	public class MemberSummary
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
