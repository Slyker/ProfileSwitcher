using System.Data;

namespace ProfileSwitcher
{
    public partial class Main : Form
    {
        private DataGridView Dgv;
        private DataSetC Ds1;
        private string PresetName;
        private string FileName;

        public Main(string _FileName, string _PresetName)
        {
            InitializeComponent();
            listBox1.MouseDoubleClick += new MouseEventHandler(listBox1_DoubleClick);
            PresetName = _PresetName;
            FileName = _FileName;
            DataSetC _Ds1 = DataSets.dataSets.First(x => x.DataSetName == FileName);
            Dgv = dataGridView1;
            Ds1 = _Ds1 ?? new DataSetC(_FileName);
        }
        #region"UI ACTIONS"
        #region"Presets Actions"
        private void RennameBox()
        {
            if (listBox1.SelectedItem == null) { return; }
            string itemName = listBox1.SelectedItem.ToString();
            if (itemName == "Defaut") { return; };
            int itemIndex = listBox1.SelectedIndex;
            InputBox inputBox = new InputBox("Renommer",
                $"Veuillez entrer un nouveau nom pour le preset : {itemName}", itemName,
                "Valider", "Annuler",
                (res) =>
                {
                    DataTable? dt = Ds1.Tables[itemName];                    
                    if (dt != null)
                    {
                        dt.Rename(res);
                        refreshUi();
                    }
                    listBox1.SelectedIndex = itemIndex;
                });
            inputBox.ShowDialog();
        }
        private void DeleteBox()
        {
            if (listBox1.SelectedItem == null) { return; }
            string? itemName = listBox1.SelectedItem.ToString();
            int itemIndex = listBox1.SelectedIndex;
            if (itemName == "Defaut") { return; };
            InputBox inputBox = new InputBox("Suppression du preset",
                $"Souhaitez vous vraiment supprimer le preset : {itemName}", null,
                "Supprimer", "Annuler",
                (res) =>
                {
                    DataTable? dt = Ds1.Tables[itemName];
                    if (dt != null)
                    {
                        dt.Remove();
                        refreshUi();
                    }
                }, null, true);
            inputBox.ShowDialog();
        }
        private void AddBox()
        {
            InputBox inputBox = new InputBox("Création d'un preset", "Veuillez selectionner un nom pour votre preset", null, "Valider", "Annuler", (res) =>
            {
               
                if (Ds1 != null)
                {
                    if (!Ds1.Tables.Contains(res))
                    {
                        Ds1.AddTab(res);
                        refreshUi();
                    }
                    else
                    {
                        MessageBox.Show("Un preset portant existe déjà merci de réessayer");
                    }
                  
                }
            });
            inputBox.ShowDialog();
        }


        #endregion
        #region"Listbox refresh & contextmenu"
        private void refreshUi()
        {
            listBox1.Items.Clear();
            foreach (DataTable dr in Ds1.Tables)
            {
                listBox1.Items.Add(dr.TableName);
            }
        }
        private void buttonEnabler()
        {
            if (listBox1.SelectedIndex != -1)
            {
                button4.Enabled = true;
                button3.Enabled = true;
                contextMenuStrip1.Items[1].Enabled = true;
                contextMenuStrip1.Items[2].Enabled = true;

            }
            else
            {
                button4.Enabled = false;
                button3.Enabled = false;
                contextMenuStrip1.Items[1].Enabled = false;
                contextMenuStrip1.Items[2].Enabled = false;
            }
        }
        #endregion
        #endregion
        #region"UI EVENTS"
        private void Main_Load(object sender, EventArgs e)
        {
            refreshUi();
        }

        #region"SplitContainer"
        private void buttonx_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
        }
        #endregion

        #region"ListBox"
        private void listBox1_DoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                if (Ds1 != null && listBox1.SelectedItem != null)
                {
                    if (Dgv.DataSource == Ds1.Tables[listBox1.SelectedItem.ToString()])
                    {
                        Ds1.Save();
                    }
                    else
                    {
                        Ds1.Tables[listBox1.SelectedItem.ToString()].Bind(dataGridView1);
                    }
                }
            }
        }
        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            buttonEnabler();
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                listBox1.SelectedIndex = index;
            }
            else
            {
                listBox1.SelectedIndex = -1;
            }
        }
        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteBox();
            }
            else if (e.KeyCode == Keys.F2)
            {
                RennameBox();
            }
        }
        #endregion

        #region"ContextMenu"
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
        #endregion

        #region"DataGridView"
        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable? dataTable2 = (DataTable)dataGridView1.DataSource;
            if (dataTable2 != null)
            {
                this.Text = "Preset actuel : " + dataTable2.TableName;
            }
        }


        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message.Contains("nulls"))
            {
                MessageBox.Show("Un des champs est une clé primaire qui n'accèpte pas de valeur nulle.\nMerci de remplir les champs");
                e.Cancel = true;
                return;
            }
            else if (e.Exception.Message.Contains("unique"))
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                MessageBox.Show("Le champ : " + dataGridView1.Columns[e.ColumnIndex].HeaderText + " doit contenir des valeurs uniques\nEn sauvagardant des doublons vous risquez de perdre de données");
                e.Cancel = true;
                return;
            }
            MessageBox.Show(e.Exception.Message);
        }
        #endregion

        #region"Buttons"
        private void button2_Click(object sender, EventArgs e)
        {
            Ds1.Save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DeleteBox();
        }




        private void button5_Click(object sender, EventArgs e)
        {
            AddBox();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RennameBox();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #endregion
    }
}

