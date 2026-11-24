using DonationHub.Application.Features.Authentication.Register;
using FluentAssertions;

namespace DonationHub.Application.Test.Features.Authentication.Register;

public class RegisterValidatorTest
{

    private readonly RegisterOrchestratorValidator _validator = new();
    private static RegisterOrchestrator CreateValidRegisterCommand()
        => new(
            "test@gmail.com",
            "Test User",
            "Test123!",
            "Test123!"
            );

    [Fact]
    public void Validate_WhenPassowrdIsValid_Success()
    {
        var command = CreateValidRegisterCommand();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }

    [Fact]
    public void Validate_WhenPasswordIsLessThanRequiredLength_ShouldFailWithErrors()
    {
        var command = CreateValidRegisterCommand() with { Password= "Test1234"};

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(command.Password));
    }

    [Fact]
    public void Validate_WhenPasswordDidnotHaveAtLeastOneUpperCharacter_ShouldFailWithErrors()
    {
        var command = CreateValidRegisterCommand() with { Password = "test123!" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(command.Password));
    }

    [Fact]
    public void Validate_WhenPasswordDidnotHaveAtLeastOneLowerCharacter_ShouldFailWithErrors()
    {
        var command = CreateValidRegisterCommand() with { Password = "TEST123!" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(command.Password));
    }

    [Fact]
    public void Validate_WhenPasswordDidnotHaveAtLeastOneUniqueCharacter_ShouldFailWithErrors()
    {
        var command = CreateValidRegisterCommand() with { Password = "Test1234" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(command.Password));
    }

    [Fact]
    public void Validate_WhenPasswordDidnotHaveAtLeastOneDigit_ShouldFailWithErrors()
    {
        var command = CreateValidRegisterCommand() with { Password = "TESTUser!" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(command.Password));
    }
}
