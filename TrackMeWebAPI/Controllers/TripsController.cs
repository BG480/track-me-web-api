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
    public class TripsController : ControllerBase
    {
        private readonly DatabaseContext databaseContext;

        public TripsController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        // GET api/trips
        [HttpGet]
        [Authorize(Roles="BasicUser")]
        public async Task<ActionResult<IEnumerable<TripViewModel>>> GetTrips()
        {
            var applicationUserID = User.Claims.First(x => x.Type == "ApplicationUserID").Value;
            var basicUserID = this.databaseContext.BasicUsers.SingleOrDefault(x => x.ApplicationUserID.Equals(applicationUserID)).ID;

            return await this.databaseContext.Trips
                .Where(x => x.BasicUserID == basicUserID)
                .Select(x => new TripViewModel
                {
                    ID = x.ID,
                    Name = x.Name

                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripViewModel>>> GetAllTrips()
        {
            return await this.databaseContext.Trips
                .Select(x => new TripViewModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    BasicUserEmail = this.databaseContext.BasicUsers.SingleOrDefault(y => y.ID == x.BasicUserID).Email
                }).ToListAsync();

        }


    }
}