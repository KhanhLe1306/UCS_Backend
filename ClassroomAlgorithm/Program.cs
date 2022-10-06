Console.WriteLine("Hello");

List<Lecture> lectures = new List<Lecture>();
List<Room> rooms = new List<Room>();

Room[] roomArray =
{
    new Room()
    {
        Id = 1,
        MaxCapaciry = 35,
        InUse = false,
    },
    new Room()
    {
        Id = 2,
        MaxCapaciry = 25,
        InUse = false,
    },
    new Room()
    {
        Id = 3,
        MaxCapaciry = 36,
        InUse = false,
    },
    new Room()
    {
        Id = 4,
        MaxCapaciry = 45,
        InUse = false,
    },
    new Room()
    {
        Id = 5,
        MaxCapaciry = 26,
        InUse = false,
    },
    new Room()
    {
        Id = 6,
        MaxCapaciry = 63,
        InUse = false,
    },
    new Room()
    {
        Id = 7,
        MaxCapaciry = 35,
        InUse = false,
    },
    new Room()
    {
        Id = 8,
        MaxCapaciry = 39,
        InUse = false,
    },
};

Lecture[] lecArray = {
   new Lecture()
   {
        Id = 1,
        Start = 830,
        End = 945,
        Room = null,
        Capacity = 50,
        IsDone = false,
   },
   new Lecture()
   {
        Id = 2,
        Start = 800,
        End = 915,
        Room = null,
        Capacity = 35,
        IsDone = false,
   },
   new Lecture()
   {
        Id = 3,
        Start = 945,
        End = 1100,
        Room = null,
        Capacity = 45,
        IsDone = false,
   },
};

lectures.AddRange(lecArray);
rooms.AddRange(roomArray);

// Sort lectures by start time asc order
for(int i = 0; i < lectures.Count; i++) {
    for (int j = 0; j < lectures.Count - 1 - i; j++)
    {
        if (lectures[j].Start > lectures[j + 1].Start)
        {
            var temp = lectures[j + 1];
            lectures[j + 1] = lectures[j];
            lectures[j] = temp;
        }
    }
}

// Sort rooms by capacity asc order
for (int i = 0; i < rooms.Count; i++)
{
    for (int j = 0; j < rooms.Count - 1 - i; j++)
    {
        if (rooms[j].MaxCapaciry > rooms[j + 1].MaxCapaciry)
        {
            var temp = rooms[j + 1];
            rooms[j + 1] = rooms[j];
            rooms[j] = temp;
        }
    }
}

foreach (Lecture lecture in lectures)
{
    Console.WriteLine(lecture.Start);
}

foreach(Room room in rooms)
{
    Console.WriteLine(room.MaxCapaciry);
}

for (int i = 0; i < lectures.Count; i++)
{
    var s = lectures[i].Start;
    for (int j = 0; j < i; j++)
    {
        if (!lectures[j].IsDone && s > lectures[j].End)
        {
            if (lectures[i].Capacity > lectures[j].Room?.MaxCapaciry) continue;
            lectures[j].IsDone = true;
            lectures[i].Room = lectures[j].Room;
            rooms[rooms.FindIndex(r => r.Id == lectures[j].Room?.Id)].InUse = true;
            break;
        }
    }
    if (lectures[i].Room == null) // There is no lecture done
    {
        // Assign to the next available room with the least capacity efficient for the lecture's capacity
        // Next Maxcapacity and InUse is false

        var index = rooms.FindIndex(r => r.MaxCapaciry >= lectures[i].Capacity && r.InUse == false);
        lectures[i].Room = rooms[index];
        rooms[index].InUse = true;
    }
}

Console.WriteLine("After assigning classroom");

foreach (Lecture lecture in lectures)
{
    Console.WriteLine("Lecture Id: " + lecture.Id + " is assigned to room id: " + lecture.Room.Id);
}

public class Room
    {
        public Room() { }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int MaxCapaciry { get; set; }
        public bool InUse { get; set; }
    }

public class Lecture
    {
        public Lecture() { }

        public int Id { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public Room? Room { get; set; }
        public int Capacity { get; set; }
        public Boolean IsDone { get; set; }
    }



