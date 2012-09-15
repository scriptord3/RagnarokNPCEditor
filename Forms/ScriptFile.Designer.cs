namespace RagnarokNpcEditor
{
    partial class ScriptFile
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptFile));
            this.txtCode = new ICSharpCode.TextEditor.TextEditorControl();
            this.AutoCompleteImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // txtCode
            // 
            this.txtCode.ConvertTabsToSpaces = true;
            this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCode.IsReadOnly = false;
            this.txtCode.Location = new System.Drawing.Point(0, 0);
            this.txtCode.Name = "txtCode";
            this.txtCode.ShowVRuler = false;
            this.txtCode.Size = new System.Drawing.Size(472, 408);
            this.txtCode.TabIndent = 2;
            this.txtCode.TabIndex = 0;
            // 
            // AutoCompleteImageList
            // 
            this.AutoCompleteImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("AutoCompleteImageList.ImageStream")));
            this.AutoCompleteImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.AutoCompleteImageList.Images.SetKeyName(0, "function.png");
            this.AutoCompleteImageList.Images.SetKeyName(1, "poring.gif");
            this.AutoCompleteImageList.Images.SetKeyName(2, "item.png");
            this.AutoCompleteImageList.Images.SetKeyName(3, "391.png");
            // 
            // ScriptFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 408);
            this.Controls.Add(this.txtCode);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScriptFile";
            this.Text = "ScriptFile";
            this.ResumeLayout(false);

        }

        #endregion

        private ICSharpCode.TextEditor.TextEditorControl txtCode;
        public System.Windows.Forms.ImageList AutoCompleteImageList;
    }
}