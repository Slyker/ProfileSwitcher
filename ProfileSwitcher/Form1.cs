using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace ProfileSwitcher
{
    public partial class Form1 : Form
    {
        public List<DataSet> DataSets { get; set; } = new List<DataSet>();
        
        public Form1()
        {
                
            InitializeComponent();

            foreach (Control _control in this.Controls)
            {
                if(_control is DataGridView)
                {
                    DataGridView control = _control as DataGridView;
                    control.Columns.Insert(0, new DataGridViewColumn() { HeaderText = "#" });
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProfileSwitcher profileSwitcher = new ProfileSwitcher(dataGridView1, tabControl1.SelectedTab.Name,"Defaut");
            // profileSwitcher.

            profileSwitcher.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileManager.Init();
            string path = FileManager.AppPath + tabControl1.SelectedTab.Name + ".xml";
            DgvSave.Add("addtext",dataGridView1,new List<DgvColumn>() { 
                new DgvColumn("Description",typeof(string)), 
                new DgvColumn("Test", typeof(string)) 
            });
            DgvSave.Add("replacetext", dataGridView2, new List<DgvColumn>() {
                new DgvColumn("Description2",typeof(string)),
                new DgvColumn("Test2", typeof(string))
            });

            //InitDataSource("addtext",dataGridView1);
            //InitDataSource("replacetext", dataGridView2);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //source.ResetBindings(false);
            dataGridView1.Refresh();

            dataGridView1.Update();
            foreach (DataTable dt in DataSets[0].Tables)
            {
                var result = "";
                foreach(DataRow dr in dt.Rows)
                {
                    
                    result += dr["Column1"] + "\n";
                }
                MessageBox.Show(result);
            }
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //DgvSave.Save();
        }
    }
}