#pragma once

#include <windows.h>

class ePSCIce
{
public:
    ePSCIce();
    virtual ~ePSCIce();

    void A7KeyOn(bool);
    void Break();
    int BuildRomMap(char*);
    int CheckRamBreakPoints();
    int CheckRomBreakPoints();
    void ClearRamBP();
    void ClearRomBP();
    bool ClearTraceMemory();
    DWORD DownLoad(char*, WORD, WORD, WORD, WORD);
    int DownLoadDataRom(char*);
    int DownLoadExternalDataRom(char*);
    int DownLoadInternalDataRom(char*);
    int ExecuteOneInstruction();
    int FreeRun();
    void GetBankedRam(DWORD*, DWORD, DWORD, DWORD);
    void GetBankedRamForMemoryCheck(DWORD*, DWORD, DWORD, DWORD);
    int GetBody();
    int GetCPUMode();
    int GetFileLine(char*, DWORD*);
    void GetFlash(BYTE*, DWORD, DWORD);
    DWORD GetPC();
    void GetPageFromLCDRam(DWORD, DWORD*);
    void GetPageFromLCDRam_136X132(DWORD, DWORD*);
    void GetPageFromLCDRam_160X160(DWORD, DWORD*);
    void GetPageFromLCDRam_80X160(DWORD, DWORD*);
    void GetPageFromLCDRam_Normal(DWORD, DWORD*);
    DWORD GetRegister(DWORD);
    DWORD GetRegisterBit(DWORD, DWORD);
    void GetRom(WORD*, DWORD, DWORD);
    void GetSFR(char*, int, int);
    int GetStatus();
    bool GetTraceMemory(DWORD, DWORD, DWORD*);
    int GotoCursor(char*, DWORD);
    void HaltCPU();
    int IceConnect();
    int Initialize(WORD, WORD, WORD, WORD);
    int IsCPUHalt();
    int IsConnect();
    int IsDownLoad();
    int IsHardware();
    int IsInitialize();
    void KeyDown(int, int, int, int);
    void KeyUp(int, int, int, int);
    int Prepare();
    void ReleaseCPU();
    void RemoveAllBreakPoints();
    void Reset();
    int Run();
    void SetBankedRam(DWORD*, DWORD, DWORD, DWORD);
    void SetBody(int, int, int);
    int SetBreakPoint(int, int);
    void SetFlashType(int);
    void SetICEMD(int);
    void SetLCDMemType(int);
    void SetPMD(int);
    void SetRamBP(void* pArray); // Simplified for CArray<RAMBP, RAMBP&>
    void SetRegister(DWORD, DWORD);
    void SetRegisterBit(DWORD, DWORD, DWORD);
    void SetRomBP(void* pArray); // Simplified for CArray<ROMBP, ROMBP&>
    void SetStatus(int);
    int StepInto();
    int StepOver();
    void Stop();
    void UseHardware(int, int);
    void WakeUp();
    void WriteFlash(BYTE*, int, int);
    void call_count(int, int);
    int get_cycle();
    long FileLineToAdr(char*, DWORD);
    int NextInstructionType();
    void GetBankedRAM_LCD(DWORD*, DWORD, DWORD, DWORD);
    char unknown[0xf00000];
};