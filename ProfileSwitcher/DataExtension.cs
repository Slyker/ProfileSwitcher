using Newtonsoft.Json;
using System.Data;

namespace ProfileSwitcher
{

    public static class DataSets
    {
        public static List<DataSetC> dataSets = new List<DataSetC>();
        public static DataSetC Add(string dataSetName, Func<DataCol[]?> dataCols = null)
        {
            DataSetC ds = new DataSetC(dataSetName, dataCols);
            dataSets.Add(ds);
            ds.Load();
            if (ds.Tables["Defaut"] == null)
            {
                ds.AddTab("Defaut");
            }
            return ds;
        }
        public static void Save()
        {
            dataSets.ForEach(x => x.Save());
        }
        public static void Load()
        {
            dataSets.ForEach(x => x.Load());
        }
    }
    public class DataSetC : DataSet
    {
        private static string DataSetName = "NewDataSet";
        public Func<DataCol[]?>? DataCols { get; set; } = null;
        public DataSetC(string dataSetName, Func<DataCol[]?>? dataCols = null) : base(dataSetName)
        {
            DataSetName = dataSetName;
            DataCols = dataCols ?? DataCols;
        }
        public DataSetC() : this(DataSetName, null)
        {
        }
    }
    public class DataCol : DataColumn
    {
        private static string? ColumnName = null;
        private static Type Type = typeof(string);
        public bool IsPrimary = false;
        public DataCol(string? columnName, Type type, string? defaultValue = null, bool isPrimary = false, bool autoIncrement = false) : base(columnName, type)
        {
            ColumnName = columnName;
            Type = type;
            AutoIncrement = autoIncrement;
            if (!autoIncrement)
            {
                DefaultValue = defaultValue;
            }
            IsPrimary = isPrimary;
        }
        public DataCol() : this(ColumnName, Type, null, false, false)
        {
            IsPrimary = false;
        }
    }
    public static class DataTableExtension
    {
        public static string PathOrigin { get; set; } = FileManager.AppPath + "Settings";
        #region "DataTables"
        public static DataTable AddTab(this DataSetC dtc, string tableName)
        {
            DataCol[] columns = dtc.DataCols() ?? new List<DataCol>().ToArray();
            DataTable dt = new DataTable(tableName);
            List<DataCol> dataCols = columns.ToList();
            var tts = dataCols.Where(x => x.IsPrimary).ToList().Count;
            if (tts == 0)
            {
                DataCol dataCol = new DataCol("id", typeof(int), null, true, true);
                List<DataCol> dataCols1 = dataCols;
                columns = dataCols.Prepend(dataCol).ToArray();
            }
            dt.Columns.AddRange(columns);
            dataCols = columns.ToList<DataCol>().Where(x => x.IsPrimary).ToList();
            dt.PrimaryKey = dataCols.ToArray();

            if (!dtc.Tables.Contains(dt.TableName))
            {
                dtc.Tables.Add(dt);
            }
            else
            {
                //var tt = dtc.Tables[dt.TableName].AsEnumerable().ToArray();
                //dtc.Tables.Remove(dt.TableName);
                //dt.Rows.CopyTo(tt, 0);
                //dtc.Tables.Add(dt);
            }
            dt.AddRow(new object[] { });
            return dt;
        }
        public static void OpenSwitcher(this DataSet dtc, string tableName = "Defaut")
        {
            Main mainSwitcher = new Main(dtc.DataSetName, tableName);
            dtc.Tables[tableName].Bind(mainSwitcher.dataGridView1);
            mainSwitcher.Show();
        }
        public static DataRow AddRow(this DataTable dtc, object?[] values)
        {
            DataCol? dt = dtc.Columns.Cast<DataCol>().ToList().Find(x => x.IsPrimary && x.ColumnName == "id");
            if (dtc.PrimaryKey.Count() == 1 && dt != null)
            {
                values = values.Prepend(null).ToArray();
            }
            return dtc.Rows.Add(values);
        }
        #endregion
        #region"SaveSystem"
        public static void Remove(this DataTable dt)
        {
            DataSetC dts = (DataSetC)dt.DataSet;
            dt.DataSet.Tables.Remove(dt);
            dts.Save();
        }
        public static void Rename(this DataTable dt, string newName)
        {
            if (dt != null && dt.DataSet != null && !dt.DataSet.Tables.Contains(newName))
            {
                DataSetC dts = (DataSetC)dt.DataSet;
                dt.TableName = newName;
                dts.Save();
            }
        }
        public static void Bind(this DataTable dt, DataGridView dataGridView)
        {
            dataGridView.DataSource = dt;
        }
        public static string Path(this DataSetC ds)
        {
            return PathOrigin + @"\" + ds.DataSetName + ".json";
        }
        public static void Save(this DataSetC ds)
        {
            var path = ds.Path();
            if (!File.Exists(path)) { File.Create(path).Close(); }
            using (StreamWriter file = new StreamWriter(path))
            {
                string json = JsonConvert.SerializeObject(ds, Formatting.Indented);
                file.Write(json);
            }
        }

