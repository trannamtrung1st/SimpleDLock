using System;

namespace SimpleDLock.Core.Entities
{
    public class BookingEntity
    {
        public BookingEntity()
        {
        }

        public Guid Id { get; set; }
        public string FieldName { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset BookedTime { get; set; }
    }
}
