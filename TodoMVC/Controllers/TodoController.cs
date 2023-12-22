using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TodoMVC.Data;
using TodoMVC.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoDBContext _context;

        public TodoController(TodoDBContext context) => _context = context;

        // GET: api/<TodoController>
        [HttpGet("/notes")]
        public async Task<ActionResult<IEnumerable<Notes>>> Get(bool? completed)
        {
            var todos = await _context.Notes.Select(n => n).ToListAsync();

            if (completed.HasValue)
            {
                todos = todos.Where(t => t.isDone == completed.Value).ToList();
            }

            return Ok(todos);
        }

        [HttpGet("/remaining")]
        public async Task<int> GetRemainingInt()
        {
            int remaining = await _context.Notes.Where(t => t.isDone == false).CountAsync();
            return remaining;
        }

        // PUT api/<TodoController>
        [HttpPut("/notes/{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] Notes model)
        {
            var todo = await _context.Notes.FindAsync(id);

            todo.text = model.text;
            todo.isDone = model.isDone;

            _context.Notes.Update(todo);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST api/<TodoController>
        [HttpPost("/notes")]
        public async Task Post([FromBody] Notes model)
        {
            var todo = new Notes { text = model.text, isDone = model.isDone };

            _context.Notes.Add(todo);
            await _context.SaveChangesAsync();
        }

        [HttpPost("/toggle-all")]
        public async Task ToggleAll()
        {
            int allTodos = await _context.Notes.CountAsync();
            var completed = await _context.Notes.Where(t => t.isDone == true).ToListAsync();
            var notCompleted = await _context.Notes.Where(t => t.isDone == false).ToListAsync();

            if(completed.Count == allTodos)
            {
                foreach (var todo in completed)
                {
                    todo.isDone = false;
                    _context.Notes.Update(todo);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                foreach (var todo in notCompleted)
                {
                    todo.isDone = true;
                    _context.Notes.Update(todo);
                    await _context.SaveChangesAsync();
                }
            }

            foreach (var todo in notCompleted)
            { 
                todo.isDone = true;
                _context.Notes.Update(todo);
                await _context.SaveChangesAsync();
            }
        }

        [HttpPost("/clear-completed")]
        public async Task ClearCompleted()
        {
            var completed = await _context.Notes.Where(t => t.isDone == true).ToListAsync();

            foreach (var todo in completed)
            {
                _context.Notes.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("/notes/{id}")]
        public async Task Delete(int id)
        {
            var todo = await _context.Notes.FindAsync(id);

            _context.Notes.Remove(todo);
            await _context.SaveChangesAsync();
        }
    }
}
