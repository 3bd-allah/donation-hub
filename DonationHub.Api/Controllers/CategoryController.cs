using DonationHub.Api.Shared;
using DonationHub.Api.ViewModels.Categories.Requests;
using DonationHub.Api.ViewModels.Categories.Responses;
using DonationHub.Application.Features.Categories.Create;
using DonationHub.Application.Features.Categories.Delete;
using DonationHub.Application.Features.Categories.GetAll;
using DonationHub.Application.Features.Categories.GetById;
using DonationHub.Application.Features.Categories.Update;
using DonationHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    public class CategoryController(IMediator mediator) : BaseController
    {

        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<ActionResult<EndPointResponse<List<CategoryViewModel>>>> GetAll(CancellationToken ct)
        {
            var result = await mediator.Send(new CategoriesListQuery(), ct);
            if (!result.Success || result.Data is null)
                return EndPointResponse<List<CategoryViewModel>>.Fail(StatusCodes.Status404NotFound);

            var resultData = result.Data;

            var response = resultData
                .Select(c => new CategoryViewModel(c.Id, c.NameAr, c.NameEn, c.DescriptionAr, c.DescriptionEn))
                .ToList();
            return EndPointResponse<List<CategoryViewModel>>.Ok(StatusCodes.Status200OK, response);
        }

        [AllowAnonymous]
        [HttpGet("categories/{id:guid:required}")]
        public async Task<ActionResult<EndPointResponse<CategoryByIdViewModel>>> GetById(Guid id, CancellationToken ct)
        {
            var result = await mediator.Send(new GetCategoryByIdQuery(id), ct);

            if (!result.Success)
                return EndPointResponse<CategoryByIdViewModel>.Fail(StatusCodes.Status404NotFound, result.Errors);

            var resultData = result.Data;

            var response = new CategoryByIdViewModel(
                resultData!.NameAr,
                resultData.NameEn,
                resultData.DescriptionAr,
                resultData.DescriptionEn,
                Campaigns: resultData.Campaigns.Select(c =>
                new CampaignInCategoryItemViewModel(
                    c.TitleAr,
                    c.TitleEn,
                    c.DescriptionAr,
                    c.DescriptionEn,
                    c.Status
                    )).ToList()
                );
            return EndPointResponse<CategoryByIdViewModel>.Ok(StatusCodes.Status200OK, response);
        }


        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost("categories")]
        public async Task<ActionResult<EndPointResponse>> AddNew(
            CreateCategoryRequestViewModel request,
            CancellationToken ct)
        {
            var result = await mediator.Send(
                new CreateCategoryCommand(
                    request.NameAr,
                    request.NameEn, 
                    request.DescriptionAr , 
                    request.DescriptionEn), ct);

            if (!result.Success)
                return EndPointResponse.Fail(StatusCodes.Status400BadRequest, result.Errors);

            return EndPointResponse.Ok(StatusCodes.Status201Created);
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPut("categories/{id:guid:required}")]
        public async Task<ActionResult<EndPointResponse<UpdateCategoryResponseViewModel>>> Update(
            Guid id,
            UpdateCategoryRequestViewModel request, 
            CancellationToken ct)
        {
            var result = await mediator.Send(
                new UpdateCategoryCommand(id, request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn),
                ct);

            if (!result.Success )
                //map for the error type code and based on that code return the proper status code 
                return EndPointResponse<UpdateCategoryResponseViewModel>.Fail(result.Code, result.Errors);

            if (result.Data is null)
                return EndPointResponse<UpdateCategoryResponseViewModel>.Fail(StatusCodes.Status204NoContent, result.Errors);

            var resultData = result.Data;
            var response = new UpdateCategoryResponseViewModel(
                resultData.NameAr,
                resultData.NameEn,
                resultData.DescriptionAr,
                resultData.DescriptionEn);

            return EndPointResponse<UpdateCategoryResponseViewModel>.Ok(StatusCodes.Status200OK, response);
        }
        

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpDelete("categories/{id:guid:required}")]
        public async Task<ActionResult<EndPointResponse>> Delete(Guid id, CancellationToken ct)
        {
            var result = await mediator.Send(new DeleteCategoryCommand(id), ct);
            if (!result.Success)
                return EndPointResponse.Fail(result.Code, result.Errors);

            return EndPointResponse.Ok(StatusCodes.Status204NoContent);
        }
    }
}
