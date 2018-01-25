namespace Client.Views
{
    partial class ViewEvaluation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_ProjectName = new System.Windows.Forms.Label();
            this.lb_ProjectedEffort = new System.Windows.Forms.Label();
            this.lb_CurrentEffort = new System.Windows.Forms.Label();
            this.tlp_Evaluation = new System.Windows.Forms.TableLayoutPanel();
            this.btn_Close = new System.Windows.Forms.Button();
            this.lb_Difference = new System.Windows.Forms.Label();
            this.tbx_ProjectName = new System.Windows.Forms.TextBox();
            this.tbx_ProjectedEffort = new System.Windows.Forms.TextBox();
            this.tbx_CurrentEffort = new System.Windows.Forms.TextBox();
            this.tbx_Difference = new System.Windows.Forms.TextBox();
            this.tlp_Evaluation.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_ProjectName
            // 
            this.lb_ProjectName.Location = new System.Drawing.Point(3, 0);
            this.lb_ProjectName.Name = "lb_ProjectName";
            this.lb_ProjectName.Size = new System.Drawing.Size(124, 23);
            this.lb_ProjectName.TabIndex = 0;
            this.lb_ProjectName.Text = "Projectname:";
            this.lb_ProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb_ProjectedEffort
            // 
            this.lb_ProjectedEffort.Location = new System.Drawing.Point(3, 25);
            this.lb_ProjectedEffort.Name = "lb_ProjectedEffort";
            this.lb_ProjectedEffort.Size = new System.Drawing.Size(124, 24);
            this.lb_ProjectedEffort.TabIndex = 0;
            this.lb_ProjectedEffort.Text = "Projected effort:";
            this.lb_ProjectedEffort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb_CurrentEffort
            // 
            this.lb_CurrentEffort.Location = new System.Drawing.Point(3, 50);
            this.lb_CurrentEffort.Name = "lb_CurrentEffort";
            this.lb_CurrentEffort.Size = new System.Drawing.Size(124, 24);
            this.lb_CurrentEffort.TabIndex = 0;
            this.lb_CurrentEffort.Text = "Current effort:";
            this.lb_CurrentEffort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlp_Evaluation
            // 
            this.tlp_Evaluation.ColumnCount = 2;
            this.tlp_Evaluation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Evaluation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Evaluation.Controls.Add(this.lb_ProjectName, 0, 0);
            this.tlp_Evaluation.Controls.Add(this.btn_Close, 1, 5);
            this.tlp_Evaluation.Controls.Add(this.lb_CurrentEffort, 0, 2);
            this.tlp_Evaluation.Controls.Add(this.lb_ProjectedEffort, 0, 1);
            this.tlp_Evaluation.Controls.Add(this.lb_Difference, 0, 3);
            this.tlp_Evaluation.Controls.Add(this.tbx_ProjectName, 1, 0);
            this.tlp_Evaluation.Controls.Add(this.tbx_ProjectedEffort, 1, 1);
            this.tlp_Evaluation.Controls.Add(this.tbx_CurrentEffort, 1, 2);
            this.tlp_Evaluation.Controls.Add(this.tbx_Difference, 1, 3);
            this.tlp_Evaluation.Location = new System.Drawing.Point(12, 12);
            this.tlp_Evaluation.Name = "tlp_Evaluation";
            this.tlp_Evaluation.RowCount = 6;
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tlp_Evaluation.Size = new System.Drawing.Size(260, 161);
            this.tlp_Evaluation.TabIndex = 1;
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(133, 123);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(124, 35);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            // 
            // lb_Difference
            // 
            this.lb_Difference.Location = new System.Drawing.Point(3, 75);
            this.lb_Difference.Name = "lb_Difference";
            this.lb_Difference.Size = new System.Drawing.Size(124, 24);
            this.lb_Difference.TabIndex = 0;
            this.lb_Difference.Text = "Difference:";
            this.lb_Difference.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbx_ProjectName
            // 
            this.tbx_ProjectName.Location = new System.Drawing.Point(133, 3);
            this.tbx_ProjectName.Name = "tbx_ProjectName";
            this.tbx_ProjectName.ReadOnly = true;
            this.tbx_ProjectName.Size = new System.Drawing.Size(124, 20);
            this.tbx_ProjectName.TabIndex = 1;
            // 
            // tbx_ProjectedEffort
            // 
            this.tbx_ProjectedEffort.Location = new System.Drawing.Point(133, 28);
            this.tbx_ProjectedEffort.Name = "tbx_ProjectedEffort";
            this.tbx_ProjectedEffort.ReadOnly = true;
            this.tbx_ProjectedEffort.Size = new System.Drawing.Size(124, 20);
            this.tbx_ProjectedEffort.TabIndex = 1;
            // 
            // tbx_CurrentEffort
            // 
            this.tbx_CurrentEffort.Location = new System.Drawing.Point(133, 53);
            this.tbx_CurrentEffort.Name = "tbx_CurrentEffort";
            this.tbx_CurrentEffort.ReadOnly = true;
            this.tbx_CurrentEffort.Size = new System.Drawing.Size(124, 20);
            this.tbx_CurrentEffort.TabIndex = 1;
            // 
            // tbx_Difference
            // 
            this.tbx_Difference.Location = new System.Drawing.Point(133, 78);
            this.tbx_Difference.Name = "tbx_Difference";
            this.tbx_Difference.ReadOnly = true;
            this.tbx_Difference.Size = new System.Drawing.Size(124, 20);
            this.tbx_Difference.TabIndex = 1;
            // 
            // ViewEvaluation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 181);
            this.Controls.Add(this.tlp_Evaluation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ViewEvaluation";
            this.Text = "Project evaluation";
            this.tlp_Evaluation.ResumeLayout(false);
            this.tlp_Evaluation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb_ProjectName;
        private System.Windows.Forms.Label lb_ProjectedEffort;
        private System.Windows.Forms.Label lb_CurrentEffort;
        private System.Windows.Forms.TableLayoutPanel tlp_Evaluation;
        private System.Windows.Forms.Label lb_Difference;
        private System.Windows.Forms.TextBox tbx_ProjectName;
        private System.Windows.Forms.TextBox tbx_ProjectedEffort;
        private System.Windows.Forms.TextBox tbx_CurrentEffort;
        private System.Windows.Forms.TextBox tbx_Difference;
        private System.Windows.Forms.Button btn_Close;
    }
}