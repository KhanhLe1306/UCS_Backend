using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private DataContext _dataContext;
        public RoomRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IEnumerable<Room> GetAll => throw new NotImplementedException();

        public Room Add(Room room)
        {
            int roomId = FindRoomIdByName(room.Name);
            if (roomId == 0)
            {
                var res = _dataContext.Rooms.Add(room).Entity;
                _dataContext.SaveChanges();
                return res;
            }
            else
            {
                return new Room
                {
                    RoomId = roomId,
                    Name = room.Name,
                    Capacity = room.Capacity
                };
            }
          
        }

        public void Delete(Room entity)
        {
            throw new NotImplementedException();
        }

        public Room? FindById(int id)
        {
            var res = from r in _dataContext.Rooms
                      where r.RoomId == id
                      select r;
            return res.FirstOrDefault();
        }

        public int FindRoomIdByName(string name)
        {
            var res = from r in _dataContext.Rooms
                      where r.Name == name
                      select r.RoomId;
            return res.FirstOrDefault();
        }

        public void Update(Room entity)
        {
            throw new NotImplementedException();
        }
    }
}
