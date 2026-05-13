using Business.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class OverdueService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OverdueService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                CheckOverdueBooks();

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                //await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private void CheckOverdueBooks()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var transactionService = scope.ServiceProvider.GetRequiredService<IBookTransactionService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

                var activeTransactions = transactionService.GetAll().Where(t => t.Status == "Approved").ToList();

                foreach (var transaction in activeTransactions)
                {
                    if (transaction.DueDate < DateTime.Now)
                    {
                        transaction.Status = "Overdue";
                        transaction.ReturnDate = null;
                        transactionService.Update(transaction);

                        // Ceza Puan Sistemi
                        var user = userService.GetById(transaction.UserId);
                        if (user != null)
                        {
                            user.PenaltyScore += 10; // 10 Ceza Puanı Yapıştır

                            if (user.TrustScore > 0)
                            {
                                user.TrustScore = 0; // Güven puanını sıfırla
                            }

                            userService.Update(user); // Kullanıcıyı veritabanında güncelle

                            Console.WriteLine($"[SİSTEM ALARMI]: {transaction.Id} nolu işlem gecikti! {user.FirstName} kullanıcısına 10 Ceza Puanı işlendi.");
                        }
                    }
                    // 2. KURAL: 3 GÜN KALDIYSA (MAİL AT)
                    else if (transaction.DueDate.Date == DateTime.Now.AddDays(3).Date)
                    {
                        var user = userService.GetById(transaction.UserId);
                        var book = bookService.GetById(transaction.BookId);

                        if (user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            string subject = "🔔 Kütüphane Teslim Hatırlatması - BibliosHub";
                            string body = $@"
                            <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background-color: #f4f4f9; padding: 30px; border-radius: 12px;'>
    
                            <div style='background-color: #4F46E5; padding: 20px; border-radius: 10px 10px 0 0; text-align: center;'>
                                <h1 style='color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px;'>BibliosHub</h1>
                                <p style='color: #e0e7ff; margin: 5px 0 0 0; font-size: 14px;'>Kütüphane Yönetim Sistemi</p>
                            </div>

                            <div style='background-color: #ffffff; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e5e7eb; border-top: none;'>
                                <h2 style='color: #1f2937; margin-top: 0;'>Merhaba {user.FirstName},</h2>
                                
                                <p style='color: #4b5563; font-size: 16px; line-height: 1.6;'>
                                    Kütüphanemizden ödünç almış olduğunuz <strong style='color: #4F46E5;'>'{book?.Title}'</strong> adlı eserin iade süresinin dolmasına çok az bir zaman kaldı.
                                </p>

                                <div style='background-color: #fef2f2; border-left: 5px solid #ef4444; padding: 15px 20px; margin: 25px 0; border-radius: 0 8px 8px 0;'>
                                    <p style='margin: 0 0 10px 0; font-size: 18px; color: #b91c1c;'>
                                        <strong>Kalan Süre:</strong> 3 Gün
                                    </p>
                                    <p style='margin: 0; font-size: 16px; color: #1f2937;'>
                                        <strong>Son Teslim Tarihi:</strong> {transaction.DueDate.ToShortDateString()}
                                    </p>
                                </div>

                                <p style='color: #4b5563; font-size: 15px; line-height: 1.6;'>
                                    Olası ceza puanlarından kaçınmak ve sistemdeki ödünç alma limitlerinizin kısıtlanmaması için lütfen kitabı belirtilen tarihe kadar kütüphaneye teslim ediniz.
                                </p>

                                <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;' />
                                
                                <p style='color: #9ca3af; font-size: 13px; text-align: center; margin: 0;'>
                                    Bu otomatik bir bilgilendirme mesajıdır, lütfen cevaplamayınız.<br>
                                    Keyifli okumalar dileriz, <strong>BibliosHub Yönetimi</strong>
                                </p>
                            </div>
                                </div>";

                            try
                            {
                                emailService.SendEmail(user.Email, subject, body);
                                Console.WriteLine($"[BİLGİ]: {user.FirstName} adlı öğrenciye ({user.Email}) hatırlatma maili gönderildi.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[HATA]: Mail gönderilemedi! Öğrenci: {user.Email} | Detay: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }
    }
}