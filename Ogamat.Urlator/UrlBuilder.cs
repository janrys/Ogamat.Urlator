using System;
using System.Text;

namespace Ogamat.Urlator
{
    public class UrlBuilder
    {
        private QueryParameterSeparator separator = QueryParameterSeparator.Ampersand;

        public UrlBuilder()
        {

        }

        public UrlBuilder(string url)
        {
            UrlParser parser = new UrlParser(url);

            this.Host(parser.Host)
                .User(parser.User)
                .Password(parser.Password)
                .Host(parser.Host)
                .Port(parser.Port)
                .Path(parser.Path)
                .Query(parser.Query)
                .Fragment(parser.Fragment);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(this.schema);
            builder.Append("://");
            builder.Append(this.user);
            builder.Append(":");
            builder.Append(this.password);
            builder.Append("@");
            builder.Append(this.host);
            builder.Append("/");
            builder.Append(this.path);
            builder.Append("?");
            builder.Append(this.query);
            builder.Append("#");
            builder.Append(this.fragment);

            return builder.ToString();
        }

        public UrlBuilder SetParameterSeparator(QueryParameterSeparator separator)
        {
            this.separator = separator;
            return this;
        }

        public UrlBuilder Schema(string schema)
        {
            this.schema = schema;
            return this;
        }

        public UrlBuilder User(string user)
        {
            this.user = user;
            return this;
        }

        public UrlBuilder Password(string password)
        {
            this.password = password;
            return this;
        }

        public UrlBuilder Host(string host)
        {
            this.host = host;
            return this;
        }

        public UrlBuilder Port(string port)
        {
            this.port = port;
            return this;
        }
        public UrlBuilder Path(string path)
        {
            this.path = path;
            return this;
        }
        public UrlBuilder Query(string query)
        {
            this.query = query;
            return this;
        }
        public UrlBuilder AddQuery(string key, string value)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                return this;
            }

            if (string.IsNullOrEmpty(this.query))
            {
                this.query = "";
            }
            else
            {
                this.query += this.separator.GetString();
            }

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                this.query += string.IsNullOrEmpty(value) ? key : value;
            }
            else
            {
                this.query += $"{key}={value}";
            }

            return this;
        }
        public UrlBuilder Fragment(string fragment)
        {
            this.fragment = fragment;
            return this;
        }

        public UrlBuilder AddFragment(string key, string value)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                return this;
            }

            if (string.IsNullOrEmpty(this.fragment))
            {
                this.fragment = "";
            }
            else
            {
                this.fragment += this.separator.GetString();
            }

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                this.fragment += string.IsNullOrEmpty(value) ? key : value;
            }
            else
            {
                this.fragment += $"{key}={value}";
            }

            return this;
        }

        public bool IsValid() => UrlParser.Validate(this.ToString());

        private string schema;
        private string user;
        private string password;
        private string host;
        private string port;
        private string path;
        private string query;
        private string fragment;

        public enum QueryParameterSeparator
        {
            Ampersand,
            Semicolon
        }

        public class UrlSchema
        {
            public const string HTTP = "http";
            public const string HTTPS = "https";
        }
    }

    public static class UrlBuilderExtensions
    {
        public static string GetString(this UrlBuilder.QueryParameterSeparator separator)
        {
            switch (separator)
            {
                case UrlBuilder.QueryParameterSeparator.Ampersand: return "&";
                case UrlBuilder.QueryParameterSeparator.Semicolon: return ";";
                default:
                    return "&";
            }
        }
    }
}
