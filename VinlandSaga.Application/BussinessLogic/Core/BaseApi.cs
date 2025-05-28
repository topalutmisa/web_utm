using System;
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
    }
} 