using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeDiff;

namespace TreeDiffConsole
{
	internal class TestStructure : IDiffTree
	{
		public int key;
		public List<TestStructure> children = new List<TestStructure>();

		public TestStructure() { }
		public TestStructure(int key) 
		{
			this.key = key;
		}

		public IEnumerable<IDiffTree> GetChildren() => children;

		public bool PrimarilyIdentical(IDiffTree other)
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
