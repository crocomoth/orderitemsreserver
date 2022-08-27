namespace OrderItemsReserver
{
    public class CosmosOptions
    {
        public const string Name = "CosmosOptions";

        public string Url { get; set; }
        public string PrimaryKey { get; set; }
        public string EventGridUrl { get; set; }
        public string EventGridKey { get; set; }
        public string LogicAppUrl { get; set; }
    }
}
