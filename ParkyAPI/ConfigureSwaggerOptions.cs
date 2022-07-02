using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.IO;
using System;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace ParkyAPI
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var desc in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    desc.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"My Project {desc.ApiVersion}",
                        Version = desc.ApiVersion.ToString()
                    });

            }


            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
               "JWT Заголовок авторизації за схемою Bearer. \r\n\r\n " +
               "Введіть 'Bearer' [пробіл], а потім свій токен у текстовому полі нижче.\r\n\r\n" +
               "Приклад: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer" //тип автентифікації
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });


            var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
            options.IncludeXmlComments(cmlCommentsFullPath);
        }
    }
}
