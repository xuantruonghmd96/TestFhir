using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Models;

namespace TestProject1
{
    public class Tests
    {
        private readonly TestController _controller;

        public Tests()
        {
            _controller = new TestController(new Mock<ILogger<TestController>>().Object);
        }

        [Test]
        public async System.Threading.Tasks.Task Test1()
        {
            var result = await _controller.Get();
            Assert.Pass();
        }

        [Test]
        public async System.Threading.Tasks.Task Test_GetAppointment()
        {
            var result = await _controller.GetAppointment();
            Assert.Pass();
        }

        [Test]
        public async System.Threading.Tasks.Task Test_CRUD_Appointment()
        {
            var getResult = await _controller.GetAppointment();

            var request = new AppointmentDTO
            {
                Status = Hl7.Fhir.Model.Appointment.AppointmentStatus.Booked,
                Start = new DateTimeOffset(DateTime.UtcNow.AddHours(-1)),
                End = new DateTimeOffset(DateTime.UtcNow.AddHours(1)),
                Extension = new List<Hl7.Fhir.Model.Extension>
                {
                    new Hl7.Fhir.Model.Extension
                    {
                        Url = "abcede http://ihis.sg/ccdp/extension/canCancel",
                        Value = new Hl7.Fhir.Model.FhirBoolean(true),
                    }
                }
            };
            var createResult = await _controller.CreateAppointment(request);

            var getResult2 = await _controller.GetAppointment();
            Assert.AreEqual(getResult.Total + 1, getResult2.Total);

            var getByIdResult = await _controller.GetAppointmentById(createResult.Id);
            Assert.AreEqual(request.Start, getByIdResult.Start);
            Assert.AreEqual(1, getByIdResult.Participant.Count);
            Assert.AreEqual(1, getByIdResult.Extension.Count);
            Assert.AreEqual(request.Extension.First().Url, getByIdResult.Extension.First().Url);
            Assert.AreEqual(true, getByIdResult.GetBoolExtension(request.Extension.First().Url));
            Assert.AreEqual(true, getByIdResult.GetExtensionValue<FhirBoolean>(request.Extension.First().Url).Value);

            var deleteResult = await _controller.DeleteAppointment(createResult.Id);
            Assert.NotNull(deleteResult);
            Assert.AreEqual(createResult.Start, deleteResult.Start);

            getResult2 = await _controller.GetAppointment();
            Assert.AreEqual(getResult.Total, getResult2.Total);
        }
    }
}