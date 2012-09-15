using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace RagnarokNpcEditor.Classes
{
    public class CodeCompletionProvider : ICompletionDataProvider
    {
        private ImageList imageList;

        public CodeCompletionProvider(ImageList imageList)
        {
            this.imageList = imageList;
        }

        public ImageList ImageList
        {
            get
            {
                return imageList;
            }
        }

        public string PreSelection
        {
            get
            {
                return null;
            }
        }

        public int DefaultIndex
        {
            get
            {
                return 0;
            }
        }

        public CompletionDataProviderKeyResult ProcessKey(char key)
        {
            if (char.IsLetterOrDigit(key) || key == '_')
            {
                return CompletionDataProviderKeyResult.NormalKey;
            }
            return CompletionDataProviderKeyResult.InsertionKey;
        }

        /// <summary>
        /// Called when entry should be inserted. Forward to the insertion action of the completion data.
        /// </summary>
        public bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
        {
            textArea.SelectionManager.SetSelection(textArea.Document.OffsetToPosition(
                Math.Min(insertionOffset - ((AEGISCompletionData)data).Expression.Length, textArea.Document.TextLength)
                ), textArea.Caret.Position);
            return data.InsertAction(textArea, key);
        }

        public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
        {
            var expr = charTyped.ToString();
            for (var i = textArea.Document.PositionToOffset(textArea.Caret.Position) - 1; i >= 0; i--)
            {
                var c = textArea.Document.GetCharAt(i);
                if (string.IsNullOrWhiteSpace(c.ToString())) break;
                expr = c + expr;
            }

            var l = new List<ICompletionData>();
            foreach (var c in MainForm.Singleton.AutoCompleteObjects.Where(o => o.Text.ToLower().StartsWith(expr.ToLower())))
            {
                c.Expression = expr;
                l.Add(c);
            }
            if (l.Count == 0) return null;
            return l.ToArray();
        }

        //ExpressionResult FindExpression(TextArea textArea)
        //{
        //    ICSharpCode.SharpDevelop.Dom.IExpressionFinder
        //    //Dom.CSharp.CSharpExpressionFinder finder;
        //    SQL.SQLExpressionFinder finder;
        //    finder = new Dom.SQL.SQLExpressionFinder(MainForm.DummyFileName);
        //    return finder.FindExpression(textArea.Document.TextContent, textArea.Caret.Offset);
        //}
    }
}
