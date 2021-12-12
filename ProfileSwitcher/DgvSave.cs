using System.Data;
using System.Reflection;

namespace ProfileSwitcher
{

    public static class DgvSave
    {
        internal static List<DataSet> DataSets { get; } = new List<DataSet>();
        public static List<DgvSet> DgvSets { get; } = new List<DgvSet>();
        public static void Add(string _FileName, DataGridView Dgv, List<DgvColumn> dgvColumns, string SetName = "Defaut", string TableName = "Defaut")
        {
            DgvSets.Add(new DgvSet(_FileName, Dgv, dgvColumns, SetName, TableName));
        }
        public static void Save(string SetName = "")
        {
            var item = (SetName != "") ? DgvSets.Find(x => x.FileName == SetName) : null;
            if (item != null)
            {
                item.Save();
            }
            else
            {
                DgvSets.ForEach(x => x.Save());
            }
        }
        public static void Load(string SetName = "")
        {
            var item = (SetName != "") ? DgvSets.Find(x => x.FileName == SetName) : null;
            if (item != null)
            {
                item.Load();
            }
            else
            {
                DgvSets.ForEach(x => x.Load());
            }
        }

    }
    public class DgvSet
    {
        public string FileName { get; set; }
        public string Description { get; set; } = "Test description";
        public DataSet DataSet { get; set; }
        public DataGridView Dgv { get; set; }
        public List<DgvColumn> Columns { get; set; } = new List<DgvColumn>();

        public string Path { get { return FileManager.AppPath + FileName + ".xml"; } }
        public void Save()
        {
            DataSet.WriteXml(Path);
        }
        public void Load(string? tableName = null)
        {
            Dgv.DataSource = null;
            Dgv.Rows.Clear();
            Dgv.Columns.Clear();
            
            if (tableName != null)
            {
                Dgv.DataSource = DataSet.Tables[tableName];
            }
            else
            {
                DataSet.ReadXml(Path);
            }
           
        }
        public DgvSet(string _FileName, DataGridView _Dgv, List<DgvColumn> dgvColumns, string SetName = "Defaut", string TableName = "Defaut")
        {

            string path = FileManager.AppPath + _FileName + ".xml";
            Dgv = _Dgv;
            FileName = _FileName;
            Columns = dgvColumns;
            SetName = _FileName ;
            
            DataSet = new DataSet(SetName);
            //DataSet.EnforceConstraints = false;
            if (File.Exists(path))
            {
                Load();
            }
            DataSet ds = DgvSave.DataSets.Find(x => x.DataSetName == SetName);
            if (ds == null)
            {
                DgvSave.DataSets.Add(DataSet);
            }
            else
            {
                DataSet = ds;
            }
            DataTable dt = AddDTable(TableName);
            AddRow(dt,  new string[] { "Defaut", "Defaut" });
            Save();
        }
        public DataTable AddDTable(string TableName)
        {
            DataTable dt = DataSet.Tables[TableName];
            DataTable DataTable = dt ?? new DataTable(TableName);

            var keys = new DataColumn[1];
            DataColumn column;
            if(dt == null)
            {
                DataSet.Tables.Add(DataTable);
            }
            foreach (DgvColumn dgvColumn in Columns)
            {

                column = new DataColumn();
                column.DataType = dgvColumn.Type;
                column.ColumnName = dgvColumn.Name;
                if (!DataTable.Columns.Contains(dgvColumn.Name))
                {
                    DataTable.Columns.Add(column);
                    
                }
                
                if (keys[0] == null)
                {
                    keys[0] = DataTable.Columns[dgvColumn.Name];
                    DataTable.PrimaryKey = keys;
                }
            }
            //DataTable.PrimaryKey = keys;


            Dgv.DataSource = DataTable;
            return DataTable;

        }
        public void AddRow(DataTable dataTable, string[] items)
        {


            DataRow row = dataTable.NewRow();
            int id = 0;

            foreach (string item in items)
            {
                row[Columns[id].Name] = item;
                id++;
            }
            try
            {
                dataTable.Rows.Add(row);
            }
            catch
            {

            }
               
            

            Save();
            Dgv.DataSource = dataTable;

        }
        //public object this[string propertyName]
        //{
        //    get
        //    {
        //        // probably faster without reflection:
        //        // like:  return Properties.Settings.Default.PropertyValues[propertyName] 
        //        // instead of the following
        //        Type myType = typeof(DgvSet);
        //        PropertyInfo myPropInfo = myType.GetProperty(propertyName);
        //        return myPropInfo.GetValue(this, null);
        //    }
        //    set
        //    {
        //        Type myType = typeof(DgvSet);
        //        PropertyInfo myPropInfo = myType.GetProperty(propertyName);
        //        myPropInfo.SetValue(this, value, null);
        //    }
        //}
    }

    public class DgvColumn
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public DgvColumn(string _name, Type _Type)
        {
            Name = _name;
            Type = _Type;
        }

    }
}
