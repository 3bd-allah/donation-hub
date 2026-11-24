using DonationHub.Api.Shared;
using DonationHub.Api.ViewModels.Authentication.Requests;
using DonationHub.Api.ViewModels.Authentication.Responses;
using DonationHub.Application.Features.Authentication.CurrentProfile;
using DonationHub.Application.Features.Authentication.Login;
using DonationHub.Application.Features.Authentication.Logout;
using DonationHub.Application.Features.Authentication.Register;
using DonationHub.Application.Features.RefreshTokens.Orchestrators.Refresh;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

public class AuthController(ISender sender) : BaseController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<EndpointResponse>> Register(RegisterRequestViewModel registerVM, CancellationToken ct)
    {
        var result = await sender.Send(new RegisterOrchestrator(
            registerVM.Email, 
            registerVM.FullName, 
            registerVM.Password,
            registerVM.ConfirmPassword,
            registerVM.PreferredLanguage), ct);

        var response = ResultMapper.Map(
            result: result,
            successMessage: "Registered successfully",
            successStatusCode: StatusCodes.Status201Created);

        return StatusCode(response.StatusCode, response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<EndpointResponse<LoginResponseViewModel>>> Login(
        LoginRequestViewModel loginViewModel, 
        CancellationToken ct)
    {
        var result = await sender.Send(new LoginOrchestrator(loginViewModel.Email, loginViewModel.Password), ct);

        var response = ResultMapper.Map(result, dto =>
        {
            return new LoginResponseViewModel(
                dto.Id,
                dto.Email,
                dto.Roles,
                dto.AccessToken,
                dto.AccessTokenExpiresAt,
                dto.RefreshToken);
        } ,
        "Login Successfully",
        200);

        return StatusCode(response.StatusCode, response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<EndpointResponse<RefreshResponseViewModel>>> Refresh(
        RefreshRequestViewModel request,
        CancellationToken ct)
    {
        var result = await sender.Send(new RotateRefreshTokenOrchestrator(request.RefreshToken), ct);

        var response = ResultMapper.Map(result, dto =>
        {
            return new RefreshResponseViewModel(
                dto.NewAccessToken, 
                dto.AccessTokenExpiration, 
                dto.NewRefreshToken);
        });
        
        return StatusCode(response.StatusCode, response); 
    }


    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<EndpointResponse>> Logout(LogoutRequestViewModel request, CancellationToken ct)
    {
        var result = await sender.Send(new LogoutOrchestrator(request.RefreshToken), ct);
        var response = ResultMapper.Map(result,"Loggedout successfuly");
        

        return StatusCode(response.StatusCode, response);
    }


    [Authorize]
    [HttpGet("me/{id:guid}")]
    public async Task<ActionResult<EndpointResponse<CurrentProfileViewModel>>> Me(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetCurrentProfileOrchestrator(id), ct);
        var response = ResultMapper.Map(result, dto =>
        {
            return new CurrentProfileViewModel(
                dto.Id,
                dto.Email,
                dto.FullName,
                dto.Roles);
        });
        return StatusCode(response.StatusCode, response);
    }

    

}
