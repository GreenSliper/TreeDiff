using System.Reflection;

namespace TreeDiff
{
	public class TreeDiffResult
	{
		public enum State { Unchanged, Changed, Added, Removed}

		public State state;
		public ITreeDiff element;
		public List<TreeDiffResult> children = new List<TreeDiffResult>();

		public bool DescendantsAltered { 
			get 
			{
				if(children.Count == 0)
					return true;
				return children.Any(x => x.DescendantsAltered);
			}
		}

		public TreeDiffResult(ITreeDiff element, State state)
		{
			this.element = element;
			this.state = state;
		}
	}

	public interface ITreeDiffResolver
	{
		TreeDiffResult Diff(ITreeDiff source, ITreeDiff changed);
	}

	public class TreeDiffResolver
	{
		public TreeDiffResult Diff(ITreeDiff source, ITreeDiff changed)
		{
			if (!source.PrimarilyIdentical(changed))
				throw new ArgumentException("Cannot diff trees with non-identical roots!");
			TreeDiffResult result = null;
			if (CompletelyIdentical(source, changed))
				result = new TreeDiffResult(source, TreeDiffResult.State.Unchanged);
			else
				result = new TreeDiffResult(changed, TreeDiffResult.State.Changed);
			Diff(source, changed, result);
			return result;
		}

		void Diff(ITreeDiff source, ITreeDiff changed, TreeDiffResult result)
		{
			var changedChildren = new List<ITreeDiff>(changed.GetChildren());
			var srcChildren = source.GetChildren();

			foreach (var srcChild in srcChildren)
			{
				//if child stays
				var changedChildIndex = changedChildren.FindIndex(x => x.PrimarilyIdentical(srcChild));
				if (changedChildIndex > -1)
				{
					TreeDiffResult diffResult = null;
					if (CompletelyIdentical(srcChild, changedChildren[changedChildIndex]))
						diffResult = new TreeDiffResult(srcChild, TreeDiffResult.State.Unchanged);
					else
						diffResult = new TreeDiffResult(changedChildren[changedChildIndex], TreeDiffResult.State.Changed);
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

		void RemoveRecursive(ITreeDiff element, TreeDiffResult result)
		{
			var tdr = new TreeDiffResult(element, TreeDiffResult.State.Removed);
			result.children.Add(tdr);
			foreach (var child in element.GetChildren())
				RemoveRecursive(child, tdr);
		}

		void AddRecursive(IEnumerable<ITreeDiff> elements, TreeDiffResult result)
		{
			foreach (var elem in elements)
			{
				var tdr = new TreeDiffResult(elem, TreeDiffResult.State.Added);
				result.children.Add(tdr);
				AddRecursive(elem.GetChildren(), tdr);
			}
		}

		static BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		bool CompletelyIdentical(ITreeDiff source, ITreeDiff changed)
		{

			var fields = source.GetType().GetFields(flags).Where(x=>x.CustomAttributes.Any(y=>y.AttributeType == typeof(UseDiffAttribute)));
			foreach (var field in fields)
				if (!field.GetValue(source).Equals(field.GetValue(changed)))
					return false;
			return true;
		}
	}
}