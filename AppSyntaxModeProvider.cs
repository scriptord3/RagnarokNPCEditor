using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using ICSharpCode.TextEditor.Document;

namespace RagnarokNpcEditor
{
    public class AppSyntaxModeProvider : ISyntaxModeFileProvider
    {
        List<SyntaxMode> syntaxModes = null;

        public ICollection<SyntaxMode> SyntaxModes
        {
            get
            {
                return syntaxModes;
            }
        }

        public AppSyntaxModeProvider()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            //enumerate resource names if need  
            //foreach (string resourceName in assembly.GetManifestResourceNames()){}  

            //load modes list  
            Stream syntaxModeStream = assembly.GetManifestResourceStream("RagnarokNpcEditor.Resources.SyntaxModes.xml");
            if (syntaxModeStream != null)
            {
                syntaxModes = SyntaxMode.GetSyntaxModes(syntaxModeStream);
            }
            else
            {
                syntaxModes = new List<SyntaxMode>();
            }
        }

        public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // load syntax schema  
            Stream stream = assembly.GetManifestResourceStream("RagnarokNpcEditor.Resources." + syntaxMode.FileName);
            return new XmlTextReader(stream);
        }

        public void UpdateSyntaxModeList()
        {
            // resources don't change during runtime  
        }
    }
}
