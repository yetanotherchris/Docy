using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docy.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class CommentsBase
	{
		public string Name { get; set; }
		public string Fullname { get; set; }
		public string Summary { get; set; }
		public string Remarks { get; set; }
		public string Returns { get; set; }
		public string Example { get; set; }
		public string Exceptions { get; set; }

		public bool UseHashCodeForId { get; set; }
		public string Id
		{
			get
			{
				if (UseHashCodeForId)
					return Fullname.GetHashCode().ToString();
				else
					return Name;
			}
		}

		// Exceptions, SeeAlso

		public CommentsBase()
		{
			Fullname = "";
			Summary = "";
			Remarks = "";
			Returns = "";
			Example = "";
			Exceptions = "";
		}
	}
}
