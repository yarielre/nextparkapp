using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Models;
using Inside.Web.Infrastructure;
using Inside.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inside.Web.Controllers.Api
{
    [Route("api/[controller]")]
    public class ParkingTypeController : BaseController<ParkingType,ParkingTypeModel>
    {
        public ParkingTypeController(IRepository<ParkingType> repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}