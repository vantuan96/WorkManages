namespace Kztek_Library.Models
{
    public class AuthPartialModel
    {
        public AuthActionModel Auth_Value { get; set; }

        public object model { get; set; }

        public string ControllerName { get; set; } = "";

        public string ActionName { get; set; } = "";

        public string RecordId { get; set; } = "";

        public bool IsUsingAjax { get; set; } = false;
    }
}