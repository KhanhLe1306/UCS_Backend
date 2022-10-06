import pandas as pd
import csv
import re

courses = {}
cross_list_mappings = {}
day_time_blockings = {}
day_mappings = {'M': 'Monday', 'T': 'Tuesday', 'W': 'Wednesday', 'Th': 'Thursday', 'F': 'Friday', 'S': 'Saturday'}


# file_names = ['CSCI1191.csv', 'BIOI1191.csv', 'BMI1191.csv', 'CIST_EMIT1191.csv', 'CYBR1191.csv', 'ISQA1191.csv', 'ITIN1191.csv']
# file_names = ['CSCI1191.csv', 'BIOI1191.csv', 'BMI1191.csv', 'CYBR1191.csv', 'ISQA1191.csv', 'ITIN1191.csv']
file_names = ['CSCI1191.csv', 'CYBR1191.csv']


def convert_to_military(std_time):
    std_time_len = len(std_time)
    am_pm = std_time[std_time_len - 2: std_time_len]
    if am_pm == 'am':
        if std_time_len == 3:
            return '0' + std_time[0] + ":00"
        elif std_time_len == 4:
            return std_time[0:2] + ":00"
        elif std_time_len == 6:
            return '0' + std_time[0:std_time_len - 2]
        elif std_time_len == 7:
            return std_time[0:std_time_len - 2]
        else:
            print(f'convert_to_military(): Hmm, I am unsure what to do with {std_time}.')
            return std_time
    if am_pm == 'pm':
        if std_time_len == 3:
            return str(12 + int(std_time[0])) + ":00"
        elif std_time_len == 4:
            return str(12 + int(std_time[0:2])) + ":00"
        elif std_time_len == 6:
            return str(12 + int(std_time[0])) + ":" + std_time[2:4]
        elif std_time_len == 7:
            return std_time[0: std_time_len - 2]
        else:
            print(f'convert_to_military(): Hmm, I am unsure what to do with {std_time}.')
            return std_time
    else:
        print(f'convert_to_military(): Hmm, I am unsure what to do with {std_time}.')
        return std_time


def weekday_converter(shorthand):
    if len(shorthand) == 1:
        return day_mappings[shorthand]
    if len(shorthand) == 2:
        if shorthand[1].isupper():
            return day_mappings[shorthand[0]] + '-' + day_mappings[shorthand[1]]
        else:
            return day_mappings[shorthand]
    if len(shorthand) == 3:
        return day_mappings[shorthand[0]] + '-' + day_mappings[shorthand[1:3]]


for file_name in file_names:
    with open(file_name) as file:
        rc = 0
        csv_reader = csv.reader(file)
        current_course = ''
        for row in csv_reader:
            if rc > 1 and (len(row) == 1 or rc == 2) and row[0] != '':
                courses[row[0]] = []
                current_course = row[0]
            elif current_course != '' and rc > 1:
                if row[16] == 'Active':
                    courses[current_course].append(row)
            rc += 1
pop_list = []
for course in courses:
    if courses[course] == []:
        pop_list.append(course)

for course in pop_list:
    courses.pop(course)

courses_revised = {course: [] for course in courses}

for course, lectures in courses.items():
    for lecture in lectures:
        switch = False

        CID = course[0:9] + '-' + str(lecture[9]).zfill(3) # Padding the section with 0's. i.e '1' -> '001' and '851' -> '851' so section # are length 3

        SID = lecture[2]

        ROOM = 'NULL'
        if s := re.search(r'(Peter Kiewit Institute \d{3})', lecture[15]):
            ROOM = 'PKI '+ s.group().strip().split(' ')[-1]

        CROSS = 'N'
        CROSSID = 'NULL'
        if lecture[34] != '':
            CROSS = 'Y'
            # Parse out the cross listing part
            if s := re.search(r'(.... \d{4}-\d{3})', lecture[34]):
                CROSSID = s.group().strip()
            elif s := re.search(r'(.... \d{4})', lecture[34]):
                CROSSID = s.group().strip()
            else:
                CROSSID = lecture[34]
                print('###### ERROR: Hmm, had trouble getting the cross listing . . . ######')
                print('\t', lecture[34])

            
            if not CROSSID in list(cross_list_mappings.keys()):
                cross_list_mappings[CROSSID] = ['NULL', 'NULL', 'NULL', 'NULL', 'NULL']
                cross_list_mappings[CROSSID][0] = CROSSID
                cross_list_mappings[CROSSID][1] = CID
                cross_list_mappings[CROSSID][2] = ROOM
            else:
                if lecture[19] == 'Totally Online':
                    ROOM = cross_list_mappings[CROSSID][2]
                    TIME = cross_list_mappings[CROSSID][3]
                    WEEKDAY = cross_list_mappings[CROSSID][4]
                    switch = True

        if not switch:
            TIMEWEEK = lecture[13]
            TIME = 'NULL'
            WEEKDAY = 'NULL'
            if not re.match('Does Not Meet', TIMEWEEK):
                TIMEWEEK = TIMEWEEK.split(' ')
                TIME = TIMEWEEK[1]
                WEEKDAY = weekday_converter(TIMEWEEK[0])
                if CROSSID != 'NULL':
                    cross_list_mappings[CROSSID][3] = TIME
                    cross_list_mappings[CROSSID][4] = WEEKDAY

        if lecture[19] == 'Totally Online':
            ROOM = 'NULL'
        
        courses_revised[course].append([ROOM, TIME, CID, WEEKDAY, CROSS, CROSSID, SID])

        # Propigate backwards in-case the NULL room came first.
        trip = False
        for k1 in courses_revised:
            if trip:
                break
            for i in range(len(courses_revised[k1])):
                for k2 in cross_list_mappings:
                    if k2 in courses_revised[k1][i]:
                        if courses_revised[k1][i][0] == 'NULL' and courses_revised[k1][i][1] != 'NULL':
                            if ROOM != 'NULL':
                                print(f"Swapping out {courses_revised[k1][i][0]} with {ROOM}")
                                courses_revised[k1][i][0] = ROOM
                                trip = True

for course, lectures in courses_revised.items():
    for lecture in lectures:
        if lecture[1] != 'NULL':
            tmp = lecture[1].split('-')
            tmp_len1 = len(tmp[0])
            tmp_len2 = len(tmp[1])
            # 3, 4, 6, 7
            tmp1 = convert_to_military(tmp[0])
            tmp2 = convert_to_military(tmp[1])
            lecture[1] = tmp1 + '-' + tmp2

for course, lectures in courses_revised.items():
    print(course)
    for lecture in lectures:
        print(f'\t{lecture[0]:8s} {lecture[1]:15s}', 
              f'{lecture[2]:15s} {lecture[3]:20s}',
              f'{lecture[4]:2s} {lecture[5]:15s}',
              f'{lecture[6]:15s}')
    print()
