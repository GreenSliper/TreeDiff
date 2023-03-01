using TreeDiff;
using TreeDiffConsole;

var foo = new TestStructure(0, 0)
{
	children = new List<TestStructure>()
	{
		new TestStructure(1, 1)
		{
			children = new List<TestStructure>()
			{
				new TestStructure(4, 4)
			}
		},
		new TestStructure(2, 2),
		new TestStructure(3, 3)
		{
			children = new List<TestStructure>()
			{
				new TestStructure(5, 5)
			}
		}
	}
};

var bar = foo.Clone();
bar.children.RemoveAt(0);
bar.children[0].children.Add(new TestStructure(6, 6));
bar.children[1].children.Add(new TestStructure(7, 7));
bar.children[1].value = 999;

var result = new TreeDiffResolver().Diff(foo, bar);
Console.ReadLine();