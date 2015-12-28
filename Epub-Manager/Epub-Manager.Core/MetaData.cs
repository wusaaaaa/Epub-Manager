using System.Collections.Generic;

namespace Epub_Manager.Core
{
    public class TableOfContentEntry
    {
        public string Name { get; set; }
        public List<TableOfContentEntry> SubEntries { get; set; }
    }

    public class MetaData
    {
        public string Title { get; set; }

        public Creator Creator { get; set; }

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

    public class Creator
    {
        public string CreatorName { get; set; }

        public string FileAs { get; set; }

        public string Role { get; set; }
    }
}