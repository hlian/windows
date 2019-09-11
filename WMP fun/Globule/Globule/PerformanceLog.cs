using System;
using System.IO;

static class PerformanceLog
{
	static DateTime startTime;

	public static void StartLog()
	{
		startTime = DateTime.Now;
	}

	static void LogBase(string intro, bool updateTime)
	{
		using (StreamWriter tmpStreamWriter = new StreamWriter(new FileStream(@"C:\globule.txt",
	FileMode.Append, FileAccess.Write)))
		{
			tmpStreamWriter.WriteLine(intro + " " + (DateTime.Now - startTime).ToString());
			if (updateTime) startTime = DateTime.Now;
		}
	}

	public static void Log(string intro)
	{
		LogBase(intro, true);
	}

	public static void EndLog(string intro)
	{
		LogBase(intro, false);
	}
}