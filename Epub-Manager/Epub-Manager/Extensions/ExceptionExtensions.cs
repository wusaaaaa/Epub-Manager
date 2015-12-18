using System;
using System.Text;

namespace Epub_Manager.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception self)
        {
            var result = new StringBuilder();

            do
            {
                result.AppendLine(self.Message);
            } while ((self = self.InnerException) != null);

            return result.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
    }
}