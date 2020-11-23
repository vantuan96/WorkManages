namespace Kztek_Library.Models
{
    public class AJAXModel
    {
        public string id { get; set; }

        public string url { get; set; }
    }

    public class AJAXModel_Modal
    {
        public string url { get; set; } = "";

        public string idrecord { get; set; } = "";

        public string idmodal { get; set; } = "";

        public string idboxrender { get; set; } = "";

        public string isupdate { get; set; } = "0"; //0 - Create (false) 1 - Update (true)

        public string title { get; set; } = "";

        public string idsub { get; set; } = "";
    }
}