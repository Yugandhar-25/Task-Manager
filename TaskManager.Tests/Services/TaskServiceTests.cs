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
using Task_Manager_API.Repositories;
using Moq;
using Microsoft.AspNetCore.Http.Features;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _repositoryMock;
        private readonly TaskService _sut;  //System Under test

        public TaskServiceTests()
        {
            //var options = new DbContextOptionsBuilder<TaskDbContext>().
            //    UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _repositoryMock = new Mock<ITaskRepository>();
            _sut = new TaskService(_repositoryMock.Object, new NullLogger<TaskService>());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTasks()
        {
            //Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Title = "Task 1", IsCompleted = false },
                new TaskItem { Title = "Task 2", IsCompleted = true }
            };
            var pagedResult = new PagedResult<TaskItem>
            {
                Items = tasks,
                TotalCount = 2,
                Page = 1,
                PageSize = 10
            };
            var pagination = new PaginationParams { Page = 1, PageSize = 10 };
            _repositoryMock.Setup(r => r.GetAllAsync(pagination)).ReturnsAsync(pagedResult);

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
            var task = new TaskItem { Id = 1, Title = "Task 1", IsCompleted = false };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

            var result = await _sut.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.title.Should().Be("Task 1");
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskDoesNotExists()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);
            var result = await _sut.GetByIdAsync(999);
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldCreateTask()
        {
            var dto = new CreateTaskDTO("Task 1");
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r=>r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _sut.CreateTaskAsync(dto);

            result.Should().NotBeNull();
            result.title.Should().Be("Task 1");
            result.isCompleted.Should().BeFalse();
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny <TaskItem>()), Times.Once);
            _repositoryMock.Verify(r=>r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_WhenTaskExists()
        {
            var task = new TaskItem { Id=1, Title = "Task 1", IsCompleted = false };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
            _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _sut.DeleteAsync(task.Id);

            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<TaskItem>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_WhenTaskDoesNotExists()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);
            var result = await _sut.DeleteAsync(999);

            result.Should().BeFalse();
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<TaskItem>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateTaskAsync_WhenTaskExists()
        {
            var task = new TaskItem { Id=1, Title = "Task 1", IsCompleted = false };
            var dto = new UpdateTaskDTO("New Title", true);
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            
            var result = await _sut.UpdateTaskAsync(task.Id, dto);

            result.Should().NotBeNull();
            result.title.Should().Be("New Title");
            result.isCompleted.Should().BeTrue();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectPage()
        {
            var tasks = Enumerable.Range(6, 5)
            .Select(i => new TaskItem { Id = i, Title = $"Task {i}", IsCompleted = false })
            .ToList();

            var pagedResult = new PagedResult<TaskItem>
            {
                Items = tasks,
                TotalCount = 15,
                Page = 2,
                PageSize = 5,
            };

            var pagination = new PaginationParams { Page = 2, PageSize = 5 };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<PaginationParams>())).ReturnsAsync(pagedResult);

            var result = await _sut.GetAllAsync(pagination);

            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(15);
            result.TotalPages.Should().Be(3);
            result.HasNext.Should().BeTrue();
            result.HasPrevious.Should().BeTrue();
        }
    }
}
