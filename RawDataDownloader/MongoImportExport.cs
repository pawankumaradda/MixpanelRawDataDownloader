using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using System.IO;
using System.Diagnostics;
using System.Configuration;

namespace RawDataDownloader
{
    class MongoImportExport
    {
        private MongoClient MongoConnection
        {
            get
            {
                return new MongoClient();
            }
        }

        private IMongoDatabase GetDb
        {
            get
            {
                return MongoConnection.GetDatabase(ConfigurationManager.AppSettings["MongoDbName"]);
            }
        }

        private IMongoCollection<BsonDocument> collection
        {
            get
            {
                return GetDb.GetCollection<BsonDocument>(ConfigurationManager.AppSettings["MongoTableName"]);
            }
        }


       

        public void LoadFromFiles()
        {
            string Command = " --db {0} --collection {1} --file \"{2}\"";
            string path = ConfigurationManager.AppSettings["MongoImportexeLocation"]; 
            DirectoryInfo Archive = new DirectoryInfo("Archive");
            DirectoryInfo dir = new DirectoryInfo("Dump");
            DirectoryInfo test = new DirectoryInfo("Test");
            int counter = 0;
            foreach (var item in Archive.GetFiles())
            {
                counter = 0;
                ExecuteCommandSync(String.Format(Command, ConfigurationManager.AppSettings["MongoDbName"],ConfigurationManager.AppSettings["MongoTableName"],item.FullName),path);
                item.MoveTo(Archive.ToString()+ "\\" + item.Name);

                Console.WriteLine("File {0} : {1}",item.Name, counter);
            }
        
        }



        public void ExecuteCommandSync(string command, string path)
        {
            Console.WriteLine("/c " + command);
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(path)
                {
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardInput = true,
                    Arguments = command 
                };
                Process proc = new Process() { StartInfo = psi };

                proc.Start();
                proc.WaitForExit();
                proc.Close();

                //System.Diagnostics.Process.Start("CMD.exe", command);
            }
            catch (Exception objException)
            {
                Console.WriteLine(objException.ToString());
            }
        }

    }
}
