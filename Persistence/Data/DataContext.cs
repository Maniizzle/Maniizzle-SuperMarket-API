using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlite("Filename=DatingApp.db");
    //    base.OnConfiguring(optionsBuilder);
    //}
}