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
        private MRUManager _mruManager;

        public List<AEGISCompletionData> AutoCompleteObjects;

        public MainForm()
        {
            InitializeComponent();
            Singleton = this;

            this.Location = Properties.Settings.Default.Location;
            if (!Properties.Settings.Default.Size.IsEmpty)
                this.Size = Properties.Settings.Default.Size;

            _mruManager = new MRUManager(mRUToolStripMenuItem, mruManager_MruOpenEvent);
            _mruManager.RegistryKeyName = "Ragnarok NPC Editor";
            _mruManager.LoadFromRegistry();

            this.Text = "Ragnarok NPC Editor v" + Config.GetVersion + " (beta)";

            NpcList = new NPCList();
            NpcList.Show(dockPanel, DockState.DockLeft);

            FillAutoCompleteList();
        }

        private void FillAutoCompleteList()
        {
            AutoCompleteObjects = new List<AEGISCompletionData>();

            // itp.def
            foreach (var kvp in ReadDefList("itp.def"))
                AutoCompleteObjects.Add(new AEGISCompletionData(kvp.Value, kvp.Value, (int)AutoCompletionImageType.Item, ""));

            // mobname.def
            foreach (var kvp in ReadDefList("mobname.def"))
                AutoCompleteObjects.Add(new AEGISCompletionData(kvp.Value, kvp.Value, (int)AutoCompletionImageType.Monster, ""));

            // skill.def
            foreach (var kvp in ReadDefList("skill.def"))
                AutoCompleteObjects.Add(new AEGISCompletionData(kvp.Value, kvp.Value, (int)AutoCompletionImageType.Skill, ""));

            // std.def
            foreach (var kvp in ReadDefList("std.def"))
                AutoCompleteObjects.Add(new AEGISCompletionData(kvp.Value, kvp.Value, (int)AutoCompletionImageType.Script, ""));
        }

        private KeyValuePair<int, string>[] ReadDefList(string filename)
        {
            var ret = new List<KeyValuePair<int, string>>();
            var fi = new FileInfo(Path.Combine("Data", filename));
            if (fi.Exists)
            {
                using (var fs = new FileStream(fi.FullName, FileMode.Open))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        while (true)
                        {
                            var line = sr.ReadLine();
                            if (line == null) break;
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            var data = line.Split(Convert.ToChar(" "));
                            ret.Add(new KeyValuePair<int, string>(Convert.ToInt32(data[1]), data[0]));
                        }
                    }
                }
            }
            return ret.ToArray();
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
                LoadFile(openFileDialog.FileName);
            }
        }

        private void mruManager_MruOpenEvent(int number, string filename)
        {
            LoadFile(filename);
        }

        private void LoadFile(string filename)
        {
            _mruManager.AddFile(filename);
            var f = new ScriptFile();
            f.LoadFile(filename);
            f.MdiParent = this;
            f.Show(dockPanel);
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

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            lock (NpcList)
            {
                ActiveDocument = (ScriptFile)((DockPanel)sender).ActiveDocument;
                if (ActiveDocument != null)
                    NpcList.NpcsChanged(ActiveDocument.Npcs);
            }
        }

        private void ChangeEncodingDefault(object sender, EventArgs e)
        {
            ChangeEncoding("ISO-8859-1");
        }

        private void ChangeEncodingKorean(object sender, EventArgs e)
        {
            ChangeEncoding("EUC-KR");
        }

        private void ChangeEncodingTaiwan(object sender, EventArgs e)
        {
            ChangeEncoding("BIG5");
        }

        private void ChangeEncodingChina(object sender, EventArgs e)
        {
            ChangeEncoding("GBK");
        }

        private void ChangeEncodingThai(object sender, EventArgs e)
        {
            ChangeEncoding("Windows-874");
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

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (ActiveDocument == null) return;
            ActiveDocument.Indent();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Location = this.Location;
            Properties.Settings.Default.Size = this.Size;
            Properties.Settings.Default.Save();
            _mruManager.SaveToRegistry();
        }
    }
}
