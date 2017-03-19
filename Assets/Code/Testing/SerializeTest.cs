using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SerializeTest
{
	public TestObject Test;

	public SerializeTest()
	{
		Test = new TestObject();


	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(File.Exists(Application.persistentDataPath + "/testSave.dat"))
		{
			file = File.Open(Application.persistentDataPath + "/testSave.dat", FileMode.Open);
		}
		else
		{
			file = File.Create(Application.persistentDataPath + "/testSave.dat");
		}

		Debug.Log("before save, address is " + Test.Address + " Name is " + Test.Twos["1"].Name + Test.Twos["2"].Name + " room " + Test.Twos["1"].TheEnum);

		Test.PrepareSave();
		bf.Serialize(file, Test);
		file.Close();

	}

	public void Load()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(File.Exists(Application.persistentDataPath + "/testSave.dat"))
		{
			file = File.Open(Application.persistentDataPath + "/testSave.dat", FileMode.Open);
		}
		else
		{
			return;
		}

		TestObject loadedObject = (TestObject)bf.Deserialize(file);

		Test = loadedObject;
		Test.PostLoad();

		Debug.Log("After load, address is " + Test.Address + " Name is " + Test.Twos["1"].Name + Test.Twos["2"].Name + " room " + Test.Twos["1"].TheEnum);
	}


}

[System.Serializable]
public class TestObject
{
	public string Address;
	public TestObject2 Object2;
	public Dictionary<string, TestObject2> Twos;

	[SerializeField] private List<KeyValuePair<string, TestObject2>> _serTwos;

	public TestObject()
	{
		Address = "710 Grove Dr";
		Object2 = new TestObject2();
		TestObject2 object22 = new TestObject2();
		TestObject2 object23 = new TestObject2();
		Twos = new Dictionary<string, TestObject2>();
		Twos.Add("1", object22);
		Twos.Add("2", object23);
	}

	public void SetNewName(string newName)
	{
		Object2.Name = newName;
		Twos["1"].Name = "Meltem";
		Twos["2"].Name = "Teddo";

		Twos["1"].TheEnum = TestEnum.BathRoom;
	}

	public void SetNewAddress(string newAddress)
	{
		Address = newAddress;
	}

	public void PrepareSave()
	{
		_serTwos = new List<KeyValuePair<string, TestObject2>>();
		foreach(KeyValuePair<string, TestObject2> two in Twos)
		{
			_serTwos.Add(two);
		}
	}

	public void PostLoad()
	{
		Twos = new Dictionary<string, TestObject2>();
		foreach(KeyValuePair<string, TestObject2> two in _serTwos)
		{
			Twos.Add(two.Key, two.Value);
		}
	}
}

[System.Serializable]
public class TestObject2
{
	public string Name;
	public TestEnum TheEnum;
	public TestObject2()
	{
		Name = "Kurt";
		TheEnum = TestEnum.LivingRoom;
	}
}

public enum TestEnum
{
	LivingRoom,
	BathRoom,
}