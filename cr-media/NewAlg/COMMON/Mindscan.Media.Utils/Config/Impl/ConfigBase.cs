using System;
using System.Configuration;
using System.Xml.Linq;

namespace Mindscan.Media.Utils.Config.Impl
{
	public abstract class ConfigBase : IConfig
	{
		protected XElement Raw { get; private set; }

		protected ConfigBase(string sectionName)
		{
			var section = ConfigurationManager.GetSection(sectionName) as MediaConfigSection;
			if (section == null)
			{
				throw new ConfigurationErrorsException($"Unable to find section '{sectionName}' in configuration file.");
			}
			Parse(section.SectionXml);
		}

		protected ConfigBase(XElement sectionXml)
		{
			Parse(sectionXml);
		}
		private void Parse(XElement rawXml)
		{
			if (rawXml != null)
			{
				Raw = rawXml;
				Debug = GetAttributeValueAndCast<bool>(nameof(Debug), bool.TryParse, "false");
				BuildConfig();
			}
		}

		public bool Debug { get; private set; }

		protected abstract void BuildConfig();

		protected string GetElementValue(string elementName, bool mandatory = true)
		{
			var result = Raw?.Element(elementName);
			if (string.IsNullOrEmpty(result?.Value))
			{
				if (mandatory)
					throw new ConfigurationErrorsException(
						$"Mandatory setting '{elementName}' is empty. Please fill it in and start all over again.");
				return string.Empty;
			}
			return result.Value;
		}

		protected string GetElementValue(string elementName, string defaultValue)
		{
			var result = GetElementValue(elementName, string.IsNullOrEmpty(defaultValue));
			if (string.IsNullOrEmpty(result))
			{
				return defaultValue;
			}
			return result;
		}

		protected delegate bool CastDelegate<TResult>(string value, out TResult result);

		protected TResult GetElementValueAndCast<TResult>(string elementName, CastDelegate<TResult> castFunc, string defaultValue = null)
		{
			var value = GetElementValue(elementName, string.IsNullOrEmpty(defaultValue));
			if (string.IsNullOrEmpty(value))
			{
				return Cast(defaultValue, castFunc);
			}
			return Cast(value, castFunc);
		}

		private TResult Cast<TResult>(string value, CastDelegate<TResult> castFunc)
		{
			if (castFunc == null)
			{
				throw new InvalidCastException("Cast function must not be empty.");
			}
			TResult result;
			if (!castFunc(value, out result))
			{
				throw new InvalidCastException($"Unable to cast '{value}' value.");
			}
			return result;
		}

		protected string GetAttributeValue(string attributeName, bool mandatory = true)
		{
			var result = Raw?.Attribute(attributeName);
			if (string.IsNullOrEmpty(result?.Value))
			{
				if (mandatory)
					throw new ConfigurationErrorsException(
						$"Mandatory setting '{attributeName}' is empty. Please fill it in and start all over again.");
				return string.Empty;
			}
			return result.Value;
		}

		protected string GetAttributeValue(string attributeName, string defaultValue)
		{
			var result = GetAttributeValue(attributeName, string.IsNullOrEmpty(defaultValue));
			if (string.IsNullOrEmpty(result))
			{
				return defaultValue;
			}
			return result;
		}

		protected TResult GetAttributeValueAndCast<TResult>(string attributeName, CastDelegate<TResult> castFunc, string defaultValue = null)
		{
			var value = GetAttributeValue(attributeName, string.IsNullOrEmpty(defaultValue));
			if (string.IsNullOrEmpty(value))
			{
				return Cast(defaultValue, castFunc);
			}
			return Cast(value, castFunc);
		}

		protected bool TryGetUri(string uriString, out Uri result)
		{
			result = null;
			if (string.IsNullOrEmpty(uriString))
				return false;
			return Uri.TryCreate(uriString, UriKind.Absolute, out result);
		}
	}
}