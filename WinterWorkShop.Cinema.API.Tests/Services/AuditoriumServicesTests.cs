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
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;



namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class AuditoriumServiceTests
    {
        private Mock<IAuditoriumsRepository> _mockAuditoriumsRepository;
        private Mock<ICinemasRepository> _mockCinemasRepository;
        private Mock<ISeatService> _mockSeatService;
        private Auditorium _auditorium;
        private CreateAuditoriumResultModel createAuditoriumResultModel;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private SeatDomainModel _seatDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {

            _auditorium = new Auditorium
            {
                Id = 1,
                CinemaId = 1,
                Name = "Auditorium",
                Cinema = new Data.Cinema { Name = "Bioskop" }
            };

            _auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = _auditorium.Id,
                CinemaId = _auditorium.CinemaId,
                Name = _auditorium.Name
            };

            _seatDomainModel = new SeatDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumId = _auditoriumDomainModel.Id,
                Row = 1,
                Number = 1
            };

            List<Auditorium> auditoriumsModelsList = new List<Auditorium>();
            auditoriumsModelsList.Add(_auditorium);
            IEnumerable<Auditorium> auditoriums = auditoriumsModelsList;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            List<SeatDomainModel> seatsModelsList = new List<SeatDomainModel>();
            seatsModelsList.Add(_seatDomainModel);
            IEnumerable<SeatDomainModel> seats = seatsModelsList;
            Task<IEnumerable<SeatDomainModel>> seatsResponseTask = Task.FromResult(seats);

            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mockCinemasRepository = new Mock<ICinemasRepository>();
            _mockSeatService = new Mock<ISeatService>();

            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
        }

        [TestMethod]
        public void AuditoriumService_GetAllAsync_ReturnsListOfAuditoriums()
        {
            //Arrange
            int expectedCount = 1;
            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            //Act
            var resultAction = auditoriumService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<AuditoriumDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.IsInstanceOfType(result[0], typeof(AuditoriumDomainModel));
            Assert.AreEqual(result[0].Id, _auditorium.Id);
        }

        [TestMethod]
        public void AuditoriumService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Auditorium> auditoriums = null;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            _mockAuditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            //Act
            var result = auditoriumService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuditoriumService_GetAuditoriumByIdAsync_ReturnAuditorium()
        {
            //Arrange
            Task<Auditorium> auditorium = Task.FromResult(_auditorium);
            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(auditorium);

            //Act
            var result = auditoriumService.GetAuditoriumByIdAsync(_auditorium.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AuditoriumDomainModel));
            Assert.AreEqual(result.Id, _auditorium.Id);

        }

        [TestMethod]
        public void AuditoriumService_GetAuditoriumByIdAsync_ReturnNull()
        {
            //Arrange
            Auditorium nullAuditorium = null;
            Task<Auditorium> auditorium = Task.FromResult(nullAuditorium);
            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockAuditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(auditorium);

            //Act
            var result = auditoriumService.GetAuditoriumByIdAsync(_auditorium.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuditoriumService_GetAuditoriumByCinemaId_ReturnsListOfAuditoriums()
        {
            //Arrange
            List<Auditorium> auditoriumsModelsList = new List<Auditorium>();
            auditoriumsModelsList.Add(_auditorium);
            IEnumerable<Auditorium> auditoriums = auditoriumsModelsList;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);
            int expectedCount = 1;

            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockAuditoriumsRepository.Setup(x => x.GetByCinemaId(It.IsAny<int>())).Returns(responseTask);

            //Act
            var result = auditoriumService.GetAuditoriumByCinemaId(_auditorium.CinemaId).ConfigureAwait(false).GetAwaiter().GetResult().ToList();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.IsInstanceOfType(result[0], typeof(AuditoriumDomainModel));
            Assert.AreEqual(result[0].Id, _auditorium.Id);
            Assert.AreEqual(result[0].CinemaId, _auditorium.CinemaId);
        }

        [TestMethod]
        public void AuditoriumService_GetAuditoriumByCinemaId_ReturnNull()
        {
            //Arrange
            IEnumerable<Auditorium> auditoriums = null;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockAuditoriumsRepository.Setup(x => x.GetByCinemaId(It.IsAny<int>())).Returns(responseTask);

            //Act
            var result = auditoriumService.GetAuditoriumByCinemaId(_auditorium.CinemaId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuditoriumService_DeleteAuditorium_ReturnsDeletedAuditoriumsAndSeats()
        {
            //Arrange
            List<SeatDomainModel> seatsModelsList = new List<SeatDomainModel>();
            seatsModelsList.Add(_seatDomainModel);
            IEnumerable<SeatDomainModel> seats = seatsModelsList;
            Task<IEnumerable<SeatDomainModel>> seatsResponseTask = Task.FromResult(seats);

            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockSeatService.Setup(x => x.DeleteByAuditoriumId(It.IsAny<int>())).Returns(seatsResponseTask);
            _mockAuditoriumsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_auditorium);

            //Act
            var result = auditoriumService.DeleteAuditorium(_auditorium.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AuditoriumDomainModel));
            Assert.IsNotNull(result.SeatsList);
            Assert.AreEqual(result.Id, _auditorium.Id);
        }

        [TestMethod]
        public void AuditoriumService_DeleteAuditorium_ReturnNull()
        {
            //Arrange
            Auditorium nullAuditorium = null;

            List<SeatDomainModel> seatsModelsList = new List<SeatDomainModel>();
            seatsModelsList.Add(_seatDomainModel);
            IEnumerable<SeatDomainModel> seats = seatsModelsList;
            Task<IEnumerable<SeatDomainModel>> seatsResponseTask = Task.FromResult(seats);

            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockSeatService.Setup(x => x.DeleteByAuditoriumId(It.IsAny<int>())).Returns(seatsResponseTask);
            _mockAuditoriumsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(nullAuditorium);

            //Act
            var result = auditoriumService.DeleteAuditorium(_auditorium.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuditoriumService_DeleteAuditorium_SeatServiceReturnsNull()
        {
            //Arrange
            IEnumerable<SeatDomainModel> seats = null;
            Task<IEnumerable<SeatDomainModel>> seatsResponseTask = Task.FromResult(seats);

            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockSeatService.Setup(x => x.DeleteByAuditoriumId(It.IsAny<int>())).Returns(seatsResponseTask);
            _mockAuditoriumsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_auditorium);

            //Act
            var result = auditoriumService.DeleteAuditorium(_auditorium.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuditoriumService_DeleteAuditoriumByCinemaId_ReturnsDeletedAuditoriums()
        {
            //Arrange
            List<Auditorium> auditoriumsModelsList = new List<Auditorium>();
            auditoriumsModelsList.Add(_auditorium);
            IEnumerable<Auditorium> auditoriums = auditoriumsModelsList;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);
            int expectedCount = 1;

            List<SeatDomainModel> seatsModelsList = new List<SeatDomainModel>();
            seatsModelsList.Add(_seatDomainModel);
            IEnumerable<SeatDomainModel> seats = seatsModelsList;
            Task<IEnumerable<SeatDomainModel>> seatsResponseTask = Task.FromResult(seats);

            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockSeatService.Setup(x => x.DeleteByAuditoriumId(It.IsAny<int>())).Returns(seatsResponseTask);
            _mockAuditoriumsRepository.Setup(x => x.GetByCinemaId(It.IsAny<int>())).Returns(responseTask);
            _mockAuditoriumsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_auditorium);

            //Act
            var result = auditoriumService.DeleteAuditoriumByCinemaId(_auditorium.CinemaId).ConfigureAwait(false).GetAwaiter().GetResult().ToList();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.IsInstanceOfType(result[0], typeof(AuditoriumDomainModel));
            Assert.AreEqual(result[0].Id, _auditorium.Id);
            Assert.AreEqual(result[0].CinemaId, _auditorium.CinemaId);
        }

        [TestMethod]
        public void AuditoriumService_DeleteAuditoriumByCinemaId_ReturnNull()
        {
            //Arrange
            IEnumerable<Auditorium> auditoriums = null;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            List<SeatDomainModel> seatsModelsList = new List<SeatDomainModel>();
            seatsModelsList.Add(_seatDomainModel);
            IEnumerable<SeatDomainModel> seats = seatsModelsList;
            Task<IEnumerable<SeatDomainModel>> seatsResponseTask = Task.FromResult(seats);

            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _mockSeatService.Setup(x => x.DeleteByAuditoriumId(It.IsAny<int>())).Returns(seatsResponseTask);
            _mockAuditoriumsRepository.Setup(x => x.GetByCinemaId(It.IsAny<int>())).Returns(responseTask);
            _mockAuditoriumsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_auditorium);

            //Act
            var result = auditoriumService.DeleteAuditoriumByCinemaId(_auditorium.CinemaId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuditoriumService_CreateAuditorium_ReturnsCreatedAuditorium()
        {
            //Arrange
            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            Auditorium auditorium = new Auditorium
            {
                Id = 1,
                CinemaId = 1,
                Name = "Auditorium",
                Cinema = new Data.Cinema { Name = "Bioskop" }
            };

            auditorium.Seats = new List<Seat>();
            var numberOfRows = 1;
            var numberOfSeats = 1;
            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfSeats; j++)
                {
                    Seat updateSeat = new Seat()
                    {
                        Row = i,
                        Number = j
                    };

                    auditorium.Seats.Add(updateSeat);
                }
            }
            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = auditorium.Id,
                CinemaId = auditorium.CinemaId,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };

            createAuditoriumResultModel = new CreateAuditoriumResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Auditorium = auditoriumDomainModel
            };

            foreach (Seat seat in auditorium.Seats) { createAuditoriumResultModel.Auditorium.SeatsList.Add(_seatDomainModel); }

            List<Auditorium> auditoriumsModelsList = new List<Auditorium>();
            IEnumerable<Auditorium> auditoriums = auditoriumsModelsList;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            _mockAuditoriumsRepository.Setup(x => x.GetByCinemaId(It.IsAny<int>())).Returns(responseTask);
            _mockAuditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>())).Returns(responseTask);
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(auditorium);
            _mockAuditoriumsRepository.Setup(x => x.Save());

            //Act
            var resultAction = auditoriumService.CreateAuditorium(auditoriumDomainModel, It.IsAny<int>(), It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsNotNull(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(CreateAuditoriumResultModel));
            Assert.AreEqual(resultAction.Auditorium.Id, auditorium.Id);
        }

        [TestMethod]
        public void AuditoriumService_CreateAuditorium_AuditoriumWithSameNameExists_ErrorMessage()
        {
            //Arrange
            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);
            var expectedMessage = Messages.AUDITORIUM_SAME_NAME;
            Auditorium auditorium = new Auditorium
            {
                Id = 1,
                CinemaId = 1,
                Name = "Auditorium",
                Cinema = new Data.Cinema { Name = "Bioskop" }
            };

            auditorium.Seats = new List<Seat>();
            var numberOfRows = 1;
            var numberOfSeats = 1;
            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfSeats; j++)
                {
                    Seat updateSeat = new Seat()
                    {
                        Row = i,
                        Number = j
                    };

                    auditorium.Seats.Add(updateSeat);
                }
            }
            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = auditorium.Id,
                CinemaId = auditorium.CinemaId,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };

            createAuditoriumResultModel = new CreateAuditoriumResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Auditorium = auditoriumDomainModel
            };

            foreach (Seat seat in auditorium.Seats) { createAuditoriumResultModel.Auditorium.SeatsList.Add(_seatDomainModel); }

            List<Auditorium> auditoriumsModelsList1 = new List<Auditorium>();
            IEnumerable<Auditorium> auditoriums1 = auditoriumsModelsList1;
            Task<IEnumerable<Auditorium>> responseTask1 = Task.FromResult(auditoriums1);

            List<Auditorium> auditoriumsModelsList = new List<Auditorium>();
            auditoriumsModelsList.Add(auditorium);
            IEnumerable<Auditorium> auditoriums = auditoriumsModelsList;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);

            _mockAuditoriumsRepository.Setup(x => x.GetByCinemaId(It.IsAny<int>())).Returns(responseTask1);
            _mockAuditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>())).Returns(responseTask);
            _mockAuditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(auditorium);
            _mockAuditoriumsRepository.Setup(x => x.Save());

            //Act
            var resultAction = auditoriumService.CreateAuditorium(auditoriumDomainModel, It.IsAny<int>(), It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        [TestMethod]
        public void AuditoriumService_UpdateAuditorium_ReturnUpdatedAuditorium()
        {
            //Arrange
            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);

            _auditorium.Seats = new List<Seat>();
            var numberOfRows = 1;
            var numberOfSeats = 1;
            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfSeats; j++)
                {
                    Seat updateSeat = new Seat()
                    {
                        Row = i,
                        Number = j
                    };

                    _auditorium.Seats.Add(updateSeat);
                }
            }
  
            _mockAuditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Returns(_auditorium);
            _mockAuditoriumsRepository.Setup(x => x.Save());


            //Act
            var result = auditoriumService.UpdateAuditorium(_auditoriumDomainModel, It.IsAny<int>(), It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_auditorium.Id, result.Id);
            Assert.AreEqual(_auditorium.Name, result.Name);
            Assert.IsInstanceOfType(result, typeof(AuditoriumDomainModel));

        }

        [TestMethod]
        public void AuditoriumService_UpdateAuditorium_ReturnNull()
        {
            //Arrange
            AuditoriumService auditoriumService = new AuditoriumService(_mockAuditoriumsRepository.Object, _mockCinemasRepository.Object, _mockSeatService.Object);
            Auditorium auditorium = null;

            //Act
            _mockAuditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Returns(auditorium);
            _mockAuditoriumsRepository.Setup(x => x.Save());
            var result = auditoriumService.UpdateAuditorium(_auditoriumDomainModel, It.IsAny<int>(), It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

    }
}
