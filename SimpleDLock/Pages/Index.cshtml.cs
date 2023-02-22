using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleDLock.Core.Entities;
using SimpleDLock.Core.Persistence;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

namespace SimpleDLock.Pages
{
    [BindProperties()]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MainDbContext _dbContext;
        private readonly ConnectionMultiplexer _multiplexer;

        public IndexModel(
            ILogger<IndexModel> logger,
            MainDbContext dbContext,
            ConnectionMultiplexer multiplexer)
        {
            _logger = logger;
            _dbContext = dbContext;
            _multiplexer = multiplexer;
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
            var acquired = AcquireLock(FieldName, out var finalKey, out var randomVal);

            if (!acquired)
            {
                Message = $"{FieldName} is busy!";
            }
            else
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

                Release(finalKey, randomVal);
            }

            FetchData();
        }

        private void FetchData()
        {
            Bookings = _dbContext.Booking.OrderByDescending(b => b.BookedTime).ToArray();
            Fields = _dbContext.Field.ToArray();
        }

        private bool AcquireLock(string key, out string finalKey, out string randomVal)
        {
            var db = _multiplexer.GetDatabase();

            // Equivalent command: SET resource_name a_random_value NX PX {Milliseconds}
            finalKey = $"redis-lock-{key}";
            randomVal = Guid.NewGuid().ToString();

            bool acquired = db.StringSet(key: finalKey, value: randomVal,
                expiry: TimeSpan.FromMilliseconds(10000),
                when: When.NotExists);

            return acquired;
        }

        private void Release(string finalKey, string randomVal)
        {
            _multiplexer.GetDatabase().ScriptEvaluate(LuaScript.Prepare($@"
if redis.call(""get"",@key) == @value then
    return redis.call(""del"", @key)
else
    return 0
end
"), parameters: new { key = finalKey, value = randomVal });
        }
    }
}
