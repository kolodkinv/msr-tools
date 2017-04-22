using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSR.Data.VersionControl.Hg
{
    public class HgBodyFile : List<string>, IBodyFile
    {
        public static HgBodyFile Parse(Stream fileData)
        {
            TextReader reader = new StreamReader(fileData);

            HgBodyFile bodyFile = new HgBodyFile();
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                bodyFile.Add(line);
            }

            return bodyFile;
        }
    }
}
