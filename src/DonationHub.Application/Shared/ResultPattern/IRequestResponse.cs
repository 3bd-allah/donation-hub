namespace DonationHub.Application.Shared.ResultPattern;

public interface IRequestResponse<TSelf>
    where TSelf : RequestResponse, IRequestResponse<TSelf>
{
    static abstract TSelf Fail(params Error[] errors);
}
