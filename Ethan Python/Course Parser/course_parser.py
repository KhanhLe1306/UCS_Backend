import pandas as pd
import csv
import re

courses = {}
cross_list_mappings = {}

with open('CSCI.csv') as file:
    rc = 0
    csv_reader = csv.reader(file)
    current_course = ''
    for row in csv_reader:
        if rc > 1 and (len(row) == 1 or rc == 2) and row[0] != '':
            courses[row[0]] = []
            current_course = row[0]
        elif current_course != '' and rc > 1:
            courses[current_course].append(row)
        rc += 1

courses_revised = {course: [] for course in courses}

for course, lectures in courses.items():
    for lecture in lectures:

        switch = False

        CID = course[0:9] + '-' + str(lecture[9]).zfill(3) # Padding the section with 0's. i.e '1' -> '001' and '851' -> '851' so section # are length 3

        SID = lecture[2]

        if len(lecture[36]) > 10:
            ROOM = lecture[36].split('\n')[0]
        elif len(lecture[36]) != 0:
            ROOM = lecture[36]
        else:
            ROOM = 'NULL'

        CROSS = 'N'
        CROSSID = 'NULL'
        if lecture[34] != '':
            CROSS = 'Y'
            if s := re.search(r'(.... \d{4}-\d{3})', lecture[34]):
                CROSSID = s.group().strip()
            elif s := re.search(r'(.... \d{4})', lecture[34]):
                CROSSID = s.group().strip()
            else:
                CROSSID = lecture[34]
                print('ERROR: Hmm, had trouble getting the cross listing . . .')
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
                WEEKDAY = TIMEWEEK[0]
                if CROSSID != 'NULL':
                    cross_list_mappings[CROSSID][3] = TIME
                    cross_list_mappings[CROSSID][4] = WEEKDAY

        if lecture[19] == 'Totally Online':
            ROOM = 'NULL'
        
        courses_revised[course].append([ROOM, TIME, CID, WEEKDAY, CROSS, CROSSID, SID])

        # Propigate backwards in-case the NULL room came first.

        for c1, s1 in courses_revised.items():
            for lecture in s1:
                for c2, s2 in cross_list_mappings.items():
                    if c2 in lecture:
                        if lecture[0] == 'NULL' and lecture[1] != 'NULL':
                            if ROOM != 'NULL':
                                courses_revised[c1]
                                # TODO: Need to change the room variable in the
                                # revised courses list to be the actual room and not
                                # NULL in the case the the NULL rooms was hit first
                                # for a crosslisted course . . .
                                # Good luck trying to understand this tomorrow - past Ethan


"""for course, lectures in courses_revised.items():
    print(course)
    for lecture in lectures:
        print(f'\t{lecture[0]:8s} {lecture[1]:15s}', 
              f'{lecture[2]:15s} {lecture[3]:5s}',
              f'{lecture[4]:2s} {lecture[5]:15s}',
              f'{lecture[6]:15s}')
    print()

for course, stuff in cross_list_mappings.items():
    if course == 'CSCI 4560-001':
        print(f'{course}: {stuff}')"""