﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TrackMeWebAPI.DAL;
using TrackMeWebAPI.Exceptions;
using TrackMeWebAPI.Models;
using TrackMeWebAPI.Services.Interfaces;
using TrackMeWebAPI.ViewModels;

namespace TrackMeWebAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        // POST api/account/login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            try
            {
                var loggedUserData = await this.accountService.Login(loginViewModel);
                return Ok(loggedUserData);
            }
            catch(UserNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }                 
        }

        // POST api/account/register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            try
            {
                await this.accountService.Register(registerViewModel);
                return Ok();
            }
            catch(DuplicatedUserException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            
        }
        
    }
}