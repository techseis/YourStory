namespace YStory.Services.ExportImport
{
    public partial class ExportArticleAttribute
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeTextPrompt { get; set; }
        public bool AttributeIsRequired { get; set; }
        public int AttributeDisplaySubscription { get; set; }
        public int PictureId { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int AttributeValueTypeId { get; set; }
        public int AssociatedArticleId { get; set; }
        public int Id { get; set; }
        public int ImageSquaresPictureId { get; set; }
        public string Name { get; set; }
        public decimal WeightAdjustment { get; set; }
        public bool CustomerEntersQty { get; set; }
        public int Quantity { get; set; }
        public bool IsPreSelected { get; set; }
        public string ColorSquaresRgb { get; set; }
        public decimal PriceAdjustment { get; set; }
        public decimal Cost { get; set; }
        public int DisplaySubscription { get; set; }

        public static int ProducAttributeCellOffset = 2;
    }
}