using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;

namespace MotoPack_project.Database
{
    public static class IniciarBaseDados
    {
        public static void Iniciar(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate(); // Aplica todas as migrações pendentes
            }
        }
    }
}
