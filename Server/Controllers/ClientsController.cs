﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using CounterCulture.Constants;
using CounterCulture.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CounterCulture.Models;
using IdentityServer4.Models;
using System.Linq;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace CounterCulture.Controllers
{

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientsController : BaseController
    {
        public ClientsController(
            ILogger<ClientsController> LoggerService,
            ConfigurationDbContext _is4Context)
        {
            Logger = LoggerService;
            is4Context = _is4Context;
        }
        private ILogger<ClientsController> Logger { get; set; }

        private ConfigurationDbContext is4Context { get; set; }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public ActionResult Register([FromBody] Registration registration) {

            // TODO: simplify using Automapper
            Client client = new Client();
            client.ClientId = String.Empty.NewClientId();
            client.ClientName = registration.ClientName;
            client.AllowedScopes = registration.AllowedScopes;
            client.RedirectUris = registration.RedirectUris;
            client.ClientSecrets = new List<Secret> {
                new Secret(String.Empty.NewClientSecret())
            };
            
            this.is4Context.Clients.Add(client.ToEntity());

            this.is4Context.SaveChanges();

            Logger.LogInformation(LoggingEvents.RegisterApp, client.ClientName);

            return Ok(client);
        }

    }
}