using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _datacontext;
        public ValuesController(DataContext _datacontext)
        {
            this._datacontext = _datacontext;

        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var values = await _datacontext.values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var values = await _datacontext.values.Where(x => x.Id == id).FirstOrDefaultAsync();
            return Ok(values);
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] Value value)
        {
            Value objs = new Value() { Name = value.Name };
            _datacontext.values.Add(objs);
            await _datacontext.SaveChangesAsync();

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
