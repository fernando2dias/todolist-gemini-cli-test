using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests
{
    public class TodoControllerTests
    {
        private TodoContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new TodoContext(options);
        }

        // GET: api/Todo
        [Fact]
        public async Task GetTodoItems_ReturnsListOfItems()
        {
            var context = GetInMemoryContext();
            context.TodoItems.Add(new TodoItem { Name = "Test 1" });
            context.TodoItems.Add(new TodoItem { Name = "Test 2" });
            await context.SaveChangesAsync();

            var controller = new TodoController(context);
            var result = await controller.GetTodoItems();

            Assert.Equal(2, result.Value.Count());
        }

        // GET: api/Todo/5 (Positive)
        [Fact]
        public async Task GetTodoItem_ReturnsItem_WhenItemExists()
        {
            var context = GetInMemoryContext();
            var item = new TodoItem { Id = 1, Name = "Test" };
            context.TodoItems.Add(item);
            await context.SaveChangesAsync();

            var controller = new TodoController(context);
            var result = await controller.GetTodoItem(1);

            Assert.Equal("Test", result.Value.Name);
        }

        // GET: api/Todo/5 (Negative)
        [Fact]
        public async Task GetTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            var context = GetInMemoryContext();
            var controller = new TodoController(context);

            var result = await controller.GetTodoItem(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // POST: api/Todo
        [Fact]
        public async Task PostTodoItem_CreatesItem()
        {
            var context = GetInMemoryContext();
            var controller = new TodoController(context);
            var item = new TodoItem { Name = "New Item" };

            var result = await controller.PostTodoItem(item);
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdItem = Assert.IsType<TodoItem>(actionResult.Value);

            Assert.Equal("New Item", createdItem.Name);
            Assert.Single(context.TodoItems);
        }

        // PUT: api/Todo/5 (Positive)
        [Fact]
        public async Task PutTodoItem_UpdatesItem_WhenItemExists()
        {
            var context = GetInMemoryContext();
            var item = new TodoItem { Id = 1, Name = "Old Name" };
            context.TodoItems.Add(item);
            await context.SaveChangesAsync();

            // Detach the item to avoid tracking conflict in the controller
            context.Entry(item).State = EntityState.Detached;

            var controller = new TodoController(context);
            var updatedItem = new TodoItem { Id = 1, Name = "New Name" };

            var result = await controller.PutTodoItem(1, updatedItem);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal("New Name", context.TodoItems.First().Name);
        }

        // PUT: api/Todo/5 (Negative - ID Mismatch)
        [Fact]
        public async Task PutTodoItem_ReturnsBadRequest_OnIdMismatch()
        {
            var context = GetInMemoryContext();
            var controller = new TodoController(context);
            var updatedItem = new TodoItem { Id = 1, Name = "Name" };

            var result = await controller.PutTodoItem(2, updatedItem);

            Assert.IsType<BadRequestResult>(result);
        }

        // DELETE: api/Todo/5 (Positive)
        [Fact]
        public async Task DeleteTodoItem_RemovesItem()
        {
            var context = GetInMemoryContext();
            var item = new TodoItem { Id = 1, Name = "ToDelete" };
            context.TodoItems.Add(item);
            await context.SaveChangesAsync();

            var controller = new TodoController(context);
            var result = await controller.DeleteTodoItem(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.TodoItems);
        }

        // DELETE: api/Todo/5 (Negative)
        [Fact]
        public async Task DeleteTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            var context = GetInMemoryContext();
            var controller = new TodoController(context);

            var result = await controller.DeleteTodoItem(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
