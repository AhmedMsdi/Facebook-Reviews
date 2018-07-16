using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Kendo.Mvc.Extensions;
using TweetSharp;
using Facebook;
using System.Data.Entity;
using Atreemo.Models;
using Newtonsoft.Json;
using Atreemo.DAL;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using static Atreemo.Models.Rating;
using Kendo.Mvc.UI;

namespace Atreemo.Controllers
{
    public class HomeController : Controller
    {
       

        public static string iduserG;
        public static fbuser fuserG ;
        // GET: Home
        public ActionResult Index()
        {
           return View();
        }



        private Uri RediredtUri

        {

            get

            {

                var uriBuilder = new UriBuilder(Request.Url);

                uriBuilder.Query = null;

                uriBuilder.Fragment = null;

                uriBuilder.Path = Url.Action("FacebookCallback");

                return uriBuilder.Uri;

            }

        }




        [AllowAnonymous]

        public ActionResult Facebook()

        {

            var fb = new FacebookClient();

            var loginUrl = fb.GetLoginUrl(new

            {




                client_id = "2253354658274452",

                client_secret = "35cf0272817393adb4c8d253f5dbe7ad",

                redirect_uri = RediredtUri.AbsoluteUri,

                response_type = "code",

                scope = "email"



            });

            return Redirect(loginUrl.AbsoluteUri);

        }

        public ActionResult FacebookCallback(string code)

        {

            var fb = new FacebookClient();

            dynamic result = fb.Post("oauth/access_token", new

            {


                client_id = "2253354658274452",

                client_secret = "35cf0272817393adb4c8d253f5dbe7ad",

                redirect_uri = RediredtUri.AbsoluteUri,

                code = code


            });

            var accessToken = result.access_token;
            var accesToken = result.access_token; //short lived acces Token
            dynamic result2 = fb.Get("https://graph.facebook.com/v3.0/oauth/access_token?grant_type=fb_exchange_token&client_id=2253354658274452&client_secret=35cf0272817393adb4c8d253f5dbe7ad&fb_exchange_token=" + accesToken);
            var accesToken2 = result2.access_token; // long lived acces Token
            Session["AccessToken"] = accesToken2;
            fb.AccessToken = accesToken2;
            dynamic me1 = fb.Get("me");
            string iduser = me1.id;

            dynamic result3 = fb.Get(" https://graph.facebook.com/v3.0/me?fields=accounts&access_token" + accesToken2);

            var pages = result3.accounts.data;
            int length = pages.Count;
           /* string id = string.Empty;
            for (int i = 0; i < length; i++)
            {
                if (pages[i] != null)
                { id = pages[i].id; }

            }


            dynamic result4 = fb.Get(" https://graph.facebook.com/v3.0/" + id + "?fields=access_token&access_token=" + accesToken2);
            var accesTokenPage = result4.access_token; //page acces Token aves expiration=jamais
            TempData["Page Acces Token"] = accesTokenPage;
            Session["AccessToken"] = accesTokenPage;
            fb.AccessToken = accesTokenPage;





            dynamic mee = fb.Get("me?id");
            dynamic me = fb.Get("me?id");

            string email = me.email;

            TempData["email"] = me;
            */



            var data = result3.accounts["data"].ToString();
            var mm = JsonConvert.DeserializeObject<List<page>>(data);
            var f1 = new fbuser();

            f1.id = iduser;
            f1.pagelist = mm;
            iduserG = iduser;

            /****/
            ReviewsContext db = new ReviewsContext();

            int abc = 0;
            if (db.Users.Find(f1.id) == null)
            {
                db.Users.Add(f1);
                abc = 1;
            }


             foreach (var item in f1.pagelist)
             {
                 if (db.Pages.Find(item.id) == null)
                 {
        
                      db.Pages.Add(item);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand(
        "UPDATE dbo.Pages SET fbuser_id = " + iduser + "where id ="+item.id);
                   


                }

             }

          




            db.SaveChanges();
            
        
        /*****/


        f1 = db.Users.Find(f1.id);
            fuserG = f1;

            if (abc==1)
            {
                return View(f1);
            }


            return RedirectToAction( "FacebookCallback3", "Home");
           // return Redirect("~/Home/Index");
        }

        public ActionResult FacebookCallback2(fbuser user)
        {
            ReviewsContext db = new ReviewsContext();
            user.id = iduserG;
            db.Users.Attach(user);

            foreach (var item in user.pagelist)
            {

                db.Pages.Attach(item);

                db.Entry(item).State = EntityState.Modified;


            }
            //user.id = 10213618568591702+"";


            db.Entry(user).State = EntityState.Modified;

            fuserG = user;
            db.SaveChanges();
            return RedirectToAction("FacebookCallback3", "Home");
        }

        public ActionResult FacebookCallback3(fbuser user)
        {
            var pg = new page();
            pg = fuserG.pagelist[0];
            return View(fuserG.pagelist);
        }
        public ActionResult Details(string id)
        {
            var fb = new FacebookClient();
            ReviewsContext db = new ReviewsContext();
            var pg = new page();
            pg = db.Pages.Find(id);

            Session["AccessToken"] = pg.access_token;
            fb.AccessToken = pg.access_token;
            
            dynamic me1 = fb.Get("https://graph.facebook.com/v3.0/me/ratings");
            var data = me1["data"].ToString();
          
            var mm = JsonConvert.DeserializeObject<List<Rating>>(data);
          //  var mm2 = JsonConvert.DeserializeObject<Rating>(data2);
            pg.ratinglist = mm;

            

            foreach (var item in pg.ratinglist)
            {
               string idReviewer = "-1";
                string nameReviewer = "Not Assigned";
                if (item.reviewer== null)
                {
                    idReviewer = "-1";
                    Reviewer ii = new Reviewer();
                    ii.id = "-1";
                    ii.name = "Not Assigned";
                    item.reviewer = ii;
                    
                }
                else
                {
                    idReviewer = item.reviewer.id;
                    nameReviewer = item.reviewer.name;
                }

                var sqlR = db.Ratings.SqlQuery(
        "select * from dbo.Ratings where convert(datetime,created_time) like convert(datetime,'"+item.created_time+"') and page_id like "+pg.id);
                if (sqlR.Count()==0)
             {
                    var sqlR2 = db.Database.SqlQuery<Rating.Reviewer>(
 "select * from dbo.Reviewer where id like " + item.reviewer.id);
                    if (sqlR2.Count() == 0 )
                    {

                         db.Database.ExecuteSqlCommand("Insert into dbo.Reviewer values" +
                        "(@id,@name)",
                        new SqlParameter("id",item.reviewer.id),
                        new SqlParameter("name", item.reviewer.name)
                        );
                    }
                  
                    db.Database.ExecuteSqlCommand("Insert into dbo.Ratings values" +
                        "(@time,@rating,@review_text,@reviewer_id,@page_id)",
                        //new SqlParameter("id",item.Id),
                        new SqlParameter("time",item.created_time),
                        new SqlParameter("rating", item.rating),
                        new SqlParameter("review_text", item.review_text),
                        new SqlParameter("reviewer_id", item.reviewer.id),
                        new SqlParameter("page_id", pg.id)
                        );

              



                    /* db.Database.ExecuteSqlCommand(
           "UPDATE dbo.Pages SET fbuser_id = " +  + "where id =" + item.id);*/



                }

            }
            return View(mm);
        }
    }
}