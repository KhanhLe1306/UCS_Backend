﻿using UCS_Backend.Models;

namespace UCS_Backend.Interfaces
{
/// <summary>
/// Creates class for scheduleRepo
/// </summary>
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        List<Schedule> GetAllSchedules();
        bool ValidateInsert(string cls, string section, string instructor, string classSize, string classTime, string roomCode, string room, string days);
    }
}
