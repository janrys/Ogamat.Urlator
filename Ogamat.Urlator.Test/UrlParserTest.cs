using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ogamat.Urlator.Test
{
    [TestClass]
    public class UrlParserTest
    {
        [DataRow("http://api.vkontakte.ru/blank.html#access_token=8860213c0a392ba0971fb35bdfb0z605d459a9dcf9d2208ab60e714c3367681c6d091aa12a3fdd31a4872&expires_in=86400&user_id=34558123",
            "http", null, null, "api.vkontakte.ru", null, "blank.html", null, "access_token=8860213c0a392ba0971fb35bdfb0z605d459a9dcf9d2208ab60e714c3367681c6d091aa12a3fdd31a4872&expires_in=86400&user_id=34558123")]
        [DataRow("foo://example.com:8042/over/there?name=ferret#nose", "foo", null, null, "example.com", "8042", "over/there", "name=ferret", "nose")]
        [DataRow("http://matt:secret@www.chilkatsoft.com:8080/somepath.asp?test=123&size=2", "http", "matt", "secret", "www.chilkatsoft.com", "8080", "somepath.asp", "test=123&size=2", null)]
        [TestMethod]
        public void Test(string url, string schema, string user, string password, string host, string port, string path, string query, string fragment)
        {
            UrlParser sut = new UrlParser(url);

            Assert.AreEqual(schema, sut.Scheme);
            Assert.AreEqual(user, sut.User);
            Assert.AreEqual(password, sut.Password);
            Assert.AreEqual(host, sut.Host);
            Assert.AreEqual(port, sut.Port);
            Assert.AreEqual(path, sut.Path);
            Assert.AreEqual(query, sut.Query);
            Assert.AreEqual(fragment, sut.Fragment);
        }
    }
}
