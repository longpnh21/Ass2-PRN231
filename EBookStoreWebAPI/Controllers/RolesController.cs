﻿using BusinessObject;
using DataAccess;
using DataAccess.Repositories.Interfaces;
using EBookStoreWebAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace EBookStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Admin")]
    public class RolesController : ODataController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRoleRepository _roleRepository;

        public RolesController(ApplicationDbContext dbContext, IRoleRepository roleRepository)
        {
            _dbContext = dbContext;
            _roleRepository = roleRepository;
            dbContext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
        }

        [EnableQuery(PageSize = 5)]
        public IActionResult Get()
        {
            return Ok(_roleRepository.GetRoles(_dbContext));
        }

        [EnableQuery]
        public IActionResult Get([FromODataUri] int key, string version)
        {
            return Ok(_roleRepository.FindRoleById(_dbContext, key));
        }

        [EnableQuery]
        public IActionResult Post([FromBody] Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _roleRepository.SaveRole(_dbContext, role);
            return Created(role);
        }

        [EnableQuery]
        public IActionResult Put([FromODataUri] int key, [FromBody] Role role)
        {
            var existedRole = _roleRepository.FindRoleById(_dbContext, key);
            if (existedRole == null)
            {
                return NotFound();
            }

            _roleRepository.UpdateRole(_dbContext, role);
            return Ok();
        }

        [EnableQuery]
        public IActionResult Delete([FromODataUri] int key)
        {
            var role = _roleRepository.FindRoleById(_dbContext, key);
            if (role == null)
            {
                return NotFound();
            }
            _roleRepository.DeleteRole(_dbContext, role);
            return Ok();
        }
    }
}

