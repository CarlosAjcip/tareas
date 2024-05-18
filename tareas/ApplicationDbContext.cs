using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tareas.Models;

namespace tareas
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Paso> Pasos { get; set; }
        public DbSet<ArchivoAdjunto> ArchivoAdjuntos { get; set; }

    }
}
