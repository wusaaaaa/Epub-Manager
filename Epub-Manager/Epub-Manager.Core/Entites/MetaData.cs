using System;

namespace Epub_Manager.Core.Entites
{
    public class MetaData
    {
        public string Title { get; set; }

        public Creator Creator { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }

        public DateTime? Date { get; set; }

        public string Subject { get; set; }

        public string Type { get; set; }

        public string Format { get; set; }

        public string Identifier { get; set; }

        public string Source { get; set; }

        public string Language { get; set; }
    }
}