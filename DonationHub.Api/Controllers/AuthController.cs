using Azure.Core;
using DonationHub.Api.Shared;
using DonationHub.Api.ViewModels.Authentication.Requests;
using DonationHub.Api.ViewModels.Authentication.Responses;
using DonationHub.Application.Features.Authentication.CurrentProfile;
using DonationHub.Application.Features.Authentication.Login;
using DonationHub.Application.Features.Authentication.Logout;
using DonationHub.Application.Features.Authentication.Refresh;
using DonationHub.Application.Features.Authentication.Register;
using DonationHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace DonationHub.Api.Controllers;

public class AuthController(IMediator mediator) : BaseController
{
    [AllowAnonymous]
    [HttpPost("auth/register")]
    public async Task<ActionResult<EndPointResponse>> Register(RegisterRequestViewModel registerVM, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterCommand(
            registerVM.Email, 
            registerVM.FullName, 
            registerVM.Password,
            registerVM.ConfirmPassword), ct);

        if (result.Success)
        {
            return EndPointResponse.Ok(StatusCodes.Status201Created);
        }
        else
        {
            return EndPointResponse.Fail(StatusCodes.Status409Conflict, result.Errors);
        }
    }

    [AllowAnonymous]
    [HttpPost("auth/login")]
    public async Task<ActionResult<EndPointResponse<LoginResponseViewModel>>> Login(
        LoginRequestViewModel loginViewModel, 
        CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(loginViewModel.Email, loginViewModel.Password), ct);
        if (!result.Success)
            return EndPointResponse<LoginResponseViewModel>.Fail(result.Code, result.Errors!);

        var resultData = result.Data;
        var loginResponse = new LoginResponseViewModel(resultData!.Id,
            resultData.Email,
            resultData.Roles,
            resultData.AccessToken,
            resultData.AccessTokenExpiresAt,
            resultData.RefreshToken);

        return EndPointResponse<LoginResponseViewModel>.Ok(result.Code, loginResponse);
    }

    [AllowAnonymous]
    [HttpPost("auth/refresh")]
    public async Task<ActionResult<EndPointResponse<RefreshResponseViewModel>>> Refresh(
        RefreshRequestViewModel request,
        CancellationToken ct)
    {
        var result = await mediator.Send(new RotateRefreshTokenCommand(request.RefreshToken), ct);

        if (!result.Success)
            return EndPointResponse<RefreshResponseViewModel>.Fail(result.Code, result.Errors!);

        var response = new RefreshResponseViewModel(
            result.Data!.NewAccessToken,
            result.Data.AccessTokenExpiration,
            result.Data.NewRefreshToken);
        return EndPointResponse<RefreshResponseViewModel>.Ok(result.Code, response); 
    }


    [Authorize]
    [HttpPost("auth/logout")]
    public async Task<ActionResult<EndPointResponse>> Logout(LogoutRequestViewModel request, CancellationToken ct)
    {
        var result = await mediator.Send(new LogoutCommand(request.RefreshToken), ct);
        if (!result.Success)
            return EndPointResponse.Fail(result.Code);
        return EndPointResponse.Ok(result.Code);
    }


    [Authorize]
    [HttpGet("auth/me")]
    public async Task<ActionResult<EndPointResponse<CurrentProfileViewModel>>> Me(MeRequestViewModel request, CancellationToken ct)
    {
        var result = await mediator.Send(new CurrentProfileQuery(request.Id), ct);
        if (!result.Success)
            return EndPointResponse<CurrentProfileViewModel>.Fail(result.Code);
        var profileResponse = new CurrentProfileViewModel(
            result.Data!.Id,
            result.Data.Email,
            result.Data.FullName,
            result.Data.Roles);
        return EndPointResponse<CurrentProfileViewModel>.Ok(result.Code, profileResponse);
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    [HttpPost("admin/staff")]
    public async Task<ActionResult<EndPointResponse>> AdminStaff()
    {
        return Ok("hello from admin page");
    }

}
