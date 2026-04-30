using Core.Entities;
using System.Linq.Expressions;

namespace Core.DataAccess
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        List<T> GetAll(Expression<Func<T, bool>> filter = null); // Tümünü veya şarta göre listele
        T Get(Expression<Func<T, bool>> filter); // Tek bir kayıt getir (Örn: Id'si 5 olan kitap)
        void Add(T entity); // Ekle
        void Update(T entity); // Güncelle
        void Delete(T entity); // Sil
    }
}