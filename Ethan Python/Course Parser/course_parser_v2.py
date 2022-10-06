import csv
import re
import time

courses = {}
weekday_time_blockings = {}
courses_revised = None
file_name_groups = {
    1: ['CSCI1191.csv', 'BIOI1191.csv', 'BMI1191.csv', 'CIST_EMIT1191.csv', 'CYBR1191.csv', 'ISQA1191.csv', 'ITIN1191.csv'],
    2: ['CSCI1191.csv', 'BIOI1191.csv', 'BMI1191.csv', 'CYBR1191.csv', 'ISQA1191.csv', 'ITIN1191.csv'],
    3: ['CSCI1191.csv', 'CYBR1191.csv']
}


def validate_military_time(military_time):
    hour = military_time // 100
    minute = military_time % 100
    if not 0 <= minute <= 59:
        hour += 1
        minute -= 60
    
    if hour > 24:
        hour = 0
    if hour == 24 and minute > 0:
        hour = 0
    return int(str(hour) + str(minute).zfill(2))


def get_availability(room, day):
    print(f"{room}")
    earliest = 9_00
    latest = 22_00
    if not room in weekday_time_blockings.keys():
        print(f"The room {room} does not exist . . . or perhaps it is not being used.")
        return
    if not day in ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday']:
        print(f"The day {day} does not exist.")
        return

    room_day_block = weekday_time_blockings[room][day]
    if room_day_block == []:
        print(f"The room is available from {earliest}-{latest}")
    else:
        tmp = []
        rdb = sorted(list(set([block[0] for block in room_day_block])), key=lambda item: item[0])
        # print(rdb)
        day_block_length = len(rdb)
        if day_block_length == 1:
            # print(room_day_block[0])
            up_to = rdb[0][0] - earliest + 15
            validate_military_time(up_to)
            if up_to > 0:
                tmp.append((earliest, earliest + up_to))
            if day_block_length == 1:
                tmp.append((validate_military_time(rdb[0][1] + 15), latest))
        else:
            for i in range(len(rdb) - 1):
                # print(rdb[i])
                if i == 0:
                    tmp.append((validate_military_time(earliest), validate_military_time(earliest + (rdb[i][0] - earliest) - 15)))
                else:
                    tmp.append((validate_military_time(rdb[i-1][1] + 15), validate_military_time(rdb[i][0] - 15)))
            tmp.append((validate_military_time(rdb[day_block_length-2][1] + 15), validate_military_time(rdb[day_block_length - 1][0] - 15)))
        print(f"The avaible time for {room} on {day} is:")
        for block in tmp:
            print(f"  {str(block[0]).zfill(4)} - {str(block[1]).zfill(4)}")
    print()


def convert_to_military(std_time):
    std_time_len = len(std_time)
    am_pm = std_time[std_time_len - 2:std_time_len]
    if am_pm == 'am':
        if std_time_len == 3:
            return int('0' + std_time[0] + "00")
        elif std_time_len == 4:
            return int(std_time[0:2] + "00")
        elif std_time_len == 6:
            return int('0' + std_time[0] + std_time[2:std_time_len - 2])
        elif std_time_len == 7:
            return int(std_time[0:2] + std_time[3:std_time_len - 2])
        else:
            print(f'convert_to_military(): Hmm, I am unsure what to do with {std_time}.')
            return std_time
    if am_pm == 'pm':
        if std_time_len == 3:
            return int(str(12 + int(std_time[0])) + "00")
        elif std_time_len == 4:
            return int(str(12 + int(std_time[0:2])) + "00")
        elif std_time_len == 6:
            return int(str(12 + int(std_time[0])) + std_time[2:4])
        elif std_time_len == 7:
            return int(str(12 + int(std_time[0:2])) + std_time[3:std_time_len - 2])
        else:
            print(f'convert_to_military(): Hmm, I am unsure what to do with {std_time}.')
            return std_time
    else:
        print(f'convert_to_military(): Hmm, I am unsure what to do with {std_time}.')
        return std_time


def weekday_converter(shorthand):
    day_mappings = {'M': 'Monday', 'T': 'Tuesday', 'W': 'Wednesday', 'Th': 'Thursday', 'F': 'Friday', 'Sa': 'Saturday', 'Su': 'Sunday'}
    if len(shorthand) == 1:
        return day_mappings[shorthand]
    if len(shorthand) == 2:
        if shorthand[1].isupper():
            return day_mappings[shorthand[0]] + '-' + day_mappings[shorthand[1]]
        else:
            return day_mappings[shorthand]
    if len(shorthand) == 3:
        return day_mappings[shorthand[0]] + '-' + day_mappings[shorthand[1:3]]
    
    if len(shorthand) == 4:
        if shorthand[2].islower():
            return day_mappings[shorthand[0]] + '-' + day_mappings[shorthand[1:3]] + '-' + day_mappings[shorthand[3]]


