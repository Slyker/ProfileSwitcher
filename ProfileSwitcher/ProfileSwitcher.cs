using System.Data;

namespace ProfileSwitcher
{
    public partial class ProfileSwitcher : Form
    {
        private DataGridView Dgv;
        private DataSet Ds1;
        private string PresetName;
        private string FileName;

        public ProfileSwitcher(DataGridView _dgvRows, string _FileName, string _PresetName)
        {
            InitializeComponent();
            PresetName = _PresetName;
            FileName = _FileName;
            DataSet? _Ds1 = DgvSave.DataSets.Find(x => x.DataSetName == FileName);
            Dgv = _dgvRows;

            if (_Ds1 != null)
            {
                Ds1 = _Ds1;
            }
            else
            {
                Ds1 = new DataSet(_FileName);
            }
        }
        private void RennameBox()
        {


        }
        private void DeleteBox()
        {


        }
        private void AddBox()
        {
            InputBox inputBox = new InputBox("Création d'un preset", "Veuillez selectionner un nom pour votre preset", "Valider", "Annuler", (res) =>
            {
                DgvSet? dgvSet = DgvSave.DgvSets.Find(x => x.FileName == FileName);
                if (dgvSet != null)
                {
                    DataTable dt = dgvSet.AddDTable(res);
                    dgvSet.AddRow(dt, new string[] { "Defaut", "" });
                    dgvSet.Save();
                    Dgv.DataSource = dt;
                }


            }, (res) => { });
            inputBox.ShowDialog();
        }

        private void ajouterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddBox();
        }

        private void renommerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RennameBox();
        }

        private void supprimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteBox();
        }
        
        private void refreshUi()
        {
            foreach (DataTable dr in Ds1.Tables)
            {
                listBox1.Items.Add(dr.TableName);
            }
        }
        private void ProfileSwitcher_Load(object sender, EventArgs e)
        {
            refreshUi();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            DgvSet? dgvSet = DgvSave.DgvSets.Find(x => x.FileName == FileName);
            if (dgvSet != null && listBox1.SelectedItem != null)
            {
                if (Dgv.DataSource == dgvSet.DataSet.Tables[listBox1.SelectedItem.ToString()])
                {
                    dgvSet.Save();
                }
                else
                {
                    dgvSet.Load(listBox1.SelectedItem.ToString());
                }



            }


        }
        private void buttonEnabler()
        {
            if (listBox1.SelectedIndex != -1)
            {
                button2.Enabled = true;
                button3.Enabled = true;
                contextMenuStrip1.Items[1].Enabled = true;
                contextMenuStrip1.Items[2].Enabled = true;

            }
            else
            {
                button2.Enabled = false;
                button3.Enabled = false;
                contextMenuStrip1.Items[1].Enabled = false;
                contextMenuStrip1.Items[2].Enabled = false;
            }
        }
        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            buttonEnabler();
        }

        private void listBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RennameBox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DeleteBox();
        }
    }
}
