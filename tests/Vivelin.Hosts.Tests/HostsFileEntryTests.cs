using System;

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

            var entry = new HostsFileEntry { Line = Line };

            entry.Line.Should().Be(Line);
        }

        [Fact]
        public void FullLineCommentIsParsed()
        {
            const string Comment = "This is a sample HOSTS file used by Microsoft TCP/IP for Windows.";

            var entry = new HostsFileEntry { Line = $"# {Comment}" };

            entry.Comment.Should().Be(Comment);
        }

        [Fact]
        public void CommentIsParsedFromTheFirstPosition()
        {
            const string Comment = "This is a sample HOSTS # file # used by # Microsoft TCP/IP for Windows.";

            var entry = new HostsFileEntry { Line = $"test test test # {Comment}" };

            entry.Comment.Should().Be(Comment);
        }

        [Fact]
        public void CommentIsParsedWithoutLeadingSpace()
        {
            const string Comment = "This is a sample HOSTS file used by Microsoft TCP/IP for Windows.";

            var entry = new HostsFileEntry { Line = $"#{Comment}" };

            entry.Comment.Should().Be(Comment);
        }

        [Fact]
        public void IPv4AddressesAreParsed()
        {
            var entry = new HostsFileEntry { Line = "127.0.0.1" };

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
        }

        [Fact]
        public void ExtraIPv4AddressesAreIgnored()
        {
            var entry = new HostsFileEntry { Line = "127.0.0.1 # 127.0.0.2 127.0.0.3" };

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
        }

        [Fact]
        public void FirstIPAddressInCommentIsParsed()
        {
            var entry = new HostsFileEntry { Line = "# 127.0.0.1" };

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
        }

        [Fact]
        public void FirstIPAddressInCommentWithoutSpaceIsParsed()
        {
            var entry = new HostsFileEntry { Line = "#127.0.0.1" };

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
        }

        [Fact]
        public void CommentIsParsedWhenFirstTokenIsIPAddress()
        {
            var entry = new HostsFileEntry { Line = "# 127.0.0.1 # Comment" };

            entry.Address.Should().Be(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
            entry.Comment.Should().Be("Comment");
        }

        [Fact]
        public void TokensOutsideOfCommentAreParsedAsHostNames()
        {
            var entry = new HostsFileEntry { Line = "127.0.0.1 localhost loopback # Comment" };

            entry.HostNames.Should().Equal("localhost", "loopback");
        }

        [Fact]
        public void TokensInOuterCommentAreParsedAsHostNames()
        {
            var entry = new HostsFileEntry { Line = "# 127.0.0.1 localhost loopback # Comment" };

            entry.Enabled.Should().BeFalse();
            entry.HostNames.Should().Equal("localhost", "loopback");
        }
    }
}
