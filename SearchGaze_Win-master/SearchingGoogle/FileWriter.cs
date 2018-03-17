using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchingGoogle
{

    class FileWriter
    {
        public static WriteCVS m_CSVWriter;
        public static bool m_RecordClicks = false;

        //CSV file writing service
        public static WriteCVS CreateCSVFile(String fileName)
        {
            m_CSVWriter = new WriteCVS(fileName);
            return m_CSVWriter;
        }
        // Write data row
        public static void writeData(CsvRow datarow, WriteCVS m_CSVWriter)
        {
            m_CSVWriter.WriteRow(datarow);
        }
        // Close file
        public static void closeCSVFile(WriteCVS m_CSVWriter)
        {
            if(m_CSVWriter != null)
                m_CSVWriter.CloseCVSFile();
        }
    }

}
