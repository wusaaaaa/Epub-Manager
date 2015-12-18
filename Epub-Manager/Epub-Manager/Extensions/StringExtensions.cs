using DevExpress.Utils;

namespace Epub_Manager.Extensions
{
    public static class StringExtensions
    {
        public static string EnsureIsShortcut(this string self)
        {
            Guard.ArgumentNotNull(self, nameof(self));

            if (self.EndsWith("...") == false && self.EndsWith("…") == false)
                return self + "…";

            return self;
        }
    }
}