public enum CsDLevel
{
	Default=0,//will always be logged
	Error=1,//any log that's considered error notification
	Info=2,//best for when an object is created
	Debug=3,//best for logs inside methods but not inside loops
	Trace=4,//only used inside loops
}

public enum CsDComponent
{
	GUI,
	AI,
	Character,
}

public enum CsDLogTarget
{
	Console,
	File,
	Both,
}