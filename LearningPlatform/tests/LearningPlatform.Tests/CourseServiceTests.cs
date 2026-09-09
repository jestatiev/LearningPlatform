using LearningPlatform.Application.Services;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Repositories;
using LearningPlatform.Domain.Entities;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LearningPlatform.Tests
{
    public class CourseServiceTests
    {
        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryRemoveAndSaveChanges_WhenCourseExists()
        {
            // Arrange
            var mockRepo = new Mock<ICourseRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var existingCourse = new Course { Id = 1, Title = "Test Tecaj" };

            mockRepo.Setup(r => r.GetByIdAsync(1))
                    .ReturnsAsync(existingCourse);

            mockUnitOfWork.Setup(u => u.Courses)
                         .Returns(mockRepo.Object);

            mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                         .ReturnsAsync(1);

            var courseService = new CourseService(mockUnitOfWork.Object);

            // Act
            await courseService.DeleteAsync(1);

            // Assert
            mockRepo.Verify(r => r.Remove(existingCourse), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}