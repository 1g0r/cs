using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FluentAssertions;
using Mindscan.Media.Utils.UnitTests.HttpExecutor.Stubs;
using Mindscan.Media.Utils.UnitTests.HttpExecutor.Stubs;
using NUnit.Framework;

namespace Mindscan.Media.Utils.UnitTests.HttpExecutor
{
	[Category("UnitTests")]
	[TestFixture]
	public class HttpExecutorTests
	{
		[TestCase(HttpStatusCode.OK, "Hello HTML executor")]
		[TestCase(HttpStatusCode.OK, "Кодировка по умолчанию UTF-8")]
		[TestCase(HttpStatusCode.BadRequest, "Привет русский текст", "windows-1251", "text/html; charset=windows-1251")]
		[TestCase(HttpStatusCode.InternalServerError, "Привет русский текст", "koi8-r", "text/html; charset=koi8-r", "https://www.test.com")]
		[TestCase(HttpStatusCode.Continue, "Привет русский текст", "windows-1251", "text/html; charset=win1251", "https://www.test.com")]
		public void Get_Head_WithDefaultContext(
			HttpStatusCode statusCode, 
			string body, 
			string bodyEncoding = "utf-8",
			string contentType = null, 
			string responseUri = null)
		{
			//Arrange
			var url = new Uri("http://test.com");
			var responseStub = CreateResponse(statusCode, body, bodyEncoding, contentType, responseUri);
			var requestStub = CreateRequest(responseStub);
			var executor = new HttpExecutorStab(requestStub);
			
			//Act
			var getResult = executor.Get(url);
			var headResult = executor.Head(url);

			//Assert
			getResult.StatusCode.Should().Be(statusCode);
			getResult.Content.Should().BeEquivalentTo(body);
			if (!string.IsNullOrEmpty(responseUri))
			{
				getResult.RedirectedUri.OriginalString.Should().BeEquivalentTo(responseUri);
			}
			else
			{
				headResult.RedirectedUri.Should().BeEquivalentTo(url);
			}

			headResult.StatusCode.Should().Be(statusCode);
			if (!string.IsNullOrEmpty(responseUri))
			{
				headResult.RedirectedUri.OriginalString.Should().BeEquivalentTo(responseUri);
			}
			else
			{
				headResult.RedirectedUri.Should().BeEquivalentTo(url);
			}
		}

		[Test]
		public void Get_WithCustomParser()
		{
			//Arrange 
			var value = 6666;
			var url = new Uri("http://test.com");
			var responseStub = CreateResponse(HttpStatusCode.OK, value.ToString());
			var requestStub = CreateRequest(responseStub);
			var executor = new HttpExecutorStab(requestStub);
			var context = executor.CreateConfigurator<int>().UseContentParser(reader =>
			{
				return int.Parse(reader.ReadToEnd());
			});

			//Act
			var response = executor.Get(url, context);

			//Assert
			response.Content.Should().Be(value);
		}

		[TestCase(HttpStatusCode.OK, "Я помню чудное мгновенье")]
		[TestCase(HttpStatusCode.BadRequest, "Привет русский текст", "windows-1251", "text/html; charset=windows-1251")]
		[TestCase(HttpStatusCode.OK, "Привет русский текст", "koi8-r", "text/html; charset=koi8-r", "https://www.test.com")]
		[TestCase(HttpStatusCode.Continue, "Привет русский текст", "windows-1251", "text/html; charset=win1251", "https://www.test.com")]
		public void Post_WithDefaultContext(
			HttpStatusCode statusCode, 
			string body,
			string bodyEncoding = "utf-8",
			string contentType = null,
			string responseUri = null)
		{
			//Arrange
			var postParameters = new Dictionary<string, string>
			{
				{"Name", "Buill" },
				{"LastName", "Gates" }
			};
			var url = new Uri("http://test.com");
			var responseStub = CreateResponse(statusCode, body, bodyEncoding, contentType, responseUri);
			var requestStub = CreateRequest(responseStub);
			var executor = new HttpExecutorStab(requestStub);
			

			//Act
			var result = executor.Post(url, postParameters);

			//Assert
			result.StatusCode.Should().Be(statusCode);
			result.Content.Should().BeEquivalentTo(body + "{ Name:Buill LastName:Gates }");
			if (!string.IsNullOrEmpty(responseUri))
			{
				result.RedirectedUri.OriginalString.Should().BeEquivalentTo(responseUri);
			}
			else
			{
				result.RedirectedUri.Should().BeEquivalentTo(url);
			}
		}

		[TestCase(HttpStatusCode.OK, "{\"count\":\"12\",\"name\":\"qwe\"}")]
		public void Post_WithBody(HttpStatusCode statusCode, string body)
		{
			//Arrange
			var url = new Uri("http://test.com");
			var responseStub = CreateResponse(statusCode, body);
			var executor = new HttpExecutorStab(CreateRequest(responseStub));

			//Act
			var result = executor.Post(url, body);

			//Assert
			result.StatusCode.Should().Be(statusCode);
			result.Content.Should().BeEquivalentTo(body);
		}

		[Test]
		public void Post_WithCustomParser()
		{
			//Arrange 
			var value = 6666;
			var url = new Uri("http://test.com");
			var responseStub = CreateResponse(HttpStatusCode.OK, value.ToString());
			var requestStub = CreateRequest(responseStub);
			var executor = new HttpExecutorStab(requestStub);
			var context = executor.CreateConfigurator<int>().UseContentParser(reader =>
			{
				return int.Parse(reader.ReadToEnd());
			});

			//Act
			Dictionary<string, string> parameters = null;
			var response = executor.Post(url, parameters, context);
			var response2 = executor.Post(url, new Dictionary<string, string>(), context);

			//Assert
			response.Content.Should().Be(value);
			response2.Content.Should().Be(value);
		}

		[Test]
		public void Behavior_CustomContext()
		{
			//Arrange
			var url = new Uri("http://test.com");
			var responseStub = CreateResponse(HttpStatusCode.OK, "Вафля", "windows-1251", "text/html; charset=windows-1251");
			var requestStub = CreateRequest(responseStub);
			var executor = new HttpExecutorStab(requestStub);
			var context = new ExecutorContextStub();
			
			//Act
			var response = executor.Get(url, context);

			//Assert
			response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.OK);
			response.Content.Should().BeEquivalentTo("Вафля");
			response.RedirectedUri.Should().Be(url);

			context.RequestFuncWasCalled.Should().BeTrue();
			context.HeadersFuncWasCalled.Should().BeTrue();
			context.CookieFuncWasCalled.Should().BeTrue();
			context.CookieParserFuncWasCalled.Should().BeTrue();
			context.EncodingParserFuncWasCalled.Should().BeTrue();
			context.ContentParserFuncWasCalled.Should().BeTrue();
			context.ProxyFuncWasCalled.Should().BeTrue();
			context.TimeoutFunkWasCalled.Should().BeTrue();
		}

		private static HttpWebResponseStub CreateResponse(
			HttpStatusCode statusCode, 
			string body,
			string bodyEncoding = "utf-8",
			string contentType = "",
			string responseUri = null)
		{
			var result = new HttpWebResponseStub(statusCode);
			result.BodyEncoding = Encoding.GetEncoding(bodyEncoding);
			result.Body = body;
			result.ContentType = contentType;
			if (!string.IsNullOrEmpty(responseUri))
			{
				result.SetResponseUri(new Uri(responseUri));
			}
			return result;
		}

		private static HttpWebRequestStub CreateRequest(HttpWebResponseStub response)
		{
			return new HttpWebRequestStub(response);
		}
	}
}
