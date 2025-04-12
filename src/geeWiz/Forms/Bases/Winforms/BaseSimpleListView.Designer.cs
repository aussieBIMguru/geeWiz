namespace geeWiz.Forms.Bases
{
    partial class BaseSimpleListView
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
            Key = new ColumnHeader();
            listView = new ListView();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnSelect = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnUncheckAll = new Button();
            btnCheckAll = new Button();
            btnCancel = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // listView
            // 
            listView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView.CheckBoxes = true;
            listView.FullRowSelect = true;
            listView.HeaderStyle = ColumnHeaderStyle.None;
            listView.Location = new System.Drawing.Point(16, 16);
            listView.Margin = new Padding(16, 16, 16, 4);
            listView.Name = "listView";
            listView.Size = new Size(510, 519);
            listView.TabIndex = 1;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = System.Windows.Forms.View.Details;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(listView, 0, 0);
            tableLayoutPanel1.Controls.Add(btnSelect, 0, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 1);
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tableLayoutPanel1.Size = new Size(542, 647);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // btnSelect
            // 
            btnSelect.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSelect.AutoSize = true;
            btnSelect.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSelect.Location = new System.Drawing.Point(16, 591);
            btnSelect.Margin = new Padding(16, 4, 16, 16);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(510, 40);
            btnSelect.TabIndex = 5;
            btnSelect.Text = "Select";
            btnSelect.UseVisualStyleBackColor = true;
            btnSelect.Click += btnSelect_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel3.AutoSize = true;
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.Controls.Add(btnUncheckAll, 2, 0);
            tableLayoutPanel3.Controls.Add(btnCheckAll, 1, 0);
            tableLayoutPanel3.Controls.Add(btnCancel, 0, 0);
            tableLayoutPanel3.Location = new System.Drawing.Point(16, 543);
            tableLayoutPanel3.Margin = new Padding(16, 4, 16, 4);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.Size = new Size(510, 40);
            tableLayoutPanel3.TabIndex = 8;
            // 
            // btnUncheckAll
            // 
            btnUncheckAll.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnUncheckAll.AutoSize = true;
            btnUncheckAll.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnUncheckAll.Location = new System.Drawing.Point(344, 0);
            btnUncheckAll.Margin = new Padding(4, 0, 0, 0);
            btnUncheckAll.MaximumSize = new Size(0, 40);
            btnUncheckAll.Name = "btnUncheckAll";
            btnUncheckAll.Size = new Size(166, 40);
            btnUncheckAll.TabIndex = 4;
            btnUncheckAll.Text = "Uncheck all";
            btnUncheckAll.UseVisualStyleBackColor = true;
            btnUncheckAll.Click += btnUncheckAll_Click;
            // 
            // btnCheckAll
            // 
            btnCheckAll.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCheckAll.AutoSize = true;
            btnCheckAll.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnCheckAll.Location = new System.Drawing.Point(174, 0);
            btnCheckAll.Margin = new Padding(4, 0, 4, 0);
            btnCheckAll.MaximumSize = new Size(0, 40);
            btnCheckAll.Name = "btnCheckAll";
            btnCheckAll.Size = new Size(162, 40);
            btnCheckAll.TabIndex = 3;
            btnCheckAll.Text = "Check all";
            btnCheckAll.UseVisualStyleBackColor = true;
            btnCheckAll.Click += btnCheckAll_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCancel.AutoSize = true;
            btnCancel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnCancel.Location = new System.Drawing.Point(0, 0);
            btnCancel.Margin = new Padding(0, 0, 4, 0);
            btnCancel.MaximumSize = new Size(0, 40);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(166, 40);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // BaseSimpleListView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(541, 647);
            Controls.Add(tableLayoutPanel1);
            ImeMode = ImeMode.Off;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(557, 686);
            Name = "BaseSimpleListView";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Default Title";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.ListView listView;
        private ColumnHeader Key;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnSelect;
        private TableLayoutPanel tableLayoutPanel3;
        private Button btnUncheckAll;
        private Button btnCheckAll;
        private Button btnCancel;
    }
}