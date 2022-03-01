using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class MoviesServiceTests
    {
        private Mock<IMoviesRepository> _moviesRepo;
        private Mock<IProjectionsRepository> _projectionsRepo;
        private Mock<ITagRepository> _tagRepo;
        private Mock<ITagService> _tagService;
        private Mock<IMovieWithTagRepository> _tagWithMovieRepo;
        private MovieService _movieService;
        private MovieDomainModel _movieDomainModel;
        private Movie _movie;

        [TestInitialize]
        public void InitalizeTest()
        {
            _moviesRepo = new Mock<IMoviesRepository>();
            _projectionsRepo = new Mock<IProjectionsRepository>();
            _tagRepo = new Mock<ITagRepository>();
            _tagService = new Mock<ITagService>();
            _tagWithMovieRepo = new Mock<IMovieWithTagRepository>();
            _movieService = new MovieService(_moviesRepo.Object, _projectionsRepo.Object, _tagService.Object, _tagWithMovieRepo.Object, _tagRepo.Object);

            _movieDomainModel = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Current = true,
                HasOscar = false,
                Rating = 9,
                Title = "ImeFilma",
                Year = 2005
            };

            _movie = new Movie
            {
                Id = Guid.NewGuid(),
                Current = true,
                HasOscar = false,
                Rating = 9,
                Title = "ImeFilma",
                Year = 2005
            };
        }

        [TestMethod]

        public void GetCurrentMovies_Returns_MovieDomainList()
        {
            //Arrange
            List<Movie> moviesList = new List<Movie>();
            moviesList.Add(_movie);
            IEnumerable<Movie> movies = moviesList;
            _moviesRepo.Setup(x => x.GetCurrentMovies()).Returns(movies);
            int expectedCount = 1;

            //Act
            var result = _movieService.GetCurrentMovies(true);
            var resultResponse = ((List<MovieDomainModel>)result);

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(resultResponse.Count, expectedCount);
            Assert.AreEqual(resultResponse[0].Id, moviesList[0].Id);
        }

        [TestMethod]
        public void GetCurrentMovies_ReturnsNull()
        {
            //Arrange
            IEnumerable<Movie> movies = null;
            _moviesRepo.Setup(x => x.GetCurrentMovies()).Returns(movies);

            //Act
            var result = _movieService.GetCurrentMovies(true);
            var resultResponse = ((List<MovieDomainModel>)result);

            //Assert
            Assert.IsNull(resultResponse);
        }

        [TestMethod]
        public void GetMovieById_Returns_MovieDomainModel()
        {
            //Arrange
            Task<Movie> responseTask = Task.FromResult(_movie);
            _moviesRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _movieService.GetMovieByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((ResponseModel<MovieDomainModel>)result);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultObject.DomainModel.Id, _movie.Id);
        }


        [TestMethod]
        public void GetMovieById_Returns_Null()
        {
            //Arrange
            _movie = null;
            Task<Movie> responseTask = Task.FromResult(_movie);
            _moviesRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            var expectedMessage = "Movie doesn't exists!";

            //Act
            var result = _movieService.GetMovieByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((ResponseModel<MovieDomainModel>)result);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultObject.ErrorMessage, expectedMessage);
            Assert.AreEqual(resultObject.IsSuccessful, false);
        }

        [TestMethod]
        public void AddMovie_Returns_CreatedMovieDomainModel()
        {
            //Arrange
            var expectedCountListOfTags = 1;

            MovieWithTag movieWithTag = new MovieWithTag
            {
                MovieId = Guid.NewGuid(),
                TagId = Guid.NewGuid()
            };

            Tag tag = new Tag
            {
                Id = Guid.NewGuid(),
                TagName = "Tag1"
            };

            TagDomainModel tagDomainModel = new TagDomainModel
            {
                Id = Guid.NewGuid(),
                TagName = "Tag1"
            };

            List<TagDomainModel> tagDomainModels = new List<TagDomainModel>();
            tagDomainModels.Add(tagDomainModel);

            Task<Tag> responseTag = Task.FromResult(tag);

            _moviesRepo.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
            _moviesRepo.Setup(x => x.Save());
            _tagRepo.Setup(x => x.SearchByName(It.IsAny<string>())).Returns(responseTag);
            _tagWithMovieRepo.Setup(x => x.Insert(It.IsAny<MovieWithTag>())).Returns(movieWithTag);
            _tagWithMovieRepo.Setup(x => x.Save());

            //Act
            var result = _movieService.AddMovie(new MovieDomainModel { Tags = tagDomainModels }).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((ResponseModel<MovieDomainModel>)result);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(resultObject.DomainModel.Id, _movie.Id);
            Assert.AreEqual(resultObject.IsSuccessful, true);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.AreEqual(result.DomainModel.Tags.Count, expectedCountListOfTags);
        }

        [TestMethod]

        public void AddMovie_Returns_CreatedMovieDomainModelWithCreatedTag()
        {
            //Arrange
            var expectedCountListOfTags = 1;

            MovieWithTag movieWithTag = new MovieWithTag
            {
                MovieId = Guid.NewGuid(),
                TagId = Guid.NewGuid()
            };

            Tag tag = null;

            TagDomainModel tagDomainModel = new TagDomainModel
            {
                Id = Guid.NewGuid(),
                TagName = "Tag1"
            };

            Tag createdTag = new Tag
            {
                Id = tagDomainModel.Id,
                TagName = tagDomainModel.TagName
            };

            List<TagDomainModel> tagDomainModels = new List<TagDomainModel>();
            tagDomainModels.Add(tagDomainModel);

            Task<Tag> responseTag = Task.FromResult(tag);

            _moviesRepo.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
            _moviesRepo.Setup(x => x.Save());
            _tagRepo.Setup(x => x.SearchByName(It.IsAny<string>())).Returns(responseTag);
            _tagRepo.Setup(x => x.Insert(It.IsAny<Tag>())).Returns(createdTag);
            _tagWithMovieRepo.Setup(x => x.Insert(It.IsAny<MovieWithTag>())).Returns(movieWithTag);
            _tagWithMovieRepo.Setup(x => x.Save());

            //Act
            var result = _movieService.AddMovie(new MovieDomainModel { Tags = tagDomainModels }).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((ResponseModel<MovieDomainModel>)result);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(resultObject.DomainModel.Id, _movie.Id);
            Assert.AreEqual(resultObject.IsSuccessful, true);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.AreEqual(result.DomainModel.Tags.Count, expectedCountListOfTags);
            Assert.AreEqual(result.DomainModel.Tags[0].TagName, createdTag.TagName);
        }

        [TestMethod]
        public void AddMovie_Returns_NullObject()
        {
            //Arrange
            _movie = null;
            _moviesRepo.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
            var expectedMessage = "Failed to add movie!";

            //Act
            var result = _movieService.AddMovie(new MovieDomainModel { Tags = new List<TagDomainModel>()}).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ResponseModel<MovieDomainModel>)result);

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.IsNull(objectResult.DomainModel);
            Assert.AreEqual(objectResult.IsSuccessful, false);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.AreEqual(expectedMessage, objectResult.ErrorMessage);
        }


        [TestMethod]
        public void DeleteMovie_Returns_UnSuccessfulResponseModel()
        {
            //Arrange
            Projection _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };

            string expectedMessage = "Movie has projections in future!";
            List<Projection> projections = new List<Projection>();
            projections.Add(_projection);

            _projectionsRepo.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(projections);

            //Act

            var result = _movieService.DeleteMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultModel = (ResponseModel<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.AreEqual(resultModel.ErrorMessage, expectedMessage);
        }

        [TestMethod]
        public void DeleteMovie_Returns_DeletedMovieDomainModel()
        {
            //Arrange
            Projection _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 1
            };

            List<Projection> projections = new List<Projection>();
            projections.Add(_projection);

            _projectionsRepo.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(projections);
            _moviesRepo.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_movie);
            _moviesRepo.Setup(x => x.Save());

            //Act

            var result = _movieService.DeleteMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultModel = (ResponseModel<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.AreEqual(resultModel.DomainModel.Id, _movie.Id);
        }

        [TestMethod]
        public void DeleteMovie_Returns_Null()
        {
            //Arrange
            Projection _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 1
            };


            List<Projection> projections = new List<Projection>();
            projections.Add(_projection);
            _movie = null;

            _projectionsRepo.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(projections);
            _moviesRepo.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_movie);
            _moviesRepo.Setup(x => x.Save());

            //Act

            var result = _movieService.DeleteMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod] 

        public void UpdateMovie_Returns_UpdatedMovieDomainModel()
        {
            //Arrange
            Movie updatedMovie = new Movie
            {
                Id = Guid.NewGuid(),
                Current = true,
                HasOscar = false,
                Rating = 10,
                Title = "ImeFilma_edit",
                Year = 2010
            };
            Task<Movie> responseTask = Task.FromResult(_movie);
            _moviesRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            _moviesRepo.Setup(x => x.Update(It.IsAny<Movie>())).Returns(updatedMovie);
            _moviesRepo.Setup(x => x.Save());

            //Act
            var result = _movieService.UpdateMovie(new MovieDomainModel { Tags = new List<TagDomainModel>() }).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (ResponseModel<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsNotNull(resultObject.DomainModel);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.AreEqual(updatedMovie.Id, resultObject.DomainModel.Id);
        }

        [TestMethod]
        public void UpdateMovie_Returns_UnsuccessfulResponseModel()
        {
            //Arrange
            var expectedMessage = "Failed to find movie to update!";
            _movie = null;
            _moviesRepo.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movie);
            _moviesRepo.Setup(x => x.Save());

            //Act
            var result = _movieService.UpdateMovie(new MovieDomainModel { Tags = new List<TagDomainModel>() }).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (ResponseModel<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsNull(resultObject.DomainModel);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.IsFalse(resultObject.IsSuccessful);
            Assert.AreEqual(expectedMessage, resultObject.ErrorMessage);
        }

        [TestMethod]
        public void GetMoviesByTag_Returns_MovieDomainModelList()
        {
            //Arrange
            List<MovieWithTag> movieWithTagsList = new List<MovieWithTag>();
            movieWithTagsList.Add(new MovieWithTag
            {
                MovieId = _movie.Id,
                TagId = Guid.NewGuid(),
                Movie = _movie,
                Tag = new Tag { TagName = "Tag1"}
            });

            var expectedCount = 1;
            IEnumerable <MovieWithTag> movieWithTags = movieWithTagsList;
            Task<IEnumerable<MovieWithTag>> responseTask = Task.FromResult(movieWithTags);
            _tagWithMovieRepo.Setup(x => x.GetByTagName(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _movieService.GetByTag(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (List<MovieDomainModel>)result;

            //Assert
            Assert.IsInstanceOfType(result,typeof(List<MovieDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultObject.Count, expectedCount);
            Assert.AreEqual(resultObject[0].Id, movieWithTagsList[0].MovieId);
            Assert.AreEqual(resultObject[0].Tags[0].TagName, movieWithTagsList[0].Tag.TagName);
        }

        [TestMethod]
        public void GetMoviesByTitle_Returns_MovieDomainModelList()
        {
            //Arrange

            List<Movie> moviesList = new List<Movie>();
            moviesList.Add(_movie);
            IEnumerable<Movie> movies = moviesList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

            var expectedCount = 1;
            _moviesRepo.Setup(x => x.GetByTitle(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _movieService.GetByTitle(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (List<MovieDomainModel>)result;

            //Assert
            Assert.IsInstanceOfType(result, typeof(List<MovieDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultObject.Count, expectedCount);
            Assert.AreEqual(resultObject[0].Id, moviesList[0].Id);
        }

        [TestMethod]
        public void GetMoviesByYear_Returns_MovieDomainModelList()
        {
            //Arrange

            List<Movie> moviesList = new List<Movie>();
            moviesList.Add(_movie);
            IEnumerable<Movie> movies = moviesList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);

            var expectedCount = 1;
            _moviesRepo.Setup(x => x.GetByYear(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _movieService.GetByYear(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (List<MovieDomainModel>)result;

            //Assert
            Assert.IsInstanceOfType(result, typeof(List<MovieDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultObject.Count, expectedCount);
            Assert.AreEqual(resultObject[0].Id, moviesList[0].Id);
        }


        [TestMethod]

        public void GetTopTenMovies_Returns_MovieDomainList()
        {
            //Act
            IEnumerable<Movie> movies;
            List<Movie> moviesList = new List<Movie>();
            moviesList.Add(_movie);
            movies = moviesList;
            int expectedCount = 1;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _moviesRepo.Setup(x => x.GetTopTenMovies()).Returns(responseTask);

            //Arrange
            var result = _movieService.GetTopTenMovies().ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (List<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(resultResponse.Count, expectedCount);
            Assert.AreEqual(resultResponse[0].Id, _movie.Id);
            Assert.IsInstanceOfType(resultResponse, typeof(List<MovieDomainModel>));

        }

        [TestMethod]
        public void GetTopTenMoviesByYear_Returns_MovieDomainList()
        {
            //Act
            int expectedCount = 1;
            List<Movie> moviesList = new List<Movie>();
            moviesList.Add(_movie);
            IEnumerable<Movie> movies = moviesList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _moviesRepo.Setup(x => x.GetTopTenMoviesByYear(It.IsAny<int>())).Returns(responseTask);

            //Arrange
            var result = _movieService.GetTopTenMoviesByYear(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (List<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, resultResponse.Count);
            Assert.AreEqual(resultResponse[0].Id, _movie.Id);
            Assert.IsInstanceOfType(resultResponse, typeof(List<MovieDomainModel>));
            Assert.IsInstanceOfType(resultResponse[0], typeof(MovieDomainModel));
        }

        [TestMethod]
        public void ActivateDeactivateMovie_Return_UnsuccessfulResponseModel()
        {
            //Act
            Projection _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };

            string expectedMessage = "Movie has projections in future!";
            List<Projection> projections = new List<Projection>();
            projections.Add(_projection);

            Task<Movie> responseTask = Task.FromResult(_movie);
            _moviesRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            _projectionsRepo.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(projections);

            //Arrange

            var result = _movieService.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultModel = (ResponseModel<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.AreEqual(resultModel.ErrorMessage, expectedMessage);
        }

        [TestMethod]
        public void ActivateDeactivateMovie_Returns_Null()
        {
            //Act
            Projection _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 1
            };

            List<Projection> projections = new List<Projection>();
            projections.Add(_projection);

            _movie = null;
            string expectedMessage = "Movie doesn't exist!";
            Task<Movie> responseTask = Task.FromResult(_movie);

            _moviesRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            _projectionsRepo.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(projections);
            //Arrange
            var result = _movieService.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultModel = (ResponseModel<MovieDomainModel>)result;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, resultModel.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
        }

        [TestMethod]

        public void ActivateDeactivateMovie_Returns_SuccessfulResponseModel()
        {
            //Act
            Projection _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 1
            };

            bool expectedState = !_movie.Current;
            List<Projection> projections = new List<Projection>();
            projections.Add(_projection);
            Task<Movie> responseTask = Task.FromResult(_movie);
            _moviesRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            _projectionsRepo.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(projections);
            _moviesRepo.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movie);
            _moviesRepo.Setup(x => x.Save());

            //Arrange
            var result = _movieService.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<MovieDomainModel>));
            Assert.AreEqual(result.DomainModel.Current, expectedState);
        }

        [TestMethod]
        public void MovieService_GetMoviesByAuditoriumId_ReturnMovies()
        {
            //Arrange
            var _projection = new Projection() { Movie = _movie, AuditoriumId = 1, Id = Guid.NewGuid(), MovieId = Guid.NewGuid(), ProjectionTime = DateTime.Now };
            var projections = new List<Projection>() { _projection };
            _projectionsRepo.Setup(x => x.GetByAuditoriumId(1)).Returns(projections);

            //Act
            var result = _movieService.GetMoviesByAuditoriumId(1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<IEnumerable<MovieDomainModel>>));
            Assert.AreEqual(_projection.Movie.Title, result.DomainModel.ToList()[0].Title);
        }

        [TestMethod]
        public void MovieService_GetMoviesByAuditoriumId_ReturnErrorModel()
        {
            //Arrange
            List<Projection> projections = null;
            _projectionsRepo.Setup(x => x.GetByAuditoriumId(1)).Returns(projections);

            //Act
            var result = _movieService.GetMoviesByAuditoriumId(1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.DomainModel);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Messages.PROJECTION_GET_ALL_PROJECTIONS_ERROR, result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<IEnumerable<MovieDomainModel>>));
        }
    }
}
