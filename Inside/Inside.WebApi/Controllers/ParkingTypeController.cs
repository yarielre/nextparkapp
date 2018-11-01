using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Data.Infrastructure;
using Inside.Data.Repositories;
using Inside.Domain.Entities;
using Inside.WebApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inside.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ParkingTypeController : BaseController<ParkingType,ParkingTypeViewModel>
    {
        public ParkingTypeController(IRepository<ParkingType> repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}