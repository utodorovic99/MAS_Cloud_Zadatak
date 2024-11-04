using System.Reflection;

namespace BookstoreDesktopClient.Configuration
{
	/// <summary>
	/// Base implementation of single configuration stored within App.config.
	/// </summary>
	public abstract class ConfigBase
	{
		/// <summary>
		/// Sets value of property addressed by <paramref name="popertyIdentifier"/> to new value of <paramref name="propertyValue"/>.
		/// </summary>
		/// <param name="popertyIdentifier">Property identifier.</param>
		/// <param name="propertyValue">New value of property.</param>
		/// <returns><c>True</c> if property exits and value is properly set; otherwise returns <c>false</c>.</returns>
		public bool SetProperty(string popertyIdentifier, string propertyValue)
		{
			foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
			{
				if (!TryGetConfigAttribute(propertyInfo, out ConfigProperty configPropertyAttribute))
				{
					continue;
				}

				if (!configPropertyAttribute.Identifier.Equals(popertyIdentifier))
				{
					continue;
				}

				if (!TryParseValue(propertyValue, configPropertyAttribute.DataType, out object parsedValue))
				{
					continue;
				}

				propertyInfo.SetValue(this, parsedValue);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Tries to retrieve <paramref name="configPropertyAttribute"/> from <paramref name="propertyInfo"/> if exits.
		/// </summary>
		/// <param name="propertyInfo">Property whose attribute is being retrieved.</param>
		/// <param name="configPropertyAttribute">Resulting retrieved property.</param>
		/// <returns><c>True</c> is <paramref name="configPropertyAttribute"/> successfully retrieved from <paramref name="propertyInfo"/>; otherwise returns <c>false</c>.</returns>
		private bool TryGetConfigAttribute(PropertyInfo propertyInfo, out ConfigProperty configPropertyAttribute)
		{
			configPropertyAttribute = propertyInfo.GetCustomAttributes(typeof(ConfigProperty)).FirstOrDefault() as ConfigProperty;

			return configPropertyAttribute != null;
		}

		/// <summary>
		/// Tries to parse <paramref name="parsedValue"/> into a <paramref name="parsedValue"/> of type <paramref name="propertyTypeToParse"/>.
		/// </summary>
		/// <param name="stringPropertyValue">Current property value as string.</param>
		/// <param name="propertyTypeToParse">Type of property to parse.</param>
		/// <param name="parsedValue">Resulting parsed property.</param>
		/// <returns><c>True</c> if <paramref name="stringPropertyValue"/> successfully parsed into <paramref name="parsedValue"/>; otherwise returns <c>false</c>.</returns>
		private bool TryParseValue(string stringPropertyValue, Type propertyTypeToParse, out object parsedValue)
		{
			if (propertyTypeToParse.Equals(typeof(string)))
			{
				parsedValue = stringPropertyValue;
			}
			else if (propertyTypeToParse.Equals(typeof(Uri)))
			{
				parsedValue = new Uri(stringPropertyValue);
			}
			else if (propertyTypeToParse.Equals(typeof(ushort))
				&& ushort.TryParse(stringPropertyValue, out ushort parsedUnsingedShort))
			{
				parsedValue = parsedUnsingedShort;
			}
			else
			{
				parsedValue = null;
				return false;
			}

			return true;
		}
	}
}
