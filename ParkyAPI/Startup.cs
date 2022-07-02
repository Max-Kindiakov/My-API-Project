using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ParkyAPI.Data;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ParkyAPI.ParkyMapper;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ParkyAPI
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
            services.AddCors();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //створюю екземпляр об'єкту для всьогго запросу(можу дістати репозиторій у будь якому запиті)
            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddScoped<IUserRepository, UserRepository>();//дає доступ для контролера
            services.AddAutoMapper(typeof(ParkyMappings));
            services.AddApiVersioning(options =>
            {
             options.AssumeDefaultVersionWhenUnspecified=true;//якщо я не визначу свою версію,то загрузиться базова версія
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions=true; //яку апі зараз показує
            });
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>(); 
            services.AddSwaggerGen();
            var appSettingsSection = Configuration.GetSection("AppSettings");

            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });



            


            
            
            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(options => { //apiversioning
                foreach (var desc in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", //ParkyOpenApiSpec
                        desc.GroupName.ToUpperInvariant()/*name ParkyApi*/ );
                options.RoutePrefix = ""; 
            });
            //app.UseSwaggerUI(options => {
            //    options.SwaggerEndpoint("/swagger/ParkyOpenApiSpec/swagger.json", "ParkyApi");
            //    //options.SwaggerEndpoint("/swagger/ParkyOpenApiSpecTrails/swagger.json", "ParkyApi Trails");
            //    options.RoutePrefix = "";
            //});


            app.UseRouting();
            app.UseCors(x => x
              .AllowAnyOrigin() //дозволяє запити CORS з усіх джерел із будь-якою схемою (http або https)
              .AllowAnyMethod() //Дозволяє будь-який метод HTTP
              .AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
