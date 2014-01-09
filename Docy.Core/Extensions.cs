using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docy.Core
{
	public static class Extensions
	{
		public static TypeBase FindType(this IEnumerable<Namespace> namespaces, string fullname)
		{
			foreach (Namespace nameSpace in namespaces)
			{
				TypeBase typeBase = nameSpace.AllTypes.FirstOrDefault(x => x.Fullname == fullname);
				if (typeBase != null)
					return typeBase;
			}

			return null;
		}

		public static Constructor FindConstructor(this TypeBase typeBase, string id)
		{
			return typeBase.Constructors.FirstOrDefault(c => c.Id == id);
		}

		public static Method FindMethod(this TypeBase typeBase, string id)
		{
			return typeBase.Methods.FirstOrDefault(m => m.Id == id);
		}

		public static Property FindProperty(this TypeBase typeBase, string id)
		{
			return typeBase.Properties.FirstOrDefault(p => p.Id == id);
		}
	}
}
