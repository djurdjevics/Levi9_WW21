using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class CinemasServiceTests
    {
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private Mock<IAuditoriumService> _mockAuditoriumService;
        private Data.Cinema _cinema;
        private Auditorium _auditorium;
        private CinemaDomainModel _cinemaDomainModel;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private SeatDomainModel _seatDomainModel;
        private CreateAuditoriumResultModel _createAuditoriumResultModel;
        private CreateCinemaWithAuditoriumModel _createCinemaAudit;
        private CinemaService cinemaService;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinema = new Data.Cinema()
            {
                Id = 1,
                Name = "Cinestar",
                Auditoriums = new List<Auditorium>() { }
            };

            _cinemaDomainModel = new CinemaDomainModel()
            {
                Id = _cinema.Id,
                Name = "Cinestar"
            };

            _auditorium = new Auditorium()
            {
                Id = 1,
                Name = "Auditorium",
                CinemaId = _cinema.Id,
                Cinema = _cinema
            };

            _auditoriumDomainModel = new AuditoriumDomainModel()
            {
                Id = _auditorium.Id,
                CinemaId = _auditorium.CinemaId,
                Name = _auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };

            _cinema.Auditoriums.Add(_auditorium);

            _seatDomainModel = new SeatDomainModel()
            {
                Id = new Guid(),
                AuditoriumId = _auditorium.Id,
                Number = 1,
                Row = 1
            };
            _auditoriumDomainModel.SeatsList.Add(_seatDomainModel);
            _createAuditoriumResultModel = new CreateAuditoriumResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Auditorium = _auditoriumDomainModel
            };

            _createCinemaAudit = new CreateCinemaWithAuditoriumModel()
            {
                AuditoriumName = _cinema.Name,
                CinemaName = _cinema.Name,
                NumberOfRows = 1,
                NumberOfColumns =1
            };

            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
        }


        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnCinemas()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            var expectedResultCount = 1;
            var cinemasList = new List<Data.Cinema>() { _cinema };
            Task<IEnumerable<Data.Cinema>> cinemasCollection = Task.FromResult((IEnumerable<Data.Cinema>)cinemasList);

            //Act
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(cinemasCollection);
            var resultAction = cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_cinema.Id, result[0].Id);
            Assert.AreEqual(_cinema.Name, result[0].Name);
            Assert.IsInstanceOfType(result, typeof(List<CinemaDomainModel>));
        }

        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnEmptyList()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            var expectedResultCount = 0;
            var cinemasList = new List<Data.Cinema>() {  };
            Task<IEnumerable<Data.Cinema>> cinemasCollection = Task.FromResult((IEnumerable<Data.Cinema>)cinemasList);

            //Act
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(cinemasCollection);
            var resultAction = cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.IsInstanceOfType(result, typeof(List<CinemaDomainModel>));
        }

        [TestMethod]
        public void CinemaService_GetAllAsync_ReturnNull()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            List<Data.Cinema> cinemasList = null;
            Task<IEnumerable<Data.Cinema>> cinemasCollection = Task.FromResult((IEnumerable<Data.Cinema>)cinemasList);

            //Act
            _mockCinemasRepository.Setup(x => x.GetAll()).Returns(cinemasCollection);
            var resultAction = cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }


        [TestMethod]
        public void CinemaService_GetCinemaByIdAsync_ReturnCinema()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            Task<Data.Cinema> cinema = Task.FromResult((Data.Cinema)_cinema);

            //Act
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(cinema);
            var result = cinemaService.GetCinemaByIdAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_cinema.Id, result.Id);
            Assert.AreEqual(_cinema.Name, result.Name);
            Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
        }


        [TestMethod]
        public void CinemaService_GetCinemaByIdAsync_ReturnNull()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            Task<Data.Cinema> cinema = Task.FromResult((Data.Cinema)null);

            //Act
            _mockCinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(cinema);
            var result = cinemaService.GetCinemaByIdAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CinemaService_DeleteCinema_ReturnsDeletedCinema()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);

            //Act
            _mockCinemasRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            var result = cinemaService.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_cinema.Id, result.Id);
            Assert.AreEqual(_cinema.Name, result.Name);
            Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_DeleteCinema_ReturnNull()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            Data.Cinema cinema = null;
            
            //Act
            _mockCinemasRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            var result = cinemaService.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CinemaService_AddCinema_ReturnNewCinema()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            Task<Data.Cinema> cinema = Task.FromResult((Data.Cinema)null);

            //Act
            _mockCinemasRepository.Setup(x => x.GetByCinemaName(It.IsAny<string>())).Returns(cinema);
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            var result = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(CreateCinemaResultModel));

        }

        [TestMethod]
        public void CinemaService_AddCinema_CinemaWithSameNameAlreadyExists_ReturnErrorMessage()
        {
            //Arrange
            var expectedMessage = Messages.CINEMA_SAME_NAME;
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            Task<Data.Cinema> cinema = Task.FromResult((Data.Cinema)_cinema);

            //Act
            _mockCinemasRepository.Setup(x => x.GetByCinemaName(It.IsAny<string>())).Returns(cinema);
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            var result = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(CreateCinemaResultModel));

        }

        [TestMethod]
        public void CinemaService_AddCinema_ReturnCinemaCreationError()
        {
            //Arrange
            var expectedMessage = Messages.CINEMA_CREATION_ERROR;
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            Task<Data.Cinema> cinema = Task.FromResult((Data.Cinema)null);
            Data.Cinema insertCinema = null;

            //Act
            _mockCinemasRepository.Setup(x => x.GetByCinemaName(It.IsAny<string>())).Returns(cinema);
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(insertCinema);
            _mockCinemasRepository.Setup(x => x.Save());
            var result = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(CreateCinemaResultModel));

        }
        [TestMethod]
        public void CinemaService_AddCinemaWithAuditorium_ReturnsNewCinemaWithAuditorium()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            Task<Data.Cinema> cinema = Task.FromResult((Data.Cinema)null);
            Task<CreateAuditoriumResultModel> createAuditoriumResultModel = Task.FromResult(_createAuditoriumResultModel);

            //Act
            _mockCinemasRepository.Setup(x => x.GetByCinemaName(It.IsAny<string>())).Returns(cinema);
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(createAuditoriumResultModel);

            var result = cinemaService.AddCinemaWithAuditorium(_createCinemaAudit).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(result.Cinema.Id, _cinema.Id);
            Assert.IsInstanceOfType(result, typeof(CreateCinemaResultModel));
        }


        [TestMethod]
        public void CinemaService_AddCinemaWithAuditorium_CinemaWithSameNameAlreadyExists_ErrorMessage()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            var expectedMessage = Messages.CINEMA_SAME_NAME;
            Task<Data.Cinema> cinema = Task.FromResult((Data.Cinema)_cinema);
            Task<CreateAuditoriumResultModel> createAuditoriumResultModel = Task.FromResult(_createAuditoriumResultModel);

            //Act
            _mockCinemasRepository.Setup(x => x.GetByCinemaName(It.IsAny<string>())).Returns(cinema);
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(createAuditoriumResultModel);

            var result = cinemaService.AddCinemaWithAuditorium(_createCinemaAudit).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ErrorMessage, expectedMessage);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(CreateCinemaResultModel));
        }

        [TestMethod]
        public void CinemaService_AddCinemaWithAuditorium_CinemaCreationError_ErrorMessage()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            var expectedMessage = Messages.CINEMA_CREATION_ERROR;
            Task<Data.Cinema> cinema = Task.FromResult((Data.Cinema)null);
            Data.Cinema insertCinema = null;
            Task<CreateAuditoriumResultModel> createAuditoriumResultModel = Task.FromResult(_createAuditoriumResultModel);

            //Act
            _mockCinemasRepository.Setup(x => x.GetByCinemaName(It.IsAny<string>())).Returns(cinema);
            _mockCinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(insertCinema);
            _mockCinemasRepository.Setup(x => x.Save());
            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(createAuditoriumResultModel);

            var result = cinemaService.AddCinemaWithAuditorium(_createCinemaAudit).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ErrorMessage, expectedMessage);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(CreateCinemaResultModel));
        }

        [TestMethod]
        public void CinemaService_UpdateCinema_ReturnsUpdatedCinema()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            
            //Act
            _mockCinemasRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Returns(_cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            var result = cinemaService.UpdateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_cinema.Id, result.Id);
            Assert.AreEqual(_cinema.Name, result.Name);
            Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_UpdateCinema_ReturnNull()
        {
            //Arrange
            cinemaService = new CinemaService(_mockCinemasRepository.Object, _mockAuditoriumService.Object);
            Data.Cinema cinema = null;

            //Act
            _mockCinemasRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Returns(cinema);
            _mockCinemasRepository.Setup(x => x.Save());
            var result = cinemaService.UpdateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }       
    }
}
