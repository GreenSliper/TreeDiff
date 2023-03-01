using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeDiff
{
	public interface ITreeDiff
	{
		IEnumerable<ITreeDiff> GetChildren();
		/// <summary>
		/// Identical by ID or other key. Need to check types!
		/// </summary>
		bool PrimarilyIdentical(ITreeDiff other);
	}
}
