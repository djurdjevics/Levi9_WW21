using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WinterWorkShop.Cinema.Repositories;
using Moq;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Domain.Models;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class TagServiceTests
    {
        private Mock<ITagRepository> _mockTagRepository;
        private Tag _tag;
        private TagService _tagService;
        private TagDomainModel _tagDomainModel;

        [TestInitialize]
        public void TestInit()
        {
            _mockTagRepository = new Mock<ITagRepository>();
            _tag = new Tag
            {
                Id = Guid.NewGuid(),
                TagName = "Tag1",
                MovieWithTags = new List<MovieWithTag>()
            };
            _tagService = new TagService(_mockTagRepository.Object);
            _tagDomainModel = new TagDomainModel
            {
                Id = _tag.Id,
                TagName = _tag.TagName
            };
        }

        [TestMethod]
        
        public void TagService_GetAll_ReturnsTagDomainModelList()
        {
            //Arrange
            var expectedCount = 1;
            List<Tag> tagsList = new List<Tag>();
            tagsList.Add(_tag);
            IEnumerable<Tag> tags = tagsList;
            Task<IEnumerable<Tag>> responseTask = Task.FromResult(tags);
            _mockTagRepository.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var result = _tagService.GetAll().ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((List<TagDomainModel>)result);

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultObject[0].Id, tagsList[0].Id);
            Assert.IsInstanceOfType(result, typeof(List<TagDomainModel>));
        }

        [TestMethod]
        public void TagService_GetTagById_Returns_SuccessfulResponseModel()
        {
            //Arrange
            Task<Tag> responseTask = Task.FromResult(_tag);
            _mockTagRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _tagService.GetById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((ResponseModel<TagDomainModel>)result);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ResponseModel<TagDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(resultObject.DomainModel.Id, _tag.Id);
            Assert.IsTrue(resultObject.IsSuccessful);
        }


        [TestMethod]
        public void TagService_GetTagById_Returns_UnSuccessfulResponseModel()
        {
            //Arrange
            var expectedMessage = "Tag doesn't exist!";
            _tag = null;
            Task<Tag> responseTask = Task.FromResult(_tag);
            _mockTagRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _tagService.GetById(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((ResponseModel<TagDomainModel>)result);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ResponseModel<TagDomainModel>));
            Assert.IsNotNull(resultObject);
            Assert.IsNull(resultObject.DomainModel);
            Assert.IsFalse(resultObject.IsSuccessful);
            Assert.AreEqual(expectedMessage, resultObject.ErrorMessage);
        }

        [TestMethod]
        public void TagService_Create_Returns_UnsuccessfulResponseModelTagAlreadyExists()
        {
            //Arrange
            var expectedMessage = "Tag already exists!";
            Task<Tag> responseTask = Task.FromResult(_tag);
            _mockTagRepository.Setup(x => x.SearchByName(It.IsAny<string>())).Returns(responseTask);

            //Act
            var result = _tagService.Create(new TagDomainModel { TagName = It.IsAny<string>() } ).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (ResponseModel<TagDomainModel>)result;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsFalse(resultObject.IsSuccessful);
            Assert.AreEqual(resultObject.ErrorMessage, expectedMessage);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<TagDomainModel>));
        }

        [TestMethod]
        public void TagService_Create_Returns_UnsuccessfulResponseModelFailedToCreate()
        {
            //Arrange
            _tag = null;
            var expectedMessage = "Failed to create tag";
            Tag tagToAdd = null;
            Task<Tag> responseTask = Task.FromResult(tagToAdd);
            _mockTagRepository.Setup(x => x.SearchByName(It.IsAny<string>())).Returns(responseTask);
            _mockTagRepository.Setup(x => x.Insert(It.IsAny<Tag>())).Returns(_tag);

            //Act
            var result = _tagService.Create(new TagDomainModel { TagName = It.IsAny<string>() }).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (ResponseModel<TagDomainModel>)result;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsFalse(resultObject.IsSuccessful);
            Assert.AreEqual(resultObject.ErrorMessage, expectedMessage);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<TagDomainModel>));
        }

        [TestMethod]
        public void TagService_Create_Returns_SuccessfulCreatedResponseModel()
        {
            //Arrange
            Tag tagToAdd = null;
            Task<Tag> responseTask = Task.FromResult(tagToAdd);
            _mockTagRepository.Setup(x => x.SearchByName(It.IsAny<string>())).Returns(responseTask);
            _mockTagRepository.Setup(x => x.Insert(It.IsAny<Tag>())).Returns(_tag);

            //Act
            var result = _tagService.Create(new TagDomainModel { TagName = It.IsAny<string>() }).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = (ResponseModel<TagDomainModel>)result;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsTrue(resultObject.IsSuccessful);
            Assert.AreEqual(resultObject.DomainModel.Id, _tag.Id);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<TagDomainModel>));
        }
    }
}
