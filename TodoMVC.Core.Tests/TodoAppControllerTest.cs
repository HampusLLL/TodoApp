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

        // Common setup method to initialize the context
        private TodoDBContext CreateInitializedContext()
        {
            var options = new DbContextOptionsBuilder<TodoDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var context = new TodoDBContext(options);

            return context;
        }
        [Fact]
        public async Task Add_Notes_Reassure_Data_Is_Correct_Api_Test()
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
        }
    }
}
