using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CommonTypeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Service.Abstract;

namespace TenantWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Rack")]
    public class RackController : Controller
    {
        private readonly IRackService _ser;
        public RackController(IRackService ser)
        {
            _ser = ser;
        }

        // GET api/Rack
        [HttpGet]
		public async Task<IActionResult> Get()
		{
			//return await GetAllRacks();
			//BadRequest(new { error = "" }); // returns 401
			//return StatusCode(500, "Internal server error");
			return Ok(await GetAllRacks());   // returns 200
        }

        // POST api/Rack/AddRack
        [HttpPost("AddRack")]
	    public async Task AddRack([FromBody]Rack model)
        {
            await _ser.AddRack(model);
        }

        // GET api/Rack/GetAllRacks
        [HttpGet("GetAllRacks")]
		//[Authorize]
		public async Task<IEnumerable<Rack>> GetAllRacks()
        {
			var principal = HttpContext.User.Identity as ClaimsIdentity;
			var login = principal.Claims
				.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
				?.Value;

			// List
			Log.Logger.Information("Web API GetAllRacks Method Started...");
			IEnumerable<Rack> racklist = await _ser.GetAllRacks();
			Log.Logger.Information("GetAllRacks Method Ended...");
			return racklist;
        }
                
        // GET api/Rack/GetRack/5
        [HttpGet("GetRackById/{id}")]
        public async Task<Rack> GetRackById(string id)
        {
			return await _ser.GetRackById(id);
        }

		// GET api/Rack/GetRack/Rack1
		[HttpGet("GetRackByName/{name}")]
		public async Task<Rack> GetRackByName(string name)
		{
			return await _ser.GetRackByName(name);
		}

		// PUT api/Rack/UpdateRack
		[HttpPut("UpdatedRack")]
		public async Task<bool> UpdateRack(Rack model)
		{
			return await _ser.UpdateRack(model);
		}

		// Delete api/Rack/RemoveRack/5
		[HttpDelete("RemoveRackById/{id}")]
		public async Task<bool> RemoveRackById(string id)
		{
			return await _ser.RemoveRackById(id);
		}

		// Delete api/Rack/RemoveRack/Rack1
		[HttpDelete("RemoveRackByName/{name}")]
		public async Task<bool> RemoveRackByName(string name)
		{
			return await _ser.RemoveRackByName(name);
		}

		// Delete api/Rack/RemoveAllRacks/5
		[HttpDelete("RemoveAllRacks")]
		public async Task<bool> RemoveAllRacks()
		{
			return await _ser.RemoveAllRacks();
		}
	}
}