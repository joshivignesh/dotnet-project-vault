namespace EfCoreVectorSearch.Domain;

/// <summary>
/// Provides deterministic pseudo-embeddings for demo/test purposes.
/// In production, replace with a real embedding API (OpenAI, Azure OpenAI, etc.)
/// </summary>
public static class EmbeddingService
{
    private const int Dimensions = 32;

    // Fixed vocabulary mapped to dimension indices
    private static readonly Dictionary<string, int[]> _termDimensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["dotnet"] = [0, 1, 15],
        ["csharp"] = [0, 2, 16],
        ["api"] = [3, 4, 17],
        ["web"] = [3, 5, 18],
        ["database"] = [6, 7, 19],
        ["sql"] = [6, 8, 20],
        ["cloud"] = [9, 10, 21],
        ["azure"] = [9, 11, 22],
        ["kubernetes"] = [9, 12, 23],
        ["docker"] = [9, 13, 24],
        ["microservices"] = [3, 9, 25],
        ["distributed"] = [9, 14, 26],
        ["performance"] = [15, 16, 27],
        ["caching"] = [6, 17, 27],
        ["security"] = [18, 19, 28],
        ["authentication"] = [18, 20, 28],
        ["testing"] = [21, 22, 29],
        ["architecture"] = [0, 23, 30],
        ["clean"] = [0, 24, 30],
        ["cqrs"] = [0, 25, 31],
        ["signalr"] = [3, 26, 17],
        ["realtime"] = [3, 27, 17],
        ["messaging"] = [3, 28, 18],
        ["search"] = [29, 30, 19],
        ["vector"] = [29, 31, 20],
        ["ai"] = [29, 30, 21],
        ["machine"] = [29, 30, 22],
        ["learning"] = [29, 31, 23],
    };

    /// <summary>
    /// Generates a semantic embedding vector for the given text.
    /// Uses a vocabulary-based approximation for demo purposes.
    /// </summary>
    public static float[] Generate(string text)
    {
        var embedding = new float[Dimensions];

        // Add base noise from text hash for uniqueness
        var hash = text.GetHashCode();
        for (int i = 0; i < Dimensions; i++)
            embedding[i] = 0.1f * (float)Math.Sin(hash * (i + 1) * 0.1);

        // Boost dimensions for recognized vocabulary terms
        var words = text.ToLowerInvariant()
            .Split([' ', ',', '.', '-', '_', '/', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            if (_termDimensions.TryGetValue(word, out var dims))
            {
                foreach (var dim in dims)
                    embedding[dim] += 1.0f;
            }
        }

        return Normalize(embedding);
    }

    /// <summary>Computes cosine similarity between two vectors. Returns value in [-1, 1].</summary>
    public static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length) return 0f;

        float dot = 0f, normA = 0f, normB = 0f;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        var denominator = MathF.Sqrt(normA) * MathF.Sqrt(normB);
        return denominator == 0f ? 0f : dot / denominator;
    }

    private static float[] Normalize(float[] v)
    {
        var norm = MathF.Sqrt(v.Sum(x => x * x));
        if (norm == 0f) return v;
        return v.Select(x => x / norm).ToArray();
    }
}
