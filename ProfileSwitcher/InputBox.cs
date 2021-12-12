using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProfileSwitcher
{
    public partial class InputBox : Form
    {
        public string formTitle = "";
        public string textValue = "";
        public string OkText = "Ok";
        public string cancelText = "Annuler";
        public Action<string> onOk = (res) => { };
        public Action<string> onCancel = (res) => { };

        public InputBox(string _formTitle , string _textValue, string _okText, string _cancelText, Action<string> _onOk, Action<string> _onCancel)
        {
            formTitle = _formTitle;
            textValue = _textValue;
            OkText = _okText;
            cancelText = _cancelText;
            onOk = _onOk;
            onCancel = _onCancel;    

            InitializeComponent();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            label1.Text = textValue;
            button1.Text = OkText;
            button2.Text = cancelText;
            this.Text = formTitle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            onOk(textBox1.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            onCancel(textBox1.Text);
            this.Close();
        }

        private void InputBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            onCancel(textBox1.Text);
        }
    }
}
