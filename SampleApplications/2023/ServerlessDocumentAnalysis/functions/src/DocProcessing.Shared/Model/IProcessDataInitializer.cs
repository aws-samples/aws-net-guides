namespace DocProcessing.Shared.Model;
public interface IProcessDataInitializer
{
    string ExecutionId { get; }
    string BucketName { get; }
    string Key { get; }
    string FileExtension { get; }
}
