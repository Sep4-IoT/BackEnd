// using Microsoft.AspNetCore.Mvc;
// using System.IO;
// using System.Threading.Tasks;
//
// namespace RestController
// {
//     [ApiController]
//     [Route("[controller]")]
//     public class JsonFileController : ControllerBase
//     {
//         [HttpGet]
//         public async Task<IActionResult> Get()
//         {
//             var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
//             if (!System.IO.File.Exists(filePath))
//             {
//                 return NotFound("JSON file not found");
//             }
//
//             var jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
//             return Content(jsonContent, "application/json");
//         }
//     }
// }