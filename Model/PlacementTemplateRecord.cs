using Newtonsoft.Json;

namespace SiloModelingTaskClient
{
    public class PlacementTemplateRecord
    {
        [JsonProperty("elementId")]
        public int ElementId { get; set; }

        [JsonProperty("familyName")]
        public string FamilyName { get; set; }

        [JsonProperty("symbolName")]
        public string SymbolName { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("z")]
        public double Z { get; set; }
    }
}
