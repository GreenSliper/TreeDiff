namespace TreeDiff
{
	public class TreeDiffResult
	{
		public enum State { Unchanged, Changed, Added, Removed}

		public State state;
		public IDiffTree element;
		public List<TreeDiffResult> children = new List<TreeDiffResult>();

		public bool DescendantsAltered { 
			get 
			{
				if(children.Count == 0)
					return true;
				return children.Any(x => x.DescendantsAltered);
			}
		}

		public TreeDiffResult(IDiffTree element, State state)
		{
			this.element = element;
			this.state = state;
		}
	}

	public class TreeDiff
	{
		public TreeDiffResult Diff(IDiffTree source, IDiffTree changed)
		{
			if (!source.PrimarilyIdentical(changed))
				throw new ArgumentException("Cannot diff trees with non-identical roots!");
			TreeDiffResult result = new TreeDiffResult(source, 
				CompletelyIdentical(source, changed) ? TreeDiffResult.State.Unchanged : TreeDiffResult.State.Changed);
			Diff(source, changed, result);
			return result;
		}

		void Diff(IDiffTree source, IDiffTree changed, TreeDiffResult result)
		{
			var changedChildren = new List<IDiffTree>(changed.GetChildren());
			var srcChildren = source.GetChildren();

			foreach (var srcChild in srcChildren)
			{
				//if child stays
				var changedChildIndex = changedChildren.FindIndex(x => x.PrimarilyIdentical(srcChild));
				if (changedChildIndex > -1)
				{
					bool identical = CompletelyIdentical(srcChild, changedChildren[changedChildIndex]);
					TreeDiffResult diffResult = new TreeDiffResult(srcChild,
						identical ? TreeDiffResult.State.Unchanged : TreeDiffResult.State.Changed);
					result.children.Add(diffResult);
					Diff(srcChild, changedChildren[changedChildIndex], diffResult);
					changedChildren.RemoveAt(changedChildIndex);
				}
				else //child was removed from src
					RemoveRecursive(srcChild, result);
				//result.children.Add(new TreeDiffResult(srcChild, TreeDiffResult.State.Removed));
			}
			AddRecursive(changedChildren, result);
		}

		void RemoveRecursive(IDiffTree element, TreeDiffResult result)
		{
			var tdr = new TreeDiffResult(element, TreeDiffResult.State.Removed);
			result.children.Add(tdr);
			foreach (var child in element.GetChildren())
				RemoveRecursive(child, tdr);
		}

		void AddRecursive(IEnumerable<IDiffTree> elements, TreeDiffResult result)
		{
			foreach (var elem in elements)
			{
				var tdr = new TreeDiffResult(elem, TreeDiffResult.State.Added);
				result.children.Add(tdr);
				AddRecursive(elem.GetChildren(), tdr);
			}
		}

		bool CompletelyIdentical(IDiffTree source, IDiffTree changed)
		{
			return true;
			//throw new NotImplementedException();
		}
	}
}