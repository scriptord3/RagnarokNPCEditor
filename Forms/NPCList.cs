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
using System.Windows.Forms;
using RagnarokNpcEditor.Classes;
using WeifenLuo.WinFormsUI.Docking;

namespace RagnarokNpcEditor
{
    public partial class NPCList : DockContent
    {
        public NPCList()
        {
            InitializeComponent();
        }

        public void NpcsChanged(List<NpcScriptLocation> list)
        {
            ClearList();
            foreach (var x in list)
            {
                listBox1.Items.Add(x);
            }
        }

        public void ClearList()
        {
            listBox1.Items.Clear();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var obj = (NpcScriptLocation)listBox1.SelectedItem;
            if (obj != null)
                MainForm.Singleton.ActiveDocument.NavigateToLine(obj.Index);
        }
    }
}