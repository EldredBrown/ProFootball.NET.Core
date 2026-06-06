using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class ConferenceRepositoryTest
    {
        [Fact]
        public void GetConferences_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnConferences()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetConferences();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Conference>();
            }
        }

        [Fact]
        public void GetConferences_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.GetConferences();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetConferences_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.GetConferences();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetConferencesAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnConferences()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetConferencesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Conference>();
            }
        }

        [Fact]
        public async Task GetConferencesAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.GetConferencesAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetConferencesAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.GetConferencesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetConference_WhenDbSetIsNeitherNullNorEmptyAndConferenceIsFound_ShouldReturnConference()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetConference(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetConference_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetConference(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetConference_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetConference(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetConference_WhenConferenceIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = testRepository.GetConference(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetConferenceAsync_WhenDbSetIsNeitherNullNorEmptyAndConferenceIsFound_ShouldReturnConference()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetConferenceAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetConferenceAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetConferenceAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetConferenceAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetConferenceAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetConferenceAsync_WhenConferenceIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = await testRepository.GetConferenceAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetConferenceByShortName_WhenDbSetIsNeitherNullNorEmptyAndConferenceIsFound_ShouldReturnConference()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var shortName = "C1";

            // Act
            var result = testRepository.GetConferenceByShortName(shortName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShortName.ShouldBe(shortName);
        }

        [Fact]
        public void GetConferenceByShortName_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var shortName = "C1";

            // Act
            var result = testRepository.GetConferenceByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetConferenceByShortName_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var shortName = "C1";

            // Act
            var result = testRepository.GetConferenceByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetConferenceByShortName_WhenConferenceIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var shortName = "C99";

            // Act
            var result = testRepository.GetConferenceByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetConferenceByShortNameAsync_WhenDbSetIsNeitherNullNorEmptyAndConferenceIsFound_ShouldReturnConference()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var shortName = "C1";

            // Act
            var result = await testRepository.GetConferenceByShortNameAsync(shortName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShortName.ShouldBe(shortName);
        }

        [Fact]
        public async Task GetConferenceByShortNameAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var shortName = "C1";

            // Act
            var result = await testRepository.GetConferenceByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetConferenceByShortNameAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var shortName = "C1";

            // Act
            var result = await testRepository.GetConferenceByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetConferenceByShortNameAsync_WhenConferenceIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var shortName = "C99";

            // Act
            var result = await testRepository.GetConferenceByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddConference()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Conference>>());
            var testRepository = new ConferenceRepository(fakeDbContext);

            var conference = new Conference { Id = 1 };

            // Act
            var result = testRepository.Add(conference);

            // Assert
            A.CallTo(() => fakeDbContext.Add(conference)).MustHaveHappenedOnceExactly();
            result.ShouldBe(conference);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Conference>>());
            var testRepository = new ConferenceRepository(fakeDbContext);

            Conference? conference = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(conference));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnConferenceWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new ConferenceRepository(fakeDbContext);

            var conference = new Conference { Id = 1 };

            // Act
            var result = testRepository.Add(conference);

            // Assert
            A.CallTo(() => fakeDbContext.Add(conference)).MustNotHaveHappened();
            result.ShouldBe(conference);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddConference()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Conference>>());
            var testRepository = new ConferenceRepository(fakeDbContext);

            var conference = new Conference { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(conference);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(conference)).MustHaveHappenedOnceExactly();
            result.ShouldBe(conference);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Conference>>());
            var testRepository = new ConferenceRepository(fakeDbContext);

            Conference? conference = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(conference));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnConferenceWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new ConferenceRepository(fakeDbContext);

            var conference = new Conference { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(conference);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(conference)).MustNotHaveHappened();
            result.ShouldBe(conference);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var conference = new Conference
            {
                Id = 1,
                LongName = "Conference 1",
                ShortName = "C1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            testRepository.Update(conference);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Conferences.FirstOrDefault(c => c.Id == conference.Id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            Conference? conference = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(conference));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnConference()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            Conference? conference = new Conference { };

            // Act
            var updated = testRepository.Update(conference);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(conference);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnConference()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            Conference? conference = new Conference { };

            // Act
            var updated = testRepository.Update(conference);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(conference);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedConferenceIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var conference = new Conference
            {
                Id = 1,
                LongName = "Conference 1",
                ShortName = "C1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var testRepository = new ConferenceRepository(fakeDbContext);

            var conferenceCountBeforeDelete = fakeDbContext.Conferences.Count();

            // Act
            var result = testRepository.Delete(conference.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Conferences.Count().ShouldBe(conferenceCountBeforeDelete - 1);
            result.ShouldBe(conference);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var conference = new Conference { Id = 1 };

            // Act
            var result = testRepository.Delete(conference.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var conference = new Conference { Id = 1 };

            // Act
            var result = testRepository.Delete(conference.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedConferenceIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var conference = new Conference
            {
                Id = 1,
                LongName = "Conference 1",
                ShortName = "C1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var testRepository = new ConferenceRepository(fakeDbContext);

            var conferenceConferenceCountBeforeDelete = fakeDbContext.Conferences.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Conferences.Count().ShouldBe(conferenceConferenceCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedConferenceIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var conference = new Conference
            {
                Id = 1,
                LongName = "Conference 1",
                ShortName = "C1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var testRepository = new ConferenceRepository(fakeDbContext);

            var conferenceCountBeforeDelete = fakeDbContext.Conferences.Count();

            // Act
            var result = await testRepository.DeleteAsync(conference.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Conferences.Count().ShouldBe(conferenceCountBeforeDelete - 1);
            result.ShouldBe(conference);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var conference = new Conference { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(conference.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var conference = new Conference { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(conference.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedConferenceIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var conference = new Conference
            {
                Id = 1,
                LongName = "Conference 1",
                ShortName = "C1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var testRepository = new ConferenceRepository(fakeDbContext);

            var conferenceConferenceCountBeforeDelete = fakeDbContext.Conferences.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Conferences.Count().ShouldBe(conferenceConferenceCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void ConferenceExists_WhenDbSetIsNotNullAndSelectedConferenceExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.ConferenceExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void ConferenceExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.ConferenceExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void ConferenceExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.ConferenceExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void ConferenceExists_WhenSelectedConferenceDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.ConferenceExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task ConferenceExistsAsync_WhenDbSetIsNotNullAndSelectedConferenceExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.ConferenceExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task ConferenceExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.ConferenceExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task ConferenceExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.ConferenceExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task ConferenceExistsAsync_WhenSelectedConferenceDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.ConferenceExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<Conference> conferences)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = conferences;
            return fakeDbContext;
        }

        private IConferenceRepository CreateTestRepositoryWithEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();
            var conferences = new List<Conference>();
            var fakeDbSet = conferences.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);

            var testRepository = new ConferenceRepository(fakeDbContext);
            return testRepository;
        }

        private IConferenceRepository CreateTestRepositoryWithNotEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();
            var conferences = new List<Conference>
            {
                new Conference { Id = 1, LongName = "Conference 1", ShortName = "C1", LeagueId = 1, FirstSeasonId = 1920 },
                new Conference { Id = 2, LongName = "Conference 2", ShortName = "C2", LeagueId = 1, FirstSeasonId = 1920 },
                new Conference { Id = 3, LongName = "Conference 3", ShortName = "C3", LeagueId = 1, FirstSeasonId = 1920 },
            };
            var fakeDbSet = conferences.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);

            var testRepository = new ConferenceRepository(fakeDbContext);
            return testRepository;
        }

        private IConferenceRepository CreateTestRepositoryWithNullDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();
            DbSet<Conference> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);

            var testRepository = new ConferenceRepository(fakeDbContext);
            return testRepository;
        }
    }
}
