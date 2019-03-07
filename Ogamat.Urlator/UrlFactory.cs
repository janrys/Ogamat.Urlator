namespace Ogamat.Urlator
{
    public static class UrlFactory
    {
        public static UrlBuilder Create() { return new UrlBuilder(); }
        public static UrlBuilder Create(string url) { return new UrlBuilder(url); }
        
    }
}