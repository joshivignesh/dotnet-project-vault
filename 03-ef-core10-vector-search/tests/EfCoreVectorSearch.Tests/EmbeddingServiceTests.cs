using EfCoreVectorSearch.Domain;

namespace EfCoreVectorSearch.Tests;

public class EmbeddingServiceTests
{
    [Fact]
    public void Generate_ShouldReturnNormalisedVector()
    {
        var embedding = EmbeddingService.Generate("dotnet csharp api");

        Assert.Equal(32, embedding.Length);
        var magnitude = MathF.Sqrt(embedding.Sum(x => x * x));
        Assert.True(MathF.Abs(magnitude - 1.0f) < 0.001f, $"Expected unit vector but magnitude was {magnitude}");
    }

    [Fact]
    public void Generate_SameInput_ProducesSameEmbedding()
    {
        var e1 = EmbeddingService.Generate("kubernetes cloud scaling");
        var e2 = EmbeddingService.Generate("kubernetes cloud scaling");

        Assert.Equal(e1, e2);
    }

    [Fact]
    public void CosineSimilarity_IdenticalVectors_ReturnsOne()
    {
        var v = EmbeddingService.Generate("docker container cloud");
        var similarity = EmbeddingService.CosineSimilarity(v, v);
        Assert.True(MathF.Abs(similarity - 1.0f) < 0.001f);
    }

    [Fact]
    public void CosineSimilarity_RelatedTopics_HigherThanUnrelated()
    {
        var query = EmbeddingService.Generate("dotnet csharp api architecture");
        var related = EmbeddingService.Generate("clean architecture cqrs dotnet csharp");
        var unrelated = EmbeddingService.Generate("kubernetes docker cloud azure container");

        var simRelated = EmbeddingService.CosineSimilarity(query, related);
        var simUnrelated = EmbeddingService.CosineSimilarity(query, unrelated);

        Assert.True(simRelated > simUnrelated,
            $"Expected related ({simRelated:F4}) > unrelated ({simUnrelated:F4})");
    }

    [Fact]
    public void CosineSimilarity_EmptyVectors_ReturnsZero()
    {
        var result = EmbeddingService.CosineSimilarity([], []);
        Assert.Equal(0f, result);
    }

    [Fact]
    public void CosineSimilarity_MismatchedDimensions_ReturnsZero()
    {
        var result = EmbeddingService.CosineSimilarity([1f, 2f], [1f, 2f, 3f]);
        Assert.Equal(0f, result);
    }
}
