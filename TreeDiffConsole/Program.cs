// See https://aka.ms/new-console-template for more information


using TreeDiff;
using TreeDiffConsole;

var foo = new TestStructure(0)
{
	children = new List<TestStructure>()
	{
		new TestStructure(1)
		{
			children = new List<TestStructure>()
			{
				new TestStructure(4)
			}
		},
		new TestStructure(2),
		new TestStructure(3)
		{
			children = new List<TestStructure>()
			{
				new TestStructure(5)
			}
		}
	}
};

var bar = foo.Clone();
bar.children.RemoveAt(0);
bar.children[0].children.Add(new TestStructure(6));
bar.children[1].children.Add(new TestStructure(7));
var result = new TreeDiff.TreeDiff().Diff(foo, bar);
Console.ReadLine();