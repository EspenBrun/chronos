using System.Collections.Generic;
using System.Threading.Tasks;
using Chronos.Enums;
using Chronos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Controllers
{
    // The [controller] token in the route is replaced by the name of the
    // controller (omitting the Controller suffix), so api/todoitems
    [Route("api/[controller]")]
    // Complex type parameters are automatically bound using the request body.
    // Consequently, the preceding action's product parameter isn't explicitly
    // annotated with the [FromBody] attribute.
    [ApiController]
    public class TodoItemsController : Controller
    {
        private readonly TodoContext Context;

        public TodoItemsController(TodoContext context)
        {
            Context = context;
        }

        [HttpGet] // GET api/todoitems
        // Method name does not matter, but each method must correspond to unique VERB + path
        // Nice conventions is just to use the verbs as method name
        // So this should have been Get()
        public async Task<ActionResult<IEnumerable<TodoItem>>> List()
        {
            // Creates an OkObjectResult object that produces an Http.StatusCodes.Status response.
            return Ok(await Context.ToDoItems.ToListAsync());
        }

        [HttpPost] // POST api/todoitems
        public async Task<ActionResult<TodoItem>> Post(TodoItem toDoItem)
        {
            // Checking ModelState.IsValid performs model validation, and
            // should be done in every API method that accepts user input.
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-2.2
            if (toDoItem == null || !ModelState.IsValid)
                return BadRequest(ErrorCodes.TodoItemNameAndNotesRequired.ToString());

            var existing = await Context.ToDoItems.FindAsync(toDoItem.Id);
            if (existing != null)
                return StatusCode(StatusCodes.Status409Conflict);

            // Write to db
            Context.ToDoItems.Add(toDoItem);
            // Commit
            await Context.SaveChangesAsync();

            // Returns a 201 response. HTTP 201 is the standard response for an HTTP POST method that creates a new
            // resource on the server.
            // Adds a Location header to the response. The Location header specifies the URI of the newly created to-do
            // item. For more information, see https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
            // Uses the "GetById" named route to create the URL. The "GetById" named route is defined in GetById()
            return CreatedAtAction(nameof(GetById), new { id = toDoItem.Id }, toDoItem);
        }

        [HttpGet("{id}")] // GET api/todoitems/
        // for e.g. Swagger documentation.
        // Here Type is redundant since type is specified in ActionResult<T>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TodoItem))]
        // for e.g. Swagger documentation
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem>> GetById(string id)
        {
            var toDoItem = await Context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
                return BadRequest();

            return Ok(toDoItem);
        }

        [HttpPut("{id}")] // PUT api/todoitems/{id}
        public async Task<ActionResult> Put(string id, TodoItem item)
        {
            if (id != item.Id)
                return BadRequest(ErrorCodes.IdsNotMatching.ToString());

            Context.Entry(item).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")] // DELETE api/todoitem/{id}
        public async Task<ActionResult<TodoItem>> Delete(string id)
        {
            var toDoItem = await Context.ToDoItems.FindAsync(id);

            if (toDoItem == null)
                return NotFound(ErrorCodes.RecordNotFound.ToString());

            Context.ToDoItems.Remove(toDoItem);
            await Context.SaveChangesAsync();

            return toDoItem;
        }
    }
}