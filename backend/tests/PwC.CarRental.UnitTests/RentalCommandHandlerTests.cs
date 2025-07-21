using FluentAssertions;
using MediatR;
using Moq;
using PwC.CarRental.Application.Commands.Customers;
using PwC.CarRental.Application.Commands.Rentals;
using PwC.CarRental.Application.Common.Interfaces;
using PwC.CarRental.Domain.Aggregates;
using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;
using PwC.CarRental.Infrastructure.Persistence;
using PwC.CarRental.UnitTests.Fixtures;

namespace PwC.CarRental.UnitTests;

public class RentalCommandHandlerTests : IClassFixture<CarRentalDbContextFixture>
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IRentalRepository> _rentalRepositoryMock;
    private readonly Mock<ICarRepository> _carRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly RentalSystem _rentalSystem;
    private readonly CarRentalDbContext _dbContext;

    public RentalCommandHandlerTests(CarRentalDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _emailServiceMock = new Mock<IEmailService>();
        _mediatorMock = new Mock<IMediator>();
        _emailServiceMock.Setup(x => x.SendReservationConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _carRepositoryMock = new Mock<ICarRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();

        _rentalSystem = new RentalSystem(_customerRepositoryMock.Object, _rentalRepositoryMock.Object, _carRepositoryMock.Object, _serviceRepositoryMock.Object);
    }

    [Fact]
    public async Task Cannot_Rent_Already_Reserved_Car()
    {
        // Arrange
        var expectedCustomer1 = _dbContext.Customers.First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCustomer1.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer1);
        var expectedCustomer2 = _dbContext.Customers.Skip(1).First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCustomer2.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer2);
        var expectedCar = _dbContext.Cars.First();
        var reservedStart = DateTime.Today.AddDays(2);
        var reservedEnd = DateTime.Today.AddDays(7);
        var expectedRental = new Rental(expectedCustomer1, reservedStart, reservedEnd, expectedCar);
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCar.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCar);
        _rentalRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([expectedRental]);
        _serviceRepositoryMock.Setup(repo => repo.GetServicesForCarAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync([new()]);
        var handler = new RegisterRentalCommandHandler(_mediatorMock.Object, _rentalSystem, _customerRepositoryMock.Object, _carRepositoryMock.Object);

        var command = new RegisterRentalCommand(
            expectedCustomer2.Id,
            expectedCar.Id,
            reservedStart,
            reservedEnd,
            It.IsAny<string>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Car is not available.");
    }

    [Fact]
    public async Task Cannot_Rent_Under_Service_Car()
    {
        // Arrange
        var expectedCustomer = _dbContext.Customers.First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCustomer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer);
        var expectedCustomer2 = _dbContext.Customers.Skip(1).First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCustomer2.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer2);
        var expectedCar = _dbContext.Cars.First();
        var reservedStart = DateTime.Today.AddDays(2);
        var reservedEnd = DateTime.Today.AddDays(7);
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCar.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCar);
        _rentalRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _serviceRepositoryMock.Setup(repo => repo.GetServicesForCarAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync([new()]);

        var handler = new RegisterRentalCommandHandler(_mediatorMock.Object, _rentalSystem, _customerRepositoryMock.Object, _carRepositoryMock.Object);
        var command = new RegisterRentalCommand(
            expectedCustomer2.Id,
            expectedCar.Id,
            reservedStart,
            reservedEnd,
            It.IsAny<string>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Car is not available.");
    }

    [Fact]
    public async Task Can_Rent_Available_Car()
    {
        // Arrange
        var expectedCustomer = _dbContext.Customers.First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCustomer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer);
        var expectedCustomer2 = _dbContext.Customers.Skip(1).First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCustomer2.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer2);
        var expectedCar = _dbContext.Cars.First();
        var expectedRental = _dbContext.Rentals.First();
        var reservedStart = expectedRental.EndDate.AddDays(2);
        var reservedEnd = expectedRental.EndDate.AddDays(7);
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCar.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCar);
        _rentalRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([expectedRental]);
        _serviceRepositoryMock.Setup(repo => repo.GetServicesForCarAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var handler = new RegisterRentalCommandHandler(_mediatorMock.Object, _rentalSystem, _customerRepositoryMock.Object, _carRepositoryMock.Object);
        var command = new RegisterRentalCommand(
            expectedCustomer2.Id,
            expectedCar.Id,
            reservedStart,
            reservedEnd,
            It.IsAny<string>()
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().NotBe(expectedRental.Id);
    }

    [Fact]
    public async Task Cannot_Rent_Car_After_Completed_Next_Day()
    {
        // Arrange
        var expectedCustomer = _dbContext.Customers.First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCustomer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer);
        var expectedCustomer2 = _dbContext.Customers.Skip(1).First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCustomer2.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer);
        var expectedCar = _dbContext.Cars.First();
        var expectedRental = _dbContext.Rentals.First();
        var reservedStart = expectedRental.EndDate.AddHours(23);
        var reservedEnd = expectedRental.EndDate.AddDays(5);
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(expectedCar.Id, It.IsAny<CancellationToken>())).ReturnsAsync(expectedCar);
        _rentalRepositoryMock.Setup(repo => repo.GetRentalsForCarAsync(expectedCar.Id, It.IsAny<CancellationToken>())).ReturnsAsync([expectedRental]);
        _serviceRepositoryMock.Setup(repo => repo.GetServicesForCarAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync([new()]);

        var handler = new RegisterRentalCommandHandler(_mediatorMock.Object, _rentalSystem, _customerRepositoryMock.Object, _carRepositoryMock.Object);
        var command = new RegisterRentalCommand(
            expectedCustomer2.Id,
            expectedCar.Id,
            reservedStart,
            reservedEnd,
            It.IsAny<string>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Car is not available.");
    }

    [Fact]
    public async Task Can_Modify_Reservation_Dates()
    {
        // Arrange
        var expectedRental = _dbContext.Rentals.First();
        var newStartDate = expectedRental.EndDate.AddDays(2);
        var newEndDate = expectedRental.EndDate.AddDays(4);
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedRental);
        _serviceRepositoryMock.Setup(repo => repo.GetServicesForCarAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var handler = new ModifyReservationCommandHandler(_rentalSystem);
        var command = new ModifyReservationCommand(
            expectedRental.Id,
            newStartDate,
            newEndDate,
            null
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Can_Modify_Reservation_Car()
    {
        // Arrange
        var expectedRental = _dbContext.Rentals.First();
        var newCarId = Guid.NewGuid();
        var expectedCar = _dbContext.Cars.First();
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedCar);
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedRental);
        _serviceRepositoryMock.Setup(repo => repo.GetServicesForCarAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var handler = new ModifyReservationCommandHandler(_rentalSystem);
        var command = new ModifyReservationCommand(
            expectedRental.Id,
            null,
            null,
            newCarId
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Cannot_Modify_To_Overlapping_Reservation()
    {
        // Arrange
        var existingRental = _dbContext.Rentals.First();
        var overlappingStartDate = existingRental.EndDate.AddDays(-2);
        var overlappingEndDate = existingRental.EndDate.AddDays(3);
        var anotherRental = new Rental(
            _dbContext.Customers.First(),
            overlappingStartDate,
            overlappingEndDate,
            _dbContext.Cars.First()
        );
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(existingRental.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existingRental);
        _rentalRepositoryMock.Setup(repo => repo.GetRentalsForCarAsync(existingRental.Car.Id, It.IsAny<CancellationToken>())).ReturnsAsync([existingRental, anotherRental]);

        var handler = new ModifyReservationCommandHandler(_rentalSystem);
        var command = new ModifyReservationCommand(
            existingRental.Id,
            overlappingStartDate,
            overlappingEndDate,
            null
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Selected car is not available for the new dates.");
    }

    [Fact]
    public async Task Can_Cancel_Active_Rental()
    {
        // Arrange
        var expectedRental = _dbContext.Rentals.First();
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedRental);

        var handler = new CancelRentalCommandHandler(_rentalSystem);
        var command = new CancelRentalCommand(
            expectedRental.Id
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Cannot_Cancel_NonActive_Rental()
    {
        // Arrange
        var expectedRentaStartDate = DateTime.UtcNow.AddDays(-1);
        var expectedRentaEndDate = DateTime.UtcNow.AddDays(10);
        var expectedRental = new Rental(_dbContext.Customers.First(), expectedRentaStartDate, expectedRentaEndDate, _dbContext.Cars.First());
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedRental);

        var handler = new CancelRentalCommandHandler(_rentalSystem);
        var command = new CancelRentalCommand(
            expectedRental.Id
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Cannot cancel a rental that has already started.");
    }

    [Fact]
    public async Task RegisterRental_Throws_When_Car_Not_Found()
    {
        // Arrange
        var expectedCustomer = _dbContext.Customers.Skip(1).First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer);
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as Car);

        var handler = new RegisterRentalCommandHandler(_mediatorMock.Object, _rentalSystem, _customerRepositoryMock.Object, _carRepositoryMock.Object);
        var command = new RegisterRentalCommand(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            It.IsAny<string>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Car not found.");
    }

    [Fact]
    public async Task RegisterRental_Throws_When_Customer_Not_Found()
    {
        // Arrange
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as Customer);

        var handler = new RegisterRentalCommandHandler(_mediatorMock.Object, _rentalSystem, _customerRepositoryMock.Object, _carRepositoryMock.Object);
        var command = new RegisterRentalCommand(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            It.IsAny<string>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Customer not found.");
    }

    [Fact]
    public async Task RegisterRental_Throws_When_Start_Before_Today()
    {
        // Arrange
        var expectedCar = _dbContext.Cars.First();
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedCar);
        var expectedCustomer = _dbContext.Customers.First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer);

        var handler = new RegisterRentalCommandHandler(_mediatorMock.Object, _rentalSystem, _customerRepositoryMock.Object, _carRepositoryMock.Object);
        var command = new RegisterRentalCommand(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            DateTime.UtcNow.AddDays(-1),
            It.IsAny<DateTime>(),
            It.IsAny<string>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ArgumentException>();
        exception.Which.Message.Should().StartWith("Rental start date must be after today.");
        exception.And.ParamName.Should().Be("start");
    }

    [Fact]
    public async Task RegisterRental_Throws_When_End_Before_Start_Today()
    {
        // Arrange
        var expectedCar = _dbContext.Cars.First();
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedCar);
        var expectedCustomer = _dbContext.Customers.First();
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedCustomer);

        var handler = new RegisterRentalCommandHandler(_mediatorMock.Object, _rentalSystem, _customerRepositoryMock.Object, _carRepositoryMock.Object);
        var command = new RegisterRentalCommand(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(-2),
            It.IsAny<string>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ArgumentException>();
        exception.Which.Message.Should().StartWith("Rental end date must be after start date.");
        exception.And.ParamName.Should().Be("end");
    }

    [Fact]
    public async Task ModifyReservation_Throws_When_Rental_Not_Found()
    {
        // Arrange
        var expectedRental = _dbContext.Rentals.First();
        var newStartDate = expectedRental.StartDate.AddDays(-1);
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as Rental);

        var handler = new ModifyReservationCommandHandler(_rentalSystem);
        var command = new ModifyReservationCommand(
            It.IsAny<Guid>(),
            newStartDate,
            null,
            null
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Rental not found.");
    }

    [Fact]
    public async Task ModifyReservation_Throws_When_New_Selected_Car_Not_Found()
    {
        // Arrange
        var expectedRental = _dbContext.Rentals.First();
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedRental);
        _carRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as Car);

        var handler = new ModifyReservationCommandHandler(_rentalSystem);
        var command = new ModifyReservationCommand(
            It.IsAny<Guid>(),
            null,
            null,
            Guid.NewGuid()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Selected car not found.");
    }

    [Fact]
    public async Task ModifyReservation_Throws_When_Start_Before_Today()
    {
        // Arrange
        var expectedRental = _dbContext.Rentals.First();
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedRental);

        var handler = new ModifyReservationCommandHandler(_rentalSystem);
        var command = new ModifyReservationCommand(
            It.IsAny<Guid>(),
            DateTime.UtcNow.AddDays(-1),
            It.IsAny<DateTime>(),
            It.IsAny<Guid>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ArgumentException>();
        exception.Which.Message.Should().StartWith("Rental start date must be after today.");
        exception.And.ParamName.Should().Be("start");
    }

    [Fact]
    public async Task ModifyReservation_Throws_When_End_Before_Start_Today()
    {
        // Arrange
        var expectedRental = _dbContext.Rentals.First();
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedRental);

        var handler = new ModifyReservationCommandHandler(_rentalSystem);
        var command = new ModifyReservationCommand(
            It.IsAny<Guid>(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(-2),
            It.IsAny<Guid>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ArgumentException>();
        exception.Which.Message.Should().StartWith("Rental end date must be after start date.");
        exception.And.ParamName.Should().Be("end");
    }

    [Fact]
    public async Task ModifyReservation_Return_Null_When_No_Changes_Are_Requested()
    {
        // Arrange
        var handler = new ModifyReservationCommandHandler(_rentalSystem);
        var command = new ModifyReservationCommand(
            It.IsAny<Guid>(),
            null,
            null,
            null
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CancelRental_Throws_When_Rental_Not_Found()
    {
        // Arrange
        _rentalRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as Rental);

        var handler = new CancelRentalCommandHandler(_rentalSystem);
        var command = new CancelRentalCommand(
            Guid.NewGuid()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Rental not found.");
    }

    [Fact]
    public async Task RegisterCustomer_Success()
    {
        // Arrange
        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as Customer);

        var handler = new RegisterCustomerCommandHandler(_rentalSystem);
        var command = new RegisterCustomerCommand(
            "FullName",
            "Address"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisterCustomer_Throws_When_Customer_FullName_IsRrequired()
    {
        // Arrange
        var handler = new RegisterCustomerCommandHandler(_rentalSystem);
        var command = new RegisterCustomerCommand(
            string.Empty,
            "Address"
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ArgumentException>();
        exception.Which.Message.Should().StartWith("Full name is required.");
        exception.And.ParamName.Should().Be("fullName");
    }

    [Fact]
    public async Task RegisterCustomer_Throws_When_Customer_Address_IsRrequired()
    {
        // Arrange
        var expectedCustomer = _dbContext.Customers.First();
        _customerRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([expectedCustomer]);

        var handler = new RegisterCustomerCommandHandler(_rentalSystem);
        var command = new RegisterCustomerCommand(
            "John Doe",
            "123 Main St"
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Customer already registered.");
    }

    [Fact]
    public async Task RegisterCustomer_Throws_When_Customer_Already_Registered()
    {
        // Arrange
        var handler = new RegisterCustomerCommandHandler(_rentalSystem);
        var command = new RegisterCustomerCommand(
            "FullName",
            string.Empty
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ArgumentException>();
        exception.Which.Message.Should().StartWith("Address is required.");
        exception.And.ParamName.Should().Be("address");
    }
}