using System;
using System.Xml.Linq;
using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.Utils.UnitTests.Config
{
	internal class ConfigBaseStub: ConfigBase
	{
		public ConfigBaseStub(XElement sectionXml) : base(sectionXml)
		{
		}

		public ConfigBaseStub(string sectionName):base(sectionName)
		{
			
		}

		protected override void BuildConfig()
		{
			BuildSettingsWasCalled = true;
		}


		public bool BuildSettingsWasCalled { get; private set; }

		public new string GetElementValue(string elementName, string defaultValue = null)
		{
			return base.GetElementValue(elementName, defaultValue);
		}

		public int GetElementInt(string elementName, int defaultValue)
		{
			return GetElementValueAndCast<int>(elementName, int.TryParse, defaultValue.ToString());
		}

		public TimeSpan GetElementTimeSpan_EmptyCast(string elementName, TimeSpan defaultValue)
		{
			return GetElementValueAndCast<TimeSpan>(elementName, null, defaultValue.ToString());
		}

		public TimeSpan GetElementTimeSpan(string elementName, TimeSpan defaultValue)
		{
			return GetElementValueAndCast<TimeSpan>(elementName, TimeSpan.TryParse, defaultValue.ToString());
		}

		public new string GetAttributeValue(string elementName, string defaultValue = null)
		{
			return base.GetAttributeValue(elementName, defaultValue);
		}

		public int GetAttributeInt(string elementName, int defaultValue)
		{
			return GetAttributeValueAndCast<int>(elementName, int.TryParse, defaultValue.ToString());
		}

		public TimeSpan GetAttributeTimeSpan_EmptyCast(string elementName, TimeSpan defaultValue)
		{
			return GetAttributeValueAndCast<TimeSpan>(elementName, null, defaultValue.ToString());
		}

		public TimeSpan GetAttributeTimeSpan(string elementName, TimeSpan defaultValue)
		{
			return GetAttributeValueAndCast<TimeSpan>(elementName, TimeSpan.TryParse, defaultValue.ToString());
		}
	}
}