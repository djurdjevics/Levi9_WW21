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
    public class TagsControllerTests
    {
        Mock<ITagService> _mockTagService;
        TagsController _tagsController;
        TagDomainModel _tagDomainModel;
        [TestInitialize]
        public void TestInit()
        {
            _mockTagService = new Mock<ITagService>();
            _tagsController = new TagsController(_mockTagService.Object);
            _tagDomainModel = new TagDomainModel
            {
                Id = Guid.NewGuid(),
                TagName = "Tag1"
            };
        }

        [TestMethod]
        public void TagsController_GetAll_Returns_TagDomainModelList()
        {
            //Arrange
            int expectedCount = 1;
            int expectedStatusCode = 200;
            List<TagDomainModel> tagDomainModelList = new List<TagDomainModel>();
            tagDomainModelList.Add(_tagDomainModel);
            IEnumerable<TagDomainModel> tagDomainModels = tagDomainModelList;
            Task<IEnumerable<TagDomainModel>> responseTask = Task.FromResult(tagDomainModels);
            _mockTagService.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var result = _tagsController.GetAll().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((OkObjectResult)result).Value;
            var ListObjectResult = ((List<TagDomainModel>)resultObject);
            var actualCount = ListObjectResult.Count;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(actualCount, expectedCount);
            Assert.AreEqual(actualStatusCode, expectedStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void TagsController_GetAll_Returns_EmptyTagDomainModelList()
        {
            //Arrange
            int expectedCount = 0;
            int expectedStatusCode = 200;
            List<TagDomainModel> tagDomainModelList = new List<TagDomainModel>();
            IEnumerable<TagDomainModel> tagDomainModels = tagDomainModelList;
            Task<IEnumerable<TagDomainModel>> responseTask = Task.FromResult(tagDomainModels);
            _mockTagService.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var result = _tagsController.GetAll().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((OkObjectResult)result).Value;
            var ListObjectResult = ((List<TagDomainModel>)resultObject);
            var actualCount = ListObjectResult.Count;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(actualCount, expectedCount);
            Assert.AreEqual(actualStatusCode, expectedStatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void TagsController_GetById_Returns_TagDomainModel()
        {
            //Arrange
            int expectedStatusCode = 200;
            ResponseModel<TagDomainModel> responseModel = new ResponseModel<TagDomainModel>
            {
                DomainModel = _tagDomainModel,
                IsSuccessful = true
            };
            Task<ResponseModel<TagDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockTagService.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result =_tagsController.GetById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((OkObjectResult)result).Value;
            var resultDomainModel = (TagDomainModel)resultObject;
            var actualStatusCode = ((OkObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(responseModel.DomainModel.Id, resultDomainModel.Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
        }

        [TestMethod]
        public void TagsController_GetById_Returns_TagNotFound()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedMessage = "Tag doesn't exist!";
            ResponseModel<TagDomainModel> responseModel = new ResponseModel<TagDomainModel>
            {
                DomainModel = null,
                IsSuccessful = false,
                ErrorMessage = "Tag doesn't exist!"
            };
            Task<ResponseModel<TagDomainModel>> responseTask = Task.FromResult(responseModel);
            _mockTagService.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _tagsController.GetById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultObject = ((BadRequestObjectResult)result).Value;
            var resultErrorMessage = (string)resultObject;
            var actualStatusCode = ((BadRequestObjectResult)result).StatusCode;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultErrorMessage, expectedMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
        }
    }
}
