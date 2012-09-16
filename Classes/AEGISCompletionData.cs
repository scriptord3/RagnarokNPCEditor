using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace RagnarokNpcEditor.Classes
{
	public class AEGISCompletionData : ICompletionData
	{
		string text;
		string description;
        int imageIndex;
        string expression;
        eType type;
		
        public enum eType
        {
            Intellisense,
            Snippet
        }

        public eType Type
        {
            get
            {
                return type;
            }
        }

		public int ImageIndex {
			get {
				return imageIndex;
			}
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string Expression {
			get {
                return expression;
            }
            set
            {
                expression = value;
            }
		}
		
		double priority;
		
		public double Priority {
			get {
				return priority;
			}
			set {
				priority = value;
			}
		}
		
		public virtual bool InsertAction(TextArea textArea, char ch)
		{
			textArea.InsertString(description);
			return false;
		}
		
		public AEGISCompletionData(string text, string description, int imageIndex, string expression, eType type)
		{
			this.text        = text;
			this.description = description;
            this.imageIndex = imageIndex;
            this.expression = expression;
            this.type = type;
		}
		
		public static int Compare(ICompletionData a, ICompletionData b)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			if (b == null)
				throw new ArgumentNullException("b");
			return string.Compare(a.Text, b.Text, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
