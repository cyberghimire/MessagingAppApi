using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.API.Controllers{

    [ApiController]
    [Route("api/tests")]
    public class TestController: ControllerBase{
        [HttpGet()]
        public IActionResult GetValues(){
            return Ok(new {name= "Aadarsha", surname= "Ghimire"});
        }
    }
    
}