using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace DonationHub.Infrastructure.Options;

public class ConnectionStringOptions
{
    public const string ConnectionStringSettings = "ConnectionStrings";
    public const string SqlServerConnectionStringName = "DefaultConnection";

    [Required]
    public string DefaultConnection { get; set; } = null!;
}

