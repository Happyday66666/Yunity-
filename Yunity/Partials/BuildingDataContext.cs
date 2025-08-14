using Microsoft.EntityFrameworkCore;

namespace Yunity.Partials
{
    public partial class BuildingDataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=BuildingData;Integrated Security=True;Encrypt=False");
    }
}
