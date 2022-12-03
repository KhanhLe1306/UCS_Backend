namespace UCS_Backend.Models.SubModels
{
    public class SuccessInfo
    {
        public bool success { get; set; }
        public List<Dictionary<string, string>> messages { get; set; }
    }
}
