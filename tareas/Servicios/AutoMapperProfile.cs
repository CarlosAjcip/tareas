using AutoMapper;
using tareas.Models;

namespace tareas.Servicios
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Tarea, TareaDTO>();
        }
    }
}
