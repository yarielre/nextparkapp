using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Data.Infrastructure;
using Inside.Data.Repositories;
using Inside.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Inside.WebApi.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inside.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ParkingCategoryController : BaseController<ParkingCategory, ParkingCategoryViewModel>
    {
        public ParkingCategoryController(IRepository<ParkingCategory> repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}
