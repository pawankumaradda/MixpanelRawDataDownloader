using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RawDataDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            //new MixpanalDataImporter().DoMixpanalDataImporter();
            new MongoImportExport().LoadFromFiles();

            Console.WriteLine("Done");
            Console.ReadKey();

        }
    }
}

