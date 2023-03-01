using System;

namespace TreeDiff
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class UseDiffAttribute : Attribute
	{
		public bool recusive;
		public UseDiffAttribute(bool recursive = false) 
		{
			this.recusive = recursive;
		}
	}
}
