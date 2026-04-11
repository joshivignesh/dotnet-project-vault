namespace JobBoard.Api.Models;

public sealed class JobPosting
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string WorkMode { get; set; } = string.Empty;
    public DateOnly PostedOn { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}