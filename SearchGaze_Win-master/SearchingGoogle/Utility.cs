using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchingGoogle
{
    public static class Utility
    {

        public static void writeFile(UserData dataObject)
        {
            string path = @"..\..\Data\UserStudy\P_" + dataObject.UserID.ToString();
            SearchingGoogle.SearchWin.createDirectory(path);

            path += @"\userInformation.txt";

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.Write(dataObject.UserID + ",");
                    sw.Write(dataObject.MFirstName + ",");
                    sw.Write(dataObject.MLastName + ",");
                    sw.Write(dataObject.MGenderCode + ",");
                    sw.Write(dataObject.MGenderCode + ",");
                    sw.WriteLine("");

                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            //using (StreamWriter sw = File.AppendText(path))
            //{

            //    sw.Write(dataObject.UserID + ",");
            //    sw.Write(dataObject.MFirstName + ",");
            //    sw.Write(dataObject.MLastName + ",");
            //    sw.Write(dataObject.MGenderCode + ",");
            //    sw.WriteLine("");
            //}
        }
    }
}

