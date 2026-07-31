using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class AssociationRepositoryTest
    {
        [Fact]
        public void GetAssociations_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnAssociations()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = testRepository.GetAssociations();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Association>();
            }
        }

        [Fact]
        public void GetAssociations_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = testRepository.GetAssociations();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetAssociations_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = testRepository.GetAssociations();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetAssociationsAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnAssociations()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = await testRepository.GetAssociationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Association>();
            }
        }

        [Fact]
        public async Task GetAssociationsAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = await testRepository.GetAssociationsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAssociationsAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = await testRepository.GetAssociationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetAssociation_WhenDbSetIsNeitherNullNorEmptyAndAssociationIsFound_ShouldReturnAssociation()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var id = 1;
            var result = testRepository.GetAssociation(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Association>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetAssociation_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var id = 1;
            var result = testRepository.GetAssociation(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetAssociation_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var id = 1;
            var result = testRepository.GetAssociation(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetAssociation_WhenAssociationIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var id = -1;
            var result = testRepository.GetAssociation(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAssociationAsync_WhenDbSetIsNeitherNullNorEmptyAndAssociationIsFound_ShouldReturnAssociation()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var id = 1;
            var result = await testRepository.GetAssociationAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Association>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetAssociationAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var id = 1;
            var result = await testRepository.GetAssociationAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAssociationAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var id = 1;
            var result = await testRepository.GetAssociationAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAssociationAsync_WhenAssociationIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var id = -1;
            var result = await testRepository.GetAssociationAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetAssociationByShortName_WhenDbSetIsNeitherNullNorEmptyAndAssociationIsFound_ShouldReturnAssociation()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var shortName = "A1";
            var result = testRepository.GetAssociationByShortName(shortName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Association>();
            result.ShortName.ShouldBe(shortName);
        }

        [Fact]
        public void GetAssociationByShortName_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var shortName = "A1";
            var result = testRepository.GetAssociationByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetAssociationByShortName_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var shortName = "A1";
            var result = testRepository.GetAssociationByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetAssociationByShortName_WhenAssociationIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var shortName = "A99";
            var result = testRepository.GetAssociationByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAssociationByShortNameAsync_WhenDbSetIsNeitherNullNorEmptyAndAssociationIsFound_ShouldReturnAssociation()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var shortName = "A1";
            var result = await testRepository.GetAssociationByShortNameAsync(shortName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Association>();
            result.ShortName.ShouldBe(shortName);
        }

        [Fact]
        public async Task GetAssociationByShortNameAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var shortName = "A1";
            var result = await testRepository.GetAssociationByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAssociationByShortNameAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var shortName = "A1";
            var result = await testRepository.GetAssociationByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAssociationByShortNameAsync_WhenAssociationIsNotFound_ShouldReturnNull()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var shortName = "A99";
            var result = await testRepository.GetAssociationByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddAssociation()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Association>>());
            var testRepository = new AssociationRepository(fakeDbContext);

            var association = new Association { Id = 1 };

            // Act
            var result = testRepository.Add(association);

            // Assert
            A.CallTo(() => fakeDbContext.Add(association)).MustHaveHappenedOnceExactly();
            result.ShouldBe(association);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Association>>());
            var testRepository = new AssociationRepository(fakeDbContext);

            Association? association = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(association));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnAssociationWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new AssociationRepository(fakeDbContext);

            var association = new Association { Id = 1 };

            // Act
            var result = testRepository.Add(association);

            // Assert
            A.CallTo(() => fakeDbContext.Add(association)).MustNotHaveHappened();
            result.ShouldBe(association);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddAssociation()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Association>>());
            var testRepository = new AssociationRepository(fakeDbContext);

            var association = new Association { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(association);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(association)).MustHaveHappenedOnceExactly();
            result.ShouldBe(association);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Association>>());
            var testRepository = new AssociationRepository(fakeDbContext);

            Association? association = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(association));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnAssociationWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new AssociationRepository(fakeDbContext);

            var association = new Association { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(association);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(association)).MustNotHaveHappened();
            result.ShouldBe(association);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonYear = 1;
            var firstSeason = new Season { Year = firstSeasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var association = new Association { Id = 1, LongName = "Association 1", ShortName = "A1", FirstSeasonYear = firstSeasonYear };
            fakeDbContext.Associations.Add(association);
            fakeDbContext.SaveChanges();

            var testRepository = new AssociationRepository(fakeDbContext);

            // Act
            testRepository.Update(association);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Associations.FirstOrDefault(l => l.Id == association.Id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act & Assert
            Association? association = null!;
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(association));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnAssociation()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            Association? association = new();
            var updated = testRepository.Update(association);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(association);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnAssociation()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            Association? association = new();
            var updated = testRepository.Update(association);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(association);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedAssociationIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonYear = 1;
            var firstSeason = new Season { Year = firstSeasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var association = new Association { Id = 1, LongName = "Association 1", ShortName = "A1", FirstSeasonYear = firstSeasonYear };
            fakeDbContext.Associations.Add(association);
            fakeDbContext.SaveChanges();

            fakeDbContext.ChangeTracker.Clear(); // <-- simulates a fresh context, like production would have

            var testRepository = new AssociationRepository(fakeDbContext);

            var associationCountBeforeDelete = fakeDbContext.Associations.Count();

            // Act
            var result = testRepository.Delete(association.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Associations.Count().ShouldBe(associationCountBeforeDelete - 1);
            result.ShouldBeOfType<Association>();
            result.Id.ShouldBe(association.Id);
            result.LongName.ShouldBe(association.LongName);
            result.ShortName.ShouldBe(association.ShortName);
            result.FirstSeasonYear.ShouldBe(association.FirstSeasonYear);
            result.LastSeasonYear.ShouldBe(association.LastSeasonYear);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            var association = new Association { Id = 1 };

            // Act
            var result = testRepository.Delete(association.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var association = new Association { Id = 1 };
            var result = testRepository.Delete(association.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedAssociationIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var association = new Association { Id = 1, LongName = "Association 1", ShortName = "A1", FirstSeasonYear = 1920 };
            fakeDbContext.Associations.Add(association);
            fakeDbContext.SaveChanges();

            var testRepository = new AssociationRepository(fakeDbContext);

            var associationAssociationCountBeforeDelete = fakeDbContext.Associations.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Associations.Count().ShouldBe(associationAssociationCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedAssociationIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonYear = 1;
            var firstSeason = new Season { Year = firstSeasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var association = new Association { Id = 1, LongName = "Association 1", ShortName = "A1", FirstSeasonYear = firstSeasonYear };
            fakeDbContext.Associations.Add(association);
            fakeDbContext.SaveChanges();

            fakeDbContext.ChangeTracker.Clear(); // <-- simulates a fresh context, like production would have

            var testRepository = new AssociationRepository(fakeDbContext);

            var associationCountBeforeDelete = fakeDbContext.Associations.Count();

            // Act
            var result = await testRepository.DeleteAsync(association.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Associations.Count().ShouldBe(associationCountBeforeDelete - 1);
            result.ShouldBeOfType<Association>();
            result.Id.ShouldBe(association.Id);
            result.LongName.ShouldBe(association.LongName);
            result.ShortName.ShouldBe(association.ShortName);
            result.FirstSeasonYear.ShouldBe(association.FirstSeasonYear);
            result.LastSeasonYear.ShouldBe(association.LastSeasonYear);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var association = new Association { Id = 1 };
            var result = await testRepository.DeleteAsync(association.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var association = new Association { Id = 1 };
            var result = await testRepository.DeleteAsync(association.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedAssociationIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var association = new Association { Id = 1, LongName = "Association 1", ShortName = "A1", FirstSeasonYear = 1 };
            fakeDbContext.Associations.Add(association);
            fakeDbContext.SaveChanges();

            var testRepository = new AssociationRepository(fakeDbContext);

            var associationAssociationCountBeforeDelete = fakeDbContext.Associations.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Associations.Count().ShouldBe(associationAssociationCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void AssociationExists_WhenDbSetIsNotNullAndSelectedAssociationExists_ShouldReturnTrue()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = testRepository.AssociationExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void AssociationExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = testRepository.AssociationExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void AssociationExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = testRepository.AssociationExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void AssociationExists_WhenSelectedAssociationDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = testRepository.AssociationExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task AssociationExistsAsync_WhenDbSetIsNotNullAndSelectedAssociationExists_ShouldReturnTrue()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = await testRepository.AssociationExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task AssociationExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<Association> associations = null!;
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = await testRepository.AssociationExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task AssociationExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var associations = new List<Association>();
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = await testRepository.AssociationExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task AssociationExistsAsync_WhenSelectedAssociationDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var associations = new List<Association>
            {
                new() { Id = 1, LongName = "Association 1", ShortName = "A1", ParentId = null },
                new() { Id = 2, LongName = "Association 2", ShortName = "A2", ParentId = 1 },
                new() { Id = 3, LongName = "Association 3", ShortName = "A3", ParentId = 1 },
            };
            AssociationRepository testRepository = CreateTestRepository(associations);

            // Act
            var result = await testRepository.AssociationExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private static ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<Association> associations)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Associations = associations;
            return fakeDbContext;
        }

        private static AssociationRepository CreateTestRepository(List<Association> associations)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Associations = A.Fake<DbSet<Association>>();
            DbSet<Association> fakeDbSet = associations is not null ? associations.BuildMockDbSet() : null!;
            A.CallTo(() => fakeDbContext.Associations).Returns(fakeDbSet);

            return new AssociationRepository(fakeDbContext);
        }
    }
}
