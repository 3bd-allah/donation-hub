using DonationHub.Api.Shared;
using DonationHub.Api.ViewModels.Campaign.Requests;
using DonationHub.Api.ViewModels.Campaign.Responses;
using DonationHub.Application.Features.Campaigns.Commands.Create;
using DonationHub.Application.Features.Campaigns.Commands.Delete;
using DonationHub.Application.Features.Campaigns.Commands.Update;
using DonationHub.Application.Features.Campaigns.Queries.GetAllActiveAdmin;
using DonationHub.Application.Features.Campaigns.Queries.GetAllActivePublic;
using DonationHub.Application.Features.Campaigns.Queries.GetAllSuccessAdmin;
using DonationHub.Application.Features.Campaigns.Queries.GetAllSuccessPublic;
using DonationHub.Application.Features.Campaigns.Queries.GetById;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DonationHub.Api.Controllers;


public class CampaignsController ( ISender sender ) : BaseController
{

    [Authorize(Roles = nameof(Roles.Admin))]
    [HttpPost]
    public async Task<ActionResult> Create(CreateCampaignRequestViewModel request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        var result = await sender.Send(new CreateCampaignCommand(
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.GoalAmount,
            request.Currency,
            request.EndDate,
            request.CategoryId,
            request.BannerFileName,
            userId
            ), ct);

        var response = ResultMapper.Map(result,
            dtoId => new {Id = dtoId},
            successMessage: "Created",
            StatusCodes.Status201Created);

        var id = response.Data?.Id;
        
        return id is null? 
            StatusCode(response.StatusCode, response) :
            CreatedAtAction(
                nameof(GetById),
                new { id },
                null);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById (Guid id, CancellationToken ct)
    {
        string language = ExtractAcceptLanguage();

        var result = await sender.Send(new GetCampaignByIdQuery(id, language), ct);

        var response = ResultMapper.Map(result,
            dto => new CampaignByIdResponseViewModel(
                dto.Id,
                dto.Title,
                dto.Description,
                dto.GoalAmount.Amount,
                dto.RaisedAmount.Amount,
                dto.GoalAmount.Currency.ToString(),
                dto.EndDate,
                dto.CreatedAt,
                dto.BannerFileName,
                dto.Status.ToString(),
                dto.CreatedByUserId,
                dto.CategoryName,
                dto.ProgressPercent,
                dto.DaysLeft
                ),
            successStatusCode: StatusCodes.Status200OK);

        return StatusCode(response.StatusCode, response);

    }

    [HttpGet]
    public async Task<ActionResult> GetAllActive (CancellationToken ct)
    {
        string language = ExtractAcceptLanguage();

        var result = await sender.Send(new GetAllActiveCampaignsQuery(language), ct);

        var response = ResultMapper.Map(result,
            res => res.Select( dto=> 
                new CampaignListResponseViewModel(
                    dto.Id,
                    dto.Title,
                    dto.Description,
                    dto.GoalAmount.Amount,
                    dto.RaisedAmount.Amount,
                    dto.GoalAmount.Currency.ToString(),
                    dto.EndDate,
                    dto.CreatedAt,
                    dto.BannerFileName,
                    dto.Status.ToString(),
                    dto.CreatedByUserId,
                    dto.CategoryName
                )
            ),
            successStatusCode: StatusCodes.Status200OK);

        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("success-stories")]
    public async Task<ActionResult> GetAllSuccess (CancellationToken ct)
    {
        string language = ExtractAcceptLanguage();

        var result = await sender.Send(new GetAllSuccessCampaignsQuery(language), ct);

        var response = ResultMapper.Map(result,
            res => res.Select(dto =>
                new CampaignListResponseViewModel(
                    dto.Id,
                    dto.Title,
                    dto.Description,
                    dto.GoalAmount.Amount,
                    dto.RaisedAmount.Amount,
                    dto.GoalAmount.Currency.ToString(),
                    dto.EndDate,
                    dto.CreatedAt,
                    dto.BannerFileName,
                    dto.Status.ToString(),
                    dto.CreatedByUserId,
                    dto.CategoryName)
            ),
            successStatusCode: StatusCodes.Status200OK);

        return StatusCode(response.StatusCode, response);

    }

    [Authorize(Roles= nameof(Roles.Admin))]
    [HttpGet("admin")]
    public async Task<ActionResult> GetAllActiveForAdmin (CancellationToken ct)
    {

        var result = await sender.Send(new GetAllActiveCampaignsForAdminQuery(), ct);

        var response = ResultMapper.Map(result,
            res => res.Select(dto =>
                new CampaignListForAdminResponseViewModel(
                    dto.Id,
                    dto.TitleAr,
                    dto.TitleEn,
                    dto.DescriptionAr,
                    dto.DescriptionEn,
                    dto.GoalAmount.Amount,
                    dto.RaisedAmount.Amount,
                    dto.GoalAmount.Currency.ToString(),
                    dto.EndDate,
                    dto.CreatedAt,
                    dto.BannerFileName,
                    dto.Status.ToString(),
                    dto.RowVersion,
                    dto.CreatedByUserId,
                    dto.CategoryNameAr,
                    dto.CategoryNameEn)
            ),
            successStatusCode: StatusCodes.Status200OK);

        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    [HttpGet("success-stories/admin")]
    public async Task<ActionResult> GetAllSuccessForAdmin(CancellationToken ct)
    {

        var result = await sender.Send(new GetAllSuccessCampaignForAdminQuery(), ct);

        var response = ResultMapper.Map(result,
            res => res.Select(dto =>
                new CampaignListForAdminResponseViewModel(
                    dto.Id,
                    dto.TitleAr,
                    dto.TitleEn,
                    dto.DescriptionAr,
                    dto.DescriptionEn,
                    dto.GoalAmount.Amount,
                    dto.RaisedAmount.Amount,
                    dto.GoalAmount.Currency.ToString(),
                    dto.EndDate,
                    dto.CreatedAt,
                    dto.BannerFileName,
                    dto.Status.ToString(),
                    dto.RowVersion,
                    dto.CreatedByUserId,
                    dto.CategoryNameAr,
                    dto.CategoryNameEn)
            ),
            successStatusCode: StatusCodes.Status200OK);

        return StatusCode(response.StatusCode, response);
    }


    [Authorize(Roles = nameof(Roles.Admin))]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateCampaignRequestViewModel request, CancellationToken ct)
    {

        var result = await sender.Send(new UpdateCampaignCommand(
            id,
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.GoalAmount,
            request.Currency,
            request.EndDate,
            request.CategoryId,
            request.BannerFileName,
            request.RowVersion
            ), ct);

        var response = ResultMapper.Map(result,
            successMessage: "Updated Successfully",
            StatusCodes.Status200OK);

        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {

        var result = await sender.Send(new DeleteCampaignCommand (id), ct);

        var response = ResultMapper.Map(result,
            successMessage: "Deleted Successfully",
            StatusCodes.Status204NoContent);

        return StatusCode(response.StatusCode, response);
    }

    private string ExtractAcceptLanguage()
    {
        string? acceptLanguage = Request.Headers.AcceptLanguage.ToString();

        string language = string.IsNullOrEmpty(acceptLanguage) ?
            WebApplicationConstants.DefaultPreferredLanguage :
            acceptLanguage[..2];

        return language;
    }

}
