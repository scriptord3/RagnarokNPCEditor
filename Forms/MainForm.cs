using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using RagnarokNpcEditor.Classes;

namespace RagnarokNpcEditor
{
    public partial class MainForm : Form
    {
        private int childFormNumber = 0;

        public static MainForm Singleton;

        public ScriptFile ActiveDocument;
        public NPCList NpcList;

        public MainForm()
        {
            InitializeComponent();
            Singleton = this;

            this.Text = "Ragnarok NPC Editor v" + Config.GetVersion + " (beta)";

            NpcList = new NPCList();
            NpcList.Show(dockPanel, DockState.DockLeft);
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            var childForm = new ScriptFile();
            childForm.MdiParent = this;
            childForm.Text = "Script " + childFormNumber++;
            childForm.Show(dockPanel);
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Script file (*.sc)|*.sc";
            openFileDialog.InitialDirectory = Properties.Settings.Default.LastFolder;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Properties.Settings.Default.LastFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                var f = new ScriptFile();
                f.LoadFile(openFileDialog.FileName);
                f.MdiParent = this;
                f.Show(dockPanel);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.SaveAsFile();            
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.SaveFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.SaveFile();
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument == null) return;
            if (ActiveDocument.HaveSelection())
                ActiveDocument.DoEditAction(new ICSharpCode.TextEditor.Actions.Cut());
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument == null) return;
            if (ActiveDocument.HaveSelection())
                ActiveDocument.DoEditAction(new ICSharpCode.TextEditor.Actions.Copy());
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveDocument == null) return;
            ActiveDocument.DoEditAction(new ICSharpCode.TextEditor.Actions.Paste());
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            lock (NpcList)
            {
                ActiveDocument = (ScriptFile)((DockPanel)sender).ActiveDocument;
                if (ActiveDocument != null)
                    NpcList.NpcsChanged(ActiveDocument.Npcs);
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ChangeEncoding("EUC-KR");
        }

        private void koreanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeEncoding("EUC-KR");
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ChangeEncoding("BIG5");
        }

        private void chineseBig5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeEncoding("BIG5");
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ChangeEncoding("ISO-8859-1");
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeEncoding("ISO-8859-1");
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ChangeEncoding("GBK");
        }

        private void chineseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeEncoding("GBK");
        }

        private void ChangeEncoding(string encoding)
        {
            if (ActiveDocument == null) return;
            var e = System.Text.Encoding.GetEncoding(encoding);
            ActiveDocument.SetEncoding(e);
            tsEncoding.Text = e.EncodingName;
        }

        private void dockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            if (dockPanel.Documents.Count() == 0)
            {
                ActiveDocument = null;
                NpcList.ClearList();
            }
        }
    }
}
