import sqlite3 as sqlite
import prettytable as prettytable
import random as rnd

POPULATION_SIZE = 1
ELITE_SCHEDULES = 1
TOURNAMENT_SELECTION_SIZE = 3
MUTATION_RATE = 0.1


class DBMgr:

    def __init__(self):
        self._conn = sqlite.connect('../../UCSDatabase.db')
        self._c = self._conn.cursor()
        self._rooms = self.get_rooms()
        self._instructors = self.get_instructors()
        self._schedule = self.get_schedules()
        self._times = self.get_times()
        self._cross = self.get_cross()
        self._classes = self.get_classes()
        self._weekdays = self.get_weekdays()

    def get_rooms(self):
        self._c.execute("SELECT * FROM Rooms")
        return self._c.fetchall()

    def get_instructors(self):
        self._c.execute("SELECT * FROM Instructors")
        return self._c.fetchall()

    def get_schedules(self):
        self._c.execute("SELECT * FROM Schedules")
        return self._c.fetchall()

    def get_times(self):
        self._c.execute("SELECT * FROM Time")
        return self._c.fetchall()

    def get_cross(self):
        self._c.execute("SELECT * FROM Cross")
        return self._c.fetchall()

    def get_classes(self):
        self._c.execute("SELECT * FROM Classes")
        return self._c.fetchall()

    def get_weekdays(self):
        self._c.execute("SELECT * FROM Weekdays")
        return self._c.fetchall()


class Data:

    def __init__(self):
        manager = DBMgr()
        self.rooms = manager._rooms
        self.instructors = manager._instructors
        self.classes = manager._classes
        self.times = manager._times
        self.crosslistings = manager._cross
        self.schedule = manager._schedule
        self.weekdays = manager._weekdays

    def get_rooms(self):
        res = {}
        rooms = self.rooms
        for i, room in enumerate(rooms):
            res[i+1] = list(room)
        return res

    def get_instructors(self):
        res = {}
        inst = self.instructors
        for i, ins in enumerate(inst):
            res[i+1] = list(ins)
        return res

    def get_classes(self):
        res = {}
        for i, clas in enumerate(self.classes):
            res[i+1] = list(clas)
        return res

    def get_crosslistings(self):
        res = {}
        for i, cross in enumerate(self.crosslistings):
            res[i+1] = list(cross)
        return res

    def get_times(self):
        res = {}
        for i, time in enumerate(self.times):
            res[i+1] = list(time)
        return res

    def get_weekdays(self):
        res = {}
        for i, week in enumerate(self.weekdays):
            res[i+1] = list(week)
        return res

    def get_schedule(self):
        res = {}
        for i, sc in enumerate(self.schedule):
            res[i+1] = list(sc)
        return res

    def is_a_crosslist(self, clssId1, clssId2):
        for cross in self.crosslistings:
            if (cross[1] == clssId1 and cross[2] == clssId2
                    or cross[1] == clssId2 and cross[2] == clssId1):
                return True
        return False


data = Data()


