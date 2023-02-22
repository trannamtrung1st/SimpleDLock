using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SimpleDLock.Core.Entities;
using SimpleDLock.Core.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

namespace SimpleDLock.Pages
{
    [BindProperties()]
    public class IndexModel : PageModel
    {
        private static readonly ConcurrentDictionary<string, object> _bookingLockMap = new ConcurrentDictionary<string, object>();

        private readonly ILogger<IndexModel> _logger;
        private readonly MainDbContext _dbContext;

        public IndexModel(
            ILogger<IndexModel> logger,
            MainDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
            var lockObj = _bookingLockMap.GetOrAdd(FieldName, (key) => new object());

            lock (lockObj)
            {
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
    }
}
