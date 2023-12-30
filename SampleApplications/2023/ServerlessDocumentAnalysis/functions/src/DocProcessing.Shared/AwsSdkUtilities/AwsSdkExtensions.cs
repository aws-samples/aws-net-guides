using Amazon.S3.Model;

namespace DocProcessing.Shared.AwsSdkUtilities;

public static class AwsSdkExtensions
{
    // Tagging Extensions
    public static string GetTagValue(this List<Tag> tagging, string key) =>
        tagging.FirstOrDefault(a => a.Key == key)?.Value;

    public static IEnumerable<string> GetTagValueList(this List<Tag> tagging, string key, string separator = ":") =>
        tagging.FirstOrDefault(a => a.Key == key)?.Value?.Split(separator) ?? Enumerable.Empty<string>();

}