class Schedule:

    def __init__(self):
        self._data = data
        self._classes = []
        self._openings = []
        self._conflicts = 0
        self._fitness = -1
        self._class_number = 0
        self._fitness_changed = True
        self._blocking_view = {}

    def get_classes(self):
        self._fitness_changed = True
        return self._classes

    def get_conflicts(self):
        return self._conflicts

    def get_fitness(self):
        if self._fitness_changed:
            self._fitness = self.calculate_fitness()
            self._fitness_changed = False
        return self._fitness

    def initialize(self):
        unknown_room_counter = 1
        for i, schedule_item in enumerate(self._data.schedule):
            if schedule_item[2] != 0 or (schedule_item[1] != 0 and schedule_item[2] != 0):
                new_class = Class(self._class_number, self._data.get_classes()[
                                  schedule_item[3]][:-1])
                self._class_number += 1
                new_class.set_meetingTime(
                    self._data.get_times()[schedule_item[2]])
                new_class.set_meetingDay(self._data.get_weekdays()[
                                         schedule_item[4]][-1])
                new_class.set_instructor(self._data.get_classes()[
                                         schedule_item[3]][-1])
                if schedule_item[1] != 0:
                    new_class.set_room(self._data.get_rooms()[
                                       schedule_item[1]])
                else:
                    new_class.set_room([None, f"TBA", None])
                    unknown_room_counter += 1
                self._classes.append(new_class)
                #print(new_class.get_course(), new_class.get_instructor(), new_class.get_meetinDay(), new_class.get_meetingTime(), new_class.get_room())
        self.fill_openings()
        return self

    def check_room(self, room, day, time):
        data = self._blocking_view
        check_data = data[room].get(day, False)
        if check_data:
            for c in check_data:
                if c[0]<=time[0]<=c[1] or c[0]<=time[1]<=c[1]:
                    return False
        return True 

    def check_inst(self, ins, day, time):
        data = self._blocking_view
        for room in data:
            check_data = data[room].get(day, False)
            if check_data:
                for c in check_data:
                    if c[2] == ins:
                        if c[0]<=time[0]<=c[1] or c[0]<=time[1]<=c[1]:
                            return False
        return True


    def check(self, ins, room, day, time):
        rm = self.check_room(room, day, time)
        inst = self.check_inst(ins,  day, time)
        if rm and inst:
            print("\tCLASS CAN BE ADDED")
        else:
            print("\tCLASS CANNOT BE ADDED")
        input()




    # These are the taken spots for each day. 
    # Use this to calculate the open blocks
    def get_blockings(self):
        res = {}
        for cls in self._classes:
            inst = cls.get_instructor()
            days = cls.get_meetinDay().split(',')
            if cls.get_room()[1] in res:
                print(days)
                if len(days) == 1:
                    if days[0] in res[cls.get_room()[1]]:
                        res[cls.get_room()[1]][days[0]].append((cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst))
                    else:
                        res[cls.get_room()[1]][days[0]] = [(cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst)]
                else: # two days
                    if days[0] in res[cls.get_room()[1]]:
                        res[cls.get_room()[1]][days[0]].append((cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst))
                    else:
                        res[cls.get_room()[1]][days[0]] = [(cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst)]
                    if days[1] in res[cls.get_room()[1]]:
                        res[cls.get_room()[1]][days[1]].append((cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst))
                    else:
                        res[cls.get_room()[1]][days[1]] = [(cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst)]
            else:
                print(days)
                if len(days) == 1:
                    res[cls.get_room()[1]] = {days[0]: [
                        (cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst)]}
                elif len(days) == 2:
                    res[cls.get_room()[1]] = {
                        days[0]: [(cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst)]}
                    res[cls.get_room()[1]] = {
                        days[1]: [(cls.get_meetingTime()[1], cls.get_meetingTime()[2], inst)]}
        
        for room in res:
            for day in res[room]:
                res[room][day] = sorted(res[room][day], key=lambda x: int(x[0]))

        self._blocking_view = res

    def print_blockings(self):
        res = self._blocking_view
        for room in res:
            print(room)
            for day in res[room]:
                print(f"\t{day}")
                for time in res[room][day]:
                    print(f"\t\t{time}")

    def get_blockings_for_day_room(self, room, day):
        start = 8_00
        end = 22_00

    def fill_openings(self):
        # Loop through the schedule, and for each day, add a class to
        # the schedule representing an opening. Then modify the GA to
        # understand what these open classes mean.
        start = 8_00
        end = 22_00
        self.get_blockings()
        self.print_blockings()
        self.add_class()
        input()

    def get_instructor_classes(self, instructor):
        res = []
        for cls in self._classes:
            if cls.get_instructor() == instructor:
                res.append(cls)
        return res

    def add_class(self):
        print("ADD A CLASS:")
        inst = input("\tinst: ")
        room = input("\troom: ")
        day = input("\tday:  ")
        time = input("\ttime: ").split(' ')
        time = (int(time[0]), int(time[1]))
        check = self.check(inst, room, day, time)
        if check:
            print("\tNO CONFLICTS PRESENT")
        else:
            print("\tCONFLICT PRESENT")

    def calculate_fitness(self):
        cross_listings = self._data.get_crosslistings()
        self._conflicts = 0
        classes = self.get_classes()
        for i in range(0, len(classes)):
            for j in range(0, len(classes)):
                if j > i:
                    if (classes[i].get_meetingTime() == classes[j].get_meetingTime()
                        and classes[i].get_meetinDay() == classes[j].get_meetinDay()
                        and classes[i].get_id() != classes[j].get_id()
                            and not self._data.is_a_crosslist(classes[i].get_course()[1], classes[j].get_course()[1])):
                        if classes[i].get_room() == classes[j].get_room() and classes[i].get_room()[1] != "TBA":
                            self._conflicts += 1
                            print(f"ROOM CONFLICT WITH THESE CLASSES:\n{classes[i]} -> {classes[i].get_room()}\n{classes[j]} -> {classes[j].get_room()}\n")
                            input()
                        if classes[i].get_room() == classes[j].get_room() and classes[i].get_room()[1] == "TBA":
                            print(f"POTENTIAL CONFLICT WITH: \n{classes[i].get_course()[2]} -> {classes[i].get_room()} : {classes[i].get_meetingTime()} : {classes[i].get_meetinDay()}\n{classes[j].get_course()[2]} -> {classes[j].get_room()} : {classes[j].get_meetingTime()} : {classes[j].get_meetinDay()}\n")
                            print(f"{classes[i]}\n{classes[j]}\n{self._data.is_a_crosslist(classes[i].get_course()[1], classes[j].get_course()[1])}")
                            input()
                        if classes[i].get_instructor() == classes[j].get_instructor():
                            print(f"INSTRUCTOR CONFLICT WITH THESE CLASSES:\n{classes[i]} -> {classes[i].get_instructor()}\n{classes[j]} -> {classes[j].get_instructor()}\n")
                            input()
                            self._conflicts += 1

        return 1 / (1.0 * self._conflicts + 1)

    def __str__(self):
        res = ""
        for i in range(0, len(self._classes) - 1):
            res += str(self._classes[i]) + ",\n"
        res += str(self._classes[len(self._classes) - 1])
        return res


