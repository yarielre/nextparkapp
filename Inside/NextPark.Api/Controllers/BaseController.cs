using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextPark.Api.Controllers
{
    public class BaseController<T, TViewModel> : ControllerBase where T : BaseEntity, new()
    {
        private readonly IMapper _mapper;
        private readonly IRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public BaseController(IRepository<T> repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET api/controller
        [HttpGet]
        protected virtual async Task<IActionResult> Get()
        {
            var list = await _repository.FindAllAsync();
            var vm = _mapper.Map<List<T>, List<TViewModel>>(list);
            return Ok(vm);
        }

        // GET api/controller/5
        [HttpGet("{id}")]
        protected virtual IActionResult Get(int id)
        {
            var entity = _repository.Find(id);
            if (entity == null)
                return BadRequest("Entity not found");
            var vm = _mapper.Map<T, TViewModel>(entity);
            return Ok(vm);
        }

        // POST api/controller
        [HttpPost]
        protected virtual async Task<IActionResult> Post([FromBody] T entity)
        {
            if (entity == null)
                return BadRequest("adding null entity");
            if (ModelState.IsValid)
            {
                _repository.Add(entity);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<T, TViewModel>(entity);
                return Ok(vm);
            }
            return BadRequest(ModelState);
        }

        // PUT api/controller/5
        [HttpPut("{id}")]
        protected virtual async Task<ActionResult> Put(int id, [FromBody]T entity)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                var vm = _mapper.Map<T, TViewModel>(entity);
                await _unitOfWork.CommitAsync();
                return Ok(vm);
            }
            return BadRequest(ModelState);
        }

        // DELETE api/controller/5
        [HttpDelete("{id}")]
        protected virtual async Task<IActionResult> Delete(int id)
        {
                var entity = _repository.Find(id);
                if (entity == null)
                    return BadRequest("Can't deleted, entity not found.");
            var vm = _mapper.Map<T, TViewModel>(entity);
                _repository.Delete(entity);

            await _unitOfWork.CommitAsync();
            return Ok(vm);
        }


        
    }
}