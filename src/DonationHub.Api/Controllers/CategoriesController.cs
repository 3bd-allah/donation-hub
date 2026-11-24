using DonationHub.Api.Shared;
using DonationHub.Api.ViewModels.Categories.Requests;
using DonationHub.Api.ViewModels.Categories.Responses;
using DonationHub.Application.Features.Categories.Commands.Create;
using DonationHub.Application.Features.Categories.Commands.Delete;
using DonationHub.Application.Features.Categories.Commands.Update;
using DonationHub.Application.Features.Categories.Queries.GetAll;
using DonationHub.Application.Features.Categories.Queries.GetById;
using DonationHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    public class CategoriesController(ISender sender) : BaseController
    {

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<EndpointResponse<List<CategoryViewModel>>>> GetAll(CancellationToken ct)
        {
            var result = await sender.Send(new CategoriesListQuery(), ct);

            var response = ResultMapper.Map(result, dtoList =>
            {
                return dtoList.Select(e => new CategoryViewModel(
                    e.Id,
                    e.NameAr,
                    e.NameEn,
                    e.DescriptionAr,
                    e.DescriptionEn)).ToList();
            });
            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid:required}")]
        public async Task<ActionResult<EndpointResponse<CategoryByIdViewModel>>> GetById(Guid id, CancellationToken ct)
        {
            var result = await sender.Send(new GetCategoryByIdQuery(id), ct);

            var response = ResultMapper.Map(result, dto =>
            {
                return new CategoryByIdViewModel(
                    dto.NameAr,
                    dto.NameEn,
                    dto.DescriptionAr,
                    dto.DescriptionEn,
                    Campaigns: [.. dto.Campaigns.Select(c => new CampaignInCategoryItemViewModel(
                        c.TitleAr,
                        c.TitleEn,
                        c.DescriptionAr,
                        c.DescriptionEn,
                        c.Status.ToString())) ]);
            });
            return StatusCode(response.StatusCode, response);
        }


        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost]
        public async Task<ActionResult<EndpointResponse>> AddNew(
            CreateCategoryRequestViewModel request,
            CancellationToken ct)
        {
            var result = await sender.Send(
                new CreateCategoryCommand(
                    request.NameAr,
                    request.NameEn, 
                    request.DescriptionAr , 
                    request.DescriptionEn), ct);

            var response = ResultMapper.Map(result, "Created", StatusCodes.Status201Created);

            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPut("{id:guid:required}")]
        public async Task<ActionResult<EndpointResponse<UpdateCategoryResponseViewModel>>> Update(
            Guid id,
            UpdateCategoryRequestViewModel request, 
            CancellationToken ct)
        {
            var result = await sender.Send(
                new UpdateCategoryCommand(id, request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn),
                ct);

            var response = ResultMapper.Map(result, dto =>
            {
                return new UpdateCategoryResponseViewModel(
                    dto.NameAr,
                    dto.NameEn,
                    dto.DescriptionAr,
                    dto.DescriptionEn);
            });
            return StatusCode(response.StatusCode, response);
        }
        

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpDelete("{id:guid:required}")]
        public async Task<ActionResult<EndpointResponse>> Delete(Guid id, CancellationToken ct)
        {
            var result = await sender.Send(new DeleteCategoryCommand(id), ct);
            var response = ResultMapper.Map(result, "Deleted Successfullt", StatusCodes.Status204NoContent);   
            return StatusCode(response.StatusCode, response);
        }
    }
}
