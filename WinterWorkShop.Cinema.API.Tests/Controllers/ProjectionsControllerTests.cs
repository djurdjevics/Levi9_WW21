using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class ProjectionsControllerTests
    {
        private Mock<IProjectionService> _projectionService;
        private ProjectionDomainModel _projectionDomainModel;
        private ProjectionsController projectionsController;

        [TestInitialize]

        public void TestInit()
        {
            _projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };
            _projectionService = new Mock<IProjectionService>();
            projectionsController = new ProjectionsController(_projectionService.Object);
        }

        [TestMethod]
        public void GetAsync_Return_All_Projections()
        {
            //Arrange
            List<ProjectionDomainModel> projectionsDomainModelsList = new List<ProjectionDomainModel>();
            projectionsDomainModelsList.Add(_projectionDomainModel);
            IEnumerable<ProjectionDomainModel> projectionDomainModels = projectionsDomainModelsList;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            _projectionService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = projectionsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.AreEqual(_projectionDomainModel.Id, projectionDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<ProjectionDomainModel> projectionDomainModels = null;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _projectionService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = projectionsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - false
        // return Created
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_True_Projection()
        {
            //Arrange
            int expectedStatusCode = 201;


            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            CreateProjectionResultModel createProjectionResultModel = new CreateProjectionResultModel
            {

                IsSuccessful = true,
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    MovieId = createProjectionModel.MovieId,
                    AuditoriumId = createProjectionModel.AuditoriumId,
                    ProjectionTime = createProjectionModel.ProjectionTime,
                    AuditoriumName = "Auditorium1",
                    MovieTitle = "FIlm1"
                }
            };
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);

            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;
            var projectionDomainModel = (ProjectionDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(projectionDomainModel);
            Assert.AreEqual(createProjectionModel.MovieId, projectionDomainModel.MovieId);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - throw DbUpdateException
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Projection()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            CreateProjectionResultModel createProjectionResultModel = new CreateProjectionResultModel
            {
                Projection = _projectionDomainModel,
                IsSuccessful = true
            };
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Throws(dbUpdateException);
            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }


        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_False_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new projection, please try again.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            CreateProjectionResultModel createProjectionResultModel = new CreateProjectionResultModel
            {
                Projection = _projectionDomainModel,
                IsSuccessful = false,
                ErrorMessage = Messages.PROJECTION_CREATION_ERROR,
            };
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);

            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        // if (!ModelState.IsValid) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 0
            };

            projectionsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ProjectionDate_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Projection time cannot be in past.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 0
            };

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
         //  var errorResponse = ((SerializableError)createdResult).GetValueOrDefault(nameof(createProjectionModel.ProjectionTime));
            var message = (ErrorResponseModel)createdResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void DeleteProjection_Returns_OkObjectResult()
        {
            //Act
            ResponseModel<ProjectionDomainModel> responseModel = new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = true,
                DomainModel = _projectionDomainModel
            };
            int expectedStatusCode = 200;

            Task<ResponseModel<ProjectionDomainModel>> responseTask = Task.FromResult(responseModel);
            _projectionService.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(responseTask);

            //Arrange
            var result = projectionsController.DeleteById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;
            var resultResponse = ((OkObjectResult)result).Value;
            var projectionDomainModelResult = (ProjectionDomainModel)resultResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actualStatusCode, expectedStatusCode);
            Assert.AreEqual(responseModel.DomainModel.Id, projectionDomainModelResult.Id);
        }

        [TestMethod]
        public void DeleteProjection_Returns_BadRequest()
        {
            //Act
            ResponseModel<ProjectionDomainModel> responseModel = new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = false,
                DomainModel = null,
                ErrorMessage = "Projection doesn't exist!"
            };
            Task<ResponseModel<ProjectionDomainModel>> responseTask = Task.FromResult(responseModel);
            string expectedErrorMessage = "Projection doesn't exist!";
            int expectedStatusCode = 400;
            _projectionService.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(responseTask);

            //Arrange
            var result = projectionsController.DeleteById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultErrorResponse = ((BadRequestObjectResult)result).Value;
            var actualStatusCode = ((BadRequestObjectResult)result).StatusCode;
            var projectionErrorResponseResult = (string)resultErrorResponse;

            //Assert

            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.AreEqual(expectedErrorMessage, projectionErrorResponseResult);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void UpdateProjection_Returns_UpdatedProjection()
        {

            //Act
            ResponseModel<ProjectionDomainModel> responseModel = new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = true,
                DomainModel = _projectionDomainModel
            };

            ResponseModel<ProjectionDomainModel> updatedResponseModel = new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = true,
                DomainModel = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AuditoriumId = new int(),
                    AuditoriumName = "Auditorium2",
                    MovieId = Guid.NewGuid(),
                    MovieTitle = "Film2",
                    ProjectionTime = DateTime.Now.AddDays(1)
                }
            };

            Task<ResponseModel<ProjectionDomainModel>> taskResponse = Task.FromResult(responseModel);
            Task<ResponseModel<ProjectionDomainModel>> taskResponse2 = Task.FromResult(updatedResponseModel);

            _projectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(taskResponse);
            _projectionService.Setup(x => x.Update(It.IsAny<ProjectionDomainModel>())).Returns(taskResponse2);
            int expectedStatusCode = 200;

            //Arrange
            var resultt = projectionsController.Update(It.IsAny<Guid>(), new ProjectionDomainModel() { AuditoriumName = "Auditorium2", ProjectionTime = DateTime.Now.AddDays(1)}).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((OkObjectResult)resultt).Value;
            var actualStatusCode = ((OkObjectResult)resultt).StatusCode;
            var resultDomainModel = ((ProjectionDomainModel)resultObject);

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultDomainModel.Id, updatedResponseModel.DomainModel.Id);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(resultt, typeof(OkObjectResult));
        }

        [TestMethod]
        public void UpdateProjection_Returns_ProjectionNotFound()
        {
            //Arrange
            ResponseModel<ProjectionDomainModel> responseModel = new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = false,
                ErrorMessage = "Projection doesn't exist!",
                DomainModel = null
            };
            int expectedStatusCode = 400;
            var projectionDomainModelReq = new ProjectionDomainModel()
            {
                ProjectionTime = DateTime.Now.AddDays(1)
            };
            Task<ResponseModel<ProjectionDomainModel>> responseTask = Task.FromResult(responseModel);
            _projectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(responseTask);
            //Act
            var result = projectionsController.Update(It.IsAny<Guid>(), projectionDomainModelReq).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)objectResult);

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(errorResponseResult.ErrorMessage, responseModel.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, (int)errorResponseResult.StatusCode);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void UpdateProjection_Returns_FailedUpdate()
        {
            ResponseModel<ProjectionDomainModel> responseModel = new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = true,
                DomainModel = _projectionDomainModel
            };

            ResponseModel<ProjectionDomainModel> updatedResponseModel = new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = false,
                DomainModel = null,
                ErrorMessage = "Failed to update projection!"

            };

            Task<ResponseModel<ProjectionDomainModel>> taskResponse = Task.FromResult(responseModel);
            Task<ResponseModel<ProjectionDomainModel>> taskResponse2 = Task.FromResult(updatedResponseModel);
            _projectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(taskResponse);

            _projectionService.Setup(x => x.Update(It.IsAny<ProjectionDomainModel>())).Returns(taskResponse2);
            int expectedStatusCode = 400;

            //Arrange
            var resultt = projectionsController.Update(It.IsAny<Guid>(), new ProjectionDomainModel() { AuditoriumName = "Auditorium2", ProjectionTime=DateTime.Now.AddDays(1) }).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((BadRequestObjectResult)resultt).Value;
            var actualStatusCode = ((BadRequestObjectResult)resultt).StatusCode;
            var resultString = ((ErrorResponseModel)resultObject);

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultString.ErrorMessage, updatedResponseModel.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(resultt, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void UpdateProjection_Throws_DbException()
        {
            //Act
            ResponseModel<ProjectionDomainModel> errorResponseModel = new ResponseModel<ProjectionDomainModel>
            {
                DomainModel = _projectionDomainModel,
                IsSuccessful = true
            };


            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            ResponseModel<ProjectionDomainModel> responseModel = new ResponseModel<ProjectionDomainModel>
            {
                DomainModel = _projectionDomainModel,
                ErrorMessage = dbUpdateException.InnerException.Message,
                IsSuccessful = false
            };
            Task<ResponseModel<ProjectionDomainModel>> getByIdResponseTask = Task.FromResult(errorResponseModel);
            Task<ResponseModel<ProjectionDomainModel>> responseTask = Task.FromResult(responseModel);
            _projectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(getByIdResponseTask);
            _projectionService.Setup(x => x.Update(_projectionDomainModel)).Throws(dbUpdateException);

            //Arrange

            var result = projectionsController.Update(It.IsAny<Guid>(), new ProjectionDomainModel() { ProjectionTime = DateTime.Now.AddDays(1)}).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
        public void ProjectionsController_Filter_ReturnFilteredProjections()
        {
            //Arrange
            var model = new ResponseModel<IEnumerable<ProjectionDomainModel>>()
            {
                DomainModel = new List<ProjectionDomainModel>() { _projectionDomainModel },
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<IEnumerable<ProjectionDomainModel>>> modelResponse = Task.FromResult((ResponseModel<IEnumerable<ProjectionDomainModel>>)model);
            _projectionService.Setup(x => x.GetProjectionsFiltered(It.IsAny<FilterProjectionsModel>())).Returns(modelResponse);

            //Act
            var resultAction = projectionsController.Filter(It.IsAny<FilterProjectionsModel>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (OkObjectResult)resultAction;
            var objectResult = ((OkObjectResult)resultAction).Value;
            var returnedModel = (List<ProjectionDomainModel>)objectResult;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_projectionDomainModel.Id, returnedModel[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void ProjectionsController_Filter_ReturnErrorModel()
        {
            //Arrange
            var model = new ResponseModel<IEnumerable<ProjectionDomainModel>>()
            {
                DomainModel = null,
                ErrorMessage = Messages.PROJECTION_GET_ALL_PROJECTIONS_ERROR,
                IsSuccessful = false
            };
            Task<ResponseModel<IEnumerable<ProjectionDomainModel>>> modelResponse = Task.FromResult((ResponseModel<IEnumerable<ProjectionDomainModel>>)model);
            _projectionService.Setup(x => x.GetProjectionsFiltered(It.IsAny<FilterProjectionsModel>())).Returns(modelResponse);

            //Act
            var resultAction = projectionsController.Filter(It.IsAny<FilterProjectionsModel>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (BadRequestObjectResult)resultAction;
            var objectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorModel = (ErrorResponseModel)objectResult;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(model.ErrorMessage, errorModel.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void ProjectionsController_GetById_ReturnProjection()
        {
            //Arrange
            var model = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = _projectionDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<ProjectionDomainModel>> modelResponse = Task.FromResult((ResponseModel<ProjectionDomainModel>)model);
            _projectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(modelResponse);

            //Act
            var resultAction = projectionsController.GetById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (OkObjectResult)resultAction;
            var objectResult = ((OkObjectResult)resultAction).Value;
            var returnedModel = (ProjectionDomainModel)objectResult;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_projectionDomainModel.Id, returnedModel.Id);
            Assert.AreEqual(_projectionDomainModel.AuditoriumId, returnedModel.AuditoriumId);
            Assert.AreEqual(_projectionDomainModel.MovieId, returnedModel.MovieId);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void ProjectionsController_GetById_ReturnErrorResponse()
        {
            //Arrange
            var model = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = null,
                ErrorMessage = Messages.PROJECTION_GET_ALL_PROJECTIONS_ERROR,
                IsSuccessful = false
            };
            Task<ResponseModel<ProjectionDomainModel>> modelResponse = Task.FromResult((ResponseModel<ProjectionDomainModel>)model);
            _projectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(modelResponse);

            //Act
            var resultAction = projectionsController.GetById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = (BadRequestObjectResult)resultAction;
            var objectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorModel = (ErrorResponseModel)objectResult;
            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(model.ErrorMessage, errorModel.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
