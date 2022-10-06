// C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static public IDictionary<string, List<string[]>> processBaseFiles(List<string> inFiles)
    {
        IDictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();
        StreamReader reader = null;
        foreach (var file in inFiles)
        {
            int rowCount = 0;
            string currentCourse = "";
            reader = new StreamReader(File.OpenRead(file));
            while (!reader.EndOfStream)
            {
                string row = reader.ReadLine();
                row = Regex.Replace(row, "\"", string.Empty);
                if (rowCount > 2 & !row.Contains(",") & !row.Contains("Cross") & !row.Contains("cross"))
                {
                    result.Add(row, new List<string[]>());
                    currentCourse = row;
                    Console.WriteLine(currentCourse + "\n");
                }
            
                rowCount++;
            }
        }
        return result;
    }
    static void Main(string[] args)
    {
        List<string> csvfiles = new List<string> { "CSCI1191.csv", "CYBR1191.csv" };
        IDictionary<string, List<string[]>> baseData = processBaseFiles(csvfiles);
    }
}
