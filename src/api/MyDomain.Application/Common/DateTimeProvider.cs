using MyDomain.Application.Common.Interfaces;

namespace MyDomain.Application.Common;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
