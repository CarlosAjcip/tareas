﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tareas.Models;
using tareas.Servicios;

namespace tareas.Controllers
{
    [Route("api/archivos")]
    public class ArchivosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IServiciosUsuarios serviciosUsuarios;
        private readonly string contenedor = "archivosAdjuntos";

        public ArchivosController(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos, IServiciosUsuarios serviciosUsuarios)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.serviciosUsuarios = serviciosUsuarios;
        }

        [HttpPost("{tareaId:int}")]
        public async Task<ActionResult<IEnumerable<ArchivoAdjunto>>> Post(int tareaId, [FromForm] IEnumerable<IFormFile> archivos)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var tarea = await context.Tareas.FirstOrDefaultAsync(x => x.Id == tareaId);

            if (tarea is null)
            {
                return NotFound();
            }

            if (tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            var existenArchivosAdjuntos = await context.ArchivoAdjuntos.AnyAsync(x => x.TareaId == tareaId);

            var ordenMayor = 0;
            if (existenArchivosAdjuntos)
            {
                ordenMayor = await context.ArchivoAdjuntos
                    .Where(a => a.TareaId == tareaId).Select(a => a.Orden).MaxAsync();
            }

            var resultado = await almacenadorArchivos.Almacenar(contenedor, archivos);

            var archivosAdjuntos = resultado.Select((resultado,indice) => new ArchivoAdjunto
            {
                TareaId = tareaId,
                FechaCreacion = DateTime.UtcNow,
                Url = resultado.URL,
                Titulo = resultado.Titulo,
                Orden = ordenMayor + indice + 1
            }).ToList();

            context.AddRange(archivosAdjuntos);
            await context.SaveChangesAsync();
            return archivosAdjuntos.ToList();
        }
    }
}
