﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAttribute
{
	public string Name;
	public object Value;

	public ItemAttribute(string name, object value)
	{
		Name = name;
		Value = value;
	}
}
