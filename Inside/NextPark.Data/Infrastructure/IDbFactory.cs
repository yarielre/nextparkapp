namespace NextPark.Data.Infrastructure
{
    public interface IDbFactory
    {
        ApplicationDbContext Init();
    }
}