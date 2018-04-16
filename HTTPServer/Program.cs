using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            Server newServer = new Server(1000, @"D:\College work\Network\project\Template[2016-2017]\redirectionRules.txt");
            // 2) Start Server
            newServer.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            string path = @"D:\College work\Network\project\Template[2016-2017]\redirectionRules.txt";
            // Delete the file if it exists.
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            // Create the file.
            using (FileStream fs = File.Create(path))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("aboutus.html,aboutus2.html");
                // Add redirection rules to the file
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
