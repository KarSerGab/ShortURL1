using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ShortURL1.Models
{
    public class Short
    {
        public int ID { get; set; }
        public string URL { get; set; }
        public string ShortenedURL { get; set; }
        public string Clicked { get; set; }
        public string DataCreated { get; set; }
    }
    public class Shortener
    {
        public List<Short> GetDataValue()
        {
            List<Short> returnmodel = new List<Short>();
            Shortener sh = new Shortener();
            string path = sh.BaseConnection();
            SqlConnection con = new SqlConnection(path);
            con.Open();
            string command = "Select * From UrlTable";
            SqlCommand com = new SqlCommand(command, con);
            SqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                Short ss = new Short
                {
                    ID=Convert.ToInt32( reader.GetValue(0)),
                    URL = reader.GetValue(1).ToString(),
                    ShortenedURL = reader.GetValue(2).ToString(),
                    Clicked = reader.GetValue(3).ToString(),
                    DataCreated = reader.GetValue(4).ToString()

                };
                returnmodel.Add(ss);
            }
            reader.Close();
            con.Close();
            return returnmodel;
        }
        public string Token { get; set; }
        private Shortener GenerateToken()
        {            
            string urlsafe = string.Empty;
            Enumerable.Range(48, 75)
                .Where(i => i < 58 || i > 64 && i < 94 || i > 96)
                .OrderBy(o => new Random().Next())
                .ToList()
                .ForEach(i => urlsafe += Convert.ToChar(i));
            Token = urlsafe.Substring(new Random().Next(0, urlsafe.Length-6), new Random().Next(3, 6));
            return this;
        }

        public string BaseConnection()
        {
            string str = @"Data Source=KAREN-PC\SQLEXPRESS;Initial Catalog=URLs;Integrated Security=True";
            return str;
        }

        private bool TokenExists(string token)
        {
            bool exists = false;
            string path = BaseConnection();
            SqlConnection con = new SqlConnection(path);
            con.Open();
            string command = "Select * From UrlTable Where ShortenedURL='" + token + "'";
            SqlCommand com = new SqlCommand(command, con);
            SqlDataReader reader = com.ExecuteReader();
            if (reader.Read())
            {
                exists = true;
            }
            reader.Close();
            con.Close();
            return exists;

        }

        private bool URLExists(string url)
        {
            bool exists = false;
            string path = BaseConnection();
            SqlConnection con = new SqlConnection(path);
            con.Open();
            string command = "Select * From UrlTable Where URL='" + url+ "'";
            SqlCommand com = new SqlCommand(command, con);
            SqlDataReader reader = com.ExecuteReader();
            if (reader.Read())
            {
                exists = true;
            }
            reader.Close();
            con.Close();
            return exists;
        }

        public void TokenSave(string url,string token)
        {
            string path = BaseConnection();
            SqlConnection con = new SqlConnection(path);
            string cmdString = string.Format("Insert INTO URLTable" + "(URL, ShortenedURL, Clicked,DataCreated) Values(@URL, @ShortenedURL, @Clicked,@DataCreated)");
            con.Open();
            using (SqlCommand cmd = new SqlCommand(cmdString, con))
            {
                cmd.Parameters.AddWithValue("@URL", url);
                cmd.Parameters.AddWithValue("@ShortenedURL", token);
                cmd.Parameters.AddWithValue("@Clicked", 0);
                cmd.Parameters.AddWithValue("@DataCreated", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }



        public string ShortURL (string url)
        {
            string token="";
            bool exists = true;
            //проверка наличия url в базе
            exists = URLExists(url);
            if (exists)
            {
                return "stopURL";
            }
            //проверка наличия токена в базе
            exists = true;
            while (exists)
            {
                token = GenerateToken().Token;
                exists = TokenExists(token);
            }
            //TokenSave(url,token);
            return token;
        }

        public  string TransitionURL(string url)
        {
            string ss = "";
            int click = 0;
            int IDurl = 0;
            string path = BaseConnection();
            SqlConnection con = new SqlConnection(path);
            con.Open();
            string command = "Select * From UrlTable Where ShortenedURL='" + url + "'";
            SqlCommand com = new SqlCommand(command, con);
            SqlDataReader reader = com.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                IDurl = Convert.ToInt32(reader.GetValue(0));
                click = Convert.ToInt32(reader.GetValue(3));
                ss = reader.GetValue(1).ToString();
            }
            reader.Close();

            if (IDurl!= 0)
            {
                click +=1;
                command = "Update UrlTable set Clicked="+ click+" Where ID='" + IDurl + "'";
                SqlCommand comm = new SqlCommand(command, con);
                comm.ExecuteNonQuery();

            }
            con.Close();
            return ss;
        }

        public void DeleteString(int id)
        {
            Shortener sh = new Shortener();
            string path = sh.BaseConnection();
            SqlConnection con = new SqlConnection(path);
            con.Open();
            string command = "Delete * From UrlTable Where ID='" + id + "'";
            SqlCommand cmd = new SqlCommand(command, con);
            cmd.Parameters.AddWithValue("@ID", "ID");
            cmd.ExecuteNonQuery();
        }
    }
}