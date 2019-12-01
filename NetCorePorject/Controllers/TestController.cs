using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Consul;

namespace NetCorePorject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private ILoggerFactory _LoggerFactory = null;

        public TestController(ILoggerFactory loggerFactory)
        {
            this._LoggerFactory = loggerFactory;
        }
        /// <summary>
        /// 通过consul来消费服务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            string msg = string.Empty;
            using (ConsulClient client = new ConsulClient(c => c.Address = new Uri("http://localhost:8500/")))
            {
                var services = client.Agent.Services().Result.Response;
                var targetServices = services.Where(s => s.Value.Service.Equals("Group1")).Select(s => s.Value);//找到目前都是监控的服务实例

                //平均调度策略：随机+取余
                var target = services.ElementAt(new Random().Next(1, 10000) % targetServices.Count());
                msg = $"current Api:{target.Value.Address} {target.Value.Port} {target.Value.ID} Group1";

            }

            return new JsonResult(new { 
               Id=1122,
               Name="Z",
               msg
            });
               
        }
    }
}