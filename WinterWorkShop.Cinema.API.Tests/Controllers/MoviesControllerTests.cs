using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WinterWorkShop.Cinema.Domain.Models;
using Moq;
using WinterWorkShop.Cinema.Repositories;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.API.Controllers;
using Microsoft.Extensions.Logging;
using WinterWorkShop.Cinema.Domain.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class MoviesControllerTests
    {

        private Mock<IMovieService> _mockMoviesService;
        private MovieDomainModel _movieDomainModel;
        private Mock<ILogger<MoviesController>> _mockMoviesLogger;
        MoviesController _moviesController;

        [TestInitialize]
        public void TestInit()
        {
            _movieDomainModel = new MovieDomainModel
            {
                Title = "ImeFilma",
                Current = false,
                HasOscar = false,
                Id = Guid.NewGuid(),
                Rating = 7,
                Year = 2010
            };
            _mockMoviesService = new Mock<IMovieService>();
            _mockMoviesLogger = new Mock<ILogger<MoviesController>>();
            _moviesController = new MoviesController(_mockMoviesLogger.Object, _mockMoviesService.Object);
        }

        [TestMethod]

        public void GetAll_Return_All_Movies()
        {
            //Arrange 
            List<MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            movieDomainModelsList.Add(_movieDomainModel);
            int expectedStatusCode = 200;
            int expectedCount = 1;

            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetAllMovies()).Returns(responseTask);

            //Act
            var result = _moviesController.GetAll().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            List<MovieDomainModel> moviesDomainModelResultList = (List<MovieDomainModel>)resultList;


            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(moviesDomainModelResultList[0].Id, movieDomainModelsList[0].Id);
            Assert.AreEqual(movieDomainModelsList.Count, expectedCount);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAll_Return_NewList()
        {
            //Arrange 
            int expectedStatusCode = 200;
            int expectedCount = 0;

            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetAllMovies()).Returns(responseTask);

            //Act
            var result = _moviesController.GetAll().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            List<MovieDomainModel> moviesDomainModelResultList = (List<MovieDomainModel>)resultList;


            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(moviesDomainModelResultList.Count, expectedCount);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetCurrent_Return_CurrentMovies()
        {
            //Arrange
            int expectedStatusCode = 200;
            int expectedCount = 1;

            List<MovieDomainModel> moviesDomainModelsList = new List<MovieDomainModel>();
            moviesDomainModelsList.Add(_movieDomainModel);
            IEnumerable<MovieDomainModel> movieDomainModels = moviesDomainModelsList;
            _mockMoviesService.Setup(x => x.GetCurrentMovies(true)).Returns(movieDomainModels);

            //Act
            var result = _moviesController.GetCurrent().Result;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var resultList = ((OkObjectResult)result).Value;
            List<MovieDomainModel> listFromResult = ((List<MovieDomainModel>)resultList);

            //Assert

            Assert.IsNotNull(resultList);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.AreEqual(expectedCount, listFromResult.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsInstanceOfType(resultList, typeof(List<MovieDomainModel>));
        }

        [TestMethod]
        public void GetCurrent_Return_NewList()
        {
            //Arrange
            int expectedStatusCode = 200;
            int expectedCount = 0;

            List<MovieDomainModel> moviesDomainModelsList = null;
            IEnumerable<MovieDomainModel> movieDomainModels = moviesDomainModelsList;
            _mockMoviesService.Setup(x => x.GetCurrentMovies(true)).Returns(movieDomainModels);

            //Act
            var result = _moviesController.GetCurrent().Result;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var resultList = ((OkObjectResult)result).Value;
            List<MovieDomainModel> listFromResult = ((List<MovieDomainModel>)resultList);

            //Assert

            Assert.IsNotNull(resultList);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.AreEqual(expectedCount, listFromResult.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsInstanceOfType(resultList, typeof(List<MovieDomainModel>));
        }

        [TestMethod]
        public void GetById_Returns_Movie()
        {
            //Arrange
            int expectedStatusCode = 200;

            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                IsSuccessful = true,
                DomainModel = _movieDomainModel
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _moviesController.GetAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var objectResult = ((OkObjectResult)result).Value;
            var resultMovieDomainModel = ((MovieDomainModel)objectResult);

            //Assert

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultMovieDomainModel.Id, _movieDomainModel.Id);
        }

        [TestMethod]
        public void GetMovieById_Return_NotFound()
        {
            //Arrange
            int expectedStatusCode = 404;
            string expectedErrorMessage = "Movie doesn't exists!";

            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                IsSuccessful = false,
                DomainModel = null,
                ErrorMessage = "Movie doesn't exists!"
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _moviesController.GetAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var actualStatusCode = ((NotFoundObjectResult)result).StatusCode;
            var objectResult = ((NotFoundObjectResult)result).Value;
            var resultErrorMessage = ((string)objectResult);

            //Assert

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(resultErrorMessage, expectedErrorMessage);
        }

        [TestMethod]
        public void GetTop10_ReturnTop10Movies()
        {
            //Act
            List<MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            movieDomainModelsList.Add(_movieDomainModel);
            int expectedStatusCode = 200;
            int expectedCount = 1;
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetTopTenMovies()).Returns(responseTask);

            //Arrange
            var result = _moviesController.GetTopTenMovies().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            List<MovieDomainModel> moviesDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultList, movieDomainModelsList);
            Assert.AreEqual(((OkObjectResult)result).StatusCode, expectedStatusCode);
            Assert.AreEqual(moviesDomainModelResultList.Count, expectedCount);
        }

        [TestMethod]
        public void Get_Top10_Returns_NewList()
        {
            //Act
            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetTopTenMovies()).Returns(responseTask);
            int expectedStatusCode = 200;
            int expectedCount = 0;

            //Arrange
            var result = _moviesController.GetTopTenMovies().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMovieDomainModel = ((OkObjectResult)result).Value;
            var resultMovieDomainModelList = (List<MovieDomainModel>)resultMovieDomainModel;

            //Assert

            Assert.IsNotNull(resultMovieDomainModel);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(((OkObjectResult)result).StatusCode, expectedStatusCode);
            Assert.AreEqual(resultMovieDomainModelList.Count, expectedCount);

        }

        [TestMethod]
        public void CreateMovie_Returns_SuccessfulCreated()
        {
            //Arrange
            int expectedStatusCode = 201;
            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel
            {
                TagName = "tag1"
            });
            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = _movieDomainModel,
                IsSuccessful = true
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);

            //Act
            var result = _moviesController.Post(new MovieModel { Tags = new List<string>()}).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((CreatedResult)result).Value;
            var actualStatusCode = ((CreatedResult)result).StatusCode;
            var resultResponseModel = (MovieDomainModel)resultObject;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
        }

        [TestMethod]
        public void CreateMovie_Returns_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid model state";
            int expectedStatusCode = 400;

            _moviesController.ModelState.AddModelError("key", "Invalid model state");


            //Act
            var result = _moviesController.Post(new MovieModel { Tags = new List<string>()}).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]

        public void CreateMovie_ReturnsNullObject()
        {
            //Arrange
            var expectedStatusCode = 500;
            var expectedErrorMessage = Messages.MOVIE_CREATION_ERROR;
            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel
            {
                TagName = "tag1"
            });
            ResponseModel<MovieDomainModel> responseModel = null;
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.AddMovie(null)).Returns(responseTask);

            //Act
            var result = _moviesController.Post(new MovieModel { Tags = new List<string>() }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((ObjectResult)result).Value ;
            var errorResponseResult = ((ErrorResponseModel)objectResult);
            string actualErrorMessage = errorResponseResult.ErrorMessage;
            int actualStatusCode = (int)errorResponseResult.StatusCode;
            

            //Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(actualStatusCode, expectedStatusCode);
            Assert.AreEqual(actualErrorMessage, expectedErrorMessage);
            
        }

        [TestMethod]
        public void CreateMovie_Returns_UnsuccessfulCreated()
        {
            //Arrange
            var expectedStatusCode = 400;
            var expectedMessage = "Failed to add movie!";
            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = null,
                IsSuccessful = false,
                ErrorMessage = "Failed to add movie!"
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);

            //Act
            var result = _moviesController.Post(new MovieModel { Tags = new List<string>() }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((BadRequestObjectResult)result).Value;
            var actualMessage = ((string)objectResult);
            var actualStatusCode = ((BadRequestObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);

        }

        [TestMethod]

        public void CreateMovie_ThrowsDbException()
        {
            //Arrange
            string expectedMessage = "Inner exception error message";
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error!", exception);
            _mockMoviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);

            //Act
            var result = _moviesController.Post(new MovieModel { Tags = new List<string>() }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)resultObject);
            var actualMessage = errorResponseResult.ErrorMessage;
            var actualStatusCode = errorResponseResult.StatusCode;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(actualMessage, expectedMessage);
            Assert.AreEqual(expectedMessage, actualMessage);

        }


        [TestMethod]
        public void EditMovie_Returns_SuccessfulAccepted()
        {
            //Arrange
            int expectedStatusCode = 202;
            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel
            {
                TagName = "tag1"
            });

            var updatedMovie = new MovieDomainModel
            {
                Id = _movieDomainModel.Id,
                Title = _movieDomainModel.Title,
                BannerUrl = _movieDomainModel.BannerUrl,
                Current = _movieDomainModel.Current,
                Rating = 9,
                HasOscar = true,
                Tags = _movieDomainModel.Tags,
                Year = _movieDomainModel.Year
            };

            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = _movieDomainModel,
                IsSuccessful = true
            };
            ResponseModel<MovieDomainModel> updatedResponseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = updatedMovie,
                IsSuccessful = true
            };

            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            Task<ResponseModel<MovieDomainModel>> updatedResponseTask = Task.FromResult(updatedResponseModel);

            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(updatedResponseTask);
            _mockMoviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);

            //Act
            var result = _moviesController.Put(It.IsAny<Guid>(),new MovieModel { Tags = new List<string>() }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((AcceptedResult)result).Value;
            var actualStatusCode = ((AcceptedResult)result).StatusCode;
            var resultResponseModel = (MovieDomainModel)resultObject;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
        }

        [TestMethod]
        public void EditMovie_Returns_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid model state";
            int expectedStatusCode = 400;

            _moviesController.ModelState.AddModelError("key", "Invalid model state");


            //Act
            var result = _moviesController.Put(It.IsAny<Guid>(),new MovieModel { Tags = new List<string>() }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
      
        public void EditMovie_ReturnsMovieNotFound()
        {
            //Arrange
            var expectedStatusCode = 400;
            var expectedErrorMessage = Messages.MOVIE_DOES_NOT_EXIST;
            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel
            {
                TagName = "tag1"
            });
            ResponseModel<MovieDomainModel> responseModel = null;
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.AddMovie(null)).Returns(responseTask);

            //Act
            var result = _moviesController.Put(It.IsAny<Guid>(),new MovieModel { Tags = new List<string>() }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)objectResult);
            string actualErrorMessage = errorResponseResult.ErrorMessage;
            int actualStatusCode = (int)errorResponseResult.StatusCode;


            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(actualStatusCode, expectedStatusCode);
            Assert.AreEqual(actualErrorMessage, expectedErrorMessage);

        }

        [TestMethod]
        public void EditMovie_Returns_UnsuccessfulEdited()
        {
            //Arrange
            var expectedStatusCode = 400;
            var expectedMessage = "Failed to update movie!";

            _movieDomainModel.Tags = new List<TagDomainModel>();

            _movieDomainModel.Tags.Add(
                new TagDomainModel
                {
                    TagName = "Tag1"
                }
            );

            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = _movieDomainModel,
                IsSuccessful = true,
            };

            ResponseModel<MovieDomainModel> updatedResponseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = null,
                IsSuccessful = false,
                ErrorMessage = "Failed to update movie!"
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            Task<ResponseModel<MovieDomainModel>> updatedResponseTask = Task.FromResult(updatedResponseModel);
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            _mockMoviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(updatedResponseTask);

            //Act
            var result = _moviesController.Put(It.IsAny<Guid>(),new MovieModel { Tags = new List<string>() }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((BadRequestObjectResult)result).Value;
            var actualMessage = ((string)objectResult);
            var actualStatusCode = ((BadRequestObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);

        }

        [TestMethod]
        public void EditMovie_ThrowsDbException()
        {
            //Arrange
            string expectedMessage = "Inner exception error message";
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error!", exception);

            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel
            {
                TagName = "tag1"
            });

            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = _movieDomainModel,
                IsSuccessful = true
            };
            Task <ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            _mockMoviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);

            //Act
            var result = _moviesController.Put(It.IsAny<Guid>(),new MovieModel { Tags = new List<string>() }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)resultObject);
            var actualMessage = errorResponseResult.ErrorMessage;
            var actualStatusCode = errorResponseResult.StatusCode;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(actualMessage, expectedMessage);
            Assert.AreEqual(expectedMessage, actualMessage);

        }

        [TestMethod]
        public void DeleteMovie_Returns_SuccessfulDeleted()
        {
            //Arrange
            int expectedStatusCode = 202;
            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel
            {
                TagName = "tag1"
            });
            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = _movieDomainModel,
                IsSuccessful = true
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _moviesController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((AcceptedResult)result).Value;
            var actualStatusCode = ((AcceptedResult)result).StatusCode;
            var resultResponseModel = (MovieDomainModel)resultObject;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
        }

        [TestMethod]
        public void DeleteMovie_ReturnsNullObject()
        {
            //Arrange
            var expectedStatusCode = 500;
            var expectedMessage = Messages.MOVIE_DOES_NOT_EXIST;
            ResponseModel<MovieDomainModel> responseModel = null;
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _moviesController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((ObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)objectResult);
            var actualStatusCode = ((ObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedMessage, errorResponseResult.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, (int)errorResponseResult.StatusCode);

        }

        [TestMethod]
        public void DeleteMovie_Returns_UnsuccessfulDeleted()
        {
            //Arrange
            var expectedStatusCode = 400;
            var expectedMessage = "Movie has projections in future!";
            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = null,
                IsSuccessful = false,
                ErrorMessage = "Movie has projections in future!"
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _moviesController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = (ErrorResponseModel)objectResult;
            var actualStatusCode = ((BadRequestObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedMessage, errorResponseResult.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, (int)errorResponseResult.StatusCode);

        }

        [TestMethod]
        public void DeleteMovie_ThrowsDbException()
        {
            //Arrange
            string expectedMessage = "Inner exception error message";
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error!", exception);
            _mockMoviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Throws(dbUpdateException);

            //Act
            var result = _moviesController.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)resultObject);
            var actualMessage = errorResponseResult.ErrorMessage;
            var actualStatusCode = errorResponseResult.StatusCode;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(actualMessage, expectedMessage);
            Assert.AreEqual(expectedMessage, actualMessage);

        }

        [TestMethod]
        public void GetTop10ByYear_ReturnTop10Movies()
        {
            //Act
            List<MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            movieDomainModelsList.Add(_movieDomainModel);
            int forParameter = new int();
            int expectedStatusCode = 200;
            int expectedCount = 1;
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetTopTenMoviesByYear(forParameter)).Returns(responseTask);

            //Arrange
            var result = _moviesController.GetTopTenMovies(forParameter).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            List<MovieDomainModel> moviesDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultList, movieDomainModelsList);
            Assert.AreEqual(((OkObjectResult)result).StatusCode, expectedStatusCode);
            Assert.AreEqual(moviesDomainModelResultList.Count, expectedCount);
        }

        [TestMethod]
        public void Get_Top10ByYear_Returns_NewList()
        {
            //Act
            IEnumerable<MovieDomainModel> movieDomainModels = null;
            int forParameter = new int();
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetTopTenMoviesByYear(forParameter)).Returns(responseTask);
            int expectedStatusCode = 200;
            int expectedCount = 0;

            //Arrange
            var result = _moviesController.GetTopTenMovies(forParameter).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMovieDomainModel = ((OkObjectResult)result).Value;
            var resultMovieDomainModelList = (List<MovieDomainModel>)resultMovieDomainModel;

            //Assert

            Assert.IsNotNull(resultMovieDomainModel);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(((OkObjectResult)result).StatusCode, expectedStatusCode);
            Assert.AreEqual(resultMovieDomainModelList.Count, expectedCount);

        }

        // if (!ModelState.IsValid) - false
        // try  await _movieService.ActivateDeactivateMovie(id); - return valid mock
        // if (changedCurrentMovie == null)-false
        // if (createProjectionResultModel.IsSuccessful) - true
        // return Accepted
        [TestMethod]
        public void Patch_ActivateDeactivate_Returns_MovieDomainModel()
        {

            //Act
            Guid forParameter = new Guid();
            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = _movieDomainModel,
                IsSuccessful = true
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            int expectedStatusCode = 202;
            _mockMoviesService.Setup(x => x.ActivateDeactivateMovie(forParameter)).Returns(responseTask);

            //Arrange
            var result = _moviesController.ActivateDeactivateMovie(forParameter).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMovieDomainModel = ((AcceptedResult)result).Value;
            var movieDomainModel = (MovieDomainModel)resultMovieDomainModel;

            //Act
            Assert.IsNotNull(resultMovieDomainModel);
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
            Assert.AreEqual(((AcceptedResult)result).StatusCode, expectedStatusCode);
            Assert.AreEqual(movieDomainModel.Id, responseModel.DomainModel.Id);
        }

        [TestMethod]
        public void Patch_ActivateDeactivate_Throws_DbException()
        {
            //Act

            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                DomainModel = _movieDomainModel,
                ErrorMessage = dbUpdateException.InnerException.Message,
                IsSuccessful = false
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>())).Throws(dbUpdateException);

            //Arrange
            var result = _moviesController.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = (ErrorResponseModel)badObjectResult;

            //Act
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(resultResponse, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedMessage, errorResponseResult.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);

        }

        [TestMethod]

        public void Patch_ActivateDeactivate_Returns_InternalServerErrorCode()
        {
            //Act
            int expectedStatusCode = 500;
            string expectedMessage = Messages.MOVIE_DOES_NOT_EXIST;
            ResponseModel<MovieDomainModel> responseModel = null;
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockMoviesService.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>())).Returns(responseTask);

            //Arrange
            var result = _moviesController.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var badResult = (ObjectResult)result;
            var badObjectResult = ((ObjectResult)result).Value;
            var ErrorResponseModel = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(badResult);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(badResult.StatusCode, expectedStatusCode);
            Assert.AreEqual(ErrorResponseModel.ErrorMessage, expectedMessage);

        }

        [TestMethod]

        public void Patch_ActivateDeactivate_IsSuccessFalse_Returns_BadRequest()
        {
            //Act
            ResponseModel<MovieDomainModel> responseModel = new ResponseModel<MovieDomainModel>
            {
                IsSuccessful = false,
                ErrorMessage = "Movie has projections in future!"
            };
            Task<ResponseModel<MovieDomainModel>> responseTask = Task.FromResult(responseModel);
            int expectedStatusCode = 400;
            string expectedMessage = "Movie has projections in future!";
            _mockMoviesService.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>())).Returns(responseTask);

            //Arrange
            var result = _moviesController.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var badResult = (BadRequestObjectResult)result;
            var badResultObject = ((BadRequestObjectResult)result).Value;

            //Assert
            Assert.IsNotNull(badResult);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(badResult.StatusCode, expectedStatusCode);
            Assert.AreEqual(badResultObject, expectedMessage);
        }

        [TestMethod]
        public void MoviesController_GetMoviesByAuditoriumId_ReturnMovies()
        {
            //Arrange
            var response = new ResponseModel<IEnumerable<MovieDomainModel>>()
            {
                DomainModel = new List<MovieDomainModel>() { _movieDomainModel },
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<IEnumerable<MovieDomainModel>>> dbReturn = Task.FromResult((ResponseModel<IEnumerable<MovieDomainModel>>)response);
            _mockMoviesService.Setup(x => x.GetMoviesByAuditoriumId(1)).Returns(dbReturn);

            //Act
            var resultAction = _moviesController.GetMoviesByAuditoriumId(1).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var model = (List<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_movieDomainModel.Title, model[0].Title);
            Assert.AreEqual(_movieDomainModel.Id, model[0].Id);
            Assert.IsInstanceOfType(model, typeof(List<MovieDomainModel>));
        }

        [TestMethod]
        public void MoviesController_GetMoviesByAuditoriumId_ReturnError()
        {
            //Arrange
            var response = new ResponseModel<IEnumerable<MovieDomainModel>>()
            {
                DomainModel = null,
                ErrorMessage = Messages.MOVIE_GET_ALL_MOVIES_ERROR,
                IsSuccessful = false
            };
            Task<ResponseModel<IEnumerable<MovieDomainModel>>> dbReturn = Task.FromResult((ResponseModel<IEnumerable<MovieDomainModel>>)response);
            _mockMoviesService.Setup(x => x.GetMoviesByAuditoriumId(1)).Returns(dbReturn);

            //Act
            var resultAction = _moviesController.GetMoviesByAuditoriumId(1).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            var model = (ErrorResponseModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(Messages.MOVIE_GET_ALL_MOVIES_ERROR, model.ErrorMessage);
            Assert.IsInstanceOfType(model, typeof(ErrorResponseModel));
        }

        [TestMethod]
        public void GetMoviesByTagName_ReturnListOfMovies()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            List <MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel { TagName = "Tag1" });
            movieDomainModelsList.Add(_movieDomainModel);
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetByTag(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _moviesController.GetByTagName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var movieDomainModelListResult = ((List<MovieDomainModel>)objectResult);
            var actualCount = movieDomainModelListResult.Count;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedCount, actualCount);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetMoviesByTagName_ReturnNewList()
        {
            //Arrange
            int expectedCount = 0;
            int expectedStatusCode = 200;
            List<MovieDomainModel> movieDomainModelsList = null;
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetByTag(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _moviesController.GetByTagName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var movieDomainModelListResult = ((List<MovieDomainModel>)objectResult);
            var actualCount = movieDomainModelListResult.Count;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedCount, actualCount);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetMoviesByTitle_ReturnListOfMovies()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            List<MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel { TagName = "Tag1" });
            movieDomainModelsList.Add(_movieDomainModel);
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetByTitle(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _moviesController.GetByTitle(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var movieDomainModelListResult = ((List<MovieDomainModel>)objectResult);
            var actualCount = movieDomainModelListResult.Count;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedCount, actualCount);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetMoviesByTitle_ReturnNewList()
        {
            //Arrange
            int expectedCount = 0;
            int expectedStatusCode = 200;
            List<MovieDomainModel> movieDomainModelsList = null;
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetByTitle(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _moviesController.GetByTitle(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var movieDomainModelListResult = ((List<MovieDomainModel>)objectResult);
            var actualCount = movieDomainModelListResult.Count;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedCount, actualCount);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public void GetMoviesByYear_ReturnListOfMovies()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            List<MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            _movieDomainModel.Tags = new List<TagDomainModel>();
            _movieDomainModel.Tags.Add(new TagDomainModel { TagName = "Tag1" });
            movieDomainModelsList.Add(_movieDomainModel);
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetByYear(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _moviesController.GetByYear(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var movieDomainModelListResult = ((List<MovieDomainModel>)objectResult);
            var actualCount = movieDomainModelListResult.Count;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedCount, actualCount);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetMoviesByYear_ReturnNewList()
        {
            //Arrange
            int expectedCount = 0;
            int expectedStatusCode = 200;
            List<MovieDomainModel> movieDomainModelsList = null;
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _mockMoviesService.Setup(x => x.GetByYear(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _moviesController.GetByYear(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var movieDomainModelListResult = ((List<MovieDomainModel>)objectResult);
            var actualCount = movieDomainModelListResult.Count;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(expectedCount, actualCount);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}
