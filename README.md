# TreeDiff
Algorithm for finding differences between 2 trees, returning result as tree with marked nodes

# Idea
Concept was developped when working with API-syncronized complex tree structures. Goal was to detect all changes performed on a structure, and then use API to transfer all the changes to remote server.

# Result structure
The result of diff operation is an istance of TreeDiffResult. It's a tree, which topology is a result of merge of source and changed trees. Each node is marked with enum State { Unchanged, Changed, Added, Removed }.

# Getting started
Source data structure(s) should implement ITreeDiff interface. All fields, marked with UseDiffAttribute will be used for change detection. Example (TreeDiffConsole/TestStructure.cs):

```cs
internal class TestStructure : ITreeDiff
{
  public int key;
  [UseDiff]
  public int value;
  public List<TestStructure> children = new List<TestStructure>();

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
  //TestStructure Clone()
}
```

The example of TreeDiffResolver use is shown in TreeDiffConsole/Program.cs:
```cs
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
```
