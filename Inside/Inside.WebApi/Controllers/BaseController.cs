using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Data.Infrastructure;
using Inside.Data.Repositories;
using Inside.Domain.Core;
using Inside.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inside.WebApi.Controllers
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
                _repository.UpdateGraph(entity);
                await _unitOfWork.CommitAsync();
               // var vm = _mapper.Map<T, TViewModel>(entity);
                return Ok(entity);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("deletemultiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody]ObjectsToDelete objectsToDelete)
        {
            foreach (var id in objectsToDelete.Ids)
            {
                var entity = _repository.Find(id);
                if (entity == null)
                    return BadRequest("Can't deleted, entity not found.");
                _repository.Delete(entity);
            }
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        [HttpPost("deleteone")]
        public async Task<IActionResult> DeleteOne([FromBody]int id)
        {
                var entity = _repository.Find(id);
                if (entity == null)
                    return BadRequest("Can't deleted, entity not found.");
                _repository.Delete(entity);

            await _unitOfWork.CommitAsync();
            return Ok(entity);
        }


        [HttpPut("edit")]
        public async Task<ActionResult> Edit([FromBody]T entity)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                return Ok();
            }
            return BadRequest(ModelState);
        }
    }
}