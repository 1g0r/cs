using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities;
using NUnit.Framework;

namespace Mindscan.Media.Domain.Tests.Entities
{
	[TestFixture]
	public class NormalizedUrl_Tests
	{
		[TestCase("http://google.com", "http://", "google.com/")]
		[TestCase("https://google.com/home/test", "https://", "google.com/home/test")]
		[TestCase("https://www.google.com/home/test", "https://www.", "google.com/home/test")]
		[TestCase("http://www.google.com/home/test", "http://www.", "google.com/home/test")]
		[TestCase("http://www.google.com/home/test/", "http://www.", "google.com/home/test/")]
		public void Should_SplitPrefix(Uri url, string prefix, string tail)
		{
			//Arrange
			var nUrl = NormalizedUrl.Build(url);

			//Act

			//Assert
			nUrl.Should().NotBeNull();
			nUrl.Prefix.Should().Be(prefix);
			nUrl.Tail.Should().Be(tail);
		}

		[TestCase("http://google.com", "google.com/")]
		[TestCase("http://google.com/home/test/", "google.com/home/test/")]
		[TestCase("http://google.com/home/test?par1=1", "google.com/home/test?par1=1")]
		[TestCase("http://google.com/home/test/?par1=1", "google.com/home/test/?par1=1")]
		[TestCase("http://google.com/home/test/image.jpg", "google.com/home/test/image.jpg")]
		[TestCase("http://google.com/home/test/image.sdflkj.wwwwww", "google.com/home/test/image.sdflkj.wwwwww")]
		public void Should_AddSlashToPath(Uri url, string tail)
		{
			//Arrange
			var nUrl = NormalizedUrl.Build(url);

			//Act

			//Assert
			nUrl.Should().NotBeNull();
			nUrl.Tail.Should().Be(tail);
		}

		[TestCase("http://google.com/%D0%B4%D0%BE%D0%BC/%D1%82%D0%B5%D1%81%D1%82", "http://google.com/дом/тест")]
		[TestCase("http://google.com/%D0%B4%D0%BE%D0%BC/%D1%82%D0%B5%D1%81%D1%82/", "http://google.com/дом/тест/")]
		[TestCase("http://google.com/%D0%B4%D0%BE%D0%BC/%D1%82%D0%B5%D1%81%D1%82?par=%D0%B8%D1%89%D1%83%20%D0%BA%D0%BE%D0%BD%D1%8F&par2=%D0%BC%D0%BE%D0%B6%D0%BD%D0%BE%20%D0%B8%20%D0%BD%D0%B5%D1%82", "http://google.com/дом/тест?par=ищу коня&par2=можно и нет")]
		public void Should_DecodeTailPart(Uri original, string result)
		{
			//Arrange
			var nUrl = NormalizedUrl.Build(original);

			//Act

			//Assert
			nUrl.Should().NotBeNull();
			nUrl.ToString().Should().Be(result);
		}

		[TestCase("http://xn--h1aakgfv.xn--p1ai/%D0%BA%D0%B0%D1%82%D0%B5%D0%B3%D0%BE%D1%80%D0%B8%D0%B8/%D0%BD%D0%BE%D0%B2%D0%BE%D1%81%D1%82%D0%B8", "http://митино.рф/категории/новости")]
		[TestCase("http://xn--h1aakgfv.xn--p1ai", "http://митино.рф/")]
		[TestCase("http://xn----7sbemcvc6aaeev1c4g.xn--p1ai/feed", "http://районные-будни.рф/feed")]
		public void Should_DecodePunyCode(Uri original, string result)
		{
			//Arrange
			var nUrl = NormalizedUrl.Build(original);

			//Act

			//Assert
			nUrl.Should().NotBeNull();
			nUrl.ToString().Should().Be(result);
		}

		[TestCase(null, null)]
		[TestCase("http://google.com/home/test", "http://google.com/home/test")]
		[TestCase("https://www.google.com/home/test", "https://www.google.com/home/test")]
		[TestCase("https://www.GOOGLE.com/home/test", "https://WWW.google.com/home/test")]
		[TestCase("http://google.com/%D0%B4%D0%BE%D0%BC/%D1%82%D0%B5%D1%81%D1%82", "http://google.com/дом/тест")]
		[TestCase("http://xn--h1aakgfv.xn--p1ai", "http://митино.рф/")]
		[TestCase("http://xn----7sbemcvc6aaeev1c4g.xn--p1ai/feed", "http://районные-будни.рф/feed")]
		public void Should_BeEqual(string first, string second)
		{
			//Arrange
			var nFirst = first == null ? null : NormalizedUrl.Build(new Uri(first));
			var nSecond = second == null ? null : NormalizedUrl.Build(new Uri(second));

			//Act
			var result = nFirst == nSecond;

			//Assert
			result.Should().BeTrue();
		}

		[TestCase("http://google.com/home/test", "https://google.com/home/test")]
		[TestCase("http://www.google.com/home/test", "http://google.com/home/test")]
		[TestCase("http://google.com/home/test", "https://google.com/Home/Test")]
		[TestCase(null, "https://google.com/Home/Test")]
		[TestCase("http://google.com/home/test", null)]
		[TestCase("http://google.com/home/image.jpg", "https://google.com/home/image.JPG")]
		[TestCase("http://google.com/%D0%B4%D0%BE%D0%BC/%D1%82%D0%B5%D1%81%D1%82", "http://google.com/дом/тесТ/")]
		public void Should_NotBeEqual(string first, string second)
		{
			//Arrange
			var nFirst = first == null ? null : NormalizedUrl.Build(new Uri(first));
			var nSecond = second == null ? null : NormalizedUrl.Build(new Uri(second));

			//Act
			var result = nFirst != nSecond;

			//Assert
			result.Should().BeTrue();
		}

		[TestCase("http://google.com", "google.com")]
		[TestCase("http://GOOgle.com", "google.com")]
		[TestCase("https://google.com", "google.com")]
		[TestCase("https://www.google.com", "google.com")]
		[TestCase("https://www.waffle.google.com", "waffle.google.com")]
		[TestCase("http://www.xn----7sbemcvc6aaeev1c4g.xn--p1ai/feed", "районные-будни.рф")]
		public void Should_NormalizeHost(Uri url, string outcome)
		{
			//Arrange
			var nUrl = NormalizedUrl.Build(url);

			//Act
			var host = nUrl.Host;

			//Assert
			host.Should().NotBeNullOrEmpty();
			host.Should().Be(outcome);
		}
	}
}
