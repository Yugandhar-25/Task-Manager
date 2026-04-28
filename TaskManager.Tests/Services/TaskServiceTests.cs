using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Task_Manager_API.DTOs;
using Task_Manager_API.Models;
using Task_Manager_API.Pagination;
using Task_Manager_API.Persistence;
using Task_Manager_API.Services;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly TaskDbContext _context;
        private readonly TaskService _sut;  //System Under test

        public TaskServiceTests()
        {
            var options = new DbContextOptionsBuilder<TaskDbContext>().
                UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new TaskDbContext(options);
            _sut = new TaskService(_context, new NullLogger<TaskService>());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTasks()
        {
            //Arrange
            _context.Tasks.AddRange(
                new TaskItem { Title = "Task 1", IsCompleted = false },
                new TaskItem { Title = "Task 2", IsCompleted = true }
                );
            await _context.SaveChangesAsync();

            var pagination = new PaginationParams { Page = 1, PageSize = 10 };

            //Act
            var result = await _sut.GetAllAsync(pagination);

            //Assert
            result.Items.Should().HaveCount(2);
            result.Items.Should().Contain(t => t.title == "Task 1");
            result.Items.Should().Contain(t => t.title == "Task 2");
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(1);
            result.HasNext.Should().BeFalse();
            result.HasPrevious.Should().BeFalse();
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskExists()
        {
            _context.Tasks.Add(new TaskItem { Title = "Task 1", IsCompleted = false });
            await _context.SaveChangesAsync();
            var result = await _sut.GetByIdAsync(1);
            result.Should().NotBeNull();
            result!.title.Should().Be("Task 1");
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskDoesNotExists()
        {
            var result = await _sut.GetByIdAsync(999);
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldCreateTask()
        {
            var dto = new CreateTaskDTO("Task 1");
            var result = await _sut.CreateTaskAsync(dto);
            result.Should().NotBeNull();
            result.title.Should().Be("Task 1");
            result.isCompleted.Should().BeFalse();
            _context.Tasks.Count().Should().Be(1);
        }

        [Fact]
        public async Task DeleteTaskAsync_WhenTaskExists()
        {
            var task = new TaskItem { Title = "Task 1", IsCompleted = false };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            var result = await _sut.DeleteAsync(task.Id);
            result.Should().BeTrue();
            _context.Tasks.Count().Should().Be(0);
        }

        [Fact]
        public async Task DeleteTaskAsync_WhenTaskDoesNotExists()
        {
            var result = await _sut.DeleteAsync(999);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateTaskAsync_WhenTaskExists()
        {
            var task = new TaskItem { Title = "Task 1", IsCompleted = false };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            var dto = new UpdateTaskDTO("Updated Task 1", true);
            var result = await _sut.UpdateTaskAsync(task.Id, dto);
            result.Should().NotBeNull();
            result.title.Should().Be("Updated Task 1");
            result.isCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectPage()
        {
            var tasks = new List<TaskItem>();
            for (int i = 1; i <= 15; i++)
            {
                tasks.Add(new TaskItem { Title = $"Task {i}", IsCompleted = false });
            }
            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();
            var pagination = new PaginationParams { Page = 2, PageSize = 5 };

            var result = await _sut.GetAllAsync(pagination);
            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(15);
            result.TotalPages.Should().Be(3);
            result.HasNext.Should().BeTrue();
            result.HasPrevious.Should().BeTrue();

        }
    }
}
