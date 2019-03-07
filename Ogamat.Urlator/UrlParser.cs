using System;
using System.Collections.Generic;
using System.Linq;

namespace Ogamat.Urlator
{
    public class UrlParser
    {
        public static UrlParser GetParser(string url) { return new UrlParser(url); }
        public static bool Validate(string url) { return new UrlParser(url).IsValid; }

        private UrlBuilder.QueryParameterSeparator queryParameterSeparator;

        public UrlParser(string url) : this(url, UrlBuilder.QueryParameterSeparator.Ampersand)
        { }

        public UrlParser(string url, UrlBuilder.QueryParameterSeparator queryParameterSeparator)
        {
            this.queryParameterSeparator = queryParameterSeparator;
            this.Url = url;
        }

        public string Scheme { get; private set; }
        public bool HasScheme => string.IsNullOrEmpty(this.Scheme);
        public string User { get; private set; }
        public bool HasUser => string.IsNullOrEmpty(this.User);
        public string Password { get; private set; }
        public bool HasPassword => string.IsNullOrEmpty(this.Password);
        public string Host { get; private set; }
        public bool HasHost => string.IsNullOrEmpty(this.Host);
        public string Port { get; private set; }
        public bool HasPort => string.IsNullOrEmpty(this.Port);
        public string Path { get; private set; }
        public bool HasPath => string.IsNullOrEmpty(this.Path);
        public string Query { get; private set; }
        public bool HasQuery => string.IsNullOrEmpty(this.Query);
        public IEnumerable<KeyValuePair<string, string>> QueryParameters { get; private set; }
        public string Fragment { get; private set; }
        public bool HasFragment => string.IsNullOrEmpty(this.Fragment);
        public IEnumerable<KeyValuePair<string, string>> FragmentParameters { get; private set; }

        private string url;
        public string Url
        {
            get => this.url;
            set
            {
                this.url = value;
                this.Parse();
            }
        }

