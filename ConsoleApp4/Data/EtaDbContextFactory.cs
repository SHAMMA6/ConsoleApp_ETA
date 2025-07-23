using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp_ETA_eReceipts.Data
{
    public class EtaDbContextFactory : IDesignTimeDbContextFactory<EtaDbContext>
    {
        public EtaDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EtaDbContext>();

            optionsBuilder.UseSqlServer("Data Source=eta.db");
            optionsBuilder.UseSqlServer("Server=.;Database=Test_EReceipts_Consoul_App;User Id=sa;Password=P@ssw0rd;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;");

            return new EtaDbContext(optionsBuilder.Options);
        }
    }
}
