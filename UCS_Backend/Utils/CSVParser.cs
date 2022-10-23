using System.Text.RegularExpressions;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Utils
{
    public class CSVParser
    {
        private IClassRepository _classRepository;
        private IRoomRepository _roomRepository;
        private ITimeRepository _timeRepository;
        private IWeekdayRepository _weekdayRepository;
        private ICrossRepository _crossRepository;
        private List<Tuple<int, string>> crossLists;
        public CSVParser(IClassRepository classRepository, IRoomRepository roomRepository, ITimeRepository timeRepository, IWeekdayRepository weekdayRepository, ICrossRepository crossRepository) { 
            this._classRepository = classRepository;
            this._roomRepository = roomRepository;
            this._timeRepository = timeRepository;
            this._weekdayRepository = weekdayRepository;
            this._crossRepository = crossRepository;
        }

        static public Dictionary<string, string> dayMappings = new Dictionary<string, string> {
            {"M", "Monday"},
            {"T", "Tuesday"},
            {"W", "Wednesday"},
            {"Th", "Thursday"},
            {"F", "Friday"},
            {"Sa", "Satruday"},
            {"Su", "Sunday"}
        };

        public Dictionary<string, List<string[]>> processBaseFiles(List<string> inFiles)
        {
            Dictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();
            StreamReader reader = null;
            foreach (var file in inFiles)
            {
                int rowCount = 0;
                string currentCourse = "";
                string currentClssID = "";
                reader = new StreamReader(File.OpenRead("CSVFiles/" +file));
                while (!reader.EndOfStream)
                {
                    /*for (int i = 0; i < 33; i++)
                    {
                        string row1 = reader.ReadLine();
                    }
                    while (true)
                    {*/
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
                            /*var a = row.Split(',');
                            foreach (var c in a)
                            {
                                crossLists.Add(Tuple.Create(currentClssID, c));
                            }                       
                            int x = 0;*/
                            // Find a way to apply this cross list to the previously processed line
                        }
                        else
                        {
                            string[] rowSplit = row.Split(',');                       
                            if (currentCourse != "" & rowCount > 2 & rowSplit.Length >= 36)
                            {
                                currentClssID = rowSplit[1];
                                result[currentCourse].Add(rowSplit);
                            }
                        }
                        rowCount++;
                    /*} */                  
                }
            }

            return result;
        }

        public void process(Dictionary<string, List<string[]>> inData)
        {
            crossLists = new List<Tuple<int, string>>();
            int count = 0; 
            //Dictionary<string, List<Tuple<string, Tuple<int, int>, string, string, string, string, string>>> result = new Dictionary<string, List<Tuple<string, Tuple<int, int>, string, string, string, string, string>>>();
            foreach (var item in inData)
            {
                foreach (string[] lecture in item.Value)
                {
                    if (lecture.Length != 0)
                    {                   
                        int clssID = Int32.Parse(lecture[1]);
                        int enrollments = lecture[29] == "" ? 0 : Int32.Parse(lecture[29]);
                        string course = lecture[8];
                        string courseTitle = lecture[10];
                        ClassModel classModel = new ClassModel { 
                            ClssId = clssID,
                            Enrollments = enrollments,
                            CourseTitle = courseTitle,  
                            Course = course,
                            Section = lecture[9],
                            CatalogNumber = lecture[7],
                        };
                        int classId = _classRepository.AddNewClass(classModel);


                        string ROOM = lecture[16];
                        Match s = Regex.Match(ROOM, @"Peter Kiewit Institute");
                        if (s.Success)
                        {
                            ROOM = "PKI " + ROOM.Substring(23, 3);
                            // Insert room
                            Room room = this._roomRepository.Add(new Room
                            {
                                Name = ROOM,
                                Capacity = Int32.Parse(lecture[30])
                            });
                        }

                        string TIMEWEEK = lecture[13];
                        Tuple<int, int> TIME = new Tuple<int, int>(-1, -1);
                        string WEEKDAY = "NULL";
                        s = Regex.Match(TIMEWEEK, @"Does Not Meet");
                        
                        if (!s.Success)
                        {
                            //Console.WriteLine("TIMEWEEK: " + TIMEWEEK);
                            string[] TIMEWEEKSplit = TIMEWEEK.Split(' ');
                            string[] hourMinute = TIMEWEEKSplit[1].Split('-');
                            TIME = new Tuple<int, int>(validateMilitaryTime(convertToMilitary(hourMinute[0])), validateMilitaryTime(convertToMilitary(hourMinute[1])));
                            this._timeRepository.Add(new Time { 
                                StartTime = TIME.Item1,
                                EndTime = TIME.Item2
                            });
                            //Console.WriteLine("TIMEWEEKSplit Length: " + TIMEWEEKSplit.Length);
                            WEEKDAY = weekdayConverter(TIMEWEEKSplit[0]);
                            this._weekdayRepository.Add(new Weekday
                            {
                                Description = WEEKDAY
                            });

                            // Will need to implement convert to military . . . 
                        }
                        /*if (lecture[20] == "Totally Online")
                        {
                            ROOM = "NULL";
                        }*/

                        // result[course].Add(new Tuple<string, Tuple<int, int>, string, string, string, string, string>(ROOM, TIME, CID, WEEKDAY, CROSS, CROSSID, SID));
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

                        // Crosslisting
                        if (lecture.Length == 37)
                        {
                            if(!string.IsNullOrEmpty(lecture[34]) && lecture[34].Length > 4)
                            {
                                var courseName = lecture[10];
                                var i = 34;
                                while (!string.IsNullOrEmpty(lecture[i]) && i < lecture.Length - 1)
                                {
                                    string crossListClass = lecture[i].ToString();
                                    string section = crossListClass.Substring(crossListClass.Length - 3, 3);
                                    string catalogNumber = crossListClass.Substring(crossListClass.Length - 8, 4);
                                    int temp = _classRepository.FindClssID(catalogNumber, section);
                                    _crossRepository.Add(new Cross { 
                                        ClssID1 = clssID,
                                        ClssID2 = temp,
                                    });
                                    crossLists.Add(Tuple.Create(clssID, temp.ToString()));
                                    i++;
                                    count++;
                                }
                            }
                        }else if (lecture.Length >= 38)
                        {
                            if (!string.IsNullOrEmpty(lecture[35]) && lecture[35].Length > 4)
                            {
                                var courseName = lecture[10];
                                var i = 35;
                                while (!string.IsNullOrEmpty(lecture[i]) && i < lecture.Length - 1)
                                {
                                    string crossListClass = lecture[i].ToString();
                                    string section = crossListClass.Substring(crossListClass.Length - 3, 3);
                                    string catalogNumber = crossListClass.Substring(crossListClass.Length - 8, 4);
                                    int temp = _classRepository.FindClssID(catalogNumber, section);
                                    var res = _crossRepository.Add(new Cross
                                    {
                                        ClssID1 = clssID,
                                        ClssID2 = temp,
                                    });
                                    crossLists.Add(Tuple.Create(clssID, temp.ToString()));
                                    i++;
                                    count++;
                                }
                            }
                                
                        // Find clssID based on CatalogNumber and Section
                        }
                    }
                }
            }
            Console.WriteLine(crossLists);
            //return result;
        }

        public int validateMilitaryTime(int militaryTime)
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

        public int convertToMilitary(string stdTime)
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

        public string weekdayConverter(string shorthand)
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
    }
}
