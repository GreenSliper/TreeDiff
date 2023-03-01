using TreeDiff;

namespace TreeDiffConsole
{
	internal class TestStructure : ITreeDiff
	{
		public class InnerStructure
		{
			[UseDiff]
			public int a;
			public InnerStructure(int a)
			{
				this.a = a;
			}
		}

		public int key;
		//field used for equality comparison
		[UseDiff]
		public int value;
		//recursive descend
		[UseDiff(recursive: true)]
		public InnerStructure innerStructure = null;

		public List<TestStructure> children = new List<TestStructure>();

		public TestStructure() { }
		public TestStructure(int key, int value, InnerStructure innerStructure = null) 
		{
			this.key = key;
			this.value = value;
			this.innerStructure = innerStructure;
		}

		public IEnumerable<ITreeDiff> GetChildren() => children;

		public bool PrimarilyIdentical(ITreeDiff other)
		{
			return other is TestStructure ts && ts.key == key;
		}

		public TestStructure Clone()
		{
			var result = new TestStructure(key, value) { children = new List<TestStructure>(children.Count) };
			foreach (var child in children)
				result.children.Add(child.Clone());
			if(innerStructure != null)
				result.innerStructure = new InnerStructure(innerStructure.a);
			return result;
		}
	}
}
