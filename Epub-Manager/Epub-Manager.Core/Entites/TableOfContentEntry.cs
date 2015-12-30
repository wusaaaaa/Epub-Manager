using System.Collections.Generic;

namespace Epub_Manager.Core.Entites
{
    public class TableOfContentEntry
    {
        public string Name { get; set; }
        public List<TableOfContentEntry> SubEntries { get; set; }
    }
}