namespace geeWiz.Forms
{
    partial class BaseListView
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
            ColumnHeader Key;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseListView));
            textFilter = new TextBox();
            listView = new ListView();
            btnCheckAll = new Button();
            btnUncheckAll = new Button();
            btnSelect = new Button();
            btnCancel = new Button();
            Key = new ColumnHeader();
            SuspendLayout();
            // 
            // textFilter
            // 
            textFilter.Anchor = AnchorStyles.Top;
            textFilter.Location = new System.Drawing.Point(18, 17);
            textFilter.Margin = new Padding(4, 3, 4, 3);
            textFilter.Name = "textFilter";
            textFilter.Size = new Size(507, 23);
            textFilter.TabIndex = 0;
            textFilter.TextChanged += txtFilter_TextChanged;
            // 
            // listView
            // 
            listView.Anchor = AnchorStyles.Top;
            listView.CheckBoxes = true;
            listView.FullRowSelect = true;
            listView.HeaderStyle = ColumnHeaderStyle.None;
            listView.Location = new System.Drawing.Point(18, 58);
            listView.Margin = new Padding(4, 3, 4, 3);
            listView.Name = "listView";
            listView.Size = new Size(507, 483);
            listView.TabIndex = 1;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = System.Windows.Forms.View.Details;
            // 
            // btnCheckAll
            // 
            btnCheckAll.Anchor = AnchorStyles.Bottom;
            btnCheckAll.Location = new System.Drawing.Point(192, 548);
            btnCheckAll.Margin = new Padding(4, 3, 4, 3);
            btnCheckAll.Name = "btnCheckAll";
            btnCheckAll.Size = new Size(158, 39);
            btnCheckAll.TabIndex = 2;
            btnCheckAll.Text = "Check all";
            btnCheckAll.UseVisualStyleBackColor = true;
            btnCheckAll.Click += btnCheckAll_Click;
            // 
            // btnUncheckAll
            // 
            btnUncheckAll.Anchor = AnchorStyles.Bottom;
            btnUncheckAll.Location = new System.Drawing.Point(368, 548);
            btnUncheckAll.Margin = new Padding(4, 3, 4, 3);
            btnUncheckAll.Name = "btnUncheckAll";
            btnUncheckAll.Size = new Size(158, 39);
            btnUncheckAll.TabIndex = 3;
            btnUncheckAll.Text = "Uncheck all";
            btnUncheckAll.UseVisualStyleBackColor = true;
            btnUncheckAll.Click += btnUncheckAll_Click;
            // 
            // btnSelect
            // 
            btnSelect.Anchor = AnchorStyles.Bottom;
            btnSelect.Location = new System.Drawing.Point(18, 594);
            btnSelect.Margin = new Padding(4, 3, 4, 3);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(507, 39);
            btnSelect.TabIndex = 4;
            btnSelect.Text = "Select";
            btnSelect.UseVisualStyleBackColor = true;
            btnSelect.Click += btnSelect_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom;
            btnCancel.Location = new System.Drawing.Point(18, 548);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(158, 39);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // SelectFromList
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(541, 647);
            Controls.Add(btnCancel);
            Controls.Add(btnSelect);
            Controls.Add(btnUncheckAll);
            Controls.Add(btnCheckAll);
            Controls.Add(listView);
            Controls.Add(textFilter);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            ImeMode = ImeMode.Off;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new Size(557, 686);
            MinimizeBox = false;
            MinimumSize = new Size(557, 686);
            Name = "SelectFromList";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Default Title";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textFilter;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnUncheckAll;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
    }
}