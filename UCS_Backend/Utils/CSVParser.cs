using System.Text.RegularExpressions;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Utils
{


    /// <summary>
    /// Creates a class for CSVParser
    /// </summary> 
    public class CSVParser
    {
        private IClassRepository _classRepository;
        private IRoomRepository _roomRepository;
        private ITimeRepository _timeRepository;
        private IWeekdayRepository _weekdayRepository;
        private ICrossRepository _crossRepository;
        private IScheduleRepository _scheduleRepository;
        private IInstructorRepository _instructorRepository;
        private IInstructorClassRepository _instructorClassRepository;
        private List<Tuple<int, string>> crossLists;
        public CSVParser(IClassRepository classRepository, IRoomRepository roomRepository, ITimeRepository timeRepository, IWeekdayRepository weekdayRepository, ICrossRepository crossRepository, IScheduleRepository scheduleRepository, IInstructorRepository instructorRepository, IInstructorClassRepository instructorClassRepository) { 
            this._classRepository = classRepository;
            this._roomRepository = roomRepository;
            this._timeRepository = timeRepository;
            this._weekdayRepository = weekdayRepository;
            this._crossRepository = crossRepository;
            this._scheduleRepository = scheduleRepository;
            this._instructorRepository = instructorRepository;
            this._instructorClassRepository = instructorClassRepository;
        }

        public Dictionary<string, string> dayMappings = new Dictionary<string, string> {
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
            string currentCourse = "";
            string[] rawCSV = System.IO.File.ReadAllLines("CSVFiles/" + inFiles[0]);
            for (int row = 0; row < rawCSV.Length; row++)
            {
                string[] tmp = rawCSV[row].Split(new string[] { ",\""}, StringSplitOptions.None);
                for (int i = 0; i < tmp.Length; i++) {
                    tmp[i] = tmp[i].Replace("\"", String.Empty);
                }

                if (row > 2) {
                    if (tmp.Length == 1 & !tmp[0].Contains("ross")) { // Course
                        currentCourse = tmp[0];
                        result.Add(currentCourse, new List<string[]>());
                    } else if (tmp.Length < 3) { // Crosslist Bug . . .
                        //Console.WriteLine(tmp[0]);
                    } else if (currentCourse != "") { // Section (Main) information
                        result[currentCourse].Add(tmp);
                    }
                }
            }

            return result;
        }

        public void process(Dictionary<string, List<string[]>> inData)
        {
            Match re;
            List<Tuple<int,string>> crossLists = new List<Tuple<int, string>>();
            foreach (var item in inData) {
                foreach (string[] lecture in item.Value) {

                    // = = = = = = Class = = = = = =
                    int clssId = Int32.Parse(lecture[1]);
                    int enrollments = lecture[28] == "" ? 0 : Int32.Parse(lecture[28]);
                    string instructor = lecture[14];
                    string course = lecture[8];
                    string courseTitle = lecture[10];
                    string section = lecture[9];
                    string catNum = lecture[7];
                    string subjectCode = lecture[6];
                    int classId = _classRepository.AddNewClass(new ClassModel
                    {
                        ClssId = clssId,
                        Enrollments = enrollments,
                        CourseTitle = courseTitle,
                        Course = course,
                        Section = section,
                        CatalogNumber = catNum,
                        SubjectCode = subjectCode,
                    });

                    // = = = = = = Instructor = = = = = =

                    if (instructor == "Staff") // STAFF
                    {

                        var instructorReturn = _instructorRepository.Add(new Instructor
                        {
                            FirstName = instructor,
                            LastName = "",
                            EmployeeNumber = ""
                        });
                        _instructorClassRepository.Add(new InstructorClass
                        {
                            ClassId = classId,
                            InstructorId = instructorReturn.InstructorId
                        });
                    }
                    else if (instructor.Contains(';')) // TWO OR MORE INSTRUCTORS
                    {
                        string[] instructors = instructor.Split("; ");
                        for (int i = 0; i < instructors.Length; i++)
                        {
                            string[] isplit = instructors[i].Split(", ");
                            var instructorReturn = _instructorRepository.Add(new Instructor
                            {
                                FirstName = isplit[1].Split(" ")[0],
                                LastName = isplit[0],
                                EmployeeNumber = isplit[1].Split(" ")[1].Replace("(", String.Empty).Replace(")", String.Empty)
                            });
                            if (i == 0)
                            {
                                _instructorClassRepository.Add(new InstructorClass
                                {
                                    ClassId = classId,
                                    InstructorId = instructorReturn.InstructorId
                                });
                            } else
                            {
                                _instructorClassRepository.Add(new InstructorClass
                                {
                                    ClassId = classId,
                                    InstructorId = instructorReturn.InstructorId
                                });
                            }
                        }
                    }
                    else // ONE INSTRUCTOR
                    {
                        string[] isplit = instructor.Split(", ");
                        // Add the instructor
                        var instructorReturn = _instructorRepository.Add(new Instructor
                        {
                            FirstName = isplit[1].Split(" ")[0],
                            LastName = isplit[0],
                            EmployeeNumber = isplit[1].Split(" ")[1].Replace("(", String.Empty).Replace(")", String.Empty)
                        });

                        // Add the InstructorClass
                        _instructorClassRepository.Add(new InstructorClass
                        {
                            ClassId = classId,
                            InstructorId = instructorReturn.InstructorId
                        });
                    }

                    // = = = = = = Room = = = = = =
                    string roomName;
                    Room room;
                    re = Regex.Match(lecture[15], @"Peter Kiewit Institute");
                    if (re.Success)
                    {
                        roomName = "PKI " + lecture[15].Substring(23, 3);
                        room = this._roomRepository.Add(new Room
                        {
                            Name = roomName,
                            Capacity = Int32.Parse(lecture[29]),
                        });
                    } else
                    {
                        room = new Room
                        {
                            RoomId = 0,
                        };
                    }

                    // = = = = = = Time = = = = = =
                    string weekdayVal;
                    string timeWeek = lecture[13];
                    Tuple<int, int> timeVal = new Tuple<int, int>(-1, -1);
                    re = Regex.Match(timeWeek, @"Does Not Meet");
                    Time time = null;
                    Weekday weekday = null;
                    if (!re.Success)
                    {
                        string[] timeWeekSplit = timeWeek.Split(' ');
                        string[] hourMinute = timeWeekSplit[1].Split('-');
                        timeVal = new Tuple<int, int> (validateMilitaryTime(convertToMilitary(hourMinute[0])), validateMilitaryTime(convertToMilitary(hourMinute[1])));
                        Console.WriteLine("TIME AFTER CONVERSION AND VALIDATION: " + timeVal.Item1 + " - " + timeVal.Item2);
                        time = this._timeRepository.Add(new Time
                        {
                            StartTime = timeVal.Item1,
                            EndTime = timeVal.Item2,
                        });
                        weekdayVal = weekdayConverter(timeWeekSplit[0]);
                        weekday = this._weekdayRepository.Add(new Weekday
                        {
                            Description = weekdayVal,
                        });
                    }

                    // = = = = = = Cross Listing = = = = = =
                    Cross cross;
                    if (!string.IsNullOrEmpty(lecture[34]))
                    {
                        string crossListClass = lecture[34];
                        string crossListSection = crossListClass.Substring(crossListClass.Length - 3, 3);
                        string crossListCat = crossListClass.Substring(crossListClass.Length - 8, 4);
                        int temp = _classRepository.FindClssID(crossListCat, section);
                        _crossRepository.Add(new Cross
                        {
                            ClssID1 = clssId,
                            ClssID2 = temp,
                        });
                    }


                    // = = = = = = Schedule = = = = = =
                    _scheduleRepository.Add(new Schedule
                    {
                        ClassId = classId,
                        RoomId = room.RoomId,
                        TimeId = time != null ? time.TimeId : 0,
                        WeekdayId = weekday != null ? weekday.WeekdayId : 0,
                    });
                }
            }
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
            return Int32.Parse(hour.ToString() + minute.ToString().PadRight(2, '0'));
        }

        public int convertToMilitary(string stdTime)
        {
            int result = -1;
            int stdTimeLength = stdTime.Length;
            string amOrPm = stdTime.Substring(stdTimeLength - 2);
            if (amOrPm == "am")
            {
                if (stdTimeLength == 3)
                {
                    result = Int32.Parse("0" + stdTime[0].ToString() + "00");
                }
                else if (stdTimeLength == 4)
                {
                    result = Int32.Parse(stdTime.Substring(0, 2) + "00");
                }
                else if (stdTimeLength == 6)
                {
                    result = Int32.Parse("0" + stdTime[0].ToString() + stdTime.Substring(2, 2));
                }
                else if (stdTimeLength == 7)
                {
                    result = Int32.Parse(stdTime.Substring(0, 2) + stdTime.Substring(3, 2));
                }
                //Console.WriteLine("CONVERTTOMILITARY: " + stdTime + " -> " + result);
                return result;
            }
            else if (amOrPm == "pm")
            {
                if (stdTimeLength == 3)
                {
                    result =  Int32.Parse((12 + Int32.Parse(stdTime[0].ToString())).ToString() + "00");
                }
                else if (stdTimeLength == 4)
                {
                    int hour = Int32.Parse(stdTime.Substring(0, 2));
                    if (hour != 12) {
                        result = Int32.Parse((hour + 12).ToString() + "00");
                    } else
                    {
                        result = Int32.Parse(hour.ToString() + "00");
                    }
                }
                else if (stdTimeLength == 6)
                {
                    result = Int32.Parse((12 + Int32.Parse(stdTime[0].ToString())).ToString() + stdTime.Substring(2, 2));
                }
                else if (stdTimeLength == 7)
                {
                    int hour = Int32.Parse(stdTime.Substring(0, 2));
                    if (hour != 12)
                    {
                        result = Int32.Parse((hour + 12).ToString() + stdTime.Substring(3, 2));
                    } else
                    {
                        result = Int32.Parse(hour.ToString() + stdTime.Substring(3, 2));
                    }
                }
                //Console.WriteLine("CONVERTTOMILITARY: " + stdTime + " -> " + result);
                return result;
            }
            else
            {
                Console.WriteLine("convertToMilitary(): ERROR: Hmm I am unsure what to do with" + stdTime + ". . .");
            }


            return result;
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
