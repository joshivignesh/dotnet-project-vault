namespace EfCoreVectorSearch.Domain;

public class Document
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public float[] Embedding { get; set; } = [];
	public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
