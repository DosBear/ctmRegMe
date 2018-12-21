using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Environment;

namespace ctmRegMe
{
    public static class Const
    {
        public struct APP
        {
            public static string version = "0.01";
            public static char delimeters = '\t';
            public static string dir = System.IO.Directory.GetCurrentDirectory();
        }

        public struct SYS
        {
            public static string path = Environment.GetFolderPath(SpecialFolder.CommonApplicationData);
            public static string file = "reginfo.xml";
            public static string fullpath = path + @"\CTM\Common\" + file;
        }

 
        public struct INFILE
        {
            public static string reglist = "reglist.txt";
            public static string regserv = "regserv.txt";
        }

    }
}
