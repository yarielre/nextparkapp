using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Models;
using Inside.Web.Infrastructure;
using Inside.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inside.Web.Controllers.Api
{
    [Route("api/ParkingCategory")]
    public class ParkingCategoryController : BaseController<ParkingCategory,ParkingCategoryModel>
    {
        public ParkingCategoryController(IRepository<ParkingCategory> repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}