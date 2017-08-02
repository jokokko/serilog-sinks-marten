using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace Serilog.Sinks.Marten.Services
{
	internal static class MartenPropertyFormatter
	{
		public static object Simplify(LogEventPropertyValue value)
		{
			var scalar = value as ScalarValue;

			if (scalar != null)
			{
				return scalar.Value;
			}

			var dict = value as DictionaryValue;

			if (dict != null)
			{
				var dictionary = new Dictionary<object, object>();
				foreach (var element in dict.Elements)
				{
					if (element.Key?.Value == null)
					{
						continue;
					}

					var itemValue = Simplify(element.Value);

					if (itemValue == null)
					{
						continue;
					}

					dictionary[element.Key.Value] = itemValue;

				}
				return dictionary;
			}

			var seq = value as SequenceValue;

			if (seq != null)
			{
				return seq.Elements.Select(Simplify).Where(itemValue => itemValue != null).ToList();
			}

			var str = value as StructureValue;

			return str?.Properties.ToDictionary(p => p.Name, p => Simplify(p.Value));
		}
	}
}