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
            #region consulע�� ������ֻ����һ��
            ConsulClient client = new ConsulClient(obj =>
            {
                obj.Address = new Uri("http://localhost:8500/");
                obj.Datacenter = "dc1";
            });//�ҵ�consul��ʵ��
            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "TestService" + Guid.NewGuid(), //Ψһ��
                Name = "Group1",//����
                Address = "127.0.0.1",
                Port = port,//ͬһ�״��룬�������ǲ�ͬ�Ķ˿�,��ô��
                Tags = new string[] { },//��ǩ--����������Ϣ��������ʱ�����Ի�ȡ����--���ؾ��������չ��
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                    Interval = TimeSpan.FromSeconds(15),
                    Timeout = TimeSpan.FromSeconds(5),
                    HTTP = $"http://127.0.0.1:{port}/api/Health"
                }
            }) ;//��ʵ��ע��
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
