using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;


public class CsDebug : MonoBehaviour
{
	#region Singleton 
	public static CsDebug Inst;
	#endregion

	public Character DebugChar;

	private Dictionary<CsDComponent, CsDLevel> _debugLevel;
	private CsDLogTarget _logTarget;
	private int _bufferSize;
	private string _bufferedLog;



	public void Initialize()
	{
		Inst = this;

		_debugLevel = CsDebug.LoadDebugSetting(out _logTarget);

		if(_debugLevel == null)
		{
			_debugLevel = new Dictionary<CsDComponent, CsDLevel>();
			CsDComponent [] components = (CsDComponent[])Enum.GetValues(typeof(CsDComponent));

			foreach(CsDComponent component in components)
			{
				_debugLevel.Add(component, CsDLevel.Default);
			}
		}

		//DebugChar = GameObject.Find("PlayerCharacter").GetComponent<Character>();

		_bufferSize = 300000;
		_bufferedLog = "";
		_bufferedLog = "\r\n\r\n\r\n****************************\r\n\r\nNew Play Through " + DateTime.Now.ToString() 
			+ "\r\n\r\n****************************\r\n\r\n";
	}



	void OnApplicationQuit()
	{
		if(_logTarget == CsDLogTarget.File || _logTarget == CsDLogTarget.Both)
		{
			_bufferedLog = _bufferedLog + "\r\n\r\n\r\n****************************\r\n\r\nApplication Quit " + DateTime.Now.ToString() 
										+ "\r\n\r\n****************************\r\n\r\n";
			LogBufferToFile();
		}
	}



	public void CharLog(Character character, string log)
	{
		if(character == DebugChar)
		{
			UnityEngine.Debug.Log(Mathf.Round(Time.realtimeSinceStartup * 1000)/1000 + log);
		}
	}

	public void Log(string log, CsDLevel level, CsDComponent component)
	{
		
		if((int)_debugLevel[component] >= (int)level)
		{
			StackTrace stackTrace = new StackTrace();
			var method = stackTrace.GetFrame(1).GetMethod();
			var parentClass = method.ReflectedType;
			var parentNameSpace = parentClass.Namespace;

			string formattedLog = Mathf.Round(Time.realtimeSinceStartup * 1000)/1000 + " " + component.ToString() + " " + level.ToString() + " " 
									+ parentNameSpace + "/" + parentClass.Name + "/" + method.Name + ": " + log;

			//if log target is console log, just use debug.log
			//if is file, add the log line to the buffered log, and if it's larger than buffer size, write to file and
			//clear the buffer

			if(_logTarget == CsDLogTarget.Console || _logTarget == CsDLogTarget.Both)
			{
				if(level == CsDLevel.Error)
				{
					UnityEngine.Debug.LogAssertion(formattedLog);
				}
				else
				{
					UnityEngine.Debug.Log(formattedLog);
				}
			}

			if(_logTarget == CsDLogTarget.File || _logTarget == CsDLogTarget.Both)
			{
				_bufferedLog = _bufferedLog + formattedLog + "\r\n";
				
				if(_bufferedLog.Length > _bufferSize)
				{
					LogBufferToFile();
				}
			}

		}
	}

	public static Dictionary<string, object> ParseLines(string [] rawFile)
	{
		Dictionary<string, object> data = new Dictionary<string, object>();

		foreach(string s in rawFile)
		{
			//parse the file, ignoring lines starting with ';'
			if(s[0] == ';' || s[0] == '[' || s[0] == '\n')
				continue;
			string [] splitString = s.Split('=');
			if(splitString.Length < 2)
				continue;
			data.Add(splitString[0], splitString[1]);

		}

		return data;
	}

	public static Dictionary<CsDComponent, CsDLevel> LoadDebugSetting(out CsDLogTarget logTarget)
	{
		string [] rawFile;

		try
		{
			rawFile = File.ReadAllLines(Application.dataPath + "/GameData/" + "Debugs.txt");
		}
		catch(Exception e)
		{
			logTarget = CsDLogTarget.File;
			UnityEngine.Debug.LogError(e.Message);
			return null;
		}


		Dictionary<string, object> data = CsDebug.ParseLines(rawFile);
		Dictionary<CsDComponent, CsDLevel> debugLevel = new Dictionary<CsDComponent, CsDLevel>();

		//build the dictionary with default values
		CsDComponent [] components = (CsDComponent[])Enum.GetValues(typeof(CsDComponent));
		foreach(CsDComponent component in components)
		{
			debugLevel.Add(component, CsDLevel.Default);
		}

		logTarget = (CsDLogTarget)System.Enum.Parse(typeof(CsDLogTarget), data["LogTarget"].ToString());
		data.Remove("LogTarget");

		foreach(KeyValuePair<string, object> line in data)
		{
			CsDComponent component = (CsDComponent)System.Enum.Parse(typeof(CsDComponent), line.Key);
			CsDLevel level = (CsDLevel)System.Enum.Parse(typeof(CsDLevel), line.Value.ToString());


			debugLevel[component] = level;

		}

		return debugLevel;
	}

	public static bool SaveDebugSetting(Dictionary<CsDComponent, CsDLevel> setting, CsDLogTarget logTarget)
	{
		string path = GameManager.Inst.AppDataPath + "/GameData/";

		if(!MakeSureFileExists(path, "Debugs.txt"))
		{
			return false;
		}

		//clear the file

		System.IO.File.WriteAllText(path + "Debugs.txt", "LogTarget=" + logTarget + "\r\n");

		foreach(KeyValuePair<CsDComponent,CsDLevel> line in setting)
		{
			string text = line.Key.ToString() + "=" + line.Value.ToString() + "\r\n";
			try
			{
				System.IO.File.AppendAllText(path + "Debugs.txt", text);
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogError(e.Message);
				return false;
			}
		}


		return true;
	}





	private static bool MakeSureFileExists(string path, string filename)
	{
		if(!System.IO.Directory.Exists(path))
		{
			try
			{
				System.IO.Directory.CreateDirectory(path);
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogError(e.Message);
				return false;
			}
		}
		if(!System.IO.File.Exists(path + filename))
		{
			try
			{
				System.IO.File.Create(path + filename).Dispose();
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogError(e.Message);
				return false;
			}
		}

		return true;
	}





	private void LogBufferToFile()
	{
		string path = GameManager.Inst.AppDataPath + "/GameData/";

		if(!MakeSureFileExists(path, "DebugLog.txt"))
		{
			return;
		}

		try
		{
			File.AppendAllText(path + "DebugLog.txt", "\r\n" + _bufferedLog);
			_bufferedLog = "";
		}
		catch(Exception e)
		{
			return;
		}
	}
}