class Class:
    def __init__(self, id, course):
        self._id = id
        self._course = course
        self._instructor = None
        self._meetingTime = None
        self._meetingDay = None
        self._room = None

    def get_id(self): return self._id
    def get_course(self): return self._course
    def get_instructor(self): return self._instructor
    def get_meetingTime(self): return self._meetingTime
    def get_meetinDay(self): return self._meetingDay
    def get_room(self): return self._room
    def set_instructor(self, instructor): self._instructor = instructor
    def set_meetingTime(self, meetingTime): self._meetingTime = meetingTime
    def set_meetingDay(self, meetingDay): self._meetingDay = meetingDay
    def set_room(self, room): self._room = room

    def __str__(self):
        return str(self._course)


class Population():
    def __init__(self, size):
        self._size = size
        self._data = data
        self._schedules = []
        for i in range(0, size):
            self._schedules.append(Schedule().initialize())

    def get_schedules(self):
        return self._schedules


class GeneticAlgorithm:

    def evolve(self, population):
        return self._mutate_population(self._crossover_population(population))

    def _crossover_population(self, pop):
        crossover_pop = Population(0)
        for i in range(ELITE_SCHEDULES):
            crossover_pop.get_schedules().append(pop.get_schedules()[i])
        i = ELITE_SCHEDULES
        while i < POPULATION_SIZE:
            schedule_one = self._select_tournament_population(pop).get_schedules()[
                0]
            schedule_two = self._select_tournament_population(pop).get_schedules()[
                0]
            crossover_pop.get_schedules().append(
                self._crossover_schedule(schedule_one, schedule_two))
            i += 1
        return crossover_pop

    def _mutate_population(self, population):
        for i in range(ELITE_SCHEDULES, POPULATION_SIZE):
            self._mutate_schedule(population.get_schedules()[i])
        return population

    def _crossover_schedule(self, schedule_one, schedule_two):
        crossover = Schedule().initialize()
        for i in range(0, len(crossover.get_classes())):
            if (rnd.random() > 0.5):
                crossover.get_classes()[i] = schedule_one.get_classes()[i]
            else:
                crossover.get_classes()[i] = schedule_two.get_classes()[i]
        return crossover

    def _mutate_schedule(self, mutate_schedule):
        schedule = Schedule().initialize()
        for i in range(0, len(mutate_schedule.get_classes())):
            if (MUTATION_RATE > rnd.random()):
                mutate_schedule.get_classes()[i] = schedule.get_classes()[i]
        return mutate_schedule

    def _select_tournament_population(self, pop):
        tournament_population = Population(0)
        i = 0
        while i < TOURNAMENT_SELECTION_SIZE:
            tournament_population.get_schedules().append(
                pop.get_schedules()[rnd.randrange(0, POPULATION_SIZE)])
            i += 1
        tournament_population.get_schedules().sort(
            key=lambda x: x.get_fitness(), reverse=True)
        return tournament_population



generation = 0
populaion = Population(POPULATION_SIZE)
populaion.get_schedules().sort(key=lambda x: x.get_fitness(), reverse=True)

gen_algo = GeneticAlgorithm()

while populaion.get_schedules()[0].get_fitness() != 1.0:
    generation += 1
    print(f"Generation: {generation}")
    print(f"Fitness: {populaion.get_schedules()[0].get_fitness()}")
    populaion = gen_algo.evolve(populaion)
    populaion.get_schedules().sort(key=lambda x: x.get_fitness(), reverse=True)

print("Generation:", generation)
print("Fitness:",populaion.get_schedules()[0].get_fitness())

main_schedule = populaion.get_schedules()[0]
