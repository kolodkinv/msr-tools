using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSR.Data.VersionControl.Git
{
    /// <summary>
    /// Keeps the body of a file
    /// </summary>
    public class GitBodyFile : List<string>, IBodyFile
    {
        public static GitBodyFile Parse(Stream fileData)
        {
            TextReader reader = new StreamReader(fileData);

            GitBodyFile bodyFile = new GitBodyFile();
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                bodyFile.Add(line);
            }

            return bodyFile;
        }
    }
}
