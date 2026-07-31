using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ViewModelTests
{
    public class AssociationViewModelMapperTests
    {
        [Fact]
        public void MapAssociationToViewModel_ShouldSucceed()
        {
            // Arrange
            (AssociationViewModelMapper testMapper, Association association) = SetUp();

            // Act
            var result = testMapper.MapAssociationToViewModel(association);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<AssociationViewModel>();
            result.Association.ShouldBe(association);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task MapViewModelToAssociation_WhenParentNameIsNullOrEmptyAndParentIsNull_ShouldSetAssociationParentIdToNull(
            string? parentName)
        {
            // Arrange
            Association parent = null!;
            (AssociationViewModelMapper testMapper, _) = SetUp(parent: parent);

            // Act
            var associationViewModel = new AssociationViewModel
            {
                Id = 1,
                LongName = "Test Association",
                ShortName = "TA",
                ParentName = parentName,
                FirstSeasonYear = 1920,
                LastSeasonYear = null
            };

            var result = await testMapper.MapViewModelToAssociation(associationViewModel);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Association>();
            result.ShouldBe(associationViewModel.Association);
            result.ParentId.ShouldBeNull();
        }

        [Fact]
        public async Task MapViewModelToAssociation_WhenParentNameIsNeitherNullNorEmptyAndParentIsNull_ShouldSetAssociationParentIdToMinusOne()
        {
            // Arrange
            Association parent = null!;
            (AssociationViewModelMapper testMapper, _) = SetUp(parent: parent);

            // Act
            var associationViewModel = new AssociationViewModel
            {
                Id = 1,
                LongName = "Test Association",
                ShortName = "TA",
                ParentName = "Test Parent",
                FirstSeasonYear = 1920,
                LastSeasonYear = null
            };

            var result = await testMapper.MapViewModelToAssociation(associationViewModel);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Association>();
            result.ShouldBe(associationViewModel.Association);
            result.ParentId.ShouldBe(-1);
        }

        [Fact]
        public async Task MapViewModelToAssociation_WhenParentNameIsNeitherNullNorEmptyAndParentIsNotNull_ShouldSetAssociationParentIdToMinusOne()
        {
            // Arrange
            Association parent = new() { Id = 2 };
            (AssociationViewModelMapper testMapper, _) = SetUp(parent: parent);

            // Act
            var associationViewModel = new AssociationViewModel
            {
                Id = 1,
                LongName = "Test Association",
                ShortName = "TA",
                ParentName = "Test Parent",
                FirstSeasonYear = 1920,
                LastSeasonYear = null
            };

            var result = await testMapper.MapViewModelToAssociation(associationViewModel);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Association>();
            result.ShouldBe(associationViewModel.Association);
            result.ParentId.ShouldBe(parent.Id);
        }

        private static (AssociationViewModelMapper testMapper, Association association) SetUp(Association? parent = null)
        {
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns(parent);

            var testMapper = new AssociationViewModelMapper(fakeAssociationRepository);
            var association = new EldredBrown.ProFootball.Net.Data.Models.Association
            {
                Id = 1,
                LongName = "Test Association",
                ShortName = "TA",
                FirstSeasonYear = 1,
                LastSeasonYear = 2
            };

            return (testMapper, association);
        }
    }
}
