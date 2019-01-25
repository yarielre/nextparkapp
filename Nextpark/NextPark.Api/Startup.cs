using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NextPark.Data;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.MapperTools;
using NextPark.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace NextPark.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });
            var mapper = config.CreateMapper();

            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            #region CORS
            services.AddCors(corsPolicy =>
            {
                corsPolicy.AddPolicy("insideCorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            #endregion

            services.AddLogging();

            #region Token Bearer configuration
            // secretKey contains a secret passphrase only your server knows
            var secretKey = Configuration["JwtSecretKey"];
            var issuer = Configuration["JwtIssuer"];
            var audience = Configuration["JwtAudience"];
            var jwtExpiration = Configuration["JwtExpireDays"];

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = false,
                ValidIssuer = issuer,
               
                // Validate the JWT Audience (aud) claim
                ValidateAudience = false,
                ValidAudience = audience
            };

            services.AddAuthentication(optSchema =>
            {
                optSchema.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                optSchema.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                optSchema.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            });
            #endregion


            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddSingleton(mapper);
            services.AddScoped(typeof(IEmailSender), typeof(EmailSender));
            services.AddScoped(typeof(IPushNotificationService), typeof(PushNotificationService));
            services.AddScoped(typeof(IDbFactory), typeof(DbFactory));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(op => op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            #region Swagger Doc Generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "NextPark Web API",
                    Description = "NextPark Web API Services",
                    TermsOfService = "None",
                    Contact = new Contact()
                    {
                        Name = "NextPark Web API",
                        Email = "info@nextpark.ch",
                        Url = "www.nextpark.ch"
                    }
                });
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseCors("insideCorsPolicy");
            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NextPark Web  API V1");
            });
        }
    }
}
