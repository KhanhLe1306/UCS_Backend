using Microsoft.AspNetCore.Mvc;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/{controller}/")]
    public class RoomController : Controller
    {
        private IRoomRepository _roomRepository;
        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        [HttpPost("add")]
        public Room Add(Room room)
        {
            return _roomRepository.Add(room);
        }
    }
}
