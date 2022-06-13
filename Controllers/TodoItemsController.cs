using Microsoft.AspNetCore.Mvc;
using TodoAPI.Helpers;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoService _todoService;
        private HttpContext context;

        public TodoItemsController(TodoService todoService, HttpContext context)
        {
            _todoService = todoService;
            this.context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            try
            {
                var json = await _todoService.GetAll();
                return json;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(new { message = "Some thing was wrong when getting users" });
            }
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(long id)
        {
            try
            {
                var todoItem = await _todoService.GetById(id);

                if (todoItem == null)
                {
                    return NotFound();
                }

                return todoItem;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(new { message = "Some thing was wrong when getting user" });
            }
        }

        //    // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            try
            {
                var json = await _todoService.UpdateById(id, todoItem);
                return json;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(new { message = "Please try again" });
            }
        }

        //    // POST: api/TodoItems
        //    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostTodoItem([Bind("TodoItemId,Name,IsComplete")] TodoItem todoItem)
        {
            try
            {
                return await _todoService.CreateTodo(todoItem, context);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(new { message = "Please try again" });

            }

        }

        //    // DELETE: api/TodoItems/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            try
            {
                var json = await _todoService.DeleteById(id);
                return json;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(new { message = "Please try again" });

            }
        }


        //    private bool TodoItemExists(long id)
        //    {
        //        return (_context.TodoItems?.Any(e => e.TodoItemId == id)).GetValueOrDefault();
        //    }
    }
}
