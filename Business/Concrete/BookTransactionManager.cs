using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using System.Collections.Generic;

namespace Business.Concrete
{
    public class BookTransactionManager : IBookTransactionService
    {
        private readonly IBookTransactionDal _bookTransactionDal;

        public BookTransactionManager(IBookTransactionDal bookTransactionDal)
        {
            _bookTransactionDal = bookTransactionDal;
        }

        public void Add(BookTransaction bookTransaction) { _bookTransactionDal.Add(bookTransaction); }
        public void Delete(BookTransaction bookTransaction) { _bookTransactionDal.Delete(bookTransaction); }
        public List<BookTransaction> GetAll() { return _bookTransactionDal.GetAll(); }
        public BookTransaction GetById(int id) { return _bookTransactionDal.Get(b => b.Id == id); }
        public void Update(BookTransaction bookTransaction) { _bookTransactionDal.Update(bookTransaction); }
    }
}