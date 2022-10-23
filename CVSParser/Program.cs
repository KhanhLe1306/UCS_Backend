// C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

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


    static public void showAvailabilities(Dictionary<string, Dictionary<string, List<Tuple<int, int>>>> weekDayTimeBlockings) {
        foreach(string room in weekDayTimeBlockings.Keys) {
            Console.WriteLine(room);
            foreach(string day in weekDayTimeBlockings[room].Keys) {
                Console.WriteLine("\tThe available time for {0} on {1} is:", room, day);
                if (!(weekDayTimeBlockings[room][day].Count == 0)) {
                    Console.WriteLine("\t\tTAKEN: ");
                    foreach(var block in weekDayTimeBlockings[room][day].OrderBy(t => t.Item1).Distinct().ToList()) {
                        Console.WriteLine("\t\t\t{0} - {1}", block.Item1, block.Item2);
                    }
                }
                List<Tuple<int,int>> result = getAvailability(weekDayTimeBlockings, room, day);
                Console.WriteLine("\t\tAVAILABLE:");
                foreach(var block in result) {
                    Console.WriteLine("\t\t\t{0} - {1}", block.Item1, block.Item2);
                }
                Console.WriteLine();
            }
        }
    }


    static public List<Tuple<int, int>> getAvailability(Dictionary<string, Dictionary<string, List<Tuple<int, int>>>> timeBlockings, string room, string day) {
        int earliest = 900;
        int latest = 2200;
        if (!timeBlockings.ContainsKey(room)) {
            Console.WriteLine("The room " + room + " does not exist, or perhaps it is not registered . . .");
            return null;
        }
        if (!timeBlockings[room].ContainsKey(day)) {
            Console.WriteLine(day + " is not a vaild day . . .");
            return null;
        }
        List<Tuple<int, int>> result = new List<Tuple<int, int>>();
        List<Tuple<int, int>> roomDayBlock = timeBlockings[room][day];
        if (roomDayBlock.Count == 0) {
            result.Add(new Tuple<int, int>(earliest, latest));
            return result;
        }
        else
        {
            roomDayBlock = roomDayBlock.OrderBy(t => t.Item1).Distinct().ToList(); //sorting taken slots by start times

            int dayBlockLength = roomDayBlock.Count;
            if (dayBlockLength == 1) {
                int upTo = roomDayBlock[0].Item1 - earliest + 15;
                upTo = validateMilitaryTime(upTo);
                if (upTo > 0) {
                    result.Add(new Tuple<int, int>(earliest, earliest + upTo));
                }
                result.Add(new Tuple<int, int> (validateMilitaryTime(roomDayBlock[0].Item2 + 15), latest));
            }
            else
            {
                for(int i = 0; i < roomDayBlock.Count - 1; i++) {
                    if (i == 0) {
                        result.Add(new Tuple<int, int>(validateMilitaryTime(earliest), validateMilitaryTime(earliest + (roomDayBlock[i].Item1 - earliest) - 15)));
                    } else {
                        result.Add(new Tuple<int, int>(validateMilitaryTime(roomDayBlock[i-1].Item2 + 15), validateMilitaryTime(roomDayBlock[i].Item1 - 15)));
                    }
                }
                result.Add(new Tuple<int, int>(validateMilitaryTime(roomDayBlock[dayBlockLength - 2].Item2 + 15), validateMilitaryTime(roomDayBlock[dayBlockLength - 1].Item1 - 15)));
            }
        }
        return result;
    }


    static public Dictionary<string, Dictionary<string, List<Tuple<int, int>>>> processWeekdayTimeBlockings(Dictionary<string, List<Tuple<string, Tuple<int, int>, string, string, string, string, string>>> coursesRev)
    {
        Dictionary<string, Dictionary<string, List<Tuple<int, int>>>> result = new Dictionary<string, Dictionary<string, List<Tuple<int, int>>>>();
        foreach (var item in coursesRev)
        {
            //Console.WriteLine(item.Key);
            foreach (var lecture in item.Value)
            {
                string room = lecture.Item1;
                Tuple<int, int> time = lecture.Item2;
                string cid = lecture.Item3;
                string weekday = lecture.Item4;
                if (room != "NULL" & weekday != "NULL")
                {
                    //Console.WriteLine("Trying to add the room to the reuslt");
                    if (!result.ContainsKey(room))
                    {
                        result.Add(room, new Dictionary<string, List<Tuple<int, int>>>(){
                    {"Monday", new List<Tuple<int, int>>()},
                    {"Tuesday", new List<Tuple<int, int>>()},
                    {"Wednesday", new List<Tuple<int, int>>()},
                    {"Thursday", new List<Tuple<int, int>>()},
                    {"Friday", new List<Tuple<int, int>>()},
                    {"Saturday", new List<Tuple<int, int>>()},
                    {"Sunday", new List<Tuple<int, int>>()}
                });
                    }
                    //Console.WriteLine("Room was successfulyl added . . .");
                    // Need to add a room, value pair to result . . .
                    if (weekday.IndexOf(',') != -1)
                    {
                        //Console.WriteLine("Many Week Days . . .");
                        string[] weekdays = weekday.Split(',');
                        foreach (string day in weekdays)
                        {
                            //Console.WriteLine(day);
                            result[room][day].Add(time);
                        }
                    }
                    else
                    {
                        //Console.WriteLine("There is one day . . .");
                        //Console.WriteLine(weekday);
                        result[room][weekday].Add(time);
                    }
                }
            }
        }
        /*foreach (var item in result)
        {
            Console.WriteLine(item.Key);
            foreach (var day in item.Value)
            {
                Console.WriteLine("\t" + day.Key);
                foreach (var item2 in day.Value)
                {
                    Console.WriteLine("\t\t" + item2.Item1 + " " + item2.Item2);
                }
            }
        }*/
        return result;
    }


    static public string weekdayConverter(string shorthand)
    {
        if (shorthand.Length == 1)
        {
            return dayMappings[shorthand];
        }
        if (shorthand.Length == 2)
        {
            if (Char.IsUpper(shorthand[1]))
            {
                return dayMappings[shorthand[0].ToString()] + "," + dayMappings[shorthand[1].ToString()];
            }
            else
            {
                return dayMappings[shorthand];
            }
        }
        if (shorthand.Length == 3)
        {
            return dayMappings[shorthand[0].ToString()] + "," + dayMappings[shorthand.Substring(1)];
        }
        if (shorthand.Length == 4)
        {
            if (Char.IsUpper(shorthand[2]))
            {
                return dayMappings[shorthand[0].ToString()] + "," + dayMappings[shorthand.Substring(1, 3)] + "," + dayMappings[shorthand[3].ToString()];
            }
        }
        return "NULL";
    }


    static public int convertToMilitary(string stdTime)
    {
        int stdTimeLength = stdTime.Length;
        string amOrPm = stdTime.Substring(stdTimeLength - 2);
        if (amOrPm == "am")
        {
            if (stdTimeLength == 3)
            {
                return Int32.Parse("0" + stdTime[0].ToString() + "00");
            }
            else if (stdTimeLength == 4)
            {
                //Console.WriteLine(Int32.Parse(stdTime.Substring(0, 2)));
                return Int32.Parse(stdTime.Substring(0, 2) + "00");
            }
            else if (stdTimeLength == 6)
            {
                return Int32.Parse("0" + stdTime[0].ToString() + stdTime.Substring(2, 2));
            }
            else if (stdTimeLength == 7)
            {
                return Int32.Parse(stdTime.Substring(0, 2) + stdTime.Substring(3, 2));
            }
        }
        else if (amOrPm == "pm")
        {
            if (stdTimeLength == 3)
            {
                return Int32.Parse((12 + Int32.Parse(stdTime[0].ToString())).ToString() + "00");
            }
            else if (stdTimeLength == 4)
            {
                return Int32.Parse((Int32.Parse(stdTime.Substring(0, 2))).ToString() + "00");
            }
            else if (stdTimeLength == 6)
            {
                return Int32.Parse((12 + Int32.Parse(stdTime[0].ToString())).ToString() + stdTime.Substring(2, 2));
            }
            else if (stdTimeLength == 7)
            {
                return Int32.Parse((12 + Int32.Parse(stdTime.Substring(0, 2))).ToString() + stdTime.Substring(3, 2));
            }
        }
        else
        {
            Console.WriteLine("convertToMilitary(): ERROR: Hmm I am unsure what to do with" + stdTime + ". . .");
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
                    //Console.WriteLine(row);
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


    static public int validateMilitaryTime(int militaryTime)
    {
        if (militaryTime == -1)
        {
            return -1;
        }
        int hour = militaryTime / 100;
        int minute = militaryTime % 100;
        if (!(0 <= minute & minute <= 59))
        {
            hour += 1;
            minute -= 60;
        }

        if (hour > 24)
        {
            hour = 0;
        }
        if (hour == 24 & minute > 0)
        {
            hour = 0;
        }
        //Console.WriteLine("BEFORE: " + militaryTime + " AFTER: " + hour.ToString() + minute.ToString().PadRight(2, '0'));
        return Int32.Parse(hour.ToString() + minute.ToString().PadRight(2, '0'));
    }


    static public Dictionary<string, List<Tuple<string, Tuple<int, int>, string, string, string, string, string>>> process(Dictionary<string, List<string[]>> inData)
    {
        Dictionary<string, List<Tuple<string, Tuple<int, int>, string, string, string, string, string>>> result = new Dictionary<string, List<Tuple<string, Tuple<int, int>, string, string, string, string, string>>>();
        foreach (var item in inData)
        {
            string course = item.Key;
            result.Add(course, new List<Tuple<string, Tuple<int, int>, string, string, string, string, string>>());
            foreach (string[] lecture in item.Value)
            {
                if (lecture.Length != 0)
                {
                    string CID = course.Substring(0, 9) + "-" + lecture[9].PadLeft(3, '0');
                    string SID = lecture[3];
                    string ROOM = lecture[16];
                    Match s = Regex.Match(ROOM, @"Peter Kiewit Institute");
                    if (s.Success) {
                        ROOM = "PKI " + ROOM.Substring(ROOM.Length - 3, 3);
                    }
                    string CROSS = "N";
                    string CROSSID = "NULL";
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
                    Tuple<int, int> TIME = new Tuple<int, int>( -1, -1 );
                    string WEEKDAY = "NULL";
                    s = Regex.Match(TIMEWEEK, @"Does Not Meet");
                    if (!s.Success)
                    {
                        //Console.WriteLine("TIMEWEEK: " + TIMEWEEK);
                        string[] TIMEWEEKSplit = TIMEWEEK.Split(' ');
                        string[] hourMinute = TIMEWEEKSplit[1].Split('-');
                        TIME = new Tuple<int, int>(validateMilitaryTime(convertToMilitary(hourMinute[0])),validateMilitaryTime(convertToMilitary(hourMinute[1])));
                        //Console.WriteLine("TIMEWEEKSplit Length: " + TIMEWEEKSplit.Length);
                        WEEKDAY = weekdayConverter(TIMEWEEKSplit[0]);
                        // Will need to implement convert to military . . . 
                    }
                    if (lecture[20] == "Totally Online")
                    {
                        ROOM = "NULL";
                    }
                    result[course].Add(new Tuple<string, Tuple<int,int>, string, string, string, string, string>( ROOM, TIME, CID, WEEKDAY, CROSS, CROSSID, SID ));
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
        Dictionary<string, List<Tuple<string, Tuple<int, int>, string, string, string, string, string>>> coursesRevised = process(baseData);
        Dictionary<string, Dictionary<string, List<Tuple<int, int>>>> weekDayTimeBlockings = processWeekdayTimeBlockings(coursesRevised);
        
        showAvailabilities(weekDayTimeBlockings);
    }
}
