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
            }
        }

        private void CheckOverdueBooks()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var transactionService = scope.ServiceProvider.GetRequiredService<IBookTransactionService>();

                var activeTransactions = transactionService.GetAll().Where(t => t.Status == "Approved").ToList();

                foreach (var transaction in activeTransactions)
                {
                    if (transaction.DueDate < DateTime.Now)
                    {
                        transaction.Status = "Overdue";
                        transaction.ReturnDate = null;
                        transactionService.Update(transaction);

                        Console.WriteLine($"[SİSTEM ALARMI]: {transaction.Id} nolu işlem gecikmeye düştü!");
                    }
                }
            }
        }
    }
}