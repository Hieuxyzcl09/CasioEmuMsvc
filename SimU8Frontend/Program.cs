// See https://aka.ms/new-console-template for more information
using SimU8;

CSimU8App.SetCodeMemoryDefaultCode(0);
byte[] program = File.ReadAllBytes("rom.bin");
CSimU8App.WriteCodeMemory(0, (uint)program.LongLength, program);
// CSimU8App.SetCodeMemorySize
CSimU8App.LogStart();
CSimU8App.SetRomWindowSize(0, 0x8fff);
CSimU8App.SetMemoryModel(1);
CSimU8App.SimStart();

uint pc = 0;
while (true)
{
    // CSimU8App.theApp.simu8.m_SimRunFlag = 0;
    CSimU8App.ReadReg(16, ref pc);
    Console.WriteLine($"{pc:x6}");
    CSimU8App.ReadReg(26, ref pc);
    Console.WriteLine($"{pc:x2}");
}