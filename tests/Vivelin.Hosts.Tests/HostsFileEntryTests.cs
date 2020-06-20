using System;
using System.Net;

using FluentAssertions;

using Xunit;

namespace Vivelin.Hosts.Tests
{
    public class HostsFileEntryTests
    {
        [Fact]
        public void ParsedEntryShouldHaveOriginalLine()
        {
            const string Line = "# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.";

            var entry = HostsFileEntry.Parse(Line);

            entry.Line.Should().Be(Line);
        }

        [Fact]
        public void FullLineCommentIsParsed()
        {
            const string Comment = "This is a sample HOSTS file used by Microsoft TCP/IP for Windows.";

            var entry = HostsFileEntry.Parse($"# {Comment}");

            entry.Comment.Should().Be(Comment);
        }

        [Fact]
        public void CommentIsParsedFromTheFirstPosition()
        {
            const string Comment = "This is a sample HOSTS # file # used by # Microsoft TCP/IP for Windows.";

            var entry = HostsFileEntry.Parse($"test test test # {Comment}");

            entry.Comment.Should().Be(Comment);
        }

        [Fact]
        public void CommentIsParsedWithoutLeadingSpace()
        {
            const string Comment = "This is a sample HOSTS file used by Microsoft TCP/IP for Windows.";

            var entry = HostsFileEntry.Parse($"#{Comment}");

            entry.Comment.Should().Be(Comment);
        }

        [Fact]
        public void IPv4AddressesAreParsed()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1");

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
        }

        [Fact]
        public void ExtraIPv4AddressesAreIgnored()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1 # 127.0.0.2 127.0.0.3");

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
        }

        [Fact]
        public void FirstIPAddressInCommentIsParsed()
        {
            var entry = HostsFileEntry.Parse("# 127.0.0.1");

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
        }

        [Fact]
        public void FirstIPAddressInCommentWithoutSpaceIsParsed()
        {
            var entry = HostsFileEntry.Parse("#127.0.0.1");

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
        }

        [Fact]
        public void CommentIsParsedWhenFirstTokenIsIPAddress()
        {
            var entry = HostsFileEntry.Parse("# 127.0.0.1 # Comment");

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
            entry.Comment.Should().Be("Comment");
        }

        [Fact]
        public void TokensOutsideOfCommentAreParsedAsHostNames()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1 localhost loopback # Comment");

            entry.HostNames.Should().Equal("localhost", "loopback");
        }

        [Fact]
        public void TokensInOuterCommentAreParsedAsHostNames()
        {
            var entry = HostsFileEntry.Parse("# 127.0.0.1 localhost loopback # Comment");

            entry.Enabled.Should().BeFalse();
            entry.HostNames.Should().Equal("localhost", "loopback");
        }

        [Fact]
        public void TokensInOuterCommentWithoutSpaceAreParsedAsHostNames()
        {
            var entry = HostsFileEntry.Parse("#127.0.0.1 localhost loopback # Comment");

            entry.Enabled.Should().BeFalse();
            entry.HostNames.Should().Equal("localhost", "loopback");
        }

        [Fact]
        public void DisablingAValidEntryShouldAddNumberSignAndASpace()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1 localhost loopback # Comment");

            entry.Enabled = false;

            entry.ToString().Should().Be("# 127.0.0.1 localhost loopback # Comment");
        }

        [Fact]
        public void DisablingAnInvalidLineShouldLeaveEverythingAsIs()
        {
            var entry = HostsFileEntry.Parse("# Comment");

            entry.Enabled = false;

            entry.ToString().Should().Be("# Comment");
        }

        [Fact]
        public void CommentCanBeChanged()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1 localhost loopback # Comment");

            entry.Comment = "asdfasdf";

            entry.ToString().Should().Be("127.0.0.1 localhost loopback # asdfasdf");
        }

        [Fact]
        public void CommentCanBeRemoved()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1 localhost loopback # Comment");

            entry.Comment = null;

            entry.ToString().Should().Be("127.0.0.1 localhost loopback");
        }

        [Fact]
        public void EmptyCommentIsTrimmed()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1 localhost loopback # Comment");

            entry.Comment = "";

            entry.ToString().Should().Be("127.0.0.1 localhost loopback #");
        }

        [Fact]
        public void HostNamesAreAddedInOrder()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1 localhost # Comment");

            entry.HostNames.Add("loopback");

            entry.ToString().Should().Be("127.0.0.1 localhost loopback # Comment");
        }

        [Fact]
        public void ChangingIPAddressShouldGiveANewLine()
        {
            var entry = HostsFileEntry.Parse("127.0.0.1 localhost loopback # Comment");

            entry.Address = IPAddress.Parse("127.0.0.2");

            entry.ToString().Should().Be("127.0.0.2 localhost loopback # Comment");
        }
    }
}
