using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCorePorject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private ILoggerFactory _LoggerFactory = null;

        public HealthController(ILoggerFactory loggerFactory)
        {
            this._LoggerFactory = loggerFactory;
        }
        public IActionResult Check()
        {
            this._LoggerFactory.CreateLogger(typeof(HealthController)).LogWarning($"{DateTime.Now.ToString()}This Health Check Being Invoked!");
            return Ok();
        }
    }
}