using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.Domain.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class MovieWithTagControllerTests
    {
        private Mock<IMovieWithTagService> _mockMovieWithTagRepo;
        private MovieWithTagsController _movieWithTagsController;
        private MovieWithTagDomainModel _movieWithTagDomainModel;
        [TestInitialize]
        public void TestInit()
        {
            _mockMovieWithTagRepo = new Mock<IMovieWithTagService>();
            _movieWithTagDomainModel = new MovieWithTagDomainModel
            {
                MovieId = Guid.NewGuid(),
                MovieTitle = "Film1",
                TagId = Guid.NewGuid(),
                TagName = "Tag1"
            };
            _movieWithTagsController = new MovieWithTagsController(_mockMovieWithTagRepo.Object);
        }

        [TestMethod]
        public void MovieWithTagController_GetAll_Returns_MovieWithTagDomainModelList()
        {
            //Arrange
            var expectedStatusCode = 200;
            List<MovieWithTagDomainModel> movieWithTagDomainModelsList = new List<MovieWithTagDomainModel>();
            movieWithTagDomainModelsList.Add(_movieWithTagDomainModel);
            IEnumerable <MovieWithTagDomainModel> movieWithTagDomainModels = movieWithTagDomainModelsList;
            Task<IEnumerable<MovieWithTagDomainModel>> responseTask = Task.FromResult(movieWithTagDomainModels);
            _mockMovieWithTagRepo.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var result = _movieWithTagsController.GetAll().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var listMovieWithTagResult = (List<MovieWithTagDomainModel>)objectResult;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.AreEqual(listMovieWithTagResult[0].TagId, movieWithTagDomainModelsList[0].TagId);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void MovieWithTagController_GetAll_Returns_EmptyMovieWithTagDomainModelList()
        {
            //Arrange
            var expectedStatusCode = 200;
            var count = 0;
            IEnumerable<MovieWithTagDomainModel> movieWithTagDomainModels = null;
            Task<IEnumerable<MovieWithTagDomainModel>> responseTask = Task.FromResult(movieWithTagDomainModels);
            _mockMovieWithTagRepo.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var result = _movieWithTagsController.GetAll().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var listMovieWithTagResult = (List<MovieWithTagDomainModel>)objectResult;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.AreEqual(count, listMovieWithTagResult.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void MovieWithTagController_GetByMovieId_Returns_MovieWithTagDomainModelList()
        {
            //Arrange
            var expectedStatusCode = 200;
            var expectedCount = 1;
            List<MovieWithTagDomainModel> movieWithTagDomainModelsList = new List<MovieWithTagDomainModel>();
            movieWithTagDomainModelsList.Add(_movieWithTagDomainModel);
            IEnumerable<MovieWithTagDomainModel> movieWithTagDomainModels = movieWithTagDomainModelsList;
            Task<IEnumerable<MovieWithTagDomainModel>> responseTask = Task.FromResult(movieWithTagDomainModels);
            _mockMovieWithTagRepo.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _movieWithTagsController.GetByMovieId(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((OkObjectResult)result).Value;
            var listMovieWithTagResult = (List<MovieWithTagDomainModel>)resultObject;
            var actualCount = listMovieWithTagResult.Count;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;

            //Assert
            Assert.AreEqual(actualCount, expectedCount);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(resultObject, typeof(List<MovieWithTagDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(listMovieWithTagResult[0].MovieId, movieWithTagDomainModelsList[0].MovieId);
        }

        [TestMethod]
        public void MovieWithTagController_GetByMovieId_Returns_EmptyMovieWithTagDomainModelList()
        {
            //Arrange
            var expectedStatusCode = 200;
            var expectedCount = 0;
            IEnumerable<MovieWithTagDomainModel> movieWithTagDomainModels = null;
            Task<IEnumerable<MovieWithTagDomainModel>> responseTask = Task.FromResult(movieWithTagDomainModels);
            _mockMovieWithTagRepo.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _movieWithTagsController.GetByMovieId(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((OkObjectResult)result).Value;
            var listMovieWithTagResult = (List<MovieWithTagDomainModel>)resultObject;
            var actualCount = listMovieWithTagResult.Count;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;

            //Assert
            Assert.AreEqual(actualCount, expectedCount);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(resultObject, typeof(List<MovieWithTagDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}
