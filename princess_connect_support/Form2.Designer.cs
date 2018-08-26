namespace princess_connect_support
{
    partial class Form2
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
            this.new_equitment_but = new System.Windows.Forms.Button();
            this.new_equitment_tb = new System.Windows.Forms.TextBox();
            this.new_equitment_label = new System.Windows.Forms.Label();
            this.save_but = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // new_equitment_but
            // 
            this.new_equitment_but.Location = new System.Drawing.Point(708, 491);
            this.new_equitment_but.Name = "new_equitment_but";
            this.new_equitment_but.Size = new System.Drawing.Size(75, 23);
            this.new_equitment_but.TabIndex = 0;
            this.new_equitment_but.Text = "新增";
            this.new_equitment_but.UseVisualStyleBackColor = true;
            this.new_equitment_but.Click += new System.EventHandler(this.new_equitment_but_Click);
            // 
            // new_equitment_tb
            // 
            this.new_equitment_tb.Location = new System.Drawing.Point(602, 493);
            this.new_equitment_tb.Name = "new_equitment_tb";
            this.new_equitment_tb.Size = new System.Drawing.Size(100, 22);
            this.new_equitment_tb.TabIndex = 1;
            // 
            // new_equitment_label
            // 
            this.new_equitment_label.AutoSize = true;
            this.new_equitment_label.Location = new System.Drawing.Point(536, 496);
            this.new_equitment_label.Name = "new_equitment_label";
            this.new_equitment_label.Size = new System.Drawing.Size(53, 12);
            this.new_equitment_label.TabIndex = 2;
            this.new_equitment_label.Text = "新增裝備";
            // 
            // save_but
            // 
            this.save_but.Location = new System.Drawing.Point(708, 462);
            this.save_but.Name = "save_but";
            this.save_but.Size = new System.Drawing.Size(75, 23);
            this.save_but.TabIndex = 3;
            this.save_but.Text = "儲存";
            this.save_but.UseVisualStyleBackColor = true;
            this.save_but.Click += new System.EventHandler(this.save_but_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 526);
            this.Controls.Add(this.save_but);
            this.Controls.Add(this.new_equitment_label);
            this.Controls.Add(this.new_equitment_tb);
            this.Controls.Add(this.new_equitment_but);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button new_equitment_but;
        private System.Windows.Forms.TextBox new_equitment_tb;
        private System.Windows.Forms.Label new_equitment_label;
        private System.Windows.Forms.Button save_but;
    }
}