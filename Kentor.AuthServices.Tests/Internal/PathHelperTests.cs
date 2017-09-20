using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Internal;
using FluentAssertions;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class PathHelperTests
    {
        [TestMethod]
        public void PathHelper_BasePath_ShouldGivePath()
        {
            String path = PathHelper.BasePath;

            // Rather poor test, although before and after this string contains variable parts.
            path.Should().Contain(@"\Kentor.AuthServices.Tests\bin\");
        }

        [TestMethod]
        public void PathHelper_MapPath_ShouldResolvePath()
        {
            String mappedPath = PathHelper.MapPath("~/test/file.test");
            mappedPath.Should().Contain(@"\Kentor.AuthServices.Tests\bin\").And.EndWith(@"\test\file.test");
        }

        [TestMethod]
        public void PathHelper_MapPath_ShouldThrowOnNull()
        {
            Action a = () => PathHelper.MapPath(null);
            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("virtualPath");
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
            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("virtualPath");
        }

        [TestMethod]
        public void PathHelper_IsRelative_ShouldDetectDegeneratePath()
        {
            bool result = PathHelper.IsWebRootRelative("");
            result.Should().BeFalse();
        }
    }
}
