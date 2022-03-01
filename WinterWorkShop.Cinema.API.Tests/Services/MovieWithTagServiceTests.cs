using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using WinterWorkShop.Cinema.Repositories;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Data;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]

    public class MovieWithTagServiceTests
    {
        private Mock<IMovieWithTagRepository> _mockMovieWithTagRepo;
        private MovieWithTagService _movieWithTagService;
        private MovieWithTag _movieWithTag;
        [TestInitialize]
        public void TestInit()
        {
            _mockMovieWithTagRepo = new Mock<IMovieWithTagRepository>();
            _movieWithTagService = new MovieWithTagService(_mockMovieWithTagRepo.Object);
            _movieWithTag = new MovieWithTag
            {
                MovieId = Guid.NewGuid(),
                TagId = Guid.NewGuid(),
                Movie = new Movie(),
                Tag = new Tag()
            };
        }

        [TestMethod]
        public void MovieWithTagService_GetAll_Returns_ListMovieWithTagDomainModel()
        {
            //Arrange
            var expectedCount = 1;
            List<MovieWithTag> movieWithTagList = new List<MovieWithTag>();
            movieWithTagList.Add(_movieWithTag);
            IEnumerable<MovieWithTag> movieWithTags = movieWithTagList;
            Task<IEnumerable<MovieWithTag>> responseTask = Task.FromResult(movieWithTags);
            _mockMovieWithTagRepo.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var result = _movieWithTagService.GetAll().ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (List<MovieWithTagDomainModel>)result;
            var actualCount = resultObject.Count;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(actualCount, expectedCount);
            Assert.IsInstanceOfType(result, typeof(List<MovieWithTagDomainModel>));
            Assert.AreEqual(resultObject[0].MovieId, movieWithTagList[0].MovieId);
        }

        [TestMethod]
        public void MovieWithTagService_GetById_Returns_ListMovieWithTagDomainModel()
        {
            //Arrange
            var expectedCount = 1;
            List<MovieWithTag> movieWithTagsList = new List<MovieWithTag>();
            movieWithTagsList.Add(_movieWithTag);
            IEnumerable<MovieWithTag> movieWithTags = movieWithTagsList;
            Task<IEnumerable<MovieWithTag>> responseTask = Task.FromResult(movieWithTags);
            _mockMovieWithTagRepo.Setup(x => x.GetByMovieID(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _movieWithTagService.GetByMovieId(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (List<MovieWithTagDomainModel>)result;
            var actualCount = resultObject.Count;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultObject[0].MovieId, movieWithTagsList[0].MovieId);
            Assert.AreEqual(actualCount, expectedCount);
            Assert.IsInstanceOfType(result, typeof(List<MovieWithTagDomainModel>));
        }
    }
}
