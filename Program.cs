using System.Xml;

namespace HelloWorld
{
    class Program
    {

        static List<string> packageList = new List<string>();
        static List<string> documentTypeList = new List<string>();
        static void Main(string[] args)
        {

            var path = @"C:\workspaces\integration\ci_esb_backend\assets\IS\Packages\ICCPhoenix";
            if (File.Exists(path))
            {
                // This path is a file
                ProcessFile(path);
            }
            else if (Directory.Exists(path))
            {
                // This path is a directory
                ProcessDirectory(path);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }
            Log();
        }

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        static string packageName = string.Empty;
        public static void ProcessDirectory(string targetDirectory)
        {

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                var dirName = subdirectory.Split('\\');
                if (!packageName.Equals(dirName))
                {
                    //Console.WriteLine(subdirectory.Split('\\')[7]);
                    packageName = dirName[7];
                }
                ProcessDirectory(subdirectory);
                //Console.WriteLine(subdirectory);
            }
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            if (Path.GetFileName(path) == "flow.xml")
            {

                GetFlowDependency(path);
            }
            else if (Path.GetFileName(path) == "node.ndf")
            {

                GetDocumentTypeDependency(path);
            }
        }


        public static void GetFlowDependency(string path)
        {
            string[] attributes = { "INVOKE", "MAPINVOKE" };
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            foreach (var attribute in attributes)
            {
                XmlNodeList n = xmlDoc.GetElementsByTagName(attribute);
                if (n != null)
                {
                    foreach (XmlNode curr in n)
                    {
                        var packageName = curr?.Attributes?["SERVICE"]?.Value;
                        if (!packageList.Any(p => p == packageName) && packageName != null)
                        {

                            packageList.Add(packageName);
                        }
                        //Console.WriteLine("--" + curr.Attributes["SERVICE"].Value);
                    }
                }
            }
        }

        public static void GetDocumentTypeDependency(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList n = xmlDoc.GetElementsByTagName("rec_ref");
            if (xmlDoc.DocumentElement != null)
            {
                XmlNode? node = xmlDoc.DocumentElement.SelectSingleNode("//value[@name='rec_ref']");
                if (node != null && !documentTypeList.Any(p => p == node.InnerText))
                {
                    documentTypeList.Add(node.InnerText);
                }
            }
        }

        private static void Log()
        {
            Console.WriteLine("-------------Packages List-------------");
            foreach (var item in packageList)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("----------DocumentTypes List----------");
            foreach (var item in documentTypeList)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("--------------------------------------");
        }
    }
}