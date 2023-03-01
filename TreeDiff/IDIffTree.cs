using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeDiff
{
	public interface IDiffTree
	{
		IEnumerable<IDiffTree> GetChildren();
		/// <summary>
		/// Identical by ID or other key. Need to check types!
		/// </summary>
		bool PrimarilyIdentical(IDiffTree other);
	}
}
