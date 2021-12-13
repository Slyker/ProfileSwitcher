namespace ProfileSwitcher
{
    public partial class InputBox : Form
    {
        public string formTitle = "";
        public string textValue = "";
        public string defaultValue = "";
        public bool isMessageBox = false;
        public string OkText = "Ok";
        public string cancelText = "Annuler";
        public Action<string>? onOk;
        public Action<string>? onCancel;

        public InputBox(string _formTitle, string _textValue, string? _defaultValue, string _okText, string _cancelText, Action<string>? _onOk = null, Action<string>? _onCancel = null, bool _isMessageBox = false)
        {
            InitializeComponent();

            formTitle = _formTitle;
            textValue = _textValue;
            defaultValue = _defaultValue ?? "";
            OkText = _okText;
            cancelText = _cancelText;
            onOk = _onOk;
            onCancel = _onCancel;
            isMessageBox = _isMessageBox;

            if (isMessageBox)
            {
                textBox1.Visible = false;
            }
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            label1.Text = textValue;
            button1.Text = OkText;
            button2.Text = cancelText;
            textBox1.Text = defaultValue;
            if (isMessageBox)
            {
                this.Size = new Size(513, 145);
            }
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.Text = formTitle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (onOk != null)
            {
                onOk(textBox1.Text);
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (onCancel != null)
            {
                onCancel(textBox1.Text);
            }
            this.Close();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                button2.PerformClick();
            }
        }
    }
}
