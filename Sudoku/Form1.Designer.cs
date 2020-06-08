namespace Sudoku
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBox_SudokuStr = new System.Windows.Forms.TextBox();
            this.button_Solve = new System.Windows.Forms.Button();
            this.button_TEST = new System.Windows.Forms.Button();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.button_Step = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(303, 313);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // textBox_SudokuStr
            // 
            this.textBox_SudokuStr.Location = new System.Drawing.Point(12, 331);
            this.textBox_SudokuStr.Name = "textBox_SudokuStr";
            this.textBox_SudokuStr.Size = new System.Drawing.Size(303, 22);
            this.textBox_SudokuStr.TabIndex = 2;
            this.textBox_SudokuStr.Validated += new System.EventHandler(this.textBox_SudokuStr_Validated);
            // 
            // button_Solve
            // 
            this.button_Solve.Location = new System.Drawing.Point(353, 214);
            this.button_Solve.Name = "button_Solve";
            this.button_Solve.Size = new System.Drawing.Size(75, 23);
            this.button_Solve.TabIndex = 3;
            this.button_Solve.Text = "Solve";
            this.button_Solve.UseVisualStyleBackColor = true;
            this.button_Solve.Click += new System.EventHandler(this.button_Solve_Click);
            // 
            // button_TEST
            // 
            this.button_TEST.Location = new System.Drawing.Point(353, 267);
            this.button_TEST.Name = "button_TEST";
            this.button_TEST.Size = new System.Drawing.Size(75, 23);
            this.button_TEST.TabIndex = 4;
            this.button_TEST.Text = "Test";
            this.button_TEST.UseVisualStyleBackColor = true;
            this.button_TEST.Click += new System.EventHandler(this.button_TEST_Click);
            // 
            // textBox_Log
            // 
            this.textBox_Log.Location = new System.Drawing.Point(328, 12);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Log.Size = new System.Drawing.Size(460, 173);
            this.textBox_Log.TabIndex = 5;
            // 
            // button_Step
            // 
            this.button_Step.Location = new System.Drawing.Point(434, 214);
            this.button_Step.Name = "button_Step";
            this.button_Step.Size = new System.Drawing.Size(75, 23);
            this.button_Step.TabIndex = 6;
            this.button_Step.Text = "Step";
            this.button_Step.UseVisualStyleBackColor = true;
            this.button_Step.Click += new System.EventHandler(this.button_Step_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_Step);
            this.Controls.Add(this.textBox_Log);
            this.Controls.Add(this.button_TEST);
            this.Controls.Add(this.button_Solve);
            this.Controls.Add(this.textBox_SudokuStr);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBox_SudokuStr;
        private System.Windows.Forms.Button button_Solve;
        private System.Windows.Forms.Button button_TEST;
        private System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.Button button_Step;
    }
}

