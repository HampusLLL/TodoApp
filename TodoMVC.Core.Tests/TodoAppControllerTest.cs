using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoMVC.Controllers;
using TodoMVC.Data;
using TodoMVC.Models;

namespace TodoMVC.Core.Tests
{
    public class TodoAppControllerTest
    {
        private TodoDBContext _context;

        private TodoDBContext CreateInitializedContext()
        {
            var options = new DbContextOptionsBuilder<TodoDBContext>()
                .UseInMemoryDatabase(databaseName: GetRandomDatabaseName())
                .Options;

            var context = new TodoDBContext(options);

            return context;
        }

        // Helper method to generate a random database name using Guid
        private string GetRandomDatabaseName()
        {
            return $"TestDatabase_{Guid.NewGuid()}";
        }

        [Fact]
        public async Task Add_Todos_Reassure_Data_Is_Correct_Api_Test()
        {
            // Arrange
            var _context = CreateInitializedContext();
            _context.Notes.Add(new Notes { ID = 1, text = "Test", isDone = false });
            _context.Notes.Add(new Notes { ID = 2, text = "Another Test", isDone = true });
            _context.Notes.Add(new Notes { ID = 3, text = "Yet Another Test", isDone = false });
            _context.SaveChanges();

            var controller = new TodoController(_context);

            // Act
            var result = await controller.Get(null);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var notes = Assert.IsType<List<Notes>>(okResult.Value);

            Assert.Contains(notes, n => n.ID == 1 && n.text == "Test" && n.isDone == false);
            Assert.Contains(notes, n => n.ID == 2 && n.text == "Another Test" && n.isDone == true);
            Assert.Contains(notes, n => n.ID == 3 && n.text == "Yet Another Test" && n.isDone == false);
            Assert.Equal(notes.Count, 3);
        }

        [Fact]
        public async Task Remove_Todos_Reassure_Data_Is_Removed_Correctly_Api_Test()
        {
            // Arrange
            var _context = CreateInitializedContext();
            var newNote = new Notes { ID = 1, text = "Test", isDone = false };
            _context.Notes.Add(newNote);
            _context.Notes.Add(new Notes { ID = 2, text = "Another Test", isDone = true });
            _context.Notes.Add(new Notes { ID = 3, text = "Yet Another Test", isDone = false });
            _context.SaveChanges();

            var controller = new TodoController(_context);

            // Act
            await controller.Delete(1);

            // Assert
            var remainingNotes = _context.Notes.ToList();

            Assert.DoesNotContain(remainingNotes, n => n.ID == 1 && n.text == "Test" && n.isDone == false);
            Assert.Contains(remainingNotes, n => n.ID == 2 && n.text == "Another Test" && n.isDone == true);
            Assert.Contains(remainingNotes, n => n.ID == 3 && n.text == "Yet Another Test" && n.isDone == false);
            Assert.Equal(2, remainingNotes.Count);
        }

        [Fact]
        public async Task Clear_Completed_Todos_Reassure_Data_Is_Removed_Correctly_Api_Test()
        {
            // Arrange
            var _context = CreateInitializedContext();
            _context.Notes.Add(new Notes { ID = 1, text = "Test", isDone = false });
            _context.Notes.Add(new Notes { ID = 2, text = "Another Test", isDone = true });
            _context.Notes.Add(new Notes { ID = 3, text = "Yet Another Test", isDone = true });
            _context.SaveChanges();

            var controller = new TodoController(_context);

            // Act
            await controller.ClearCompleted();

            // Assert
            var remainingNotes = _context.Notes.ToList();

            Assert.Contains(remainingNotes, n => n.ID == 1 && n.text == "Test" && n.isDone == false);
            Assert.Single(remainingNotes);
        }
    }
}
