using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
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
    public class SeatsServiceTests
    {
        private Mock<ISeatsRepository> _mockSeatsRepository;
        private Seat _seat;
        private SeatDomainModel _seatDomainModel;
        private Ticket _ticket;
        private TicketDomainModel _ticketDomainModel;
        private SeatService seatService;

        [TestInitialize]
        public void TestInitialize()
        {
            _seat = new Seat()
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Row = 1,
                AuditoriumId = 1
            };

            _seatDomainModel = new SeatDomainModel()
            {
                Id = _seat.Id,
                Number = 1,
                Row = 1,
                AuditoriumId = 1
            };

            _ticket = new Ticket()
            {
                Id = Guid.NewGuid(),
                Price = 10,
                ProjectionId = Guid.NewGuid(),
                SeatId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _ticketDomainModel = new TicketDomainModel()
            {
                UserId = _ticket.UserId,
                SeatId = _ticket.SeatId,
                ProjectionId = _ticket.ProjectionId,
                Id = _ticket.Id,
                Price = _ticket.Price
            };

            _mockSeatsRepository = new Mock<ISeatsRepository>();
        }

        [TestMethod]
        public void SeatsService_GetAllAsync_ReturnListOfSeats()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            var expectedResultCount = 1;
            var seatsList = new List<Seat>() { _seat };
            Task<IEnumerable<Seat>> seatsCollection = Task.FromResult((IEnumerable<Seat>)seatsList);

            //Act
            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(seatsCollection);
            var resultAction = seatService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<SeatDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_seat.Id, result[0].Id);
            Assert.AreEqual(_seat.Number, result[0].Number);
            Assert.AreEqual(_seat.Row, result[0].Row);
            Assert.AreEqual(_seat.AuditoriumId, result[0].AuditoriumId);
            Assert.IsInstanceOfType(result, typeof(List<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_GetAllAsync_ReturnNull()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            List<Seat> seatsList = null;
            Task<IEnumerable<Seat>> seatsCollection = Task.FromResult((IEnumerable<Seat>)seatsList);
            
            //Act
            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(seatsCollection);
            var resultAction = seatService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            
            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void SeatsService_GetAllAsync_ReturnEmptyList()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            var expectedResultCount = 0;
            var seatsList = new List<Seat>() { };
            Task<IEnumerable<Seat>> seatsCollection = Task.FromResult((IEnumerable<Seat>)seatsList);
            
            //Act
            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(seatsCollection);
            var resultAction = seatService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<SeatDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.IsInstanceOfType(result, typeof(List<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_GetSeatByIdAsync_ReturnSeat()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            Task<Seat> seat = Task.FromResult((Seat)_seat);

            //Act
            _mockSeatsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(seat);
            var result = seatService.GetSeatByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(_seat.Id, result.DomainModel.Id);
            Assert.AreEqual(_seat.Number, result.DomainModel.Number);
            Assert.AreEqual(_seat.Row, result.DomainModel.Row);
            Assert.AreEqual(_seat.AuditoriumId, result.DomainModel.AuditoriumId);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_GetSeatByIdAsync_ReturnNull()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            var expectedMessage = Messages.SEAT_GET_BY_ID_ERROR;
            Task<Seat> seat = Task.FromResult((Seat)null);

            //Act
            _mockSeatsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(seat);
            var result = seatService.GetSeatByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.DomainModel);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_AddSeat_ReturnSeat()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            List<Seat> seats = new List<Seat>() { };
            Task<IEnumerable<Seat>> seatsCollection = Task.FromResult((IEnumerable<Seat>)seats);

            //Act
            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(seatsCollection);
            _mockSeatsRepository.Setup(x => x.Insert(It.IsAny<Seat>())).Returns(_seat);
            _mockSeatsRepository.Setup(x => x.Save());

            var resultAction = seatService.AddSeat(_seatDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(_seatDomainModel.Id, resultAction.DomainModel.Id);
            Assert.AreEqual(_seatDomainModel.Number, resultAction.DomainModel.Number);
            Assert.AreEqual(_seatDomainModel.Row, resultAction.DomainModel.Row);
            Assert.AreEqual(_seatDomainModel.AuditoriumId, resultAction.DomainModel.AuditoriumId);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_AddSeat_ReturnSeatAlreadyExists()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            var expectedMessage = Messages.SEAT_WITH_SAME_NUMBER_ROW_AUDITORIUMID_ALREADY_EXISTS;
            List<Seat> seats = new List<Seat>() { _seat };
            Task<IEnumerable<Seat>> seatsCollection = Task.FromResult((IEnumerable<Seat>)seats);

            //Act
            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(seatsCollection);
            _mockSeatsRepository.Setup(x => x.Insert(It.IsAny<Seat>())).Returns(_seat);
            _mockSeatsRepository.Setup(x => x.Save());

            var resultAction = seatService.AddSeat(_seatDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNotNull(resultAction.ErrorMessage);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_AddSeat_SeatCreationError_RetrunErrorMessage()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            var expectedMessage = Messages.SEAT_CREATION_ERROR;
            Seat seat = null;
            List<Seat> seats = new List<Seat>() {  };
            Task<IEnumerable<Seat>> seatsCollection = Task.FromResult((IEnumerable<Seat>)seats);

            //Act
            _mockSeatsRepository.Setup(x => x.GetAll()).Returns(seatsCollection);
            _mockSeatsRepository.Setup(x => x.Insert(It.IsAny<Seat>())).Returns(seat);
            _mockSeatsRepository.Setup(x => x.Save());

            var resultAction = seatService.AddSeat(_seatDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNotNull(resultAction.ErrorMessage);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_DeleteSeat_ReturnDeletedSeat()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);

            //Act
            _mockSeatsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_seat);
            _mockSeatsRepository.Setup(x => x.Save());
            var result = seatService.DeleteSeat(It.IsAny<Guid>());

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(_seat.Id, result.DomainModel.Id);
            Assert.AreEqual(_seat.AuditoriumId, result.DomainModel.AuditoriumId);
            Assert.AreEqual(_seat.Row, result.DomainModel.Row);
            Assert.AreEqual(_seat.Number, result.DomainModel.Number);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_DeleteSeat_ReturnNull()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            Seat seat = null;
            var expectedMessage = Messages.SEAT_DOESNT_EXISTS;

            //Act
            _mockSeatsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(seat);
            _mockSeatsRepository.Setup(x => x.Save());
            var result = seatService.DeleteSeat(It.IsAny<Guid>());

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsNull(result.DomainModel);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(ResponseModel<SeatDomainModel>));
        }

        [TestMethod]
        public void SeatsService_DeleteByAuditoriumid_ReturnDeletedSeats()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            IEnumerable<Seat> seats = new List<Seat>() { _seat };
            Task<IEnumerable<Seat>> deletedSeats = Task.FromResult((IEnumerable<Seat>)seats);

            //Act
            _mockSeatsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(seats);
            _mockSeatsRepository.Setup(x => x.DeleteByAuditoriumId(It.IsAny<int>())).Returns(deletedSeats);
            var resultAction = seatService.DeleteByAuditoriumId(1).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<SeatDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(result[0].Id, _seat.Id);
            Assert.AreEqual(result[0].AuditoriumId, _seat.AuditoriumId);
            Assert.AreEqual(result[0].Number, _seat.Number);
            Assert.AreEqual(result[0].Row, _seat.Row);
            Assert.IsInstanceOfType(result[0], typeof(SeatDomainModel));
        }

        [TestMethod]
        public void SeatsService_DeleteByAuditoriumid_GetByAuditoriumIdReturnsNull()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            IEnumerable<Seat> seats = null;
            Task<IEnumerable<Seat>> deletedSeats = Task.FromResult((IEnumerable<Seat>)seats);

            //Act
            _mockSeatsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(seats);
            var resultAction = seatService.DeleteByAuditoriumId(1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void SeatsService_DeleteByAuditoriumid_DeleteRepositoryReturnsNull()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            IEnumerable<Seat> seats = new List<Seat>() { _seat };
            IEnumerable<Seat> seatsToDelete = null;
            Task<IEnumerable<Seat>> deletedSeats = Task.FromResult((IEnumerable<Seat>)seatsToDelete);

            //Act
            _mockSeatsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(seats);
            _mockSeatsRepository.Setup(x => x.DeleteByAuditoriumId(It.IsAny<int>())).Returns(deletedSeats);
            var resultAction = seatService.DeleteByAuditoriumId(1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void SeatsService_GetSeatsByAuditoriumId_ReturnSeats()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            IEnumerable<Seat> seats = new List<Seat>() { _seat };

            //Act
            _mockSeatsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(seats);
            var resultAction = seatService.GetSeatsByAuditoriumId(It.IsAny<int>());
            var result = (List<SeatDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(result[0].Id, _seat.Id);
            Assert.AreEqual(result[0].AuditoriumId, _seat.AuditoriumId);
            Assert.AreEqual(result[0].Number, _seat.Number);
            Assert.AreEqual(result[0].Row, _seat.Row);
            Assert.IsInstanceOfType(result[0], typeof(SeatDomainModel));
        }

        [TestMethod]
        public void SeatsService_GetSeatsByAuditoriumId_GetbyAuditoriumIdReturnsNull()
        {
            //Arrange
            seatService = new SeatService(_mockSeatsRepository.Object);
            IEnumerable<Seat> seats = null;

            //Act
            _mockSeatsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(seats);
            var resultAction = seatService.GetSeatsByAuditoriumId(It.IsAny<int>());

            //Assert
            Assert.IsNull(resultAction);
        }
    }
}
