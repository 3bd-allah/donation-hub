using DonationHub.Domain.Enums;
using DonationHub.Domain.Interfaces;
using DonationHub.Domain.Primitives;

namespace DonationHub.Domain.Entities;

public sealed class PaymentAttempt : Entity, IAuditable
{
    public Guid DonationId { get; private set; }
    public Guid StripeCheckoutSessionId { get; private set; }
    public Guid? StripePaymentId { get; private set; }

    public double Amount { get; private set; }
    public PaymentAttemptStatus Status { get; private set; }

    public int? FailureCode{ get; private set; }

    // Audit
    public DateTimeOffset? UpdatedAt { get;  set; }
    public DateTimeOffset CreatedAt { get;  set; }
    
}
