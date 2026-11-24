using DonationHub.Api.Shared;
using DonationHub.Api.ViewModels.Admin.Requests;
using DonationHub.Application.Features.Authentication.CreateUser;
using DonationHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminController (ISender sender ) : BaseController
    {
        [HttpPost("staff")]
        public async Task<ActionResult<EndpointResponse>> AdminStaff(CreateUserRequestViewModel request, CancellationToken ct)
        {
            var result = await sender.Send(new CreateUserOrchestrator(
                request.Email,
                request.FullName,
                request.Password,
                request.ConfirmPassword,
                request.Role,
                request.PreferredLanguage!), ct);

            var response = ResultMapper.Map(result,
                successMessage: "User Created Successfully",
                successStatusCode: 201);

            return StatusCode(response.StatusCode, response);
        }
    }
}
