using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjetoAdvenTec.Controllers.SistemaAutomatizado;
using ProjetoAdvenTec.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdvenTec
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
            //Configura��es basicas para o uso hangfire(automatizar a��es/rotinas) - *Pacote Nulget*
            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());

            //Configura��o de acesso ao banco de dados (String de conex�o no arquivo 'appsettings.json')
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DataContextSQL")));

            //AddScoped para que a 'automazi��o' seja feita em cada requisi��o ao sistema com dados atualizados'
            services.AddScoped<IAcoesSistema, AcoesSistema>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobClient,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=UsuarioAcesso}/{action=Logar}/{id?}");
            });

            //app.UseHangfireDashboard();

            //Configura��es para uso ativo do Hangfire
            app.UseHangfireServer();

            backgroundJobClient.Enqueue(() => Console.WriteLine("Sistema de Automatiza��o Ativado!"));

            //Configura��e para que a a��o seja realizada em um horario pre determinado do dia.
            RecurringJob.AddOrUpdate(
                "Run Every 00:00",
                () => serviceProvider.GetService<IAcoesSistema>().VerificarNovasAvalicoes(),
                //Cron.Daily(02, 59), TimeZoneInfo.Utc 
                Cron.Daily(00, 00), TimeZoneInfo.Local // Usando o hor�rio local do PC
                );
            RecurringJob.AddOrUpdate(
                "Run Every 00:01",
                () => serviceProvider.GetService<IAcoesSistema>().VerificarAvalicoesProgresso(),
               Cron.Daily(00,00 ), TimeZoneInfo.Local // Usando o hor�rio local do PC
                );
        }
    }
}
