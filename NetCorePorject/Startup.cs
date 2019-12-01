using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Consul;

namespace NetCorePorject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.CreateLogger("Startup").LogWarning(this.Configuration["Port"]);
            int port = int.Parse(this.Configuration["Port"]?? "44308");
            #region consul注册 运行且只运行一次
            ConsulClient client = new ConsulClient(obj =>
            {
                obj.Address = new Uri("http://localhost:8500/");
                obj.Datacenter = "dc1";
            });//找到consul的实例
            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "TestService" + Guid.NewGuid(), //唯一的
                Name = "Group1",//分组
                Address = "127.0.0.1",
                Port = port,//同一套代码，但是又是不同的端口,怎么办
                Tags = new string[] { },//标签--可以设置信息，服务发现时，可以获取到的--负载均衡策略扩展的
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                    Interval = TimeSpan.FromSeconds(15),
                    Timeout = TimeSpan.FromSeconds(5),
                    HTTP = $"http://127.0.0.1:{port}/api/Health"
                }
            }) ;//向实例注册
            #endregion

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
