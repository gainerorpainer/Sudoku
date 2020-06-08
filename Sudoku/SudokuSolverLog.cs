using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public class SudokuSolverLog
    {
        public int Iterations { get; set; } = 0;
        public StringBuilder LogBuilder { get; set; } = new StringBuilder();
        public Dictionary<byte, double> Heatmap { get; set; } = new Dictionary<byte, double>();

        public string LogText => LogBuilder.ToString();
    }
}
