using System;
using System.Collections.Generic;

namespace Epub_Manager.Views.MetaData
{
    public class MetaData
    {
        public string Title { get; set; }

        public ICollection<MetaDataViewModel> Creator { get; set; }

        public ICollection<string> Subjects { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; }

        public string Format { get; set; }

        public string Identifier { get; set; }

        public string Source { get; set; }

        public string Language { get; set; }
    }

    internal class MetaDataCrator
    {
        public string Creator { private get; set; }
        public string Role { private get; set; }
        public string FileAs { private get; set; }

        public MetaDataCrator(string creator, string role, string fileAs)
        {
            this.Creator = creator;
            this.Role = role;
            this.FileAs = fileAs;
        }
    }
}