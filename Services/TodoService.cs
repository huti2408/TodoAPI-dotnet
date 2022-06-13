using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class TodoService
    {
        private readonly TodoContext _todoContext;
        public TodoService(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        public async Task<IActionResult> GetAll()
        {
            var todos = await _todoContext.TodoItems.ToListAsync();
            return new OkObjectResult(todos);
        }
        public async Task<IActionResult> GetById(long id)
        {
            var todo = await _todoContext.TodoItems.FindAsync(id);
            return new OkObjectResult(todo);
        }
        public async Task<ActionResult> CreateTodo(TodoItem todo, HttpContext context)
        {

            if (todo == null)
            {
                return new BadRequestResult();
            }

            User currentUser = (User)context.Items["User"];
            todo.UserID = currentUser.UserID;
            await _todoContext.TodoItems.AddAsync(todo);
            await _todoContext.SaveChangesAsync();
            return new CreatedAtActionResult("GetById", "TodoItems", new { id = todo.TodoItemId }, todo);
        }
        public async Task<ActionResult> UpdateById(long id, TodoItem todo)
        {
            var item = await _todoContext.TodoItems.FindAsync(id);
            if (item == null)
            {
                return new NotFoundResult();
            }
            else
            {
                _todoContext.Entry(item).State = EntityState.Modified;
                await _todoContext.SaveChangesAsync();
                return new OkObjectResult(new { message = "Update Todo Item successfully", todo });
            }

        }
        public async Task<ActionResult> DeleteById(long id)
        {
            var todoItem = _todoContext.TodoItems.Find(id);
            _todoContext.TodoItems.Remove(todoItem);
            await _todoContext.SaveChangesAsync();
            return new OkObjectResult(new { message = "Delete successful" });
        }
    }
}
