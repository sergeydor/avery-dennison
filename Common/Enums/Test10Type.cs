using System;

namespace Common.Enums
{
	//[Flags]
	//public enum Test10Type : byte
	//{
 //       NotSet = 0,
 //       ForceKill = 1,
	//	IDFilter = 8,
	//	TIDTest = 16,
	//	SensTest = 32,
	//	WriteTest = 64,
	//	ReadTest = 128
	//}

    [Flags]
    public enum Test10Type : byte
    {
        NotSet = 0,
        ForceKill = 128,
        IDFilter = 16,
        TIDTest = 8,
        SensTest = 4,
        WriteTest = 2,
        ReadTest = 1
    }
}