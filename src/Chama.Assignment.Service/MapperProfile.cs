using AutoMapper;
using Chama.Assignment.Service.Data.DataModels;
using Chama.Assignment.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chama.Assignment.Service
{
    public class MapperProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<TimerEntry, Stopwatch>();
        }
    }
}
