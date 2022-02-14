using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class PublicController : ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public IActionResult Hash(string secretKey, string data)
        {
            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(data))
            {
                return BadRequest();
            }

            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(secretKey));
            var hashResult = hmac.ComputeHash(Encoding.ASCII.GetBytes(data));

            return Ok(Convert.ToBase64String(hashResult));
        }
    }
}