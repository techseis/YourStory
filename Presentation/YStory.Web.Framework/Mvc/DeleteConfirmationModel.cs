namespace YStory.Web.Framework.Mvc
{
    public class DeleteConfirmationModel : BaseYStoryEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string WindowId { get; set; }
    }
}