using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private const byte NDEF_VALUE = 0;
        byte[] Sudoku { get; set; } = new byte[9 * 9];
        Dictionary<byte, double> Heatmap { get; set; } = new Dictionary<byte, double>();
        HashSet<byte> PresetIndexes { get; set; } = new HashSet<byte>();
        SudokuSolver Solver { get; set; } 

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            // Find scale
            float scale = Math.Min(pictureBox1.Width, pictureBox1.Height);

            g.ScaleTransform(scale, scale);

            // Draw grid
            Queue<Tuple<PointF, PointF>> gridLines = new Queue<Tuple<PointF, PointF>>();
            // Split = 1/9 then 2/9 ...
            for (int i = 0; i < 8; i++)
            {
                float split = (i + 1.0f) / 9.0f;

                // Add vertical then horizontal line
                gridLines.Enqueue(new Tuple<PointF, PointF>(
                    new PointF(split, 0.0f),
                    new PointF(split, 1.0f)
                    ));
                gridLines.Enqueue(new Tuple<PointF, PointF>(
                    new PointF(0.0f, split),
                    new PointF(1.0f, split)
                    ));
            }

            // Draw each 3rd grid pair thicker
            for (int i = 0; i < 8; i++)
            {
                var pen = (i + 1) % 3 != 0 ? new Pen(Brushes.Black, 0.01f) : new Pen(Brushes.Black, 0.02f);

                var points = gridLines.Dequeue();
                g.DrawLine(pen, points.Item1, points.Item2);
                points = gridLines.Dequeue();
                g.DrawLine(pen, points.Item1, points.Item2);
            }

            // Draw each digit
            Font f = new Font(SystemFonts.DefaultFont.FontFamily, 1.0f / 9, FontStyle.Regular, GraphicsUnit.Pixel);
            Font f_bold = new Font(f, FontStyle.Bold);
            StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            for (byte row = 0; row < 9; row++)
            {
                for (byte col = 0; col < 9; col++)
                {
                    byte index = (byte)(row * 9 + col);

                    byte digit = Sudoku[index];

                    if (digit == NDEF_VALUE)
                        continue;


                    Brush brush = Heatmap.TryGetValue(index, out double heat) ? new SolidBrush(Color.FromArgb((int)(heat * 255), 125, 125)) : Brushes.Black;
                    Font font = Heatmap.ContainsKey(index) ? f : f_bold;

                    RectangleF layoutRectangle = new RectangleF(col * 1.0f / 9.0f, row * 1.0f / 9.0f, 1.0f / 9.0f, 1.0f / 9.0f);
                    g.DrawString(digit.ToString(), font, brush, layoutRectangle, sf);
                }
            }
        }

        private void textBox_SudokuStr_Validated(object sender, EventArgs e)
        {
            var str = textBox_SudokuStr.Text.Trim();

            // Check how many chars
            if (str.Length != 9 * 9)
                return;

            // check each char
            foreach (var c in str)
            {
                if ((c != '.')
                    &&
                    (c < '0' || c > '9'))
                    return;
            }

            // Clear model
            Sudoku.Initialize();
            PresetIndexes.Clear();

            for (byte i = 0; i < str.Length; i++)
            {
                char c = str[i];
                byte byteVal = c == '.' ? NDEF_VALUE : (byte)(c - '0');

                Sudoku[i] = byteVal;

                if (byteVal != NDEF_VALUE)
                    PresetIndexes.Add(i);
            }

            // Init solver
            Solver = new SudokuSolver(Sudoku);

            // Repaint
            pictureBox1.Refresh();
        }

        private void button_Solve_Click(object sender, EventArgs e)
        {
            var timer = Stopwatch.StartNew();

            SudokuSolver sudokuSolver = new SudokuSolver(Sudoku);
            sudokuSolver.SolveSudoku();
            var log = sudokuSolver.Log;

            Heatmap = log.Heatmap;
            textBox_Log.Text = log.LogText;

            Text = timer.ElapsedMilliseconds + " ms";

            pictureBox1.Refresh();
        }


        private void button_TEST_Click(object sender, EventArgs e)
        {
            textBox_SudokuStr.Text = "....91.4..7.2.4.8..4278..9...5.....96..4.5..13.....6...5..4736..8.6.9.1..6.32....";
            textBox_SudokuStr_Validated(this, null);

            //button_Solve_Click(this, null);
        }

        private void button_Step_Click(object sender, EventArgs e)
        {
            Solver.SolverStep();

            pictureBox1.Refresh();
        }
    }
}
