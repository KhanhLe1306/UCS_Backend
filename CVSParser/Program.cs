// C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class CVSParser
{

    static public Dictionary<string, string> dayMappings = new Dictionary<string, string> {
        {"M", "Monday"},
        {"T", "Tuesday"},
        {"W", "Wednesday"},
        {"Th", "Thursday"},
        {"F", "Friday"},
        {"Sa", "Satruday"},
        {"Su", "Sunday"}
    };


    static public string weekdayConverter(string shorthand) {
        if (shorthand.Length == 1){
            return dayMappings[shorthand];
        }
        if (shorthand.Length == 2) {
            if (Char.IsUpper(shorthand[1])) {
                return dayMappings[shorthand[0].ToString()] + ", " + dayMappings[shorthand[1].ToString()];
            } else {
                return dayMappings[shorthand];
            }
        }
        if (shorthand.Length == 3) {
            return dayMappings[shorthand[0].ToString()] + ", " + dayMappings[shorthand.Substring(1)];
        }
        if (shorthand.Length == 4) {
            if (Char.IsUpper(shorthand[2])) {
                return dayMappings[shorthand[0].ToString()] + ", " + dayMappings[shorthand.Substring(1,3)] + ", " + dayMappings[shorthand[3].ToString()];
            }
        }
        return "NULL";
    }


    static public int convertToMilitary(string stdTime) {
        int stdTimeLength = stdTime.Length;
        string amOrPm = stdTime.Substring(stdTimeLength - 2);
        if (amOrPm == "am") {
            if (stdTimeLength == 3) {
                return Int32.Parse("0" + stdTime[0].ToString() + "00");
            } else if (stdTimeLength == 4) {
                Console.WriteLine(Int32.Parse(stdTime.Substring(0,2)));
                return Int32.Parse(stdTime.Substring(0, 2) + "00");
            } else if (stdTimeLength == 6) {
                return Int32.Parse("0" + stdTime[0].ToString() + stdTime.Substring(2, 2));
            } else if (stdTimeLength == 7) {
                return Int32.Parse(stdTime.Substring(0, 2) + stdTime.Substring(3, 2));
            }
        } else if (amOrPm == "pm") {
            if (stdTimeLength == 3) {
                return Int32.Parse((12 + Int32.Parse(stdTime[0].ToString())).ToString() + "00");
            } else if (stdTimeLength == 4) {
                return Int32.Parse((12 + Int32.Parse(stdTime.Substring(0, 2))).ToString() + "00");
            } else if (stdTimeLength == 6) {
                return Int32.Parse((12 + Int32.Parse(stdTime[0].ToString())).ToString() + stdTime.Substring(2, 2));
            } else if (stdTimeLength == 7) {
                return Int32.Parse((12 + Int32.Parse(stdTime.Substring(0, 2))).ToString() + stdTime.Substring(3, 2));
            }
        } else {
            Console.WriteLine("convertToMilitary(): ERROR: Hmm I am unsure what to do with" + stdTime +". . .");
        }

        
        return -1;
    }

    static public Dictionary<string, List<string[]>> processBaseFiles(List<string> inFiles)
    {
        Dictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();
        StreamReader reader = null;
        foreach (var file in inFiles)
        {
            int rowCount = 0;
            string currentCourse = "";
            reader = new StreamReader(File.OpenRead("bin/" + file));
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


    static public int validateMilitaryTime(int militaryTime) {
        if (militaryTime == -1) {
            return -1;
        }
        int hour = militaryTime / 100;
        int minute = militaryTime % 100;
        if (!(minute <= 0) & !(minute >= 50)) {
            hour += 1;
            minute -= 60;
        }

        if (hour > 24) {
            hour = 0;
        }
        if (hour == 24 & minute > 0) {
            hour = 0;
        }
        return Int32.Parse(hour.ToString() + minute.ToString().PadRight(2));
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
                        string[] hourMinute = TIMEWEEKSplit[1].Split('-');
                        TIME = validateMilitaryTime(convertToMilitary(hourMinute[0])).ToString() + "-" + validateMilitaryTime(convertToMilitary(hourMinute[1])).ToString();
                        if (TIME == "-1--1") {
                            TIME = "NULL";
                        }
                        Console.WriteLine("TIME: " + TIME);
                        //Console.WriteLine("TIMEWEEKSplit Length: " + TIMEWEEKSplit.Length);
                        WEEKDAY = weekdayConverter(TIMEWEEKSplit[0]);
                        // Will need to implement convert to military . . . 
                    }
                    if (lecture[20] == "Totally Online")
                    {
                        ROOM = "NULL";
                    }
                    result[course].Add(new string[] { ROOM, TIME, CID, WEEKDAY, CROSS, CROSSID, SID });
                    /*foreach (string[] s1 in result[course])
                    {
                        for (int k = 0; k < s1.Length; k++)
                        {
                            if (k == 0)
                            {
                                Console.Write(s1[k].PadRight(30, ' '));
                            }
                            else
                            {
                                Console.Write(s1[k].PadRight(20, ' '));
                            }
                        }
                        Console.WriteLine();
                    }*/
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
