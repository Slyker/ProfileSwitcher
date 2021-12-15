using ProfileSwitcher;

namespace PfTester
{
    public partial class Form1 : Form
    {
        private DataSetC AddText;
        private DataSetC repText;
        public Form1()
        {
            InitializeComponent();
            FileManager.Init();
            AddText = DataSets.Add("addtext", () =>
            {
                return new DataCol[]
                {
                    new DataCol("#",typeof(string),null,true,true),
                    new DataCol("Name", typeof(string)),
                    new DataCol("Surname", typeof(string))
                };
            });

            repText = DataSets.Add("replacetext", () =>
            {
                return new DataCol[]
                {
                    new DataCol("#",typeof(string),null,true,true),
                    new DataCol("Name", typeof(string)),
                    new DataCol("Surname", typeof(string))
                };
            });
            Focus();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddText.OpenSwitcher();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            repText.OpenSwitcher();
        }
    }
}