def process_weekday_time_blocks():
    global courses_revised
    global weekday_time_blockings

    for course, lectures in courses_revised.items():
        for room, time, cid, weekday, cross, crossid, sid in lectures:
            if room not in weekday_time_blockings and room != 'NULL':
                weekday_time_blockings[room] = {'Monday': [], 'Tuesday': [], 'Wednesday': [], 'Thursday': [], 'Friday': [], 'Saturday': [], 'Sunday': []}
                if re.search('-', weekday):
                    weekdays = weekday.split('-')
                    for weekday in weekdays:
                        weekday_time_blockings[room][weekday].append((time, cid))
                else:
                    weekday_time_blockings[room][weekday].append((time, cid))
            elif room != 'NULL':
                if re.search('-', weekday):
                    weekdays = weekday.split('-')
                    for weekday in weekdays:
                        weekday_time_blockings[room][weekday].append((time, cid))
                else:
                    weekday_time_blockings[room][weekday].append((time, cid))
    for room in weekday_time_blockings:
        for day in weekday_time_blockings[room]:
            if weekday_time_blockings[room][day] != []:
                weekday_time_blockings[room][day] = sorted(weekday_time_blockings[room][day], key=lambda item: item[0][0])


def process_files_base(files):
    global courses
    global courses_revised
    for file_name in files:
        with open(file_name) as file:
            row_count = 0
            csv_reader = csv.reader(file)
            current_course = ''
            for row in csv_reader:
                if row_count > 1 and (len(row) == 1 or row_count == 2) and row[0] != '':
                    courses[row[0]] = []
                    current_course = row[0]
                elif current_course != '' and row_count > 1:
                    if row[16] == 'Active':
                        courses[current_course].append(row)
                row_count+=1
    courses = {k: v for k, v in courses.items() if v != []}
    courses_revised = {course: [] for course in courses}


def process():
    global courses
    global courses_revised
    for course, lectures in courses.items():
        for lecture in lectures:
            CID = course[0:9] + '-' + str(lecture[9]).zfill(3)
            SID = lecture[2]
            ROOM = 'NULL'
            CROSS = 'N'
            CROSSID = 'NULL'
            if s := re.search(r'(Peter Kiewit Institute \d{3})', lecture[15]):
                ROOM = 'PKI '+ s.group().strip().split(' ')[-1]
            if lecture[34] != '':
                CROSS = 'Y'
                if s := re.search(r'(.... \d{4}-\d{3})', lecture[34]):
                    CROSSID = s.group().strip()
                elif s := re.search(r'(.... \d{4})', lecture[34]):
                    CROSSID = s.group().strip()
                else:
                    CROSSID = lecture[34]
                    print('###### ERROR: Hmm, had trouble getting the cross listing . . . ######')
                    print('\t', lecture[34])
            TIMEWEEK = lecture[13]
            TIME = 'NULL'
            WEEKDAY = 'NULL'
            if not re.match('Does Not Meet', TIMEWEEK):
                TIMEWEEK = TIMEWEEK.split(' ')
                hour_min = TIMEWEEK[1].split('-')
                TIME = (convert_to_military(hour_min[0]), convert_to_military(hour_min[1]))
                WEEKDAY = weekday_converter(TIMEWEEK[0])
            if lecture[19] == 'Totally Online':
                ROOM = 'NULL'
            courses_revised[course].append((ROOM, TIME, CID, WEEKDAY, CROSS, CROSSID, SID))
            
            
def print_data(schedule, time_blockings, availability):
    global courses_revised
    global weekday_time_blockings
    if schedule:
        for course, lectures in courses_revised.items():
            print(course)
            for lecture in lectures:
                print(f'\t{lecture[0]:8s} {lecture[1]}', 
                    f'{lecture[2]:15s} {lecture[3]:20s}',
                    f'{lecture[4]:2s} {lecture[5]:15s}',
                    f'{lecture[6]:15s}')
            print()
    if time_blockings:
        for room in weekday_time_blockings:
            print(room)
            for day in weekday_time_blockings[room]:
                print(day)
                for tup in weekday_time_blockings[room][day]:
                    print(tup, end=' ')
                print()
            print()
    if availability:
        show_availabilities()
    

def show_availabilities():
    global weekday_time_blockings
    for room in weekday_time_blockings:
        for day in weekday_time_blockings[room]:
            get_availability(room, day)



if __name__ == "__main__":
    process_files_base(file_name_groups[1])
    process()
    process_weekday_time_blocks()
    """in_room = input("Please enter a room: ")
    in_day = input("Please enter a day: ")"""
    print_data(False, False, True)
    


# Room: 'Monday': [((1100, 1245), Lecture), ((1430, 1545), Lecture)]

# FREE: (900 - 1045) (1300 - 1415) (1600 - 2200)
# TAKEN: (1100 - 1245) (1430 - 1545)
# 1385 -> 1425
# 2425 -> 0025