        private void Parse()
        {
            // https://tools.ietf.org/html/rfc3986#page-16
            // URI         = scheme ":" hier-part [ "?" query ] [ "#" fragment ]

            int schemeSeparatorIndex = this.Url.IndexOf(SCHEME_TERMINATOR, StringComparison.InvariantCultureIgnoreCase);
            int querySeparatorIndex = this.Url.IndexOf(QUERY_SEPARATOR, StringComparison.InvariantCultureIgnoreCase);
            int fragmentSeparatorIndex = this.Url.IndexOf(FRAGMENT_SEPARATOR, StringComparison.InvariantCultureIgnoreCase);

            if (fragmentSeparatorIndex > 0 && (fragmentSeparatorIndex < schemeSeparatorIndex || fragmentSeparatorIndex < querySeparatorIndex))
            {
                this.IsValid = false;
                return;
            }

            if (querySeparatorIndex > 0 && (querySeparatorIndex < schemeSeparatorIndex))
            {
                this.IsValid = false;
                return;
            }

            if (schemeSeparatorIndex > 0)
            {
                this.Scheme = this.Url.Substring(0, schemeSeparatorIndex);
            }
            else
            {
                this.IsValid = false;
                return;
            }

            int hierPartStartIndex = schemeSeparatorIndex + 1;
            while (this.Url[hierPartStartIndex] == '/')
            {
                hierPartStartIndex++;
            }

            int hierPartEndIndex;
            if (querySeparatorIndex > 0)
            {
                hierPartEndIndex = querySeparatorIndex;
            }
            else if(fragmentSeparatorIndex > 0)
            {
                hierPartEndIndex = fragmentSeparatorIndex;
            }
            else
            {
                hierPartEndIndex = this.Url.Length;
            }

            string hierPart = this.Url.Substring(hierPartStartIndex, hierPartEndIndex - hierPartStartIndex);

            if (fragmentSeparatorIndex > 0)
            {
                if (querySeparatorIndex > 0)
                {
                    this.Fragment = this.Url.Substring(fragmentSeparatorIndex + FRAGMENT_SEPARATOR.Length, this.Url.Length - (fragmentSeparatorIndex + FRAGMENT_SEPARATOR.Length));
                    this.Query = this.Url.Substring(querySeparatorIndex + QUERY_SEPARATOR.Length, fragmentSeparatorIndex - (querySeparatorIndex + QUERY_SEPARATOR.Length));
                }
                else
                {
                    this.Fragment = this.Url.Substring(fragmentSeparatorIndex + FRAGMENT_SEPARATOR.Length);
                }
                
            }
            else
            {
                if (querySeparatorIndex > 0)
                {
                    this.Query = this.Url.Substring(querySeparatorIndex + QUERY_SEPARATOR.Length);
                }
            }

            if (!String.IsNullOrEmpty(this.Query))
            {
                this.QueryParameters = this.GetKeyValues(this.Query);
            }

            if (!String.IsNullOrEmpty(this.Fragment))
            {
                this.FragmentParameters = this.GetKeyValues(this.Fragment);
            }

            // authority   = [ userinfo "@" ] host [ ":" port ] [/path]
            int userInfoSeparatorIndex = hierPart.IndexOf(PASSWORD_TERMINATOR, StringComparison.InvariantCultureIgnoreCase);
            int portSeparatorIndex = hierPart.IndexOf(PORT_SEPARATOR, Math.Max(0, userInfoSeparatorIndex),StringComparison.InvariantCultureIgnoreCase);
            int pathSeparatorIndex = hierPart.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
            int hostStartIndex = 0;
            int hostEndIndex = hierPart.Length;

            if (userInfoSeparatorIndex > 0 && (userInfoSeparatorIndex > portSeparatorIndex || userInfoSeparatorIndex > pathSeparatorIndex
                || portSeparatorIndex > pathSeparatorIndex))
            {
                this.IsValid = false;
                return;
            }

            if (pathSeparatorIndex > 0 && (pathSeparatorIndex < portSeparatorIndex ))
            {
                this.IsValid = false;
                return;
            }

            string userInfo = "";

            if (userInfoSeparatorIndex > 0)
            {
                userInfo = hierPart.Substring(0, userInfoSeparatorIndex);
                hostStartIndex = userInfoSeparatorIndex + 1;

                int passwordSeparatorIndex = userInfo.IndexOf(USER_TERMINATOR, StringComparison.InvariantCultureIgnoreCase);

                if (passwordSeparatorIndex > 0)
                {
                    this.User = userInfo.Substring(0, passwordSeparatorIndex);
                    this.Password = userInfo.Substring(passwordSeparatorIndex + USER_TERMINATOR.Length);
                }
            }

            if (pathSeparatorIndex > 0)
            {
                this.Path = hierPart.Substring(pathSeparatorIndex + 1);
                hostEndIndex = pathSeparatorIndex;
            }

            if (portSeparatorIndex > 0)
            {
                this.Port = hierPart.Substring(portSeparatorIndex + 1, hostEndIndex - portSeparatorIndex - 1);
                hostEndIndex = portSeparatorIndex;
            }

            this.Host = hierPart.Substring(hostStartIndex, hostEndIndex - hostStartIndex);
        }

        private IEnumerable<KeyValuePair<string, string>> GetKeyValues(string query)
        {
            string[] keyValues = query.Split(new string[] { this.queryParameterSeparator.GetString() }, StringSplitOptions.None);
            List<KeyValuePair<string, string>> keyValueList = new List<KeyValuePair<string, string>>();

            foreach (string keyValue in keyValues)
            {
                string[] splitKeyValue = keyValue.Split(new char[] { '=' }, 2);
                KeyValuePair<string, string> keyValuePair;

                if (splitKeyValue.Length == 2)
                {
                    keyValuePair = new KeyValuePair<string, string>(splitKeyValue[0], splitKeyValue[1]);
                }
                else if(splitKeyValue.Length == 1)
                {
                    keyValuePair = new KeyValuePair<string, string>(splitKeyValue[0], null);
                }

                keyValueList.Add(keyValuePair);
            }

            return keyValueList.ToArray();
        }

        public bool IsValid { get; private set; }

        private const string SCHEME_TERMINATOR = ":";
        private const string USER_TERMINATOR = ":";
        private const string PORT_SEPARATOR = ":";
        private const string PASSWORD_TERMINATOR = "@";
        private const string PATH_SEPARATOR = "/";
        private const string QUERY_SEPARATOR = "?";
        private const string FRAGMENT_SEPARATOR = "#";
    }
}
