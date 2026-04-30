using Core.DataAccess;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IBookDal : IEntityRepository<Book>
    {
        // (örneğin yazara göre getirme gibi) sorgular.
    }
}