using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RawDataDownloader
{
    class MixpanalDataImporter
    {

        string Api_secret = ConfigurationManager.AppSettings["MixpanelAPISecret"];
        string Api_key = ConfigurationManager.AppSettings["MixpanelAPIKey"];
        DateTime From_date = DateTime.Parse(ConfigurationManager.AppSettings["StartDate"]);

        DateTime Temp_date, End_date;

        public void DoMixpanalDataImporter()
        {
            Console.WriteLine("Calling web call");
            string end_date_string = ConfigurationManager.AppSettings["EndDate"];
            if (!string.IsNullOrEmpty(end_date_string))
                End_date = DateTime.Parse(ConfigurationManager.AppSettings["EndDate"]);
            else
                End_date = DateTime.Today;
                
            double expire = DateTimeToUnixTimestamp(DateTime.Today.AddDays(1));
            
            while (From_date < End_date)
            {
                Temp_date = From_date.AddDays(1);
                Console.WriteLine(From_date.ToString());
                WebCall(Api_secret, Api_key, From_date.ToString("yyyy-MM-dd"), Temp_date.ToString("yyyy-MM-dd"), expire);
                From_date = Temp_date;

            }

           
        }

        private void WebCall(String Api_secret, String Api_key, String From_date, String To_date, double expire)
        {
            String signature = GetSignature(string.Format("api_key={0}expire={1}from_date={2}to_date={3}",
                WebUtility.HtmlEncode(Api_key),
                WebUtility.UrlEncode(expire.ToString()),
                WebUtility.UrlEncode(From_date),
                WebUtility.UrlEncode(To_date))
                , Api_secret);


            String Request = String.Format("https://data.mixpanel.com/api/2.0/export/?api_key={0}&expire={1}&from_date={2}&sig={3}&to_date={4}",
                WebUtility.UrlEncode(Api_key),
                WebUtility.UrlEncode(expire.ToString()),
                WebUtility.UrlEncode(From_date),
                WebUtility.UrlEncode(signature),
                WebUtility.UrlEncode(To_date)
                );


            Debug.WriteLine(Request);
            try
            {


                WebClient client = new WebClient();
                client.Headers.Add("content-type", "text/plain; charset=utf-8");

                using (FileStream File = new FileStream(String.Format("Dump/{0}", From_date), FileMode.Create, FileAccess.Write))
                {

                    using (Stream responsestream = client.OpenRead(Request))
                    {
                        StreamReader streamreader = new StreamReader(responsestream);
                        using (StreamWriter write = new StreamWriter(File))
                        {
                            write.Write(streamreader.ReadToEnd());

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private string GetSignature(string sigdata, string Api_secret)
        {
            string formattedsignature = String.Concat(sigdata, Api_secret);

            MD5 crypt = MD5.Create();
            byte[] data = crypt.ComputeHash(Encoding.UTF8.GetBytes(formattedsignature));


            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }


            return sBuilder.ToString();
        }


        public double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }
    }
}

