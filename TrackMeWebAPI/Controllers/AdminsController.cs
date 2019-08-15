﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackMeWebAPI.DAL;
using TrackMeWebAPI.ViewModels;

namespace TrackMeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminsController : ControllerBase
    {
        private readonly DatabaseContext databaseContext;

        public AdminsController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        // GET api/admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminViewModel>>> GetAdmins()
        {
            return await this.databaseContext.Admins
                .Select(x => new AdminViewModel
                {
                    ID = x.ID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email
                }).ToListAsync();

        }

        // GET api/admins/4
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminViewModel>> GetAdminDetails(int id)
        {
            var admin = await this.databaseContext.Admins.FindAsync(id);

            return new AdminViewModel
            {
                ID = admin.ID,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email
            };
        }

    }
}