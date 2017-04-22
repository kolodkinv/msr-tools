using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSR.Data.VersionControl.Svn
{
    public class SvnBodyFile : List<string>, IBodyFile
    {
        public static SvnBodyFile Parse(Stream fileData)
        {
            TextReader reader = new StreamReader(fileData);

            SvnBodyFile bodyFile = new SvnBodyFile();
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                bodyFile.Add(line);
            }

            return bodyFile;
        }
    }
}
