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
                return MongoConnection.GetDatabase("Mixpanel");
            }
        }

        private IMongoCollection<BsonDocument> collection
        {
            get
            {
                return GetDb.GetCollection<BsonDocument>("Events");
            }
        }


       

        public void LoadFromFiles()
        {
            string Command = " --db Mixpanel --collection Events --file \"{0}\"";
            string path = @"C:\Program Files\MongoDB\Server\3.0\bin\mongoimport.exe";
            DirectoryInfo Archive = new DirectoryInfo("Archive");
            DirectoryInfo dir = new DirectoryInfo("Dump");
            int counter = 0;
            foreach (var item in Archive.GetFiles())
            {
                counter = 0;
                //using (var streamReader = new StreamReader(item.FullName))
                //{

                //    string line;
                    

                //    while ((line = streamReader.ReadLine()) != null)
                //    {
                //        line = line.Replace("$", "");
                //        BsonDocument bs = BsonDocument.Parse(line);
                //        collection.BulkWriteAsync()
                //        counter++;
                        

                //    }

                //}
                ExecuteCommandSync(String.Format(Command, item.FullName),path);
                //item.MoveTo(Archive.ToString()+ "\\" + item.Name);

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
