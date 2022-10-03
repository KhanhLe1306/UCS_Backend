import pandas as pd
import csv
import re

courses = {}

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
for course, lectures in courses.items():
    print(f"{course}:")

    for lecture in lectures:
        SID = lecture[2]
        ROOM = lecture[36]
        if re.search("Cross-list", ROOM):
            ROOM = ' '.join(ROOM.split('\n'))
            print(ROOM)
        TIMEWEEK = lecture[13]
        CID = lecture[1]
        TIME = 'DNM'
        WEEKDAY = 'DNM'
        if not re.match("Does Not Meet", TIMEWEEK):
            TIMEWEEK = TIMEWEEK.split(' ')
            TIME = TIMEWEEK[1]
            WEEKDAY = TIMEWEEK[0]
        #print(f"\t{SID:10s} {ROOM:10s} {TIME:20s} {CID:10s} {WEEKDAY:10s}")
    print()