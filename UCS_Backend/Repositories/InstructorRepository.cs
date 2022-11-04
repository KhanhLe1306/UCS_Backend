using System;
using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UCS_Backend.Repositories
{
    public class InstructorRepository: IInstructorRepository, IBaseRepository<Instructor>
    {
        private DataContext dataContext;
        public InstructorRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public IEnumerable<Instructor> GetAll => this.dataContext.Instructors.AsEnumerable();

        public async Task<List<Instructor>> GetAllInstructors()
        {
            return await this.dataContext.Instructors.ToListAsync();
        }
        public async Task<Instructor?> GetInstructorById(int id)
        {
            var res = from i in this.dataContext.Instructors
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
            var res = (await dataContext.Instructors.AddAsync(instructor)).Entity;
            await dataContext.SaveChangesAsync();
            return res;
        }

        public Instructor Add(Instructor instructor)
        {
            var res =  dataContext.Instructors.Add(instructor).Entity;
            dataContext.SaveChanges();
            return res;
        }

        public async Task<(bool, Instructor)> UpdateIndividual(Instructor instructor)
        {
            var temp = await this.dataContext.Instructors.Where(i => i.InstructorId == instructor.InstructorId).FirstAsync();
            if (temp != null)
            {
                temp.FirstName = instructor.FirstName;
                temp.LastName = instructor.LastName;
                await dataContext.SaveChangesAsync();
                return (true, temp);
            }
            else
            {
                return (false, instructor);
            }
        }
        public async Task<bool> DeleteInstructor(Instructor instructor)
        {
            var res = this.dataContext.Instructors.Remove(instructor).Entity;
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
            return dataContext.Instructors.Find(id);
        }

        public void Delete(Instructor instructor)
        {
            this.dataContext.Instructors.Remove(instructor);
            this.dataContext.SaveChanges();
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
