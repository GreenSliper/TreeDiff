using TreeDiff;

namespace TreeDiffConsole
{
	internal class TestStructure : ITreeDiff
	{
		public int key;
		[UseDiff]
		public int value;
		public List<TestStructure> children = new List<TestStructure>();

		public TestStructure() { }
		public TestStructure(int key, int value) 
		{
			this.key = key;
			this.value = value;
		}

		public IEnumerable<ITreeDiff> GetChildren() => children;

		public bool PrimarilyIdentical(ITreeDiff other)
		{
			return other is TestStructure ts && ts.key == key;
		}

		public TestStructure Clone()
		{
			var result = new TestStructure() { key = this.key, children = new List<TestStructure>(children.Count) };
			foreach (var child in children)
				result.children.Add(child.Clone());
			return result;
		}
	}
}
