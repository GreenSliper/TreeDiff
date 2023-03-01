namespace TreeDiff
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class UseDiffAttribute : Attribute
	{
		public UseDiffAttribute() { }
	}
}
