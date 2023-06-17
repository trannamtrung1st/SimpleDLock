using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;
using SimpleDLock.Core.Entities;
using SimpleDLock.Core.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleDLock.Pages
{
    [BindProperties()]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MainDbContext _dbContext;
        private readonly RedLockFactory _redLockFactory;

        public IndexModel(
            ILogger<IndexModel> logger,
            MainDbContext dbContext,
            RedLockFactory redLockFactory)
        {
            _logger = logger;
            _dbContext = dbContext;
            _redLockFactory = redLockFactory;
        }

        public IEnumerable<BookingEntity> Bookings { get; set; }
        public IEnumerable<FieldEntity> Fields { get; set; }

        public void OnGet()
        {
            FetchData();
        }

        [Required]
        public string FieldName { get; set; }

        [Required]
        public string UserName { get; set; }

        [BindNever]
        public string Message { get; private set; }

        public void OnPost()
        {
            using (var redLock = AcquireLock(FieldName))
            {
                if (!redLock.IsAcquired) throw new Exception("Timed out, couldn't acquire lock!");

                var exists = _dbContext.Booking.Any(b => b.FieldName == FieldName);

                if (exists)
                {
                    Message = $"{FieldName} is not available!";
                }
                else
                {
                    _dbContext.Add(new BookingEntity
                    {
                        Id = Guid.NewGuid(),
                        UserName = UserName,
                        FieldName = FieldName,
                        BookedTime = DateTimeOffset.Now
                    });

                    Thread.Sleep(2000);

                    _dbContext.SaveChanges();

                    Message = "Booked successfully!";
                }
            }

            FetchData();
        }

        private void FetchData()
        {
            Bookings = _dbContext.Booking.OrderByDescending(b => b.BookedTime).ToArray();
            Fields = _dbContext.Field.ToArray();
        }

        public IRedLock AcquireLock(string key)
        {
            var redLock = _redLockFactory.CreateLock(key,
                expiryTime: TimeSpan.FromMilliseconds(10000),
                waitTime: TimeSpan.FromMilliseconds(5000),
                retryTime: TimeSpan.FromMilliseconds(500));

            return redLock;
        }
    }
}
