using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using System;
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

        public List<BookTransaction> GetAll()
        {
            return _bookTransactionDal.GetAll();
        }

        public BookTransaction GetById(int id)
        {
            return _bookTransactionDal.Get(t => t.Id == id);
        }

        public void Add(BookTransaction bookTransaction)
        {
            _bookTransactionDal.Add(bookTransaction);
        }

        public void Update(BookTransaction bookTransaction)
        {
            _bookTransactionDal.Update(bookTransaction);
        }

        public void Delete(BookTransaction bookTransaction)
        {
            _bookTransactionDal.Delete(bookTransaction);
        }

        public List<BookTransaction> GetPendingRequests()
        {
            return _bookTransactionDal.GetAll(t => t.Status == "Pending");
        }

        public void RequestBook(BookTransaction bookTransaction)
        {
            var isAlreadyActive = _bookTransactionDal.Get(t =>
                t.UserId == bookTransaction.UserId &&
                t.BookId == bookTransaction.BookId &&
                (t.Status == "Pending" || t.Status == "Approved")
            ) != null;

            if (isAlreadyActive)
            {
                throw new Exception("Bu kitap için zaten onay bekleyen bir talebiniz veya aktif olarak ödünç aldığınız bir kaydınız var!");
            }

            var currentBookCount = _bookTransactionDal.GetAll(t => t.UserId == bookTransaction.UserId && (t.Status == "Pending" || t.Status == "Approved")).Count;

            if (currentBookCount >= 5)
            {
                throw new Exception("Limit doldu! Aynı anda en fazla 5 kitap talebinde bulunabilir veya ödünç alabilirsiniz.");
            }

            bookTransaction.Status = "Pending";
            bookTransaction.TransactionDate = DateTime.Now;
            _bookTransactionDal.Add(bookTransaction);
        }

        public void ApproveRequest(int transactionId)
        {
            var transaction = _bookTransactionDal.Get(t => t.Id == transactionId);
            if (transaction != null)
            {
                var activeBooksCount = _bookTransactionDal.GetAll(t => t.UserId == transaction.UserId && t.Status == "Approved").Count;

                if (activeBooksCount >= 5)
                {
                    throw new Exception("Öğrencinin zaten 5 adet aktif kitabı var, limit dolu olduğu için daha fazla onay verilemez!");
                }

                transaction.Status = "Approved"; 
                _bookTransactionDal.Update(transaction);
            }
        }

        public void RejectRequest(int transactionId)
        {
            var transaction = _bookTransactionDal.Get(t => t.Id == transactionId);
            if (transaction != null)
            {
                transaction.Status = "Rejected"; 
                _bookTransactionDal.Update(transaction);
            }
        }
    }
}