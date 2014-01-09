using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Docy.Core
{
	[DebuggerDisplay("{Name}")]
	public class Namespace
	{
		public string Name { get; set; }
		public IList<TypeBase> Classes { get; set; }
		public IList<TypeBase> Interfaces { get; set; }
		public IList<TypeBase> Structures { get; set; }
		public IList<TypeBase> Enumerations { get; set; }
		public IList<TypeBase> Delegates { get; set; }
		public IList<TypeBase> AllTypes
		{
			get
			{
				return Classes.Union(Interfaces).Union(Structures).Union(Enumerations).Union(Delegates).OrderBy(t => t.Name).ToList();
			}
		}

		public Namespace()
		{
			Classes = new List<TypeBase>();
			Interfaces = new List<TypeBase>();
			Structures = new List<TypeBase>();
			Enumerations = new List<TypeBase>();
			Delegates = new List<TypeBase>();
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			Namespace b = obj as Namespace;
			if (b == null)
				return false;

			return Name == b.Name;
		}
	}
}
