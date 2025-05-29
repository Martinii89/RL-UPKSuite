using System.ComponentModel.DataAnnotations;

namespace RlUpk.RLUpkSuite.AppSettings;

public class Deployment
{
    public const string Section = "Deployment";

    [Required] public string Source { get; set; } = null!;

    [Required]
    [AllowedValues("github", "local")]
    public string SourceType { get; set; } = null!;
}