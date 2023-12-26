using Common.Domain;
using Common.Domain.TestSetupCommands;
using Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class MongoReadWriteTest
	{
		[TestMethod]
		public void TestAuxSettingsMongoDbReadWrite()
		{
			var client = new MongoClient("mongodb://localhost");
			var database = client.GetDatabase("commands");
			var collection = database.GetCollection<AuxSettings>("AuxSettings");

			collection.DeleteMany(FilterDefinition<AuxSettings>.Empty);

			var auxSettings = new AuxSettings
			{
				Function = AuxSettingsFunction.Disabled,
				Option1 = 0x0,
				Option2 = 0x0,
				EdgeType = EdgeType.Rising,
				Debounce = 0x1,
				DeafTime = 0x1
			};
			collection.InsertOne(auxSettings);

			var count = collection.Count(FilterDefinition<AuxSettings>.Empty);

			Assert.AreEqual(count, 1);

			var restoredAuxSettings = collection.Find(FilterDefinition<AuxSettings>.Empty).FirstOrDefault();

			Assert.IsNotNull(restoredAuxSettings);
			Assert.AreEqual(auxSettings.Function, restoredAuxSettings.Function);
			Assert.AreEqual(auxSettings.Option1, restoredAuxSettings.Option1);
			Assert.AreEqual(auxSettings.Option2, restoredAuxSettings.Option2);
			Assert.AreEqual(auxSettings.EdgeType, restoredAuxSettings.EdgeType);
			Assert.AreEqual(auxSettings.Debounce, restoredAuxSettings.Debounce);
			Assert.AreEqual(auxSettings.DeafTime, restoredAuxSettings.DeafTime);
		}

		[TestMethod]
		public void TestEncoderSettingsMongoDbReadWrite()
		{
			var client = new MongoClient("mongodb://localhost");
			var database = client.GetDatabase("commands");
			var collection = database.GetCollection<EncoderSettings>("EncoderSettings");

			collection.DeleteMany(FilterDefinition<EncoderSettings>.Empty);

			var encoderSettings = new EncoderSettings
			{
				TriggerFilterMin = 0x0,
				TesterOffset = 0x0,
				MarkerOffset = 0x3,
				PunchOffset = 0x3,
				PunchFlight = 0x0,
				TriggerFilterMax = 0x1
			};
			collection.InsertOne(encoderSettings);

			var count = collection.Count(FilterDefinition<EncoderSettings>.Empty);

			Assert.AreEqual(count, 1);

			var restoredEncoderSettings = collection.Find(FilterDefinition<EncoderSettings>.Empty).FirstOrDefault();

			Assert.IsNotNull(restoredEncoderSettings);
			Assert.AreEqual(encoderSettings.TriggerFilterMin, restoredEncoderSettings.TriggerFilterMin);
			Assert.AreEqual(encoderSettings.TesterOffset, restoredEncoderSettings.TesterOffset);
			Assert.AreEqual(encoderSettings.MarkerOffset, restoredEncoderSettings.MarkerOffset);
			Assert.AreEqual(encoderSettings.PunchOffset, restoredEncoderSettings.PunchOffset);
			Assert.AreEqual(encoderSettings.PunchFlight, restoredEncoderSettings.PunchFlight);
			Assert.AreEqual(encoderSettings.TriggerFilterMax, restoredEncoderSettings.TriggerFilterMax);
		}

		[TestMethod]
		public void TestIntegralTypesMongoDbReadWrite()
		{
			var testObject = new TestObject
			{
				SByte = -1,
				Byte = 1,
				Char = 'a',
				Short = -2,
				Ushort = 2,
				Int = -3,
				Uint = 3,
				Long = -4,
				Ulong = 4
			};

			var client = new MongoClient("mongodb://localhost");
			var database = client.GetDatabase("types_test");
			var collection = database.GetCollection<TestObject>("TestTypes");

			collection.DeleteMany(FilterDefinition<TestObject>.Empty);

			collection.InsertOne(testObject);

			var count = collection.Count(FilterDefinition<TestObject>.Empty);

			Assert.AreEqual(count, 1);

			var restoredTestObject = collection.Find(FilterDefinition<TestObject>.Empty).FirstOrDefault();

			Assert.IsNotNull(restoredTestObject);

			Assert.AreEqual(testObject.SByte, restoredTestObject.SByte);
			Assert.AreEqual(testObject.Byte, restoredTestObject.Byte);
			Assert.AreEqual(testObject.Char, restoredTestObject.Char);
			Assert.AreEqual(testObject.Short, restoredTestObject.Short);
			Assert.AreEqual(testObject.Ushort, restoredTestObject.Ushort);
			Assert.AreEqual(testObject.Int, restoredTestObject.Int);
			Assert.AreEqual(testObject.Uint, restoredTestObject.Uint);
			Assert.AreEqual(testObject.Long, restoredTestObject.Long);
			Assert.AreEqual(testObject.Ulong, restoredTestObject.Ulong);
		}

		private class TestObject
		{
			//public ObjectId _id { get; set; }

			public sbyte SByte { get; set; }
			public byte Byte { get; set; }
			public char Char { get; set; }
			public short Short { get; set; }
			public ushort Ushort { get; set; }
			public int Int { get; set; }
			public uint Uint { get; set; }
			public long Long { get; set; }
			public ulong Ulong { get; set; }
		}
	}
}