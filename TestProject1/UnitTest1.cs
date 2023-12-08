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
            string encounterId = Guid.NewGuid().ToString();
            string organizationId = Guid.NewGuid().ToString();
            string locationId = Guid.NewGuid().ToString();
            string slotId = Guid.NewGuid().ToString();
            string patientId = Guid.NewGuid().ToString();
            string practitionerId = Guid.NewGuid().ToString();

            var request = new AppointmentDTO
            {
                Start = new DateTimeOffset(DateTime.UtcNow.AddHours(-1)),
                End = new DateTimeOffset(DateTime.UtcNow.AddHours(1)),
                CanCancel = true,
                CanReschedule = false,
                IsExceedRescheduleLimit = true,
                IsCheckedIn = false,
                ContactNo = "ContactNo " + Guid.NewGuid().ToString(),
                AppointmentIdForRegistration = "AppointmentIdForRegistration " + Guid.NewGuid().ToString(),
                VCPZoomParticipantUrl = "VCPZoomParticipantUrl " + Guid.NewGuid().ToString(),
                DepartmentCode = "DepartmentCode " + Guid.NewGuid().ToString(),
                EncounterId = encounterId,
                OrganizationUnit = "OrganizationUnit " + Guid.NewGuid().ToString(),
                OrganizationId = organizationId,
                OrganizationDisplay = "OrganizationDisplay " + organizationId,
                Description = "Description " + Guid.NewGuid().ToString(),
                Location = "Location " + locationId,
                LocationId = locationId,
                Status = Hl7.Fhir.Model.Appointment.AppointmentStatus.Booked,
                RegistrationAvailability = "RegistrationAvailability " + Guid.NewGuid().ToString(),
                PossibleRescheduleStartDate = DateTime.UtcNow.AddDays(-1),
                PossibleRescheduleEndDate = DateTime.UtcNow.AddDays(1),
                PatientInstruction = "PatientInstruction " + Guid.NewGuid().ToString(),
                Slot = slotId,
                VisitType = "VisitType " + Guid.NewGuid().ToString(),
                VisitTypeId = "VisitTypeId " + Guid.NewGuid().ToString(),
                PatientId = patientId,
                ProviderPractitionerId = practitionerId,
                ProviderPractitioner = "ProviderPractitioner " + practitionerId,
                LocationZoneInfo = "LocationZoneInfo " + Guid.NewGuid().ToString(),
                ServiceType = new List<string> { "serviceType1", "serviceType2" },
                AppointmentLinkId = "AppointmentLinkId " + Guid.NewGuid().ToString(),
                RescheduleToDays = "RescheduleToDays " + Guid.NewGuid().ToString(),
                OrderExpectedDate = DateTime.UtcNow.AddDays(-2),
                Comment = "Comment " + Guid.NewGuid().ToString(),
            };
            var createResult = await _controller.CreateAppointment(request);

            var getResult2 = await _controller.GetAppointment();
            var getByIdResult = await _controller.GetAppointmentById(createResult.Id);

            var deleteResult = await _controller.DeleteAppointment(createResult.Id);
            Assert.NotNull(deleteResult);
            Assert.AreEqual(createResult.Start, deleteResult.Start);

            Assert.AreEqual(getResult.Total + 1, getResult2.Total);

            Assert.AreEqual(request.Start, getByIdResult.Start);
            Assert.AreEqual(request.CanCancel, getByIdResult.CanCancel);
            Assert.AreEqual(request.CanReschedule, getByIdResult.CanReschedule);
            Assert.AreEqual(request.IsExceedRescheduleLimit, getByIdResult.IsExceedRescheduleLimit);
            Assert.AreEqual(request.IsCheckedIn, getByIdResult.IsCheckedIn);
            Assert.AreEqual(request.ContactNo, getByIdResult.ContactNo);
            Assert.AreEqual(request.AppointmentIdForRegistration, getByIdResult.AppointmentIdForRegistration);
            Assert.AreEqual(request.VCPZoomParticipantUrl, getByIdResult.VCPZoomParticipantUrl);
            Assert.AreEqual(request.DepartmentCode, getByIdResult.DepartmentCode);
            Assert.AreEqual(encounterId, getByIdResult.EncounterId);
            Assert.AreEqual(request.OrganizationUnit, getByIdResult.OrganizationUnit);
            Assert.AreEqual(organizationId, getByIdResult.OrganizationId);
            Assert.AreEqual(request.OrganizationDisplay, getByIdResult.OrganizationDisplay);
            Assert.AreEqual(request.Description, getByIdResult.Description);
            Assert.AreEqual(locationId, getByIdResult.LocationId);
            Assert.IsTrue(getByIdResult.Location.Contains(request.LocationId));
            Assert.IsTrue(getByIdResult.Location.Contains(request.Location));
            Assert.AreEqual(request.Status, getByIdResult.Status);
            Assert.AreEqual(request.RegistrationAvailability, getByIdResult.RegistrationAvailability);
            Assert.AreEqual(request.PossibleRescheduleStartDate, getByIdResult.PossibleRescheduleStartDate);
            Assert.AreEqual(request.PossibleRescheduleEndDate, getByIdResult.PossibleRescheduleEndDate);
            Assert.AreEqual(request.PatientInstruction, getByIdResult.PatientInstruction);
            Assert.IsTrue(getByIdResult.Slot.Contains(request.Slot));
            Assert.IsTrue(getByIdResult.Slot.Contains("Slot/"));
            Assert.AreEqual(request.VisitType, getByIdResult.VisitType);
            Assert.AreEqual(request.VisitTypeId, getByIdResult.VisitTypeId);
            Assert.AreEqual(patientId, getByIdResult.PatientId);
            Assert.AreEqual(practitionerId, getByIdResult.ProviderPractitionerId);
            Assert.AreEqual(request.ProviderPractitioner, getByIdResult.ProviderPractitioner);
            Assert.AreEqual(request.LocationZoneInfo, getByIdResult.LocationZoneInfo);
            Assert.AreEqual(request.ServiceType.OrderBy(x => x), getByIdResult.ServiceType.OrderBy(x => x));
            Assert.AreEqual(request.AppointmentLinkId, getByIdResult.AppointmentLinkId);
            Assert.AreEqual(request.RescheduleToDays, getByIdResult.RescheduleToDays);
            Assert.AreEqual(request.OrderExpectedDate, getByIdResult.OrderExpectedDate);
            Assert.AreEqual(request.Comment, getByIdResult.Comment);

            getResult2 = await _controller.GetAppointment();
            Assert.AreEqual(getResult.Total, getResult2.Total);
        }
    }
}