using System;
using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UCS_Backend.Repositories
{
    public class InstructorRepository: IInstructorRepository, IBaseRepository<Instructor>
    {
        private DataContext _dataContext;
        public InstructorRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }
        public IEnumerable<Instructor> GetAll => this._dataContext.Instructors.AsEnumerable();

        public async Task<List<Instructor>> GetAllInstructors()
        {
            return await this._dataContext.Instructors.ToListAsync();
        }
        public async Task<Instructor?> GetInstructorById(int id)
        {
            var res = from i in this._dataContext.Instructors
                      where i.InstructorId == id
                      select new Instructor
                      {
                          InstructorId = i.InstructorId,
                          FirstName = i.FirstName,
                          LastName = i.LastName,
                      };

            return await res.FirstAsync();
        }

        public async Task<Instructor> AddInstructor(Instructor instructor)
        {
            var res = (await _dataContext.Instructors.AddAsync(instructor)).Entity;
            await _dataContext.SaveChangesAsync();
            return res;
        }

        public Instructor Add(Instructor instructor)
        {
            int instructorId = FindInstructorByName(instructor.FirstName, instructor.LastName);
            if (instructorId == 0)
            {
                var res = _dataContext.Instructors.Add(instructor).Entity;
                _dataContext.SaveChanges();
                return res;
            }
            return null;
        }

        public async Task<(bool, Instructor)> UpdateIndividual(Instructor instructor)
        {
            var temp = await this._dataContext.Instructors.Where(i => i.InstructorId == instructor.InstructorId).FirstAsync();
            if (temp != null)
            {
                temp.FirstName = instructor.FirstName;
                temp.LastName = instructor.LastName;
                await _dataContext.SaveChangesAsync();
                return (true, temp);
            }
            else
            {
                return (false, instructor);
            }
        }
        public async Task<bool> DeleteInstructor(Instructor instructor)
        {
            var res = this._dataContext.Instructors.Remove(instructor).Entity;
            if (res != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public Instructor? FindById(int id)
        {
            return _dataContext.Instructors.Find(id);
        }

        public int FindInstructorByName(string firstName, string lastName)
        {
            var res = from r in _dataContext.Instructors
                      where r.FirstName == firstName && r.LastName == lastName
                      select r.InstructorId;
            return res.FirstOrDefault();
        }

        public void Delete(Instructor instructor)
        {
            this._dataContext.Instructors.Remove(instructor);
            this._dataContext.SaveChanges();
        }

        public void Update(Instructor instructor)
        {

        }

        public Task<(bool, Instructor)> UpdateInstrutor(Instructor instructor)
        {
            throw new NotImplementedException();
        }
    }
}
