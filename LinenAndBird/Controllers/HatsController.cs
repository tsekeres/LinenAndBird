using LinenAndBird.DataAccess;
using LinenAndBird.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinenAndBird.Controllers
{
    [Route("api/hats")]
    [ApiController]
    public class HatsController : ControllerBase
    {
        HatRepository _repo;
        public HatsController()
        {
            _repo = new HatRepository();
        }
        [HttpGet]
        public List<Hat> GetAllHats()
        {
            return _repo.GetAll();
        }

        [HttpGet("styles/{style}")]
        public IEnumerable<Hat> GetHatsByStyle(HatStyle style)
        {
            return _repo.GetByStyle(style);
        }

        [HttpPost]
        public void AddAHat(Hat newHat)
        {
            _repo.Add(newHat);
        }
    }
}
