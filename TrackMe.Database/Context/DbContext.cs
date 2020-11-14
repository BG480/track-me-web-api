﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackMe.Domain.Entities;
using TrackMeWebAPI.Models;

namespace TrackMeWebAPI.DAL
{
    public class DbContext : IdentityDbContext<ApplicationUserIdentity>
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<BasicUser> BasicUsers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<SensorData> SensorData { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
            .ToTable("ApplicationUsers")
            .HasDiscriminator<string>("ApplicationUserType")
            .HasValue<Admin>(ApplicationUserRoles.Admin.ToString())
            .HasValue<BasicUser>(ApplicationUserRoles.BasicUser.ToString());
        }
    }
}
