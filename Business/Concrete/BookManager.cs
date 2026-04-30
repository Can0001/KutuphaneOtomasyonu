using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;

namespace Business.Concrete
{
    public class BookManager : IBookService
    {
        private readonly IBookDal _bookDal;

        public BookManager(IBookDal bookDal)
        {
            _bookDal = bookDal;
        }

        public List<Book> GetAll()
        {
            return _bookDal.GetAll();
        }

        public Book GetById(int id)
        {
            return _bookDal.Get(b => b.Id == id);
        }

        public void Add(Book book)
        {
            _bookDal.Add(book);
        }

        public void Update(Book book)
        {
            _bookDal.Update(book);
        }

        public void Delete(Book book)
        {
            if (book.IsAvailable == false)
            {
                throw new Exception("Bu eser şu an bir öğrencide ödünç olarak görünüyor. Silmeden önce iade almalısınız.");
            }
            _bookDal.Delete(book);
        }

        public void BorrowBook(int id, int userId, string userName)
        {
            var book = _bookDal.Get(b => b.Id == id);
            if (book != null)
            {
                book.IsAvailable = false;
                book.BorrowerId = userId;  
                book.BorrowedBy = userName; 
                book.BorrowedDate = DateTime.Now; 
                _bookDal.Update(book);
            }
        }

        public void ReturnBook(int id)
        {
            var book = _bookDal.Get(b => b.Id == id);
            if (book != null)
            {
                book.IsAvailable = true;
                book.BorrowerId = null;   
                book.BorrowedBy = null;
                book.BorrowedDate = null;
                _bookDal.Update(book);
            }
        }
    }
}