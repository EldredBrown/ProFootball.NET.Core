using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Tests.ModelTests
{
    public class GameTest
    {
        private readonly Game _testGame;

        public GameTest()
        {
            _testGame = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Test"
            };
        }

        [Fact]
        public void IsTie_WhenGuestScoreIsGreaterThanHostScore_ShouldReturnFalse()
        {
            _testGame.GuestScore = 1;
            _testGame.HostScore = 0;

            var result = _testGame.IsTie;

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsTie_WhenHostScoreIsGreaterThanGuestScore_ShouldReturnFalse()
        {
            _testGame.GuestScore = 0;
            _testGame.HostScore = 1;

            var result = _testGame.IsTie;

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsTie_WhenGuestScoreEqualsHostScore_ShouldReturnTrue()
        {
            _testGame.GuestScore = 0;
            _testGame.HostScore = 0;

            var result = _testGame.IsTie;

            result.ShouldBeTrue();
        }

        [Fact]
        public void WinnerLoserProperties_WhenGameIsTie_ShouldReturnNull()
        {
            _testGame.GuestScore = 3;
            _testGame.HostScore = 3;

            _testGame.WinnerName.ShouldBeNull();
            _testGame.WinnerScore.ShouldBeNull();
            _testGame.LoserName.ShouldBeNull();
            _testGame.LoserScore.ShouldBeNull();
        }

        [Fact]
        public void WinnerLoserProperties_WhenGuestScoreGreaterThanHostScore_ShouldReturnCorrectValues()
        {
            _testGame.GuestScore = 3;
            _testGame.HostScore = 2;

            _testGame.WinnerName.ShouldBe(_testGame.GuestName);
            _testGame.WinnerScore.ShouldBe(_testGame.GuestScore);
            _testGame.LoserName.ShouldBe(_testGame.HostName);
            _testGame.LoserScore.ShouldBe(_testGame.HostScore);
        }

        [Fact]
        public void WinnerLoserProperties_WhenHostScoreGreaterThanGuestScore_ShouldReturnCorrectValues()
        {
            _testGame.GuestScore = 2;
            _testGame.HostScore = 3;

            _testGame.WinnerName.ShouldBe(_testGame.HostName);
            _testGame.WinnerScore.ShouldBe(_testGame.HostScore);
            _testGame.LoserName.ShouldBe(_testGame.GuestName);
            _testGame.LoserScore.ShouldBe(_testGame.GuestScore);
        }

        [Fact]
        public void Edit_ShouldUpdateGameDataWithDataFromSource()
        {
            var srcGame = new Game
            {
                Week = 1,
                GuestName = "Guest",
                GuestScore = 3,
                HostName = "Host",
                HostScore = 3,
                IsPlayoff = true,
                Notes = "Notes"
            };

            _testGame.Edit(srcGame);

            _testGame.Week.ShouldBe(srcGame.Week);
            _testGame.GuestName.ShouldBe(srcGame.GuestName);
            _testGame.GuestScore.ShouldBe(srcGame.GuestScore);
            _testGame.HostName.ShouldBe(srcGame.HostName);
            _testGame.HostScore.ShouldBe(srcGame.HostScore);
            _testGame.IsPlayoff.ShouldBe(srcGame.IsPlayoff);
            _testGame.Notes.ShouldBe(srcGame.Notes);
        }
    }
}
