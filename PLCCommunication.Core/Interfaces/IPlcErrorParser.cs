using System;

namespace PLCCommunication.Core.Interfaces
{
    public interface IPlcErrorParser
    {
        event Action<string, long> WarningL1Emit;
        event Action<string, long> WarningL2Emit;
        event Action<string, long> WarningL3Emit;
        event Action<string, long> WarningL4Emit;
        void ParseErrorCode(long errorCode);
    }
}