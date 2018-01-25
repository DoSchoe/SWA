namespace Client.Views
{
    partial class ViewAddProject
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
            this.tlp_Evaluation = new System.Windows.Forms.TableLayoutPanel();
            this.tbx_ProjectName = new System.Windows.Forms.TextBox();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.tbx_ProjectedEffort = new System.Windows.Forms.TextBox();
            this.tlp_Evaluation.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_ProjectName
            // 
            this.lb_ProjectName.Location = new System.Drawing.Point(3, 0);
            this.lb_ProjectName.Name = "lb_ProjectName";
            this.lb_ProjectName.Size = new System.Drawing.Size(84, 23);
            this.lb_ProjectName.TabIndex = 0;
            this.lb_ProjectName.Text = "Projectname:";
            this.lb_ProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_ProjectedEffort
            // 
            this.lb_ProjectedEffort.Location = new System.Drawing.Point(3, 25);
            this.lb_ProjectedEffort.Name = "lb_ProjectedEffort";
            this.lb_ProjectedEffort.Size = new System.Drawing.Size(84, 24);
            this.lb_ProjectedEffort.TabIndex = 0;
            this.lb_ProjectedEffort.Text = "Projected effort:";
            this.lb_ProjectedEffort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tlp_Evaluation
            // 
            this.tlp_Evaluation.ColumnCount = 3;
            this.tlp_Evaluation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlp_Evaluation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlp_Evaluation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlp_Evaluation.Controls.Add(this.lb_ProjectName, 0, 0);
            this.tlp_Evaluation.Controls.Add(this.lb_ProjectedEffort, 0, 1);
            this.tlp_Evaluation.Controls.Add(this.tbx_ProjectName, 1, 0);
            this.tlp_Evaluation.Controls.Add(this.btn_Cancel, 0, 3);
            this.tlp_Evaluation.Controls.Add(this.btn_OK, 2, 3);
            this.tlp_Evaluation.Controls.Add(this.tbx_ProjectedEffort, 1, 1);
            this.tlp_Evaluation.Location = new System.Drawing.Point(12, 12);
            this.tlp_Evaluation.Name = "tlp_Evaluation";
            this.tlp_Evaluation.RowCount = 4;
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlp_Evaluation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_Evaluation.Size = new System.Drawing.Size(354, 95);
            this.tlp_Evaluation.TabIndex = 1;
            // 
            // tbx_ProjectName
            // 
            this.tlp_Evaluation.SetColumnSpan(this.tbx_ProjectName, 2);
            this.tbx_ProjectName.Location = new System.Drawing.Point(93, 3);
            this.tbx_ProjectName.Name = "tbx_ProjectName";
            this.tbx_ProjectName.Size = new System.Drawing.Size(258, 20);
            this.tbx_ProjectName.TabIndex = 1;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(3, 63);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(84, 29);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.AutoSize = true;
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(267, 63);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(84, 29);
            this.btn_OK.TabIndex = 2;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // tbx_ProjectedEffort
            // 
            this.tbx_ProjectedEffort.Location = new System.Drawing.Point(93, 28);
            this.tbx_ProjectedEffort.Name = "tbx_ProjectedEffort";
            this.tbx_ProjectedEffort.Size = new System.Drawing.Size(65, 20);
            this.tbx_ProjectedEffort.TabIndex = 1;
            // 
            // ViewAddProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(378, 119);
            this.Controls.Add(this.tlp_Evaluation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ViewAddProject";
            this.Text = "Add project";
            this.tlp_Evaluation.ResumeLayout(false);
            this.tlp_Evaluation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb_ProjectName;
        private System.Windows.Forms.Label lb_ProjectedEffort;
        private System.Windows.Forms.TableLayoutPanel tlp_Evaluation;
        private System.Windows.Forms.TextBox tbx_ProjectName;
        private System.Windows.Forms.TextBox tbx_ProjectedEffort;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
    }
}