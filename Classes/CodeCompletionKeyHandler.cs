using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace RagnarokNpcEditor.Classes
{
    public class CodeCompletionKeyHandler
    {
        ScriptFile mainForm;
        TextEditorControl editor;
        CodeCompletionWindow codeCompletionWindow;

        private CodeCompletionKeyHandler(ScriptFile mainForm, TextEditorControl editor)
        {
            this.mainForm = mainForm;
            this.editor = editor;
        }

        public static CodeCompletionKeyHandler Attach(ScriptFile mainForm, TextEditorControl editor)
        {
            CodeCompletionKeyHandler h = new CodeCompletionKeyHandler(mainForm, editor);

            //editor.ActiveTextAreaControl.TextArea.KeyEventHandler += h.TextAreaKeyEventHandler;
            editor.ActiveTextAreaControl.TextArea.KeyDown += h.TextAreaKeyEventHandler;
            // When the editor is disposed, close the code completion window
            editor.Disposed += h.CloseCodeCompletionWindow;

            return h;
        }

        /// <summary>
        /// Return true to handle the keypress, return false to let the text area handle the keypress
        /// </summary>
        void TextAreaKeyEventHandler(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.OemOpenBrackets && e.Shift)
            {
                ICompletionDataProvider snippetDataProvider = new SnippetDataProvider(mainForm.AutoCompleteImageList);
                codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
                    mainForm,					// The parent window for the completion window
                    editor, 					// The text editor to show the window for
                    "",		// Filename - will be passed back to the provider
                    snippetDataProvider,		// Provider to get the list of possible completions
                    Convert.ToChar(e.KeyCode)	    					// Key pressed - will be passed to the provider
                );
            }
            else
            {
                ICompletionDataProvider completionDataProvider = new CodeCompletionProvider(mainForm.AutoCompleteImageList);
                codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
                    mainForm,					// The parent window for the completion window
                    editor, 					// The text editor to show the window for
                    "",		// Filename - will be passed back to the provider
                    completionDataProvider,		// Provider to get the list of possible completions
                    Convert.ToChar(e.KeyCode)	    					// Key pressed - will be passed to the provider
                );
            }
            if (codeCompletionWindow != null)
            {
                // ShowCompletionWindow can return null when the provider returns an empty list
                codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
            }
            return;
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
    }
}
