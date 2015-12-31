using DevExpress.Utils;
using Epub_Manager.Core.Services;
using Epub_Manager.Windsor.Interceptors;
using System.IO;

namespace Epub_Manager.Services
{
    public class RenameService : IRenameService
    {
        [CatchException("Error while renaming the file")]
        public void RenameFile(FileInfo file, string fileName)
        {
            Guard.ArgumentNotNull(fileName, nameof(fileName));
            Guard.ArgumentNotNull(file, nameof(file));

            if (File.Exists(file.FullName))
            {
                var newFileName = file.FullName.Replace(file.Name, fileName);
                File.Move(file.FullName, newFileName);
            }
        }
    }
}