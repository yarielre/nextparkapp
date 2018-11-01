using Inside.Web.Data;

namespace Inside.Web.Infrastructure
{
    public interface IDbFactory
    {
        ApplicationDbContext Init();
    }
}