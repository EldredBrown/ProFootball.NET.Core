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
        public ConferenceRepositoryTest() { }

        [Fact]
        public void GetConferences_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();

            var conferences = new List<Conference>
            {
                new Conference { Id = 1, ShortName = "C1", LongName = "Conference 1" },
                new Conference { Id = 2, ShortName = "C2", LongName = "Conference 2" },
                new Conference { Id = 3, ShortName = "C3", LongName = "Conference 3" },
            };

            var fakeDbSet = conferences.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.GetConferences();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Conference>();
            }
        }

        [Fact]
        public void GetConferencesAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();

            var conferences = new List<Conference>
            {
                new Conference { Id = 1, ShortName = "C1", LongName = "Conference 1" },
                new Conference { Id = 2, ShortName = "C2", LongName = "Conference 2" },
                new Conference { Id = 3, ShortName = "C3", LongName = "Conference 3" },
            };

            var fakeDbSet = conferences.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.GetConferences();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetConference_WhenConferencesIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C1",
                LongName = "Conference 1",
                LeagueId = 1,
                FirstSeasonId = 1920
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = testRepository.GetConference(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(conference);
        }

        [Fact]
        public void GetConference_WhenConferencesIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Conferences = null;
            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = testRepository.GetConference(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetConferenceAsync_WhenConferencesIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C1",
                LongName = "Conference 1",
                LeagueId = 1,
                FirstSeasonId = 1920
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetConferenceAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(conference);
        }

        [Fact]
        public async Task GetConferenceAsync_WhenConferencesIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Conferences = null;
            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetConferenceAsync(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            var result = repository.Add(conference);

            // Assert
            A.CallTo(() => fakeDbContext.Add(conference)).MustHaveHappenedOnceExactly();
            result.ShouldBe(conference);
        }

        [Fact]
        public void AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            var result = repository.AddAsync(conference).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(conference)).MustHaveHappenedOnceExactly();
            result.ShouldBe(conference);
        }

        [Fact]
        public async Task Update_WhenConferencesIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            conference.LastSeasonId = 2026;
            testRepository.Update(conference);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Conferences.FirstOrDefault(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenConferencesIsNotNullAndSelectedConferenceIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C1",
                LongName = "Conference 1",
                LeagueId = 1,
                FirstSeasonId = 1920
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var conferenceCountBeforeDelete = fakeDbContext.Conferences.Count();

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(conference.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Conferences.Count().ShouldBe(conferenceCountBeforeDelete - 1);
            result.ShouldBe(conference);
        }

        [Fact]
        public void Delete_WhenConferencesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Conferences = null;

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            var result = testRepository.Delete(conference.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenConferencesIsNotNullAndSelectedConferenceIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };

            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            int id = 1;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var conferenceCountBeforeDelete = fakeDbContext.Conferences.Count();

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Conferences.Count().ShouldBe(conferenceCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenConferencesIsNotNullAndSelectedConferenceIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C1",
                LongName = "Conference 1",
                LeagueId = 1,
                FirstSeasonId = 1920
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var conferenceCountBeforeDelete = fakeDbContext.Conferences.Count();

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(conference.Id);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Conferences.Count().ShouldBe(conferenceCountBeforeDelete - 1);
            result.ShouldBe(conference);
        }

        [Fact]
        public async Task DeleteAsync_WhenConferencesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Conferences = null;

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            var result = await testRepository.DeleteAsync(conference.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenConferencesIsNotNullAndSelectedConferenceIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };

            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            int id = 1;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Conferences.Add(conference);
            fakeDbContext.SaveChanges();

            var conferenceCountBeforeDelete = fakeDbContext.Conferences.Count();

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(2);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Conferences.Count().ShouldBe(conferenceCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void ConferenceExists_WhenConferencesIsNotNullAndSelectedConferenceExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<Conference> { conference }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = testRepository.ConferenceExists(conference.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void ConferenceExists_WhenConferencesIsNotNullAndSelectedConferenceDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<Conference> { conference }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = testRepository.ConferenceExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task ConferenceExistsAsync_WhenConferencesIsNotNullAndSelectedConferenceExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<Conference> { conference }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = await testRepository.ConferenceExistsAsync(conference.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task ConferenceExistsAsync_WhenConferencesIsNotNullAndSelectedConferenceDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<Conference> { conference }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Conferences).Returns(fakeDbSet);

            var testRepository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = await testRepository.ConferenceExistsAsync(2);

            // Assert
            result.ShouldBeFalse();
        }
    }
}
