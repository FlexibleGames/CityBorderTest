namespace CityBorderTest
{
    partial class BorderTestForm
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelBorder = new System.Windows.Forms.Panel();
            this.comboCitySelect = new System.Windows.Forms.ComboBox();
            this.panelBorder2 = new System.Windows.Forms.Panel();
            this.btnAddOwner = new System.Windows.Forms.Button();
            this.cbAddingOwner = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Location = new System.Drawing.Point(12, 40);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(260, 238);
            this.panelMain.TabIndex = 0;
            this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
            this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDown);
            this.panelMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseMove);
            // 
            // panelBorder
            // 
            this.panelBorder.Location = new System.Drawing.Point(279, 40);
            this.panelBorder.Name = "panelBorder";
            this.panelBorder.Size = new System.Drawing.Size(200, 100);
            this.panelBorder.TabIndex = 1;
            // 
            // comboCitySelect
            // 
            this.comboCitySelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCitySelect.FormattingEnabled = true;
            this.comboCitySelect.Items.AddRange(new object[] {
            "City 1",
            "City 2"});
            this.comboCitySelect.Location = new System.Drawing.Point(12, 13);
            this.comboCitySelect.Name = "comboCitySelect";
            this.comboCitySelect.Size = new System.Drawing.Size(116, 21);
            this.comboCitySelect.TabIndex = 2;
            this.comboCitySelect.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboCitySelect_KeyDown);
            // 
            // panelBorder2
            // 
            this.panelBorder2.Location = new System.Drawing.Point(279, 146);
            this.panelBorder2.Name = "panelBorder2";
            this.panelBorder2.Size = new System.Drawing.Size(200, 100);
            this.panelBorder2.TabIndex = 3;
            // 
            // btnAddOwner
            // 
            this.btnAddOwner.Location = new System.Drawing.Point(135, 11);
            this.btnAddOwner.Name = "btnAddOwner";
            this.btnAddOwner.Size = new System.Drawing.Size(137, 23);
            this.btnAddOwner.TabIndex = 4;
            this.btnAddOwner.Text = "Add Owner";
            this.btnAddOwner.UseVisualStyleBackColor = true;
            this.btnAddOwner.Click += new System.EventHandler(this.btnAddOwner_Click);
            this.btnAddOwner.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnAddOwner_KeyDown);
            // 
            // cbAddingOwner
            // 
            this.cbAddingOwner.AutoSize = true;
            this.cbAddingOwner.Enabled = false;
            this.cbAddingOwner.Location = new System.Drawing.Point(148, 16);
            this.cbAddingOwner.Name = "cbAddingOwner";
            this.cbAddingOwner.Size = new System.Drawing.Size(15, 14);
            this.cbAddingOwner.TabIndex = 5;
            this.cbAddingOwner.Tag = "is_add_owner";
            this.cbAddingOwner.UseVisualStyleBackColor = true;
            this.cbAddingOwner.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbAddingOwner_KeyDown);
            // 
            // BorderTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(851, 614);
            this.Controls.Add(this.cbAddingOwner);
            this.Controls.Add(this.btnAddOwner);
            this.Controls.Add(this.panelBorder2);
            this.Controls.Add(this.comboCitySelect);
            this.Controls.Add(this.panelBorder);
            this.Controls.Add(this.panelMain);
            this.Name = "BorderTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "";
            this.Text = "Border Test";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BorderTestForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BorderTestForm_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BorderTestForm_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelBorder;
        private System.Windows.Forms.ComboBox comboCitySelect;
        private System.Windows.Forms.Panel panelBorder2;
        private System.Windows.Forms.Button btnAddOwner;
        private System.Windows.Forms.CheckBox cbAddingOwner;
    }
}

