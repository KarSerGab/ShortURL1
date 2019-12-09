using ShortURL1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ShortURL1.Controllers
{
    public class HomeController : Controller
    {
        public string strurl="";
        public ActionResult Index()
        {
            strurl = new Uri(Request.Url.AbsoluteUri).ToString();
            strurl = strurl.Trim();
            Shortener sh = new Shortener();
            string ss = sh.TransitionURL(strurl);
            if (ss != "")
            {
                Response.Redirect(ss);
            }
            Short datashort = new Short();
            List<Short> listshort = sh.GetDataValue();

            return View(listshort);
        }

        public void  Delete()
        {
            string b = HttpContext.Request.Url.Segments[3];
            int ID = Convert.ToInt32(b);
            Shortener sh = new Shortener();
            string path = sh.BaseConnection();
            SqlConnection con = new SqlConnection(path);
            con.Open();
            string command =string.Format("Delete  From UrlTable Where ID='{0}'",ID);
            using (SqlCommand cmd = new SqlCommand(command, con))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Exception error = new Exception("Удаление невозможно", ex);
                    throw error;
                }
            }
            Response.Redirect("https://localhost:44302/Home/Index");
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public class Dat
        {
           public string Longurl { get; set; }
        }
        [HttpPost]
        public JsonResult StartShort(Dat dt)
        {

            strurl = new Uri(Request.Url.AbsoluteUri).ToString();
            string sub = strurl.Substring(0, strurl.Length-10);
            string ss = dt.Longurl;
            Shortener sh = new Shortener();
            string stoped = sh.ShortURL(ss);
            string str = sub + @"Index/" + stoped;
            if (stoped == "stopURL")
            {
                str = "stop";
            }
            else
            {
                sh.TokenSave(ss, str);
            }
            var dat = new
            {
                name = str
            };

            return Json(dat, JsonRequestBehavior.AllowGet);
        }
    }
}