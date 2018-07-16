using System;
using System.Collections.Generic;
using Atreemo.Models;
using System.Collections;
using System.Resources;
using Atreemo.Views.Tools;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;

using System.Net.Mail;
using OfficeOpenXml;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Net.Http;
using Atreemo.Controllers;
using System.Net;
using Newtonsoft.Json;

namespace Atreemo.Tools
{
    public static class GlobalHelper
    {
        public static string[] CHART_COLORS = {
                                              "#ff832e", "#a7ad13" , "#759418" 
                                              , "#ff63a5", "#1c9ec4", "#a2df53", "#ffb74f", "#ff7663", "#0c779b", "#b20753"
                                              ,"#1ba7e2","#eb7b25", "#017e82",  "#f6981e", "#651e56","#ff7c7c" ,"#10c4b2"
                                              , "#2b893c", "#ffe13a", "#be5138", "#0081da", "#3aafff", "#99c900", "#ffeb3d"
                                              , "#b20753", "#ff4195", "#a7008f", "#ffb800", "#3aafff", "#99c900", "#b20753"
                                              , "#ff4195", "#ff5722", "#4caf50", "#03a9f4", "#ff9800", "#7bd2f6", "#ffeb3d","#ff9819"
                                              };

        public class AddSelectionModel
        {
            public bool CtcSeg { get; set; }
            public bool DisplayViewSelectionButton { get; set; }
            public bool DisplayAddSelectionButton { get; set; }
            public string AddSelectionMethodeName { get; set; }
            public bool DisplayOpenSelectionButton { get; set; }
            public string OpenSelectionMethodeName { get; set; }
            public string UseSelectionMethodeName { get; set; }
            public string OpenSelectionIconName { get; set; }
            public string AddSelectionIconName { get; set; }
            public string InverseSelectionMethodeName { get; set; }
            public bool DisplayInverseSelectionIcon { get; set; }
            public AddSelectionModel(bool? CtcSeg, bool? DisplayViewSelectionButton, bool? DisplayAddSelectionButton, bool? DisplayOpenSelectionButton, string AddSelectionMethodeName, string OpenSelectionMethodeName)
            {
                this.CtcSeg = CtcSeg.HasValue ? (bool)CtcSeg : true;
                this.DisplayViewSelectionButton = DisplayViewSelectionButton.HasValue ? (bool)DisplayViewSelectionButton : true;
                this.DisplayAddSelectionButton = DisplayAddSelectionButton.HasValue ? (bool)DisplayAddSelectionButton : true;
                this.DisplayOpenSelectionButton = DisplayOpenSelectionButton.HasValue ? (bool)DisplayOpenSelectionButton : true;
                this.OpenSelectionMethodeName = OpenSelectionMethodeName;
                this.AddSelectionMethodeName = AddSelectionMethodeName;
            }

            public AddSelectionModel(bool? CtcSeg, bool? DisplayViewSelectionButton, bool? DisplayAddSelectionButton, bool? DisplayOpenSelectionButton,bool? DisplayInverseSelectionIcon, string AddSelectionMethodeName, string OpenSelectionMethodeName, string InverseSelectionMethodeName)
            {
                this.CtcSeg = CtcSeg.HasValue ? (bool)CtcSeg : true;
                this.DisplayViewSelectionButton = DisplayViewSelectionButton.HasValue ? (bool)DisplayViewSelectionButton : true;
                this.DisplayAddSelectionButton = DisplayAddSelectionButton.HasValue ? (bool)DisplayAddSelectionButton : true;
                this.DisplayOpenSelectionButton = DisplayOpenSelectionButton.HasValue ? (bool)DisplayOpenSelectionButton : true;
                this.DisplayInverseSelectionIcon = DisplayInverseSelectionIcon.HasValue ? (bool)DisplayInverseSelectionIcon : true;
                this.OpenSelectionMethodeName = OpenSelectionMethodeName;
                this.AddSelectionMethodeName = AddSelectionMethodeName;
                this.InverseSelectionMethodeName = InverseSelectionMethodeName;
            }