        public static DataSetC Load(this DataSetC ds)
        {
            if (File.Exists(ds.Path()))
            {
                DataSetC? dataSet = JsonConvert.DeserializeObject<DataSetC>(File.ReadAllText(ds.Path()).ToString());
                if (dataSet != null)
                {
                    foreach (DataTable dt in dataSet.Tables)
                    {
                        List<DataCol> list = ds.DataCols().ToList();
                        if (dt.PrimaryKey.Count() == 0 && list.FindAll(x => x.IsPrimary).Count() > 0)
                        {
                            foreach (DataColumn col in dt.Columns)
                            {
                                DataCol? dtc = list.Find(x => x.ColumnName == col.ColumnName && x.IsPrimary);
                                if (dtc != null)
                                {
                                    dt.PrimaryKey = dt.PrimaryKey.Append(col).ToArray();
                                }
                            }
                        }
                    }
                    ds.Merge(dataSet);
                }
            }
            return ds;
        }
        #endregion
        #region"Get"
        public static DataRow? GetRow(this DataTable dt, object key, Action<DataRow>? action = null)
        {
            if (dt == null) { return null; }
            if (dt.PrimaryKey == null || dt.PrimaryKey.Count() == 0) { return null; }
            var converted = Convert.ChangeType(key, typeof(object));
            DataRow? dr = null;
            try
            {
                dr = dt.Rows.Find(key);
                if (action != null && dr != null)
                {
                    action(dr);
                }
                return dr;
            }
            catch
            {
                var tes = dt.PrimaryKey.Select((x) => x.ColumnName).ToList();
                tes.ForEach((_columnName) =>
                {

                    var drNew = dt.AsEnumerable().Where(p => (p.ItemArray.Contains(converted)) || (p.Field<object>(_columnName).Equals(converted)) || (p.Field<object>(_columnName).ToString().ToLower().Contains(converted.ToString().ToLower()))).FirstOrDefault();
                    if (drNew != null)
                    {
                        dr = drNew;
                    }
                });

                if (dr != null)
                {
                    if (action != null && dr != null)
                    {
                        action(dr);
                    }
                    return dr;
                }
            }
            return null;
        }
        #endregion

        #region"Set"

        public static void SetRowValue(this DataRow dr, object?[] values)
        {
            if (dr == null) { return; }
            DataTable dt = dr.Table;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (values[i] != null)
                {
                    dr.SetField(dt.Columns[i].ColumnName, values[i]);
                }
            }
        }
        public static void SetRowValue(this DataRow dr, object value, string rowIndex)
        {
            DataTable dt = dr.Table;
            dr.SetField(dt.Columns[rowIndex].ColumnName, value);
        }

        #endregion



    }

}
