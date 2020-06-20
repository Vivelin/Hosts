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
    }
}
