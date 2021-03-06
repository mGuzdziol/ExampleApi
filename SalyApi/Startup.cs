using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ExampleApi.Repository;
using ExampleApi.Service;

namespace ExampleApi
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
      services.AddScoped<IJWTAuthManager, JWTAuthManager>();
      services.AddScoped<IOrders, Orders>();

      services.AddSingleton(Configuration);

      services.AddAuthentication(options =>
      {
        // Identity made Cookie authentication the default.
        // However, we want JWT Bearer Auth to be the default.
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
            .AddJwtBearer(options =>
            {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["JwtAuth:Issuer"],
                ValidAudience = Configuration["JwtAuth:Issuer"],
                RequireSignedTokens = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtAuth:Key"]))
              };
             
            });

      services.AddControllers();
      //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
      //services.AddRazorPages();

      //services.AddMvc(options => options.EnableEndpointRouting = false);
      //services.AddAuthorization();

      services.AddMvcCore(options=>options.EnableEndpointRouting = false).AddAuthorization();

      services.AddControllers(options => options.OutputFormatters.RemoveType<StringOutputFormatter>());

      services.ConfigureAll<HttpClientFactoryOptions>(options =>
      {
        options.HttpMessageHandlerBuilderActions.Add(builder =>
        {
          builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<LogRequestAndResponseHandler>());
        });
      });


      // Register Swagger  
      services.AddSwaggerGen(c =>
      {
        var filePath = Path.Combine(System.AppContext.BaseDirectory, "ExampleApi.xml");
        c.IncludeXmlComments(filePath);

        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Test API", Version = "v1.0" });


        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                      In = ParameterLocation.Header
                   },
                   new string[] {}
           }
       });

      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddLog4Net();
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      if (env.IsEnvironment("Development"))
      {
        app.UseDeveloperExceptionPage();

      }

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();

        // Enable middleware to serve generated Swagger as a JSON endpoint.  
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),  
        // specifying the Swagger JSON endpoint.  
        app.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API V1");
        });
      }
      else
      {
        app.UseHsts();
      }
      
      app.UseAuthentication();
      app.UseAuthorization();
  
      app.UseMvc();
      app.UseHttpsRedirection();
    }
  }
}
