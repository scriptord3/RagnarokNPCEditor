using System;
using System.Diagnostics;
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
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using WeifenLuo.WinFormsUI.Docking;

namespace RagnarokNpcEditor
{
    public partial class ScriptFile : DockContent
    {
        public List<NpcScriptLocation> Npcs;
        private CodeCompletionWindow codeCompletionWindow;

        private string _filename = string.Empty;
        private string _text = string.Empty;
        private Encoding _encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
        private bool _hasChanges = false;

        public ScriptFile()
        {
            InitializeComponent();
            Npcs = new List<NpcScriptLocation>();
            ICSharpCode.TextEditor.Document.HighlightingManager.Manager.AddSyntaxModeFileProvider(new AppSyntaxModeProvider());
            txtCode.SetHighlighting("AEGIS");
            txtCode.Document.FoldingManager.FoldingStrategy = new RegionFoldingStrategy();
            txtCode.Document.DocumentChanged += txtCode_TextChanged;
            CodeCompletionKeyHandler.Attach(this, txtCode);
            txtCode.IsReadOnly = false;
        }

        private string GetLastWord()
        {
            var search = string.Empty;
            for (var i = txtCode.Document.PositionToOffset(txtCode.ActiveTextAreaControl.Caret.Position) - 1; i >= 0; i--)
            {
                var c = txtCode.Document.GetCharAt(i);
                if (string.IsNullOrWhiteSpace(c.ToString())) break;
                search = c + search;
            }
            return search;
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
                        file = file.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
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


        private void ShowIntellisense()
        {
            ICompletionDataProvider completionDataProvider = new CodeCompletionProvider(this.AutoCompleteImageList);

            var x = CodeCompletionWindow.ShowCompletionWindow(
                this,                // The parent window for the completion window
                txtCode, 	     // The text editor to show the window for
                "",	       	     // Filename - will be passed back to the provider
                completionDataProvider,// Provider to get the list of possible completions
                ' '		     // Key pressed - will be passed to the provider
                );

            if (codeCompletionWindow != null)
            {
                // ShowCompletionWindow can return null when the provider returns an empty list
                codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
            }
        }

        void CloseCodeCompletionWindow(object sender, EventArgs e)
        {
            if (codeCompletionWindow != null)
            {
                codeCompletionWindow.Closed -= new EventHandler(CloseCodeCompletionWindow);
                codeCompletionWindow.Dispose();
                codeCompletionWindow = null;
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
                        var text = txtCode.Text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
                        if (Config.ReplaceTabWithSpace())
                            text = text.Replace("\t", String.Concat(Enumerable.Repeat(" ", Config.Tabsize())));
                        sw.Write(text);
                    }
                }
                _hasChanges = false;
                txtCode.Document.FoldingManager.UpdateFoldings(null, null);
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
            txtCode.Document.FoldingManager.UpdateFoldings(null, null);
        }

        public void Indent()
        {
            _text = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(_encoding.GetBytes(txtCode.Text));
            _text = ScriptIdenter.Ident(_text);
            txtCode.Text = _encoding.GetString(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_text));
            LoadNpcs();
            txtCode.Document.FoldingManager.UpdateFoldings(null, null);
        }

        public void Compile()
        {
            if (CheckForMissingQuotes())
                MessageBox.Show("Compilation successful.", "Compiler", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool CheckForMissingQuotes()
        {
            for (int i = 0; i < txtCode.Document.TotalNumberOfLines; i++)
            {
                var seg = txtCode.Document.GetLineSegment(i);
                int offs, end = txtCode.Document.TextLength;
                char c;
                for (offs = seg.Offset; offs < end && ((c = txtCode.Document.GetCharAt(offs)) == ' ' || c == '\t'); offs++)
                { }
                if (offs == end)
                    break;
                int spaceCount = offs - seg.Offset;
                string text = txtCode.Document.GetText(offs, seg.Length - spaceCount);
                var amount = text.Count(f => f == '"');
                //Debug.Print("{0}", amount % 2);
                if (amount % 2 == 1)
                {
                    MessageBox.Show(string.Format("Incorrect number of quotes (\") on line {0}", i + 1), "Compiler", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtCode.ActiveTextAreaControl.Caret.Position = new TextLocation(txtCode.ActiveTextAreaControl.Caret.Position.X, i);
                    txtCode.ActiveTextAreaControl.ScrollToCaret();
                    return false;
                }
            }
            return true;
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

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            _hasChanges = true;
            this.Text = Path.GetFileName(_filename) + "*";
            txtCode.Document.FoldingManager.UpdateFoldings(null, null);
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

            Stack<Tuple<int, string>> startLines = new Stack<Tuple<int, string>>();
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

                if (text.StartsWith("OnTimer") || text.StartsWith("event On") || text.StartsWith("if") || text.StartsWith("choose") || text.StartsWith("case"))
                    startLines.Push(new Tuple<int, string>(i, text));

                if ((text.StartsWith("return") || text.StartsWith("endif") || text.StartsWith("endchoose") || text.StartsWith("break")) && startLines.Count > 0)
                {
                    Tuple<int, string> start = startLines.Pop();
                    switch (text)
                    {
                        case "endif":
                            if (!start.Item2.StartsWith("if"))
                            {
                                startLines.Push(start);
                                continue;
                            }
                            break;
                        case "endchoose":
                            if (!start.Item2.StartsWith("choose"))
                            {
                                startLines.Push(start);
                                continue;
                            }
                            break;
                        case "break":
                            if (!start.Item2.StartsWith("case"))
                            {
                                startLines.Push(start);
                                continue;
                            }
                            break;
                        default:
                            if (start.Item2.StartsWith("if") || start.Item2.StartsWith("choose") || start.Item2.StartsWith("case"))
                            {
                                startLines.Push(start);
                                continue;
                            }
                            break;
                    }
                    list.Add(new FoldMarker(document, start.Item1,
                        document.GetLineSegment(start.Item1).Length,
                        i, spaceCount + text.Length));
                }
                //}
            }

            return list;
        }
    }
}
