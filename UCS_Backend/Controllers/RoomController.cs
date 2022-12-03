using Microsoft.AspNetCore.Mvc;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]

    /// <summary>
    /// Creates a class for RoomController
    /// </summary> 
    public class RoomController : Controller
    {
       /// <summary>
       /// creates room repository
       /// </summary>
        private IRoomRepository _roomRepository;
        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        [HttpPost("add")]
        /// <summary>
        /// creates room
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public Room Add(Room room)
        {
            return _roomRepository.Add(room);
        }
        [HttpGet("getScheduleByRoomNumber/{roomNumber}")]
         /// <summary>
         /// List generated from schedule info by passing room number
         /// </summary>
         /// <param name="roomNumber"></param>
         /// <returns></returns>
        public List<ScheduleInfo> GetScheduleByRoomNumber(int roomNumber)
        {
            var res = _roomRepository.GetScheduleByRoomNumber(roomNumber);
            return res;
        }
    }
}
