namespace Epub_Manager.Core
{
    public class MetaDataViewModel
    {
        public string Title { get; set; }

        public CreatorViewModel Creator { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }

        public string Date { get; set; }

        public string Subject { get; set; }

        public string Type { get; set; }

        public string Format { get; set; }

        public string Identifier { get; set; }

        public string Source { get; set; }

        public string Language { get; set; }
    }

    public class CreatorViewModel
    {
        public string Creator { get; set; }

        public string FileAs { get; set; }

        public string Role { get; set; }
    }
}