            public AddSelectionModel(bool? CtcSeg, bool? DisplayViewSelectionButton, bool? DisplayAddSelectionButton, bool? DisplayOpenSelectionButton, string AddSelectionMethodeName, string OpenSelectionMethodeName, string UseSelectionMethodeName, string OpenSelectionIconName = "../Content/images/Functions/open24.png", string AddSelectionIconName = "../Content/images/Functions/add24.png")
            {
                this.CtcSeg = CtcSeg.HasValue ? (bool)CtcSeg : true;
                this.DisplayViewSelectionButton = DisplayViewSelectionButton.HasValue ? (bool)DisplayViewSelectionButton : true;
                this.DisplayAddSelectionButton = DisplayAddSelectionButton.HasValue ? (bool)DisplayAddSelectionButton : true;
                this.DisplayOpenSelectionButton = DisplayOpenSelectionButton.HasValue ? (bool)DisplayOpenSelectionButton : true;
                this.OpenSelectionMethodeName = OpenSelectionMethodeName;
                this.AddSelectionMethodeName = AddSelectionMethodeName;
                this.UseSelectionMethodeName = UseSelectionMethodeName;
                this.OpenSelectionIconName = OpenSelectionIconName;
                this.AddSelectionIconName = AddSelectionIconName;
            }
        }

        public class HelpTooltip
        {
            public string Body { get; set; }
            public string Name { get; set; }
            public int ID { get; set; }
        }

