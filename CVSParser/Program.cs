// C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class CVSParser
{
    static public Dictionary<string, List<string[]>> processBaseFiles(List<string> inFiles)
    {
        Dictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();
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
                bool cross = row.Contains("ross");
                if (rowCount > 2 & !row.Contains(",") & !cross)
                {
                    result.Add(row, new List<string[]>());
                    currentCourse = row;
                }
                else if (cross & rowCount > 2 & row.Length < 50)
                {
                    Console.WriteLine(row);
                    int x = 0;
                    // Find a way to apply this cross list to the previously processed line
                }
                else
                {
                    string[] rowSplit = row.Split(',');
                    if (currentCourse != "" & rowCount > 2 & rowSplit.Length == 38)
                    {
                        // Console.WriteLine("Row Split Arrray is " + rowSplit.Length + " elements long . . ."); 

                        result[currentCourse].Add(rowSplit);

                    }
                }
                rowCount++;
            }
        }

        return result;
    }


    static public Dictionary<string, List<string[]>> process(Dictionary<string, List<string[]>> inData)
    {
        Dictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();
        foreach (var item in inData)
        {
            string course = item.Key;
            result.Add(course, new List<string[]>());
            foreach (string[] lecture in item.Value)
            {
                if (lecture.Length != 0)
                {
                    string CID = course.Substring(0, 9) + "-" + lecture[9].PadLeft(3, '0');
                    string SID = lecture[3];
                    string ROOM = lecture[16];
                    string CROSS = "N";
                    string CROSSID = "NULL";
                    Match s;
                    if (lecture[35] != "")
                    {
                        Regex.Replace(lecture[35], "\n", " ");
                        CROSS = "Y";
                        s = Regex.Match(lecture[35], @"(.... \d{4}-\d{3})");
                        if (s.Success)
                        {
                            CROSSID = s.Value;
                        }
                        else
                        {
                            s = Regex.Match(lecture[35], @"(.... \d{4})");
                            if (s.Success)
                            {
                                CROSSID = s.Value;
                            }
                            else
                            {
                                Console.WriteLine("Couldn't get the Cross-List");
                            }
                        }
                    }
                    string TIMEWEEK = lecture[13];
                    string TIME = "NULL";
                    string WEEKDAY = "NULL";
                    s = Regex.Match(TIMEWEEK, @"Does Not Meet");
                    if (!s.Success)
                    {
                        //Console.WriteLine("TIMEWEEK: " + TIMEWEEK);
                        string[] TIMEWEEKSplit = TIMEWEEK.Split(' ');
                        //Console.WriteLine("TIMEWEEKSplit Length: " + TIMEWEEKSplit.Length);
                        WEEKDAY = TIMEWEEKSplit[0];
                        TIME = TIMEWEEKSplit[1];
                        // Will need to implement convert to military . . . 
                    }
                    if (lecture[20] == "Totally Online")
                    {
                        ROOM = "NULL";
                    }
                    result[course].Add(new string[] { ROOM, TIME, CID, WEEKDAY, CROSS, CROSSID, SID });
                    foreach (string[] s1 in result[course])
                    {
                        for (int k = 0; k < s1.Length; k++)
                        {
                            if (k == 0)
                            {
                                Console.Write(s1[k].PadRight(30, ' '));
                            }
                            else
                            {
                                Console.Write(s1[k].PadRight(16, ' '));
                            }
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
        return result;
    }


    static void Main(string[] args)
    {
        // List<string> csvfiles = new List<string> { "CSCI1191.csv", "CYBR1191.csv" };
        List<string> csvfiles = new List<string> { "CSCI1191.csv", "BIOI1191.csv", "BMI1191.csv", "CYBR1191.csv", "ISQA1191.csv", "ITIN1191.csv" };
        Dictionary<string, List<string[]>> baseData = processBaseFiles(csvfiles);
        /*foreach( var item in baseData) {
            Console.WriteLine(item.Key);
            foreach (string[] s in item.Value) {
                for(int i = 0; i < s.Length; i++) {
                    if (s[i] != string.Empty)
                    {
                        Console.Write(s[i] + "  ");
                    }
                }
                Console.WriteLine();
            }
        }*/
        Dictionary<string, List<string[]>> coursesRevised = process(baseData);
    }
}
