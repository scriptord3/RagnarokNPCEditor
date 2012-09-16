using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RagnarokNpcEditor.Classes
{
    public class Config
    {
        public static string GetVersion {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            set {}
        }

        public static bool ReplaceTabWithSpace()
        {
            return false;
        }

        public static int Tabsize()
        {
            return 2;
        }
    }
}
