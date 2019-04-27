using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextPark.Models;
using NextPark.Services.Services;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public ValuesController(IFileService fileService)
        {
            _fileService = fileService;
        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _fileService.CreateFile(new OrderModel
            {
                EndDate = DateTime.Now,
                Id = 12
            });
            return Ok();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
