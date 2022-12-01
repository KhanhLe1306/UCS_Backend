using System;
using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UCS_Backend.Interfaces.IRepositories;

namespace UCS_Backend.Repositories
{


    /// <summary>
    /// Creates a class for IndividualClassRepositoty
    /// </summary> 
    public class InstructorClassRepository : IInstructorClassRepository, IBaseRepository<InstructorClass>
    {
        private DataContext _dataContext;

        public InstructorClassRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<InstructorClass> GetAll => this._dataContext.InstructorClasses.AsEnumerable();

        public async Task<List<InstructorClass>> GetInstructorClasses()
        {
            return await this._dataContext.InstructorClasses.ToListAsync();
        }

        public async Task<InstructorClass> GetInstructorClassById(int id)
        {
            var res = from i in this._dataContext.InstructorClasses
                      where i.InstructorClassId == id
                      select new InstructorClass
                      {
                          InstructorClassId = i.InstructorClassId,
                          InstructorId = i.InstructorId,
                          ClassId = i.ClassId,
                      };

            return await res.FirstAsync();
        }

        public async Task<InstructorClass> AddInstructorClass(InstructorClass instructorClass)
        {
            var res = (await _dataContext.InstructorClasses.AddAsync(instructorClass)).Entity;
            await _dataContext.SaveChangesAsync();
            return res;
        }

        public InstructorClass Add(InstructorClass instructorClass)
        {
            var res = _dataContext.InstructorClasses.Add(instructorClass).Entity;
            _dataContext.SaveChanges();
            return res;
        }

        public async Task<(bool, InstructorClass)> UpdateInstructorClass(InstructorClass instructorClass)
        {
            var temp = await this._dataContext.InstructorClasses.Where(i => i.InstructorId == instructorClass.InstructorId).FirstAsync();
            if (temp != null)
            {
                temp.InstructorId = instructorClass.InstructorId;
                temp.ClassId = instructorClass.ClassId;
                await _dataContext.SaveChangesAsync();
                return (true, temp);
            }
            else
            {
                return (false, instructorClass);
            }
        }

        public async Task<bool> DeleteInstructorClass(InstructorClass instructorClass)
        {
            var res = this._dataContext.InstructorClasses.Remove(instructorClass).Entity;
            if (res != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public InstructorClass? FindById(int id)
        {
            return _dataContext.InstructorClasses.Find(id);
        }

        public void Delete(InstructorClass instructorClass)
        {
            this._dataContext.InstructorClasses.Remove(instructorClass);
            this._dataContext.SaveChanges();
        }

        public void Update(InstructorClass instructorClass)
        {

        }

        public Task<(bool, InstructorClass)> UpdateInstrutor(InstructorClass instructorClass)
        {
            throw new NotImplementedException();
        }

    }
}
