﻿namespace Client
{
    partial class ViewMain
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btn_Record = new System.Windows.Forms.Button();
            this.btn_AddProject = new System.Windows.Forms.Button();
            this.btn_Evaluate = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 15);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(201, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // btn_Record
            // 
            this.btn_Record.Location = new System.Drawing.Point(222, 12);
            this.btn_Record.Name = "btn_Record";
            this.btn_Record.Size = new System.Drawing.Size(50, 25);
            this.btn_Record.TabIndex = 1;
            this.btn_Record.Text = "Record";
            this.btn_Record.UseVisualStyleBackColor = true;
            // 
            // btn_AddProject
            // 
            this.btn_AddProject.Location = new System.Drawing.Point(12, 42);
            this.btn_AddProject.Name = "btn_AddProject";
            this.btn_AddProject.Size = new System.Drawing.Size(34, 23);
            this.btn_AddProject.TabIndex = 2;
            this.btn_AddProject.Text = "+";
            this.btn_AddProject.UseVisualStyleBackColor = true;
            // 
            // btn_Evaluate
            // 
            this.btn_Evaluate.Location = new System.Drawing.Point(52, 42);
            this.btn_Evaluate.Name = "btn_Evaluate";
            this.btn_Evaluate.Size = new System.Drawing.Size(75, 23);
            this.btn_Evaluate.TabIndex = 3;
            this.btn_Evaluate.Text = "Evaluation";
            this.btn_Evaluate.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 71);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(284, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ViewMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 93);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btn_Evaluate);
            this.Controls.Add(this.btn_AddProject);
            this.Controls.Add(this.btn_Record);
            this.Controls.Add(this.comboBox1);
            this.Name = "ViewMain";
            this.Text = "Project Tracker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btn_Record;
        private System.Windows.Forms.Button btn_AddProject;
        private System.Windows.Forms.Button btn_Evaluate;
        private System.Windows.Forms.StatusStrip statusStrip1;
    }
}

