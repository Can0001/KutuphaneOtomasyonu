using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IBookTransactionService
    {
        List<BookTransaction> GetAll();
        BookTransaction GetById(int id);
        void Add(BookTransaction bookTransaction);
        void Update(BookTransaction bookTransaction);
        void Delete(BookTransaction bookTransaction);
    }
}