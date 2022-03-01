using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class Levi9PaymentControllerTests
    {
        private Mock<ILevi9PaymentService> _mockPaymentService;
        private PaymentResponse _paymentResponse;
        private PaymentResponseModel _paymentResponseModel;
        private Levi9PaymentController _levi9PaymentController;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockPaymentService = new Mock<ILevi9PaymentService>();
        }


        [TestMethod]
        public void Levi9PaymentController_Post_ReturnSuccessfulPayment()
        {
            //Arrange
            _paymentResponse = new PaymentResponse()
            {
                Message = "Payment is successful.",
                IsSuccess = true
            };

            _paymentResponseModel = new PaymentResponseModel()
            {
                IsSuccess = true,
                Message = "Payment is successful."
            };

            _levi9PaymentController = new Levi9PaymentController(_mockPaymentService.Object);
            Task<PaymentResponse> payment = Task.FromResult((PaymentResponse)_paymentResponse);
            var expectedStatusCode = 200;
            
            //Act
            _mockPaymentService.Setup(x => x.MakePayment()).Returns(payment);
            var resultAction = _levi9PaymentController.Post().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var paymentResponseModel = (PaymentResponseModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsTrue(paymentResponseModel.IsSuccess);
            Assert.AreEqual(_paymentResponseModel.IsSuccess, paymentResponseModel.IsSuccess);
            Assert.AreEqual(_paymentResponseModel.Message, paymentResponseModel.Message);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);

        }

        [TestMethod]
        public void Levi9PaymentController_Post_ReturnBadRequestInsufficientFounds()
        {
            //Arrange
            _paymentResponse = new PaymentResponse()
            {
                Message = "Insufficient founds.",
                IsSuccess = false
            };

            _paymentResponseModel = new PaymentResponseModel()
            {
                IsSuccess = false,
                Message = "Insufficient founds."
            };

            _levi9PaymentController = new Levi9PaymentController(_mockPaymentService.Object);
            Task<PaymentResponse> payment = Task.FromResult((PaymentResponse)_paymentResponse);
            var expectedStatusCode = 400;

            //Act
            _mockPaymentService.Setup(x => x.MakePayment()).Returns(payment);
            var resultAction = _levi9PaymentController.Post().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((BadRequestObjectResult)resultAction).Value;
            var paymentResponseModel = (ErrorResponseModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_paymentResponseModel.Message, paymentResponseModel.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)resultAction).StatusCode);

        }

        [TestMethod]
        public void Levi9PaymentController_Post_ReturnBadRequestPaymentCreationError()
        {
            //Arrange
            _paymentResponse = new PaymentResponse()
            {
                Message = "Connection error.",
                IsSuccess = false
            };

            _paymentResponseModel = new PaymentResponseModel()
            {
                IsSuccess = false,
                Message = "Connection error."
            };

            _levi9PaymentController = new Levi9PaymentController(_mockPaymentService.Object);
            Task<PaymentResponse> payment = Task.FromResult((PaymentResponse)_paymentResponse);
            var expectedStatusCode = 400;
            var expectedMessage = Messages.PAYMENT_CREATION_ERROR;

            //Act
            _mockPaymentService.Setup(x => x.MakePayment()).Returns(payment);
            var resultAction = _levi9PaymentController.Post().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorResponseModel = (ErrorResponseModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, errorResponseModel.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)resultAction).StatusCode);

        }
    }
}
