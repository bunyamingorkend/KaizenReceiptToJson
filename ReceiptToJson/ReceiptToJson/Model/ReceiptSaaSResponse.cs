namespace ReceiptToJson.Model
{
    public class ReceiptSaaSResponse
    {
        public string? locale { get; set; }
        public string description { get; set; }
        public BoundingPoly boundingPoly { get; set; } = new();
    }

    public class BoundingPoly
    {
        public List<Vertex> vertices { get; set; } = new();
    }

    public class Vertex
    {
        public int x { get; set; }
        public int y { get; set; }
    }

}
