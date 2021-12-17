using System.Xml;

namespace HelloWorld
{
    class Program
    {
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

            foreach (var item in packageList)
            {
                Console.WriteLine(item);
            }
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

                GetXmlNode(path,"INVOKE");
            } else if (Path.GetFileName(path) == "node.ndf")
            {

                //GetXmlNode(path);
            }

        }

        static List<string> packageList = new List<string>();
        public static void GetXmlNode(string path,string tagname)
        {
            XmlDocument d = new XmlDocument();
            d.Load(path);
            XmlNodeList n = d.GetElementsByTagName(tagname);
            if (n != null)
            {
                foreach (XmlNode curr in n)
                {
                    var packageName = curr.Attributes["SERVICE"].Value;
                    if (!packageList.Any(p => p == packageName))
                    {

                        packageList.Add(packageName);
                    }
                    //Console.WriteLine("--" + curr.Attributes["SERVICE"].Value);
                }
            }
        }
    }
}