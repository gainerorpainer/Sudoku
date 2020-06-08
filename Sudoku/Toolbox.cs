using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    static class Toolbox
    {
        public static bool AllDistinct<T>(IEnumerable<T> sequence) => sequence.Distinct().Count() == sequence.Count();
    }
}
