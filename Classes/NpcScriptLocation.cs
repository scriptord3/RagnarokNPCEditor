using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RagnarokNpcEditor.Classes
{
    public class NpcScriptLocation
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
