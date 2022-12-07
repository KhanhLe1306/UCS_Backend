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
        /// <summary>
        /// datacontext for Instructor repo
        /// </summary>
        /// <param name="dataContext"></param>
        public InstructorClassRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<InstructorClass> GetAll => this._dataContext.InstructorClasses.AsEnumerable();
        /// <summary>
        /// list of instructorClass
        /// </summary>
        /// <returns></returns>
        public async Task<List<InstructorClass>> GetInstructorClasses()
        {
            return await this._dataContext.InstructorClasses.ToListAsync();
        }
        /// <summary>
        /// method to get instructorClass by classID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// add instructorClassto data context
        /// </summary>
        /// <param name="instructorClass"></param>
        /// <returns></returns>
        public async Task<InstructorClass> AddInstructorClass(InstructorClass instructorClass)
        {
            var res = (await _dataContext.InstructorClasses.AddAsync(instructorClass)).Entity;
            await _dataContext.SaveChangesAsync();
            return res;
        }
        /// <summary>
        /// add instructorClass
        /// </summary>
        /// <param name="instructorClass"></param>
        /// <returns></returns>
        public InstructorClass Add(InstructorClass instructorClass)
        {
            var res = _dataContext.InstructorClasses.Add(instructorClass).Entity;
            _dataContext.SaveChanges();
            return res;
        }
        /// <summary>
        /// Update instructor class
        /// </summary>
        /// <param name="instructorClass"></param>
        /// <returns></returns>
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
        /// <summary>
        /// be able to delete instructorClass
        /// </summary>
        /// <param name="instructorClass"></param>
        /// <returns></returns>
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
        /// <summary>
        /// find instructor class by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public InstructorClass? FindById(int id)
        {
            return _dataContext.InstructorClasses.Find(id);
        }
        /// <summary>
        /// delete instructorClass
        /// </summary>
        /// <param name="instructorClass"></param>
        public void Delete(InstructorClass instructorClass)
        {
            this._dataContext.InstructorClasses.Remove(instructorClass);
            this._dataContext.SaveChanges();
        }
        /// <summary>
        /// updates instructorClass
        /// </summary>
        /// <param name="instructorClass"></param>
        public void Update(InstructorClass instructorClass)
        {

        }
        /// <summary>
        /// updates instructorClass
        /// </summary>
        /// <param name="instructorClass"></param>
        /// <returns></returns>
        public Task<(bool, InstructorClass)> UpdateInstrutor(InstructorClass instructorClass)
        {
            throw new NotImplementedException();
        }

        public int AddUpdateInstructorClass(InstructorClass instructureClass)
        {
            var temp = this._dataContext.InstructorClasses.Where(x => x.ClassId == instructureClass.ClassId && x.InstructorId == instructureClass.InstructorId).FirstOrDefault();
            if(temp == null)
            {
                var res = this._dataContext.InstructorClasses.Add(instructureClass).Entity;
                this._dataContext.SaveChanges();
                return res.InstructorClassId;
            }
            else
            {
                return temp.InstructorClassId;
            }
        }
    }
}
