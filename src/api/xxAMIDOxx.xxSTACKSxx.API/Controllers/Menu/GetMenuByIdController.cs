﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amido.Stacks.Application.CQRS.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using xxAMIDOxx.xxSTACKSxx.API.Models.Responses;
using Query = xxAMIDOxx.xxSTACKSxx.CQRS.Queries.GetMenuById;

namespace xxAMIDOxx.xxSTACKSxx.API.Controllers
{
    /// <summary>
    /// Menu related operations
    /// </summary>
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiExplorerSettings(GroupName = "Menu")]
    [ApiController]
    public class GetMenuByIdController : ApiControllerBase
    {
        readonly IQueryHandler<Query.GetMenuById, Query.Menu> queryHandler;

        public GetMenuByIdController(IQueryHandler<Query.GetMenuById, Query.Menu> queryHandler)
        {
            this.queryHandler = queryHandler;
        }

        /// <summary>
        /// Get a menu
        /// </summary>
        /// <remarks>By passing the menu id, you can get access to available categories and items in the menu </remarks>
        /// <param name="id">menu id</param>
        /// <response code="200">Menu</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Resource not found</response>
        [HttpGet("/v1/menu/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Menu), 200)]
        public async Task<IActionResult> GetMenu([FromRoute][Required]Guid id)
        {
            // NOTE: Please ensure the API returns the response codes annotated above

            var result = await queryHandler.ExecuteAsync(new Query.GetMenuById() { Id = id });

            if (result == null)
                return NotFound();

            var menu = new Menu
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description,
                Categories = result.Categories.Select(i => new Category()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Items = i.Items.Select(x => new Item()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price,
                        Available = x.Available
                    }).ToList(),
                }).ToList(),
                Enabled = result.Enabled
            };

            return new ObjectResult(menu);
        }
    }
}
