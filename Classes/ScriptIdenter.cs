using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RagnarokNpcEditor.Classes
{
    public class ScriptIdenter
    {
        public static string Ident(string text)
        {
            var bLastWasReturn = false;
            var indention = 0;
            text = text.Replace("\r\n", "\n");
            text = text.Replace("\r", "\n");
            var lines = text.Split(Convert.ToChar("\n"));
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                var l = line.Replace("\t", " ").Replace("  ", " ").Trim();
                if (string.IsNullOrEmpty(l)) continue;

                var cmd1 = l.Split(Convert.ToChar("("));
                var cmd2 = cmd1[0].Split(Convert.ToChar(" "));
                switch (cmd2[0].ToLower())
                {
                    case "trader":
                    case "npc":
                    case "hiddenwarp":
                        if (bLastWasReturn)
                        {
                            sb.Append("\n");
                            bLastWasReturn = false;
                        }
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
                        break;

                    case "oninit:":
                    case "onclick:":
                    case "ontouch:":
                    case "ontouch2:":
                    case "onmymobdead:":
                    case "ontimer:":
                    case "oncommand:":
                    case "onstartarena:":
                        if (indention != 0)
                        {
                            indention = 0;
                            sb.AppendFormat("return\n\n{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
                            indention++;
                        }
                        else
                        {
                            sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
                            indention++;
                        }
                        break;
                    case "if":
                    case "case":
                    case "while":
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
                        indention++;
                        break;
                    case "elseif":
                    case "else":
                        indention--;
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
                        indention++;
                        break;
                    case "endif":
                    case "break":
                    case "endwhile":
                        indention--;
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
                        break;
				case "return":
					if (indention == 1)
					{
                        //indention--;
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
                        bLastWasReturn = true;
					}
					else
					{
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
					}
				break;
				default:
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat("  ", indention)), l);
				break;
                }
            }
            return sb.ToString().Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
        }
    }
}
