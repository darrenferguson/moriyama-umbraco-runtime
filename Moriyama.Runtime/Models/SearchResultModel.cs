namespace Moriyama.Runtime.Models
{
    public class SearchResultModel
    {
        public double Score { get; set; }
        public string PreviewText { get; set; }
        public string Url { get; set; }
        public RuntimeContentModel Content { get; set; }
    }
}
