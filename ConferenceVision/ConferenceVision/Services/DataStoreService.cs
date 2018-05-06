using System;
using System.IO;
using System.Xml.Serialization;
using ConferenceVision.Models;
using Xamarin.Essentials;

namespace ConferenceVision.Services
{
	public class DataStoreService
	{
		public void Save(DataStore model)
		{
			var serializer = new XmlSerializer(model.GetType());
			var stringWriter = new StringWriter();
			serializer.Serialize(stringWriter, model);

			Preferences.Set("DataStore", stringWriter.ToString());

		}

		public void Load(DataStore model)
		{
			var serializer = new XmlSerializer(typeof(DataStore));
			var dataString = Preferences.Get("DataStore", string.Empty);
			var stringReader = new StringReader(dataString);

			try
			{
				App.DataStore = (DataStore)serializer.Deserialize(stringReader);
			}
			catch
			{
				App.DataStore = new DataStore();
				Save(App.DataStore);
			}
		}

		public void DeleteMemory(Memory memory)
		{
			App.DataStore.Memories.Remove(memory);
			Save(App.DataStore);
		}
	}
}
