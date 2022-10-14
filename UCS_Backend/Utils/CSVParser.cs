using System.Text.RegularExpressions;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Utils
{
    public class CSVParser
    {
        private IClassRepository _classRepository;
        public CSVParser(IClassRepository classRepository) { 
            this._classRepository = classRepository;
        }

        public Dictionary<string, List<string[]>> processBaseFiles(List<string> inFiles)
        {
            Dictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();
            StreamReader reader = null;
            foreach (var file in inFiles)
            {
                int rowCount = 0;
                string currentCourse = "";
                reader = new StreamReader(File.OpenRead("CSVFiles/" +file));
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
                        if (currentCourse != "" & rowCount > 2) // & rowSplit.Length == 38 ?
                        {
                            result[currentCourse].Add(rowSplit);
                        }
                    }
                    rowCount++;
                }
            }

            return result;
        }

        public void process(Dictionary<string, List<string[]>> inData)
        {
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
                        };
                        int id = _classRepository.AddNewClass(classModel);


                        /*string ROOM = lecture[16];
                        Match s = Regex.Match(ROOM, @"Peter Kiewit Institute");
                        if (s.Success)
                        {
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
                        Tuple<int, int> TIME = new Tuple<int, int>(-1, -1);
                        string WEEKDAY = "NULL";
                        s = Regex.Match(TIMEWEEK, @"Does Not Meet");*/
                        /*
                        if (!s.Success)
                        {
                            //Console.WriteLine("TIMEWEEK: " + TIMEWEEK);
                            string[] TIMEWEEKSplit = TIMEWEEK.Split(' ');
                            string[] hourMinute = TIMEWEEKSplit[1].Split('-');
                            TIME = new Tuple<int, int>(validateMilitaryTime(convertToMilitary(hourMinute[0])), validateMilitaryTime(convertToMilitary(hourMinute[1])));
                            //Console.WriteLine("TIMEWEEKSplit Length: " + TIMEWEEKSplit.Length);
                            WEEKDAY = weekdayConverter(TIMEWEEKSplit[0]);
                            // Will need to implement convert to military . . . 
                        }
                        if (lecture[20] == "Totally Online")
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
                    }
                }
            }
            //return result;
        }

    }
}
