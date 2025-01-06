namespace geeWiz.Forms
{
    partial class BaseEnterValue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseEnterValue));
            labelTooltip = new Label();
            textBox = new TextBox();
            btnCancel = new Button();
            btnSelect = new Button();
            SuspendLayout();
            // 
            // labelTooltip
            // 
            labelTooltip.AutoSize = true;
            labelTooltip.Location = new System.Drawing.Point(18, 17);
            labelTooltip.Margin = new Padding(4, 0, 4, 0);
            labelTooltip.MaximumSize = new Size(507, 0);
            labelTooltip.MinimumSize = new Size(507, 0);
            labelTooltip.Name = "labelTooltip";
            labelTooltip.Size = new Size(507, 15);
            labelTooltip.TabIndex = 0;
            labelTooltip.Text = "Default Message";
            // 
            // textBox
            // 
            textBox.Location = new System.Drawing.Point(18, 52);
            textBox.Margin = new Padding(4, 3, 4, 3);
            textBox.MaxLength = 100;
            textBox.Name = "textBox";
            textBox.Size = new Size(507, 23);
            textBox.TabIndex = 1;
            textBox.WordWrap = false;
            textBox.KeyPress += TextBox_KeyPress;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(350, 110);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(175, 39);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSelect
            // 
            btnSelect.Location = new System.Drawing.Point(163, 110);
            btnSelect.Margin = new Padding(4, 3, 4, 3);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(175, 39);
            btnSelect.TabIndex = 3;
            btnSelect.Text = "OK";
            btnSelect.UseVisualStyleBackColor = true;
            btnSelect.Click += btnSelect_Click;
            // 
            // EnterValue
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(541, 163);
            Controls.Add(btnSelect);
            Controls.Add(btnCancel);
            Controls.Add(textBox);
            Controls.Add(labelTooltip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            ImeMode = ImeMode.Off;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new Size(557, 202);
            MinimizeBox = false;
            MinimumSize = new Size(557, 202);
            Name = "EnterValue";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Default Title";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelTooltip;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelect;
    }
}