using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchingGoogle
{
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }
    public class WriteCVS : StreamWriter
    {
        private static readonly object thisLock = new object();

        public WriteCVS(Stream stream)
            : base(stream)
        {
        }

        public WriteCVS(string filename)
            : base(filename)
        {            
        }
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;

            try
            {
                foreach (string value in row)
                {
                    // Add separator if this isn't the first value
                    if (!firstColumn)
                        builder.Append(',');

                    // Implement special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes

                    /*if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                         builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                     else*/

                    builder.Append(value);
                    firstColumn = false;
                }
            }
            catch { Console.WriteLine(row); }


            builder.Append('\n');
            row.LineText = builder.ToString();
            //WriteLine(row.LineText);

            lock (thisLock)
            {
                if(row.LineText.Count() > 0)
                    Write(row.LineText);
            }
        }

        public static void WriteRow1(CsvRow row, StreamWriter writer)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');

                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes

                /*if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                     builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                 else*/

                builder.Append(value);
                firstColumn = false;
            }
            builder.Append('\n');
            row.LineText = builder.ToString();
            //WriteLine(row.LineText);
            writer.Write(row.LineText);
        }

        public void CloseCVSFile()
        {
            //this.Flush();
            this.Close();
        }
    }
}
