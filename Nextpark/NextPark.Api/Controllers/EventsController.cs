using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Models;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IRepository<Event> _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EventsController(IRepository<Event> repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // GET api/controller
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _repository.FindAllAsync();
            var vm = _mapper.Map<List<Event>, List<EventModel>>(list);
            return Ok(vm);
        }

        // GET api/controller/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _repository.Find(id);
            if (entity == null)
                return BadRequest("Entity not found");
            var vm = _mapper.Map<Event, EventModel>(entity);
            return Ok(vm);
        }

        // POST api/controller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Event entity)
        {
             if (entity == null)
                return BadRequest("adding null entity");
            if (ModelState.IsValid)
            {
                _repository.Add(entity);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<Event, EventModel>(entity);
                return Ok(vm);
            }
            return BadRequest(ModelState);
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]Event entity)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                var vm = _mapper.Map<Event, EventModel>(entity);
                await _unitOfWork.CommitAsync();
                return Ok(vm);
            }
            return BadRequest(ModelState);
        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _repository.Find(id);
            if (entity == null)
                return BadRequest("Can't deleted, entity not found.");
            var vm = _mapper.Map<Event, EventModel>(entity);
            _repository.Delete(entity);

            await _unitOfWork.CommitAsync();
            return Ok(vm);
        }
    }
}