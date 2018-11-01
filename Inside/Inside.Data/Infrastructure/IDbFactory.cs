using Inside.Data.Context;

namespace Inside.Data.Infrastructure
{
    public interface IDbFactory
    {
        InsideContext Init();
    }
}