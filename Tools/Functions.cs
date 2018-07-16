using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Atreemo.Models;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Web.Mvc;
using System.Net.Http.Headers;
using System.Text;
using Atreemo.Controllers;
using System.Net.Mail;
using System.Web.Http.Results;
using System.Threading;

using Atreemo.Tools;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using System.Security.Claims;

namespace Atreemo.Views.Tools
{
    public static class Functions
    {

        public static string SERVICE_URI()
        {
            string _SERVICE_URI = "";
            try
            {
                _SERVICE_URI = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("//") + 2) + HttpContext.Current.Request.Url.Authority
               + (System.Configuration.ConfigurationManager.AppSettings["SubDomainName"] != null ? "/" + System.Configuration.ConfigurationManager.AppSettings["SubDomainName"] : "") + "/API/";
            }
            catch { }
            return _SERVICE_URI;
        }

        public static bool? GetCheckStatus(int? CtcID, int? CpyID, int NodeCode, short? NodeType)
        {
            string Query = "";
            if (NodeType != null)
            {

                if (CtcID != null)
                {
                    if ((short)NodeType == 3)
                    {
                        Query = string.Format("IF EXISTS (SELECT CtcID FROM CInfo WITH (NOLOCK) WHERE CtcID = {0} AND NodeCode = {1} ) SELECT CAST(1 AS BIT) ELSE SELECT CAST(0 AS BIT)", CtcID.ToString(), NodeCode.ToString());
                    }

                }
                else if (CpyID != null)
                {
                    if ((short)NodeType == 1)
                    {
                        Query = string.Format("IF EXISTS (SELECT CpyID FROM CInfo WITH (NOLOCK) WHERE CpyID = {0} AND NodeCode = {1} ) SELECT CAST(1 AS BIT) ELSE SELECT CAST(0 AS BIT)", CpyID.ToString(), NodeCode.ToString());
                    }
                }

            }

            if (Query != "")
            {
                try
                {
                    using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                    {
                        SqlCommand Cmd = new SqlCommand(Query, Cnx);
                        Cmd.CommandTimeout = 3600;
                        Cmd.CommandType = CommandType.Text;
                        Cnx.Open();
                        bool result = (bool)Cmd.ExecuteScalar();
                        Cnx.Close();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Functions.SendErrorEmail(ex, "FUN001 GetCheckStatus [CtcID:" + (CtcID ?? 0) + ", CpyID:" + (CpyID ?? 0) + ",NodeCode:" + NodeCode + "]");
                    return false;
                }
            }
            return false;
        }

        public class ToolURLInfo
        {
            public string Name { get; set; }
            public string URL { get; set; }
        }

        public static List<ToolURLInfo> GetToolPath(string ViewName, string ControllerName)
        {
            List<ToolURLInfo> result = new List<ToolURLInfo>();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("Atreemo.[GetToolPath]", Cnx);
                    Cmd.CommandTimeout = 3600;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("ViewName", ViewName));
                    Cmd.Parameters.Add(new SqlParameter("ControllerName", ControllerName));
                    DataTable Dt = new DataTable();
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }

