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
        /// <summary>
        /// creates room
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public Room Add(Room room)
        {
            return _roomRepository.Add(room);
        }
        /// <summary>
         /// List generated from schedule info by passing room number
         /// </summary>
         /// <param name="roomNumber"></param>
         /// <returns></returns>
        [HttpGet("getScheduleByRoomNumber/{roomNumber}")]
        public List<ScheduleInfo> GetScheduleByRoomNumber(int roomNumber)
        {
            var res = _roomRepository.GetScheduleByRoomNumber(roomNumber);
            return res;
        }
    }
}
