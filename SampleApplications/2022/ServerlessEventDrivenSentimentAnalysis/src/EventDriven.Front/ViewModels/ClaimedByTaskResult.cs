namespace EventDriven.Front;

public class ClaimedByTaskResult
{
    public ClaimedByTaskResult(string claimedBy)
    {
        this.ClaimedBy = claimedBy ?? throw new ArgumentNullException(nameof(claimedBy));
    }
    
    public string ClaimedBy { get; set; }
}