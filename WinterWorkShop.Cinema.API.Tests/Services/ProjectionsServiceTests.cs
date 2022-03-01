using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class ProjectionsServiceTests
    {
        private Mock<IProjectionsRepository> _mockProjectionsRepository;
        private Mock<IAuditoriumService> _mockAuditoriumService;
        private Projection _projection;
        private ProjectionDomainModel _projectionDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                ProjectionTime = new DateTime(2021, 2, 21),
                AuditoriumId = 1
            };

            _projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = new DateTime(2021, 2, 21),
            };

            List<Projection> projectionsModelsList = new List<Projection>();

            projectionsModelsList.Add(_projection);
            IEnumerable<Projection> projections = projectionsModelsList;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            _mockAuditoriumService = new Mock<IAuditoriumService>();
        }

        [TestMethod]
        public void ProjectionService_GetAllAsync_ReturnListOfProjecrions()
        {
            //Arrange
            int expectedResultCount = 1;
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = projectionsController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_projection.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(ProjectionDomainModel));
        }

        [TestMethod]
        public void ProjectionService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Projection> projections = null;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = projectionsController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return list with projections
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - true
        // return ErrorMessage
        [TestMethod]
        public void ProjectionService_CreateProjection_WithProjectionAtSameTime_ReturnErrorMessage()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();
            projectionsModelsList.Add(_projection);
            string expectedMessage = "Cannot create new projection, there are projections at same time alredy.";

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return null
        //  if (insertedProjection == null) - true
        // return CreateProjectionResultModel  with errorMessage
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();
            _projection = null;
            string expectedMessage = "Error occured while creating new projection, please try again.";

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(_projection);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return valid EntityEntry<Projection>
        //  if (insertedProjection == null) - false
        // return valid projection 
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMocked_ReturnProjection()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(_projection);
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_projection.Id, resultAction.Projection.Id);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void Projectionervice_CreateProjection_When_Updating_Non_Existing_Movie()
        {
            // Arrange
            List<Projection> projectionsModelsList = new List<Projection>();

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Throws(new DbUpdateException());
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ProjectionService_UpdateProjection_UpdateMocked_ReturnProjection()
        {
            //Arrange
            _mockProjectionsRepository.Setup(x => x.Update(It.IsAny<Projection>())).Returns(_projection);
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = projectionService.Update(new ProjectionDomainModel()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (ResponseModel<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<ProjectionDomainModel>));
            Assert.AreEqual(resultAction.DomainModel.Id, _projection.Id);
        }
        //[TestMethod]
        //public void ProjectionService_UpdateProjection_UpdateMocked_ReturnErrorResponse()
        //{
        //    //Arrange
        //    var listofProjections = new List<Projection>() { _projection };
        //    _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(listofProjections);
        //    _mockProjectionsRepository.Setup(x => x.Update(It.IsAny<Projection>())).Returns(_projection);
        //    _mockProjectionsRepository.Setup(x => x.Save());
        //    ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

        //    //Act
        //    var resultAction = projectionService.Update(new ProjectionDomainModel() { ProjectionTime=_projection.ProjectionTime}).ConfigureAwait(false).GetAwaiter().GetResult();
        //    var resultObject = (ResponseModel<ProjectionDomainModel>)resultAction;

        //    //Assert
        //    Assert.IsNotNull(resultObject);
        //    Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<ProjectionDomainModel>));
        //    Assert.AreEqual(resultAction.DomainModel.Id, _projection.Id);
        //}


        [TestMethod]
        public void ProjectionService_UpdateProjection_NullMocked_ReturnUnsuccessfulUpdate()
        {
            //Arrange
            _projection = null;
            _mockProjectionsRepository.Setup(x => x.Update(It.IsAny<Projection>())).Returns(_projection);
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            string expectedMessage = "Failed to update projection!";

            //Act
            var resultAction = projectionService.Update(new ProjectionDomainModel()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultobject = (ResponseModel<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(resultobject);
            Assert.IsNull(resultobject.DomainModel);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<ProjectionDomainModel>));
        }

        [TestMethod]

        public void ProjectionService_DeleteProjection_DeleteMocked_Return_Projection()
        {
            //Arrange
            Task<Projection> responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            _mockProjectionsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_projection);
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);

            //Act
            var resultAction = projectionService.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (ResponseModel<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<ProjectionDomainModel>));
            Assert.AreEqual(resultObject.DomainModel.Id, _projection.Id);
        }

        [TestMethod]
        public void ProjectionService_DeleteProjection_NullMocked_ReturnUnsuccessfulDelete()
        {
            //Arrange
            _projection = null;
            Task<Projection> responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            string expectedMessage = "Projection doesn't exist!";

            //Act
            var resultAction = projectionService.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (ResponseModel<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsNull(resultObject.DomainModel);
            Assert.AreEqual(resultObject.ErrorMessage, expectedMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<ProjectionDomainModel>));

        }

        [TestMethod]
        public void ProjectionService_GetProjectionsFiltered_ReturnAllProjections()
        {
            //Arrange
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            var projections = new List<Projection>() { _projection };
            Task<IEnumerable<Projection>> projectionsModels = Task.FromResult((IEnumerable<Projection>)projections);
            var filterModel = new FilterProjectionsModel() { AuditoriumId = 0, CinemaId = 0, MovieId = Guid.Empty };
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(projectionsModels);

            //Act
            var resultAction = projectionService.GetProjectionsFiltered(filterModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(_projection.Id, resultAction.DomainModel.ToList()[0].Id);
            Assert.AreEqual(_projection.MovieId, resultAction.DomainModel.ToList()[0].MovieId);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<IEnumerable<ProjectionDomainModel>>));
        }

        [TestMethod]
        public void ProjectionService_GetProjectionsFiltered_ReturnErrorResponse()
        {
            //Arrange
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            List<Projection> projections = null;
            Task<IEnumerable<Projection>> projectionsModels = Task.FromResult((IEnumerable<Projection>)projections);
            var filterModel = new FilterProjectionsModel() { AuditoriumId = 0, CinemaId = 0, MovieId = Guid.Empty };
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(projectionsModels);

            //Act
            var resultAction = projectionService.GetProjectionsFiltered(filterModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<IEnumerable<ProjectionDomainModel>>));
        }

        [TestMethod]
        public void ProjectionService_GetProjectionsFiltered_ReturnProjectionsFilteredWithCinema()
        {
            //Arrange
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            var projections = new List<Projection>() { _projection };
            Task<IEnumerable<Projection>> projectionsModels = Task.FromResult((IEnumerable<Projection>)projections);
            var filterModel = new FilterProjectionsModel() { AuditoriumId = 0, CinemaId = 1, MovieId = Guid.Empty };
            var auditoriums = new List<AuditoriumDomainModel>() { new AuditoriumDomainModel() { CinemaId = 1, Id = 1, Name = "Nikola Tesla" } };
            Task<IEnumerable<AuditoriumDomainModel>> auditoriumsFiltered = Task.FromResult((IEnumerable<AuditoriumDomainModel>)auditoriums);
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(projectionsModels);
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByCinemaId(filterModel.CinemaId)).Returns(auditoriumsFiltered);
            //Act
            var resultAction = projectionService.GetProjectionsFiltered(filterModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(_projection.Id, resultAction.DomainModel.ToList()[0].Id);
            Assert.AreEqual(_projection.MovieId, resultAction.DomainModel.ToList()[0].MovieId);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<IEnumerable<ProjectionDomainModel>>));
        }

        [TestMethod]
        public void ProjectionService_GetProjectionsFiltered_ReturnProjectionsFilteredWithAuditorium()
        {
            //Arrange
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            var projections = new List<Projection>() { _projection };
            Task<IEnumerable<Projection>> projectionsModels = Task.FromResult((IEnumerable<Projection>)projections);
            var filterModel = new FilterProjectionsModel() { AuditoriumId = 1, CinemaId = 0, MovieId = Guid.Empty };
            var auditoriums = new List<AuditoriumDomainModel>() { new AuditoriumDomainModel() { CinemaId = 1, Id = 1, Name = "Nikola Tesla" } };
            Task<IEnumerable<AuditoriumDomainModel>> auditoriumsFiltered = Task.FromResult((IEnumerable<AuditoriumDomainModel>)auditoriums);
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(projectionsModels);
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByCinemaId(filterModel.CinemaId)).Returns(auditoriumsFiltered);
            //Act
            var resultAction = projectionService.GetProjectionsFiltered(filterModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(_projection.Id, resultAction.DomainModel.ToList()[0].Id);
            Assert.AreEqual(_projection.MovieId, resultAction.DomainModel.ToList()[0].MovieId);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<IEnumerable<ProjectionDomainModel>>));
        }

        [TestMethod]
        public void ProjectionService_GetProjectionsFiltered_ReturnProjectionsFilteredWithMovie()
        {
            //Arrange
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            var projections = new List<Projection>() { _projection };
            Task<IEnumerable<Projection>> projectionsModels = Task.FromResult((IEnumerable<Projection>)projections);
            var filterModel = new FilterProjectionsModel() { AuditoriumId = 0, CinemaId = 0, MovieId = _projection.MovieId };
            var auditoriums = new List<AuditoriumDomainModel>() { new AuditoriumDomainModel() { CinemaId = 1, Id = 1, Name = "Nikola Tesla" } };
            Task<IEnumerable<AuditoriumDomainModel>> auditoriumsFiltered = Task.FromResult((IEnumerable<AuditoriumDomainModel>)auditoriums);
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(projectionsModels);
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByCinemaId(filterModel.CinemaId)).Returns(auditoriumsFiltered);
            //Act
            var resultAction = projectionService.GetProjectionsFiltered(filterModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(_projection.Id, resultAction.DomainModel.ToList()[0].Id);
            Assert.AreEqual(_projection.MovieId, resultAction.DomainModel.ToList()[0].MovieId);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<IEnumerable<ProjectionDomainModel>>));
        }

        [TestMethod]
        public void ProjectionService_GetProjectionsFiltered_ReturnProjectionsFilteredWithDateTime()
        {
            //Arrange
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            var projections = new List<Projection>() { _projection };
            Task<IEnumerable<Projection>> projectionsModels = Task.FromResult((IEnumerable<Projection>)projections);
            var filterModel = new FilterProjectionsModel() { AuditoriumId = 0, CinemaId = 0, MovieId = Guid.Empty, DateTime = new DateTime(2021, 2, 21) };
            var auditoriums = new List<AuditoriumDomainModel>() { new AuditoriumDomainModel() { CinemaId = 1, Id = 1, Name = "Nikola Tesla" } };
            Task<IEnumerable<AuditoriumDomainModel>> auditoriumsFiltered = Task.FromResult((IEnumerable<AuditoriumDomainModel>)auditoriums);
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(projectionsModels);
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByCinemaId(filterModel.CinemaId)).Returns(auditoriumsFiltered);
            //Act
            var resultAction = projectionService.GetProjectionsFiltered(filterModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(_projection.Id, resultAction.DomainModel.ToList()[0].Id);
            Assert.AreEqual(_projection.MovieId, resultAction.DomainModel.ToList()[0].MovieId);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<IEnumerable<ProjectionDomainModel>>));
        }

        [TestMethod]
        public void ProjectionService_GetProjectionById_ReturnProjection()
        {
            //Arrange
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            Task<Projection> proj = Task.FromResult((Projection)_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(proj);

            //Act
            var resultAction = projectionService.GetProjectionById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.AreEqual(_projection.Id, resultAction.DomainModel.Id);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<ProjectionDomainModel>));

        }

        [TestMethod]
        public void ProjectionService_GetProjectionById_ReturnErrorModel()
        {
            //Arrange
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockAuditoriumService.Object);
            Task<Projection> proj = Task.FromResult((Projection)null);
            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(proj);

            //Act
            var resultAction = projectionService.GetProjectionById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsNull(resultAction.DomainModel);
            Assert.AreEqual("Projection doesn't exist!", resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<ProjectionDomainModel>));

        }
    }
}
