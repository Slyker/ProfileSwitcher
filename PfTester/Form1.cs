using ProfileSwitcher;
namespace PfTester
{
    public partial class Form1 : Form
    {
        private DgvSet AddText;
        private DgvSet repText;
        public Form1()
        {
            InitializeComponent();
            FileManager.Init();
            AddText = DgvSave.Add("addtext", new List<DgvColumn>() {
                new DgvColumn("#",typeof(string),true,"999"),
                new DgvColumn("Name", typeof(string)),
                new DgvColumn("Surname", typeof(string))
            });
            repText = DgvSave.Add("replacetext", new List<DgvColumn>() {
                new DgvColumn("#",typeof(string)),
                new DgvColumn("Name", typeof(string)),
                new DgvColumn("Surname", typeof(string))
            });
            this.Focus();
            
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