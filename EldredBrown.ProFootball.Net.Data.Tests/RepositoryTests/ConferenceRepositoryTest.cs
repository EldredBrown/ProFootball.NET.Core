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
                new Conference { Id = 1, ShortName = "C1", LongName = "Conference 1", FirstSeasonId = 1920 },
                new Conference { Id = 2, ShortName = "C2", LongName = "Conference 2", FirstSeasonId = 1921 },
                new Conference { Id = 3, ShortName = "C3", LongName = "Conference 3", FirstSeasonId = 1922 },
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
                new Conference { Id = 1, ShortName = "C1", LongName = "Conference 1", FirstSeasonId = 1920 },
                new Conference { Id = 2, ShortName = "C2", LongName = "Conference 2", FirstSeasonId = 1921 },
                new Conference { Id = 3, ShortName = "C3", LongName = "Conference 3", FirstSeasonId = 1922 },
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
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();

            int id = 1;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };

            A.CallTo(() => fakeDbContext.Conferences.Find(An<int>.Ignored)).Returns(conference);
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.GetConference(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(conference);
        }

        [Fact]
        public void GetConference_WhenConferencesIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = null;
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.GetConference(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetConferenceAsync_WhenConferencesIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();

            int id = 1;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };

            _ = A.CallTo(() => fakeDbContext.Conferences.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Conference?>(conference));
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.GetConferenceAsync(id).Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(conference);
        }

        [Fact]
        public void GetConferenceAsync_WhenConferencesIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = null;
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.GetConferenceAsync(1).Result;

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

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };

            // Act
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

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };

            // Act
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
                LeagueId = 1,
                FirstSeasonId = 1920
            };

            fakeDbContext.Conferences.Add(conference);
            await fakeDbContext.SaveChangesAsync();

            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            conference.LastSeasonId = 2026;
            repository.Update(conference);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            var updated = await fakeDbContext.Conferences.FirstOrDefaultAsync(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenConferencesIsNotNullAndSelectedConferenceIsNotNull_ShouldSucceed()
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

            A.CallTo(() => fakeDbContext.Conferences.Find(An<int>.Ignored)).Returns(conference);
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.Delete(conference.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Conferences.Remove(conference)).MustHaveHappenedOnceExactly();
            result.ShouldBe(conference);
        }

        [Fact]
        public void Delete_WhenConferencesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = null;
            var repository = new ConferenceRepository(fakeDbContext);

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };

            // Act
            var result = repository.Delete(conference.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenConferencesIsNotNullAndSelectedConferenceIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();
            A.CallTo(() => fakeDbContext.Conferences.Find(An<int>.Ignored)).Returns(null);
            var repository = new ConferenceRepository(fakeDbContext);

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };

            // Act
            var result = repository.Delete(conference.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Conferences.Remove(conference)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenConferencesIsNotNullAndSelectedConferenceIsNotNull_ShouldSucceed()
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

            _ = A.CallTo(() => fakeDbContext.Conferences.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Conference?>(conference));
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.DeleteAsync(conference.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Conferences.Remove(conference)).MustHaveHappenedOnceExactly();
            result.ShouldBe(conference);
        }

        [Fact]
        public void DeleteAsync_WhenConferencesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = null;
            var repository = new ConferenceRepository(fakeDbContext);

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };

            // Act
            var result = repository.DeleteAsync(conference.Id).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenConferencesIsNotNullAndSelectedConferenceIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Conferences = A.Fake<DbSet<Conference>>();
            _ = A.CallTo(() => fakeDbContext.Conferences.FindAsync(An<int>.Ignored)).Returns(null);
            var repository = new ConferenceRepository(fakeDbContext);

            var conference = new Conference
            {
                Id = 1,
                ShortName = "C1",
                LongName = "Conference 1",
                FirstSeasonId = 1920
            };

            // Act
            var result = repository.DeleteAsync(conference.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Conferences.Remove(conference)).MustNotHaveHappened();
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
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.ConferenceExists(conference.Id);

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
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.ConferenceExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void ConferenceExistsAsync_WhenConferencesIsNotNullAndSelectedConferenceExists_ShouldReturnTrue()
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
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.ConferenceExistsAsync(conference.Id).Result;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void ConferenceExistsAsync_WhenConferencesIsNotNullAndSelectedConferenceDoesNotExist_ShouldReturnFalse()
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
            var repository = new ConferenceRepository(fakeDbContext);

            // Act
            var result = repository.ConferenceExistsAsync(2).Result;

            // Assert
            result.ShouldBeFalse();
        }
    }
}
