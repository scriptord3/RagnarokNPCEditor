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
            bool lastWasReturn = false;
            Stack<string> startLines = new Stack<string>();
            var indention = 0;
            text = text.Replace("\r\n", "\n");
            text = text.Replace("\r", "\n");
            var lines = text.Split(Convert.ToChar("\n"));
            var sb = new StringBuilder();

            var tabchar = "\t";
            if (Config.ReplaceTabWithSpace())
                tabchar = String.Concat(Enumerable.Repeat(" ", Config.Tabsize()));

            var lineNum = 0;
            foreach (var line in lines)
            {
                lineNum++;
                var l = line.Trim();
                //.Replace("\t", " ").Replace("  ", " ").Trim();
                if (string.IsNullOrEmpty(l)) continue;

                var cmd1 = l.Split(Convert.ToChar("("));
                var cmd2 = cmd1[0].ToLower().Split(Convert.ToChar(" "));
                if (lastWasReturn && cmd2[0] != "event" && cmd2[0].StartsWith("on") == false)
                {
                    sb.Append("\n");
                }
                lastWasReturn = false;
                switch (cmd2[0])
                {
                    case "guide":
                    case "npc":
                    case "mob":
                    case "warp":
                    case "trader":
                    case "arenaguide":
                    case "hiddenwarp":
                    case "effect":
                    case "cashtrader":
                    case "npc2":
                    case "md_npc":
                    case "md_warp":
                    case "md_hiddenwarp":
                    case "kvm_npc":
                    case "item":
                    case "item2":
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                        break;

                    case "oninit:":
                    case "onclick:":
                    case "ontouch:":
                    case "onmymobdead:":
                    case "ontimer:":
                    case "oncommand:":
                    case "onstartarena:":
                    case "ontouchnpc:":
                    case "ontouch2:":
                    case "onmovenpccmd:":
                    case "oncampcommand:":
                    case "oncampcommand2:":
                    case "onagitinvest:":
                    case "event":
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                        startLines.Push(cmd2[0]);
                        indention++;
                        break;
                    case "if":
                    case "case":
                    case "while":
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                        startLines.Push(cmd2[0]);
                        indention++;
                        break;
                    case "elseif":
                    case "else":
                        {
                            var p = startLines.Pop();
                            if (p == "if")
                            {
                                indention--;
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                                startLines.Push("if");
                                indention++;
                            }
                            else
                            {
                                startLines.Push(p);
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                        }
                        break;
                    case "endif":
                        {
                            var p = startLines.Pop();
                            if (p == "if")
                            {
                                indention--;
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                            else
                            {
                                startLines.Push(p);
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                        }
                        break;
                    case "break":
                        {
                            var p = startLines.Pop();
                            if (p == "case")
                            {
                                indention--;
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                            else
                            {
                                startLines.Push(p);
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                        }
                        break;
                    case "endwhile":
                        {
                            var p = startLines.Pop();
                            if (p == "while")
                            {
                                indention--;
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                            else
                            {
                                startLines.Push(p);
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                        }
                        break;
                    case "return":
                        {
                            lastWasReturn = true;
                            var p = startLines.Pop();
                            if (p.StartsWith("on") || p == "event")
                            {
                                indention--;
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                            else
                            {
                                startLines.Push(p);
                                sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                            }
                        }
                        break;
                    default:
                        sb.AppendFormat("{0}{1}\n", String.Concat(Enumerable.Repeat(tabchar, indention)), l);
                        break;
                }
            }
            return sb.ToString().Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
        }
    }
}
