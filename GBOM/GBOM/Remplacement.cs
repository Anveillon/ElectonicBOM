using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GBOM
{
    public partial class Remplacement : Form
    {
        public string new_Ref;
        public Remplacement(string val)
        {
            InitializeComponent();
            textBox2.Text = val;
            textBox2.ReadOnly = true;
            textBox2.BorderStyle = 0;
            textBox2.BackColor = this.BackColor;
            textBox2.TabStop = false;
            
            textBox1.Text = new_Ref = "";
        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                new_Ref = textBox1.Text;
                this.Close();
            }
        }

        private void BTN_CANCEL_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
