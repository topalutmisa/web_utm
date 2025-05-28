using System;
using System.Collections.Generic;
using System.Linq;
using VinlandSaga.Infrastructure.Data;

namespace VinlandSaga.Application.BussinessLogic.Core
{
    public abstract class BaseApi
    {
        protected readonly VinlandSagaDbContext _context;
        
        protected BaseApi()
        {
            _context = new VinlandSagaDbContext();
        }
        
        protected BaseApi(VinlandSagaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public virtual void Dispose()
        {
            _context?.Dispose();
        }
        
        protected virtual void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
                throw;
            }
        }

        // Базовые CRUD операции
        protected virtual T GetById<T>(Guid id) where T : class
        {
            try
            {
                return _context.Set<T>().Find(id);
            }
            catch
            {
                return null;
            }
        }

        protected virtual List<T> GetAll<T>() where T : class
        {
            try
            {
                return _context.Set<T>().ToList();
            }
            catch
            {
                return new List<T>();
            }
        }

        protected virtual bool Create<T>(T entity) where T : class
        {
            try
            {
                _context.Set<T>().Add(entity);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected virtual bool Update<T>(T entity) where T : class
        {
            try
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected virtual bool Delete<T>(Guid id) where T : class
        {
            try
            {
                var entity = GetById<T>(id);
                if (entity == null) return false;
                
                _context.Set<T>().Remove(entity);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected virtual int Count<T>() where T : class
        {
            try
            {
                return _context.Set<T>().Count();
            }
            catch
            {
                return 0;
            }
        }

        protected virtual List<T> GetPaged<T>(int page, int pageSize) where T : class
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return _context.Set<T>().Skip(skip).Take(pageSize).ToList();
            }
            catch
            {
                return new List<T>();
            }
        }
    }
} 