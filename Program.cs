using System.Xml;

namespace HelloWorld
{
    class Program
    {
        static List<string> packageList = new List<string>();

        static List<string> documentTypeList = new List<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("Enter Enviroment Name (BE or FE)");
            string? envName = Console.ReadLine();
            envName = string.IsNullOrEmpty(envName) ? "ICCPhoenix" : envName;
            envName =
                envName == "BE" ? "ci_esb_is_backend" : "ci_esb_is_frontend";
            Console.WriteLine("Enter Package Name");
            string? packageName = Console.ReadLine();
            packageName =
                string.IsNullOrEmpty(packageName) ? "ICCPhoenix" : packageName;
            var path =
                @"C:\workspaces\integration\" +
                envName +
                @"\assets\IS\Packages\" +
                packageName;
            if (File.Exists(path))
            {
                // This path is a file
                ProcessFile (path);
                Log();
            }
            else if (Directory.Exists(path))
            {
                // This path is a directory
                ProcessDirectory (path);
                Log();
            }
            else
            {
                Console
                    .WriteLine("{0} is not a valid file or directory.", path);
            }
        }

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string packageName = string.Empty;

            foreach (string fileName in fileEntries)
            {
                ProcessFile (fileName);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries =
                Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                var dirName = subdirectory.Split('\\');
                if (!packageName.Equals(dirName))
                {
                    packageName = dirName[7];
                }
                ProcessDirectory (subdirectory);
            }
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            if (Path.GetFileName(path) == "flow.xml")
            {
                GetFlowDependency (path);
            }
            else if (Path.GetFileName(path) == "node.ndf")
            {
                GetDocumentTypeDependency (path);
            }
        }

        public static void GetFlowDependency(string path)
        {
            try
            {
                string[] attributes = { "INVOKE", "MAPINVOKE" };
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load (path);
                foreach (var attribute in attributes)
                {
                    XmlNodeList n = xmlDoc.GetElementsByTagName(attribute);
                    if (n != null)
                    {
                        foreach (XmlNode curr in n)
                        {
                            var packageName =
                                curr?.Attributes?["SERVICE"]?.Value;
                            if (
                                !packageList.Any(p => p == packageName) &&
                                packageName != null
                            )
                            {
                                packageList.Add (packageName);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + path);
            }
        }

        public static void GetDocumentTypeDependency(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load (path);
            XmlNodeList n = xmlDoc.GetElementsByTagName("rec_ref");
            if (xmlDoc.DocumentElement != null)
            {
                XmlNode? node =
                    xmlDoc
                        .DocumentElement
                        .SelectSingleNode("//value[@name='rec_ref']");
                if (
                    node != null &&
                    !documentTypeList.Any(p => p == node.InnerText)
                )
                {
                    documentTypeList.Add(node.InnerText);
                }
            }
        }

        private static void Log()
        {
            Console.WriteLine("-------------Packages List-------------");
            foreach (var item in packageList.OrderBy(x => x).ToArray())
            {
                Console.WriteLine (item);
            }
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("----------DocumentTypes List----------");
            foreach (var item in documentTypeList.OrderBy(x => x).ToArray())
            {
                Console.WriteLine (item);
            }
            Console.WriteLine("--------------------------------------");
        }
    }
}
