﻿using AutoMapper;
using tareas.Models;

namespace tareas.Servicios
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Tarea, TareaDTO>()
                .ForMember(dto => dto.PasosTotal, ent => ent.MapFrom(x => x.Pasos.Count()))
                .ForMember(dto => dto.PasosRealizados, ent => ent.MapFrom(x => x.Pasos.Where(p => p.Realizado).Count()));
        }
    }
}
