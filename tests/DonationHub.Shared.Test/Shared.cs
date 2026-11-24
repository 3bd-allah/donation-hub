using DonationHub.Domain.Entities;
using DonationHub.Domain.Exceptions;
using DonationHub.Domain.ValueObjects;

namespace DonationHub.Shared.Test
{
    public static class Shared
    {
        public static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

        public static readonly DateTimeOffset FutureDate =
            Now.AddDays(10);

        public static readonly Guid CategoryId =
            Guid.NewGuid();

        public static readonly Guid CreatedByUserId =
            Guid.NewGuid();

        public static Campaign CreateCampaign(
            decimal goalAmount = 100m,
            DateTimeOffset? endDate = null)
        {
            return Campaign.Create(
                titleAr: "حملة تبرعات",
                titleEn: "Donation Campaign",
                descriptionAr: "وصف الحملة",
                descriptionEn: "Campaign description",
                goalAmount: new Money(goalAmount, "USD"),
                endDate: endDate ?? FutureDate,
                categoryId: CategoryId,
                bannerFileName: null,
                createdByUserId: CreatedByUserId,
                utcNow: Now);
        }

        public static Money CreateMoney(decimal amount)
            => new(amount, "USD");

       
    }
}
