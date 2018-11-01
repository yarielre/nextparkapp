using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Domain.Core;
using Inside.Web.Infrastructure;
using Inside.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inside.Web.Controllers.Api
{
  //  [Authorize]
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

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repository.FindAllAsync();
            var vm = _mapper.Map<List<T>, List<TViewModel>>(list);
            return Ok(vm);
        }

        [HttpGet("getone/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var entity =  _repository.Find(id);
            if (entity == null)
                return BadRequest("Entity not found");
            var vm = _mapper.Map<T, TViewModel>(entity);
            return Ok(vm);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] T entity)
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
 

        [HttpPost("deleteone")]
        public async Task<IActionResult> DeleteOne([FromBody]int id)
        {
                var entity = _repository.Find(id);
                if (entity == null)
                    return BadRequest("Can't deleted, entity not found.");
            var vm = _mapper.Map<T, TViewModel>(entity);
                _repository.Delete(entity);

            await _unitOfWork.CommitAsync();
            return Ok(vm);
        }


        [HttpPut("edit")]
        public async Task<ActionResult> Edit([FromBody]T entity)
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
    }
}