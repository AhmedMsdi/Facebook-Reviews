using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Atreemo
{
    public class LoggingHelper 
    {
        public static IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
        public static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!String.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }
        public static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        public static int GetUserIDByUserName(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                UserName = HttpContext.Current.Session["model.UserName"].ToString();
            }
            var mmuser = Membership.GetUser(UserName);

            return (int)mmuser.ProviderUserKey;
        }

        public static int GetUserDefaultEnvByUserName(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                UserName = HttpContext.Current.Session["model.UserName"].ToString();
            }
            var mmuser = Membership.GetUser(UserName);
            int DefaultEnv = 0;
            using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
            {
                SqlCommand Cmd = new SqlCommand("SELECT CAST( DefaultEnv AS INT) DefaultEnv FROM aclUsers WITH (NOLOCK) WHERE UserID = " + (int)mmuser.ProviderUserKey, Cnx);
                Cmd.CommandTimeout = 3600;
                Cnx.Open();
                DefaultEnv = (int) Cmd.ExecuteScalar();
                Cnx.Close();
            }

            return DefaultEnv;
        }

        public static UserEnv GetUserEnvByUserName(string UserName)
        {
            UserEnv userEnv = new UserEnv();
            userEnv.UserName = UserName;
            using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
            {
                SqlCommand Cmd = new SqlCommand("SELECT CAST(DefaultEnv AS INT) DefaultEnv,UserID FROM aclUsers WITH (NOLOCK) WHERE UserName = '" + UserName.Replace("'", "''") + "' AND ApplicationName LIKE '" + ConfigurationManager.AppSettings["ApplicationName"].ToString() + "'", Cnx);
                Cmd.CommandTimeout = 3600;
                Cnx.Open();
                SqlDataReader Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    userEnv.UserID = (int)Dr["UserID"];
                    userEnv.DefaultEnv = (int)Dr["DefaultEnv"];
                }
                Cnx.Close();
            }

            return userEnv;
        }

        public static UserEnv GetUserEnvByUserID(int UserID)
        {
            UserEnv userEnv = new UserEnv();
            
            using (SqlConnection Cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["Atreemo_Cnx"].ConnectionString))
            {
                SqlCommand Cmd = new SqlCommand("SELECT CAST(DefaultEnv AS INT) DefaultEnv,UserID,UserName FROM aclUsers WITH (NOLOCK) WHERE UserID = " + UserID + " AND ApplicationName LIKE '" + ConfigurationManager.AppSettings["ApplicationName"].ToString() + "'", Cnx);
                Cmd.CommandTimeout = 3600;
                Cnx.Open();
                SqlDataReader Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    userEnv.UserID = (int)Dr["UserID"];
                    userEnv.DefaultEnv = (int)Dr["DefaultEnv"];
                    userEnv.UserName = (string)Dr["UserName"];
                }
                Cnx.Close();
            }

            return userEnv;
        }

        public class UserEnv
        {
            public string UserName { get; set; }
            public int UserID { get; set; }
            public int DefaultEnv { get; set; }
        }
    }
}
