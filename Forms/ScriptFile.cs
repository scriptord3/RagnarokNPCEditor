using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using RagnarokNpcEditor.Classes;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using WeifenLuo.WinFormsUI.Docking;

namespace RagnarokNpcEditor
{
    public partial class ScriptFile : DockContent
    {
        public List<NpcScriptLocation> Npcs;

        private string _filename = string.Empty;
        private string _text = string.Empty;
        private Encoding _encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
        private bool _hasChanges;

        public ScriptFile()
        {
            InitializeComponent();
            Npcs = new List<NpcScriptLocation>();
            //ICSharpCode.TextEditor.Document.FileSyntaxModeProvider fsmProvider;
            //fsmProvider = new ICSharpCode.TextEditor.Document.FileSyntaxModeProvider(Directory.GetCurrentDirectory());
            //ICSharpCode.TextEditor.Document.HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider);
            ICSharpCode.TextEditor.Document.HighlightingManager.Manager.AddSyntaxModeFileProvider(new AppSyntaxModeProvider());
            txtCode.SetHighlighting("AEGIS");
            txtCode.Document.FoldingManager.FoldingStrategy = new RegionFoldingStrategy();
            txtCode.IsReadOnly = false;
        }

        public void NavigateToLine(int index)
        {
            var pos = txtCode.Document.OffsetToPosition(index);
            txtCode.ActiveTextAreaControl.Caret.Position = pos;
            txtCode.ActiveTextAreaControl.ScrollToCaret();
        }

        public void LoadFile(string filename)
        {
            _filename = filename;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                using (var fs = new FileStream(filename, FileMode.Open))
                {
                    using (var sr = new StreamReader(fs, System.Text.Encoding.GetEncoding("ISO-8859-1")))
                    {
                        var file = sr.ReadToEnd();
                        _text = file;
                        txtCode.Text = _text;
                        this.Text = Path.GetFileName(_filename);
                        LoadNpcs();
                    }
                    _hasChanges = false;
                    txtCode.Document.FoldingManager.UpdateFoldings(null, null);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }



        public void SaveFile()
        {
            // already saved?
            if (string.IsNullOrEmpty(_filename))
            {
                SaveAsFile();
                return;
            }
            try
            {
                this.Cursor = Cursors.WaitCursor;
                using (var fs = new FileStream(_filename, FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs, _encoding))
                    {
                        sw.Write(txtCode.Text);
                    }
                }
                _hasChanges = false;
                this.Text = Path.GetFileName(_filename);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void SaveAsFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Script file (*.sc)|*.sc";
            saveFileDialog.InitialDirectory = Properties.Settings.Default.LastFolder;
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _filename = saveFileDialog.FileName;
                this.Text = Path.GetFileName(_filename);
                SaveFile();
            }
        }

        public bool HaveSelection()
        {
            var editor = txtCode;
            return editor != null &&
                editor.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected;
        }

        public void DoEditAction(ICSharpCode.TextEditor.Actions.IEditAction action)
        {
            if (txtCode != null && action != null)
            {
                var area = txtCode.ActiveTextAreaControl.TextArea;
                txtCode.BeginUpdate();
                try
                {
                    lock (txtCode.Document)
                    {
                        action.Execute(area);
                        if (area.SelectionManager.HasSomethingSelected && area.AutoClearSelection /*&& caretchanged*/)
                        {
                            if (area.Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal)
                            {
                                area.SelectionManager.ClearSelection();
                            }
                        }
                    }
                }
                finally
                {
                    txtCode.EndUpdate();
                    area.Caret.UpdateCaretPosition();
                }
            }
        }

        public void SetEncoding(Encoding encoding)
        {
            _text = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(_encoding.GetBytes(txtCode.Text));
            _encoding = encoding;
            txtCode.Text = _encoding.GetString(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_text));
            LoadNpcs();
        }

        private void LoadNpcs()
        {
            Npcs.Clear();
            var rx = new Regex(@"npc ""(.*)"" ""(.*)"" (.*) ([0-9]*) ([0-9]*) ([0-9]*) ([0-9]*) ([0-9]*)");
            var matches = rx.Matches(txtCode.Text);
            foreach (Match match in matches)
            {
                var text = string.Format("{0} ({1} {2}, {3})", match.Groups[2].Value, match.Groups[1].Value, match.Groups[4].Value, match.Groups[5].Value);
                Npcs.Add(new NpcScriptLocation() { Text = text, Type = "npc", Index = match.Index });
            }
            MainForm.Singleton.NpcList.NpcsChanged(Npcs);
        }

        private void ScriptFile_Load(object sender, EventArgs e)
        {

        }

        private void txtCode_Load(object sender, EventArgs e)
        {

        }

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            _hasChanges = true;
            this.Text = Path.GetFileName(_filename) + "*";
        }
    }

    /// <summary>
    /// The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
    /// </summary>
    public class RegionFoldingStrategy : IFoldingStrategy
    {
        /// <summary>
        /// Generates the foldings for our document.
        /// </summary>
        /// <param name="document">The current document.</param>
        /// <param name="fileName">The filename of the document.</param>
        /// <param name="parseInformation">Extra parse information, not used in this sample.</param>
        /// <returns>A list of FoldMarkers.</returns>
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            List<FoldMarker> list = new List<FoldMarker>();

            Stack<int> startLines = new Stack<int>();

            // Create foldmarkers for the whole document, enumerate through every line.
            for (int i = 0; i < document.TotalNumberOfLines; i++)
            {
                var seg = document.GetLineSegment(i);
                int offs, end = document.TextLength;
                char c;
                for (offs = seg.Offset; offs < end && ((c = document.GetCharAt(offs)) == ' ' || c == '\t'); offs++)
                { }
                if (offs == end)
                    break;
                int spaceCount = offs - seg.Offset;

                // now offs points to the first non-whitespace char on the line
                //if (document.GetCharAt(offs) == 'O' || document.GetCharAt(offs) == 'r')
                //{
                string text = document.GetText(offs, seg.Length - spaceCount);

                if (text.StartsWith("On") || text.StartsWith("if") || text.StartsWith("choose") || text.StartsWith("case"))
                    startLines.Push(i);

                if ((text.StartsWith("return") || text.StartsWith("endif") || text.StartsWith("endchoose") || text.StartsWith("break")) && startLines.Count > 0)
                {
                    // Add a new FoldMarker to the list.
                    int start = startLines.Pop();
                    list.Add(new FoldMarker(document, start,
                        document.GetLineSegment(start).Length,
                        i, spaceCount + "return".Length));
                }
                //}
            }

            return list;
        }
    }
}
