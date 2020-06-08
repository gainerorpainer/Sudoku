using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public partial class SudokuSolver
    {
        private const int ROW_ELEMENT_COUNT = 9;
        private static readonly byte NDEF_VALUE = 0;


        public byte[] Sudoku { get; private set; }
        public HashSet<byte>[] PossibilitySpace { get; private set; }

        public SudokuSolverLog Log { get; private set; } = new SudokuSolverLog();

        public SudokuSolver(byte[] sudoku)
        {
            Sudoku = sudoku;

            InitSolver();
        }

        static public IEnumerable<int> GetRowIterator(int refIndex)
        {
            // figure out start of line
            int startOfLine = (refIndex / ROW_ELEMENT_COUNT) * ROW_ELEMENT_COUNT;

            // return sequence
            return Enumerable.Range(startOfLine, ROW_ELEMENT_COUNT);
        }

        static public IEnumerable<int> GetCellIterator(int refIndex)
        {
            // figure out quadrant: row 0..2 ; col 0..2
            int rowNum = refIndex / ROW_ELEMENT_COUNT;
            int rowQuadrant = rowNum / 3;
            int colNum = refIndex % ROW_ELEMENT_COUNT;
            int colQuadrant = colNum / 3;

            // return three sequences:
            //           x ; x + 1; x + 2
            // x += 9 => x ; x + 1; x + 2
            // x += 9 => x ; x + 1; x + 2

            // offset the index by the row_quadrant * lines/row_quadrant * elems/line => then by col_quadrant * elems/col_quadrant
            IEnumerable<int> upperRowSequence = Enumerable.Range((rowQuadrant * 3) * ROW_ELEMENT_COUNT + colQuadrant * 3, 3);

            // Generate both other sequences
            return upperRowSequence
                .Union(upperRowSequence.Select(x => x + ROW_ELEMENT_COUNT))
                .Union(upperRowSequence.Select(x => x + ROW_ELEMENT_COUNT * 2));
        }

        public static IEnumerable<int> GetColIterator(int refIndex)
        {
            // figure out col index
            int colNum = refIndex % ROW_ELEMENT_COUNT;

            // each sequence entry is x + 0 ; x + 9 ; x + 18 ; ...
            return Enumerable.Range(0, ROW_ELEMENT_COUNT).Select(x => x * ROW_ELEMENT_COUNT + colNum);
        }

        bool VerifySoduku()
        {
            // All bytes must be 1..9
            if (Sudoku.Any(x => x < 1 || x > 9))
                return false;


            // Search each row, col and cell for non-uniqvue valze
            for (int i = 0; i < ROW_ELEMENT_COUNT; i++)
            {
                // Generate sequence for row, col and cell
                // Make sure to generate an index that satifies these conditions
                var row = GetRowIterator(i * 9).ToHashSet();
                var col = GetColIterator(i).ToHashSet();
                var cell = GetCellIterator((i / 3) * ROW_ELEMENT_COUNT * 3 + i).ToHashSet();

                // There must be 9 distinct values
                if (Toolbox.AllDistinct(row.Select(x => Sudoku[x])) == false) return false;
                if (Toolbox.AllDistinct(col.Select(x => Sudoku[x])) == false) return false;
                if (Toolbox.AllDistinct(cell.Select(x => Sudoku[x])) == false) return false;
            }

            return true;
        }

        public bool SolveSudoku()
        {
            // while not solved
            int newSolutions = 0;
            do
            {
                newSolutions = SolverStep();
            } while (newSolutions > 0);

            double percentageDone() => Sudoku.Count(x => x != NDEF_VALUE) / (double)(ROW_ELEMENT_COUNT * ROW_ELEMENT_COUNT);
            Log.LogBuilder.AppendLine($"DONE ({percentageDone():P1})");

            Log.Heatmap = Log.Heatmap.ToDictionary(x => x.Key, x => 1.0 - x.Value / Log.Iterations);

            if (VerifySoduku() == false)
            {
                Log.LogBuilder.AppendLine("SODUKO IS NOT VALID");
                return false;
            }

            return percentageDone() == 1.0;
        }

        private void InitSolver()
        {
            // Store which fields are already set
            var presetIndexes = Sudoku.Select((x, i) => new { digit = x, index = (byte)i }).Where(x => x.digit != NDEF_VALUE).Select(x => x.index);

            // Init Possibility space
            PossibilitySpace = new HashSet<byte>[ROW_ELEMENT_COUNT * ROW_ELEMENT_COUNT];
            for (byte i = 0; i < PossibilitySpace.Length; i++)
                PossibilitySpace[i] = presetIndexes.Contains(i) ? null : new HashSet<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // each entry is new
            List<byte> newIndices = new List<byte>(presetIndexes);

            // Eliminate presets
            List<byte> presetEliminationSolutions = EliminationSearch(presetIndexes);

            Log.LogBuilder.AppendLine($"Preset: First Elimination-Search found {presetEliminationSolutions.Count} solutions");
        }

        public int SolverStep()
        {
            var allNewSolutions = new List<byte>();
            Log.Iterations++;

            // Search each row + each col + each cell for single missing entry
            var rowColCellSearchSolutions = RowColCellSearch();

            Log.LogBuilder.AppendLine($"Run#{Log.Iterations}: Row-Col-Cell-Search found {rowColCellSearchSolutions.Count} solutions");

            allNewSolutions.AddRange(rowColCellSearchSolutions);

            // Eliminate the new hints
            List<byte> eliminationSearchSolutions = EliminationSearch(rowColCellSearchSolutions);

            Log.LogBuilder.AppendLine($"Run#{Log.Iterations}: Elimination-Search found {eliminationSearchSolutions.Count} solutions");

            allNewSolutions.AddRange(eliminationSearchSolutions);

            // add to heatmap
            allNewSolutions.ForEach(x => Log.Heatmap[x] = Log.Iterations);

            return allNewSolutions.Count;
        }

        private List<byte> EliminationSearch(IEnumerable<byte> rowColCellSearchSolutions)
        {
            var eliminationSearchSolutions = new List<byte>();
            foreach (var recentlyFoundIndex in rowColCellSearchSolutions)
            {
                var eliminatedElements = new List<byte>();

                // store digit for later
                var digit = Sudoku[recentlyFoundIndex];

                // Remove all line, col and cell entries (cannot go there again)
                foreach (var removerIndex in
                        SudokuSolver.GetRowIterator(recentlyFoundIndex)
                        .Union(SudokuSolver.GetColIterator(recentlyFoundIndex))
                        .Union(SudokuSolver.GetCellIterator(recentlyFoundIndex))
                    )
                {
                    var possibleSolutions = PossibilitySpace[removerIndex];

                    // Check if already set
                    if (possibleSolutions is null)
                        continue;

                    // Remove possibility
                    possibleSolutions.Remove(digit);

                    // check if only solution
                    if (possibleSolutions.Count == 1)
                        ProcessNewMatch(possibleSolutions.First(), (byte)removerIndex, ref eliminatedElements);
                }

                eliminationSearchSolutions.AddRange(eliminatedElements);
            }

            return eliminationSearchSolutions;
        }

        private List<byte> RowColCellSearch()
        {
            var newHints = new List<byte>();

            void searchLogicalUnit(HashSet<int> indexes)
            {
                for (byte digit = 1; digit < ROW_ELEMENT_COUNT + 1; digit++)
                {
                    // How many squares are still missing this digit?
                    var candidates = PossibilitySpace.Select((x, index) => new { possibilities = x, index = (byte)index })
                        .Where(x => indexes.Contains(x.index))
                        .Where(x => x.possibilities?.Contains(digit) ?? false)
                        .Select(x => x.index).ToList();

                    // this must be a hit
                    if (candidates.Count == 1)
                        ProcessNewMatch(digit, candidates.First(), ref newHints);
                }
            }

            // Search each row, col and cell for each missing entry
            for (int i = 0; i < ROW_ELEMENT_COUNT; i++)
            {
                // Generate sequence for row, col and cell
                // Make sure to generate an index that satifies these conditions
                var row = GetRowIterator(i * 9).ToHashSet();
                var col = GetColIterator(i).ToHashSet();
                var cell = GetCellIterator((i / 3) * ROW_ELEMENT_COUNT * 3 + i).ToHashSet();

                searchLogicalUnit(row);
                searchLogicalUnit(col);
                searchLogicalUnit(cell);
            }

            return newHints;
        }

        private void ProcessNewMatch(byte digit, byte solutionIndex, ref List<byte> newSolutions)
        {
            // store solution
            Sudoku[solutionIndex] = digit;

            // Add for hints collection
            newSolutions.Add(solutionIndex);

            // Flag this as set
            PossibilitySpace[solutionIndex] = null;
        }
    }
}