        public static List<SelectListItem> GetIntValues(int MinValue = 1, int MaxValue = 365)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = MinValue; i <= MaxValue; i++)
            {
                list.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }
            return list;
        }

        public static int SaveUpdateFormat(string FormatName, string selectedfields, string Type,int UserID)
        {
            int FormatID = 0;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[SaveOrUpdateFormat]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandTimeout = 3600;
                    Cmd.Parameters.Add(new SqlParameter("@FormatName", FormatName));
                    Cmd.Parameters.Add(new SqlParameter("@FieldCodeIDList", selectedfields));
                    Cmd.Parameters.Add(new SqlParameter("@Type", Type));
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cnx.Open();
                    FormatID = (int)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "GLOBAL001 SaveUpdateFormat [FormatName:" + FormatName + " ,selectedfields:" + selectedfields + " ,Type:" + Type + "]");
            }
            return FormatID;
        }

        public static string GetVersionNumber()
        {
            Assembly web = Assembly.GetExecutingAssembly();
            AssemblyName webName = web.GetName();
            string myVersion = webName.Version.ToString();
            return myVersion;
        }

        public static string GetHelpTooltipText(String path, string ResourceName)
        {
            Hashtable resourceEntries = new Hashtable();
            //Get existing resources
            ResXResourceReader reader = new ResXResourceReader(path);
            if (reader != null)
            {
                IDictionaryEnumerator id = reader.GetEnumerator();
                foreach (DictionaryEntry d in reader)
                {
                    if (d.Value == null)
                        resourceEntries.Add(d.Key.ToString(), "");
                    else
                        resourceEntries.Add(d.Key.ToString(), d.Value.ToString());
                }
                reader.Close();
            }

            string value = "";
            if (resourceEntries.ContainsKey(ResourceName))
            {
                value = resourceEntries[ResourceName].ToString();
            }

            return value;

        }

        public static string GetDomainNameFromHost(string HostName)
        {
            string DomainName = HostName;

            if (!string.IsNullOrEmpty(HostName))
            {
                var tokens = HostName.Split('.');
                if (tokens.Length > 2)
                {
                    //Add only second level exceptions to the < 3 rule here
                    List<string> exceptions = new List<string>() { "info", "firm", "name", "com", "biz", "gen", "ltd", "web", "net", "pro", "org" };
                    var validTokens = 2 + ((tokens[tokens.Length - 2].Length < 3 || exceptions.Contains(tokens[tokens.Length - 2])) ? 1 : 0);
                    DomainName = string.Join(".", tokens, tokens.Length - validTokens, validTokens);
                }
            }

            return DomainName;
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static void UpdateResourceFile(Hashtable data, String path)
        {
            Hashtable resourceEntries = new Hashtable();

            //Get existing resources
            ResXResourceReader reader = new ResXResourceReader(path);
            if (reader != null)
            {
                IDictionaryEnumerator id = reader.GetEnumerator();
                foreach (DictionaryEntry d in reader)
                {
                    if (d.Value == null)
                        resourceEntries.Add(d.Key.ToString(), "");
                    else
                        resourceEntries.Add(d.Key.ToString(), d.Value.ToString());
                }
                reader.Close();
            }

            //Modify resources here...
            foreach (String key in data.Keys)
            {
                if (!resourceEntries.ContainsKey(key))
                {
                    String value = data[key].ToString();
                    if (value == null)
                        value = "";
                    resourceEntries.Add(key, value);
                }
                else
                {
                    String value = data[key].ToString();
                    if (value == null)
                        value = "";
                    resourceEntries.Remove(key);
                    resourceEntries.Add(key, data[key].ToString());
                }
            }
            //Write the combined resource file
            ResXResourceWriter resourceWriter = new ResXResourceWriter(path);

            foreach (String key in resourceEntries.Keys)
            {
                resourceWriter.AddResource(key, resourceEntries[key]);
            }
            resourceWriter.Generate();
            resourceWriter.Close();


        }

        public class ContactSummary
        {
            public bool? IsEmailOptedIn { get; set; }
            public bool? IsSMSOptedIn { get; set; }
            public bool? IsMailOptedIn { get; set; }
            public bool? IsAppOptedIn { get; set; }
            public bool? IsPhoneOptedIn { get; set; }
            public bool? IsEmailAvailable { get; set; }
            public bool? IsSMSAvailable { get; set; }
            public bool? IsMailAvailable { get; set; }
            public bool? IsAppAvailable { get; set; }
            public bool? IsPhoneAvailable { get; set; }
            public int? CommsInLast12Months { get; set; }
            public int? EmailsInLast12Months { get; set; }
            public int? MailsInLast12Months { get; set; }
            public int? SMSsInLast12Months { get; set; }
            public int? APPsInLast12Months { get; set; }
            public Dictionary<string, string> TheirLast12Months { get; set; }
            public string FormatedAddress { get; set; }
        }

        public class HistoryGridColumns
        {
            public string[] GridHeaderColumns { get; set; }
            public string[] ExcludedColumns { get; set; }
            public List<string> ColumnsName { get; set; } // Fileds name in the DB
            public List<string> ColumnsDisplayName { get; set; } // Fields Display name
        }

        public static bool CreateIndexFileTable(string TableName, DataTable IndexDataTable, string ServerName, string DBName, string ConnectionString)
        {
            try
            {

                using (SqlConnection SqlCnx = new SqlConnection(ConnectionString))
                {
                    SqlCnx.Open();
                    SqlCommand SqlCmd = new SqlCommand();
                    SqlCmd.Connection = SqlCnx;
                    SqlCmd.CommandType = CommandType.Text;
                    SqlCmd.CommandText = "IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')) DROP TABLE " + TableName;
                    SqlCmd.ExecuteNonQuery();

                    try
                    {
                        // Connect to Server/database
                        ServerConnection aCnx = new ServerConnection();
                        aCnx.LoginSecure = false;
                        aCnx.ServerInstance = ServerName;
                        aCnx.Login = ConfigurationManager.AppSettings["SQLUserName"].ToString();
                        aCnx.Password = ConfigurationManager.AppSettings["SQLPassword"].ToString();
                        Server aServer = new Server(aCnx);
                        Microsoft.SqlServer.Management.Smo.Database DB = aServer.Databases[DBName];
                        // Create Structure
                        Microsoft.SqlServer.Management.Smo.Table newTable = new Microsoft.SqlServer.Management.Smo.Table(DB, TableName, "dbo");
                        foreach (DataColumn aCol in IndexDataTable.Columns)
                        {
                            Column anewCol = new Column(newTable, aCol.ColumnName);
                            if (aCol.MaxLength > 0)
                                anewCol.DataType = Microsoft.SqlServer.Management.Smo.DataType.VarChar(aCol.MaxLength);
                            else
                                anewCol.DataType = Microsoft.SqlServer.Management.Smo.DataType.VarChar(8000);
                            newTable.Columns.Add(anewCol);
                        }
                        // Physically create the table in the database
                        newTable.Create();
                        // Transfer the data
                        using (SqlConnection Cnx = new SqlConnection(ConnectionString))
                        {
                            Cnx.Open();
                            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default;
                            using (SqlBulkCopy bcp = new SqlBulkCopy(Cnx, options, null))
                            {
                                foreach (DataColumn aCol in IndexDataTable.Columns)
                                {
                                    bcp.ColumnMappings.Add(aCol.ColumnName, aCol.ColumnName);
                                }
                                bcp.DestinationTableName = TableName;
                                bcp.BulkCopyTimeout = 360000;
                                bcp.WriteToServer(IndexDataTable);
                                bcp.Close();
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        Functions.SendErrorEmail(err, "GLOBAL002 CreateIndexFileTable (BCP) [TableName:" + TableName + "]");
                        return false;
                    }
                    return true;
                }


            }
            catch (Exception err)
            {
                Functions.SendErrorEmail(err, "GLOBAL003 CreateIndexFileTable [TableName:" + TableName + "]");
                return false;
            }

        }

        /// <summary>
        /// Returns true if the user is authorised to access the profile and false if not
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="Value">Value to check depending on the authorization type (CtcID, CpyID, PrjMkgID , OpCode)</param>
        /// <param name="AuthorizationType">Authorization Type: 0--> Contact; 1 --> Company ; 2--> MarketingProject; 3--> Ecast; 4--> SMS</param>
        /// <returns></returns>

        public static bool IsProfileAccessAuthorised(int UserID, int? Value, int AuthorizationType)
        {
            if (Value == null)
                return false;

            try
            {
                DataTable Dt = new DataTable();
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    Cnx.Open();
                    SqlCommand SqlCmd = new SqlCommand();
                    SqlCmd.Connection = Cnx;
                    SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlCmd.CommandText = "[Atreemo].[AuthorizeByUserID]";
                    SqlParameter ValueParam = new SqlParameter("@Value", Value);
                    SqlParameter AuthorizationTypeParam = new SqlParameter("@AuthorizationType", AuthorizationType);
                    SqlParameter UserIDParam = new SqlParameter("@UserID", UserID);
                    SqlParameter UseEnvironementFilterParam = new SqlParameter("@UseEnvironementFilter", ConfigurationManager.AppSettings["ApplyEnvironementFilter"] == "Yes" ? true : false);

                    SqlCmd.Parameters.Add(ValueParam);
                    SqlCmd.Parameters.Add(AuthorizationTypeParam);
                    SqlCmd.Parameters.Add(UserIDParam);
                    SqlCmd.Parameters.Add(UseEnvironementFilterParam);

                    SqlDataAdapter dataA = new SqlDataAdapter();
                    dataA.SelectCommand = SqlCmd;
                    dataA.Fill(Dt);
                    if (!(bool)Dt.Rows[0]["HasAccess"])
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "GLOBAL004 IsProfileAccessAuthorised [UserID:" + UserID + " ,Value:" + (Value??0) + "]");
                return false;
            }
        }

        #region Excel Tools
        public static DataTable LoadContentFromXLS(string filename,string sheetname = null)
        {
            string tcdConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                "Data Source=\"" + filename + "\";" +
                "Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

            if ((Path.GetExtension(filename).ToLower() == ".csv") && (filename != null))
                return LoadContentFromText(filename);
            try
            {
                using (OleDbConnection tcdConn = new OleDbConnection(tcdConnStr))
                {
                    tcdConn.Open();
                    string select = "";
                    if (sheetname == null)
                    {
                        DataTable adt = tcdConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        select = "SELECT * FROM [" + adt.Rows[0]["TABLE_NAME"].ToString() + "]";
                        adt.Dispose();
                    }
                    else
                        select = "SELECT * FROM [" + sheetname + "]";

                    OleDbDataAdapter oda = new OleDbDataAdapter(select, tcdConn);
                    DataTable dt = new DataTable();
                    oda.FillSchema(dt, SchemaType.Source);
                    oda.Fill(dt);

                    foreach (var column in dt.Columns.Cast<DataColumn>().ToArray())
                    {
                        if (dt.AsEnumerable().All(dr => dr.IsNull(column)))
                            dt.Columns.Remove(column);
                    }
                    tcdConn.Close();
                    return (dt);
                }
            }
            catch (Exception err)
            {
                //Functions.SendErrorEmail(err, "GLOBAL005 LoadContentFromXLS [filename:" + filename + "]");
                return GetDataTableFromExcel(filename);
            }

        }

        public static DataTable LoadContentFromText(string filename)
        {
            string tcdConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                "Data Source=" + Path.GetDirectoryName(filename) + ";" +
                "Extended Properties=\"Text;\"";

            try
            {
                using (OleDbConnection tcdConn = new OleDbConnection(tcdConnStr))
                {
                    tcdConn.Open();
                    string ss = Path.GetFileName(filename);
                    DataTable adt = tcdConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string select = "SELECT * FROM [" + Path.GetFileName(filename) + "]";
                    adt.Dispose();
                    OleDbDataAdapter oda = new OleDbDataAdapter(select, tcdConn);
                    DataTable dt = new DataTable();
                    oda.FillSchema(dt, SchemaType.Source);
                    oda.Fill(dt);
                    return (dt);
                }
            }
            catch (Exception err)
            {
                Functions.SendErrorEmail(err, "GLOBAL006 LoadContentFromText [filename:" + filename + "]");
                return null;
            }
        }



        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            try
            {
                using (ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        pck.Load(stream);
                    }
                    ExcelWorksheet ws = pck.Workbook.Worksheets.First();
                    DataTable tbl = new DataTable();
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Value;

                        }
                    }
                    return tbl;
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "GLOBAL007 GetDataTableFromExcel [path:" + path + "]");
                return null;
            }
        }
        #endregion

        public class SearchContactModel
        {
            public string SelectContactMethodeName { get; set; }
            public SearchContactModel(string SelectContactMethodeName)
            {
                this.SelectContactMethodeName = SelectContactMethodeName;
            }
        }

        /// <summary>
        /// Counting the result of a selection with the option to apply a node filter and to change the segmentation mode
        /// </summary>
        /// <param name="selectionType">Segmentation Mode</param>
        /// /// <param name="SelectionNodes">SelectionNodes</param>
        /// <param name="ExtraFilter">Nodecode of any node to be used as a filter (e.g. Emailable node)</param>
        /// <returns>It returns the count of the selection with the applied filter</returns>

        public static string AddSpacesToSentence(string Text)
        {
            return Regex.Replace(Text, "([a-z])([A-Z])", "$1 $2");
        }

        public static bool IsContainedInArray(string SearchItem, string[] arr)
        {
            bool IsContained = false;
            foreach (string item in arr)
            {
                if (SearchItem.Contains(item))
                {
                    IsContained = true;
                    break;
                }
            }
            return IsContained;
        }

        public static List<SelectListItem> ConvertStringListToSelectListItem(string[] StringList,string SelectedIDs = null)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            int Index = 0;
            List<string> SelectedIDsList = new List<string>();

            if(SelectedIDs != null && SelectedIDs.Length >= 1){
                SelectedIDsList = SelectedIDs.Split(',').ToList();
            }
            foreach (string str in StringList)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    Index++;
                    SelectListItem NewItem = new SelectListItem();
                    NewItem.Text = str;
                    NewItem.Value = Index.ToString();
                    NewItem.Selected = (SelectedIDsList.Contains(Index.ToString()));
                    list.Add(NewItem);
                }
            }
            
            return list;
        }
    }
}