﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrackMeWebAPI.DAL;
using TrackMeWebAPI.Data;
using TrackMeWebAPI.Models;
using TrackMeWebAPI.Services.Interfaces;
using TrackMeWebAPI.Services.Logic;

namespace TrackMeWebAPI
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
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddDbContext<DatabaseContext>(op => op.UseSqlServer(Configuration["ConnectionString:DBConnection"]));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["ApplicationSettings:Audience"],
                    ValidIssuer = Configuration["ApplicationSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:AuthSigningKey"]))
                };
            });

            services.AddCors();
            services.AddTransient<ITripsService, TripsService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAdminsService, AdminsService>();
            services.AddTransient<IBasicUsersService, BasicUsersService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                using (var scope = app.ApplicationServices.GetRequiredService<IServiceProvider>().CreateScope())
                {
                    DataSeeder.CreateRoles(scope.ServiceProvider);
                    DataSeeder.CreateAdmins(scope.ServiceProvider);
                    DataSeeder.CreateBasicUsers(scope.ServiceProvider);
                    DataSeeder.CreateTrips(scope.ServiceProvider);
                    DataSeeder.CreateSensorsValues(scope.ServiceProvider);
                }

            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(builder =>
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());


            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
            
        }
    }
}