                    result = (from e in Dt.AsEnumerable()
                              select new ToolURLInfo()
                            {
                                Name = Functions.ConvertFromDBVal<string>(e["Name"]),
                                URL = Functions.ConvertFromDBVal<string>(e["URL"])
                            }).ToList<ToolURLInfo>();

                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN002 GetToolPath [ViewName:" + (ViewName ?? "") + ", ControllerName:" + (ControllerName ?? "") + "]");
            }
            return result;
        }


        public static bool IsSuperUser(int UserID)
        {
            int SuperUsersGroupID = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["SuperUsersGroupID"].ToString(), out SuperUsersGroupID))
            {
                return CheckUserExistanceInGroup(UserID, SuperUsersGroupID);
            }
            else
            {
                return false;
            }
        }

        public static bool IsAdminstrator(int UserID)
        {
            int SuperUsersGroupID = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["AdministratorsGroupID"].ToString(), out SuperUsersGroupID))
            {
                return CheckUserExistanceInGroup(UserID, SuperUsersGroupID);
            }
            else
            {
                return false;
            }
        }

        public static bool IsAuthorizedAction(int ActionID, int UserID)
        {
            bool IsAuthorizedAction = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    string Query = string.Format("IF EXISTS ( SELECT * FROM Atreemo.Tools_Actions_Users WITH (NOLOCK) WHERE ActionID = {0} AND (  UserID IN ( SELECT UserGroupID FROM aclUserGroups WITH (NOLOCK) WHERE UserID = {1} ) OR UserID = {1} ) )" +
                                                    "SELECT CAST(1 AS BIT) IsAuthorizedAction ELSE SELECT CAST(0 AS BIT) IsAuthorizedAction",
                                                    ActionID,
                                                    UserID
                                                    );
                    SqlCommand Cmd = new SqlCommand(Query, Cnx);
                    Cnx.Open();
                    IsAuthorizedAction = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN032 IsAuthorizedAction [ActionID:" + ActionID + ", UserID:" + UserID + "]");
            }

            return IsAuthorizedAction;
        }

        public class ToolActionAuthorization
        {
            public int ActionID { get; set; }
            public int ToolID { get; set; }
            public bool IsAuthorizedAction { get; set; }
        }


        public static List<ToolActionAuthorization> GetActionsAuthorization(int UserID, int? ToolID = null)
        {
            List<ToolActionAuthorization> ToolActions = new List<ToolActionAuthorization>();
            DataTable Dt = new DataTable();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("Atreemo.GetActionsAuthorization", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    if (ToolID != null)
                    {
                        Cmd.Parameters.Add(new SqlParameter("@ToolID", ToolID));
                    }
                    Cmd.CommandTimeout = 3600;
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN003 GetActionsAuthorization [ToolID:" + (ToolID ?? 0) + ", UserID:" + UserID + "]");
            }

            ToolActions = (from e in Dt.AsEnumerable()
                           select new ToolActionAuthorization()
                           {
                               ToolID = (int)e["ToolID"],
                               ActionID = (int)e["ActionID"],
                               IsAuthorizedAction = (bool)(((int)e["IsAuthorizedAction"]) == 1)
                           }).ToList();

            return ToolActions;
        }

        public static bool IsAuthorizedTool(int toolID, int UserID)
        {
            bool IsAuthorizedAction = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    string Query = string.Format("IF EXISTS ( SELECT * FROM Atreemo.Tools_Users WITH (NOLOCK) WHERE ToolID = {0} AND ToolID IN (SELECT ToolID FROM Atreemo.Tools WITH (NOLOCK) WHERE IsActive = 1 AND (GETDATE() >= AvailableFrom OR AvailableFrom IS NULL) AND (GETDATE() < AvailableTo OR AvailableTo IS NULL)) AND (  UserID IN ( SELECT UserGroupID FROM aclUserGroups WITH (NOLOCK) WHERE UserID = {1} ) OR UserID = {1} ) )" +
                                                    "SELECT CAST(1 AS BIT) IsAuthorizedTool ELSE SELECT CAST(0 AS BIT) IsAuthorizedTool",
                                                    toolID,
                                                    UserID
                                                    );
                    SqlCommand Cmd = new SqlCommand(Query, Cnx);
                    Cnx.Open();
                    IsAuthorizedAction = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN004 IsAuthorizedTool [ToolID:" + toolID + ", UserID:" + UserID + "]");
            }

            return IsAuthorizedAction;
        }

        private static bool CheckUserExistanceInGroup(int UserID, int GroupID)
        {
            bool DoesExists = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    string Query = string.Format("IF EXISTS ( SELECT UserID FROM aclUserGroups WHERE UserID = {0} AND UserGroupID = {1} )" +
                                                    "SELECT CAST(1 AS BIT) DoesExist ELSE SELECT CAST(0 AS BIT) DoesExist",
                                                    UserID,
                                                    GroupID
                                                    );
                    SqlCommand Cmd = new SqlCommand(Query, Cnx);
                    Cnx.Open();
                    DoesExists = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN005 CheckUserExistanceInGroup [UserID:" + UserID + ", GroupID:" + GroupID + "]");
            }

            return DoesExists;
        }

        public static void LogContentView(int Value, int UserID, int Type)
        {
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("Atreemo.[LogContentViewByUserID]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cmd.Parameters.Add(new SqlParameter("@Value", Value));
                    Cmd.Parameters.Add(new SqlParameter("@Type", Type));
                    Cnx.Open();
                    Cmd.ExecuteNonQuery();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN012 LogContentView [UserID:" + UserID + " ,Value:" + Value + " ,Type:" + Type + "]");
            }
        }

        public static void LogAction(int? ToolID, string Url, int UserID, string SessionID)
        {
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("Atreemo.Log_Tools_Activity", Cnx);
                    Cmd.CommandTimeout = 3600;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@ToolID", ToolID == null ? 0 : (int)ToolID));
                    Cmd.Parameters.Add(new SqlParameter("@Url", Url));
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cmd.Parameters.Add(new SqlParameter("@SessionID", SessionID));
                    Cnx.Open();
                    Cmd.ExecuteNonQuery();
                    Cnx.Close();

                }
            }
            catch (Exception ex)
            {
                SendErrorEmail(ex, "FUN014 LogAction [ToolID:" + (ToolID ?? 0) + " ,Url: " + Url + " ,UserID:" + UserID + "]");
            }
        }

        public static int GetSalutationID(int UserID)
        {
            int result = 1;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("Atreemo.GetSalutationMessageID", Cnx);
                    Cmd.CommandTimeout = 3600;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cnx.Open();
                    result = (int)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                SendErrorEmail(ex, "FUN015 GetSalutationID [UserID:" + UserID + "]");
            }
            return result;
        }

        public static void CopyHeaders(WebClient rootTo, NameValueCollection to, NameValueCollection from)
        {
            foreach (string header in from.AllKeys)
            {
                try
                {
                    if (header != "Connection")
                    {
                        to.Add(header, from[header]);
                    }
                }
                catch
                {
                    try
                    {
                        rootTo.GetType().GetProperty(header.Replace("-", "")).SetValue(rootTo, from[header]);
                    }
                    catch { }
                }
            }
        }

        public static string GetCultureName()
        {
            string Culture = ConfigurationManager.AppSettings["Culture"].ToString();
            return Culture;
        }

        public class NodeLabels
        {
            public string ToolTip { get; set; }
            public string Text { get; set; }
        }

        public static NodeLabels GetNodeInfoByNodeCode(int Nodecode)
        {
            NodeLabels result = new NodeLabels();
            string SelectCmd = " DECLARE @Str as varchar(255),@NodeName VARCHAR(255) SET @Str = '' ";
            SelectCmd += " SELECT @NodeName = ShortLabelE, @Str = @Str + COALESCE(CASE WHEN LEN(ShortLabelE) > 50 THEN SUBSTRING(ShortLabelE,0,48) ";
            SelectCmd += "+ '...' ELSE ShortLabelE END + ' / ' , '')  From R_Info RI join ( ";
            SelectCmd += " SELECT * fROM dbo.fn_GetNodeParents('R_Info'," + Nodecode + ") Where Pnc > 1 ";
            SelectCmd += " ) X on RI.NodeCode = X.Pnc order by X.Idx desc SELECT SUBSTRING(@Str,0,LEN(@Str)) ToolTip , @NodeName NodeName";
            SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ToString());
            SqlCommand Cmd = new SqlCommand(SelectCmd, Cnx);
            Cmd.CommandTimeout = 6000;
            using (Cnx)
            {
                Cnx.Open();
                SqlDataReader Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string Tooltip = Dr["ToolTip"].ToString();
                    result.Text = Dr["NodeName"].ToString();
                    Tooltip = Tooltip.Replace(" / ", "<span style='color:#8b1c14;font-weight:bold;font-size:13pt'>&nbsp;/&nbsp;</span>").Trim();
                    result.ToolTip = Tooltip;
                }
                Dr.Close();
                Cnx.Close();
            }
            return result;
        }

        public static void DropTableByName(string name)
        {
            try
            {

                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand(string.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP TABLE {0}", name), Cnx);
                    Cmd.CommandTimeout = 3600;
                    Cnx.Open();
                    Cmd.ExecuteNonQuery();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                SendErrorEmail(ex, "FUN017 DropTableByName [TableName:" + name + "]");
            }
        }

        public static bool GetEnvCubeFilterTableByUserID(int UserID, bool IsCtcSeg)
        {
            bool result = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("SELECT EnvCube" + (IsCtcSeg ? "Ctc" : "") + "ListUpdated FROM aclUsers WITH (NOLOCK) WHERE UserID = " + UserID, Cnx);
                    Cmd.CommandType = CommandType.Text;
                    Cnx.Open();
                    result = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN019 GetEnvCubeFilterTableByUserID [UserID:" + UserID + " ,IsCtcSeg:" + (IsCtcSeg ? "Ctc" : "Cpy") + "]");
            }
            return result;

        }

        public static void SendErrorEmail(Exception err, string ErrorCode = null)
        {
            //string MyMailMessageBody = "<p style='font-family: Arial;'><b>Subject:</b><br/>"
            //                         + (err.Message ?? "")
            //                         + "<br/><br/><b>SQL database name:</b>" + ConfigurationManager.AppSettings["SQLDbName"].ToString()
            //                         + "<br/><b>SQL server name:</b>" + ConfigurationManager.AppSettings["SQLServerName"].ToString()
            //                         + "<br/><b>Customer code:</b>" + ConfigurationManager.AppSettings["CustomerCode"].ToString()
            //                         + "<br/><b>Ecast database name:</b>" + ConfigurationManager.AppSettings["EcastDbName"].ToString()
            //                         + "<br/><b>Ecast server name:</b>" + ConfigurationManager.AppSettings["EcastServerName"].ToString()
            //                         + "<br/><b>Date:</b>" + DateTime.Now.ToLongDateString() ;
            //if (!string.IsNullOrEmpty(ErrorCode))
            //{
            //    MyMailMessageBody += "<br/><b>Error Code:</b>" + ErrorCode;
            //}
            //try
            //{
            //    MyMailMessageBody += "<br/><b>URL:</b>&nbsp;" + ((System.Web.HttpContext.Current.Request.Url != null) ? System.Web.HttpContext.Current.Request.Url.ToString() : "");
            //}
            //catch
            //{

            //}
            //if (err.StackTrace != null)
            //{
            //    MyMailMessageBody += "<br/><br/><b>Stack Trace:</b><br/>" + err.StackTrace.ToString();

            //    //Get a StackTrace object for the exception
            //    StackTrace st = new StackTrace(err, true);

            //    //Get the first stack frame
            //    StackFrame frame = st.GetFrame(0);

            //    if (frame != null)
            //    {
            //        //Get the file name
            //        string fileName = frame.GetFileName();
            //        MyMailMessageBody += "<br/><br/>File name:" + fileName ?? "";

            //        //Get the method name
            //        string methodName = frame.GetMethod().Name;
            //        MyMailMessageBody += "<br/><br/>Method name:" + methodName ?? "";

            //        //Get the line number from the stack frame
            //        int line = frame.GetFileLineNumber();
            //        MyMailMessageBody += "<br/><br/>Line:" + line ?? "";

            //        //Get the column number
            //        int col = frame.GetFileColumnNumber();
            //        MyMailMessageBody += "<br/><br/>Col:" + col ?? "";
            //    }
            //}

            //MyMailMessageBody += "</p>";
            //AtreemoServiceTool.SendErrorEmail(err,
            //                                  ConfigurationManager.AppSettings["ErrorToEmails"],
            //                                  ConfigurationManager.AppSettings["ErrorFromAddress"],
            //                                  MyMailMessageBody,
            //                                  ConfigurationManager.AppSettings["ErrorHostIP"]);
        }

        public class SearchPreferences
        {
            public bool IsFullDBSearchEnabled { get; set; }
            public bool SearchOnlyForAffectedToContacts { get; set; }
            public bool SearchOnlyForAffectedToCompanies { get; set; }
            public SearchPreferences()
            {
                this.IsFullDBSearchEnabled = true;
                this.SearchOnlyForAffectedToContacts = true;
                this.SearchOnlyForAffectedToCompanies = true;
            }
        }

        public static SearchPreferences GetUserSearchPreferences(int UserID)
        {
            SearchPreferences searchPreferences = new SearchPreferences();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("SELECT EnableFullDBSearch , SearchOnlyForAffectedToContacts , SearchOnlyForAffectedToCompanies FROM aclUsers WHERE UserID = " + UserID, Cnx);
                    Cmd.CommandType = CommandType.Text;
                    Cnx.Open();
                    SqlDataReader Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        searchPreferences.IsFullDBSearchEnabled = (bool)Dr["EnableFullDBSearch"];
                        searchPreferences.SearchOnlyForAffectedToContacts = (bool)Dr["SearchOnlyForAffectedToContacts"];
                        searchPreferences.SearchOnlyForAffectedToCompanies = (bool)Dr["SearchOnlyForAffectedToCompanies"];
                    }

                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                SendErrorEmail(ex, "FUN020 GetUserSearchPreferences [UserID:" + UserID + "]");
            }
            return searchPreferences;

        }

        public static User GetUserByUserName(string UserName)
        {
            User user = new User();
            DataTable Dt = new DataTable();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[GetUserByUserName]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                    Cmd.Parameters.Add(new SqlParameter("@ApplicationName", ConfigurationManager.AppSettings["ApplicationName"].ToString()));
                    Cmd.CommandTimeout = 3600;
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN0038 GetUserByUserName [UserName:" + UserName + "]");
            }
            List<User> list = new List<User>();
            list = (from e in Dt.AsEnumerable()
                    select new User()
                    {
                        UserID = (int)e["UserID"],
                        _Title = Functions.ConvertFromDBVal<string>(e["Title"]),
                        Username = (string)e["UserName"],
                        FirstName = Functions.ConvertFromDBVal<string>(e["FirstName"]),
                        LastName = Functions.ConvertFromDBVal<string>(e["LastName"]),
                        Phone = Functions.ConvertFromDBVal<string>(e["Phone"]),
                        MobilPhone = Functions.ConvertFromDBVal<string>(e["MobilePhone"]),
                        Email = Functions.ConvertFromDBVal<string>(e["Email"]),
                        PreferedLanguageID = Functions.ConvertFromDBVal<int>(e["PreferedLanguageID"]),
                        DefaultEnv = Functions.ConvertFromDBVal<int>(e["DefaultEnv"]),
                        CreationDate = Functions.ConvertFromDBVal<DateTime?>(e["CreationDate"]),
                        LastActivityDate = Functions.ConvertFromDBVal<DateTime?>(e["LastActivityDate"]),
                        LastLoginDate = Functions.ConvertFromDBVal<DateTime?>(e["LastLoginDate"]),
                        LastPasswordChangedDate = Functions.ConvertFromDBVal<DateTime?>(e["LastPasswordChangedDate"]),
                        Gender = Functions.ConvertFromDBVal<string>(e["Gender"]),
                        Password = (string)e["Password"],
                        UserFullName = Functions.ConvertFromDBVal<string>(e["UserFullName"]),
                        BirthDate = Functions.ConvertFromDBVal<DateTime?>(e["BirthDate"]),
                        ApproverUserID = string.IsNullOrEmpty(Functions.ConvertFromDBVal<string>(e["ApproverUserID"])) ? null : Functions.ConvertFromDBVal<string>(e["ApproverUserID"]).Split(',').Select(Int32.Parse).ToList(),
                        IsFinalApprover = Functions.ConvertFromDBVal<bool>(e["IsFinalApprover"]),
                        AwayFrom = Functions.ConvertFromDBVal<DateTime?>(e["AwayFrom"]),
                        AwayTo = Functions.ConvertFromDBVal<DateTime?>(e["AwayTo"]),
                        ForwardTo = Functions.ConvertFromDBVal<int?>(e["ForwardTo"]),
                        EventSupervisedBy = Functions.ConvertFromDBVal<int?>(e["EventSupervisedBy"]),
                        IsEventFinalApprover = Functions.ConvertFromDBVal<bool?>(e["IsEventFinalApprover"]),
                        PageID = Functions.ConvertFromDBVal<int?>(e["AtreemoHomePageID"]),
                        PageURL = Functions.ConvertFromDBVal<string>(e["AtreemoHomePageURL"]),
                        PageActionName = Functions.ConvertFromDBVal<string>(e["AtreemoHomePageActionName"]),
                        PageControllerName = Functions.ConvertFromDBVal<string>(e["AtreemoHomePageControllerName"]),
                        PageLastModifiedOn = Functions.ConvertFromDBVal<DateTime?>(e["AtreemoHomePageLastModifiedOn"]),
                        NeverExpires = Functions.ConvertFromDBVal<bool>(e["NeverExpires"]),
                        RemainingDaysBeforeExpiry = Functions.ConvertFromDBVal<int?>(e["RemainingDaysBeforeExpiry"]),
                    }).ToList<User>();

            if (list.Count == 1)
            {
                user = list[0];
            }
            return user;

        }

        public static string GetUserFullNameByUserID(int UserID)
        {
            string UserFullName = "";
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("SELECT COALESCE(UserFullName,UserName) FROM aclUsers WITH (NOLOCK) WHERE UserID = " + UserID, Cnx);
                    Cmd.CommandType = CommandType.Text;
                    Cnx.Open();
                    UserFullName = (string)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN0033 GetUserFullNameByUserID [UserID:" + UserID + "]");
            }
            return UserFullName;

        }

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                if (obj is string)
                {
                    obj = "";
                    return (T)obj;
                }
                else
                {
                    return default(T); // returns the default value for the type
                }
            }
            else
            {
                return (T)obj;
            }
        }

        public static string EmptyIfNullString(string obj)
        {
            return string.IsNullOrEmpty(obj) ? "" : obj;
        }


        #region Json
        const int MaxJsonLength = 2147483644;


        public static System.Web.Mvc.JsonResult LargeJson(object data)
        {
            return new System.Web.Mvc.JsonResult()
            {
                Data = data,
                MaxJsonLength = MaxJsonLength,
            };
        }
        public static System.Web.Mvc.JsonResult LargeJson(object data, System.Web.Mvc.JsonRequestBehavior behavior)
        {
            return new System.Web.Mvc.JsonResult()
            {
                Data = data,
                JsonRequestBehavior = behavior,
                MaxJsonLength = MaxJsonLength
            };
        }
        #endregion

        static public IEnumerable<T> Descendants<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> descendBy)
        {
            if (source != null)
            {
                foreach (T value in source)
                {
                    yield return value;

                    if (descendBy(value) != null)
                    {
                        foreach (T child in descendBy(value).Descendants<T>(descendBy))
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        public static DataTable GetMarketingProjectsByIdDT(int ID, int UserID, bool IsCtcID)
        {
            DataTable Dt = new DataTable();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[GetMarketingProjectsByID]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cmd.Parameters.Add(new SqlParameter("@EcastDbName", ConfigurationManager.AppSettings["EcastDbName"].ToString()));
                    if (!ConfigurationManager.AppSettings["SQLServerName"].Equals(ConfigurationManager.AppSettings["EcastServerName"]))
                    {
                        Cmd.Parameters.Add(new SqlParameter("@EcastServerName", ConfigurationManager.AppSettings["EcastServerName"].ToString()));
                    }
                    Cmd.Parameters.Add(new SqlParameter("@IsCtcID", IsCtcID));
                    Cmd.CommandTimeout = 3600;
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex);
            }
            return Dt;
        }

        public static DataTable GetSurveyResponsesByCtcIdDT(int CtcID)
        {
            DataTable Dt = new DataTable();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[GetSurveyReponsesByCtcID]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@CtcID", CtcID));
                    Cmd.Parameters.Add(new SqlParameter("@SurveyURL", ConfigurationManager.AppSettings["LinkSurvey"].ToString()));
                    Cmd.CommandTimeout = 3600;
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex);
            }
            return Dt;
        }

        public static List<SelectListItem> GetAllCountryCode(bool ShowEmptyValue = false)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            DataTable Dt = new DataTable();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[GetAllCountryCode]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }
                    SelectListItem l = new SelectListItem();

                    if (ShowEmptyValue)
                    {
                        list.Add(new SelectListItem
                        {
                            Value = "",
                            Text = "-- Please Select --"
                        });
                    }

                    foreach (DataRow Dr in Dt.Rows)
                    {
                        list.Add(new SelectListItem
                        {
                            Value = Dr["CountryCode"].ToString(),
                            Text = Dr["Country"].ToString()
                        });
                    }
                }
            }
            catch (Exception err)
            {
                SendErrorEmail(err, "FUN0045 GetAllCountryCode [ShowEmptyValue:" + ShowEmptyValue + "]");
            }
            return list;
        }

        public static List<SelectListItem> GetAccountManagersByGroupID(int UserID)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            DataTable Dt = new DataTable();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[GetAccountManagersByGroupID]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    int AccountManagerGroupID = 0;
                    if (ConfigurationManager.AppSettings["AccountManagerGroupID"] != null)
                    {
                        if (int.TryParse(ConfigurationManager.AppSettings["AccountManagerGroupID"], out AccountManagerGroupID))
                        {
                            Cmd.Parameters.Add(new SqlParameter("@GroupID", AccountManagerGroupID));
                        }
                    }
                    bool IsToshibaSpecific = ((System.Configuration.ConfigurationManager.AppSettings["ToshibaSpecific"] != null) ? (System.Configuration.ConfigurationManager.AppSettings["ToshibaSpecific"].ToString().ToLower() == "yes") : false);
                    if (IsToshibaSpecific)
                    {
                        Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));

                    }
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }

                    foreach (DataRow Dr in Dt.Rows)
                    {
                        list.Add(new SelectListItem
                        {
                            Value = Dr["UserID"].ToString(),
                            Text = Dr["UserFullName"].ToString()
                        });
                    }
                }
            }
            catch (Exception err)
            {
                SendErrorEmail(err, "FUN0046 GetAccountManagersByGroupID [UserID:" + UserID + "]");
            }
            return list;
        }

        public static bool IsCompanyEditableByUserID(int UserID, int CpyID)
        {
            bool IsCompanyEditable = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    Cnx.Open();
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[UserIsCreatorOrAffectedToCompany]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cmd.Parameters.Add(new SqlParameter("@CpyID", CpyID));
                    bool ManyAccountsPerCompanyEnabled = (System.Configuration.ConfigurationManager.AppSettings["ManyAccountsPerCompany"] != null) ? System.Configuration.ConfigurationManager.AppSettings["ManyAccountsPerCompany"].ToLower().Equals("yes") : false;
                    Cmd.Parameters.Add(new SqlParameter("@ManyAccountsPerCompany", ManyAccountsPerCompanyEnabled));
                    IsCompanyEditable = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception err)
            {
                SendErrorEmail(err, "FUN0047 IsCompanyEditableByUserID [UserID:" + UserID + " ,CpyID:" + CpyID + "]");
            }
            return IsCompanyEditable;
        }

        public static bool IsContactEditableByUserID(int UserID, int CtcID)
        {
            bool IsContactEditable = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    Cnx.Open();
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[UserIsCreatorOfContact]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cmd.Parameters.Add(new SqlParameter("@CtcID", CtcID));
                    IsContactEditable = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception err)
            {
                SendErrorEmail(err, "FUN0048 IsContactEditableByUserID [UserID:" + UserID + " ,CtcID:" + CtcID + "]");
            }
            return IsContactEditable;
        }

        public static bool LockUserAccountIfPasswordIsExpired(string UserName)
        {
            bool IsAccountLockedOut = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    Cnx.Open();
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[LockUserAccount_PasswordExpired]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                    IsAccountLockedOut = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception err)
            {
                SendErrorEmail(err, "FUN0049 LockUserAccountIfPasswordIsExpired [UserName:" + UserName + "]");
            }
            return IsAccountLockedOut;
        }

        public static bool IsUserLockedOut(string UserName, string Password)
        {
            bool IsAccountLockedOut = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    Cnx.Open();
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[IsUserLockedOut]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                    Cmd.Parameters.Add(new SqlParameter("@Password", Password));
                    IsAccountLockedOut = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception err)
            {
                SendErrorEmail(err, "FUN0050 IsUserLockedOut [UserName:" + UserName + "]");
            }
            return IsAccountLockedOut;
        }

        public static bool UserHasSite(string UserName)
        {
            bool HasSite = false;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["LowCost_Cnx"].ConnectionString))
                {
                    Cnx.Open();
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[UserHasSite]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                    HasSite = (bool)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception err)
            {
                SendErrorEmail(err, "FUN0053 UserHasSite [UserName:" + UserName + "]");
            }
            return HasSite;
        }

        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static bool bulkCopy(DataTable dataToInsert, string destinationTable)
        {
            bool Response = true;
            using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
            {
                try
                {

                    Cnx.Open();
                    SqlBulkCopy BulkData = new SqlBulkCopy(Cnx);
                    foreach (DataColumn aCol in dataToInsert.Columns)
                    {
                        BulkData.ColumnMappings.Add(aCol.ColumnName, aCol.ColumnName);
                    }

                    BulkData.DestinationTableName = destinationTable;
                    BulkData.WriteToServer(dataToInsert);
                    BulkData.Close();
                    dataToInsert.Dispose();
                }
                catch (Exception ex)
                {
                    SendErrorEmail(ex);
                    Response = false;
                }
                finally
                {
                    Cnx.Close();
                    Cnx.Dispose();
                }
            }

            return Response;
        }

        public static bool UpdateTemplatesByGroup(int GroupID, string Operation, int EmailID, int UserID)
        {
            bool res = true;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    Cnx.Open();
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[UpdateTemplatesByGroup]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@GroupID", GroupID));
                    Cmd.Parameters.Add(new SqlParameter("@EmailID", EmailID));
                    Cmd.Parameters.Add(new SqlParameter("@Commend", Operation));
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cmd.CommandTimeout = 3600;
                    Cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN0052 UpdateTemplatesByGroup [GroupID:" + GroupID + " ,EmailID:" + EmailID + " ,UserID:" + UserID + " ,Commend:" + Operation + "]");
                res = false;
            }
            return res;
        }

        public static List<string> StringToStringList(string StringWithComma)
        {
            if (StringWithComma == null)
                return new List<string>();
            return (StringWithComma.Split(',').ToList());
        }

        public static void LogApi(string XmlParameter, string MethodName, string Error, string XmlOutput, string Controller, int UserID, DateTime? Date = null)
        {
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    Cnx.Open();
                    SqlCommand SqlCmd = new SqlCommand();
                    SqlCmd.Connection = Cnx;
                    SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlCmd.CommandText = "[Atreemo].[LogApi]";
                    SqlCmd.Parameters.Add(new SqlParameter("@XmlParameter", XmlParameter));
                    SqlCmd.Parameters.Add(new SqlParameter("@MethodName", MethodName));
                    SqlCmd.Parameters.Add(new SqlParameter("@XmlOutput", XmlOutput));
                    SqlCmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    SqlCmd.Parameters.Add(new SqlParameter("@Controller", Controller));
                    SqlCmd.Parameters.Add(new SqlParameter("@Error", Error));
                    SqlCmd.Parameters.Add(new SqlParameter("@CallDate", Date));
                    SqlCmd.CommandTimeout = 3600;
                    SqlCmd.ExecuteNonQuery();
                    Cnx.Close();

                }
            }
            catch (Exception ex)
            {
                SendErrorEmail(ex, "FUN014 LogAPI [Controller:" + Controller + " ,Method: " + MethodName + " ,UserID:" + UserID + "]");
            }
        }

        public static string ToXML(object Obj)
        {
            string Value = "";
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(Obj.GetType());
                serializer.Serialize(stringwriter, Obj);
                Value = stringwriter.ToString();
            }
            catch
            {
                Value = "";
            }
            return Value;
        }

        public static string IsCampaignSlpConfigured()
        {
            string Link = "";
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[GetSlpSettings]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    DataTable Dt = new DataTable();
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }

                    if (Dt != null && Dt.Rows.Count > 0)
                    {
                        Link = ConvertFromDBVal<string>(Dt.Rows[0]["Surveylink"]);
                    }


                }
            }
            catch (Exception err)
            {
                Link = "";
                SendErrorEmail(err, "FUN0050 IsCampaignSlpConfigured");
            }
            return Link;
        }

        public static bool VerifyDataTable(DataTable Data)
        {
            if (Data != null && Data.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public static int AddAdfsUser(IEnumerable<Claim> Claims, string defPassword, bool IsActiveDirectory)
        {
            int userID = 0;
            string userUPN = "", userEmail = "", userFullName = "";
            try
            {
                userUPN = Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                userEmail = Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                userFullName = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    string StrUserGroups = "";
                    string SenderVenuesgroups = "";
                    string SenderVenues = "";
                    foreach (Claim C in Claims)
                    {
                        if ((C.Type == ClaimTypes.Role) && (C.Value.StartsWith("AtreemoGroup_")))
                        {
                            if (StrUserGroups == "")
                                StrUserGroups = C.Value.Replace("AtreemoGroup_", "").Replace("_", " ");
                            else
                                StrUserGroups += "," + C.Value.Replace("AtreemoGroup_", "").Replace("_", " ");
                        }
                        if (C.Type.ToLower() == ConfigurationManager.AppSettings["ida:SenderVenuesgroups"].ToLower())
                        {
                            SenderVenuesgroups = C.Value;
                        }
                        if (C.Type.ToLower() == ConfigurationManager.AppSettings["ida:SenderVenues"].ToLower())
                        {
                            SenderVenues = C.Value;
                        }
                        if (C.Type.ToLower() == ConfigurationManager.AppSettings["ida:Title"].ToLower())
                        {
                            if (!string.IsNullOrEmpty(C.Value))
                            {
                                userFullName += " - " + C.Value;
                            }
                        }
                    }
                    if (StrUserGroups == "")
                        return 0;
                    //StrUserGroups += ")";

                    string EncodedPassword = new MssProvider().EncodePassword(defPassword);

                    Cnx.Open();
                    SqlCommand SqlCmd = new SqlCommand();
                    SqlCmd.Connection = Cnx;
                    SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlCmd.CommandText = "[Atreemo].[CreateMemberShipUserForActiveDirectory]";
                    SqlCmd.Parameters.Add(new SqlParameter("@UserName", userUPN));
                    SqlCmd.Parameters.Add(new SqlParameter("@UserEmail", userEmail));
                    SqlCmd.Parameters.Add(new SqlParameter("@UserFullName", userFullName));
                    SqlCmd.Parameters.Add(new SqlParameter("@Password", EncodedPassword));
                    SqlCmd.Parameters.Add(new SqlParameter("@RoleGroups", StrUserGroups));
                    SqlCmd.Parameters.Add(new SqlParameter("@SenderVenues", SenderVenues));
                    SqlCmd.Parameters.Add(new SqlParameter("@SenderVenuesGroup", SenderVenuesgroups));
                    SqlCmd.Parameters.Add(new SqlParameter("@IsActiveDirectory", IsActiveDirectory));
                    SqlCmd.Parameters.Add(new SqlParameter("@ApplicationName", ConfigurationManager.AppSettings["ApplicationName"]));

                    userID = (int)SqlCmd.ExecuteScalar();
                    Cnx.Close();

                    #region Calims Log

                    DataTable ClaimsLog = new DataTable();
                    ClaimsLog.Columns.Add("UserID");
                    ClaimsLog.Columns.Add("ClaimName");
                    ClaimsLog.Columns.Add("ClaimValue");

                    foreach (Claim C in Claims)
                    {
                        ClaimsLog.Rows.Add(userID, C.Type, C.Value);
                    }

                    Functions.bulkCopy(ClaimsLog, "Atreemo.ActiveDirectoryClaimsLog");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "ADFS Add User to Membership [UserName:" + userUPN + "," + userFullName + "]");
            }

            return userID;
        }

        public static int UpdateAdfsClaims(IEnumerable<Claim> Claims)
        {
            int userID = 0;
            string userUPN = "", userEmail = "", userFullName = "";
            try
            {



                userUPN = Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                userEmail = Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                userFullName = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    string StrUserGroups = "";
                    string SenderVenuesgroups = "";
                    string SenderVenues = "";
                    foreach (Claim C in Claims)
                    {
                        if ((C.Type == ClaimTypes.Role) && (C.Value.StartsWith("AtreemoGroup_")))
                        {
                            if (StrUserGroups == "")
                                StrUserGroups = C.Value.Replace("AtreemoGroup_", "").Replace("_", " ");
                            else
                                StrUserGroups += "," + C.Value.Replace("AtreemoGroup_", "").Replace("_", " ");
                        }
                        if (C.Type.ToLower() == ConfigurationManager.AppSettings["ida:SenderVenuesgroups"].ToLower())
                        {
                            SenderVenuesgroups = C.Value;
                        }
                        if (C.Type.ToLower() == ConfigurationManager.AppSettings["ida:SenderVenues"].ToLower())
                        {
                            SenderVenues = C.Value;
                        }
                        if (C.Type.ToLower() == ConfigurationManager.AppSettings["ida:Title"].ToLower())
                        {
                            if (!string.IsNullOrEmpty(C.Value))
                            {
                                userFullName += " - " + C.Value;
                            }
                        }
                    }
                    if (StrUserGroups == "")
                        return 0;
                    //StrUserGroups += ")";

                    Cnx.Open();
                    SqlCommand SqlCmd = new SqlCommand();
                    SqlCmd.Connection = Cnx;
                    SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlCmd.CommandText = "[Atreemo].[UpdateMemberShipUserForActiveDirectory]";
                    SqlCmd.Parameters.Add(new SqlParameter("@UserName", userUPN));
                    SqlCmd.Parameters.Add(new SqlParameter("@UserEmail", userEmail));
                    SqlCmd.Parameters.Add(new SqlParameter("@UserFullName", userFullName));
                    SqlCmd.Parameters.Add(new SqlParameter("@RoleGroups", StrUserGroups));
                    SqlCmd.Parameters.Add(new SqlParameter("@SenderVenues", SenderVenues));
                    SqlCmd.Parameters.Add(new SqlParameter("@SenderVenuesGroup", SenderVenuesgroups));
                    SqlCmd.Parameters.Add(new SqlParameter("@ApplicationName", ConfigurationManager.AppSettings["ApplicationName"]));

                    userID = (int)SqlCmd.ExecuteScalar();
                    Cnx.Close();

                    #region Calims Log

                    DataTable ClaimsLog = new DataTable();
                    ClaimsLog.Columns.Add("UserID");
                    ClaimsLog.Columns.Add("ClaimName");
                    ClaimsLog.Columns.Add("ClaimValue");

                    foreach (Claim C in Claims)
                    {
                        ClaimsLog.Rows.Add(userID, C.Type, C.Value);
                    }

                    Functions.bulkCopy(ClaimsLog, "Atreemo.ActiveDirectoryClaimsLog");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "ADFS Add User to Membership [UserName:" + userUPN + "," + userFullName + "]");
            }

            return userID;
        }

        public static bool ValidateDatatable(DataTable Dt)
        {
            if (Dt != null && Dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public static string RTFToHTML(string ServerPath, string textRTF, bool IsHTML = true)
        {
            if (textRTF != null && textRTF.StartsWith(@"{\rtf1\"))
            {
                string TextHtml = "";
                try
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(textRTF);
                    MemoryStream stream = new MemoryStream(byteArray);
                    Syncfusion.DocIO.DLS.WordDocument document = null;
                    document = new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Rtf);

                    string S = System.Guid.NewGuid().ToString();
                    string _id = S.GetHashCode().ToString("x");

                    if (File.Exists(ServerPath + "\\Html2Rtf_" + _id + ".doc"))
                    {
                        File.Delete(ServerPath + "\\Html2Rtf_" + _id + ".doc");
                    }
                    document.Save(ServerPath + "\\Html2Rtf_" + _id + ".doc", Syncfusion.DocIO.FormatType.Doc);

                    Syncfusion.DocIO.DLS.WordDocument document1 = null;
                    if (IsHTML)
                        document1 = new Syncfusion.DocIO.DLS.WordDocument(ServerPath + "\\Html2Rtf_" + _id + ".doc", Syncfusion.DocIO.FormatType.Doc);
                    else
                        document1 = new Syncfusion.DocIO.DLS.WordDocument(ServerPath + "\\Html2Rtf_" + _id + ".txt", Syncfusion.DocIO.FormatType.Txt);

                    Syncfusion.DocIO.DLS.HTMLExport htmlExport = new Syncfusion.DocIO.DLS.HTMLExport();
                    MemoryStream stream1 = new MemoryStream();
                    htmlExport.SaveAsXhtml(document1, stream1);
                    TextHtml = Encoding.ASCII.GetString(stream1.ToArray());
                    TextHtml = TextHtml.Substring(3);
                    TextHtml = TextHtml.Replace("??", "");
                }
                catch (Exception ex)
                {
                    Functions.SendErrorEmail(ex);
                }
                return TextHtml;
            }
            else
                return textRTF;

        }

        public static string HTMLtoRTF(string textHTML)
        {
            string rtfText = "";
            try
            {
                textHTML = textHTML.Replace("£", "\"");
                string textHTML2 = textHTML.Replace("<!--?xml version=\"1.0\" encoding=\"utf-8\"?-->", "");
                textHTML2 = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\">" + textHTML2 + "</html>";

                textHTML2 = textHTML2.Replace("<meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml; charset=utf-8\">", "");
                textHTML2 = textHTML2.Replace("<meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml\"; charset=\"utf-8\">", "");
                textHTML2 = textHTML2.Replace("<meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml;\" charset=\"utf-8\">", "");
                SautinSoft.HtmlToRtf h = new SautinSoft.HtmlToRtf();
                h.PageStyle.PageSize.Letter();
                h.PageStyle.PageMarginLeft.Mm(20f);


                string word = "";
                word = h.ConvertString(textHTML2);

                rtfText = word + "";
                rtfText = rtfText.Replace("________________________________________________________", "");
                rtfText = rtfText.Replace("\\par{\\b Trial version converts only first 100000 characters. Evaluation only.\\line Converted by HTML-to-RTF Pro DLL .Net 5.1.10.14.\\line {\\i(Licensed version doesn't display this notice!)}\\line\\par", "");
                rtfText = rtfText.Replace("\\cf2- {\\field\\fldedit{\\*\\fldinst { HYPERLINK \"http://www.sautinsoft.com/products/html-to-rtf/order.php\" }}{\\fldrslt {\\ul Get license for the HTML-to-RTF Pro DLL .Net}}}}", "");
                rtfText = rtfText.Replace("\\pard\\page\\par", "");
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex);
            }
            return rtfText;
        }

        public static DataTable GetSalesForceDetails(int? PrjmkhID, int? SenderProfileID)
        {
            DataTable Dt = new DataTable();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[GetSalesForceDetails]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@PrjmkhID", PrjmkhID));
                    Cmd.Parameters.Add(new SqlParameter("@SenderProfileID", SenderProfileID));
                    Cmd.CommandTimeout = 3600;
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex, "FUN0501 GetSalesForceDetails");
            }

            return Dt;
        }

        public static int GetCampaignsFromDateFilter(int UserID)
        {
            int? BrandID = Atreemo.Views.Tools.Functions.GetBrandIDFromConnectedUser(UserID);
            int result = 1;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("Atreemo.GetCampaignsFromDateFilter", Cnx);
                    Cmd.CommandTimeout = 3600;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("BrandID", BrandID == null ? DBNull.Value : (object)BrandID);
                    Cnx.Open();
                    result = (int)Cmd.ExecuteScalar();
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                SendErrorEmail(ex, "FUN0502 GetCampaignsFromDateFilter");
            }
            return result;
        }

        public static int? GetBrandIDFromConnectedUser(int UserID)
        {
            int? BrandID = null;
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[GetBrandIDByUserID]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    Cmd.Parameters.Add(new SqlParameter("@ApplicationName", ConfigurationManager.AppSettings["ApplicationName"].ToString()));
                    Cmd.CommandTimeout = 3600;
                    Cnx.Open();
                    BrandID = !string.IsNullOrEmpty(Cmd.ExecuteScalar().ToString()) ? (int?)Cmd.ExecuteScalar() : (int?)null;
                    Cnx.Close();
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex);
            }
            return BrandID;
        }

        public static bool BrandsExists()
        {
            bool HasBrand = false;
            DataTable Dt = new DataTable();
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[Get_BrandList]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandTimeout = 3600;
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }
                }
                if (Dt != null && Dt.Rows.Count > 0)
                {
                    HasBrand = true;
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex);
            }
            return HasBrand;
        }

        public static DataTable GetBrandsDatatable()
        {
            DataTable Dt = new DataTable();
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("[Atreemo].[Get_BrandList]", Cnx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandTimeout = 3600;
                    using (SqlDataAdapter Adapter = new SqlDataAdapter(Cmd))
                    {
                        Adapter.Fill(Dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex);
            }
            return Dt;
        }

        public static List<SelectListItem> GetBrands_DDL()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                foreach (DataRow e in Functions.GetBrandsDatatable().Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Value = Functions.ConvertFromDBVal<string>(e["Value"].ToString()),
                        Text = Functions.ConvertFromDBVal<string>(e["Text"].ToString())
                    });
                }

            }
            catch (Exception ex)
            {
                Functions.SendErrorEmail(ex);
            }
            return list;
        }

    }
}