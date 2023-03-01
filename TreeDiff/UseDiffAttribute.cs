using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeDiff
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class UseDiffAttribute : Attribute
	{
		public UseDiffAttribute() { }
	}
}
