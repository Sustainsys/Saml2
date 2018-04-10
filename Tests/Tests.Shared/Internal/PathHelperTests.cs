using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Internal;
using FluentAssertions;
using System.Reflection;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class PathHelperTests
    {
		static string AssemblyFolderName { get; set; }

		static PathHelperTests()
		{
			string name = Assembly.GetExecutingAssembly().FullName;
			int commaPos = name.IndexOf(',');
			if (commaPos >= 0)
			{
				name = name.Substring(0, commaPos).Trim();
			}
			AssemblyFolderName = name;
		}

		[TestMethod]
        public void PathHelper_BasePath_ShouldGivePath()
        {
            String path = PathHelper.BasePath;

            // Rather poor test, although before and after this string contains variable parts.
            path.Should().Contain($"\\{AssemblyFolderName}\\bin\\");
        }

        [TestMethod]
        public void PathHelper_MapPath_ShouldResolvePath()
        {
            String mappedPath = PathHelper.MapPath("~/test/file.test");
            mappedPath.Should().Contain($"\\{AssemblyFolderName}\\bin\\").And.EndWith(@"\test\file.test");
        }

        [TestMethod]
        public void PathHelper_MapPath_ShouldThrowOnNull()
        {
            Action a = () => PathHelper.MapPath(null);
            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("virtualPath");
        }

        [TestMethod]
        public void PathHelper_MapPath_ShouldIgnoreBasePathWhenNotRelative()
        {
            String result = PathHelper.MapPath("C:/Test");
            String expected = "C:\\Test";

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void PathHelper_IsRelative_ShouldDetectRelative()
        {
            bool result = PathHelper.IsWebRootRelative("~/test/file.test");
            result.Should().BeTrue();
        }

        [TestMethod]
        public void PathHelper_IsRelative_ShouldDetectNonRelative()
        {
            bool result = PathHelper.IsWebRootRelative("/test/file.test");
            result.Should().BeFalse();
        }

        [TestMethod]
        public void PathHelper_IsRelative_ShouldThrowOnNull()
        {
            Action a = () => PathHelper.IsWebRootRelative(null);
            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("virtualPath");
        }

        [TestMethod]
        public void PathHelper_IsRelative_ShouldDetectDegeneratePath()
        {
            bool result = PathHelper.IsWebRootRelative("");
            result.Should().BeFalse();
        }

        [TestMethod]
        public void PathHelper_IsLocal_ShouldThrowOnNull()
        {
            Action a = () => PathHelper.IsLocalWebUrl(null);
            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("url");
        }

        [TestMethod]
        public void PathHelper_IsLocal_ShouldAcceptRootRelative()
        {
            PathHelper.IsLocalWebUrl("/").Should().BeTrue();
            PathHelper.IsLocalWebUrl("/myfolder").Should().BeTrue();
            PathHelper.IsLocalWebUrl("/google.com").Should().BeTrue();
        }

        [TestMethod]
        public void PathHelper_IsLocal_ShouldAcceptAppRelative()
        {
            PathHelper.IsLocalWebUrl("~/").Should().BeTrue();
            PathHelper.IsLocalWebUrl("~/folder/1").Should().BeTrue();
            PathHelper.IsLocalWebUrl("~/1.html").Should().BeTrue();
        }

        [TestMethod]
        public void PathHelper_IsLocal_ShouldRejectAbsolute()
        {
            PathHelper.IsLocalWebUrl("http://google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl("https://google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl("httP://google.com").Should().BeFalse();
        }

        [TestMethod]
        public void PathHelper_IsLocal_ShouldRejectProtocolRelative()
        {
            PathHelper.IsLocalWebUrl("//google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl("//google").Should().BeFalse();
        }

        [TestMethod]
        public void PathHelper_IsLocal_ShouldRejectPathRelative()
        {
            PathHelper.IsLocalWebUrl("./folder").Should().BeFalse();
            PathHelper.IsLocalWebUrl("../folder").Should().BeFalse();
            PathHelper.IsLocalWebUrl("../../folder").Should().BeFalse();
            PathHelper.IsLocalWebUrl("file.html").Should().BeFalse();
        }

        [TestMethod]
        public void PathHelper_IsLocal_ShouldRejectInvalidUrls()
        {
            PathHelper.IsLocalWebUrl(@"///google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"/\google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"/\").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"http:/google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"http/:google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"http//:google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"http:///google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"http:\\google.com").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"http:\\google.com\").Should().BeFalse();
            PathHelper.IsLocalWebUrl(@"http:\\google.com/").Should().BeFalse();
        }
    }
}
