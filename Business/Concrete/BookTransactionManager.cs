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
        private readonly IBookDal _bookDal;
        private readonly IUserService _userService;

        public BookTransactionManager(IBookTransactionDal bookTransactionDal, IBookDal bookDal, IUserService userService)
        {
            _bookTransactionDal = bookTransactionDal;
            _bookDal = bookDal;
            _userService = userService;
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
            var book = _bookDal.Get(b => b.Id == bookTransaction.BookId);
            if (book != null && book.Status == false)
            {
                throw new Exception("Hata: Bu kitap şu an kullanım dışı (pasif) olduğu için talep edilemez!");
            }

            var isAlreadyActive = _bookTransactionDal.Get(t =>
                t.UserId == bookTransaction.UserId &&
                t.BookId == bookTransaction.BookId &&
                (t.Status == "Pending" || t.Status == "Approved" || t.Status == "Overdue")
            ) != null;

            if (isAlreadyActive)
            {
                throw new Exception("Bu kitap için zaten onay bekleyen bir talebiniz veya aktif olarak ödünç aldığınız bir kaydınız var!");
            }

            var user = _userService.GetById(bookTransaction.UserId);
            int maxLimit = 5; 

            if (user != null)
            {
                if (user.TrustScore >= 100) maxLimit = 10; 
                else if (user.TrustScore >= 50) maxLimit = 7; 
            }

            var currentBookCount = _bookTransactionDal.GetAll(t =>
                t.UserId == bookTransaction.UserId &&
                (t.Status == "Pending" || t.Status == "Approved" || t.Status == "Overdue")
            ).Count;

            if (currentBookCount >= maxLimit)
            {
                throw new Exception($"Limit doldu! Güven puanınıza göre aynı anda en fazla {maxLimit} kitap talebinde bulunabilir veya ödünç alabilirsiniz.");
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
                var book = _bookDal.Get(b => b.Id == transaction.BookId);
                if (book != null && book.Status == false)
                {
                    throw new Exception("Hata: Bu kitap sistemde pasif duruma alındığı için talebi onaylayamazsınız!");
                }

                var user = _userService.GetById(transaction.UserId);
                int maxLimit = 5;

                if (user != null)
                {
                    if (user.TrustScore >= 100) maxLimit = 10;
                    else if (user.TrustScore >= 50) maxLimit = 7;
                }

                var activeBooksCount = _bookTransactionDal.GetAll(t =>
                    t.UserId == transaction.UserId &&
                    (t.Status == "Approved" || t.Status == "Overdue")
                ).Count;

                if (activeBooksCount >= maxLimit)
                {
                    throw new Exception($"Öğrencinin elinde zaten {activeBooksCount} adet aktif/gecikmiş kitap var. Güven puanına göre ({maxLimit} limit) daha fazla onay verilemez!");
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

        public void ReturnBook(int transactionId)
        {
            var transaction = _bookTransactionDal.Get(t => t.Id == transactionId);
            if (transaction != null)
            {
                transaction.Status = "Returned";
                transaction.ReturnDate = DateTime.Now;
                _bookTransactionDal.Update(transaction);

                var user = _userService.GetById(transaction.UserId);
                if (user != null)
                {
                    if (transaction.ReturnDate <= transaction.DueDate)
                    {
                        if (user.PenaltyScore > 0)
                        {
                            user.PenaltyScore -= 5;
                            if (user.PenaltyScore < 0) user.PenaltyScore = 0;
                        }
                        else
                        {
                            user.TrustScore += 5;
                            if (user.TrustScore > 100) user.TrustScore = 100;
                        }
                        _userService.Update(user);
                    }
                }
            }
        }

        public List<BookTransaction> GetByUserId(int userId)
        {
            return _bookTransactionDal.GetAll(t => t.UserId == userId);
        }

        public void ReturnByBookId(int bookId)
        {
            var transaction = _bookTransactionDal.Get(t =>
                t.BookId == bookId &&
                (t.Status == "Approved" || t.Status == "Overdue")
            );

            if (transaction != null)
            {
                transaction.Status = "Returned";
                transaction.ReturnDate = DateTime.Now;
                _bookTransactionDal.Update(transaction);

                var user = _userService.GetById(transaction.UserId);
                if (user != null)
                {
                    if (transaction.ReturnDate <= transaction.DueDate)
                    {
                        if (user.PenaltyScore > 0)
                        {
                            user.PenaltyScore -= 5;
                            if (user.PenaltyScore < 0) user.PenaltyScore = 0;
                        }
                        else
                        {
                            user.TrustScore += 5;
                            if (user.TrustScore > 100) user.TrustScore = 100;
                        }
                        _userService.Update(user);
                    }
                }
            }
            else
            {
                throw new Exception("Bu kitap şu an kimseye ödünç verilmemiş veya zaten iade edilmiş!");
            }
        }
    }
}