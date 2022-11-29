using Newtonsoft.Json;

namespace PhreesiaAPI
{
    class Results
    {
        [JsonProperty("numFound")]
        public int? TotalBooks { get; set; }
        public int? start { get; set; }
        public bool? numFoundExact { get; set; }
        [JsonProperty("docs")]
        public List<BookList>? Books { get; set; }
        public int? num_found { get; set; }
        public string? q { get; set; }
        public object? offset { get; set; }
    }
}
