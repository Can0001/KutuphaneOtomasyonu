using Core.DataAccess;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IBookTransactionDal : IEntityRepository<BookTransaction>
    {
    }
}