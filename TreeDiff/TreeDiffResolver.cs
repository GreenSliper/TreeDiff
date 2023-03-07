using System.Reflection;

namespace TreeDiff
{
	public class TreeDiffResult
	{
		public enum State { Unchanged, Changed, Added, Removed}

		public State state;
		public ITreeDiff element, otherValue;
		public List<TreeDiffResult> children = new List<TreeDiffResult>();

		public bool DescendantsAltered { 
			get 
			{
				if(children.Count == 0)
					return false;
				return children.Any(x => x._descendantsAltered);
			}
		}

		bool _descendantsAltered { 
			get 
			{
				return state != State.Unchanged || children.Any(x => x._descendantsAltered);
			}
		}

		public TreeDiffResult(ITreeDiff element, State state, ITreeDiff otherValue = null)
		{
			this.element = element;
			this.state = state;
			this.otherValue = otherValue;
		}
	}

	public interface ITreeDiffResolver
	{
		TreeDiffResult Diff(ITreeDiff source, ITreeDiff changed);
	}

	public class TreeDiffResolver : ITreeDiffResolver
	{
		public TreeDiffResult Diff(ITreeDiff source, ITreeDiff changed)
		{
			if (!source.PrimarilyIdentical(changed))
				throw new ArgumentException("Cannot diff trees with non-identical roots!");
			TreeDiffResult result = null;
			if (CompletelyIdentical(source, changed))
				result = new TreeDiffResult(source, TreeDiffResult.State.Unchanged, changed);
			else
				result = new TreeDiffResult(changed, TreeDiffResult.State.Changed, source);
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
						diffResult = new TreeDiffResult(srcChild, TreeDiffResult.State.Unchanged, changedChildren[changedChildIndex]);
					else
						diffResult = new TreeDiffResult(changedChildren[changedChildIndex], TreeDiffResult.State.Changed, srcChild);
					result.children.Add(diffResult);
					Diff(srcChild, changedChildren[changedChildIndex], diffResult);
					changedChildren.RemoveAt(changedChildIndex);
				}
				else //child was removed from src
					RemoveRecursive(srcChild, result);
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
		bool CompletelyIdentical(object source, object changed)
		{
			if (source == null || changed == null)
				return source == changed; //true if both null only

			var fieldsAttributes = source.GetType().GetFields(flags)
				.Where(x=>x.CustomAttributes.Any(y=>y.AttributeType == typeof(UseDiffAttribute)))
				.Select(x => 
					new { 
						field = x, 
						attr = x.GetCustomAttribute<UseDiffAttribute>()
					});

			foreach (var fa in fieldsAttributes)
			{
				object firstValue = fa.field.GetValue(source),
						secondValue = fa.field.GetValue(changed);
				if (fa.attr.recusive)
				{
					if (!CompletelyIdentical(firstValue, secondValue))
						return false;
				}
				else if (firstValue == null || secondValue == null)
				{
					if (firstValue != secondValue)
						return false;
				}
				else if (!firstValue.Equals(secondValue))
					return false;
			}
			return true;
		}
	}
}