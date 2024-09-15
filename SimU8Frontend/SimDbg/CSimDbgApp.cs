namespace SimDbg;

public class CSimDbgApp
{
	public const int TRACESIZE = 262144;

	public const byte BIT_ADRSBRK = 1;

	public const byte BIT_RAMDMBRK = 2;

	public const ushort BreakCond_PD = 1;

	public const ushort BreakCond_TF = 2;

	public const ushort BreakCond_XP = 4;

	public const ushort BreakCond_BP = 8;

	public const ushort BreakCond_NROM = 16;

	public const ushort BreakCond_NRAM = 32;

	public const ushort none = 0;

	public const ushort powerDown = 1;

	public const ushort tmpAdrs = 2;

	public const ushort permBreakPoint = 4;

	public const ushort ramMatch = 8;

	public const ushort step = 64;

	public const ushort forceByUser = 128;

	public const ushort traceMemFull = 256;

	public const ushort externalSignal = 512;

	public const ushort romNAAccess = 1024;

	public const ushort ramNAAccess = 2048;

	private U8Memory m_IEMemory = new U8Memory();

	private uint m_IEIncCount;

	private TRMEM[] m_TraceMemory;

	private uint m_TracePointer;

	private readonly uint m_TraceNum;

	private byte m_TraceLapFlag;

	private uint m_TraceCountBPval;

	private uint m_TraceCountBPinitval;

	private ulong m_CycleCounter;

	private byte m_CycleCounterOVF;

	private ulong m_beforeCycleCounter;

	private uint m_BreakStatus;

	private ushort m_BreakCondition;

	private BRKPARAM m_BreakParam;

	public static CSimDbgApp theApp = new CSimDbgApp();

	public CSimDbgApp()
	{
		m_TraceMemory = new TRMEM[262144];
		m_BreakParam.InitDMParam();
		m_IEIncCount = 0u;
		for (int i = 0; i < 262144; i++)
		{
			m_TraceMemory[i].pc = 0u;
			m_TraceMemory[i].psw = 0;
			m_TraceMemory[i].ramad = 0u;
			m_TraceMemory[i].ramdt = 0;
			m_TraceMemory[i].ramdt16 = 0;
			m_TraceMemory[i].probe = 0;
			m_TraceMemory[i].intcycle = 0;
			m_TraceMemory[i].atr = 0;
		}
		m_TracePointer = 0u;
		m_TraceNum = 0u;
		m_TraceLapFlag = 0;
		m_TraceCountBPval = 3u;
		m_TraceCountBPinitval = 0u;
		m_CycleCounter = 0uL;
		m_CycleCounterOVF = 0;
		m_beforeCycleCounter = 0uL;
		m_BreakCondition = 56;
		m_BreakStatus = 0u;
		m_BreakParam.adrbrk_adrs = 0u;
		m_BreakParam.adrbrk_pcnt = 1;
		for (int i = 0; i < 4; i++)
		{
			m_BreakParam.dm_param[i].ramadrs = 0u;
			m_BreakParam.dm_param[i].ramadrsmask = uint.MaxValue;
			m_BreakParam.dm_param[i].ramdata = 0;
			m_BreakParam.dm_param[i].ramdatamask = byte.MaxValue;
			m_BreakParam.dm_param[i].condition = 1;
			m_BreakParam.dm_param[i].access = 0;
		}
		m_BreakParam.dm_pcnt = 1;
		m_BreakParam.dm_pair = 6;
		m_BreakParam.brkcond = 0;
	}

	public bool InitInstance()
	{
		return true;
	}

	public int dbgApi_SetIEVal(uint nIndex, byte val)
	{
		return theApp.m_IEMemory.SetVal(nIndex, val);
	}

	public int dbgApi_GetIEVal(uint nIndex, ref byte val)
	{
		return theApp.m_IEMemory.GetVal(nIndex, ref val);
	}

	public int dbgApi_SetIEWordVal(uint nIndex, ushort val)
	{
		return theApp.m_IEMemory.SetWordVal(nIndex, val);
	}

	public int dbgApi_GetIEWordVal(uint nIndex, ref ushort val)
	{
		return theApp.m_IEMemory.GetWordVal(nIndex, ref val);
	}

	public int dbgApi_SetIERange(uint sadr, uint eadr)
	{
		return theApp.m_IEMemory.SetRange(sadr, eadr);
	}

	public int dbgApi_GetIERange(ref uint sadr, ref uint eadr)
	{
		theApp.m_IEMemory.GetRange(ref sadr, ref eadr);
		return 0;
	}

	public int dbgApi_GetIEIncCount(ref uint cnt)
	{
		cnt = theApp.m_IEIncCount;
		theApp.m_IEIncCount = 0u;
		return 0;
	}

	public int dbgApi_SetIECount(uint cnt)
	{
		theApp.m_IEIncCount = cnt;
		return 0;
	}

	public int dbgApi_GetIECount(ref uint cnt)
	{
		cnt = theApp.m_IEIncCount;
		return 0;
	}

	public int dbgApi_WriteTraceMemory(uint tp, ushort n, ref ushort m, ref TRMEM tracedat)
	{
		int num = 0;
		uint num2 = 0u;
		if (num == 0)
		{
			uint num3 = tp;
			do
			{
				theApp.m_TraceMemory[num3].atr = tracedat.atr;
				theApp.m_TraceMemory[num3].intcycle = tracedat.intcycle;
				theApp.m_TraceMemory[num3].pc = tracedat.pc;
				theApp.m_TraceMemory[num3].probe = tracedat.probe;
				theApp.m_TraceMemory[num3].psw = tracedat.psw;
				theApp.m_TraceMemory[num3].ramad = tracedat.ramad;
				theApp.m_TraceMemory[num3].ramdt = tracedat.ramdt;
				theApp.m_TraceMemory[num3].ramdt16 = tracedat.ramdt16;
				num3++;
				num2++;
				if (num3 >= 262144)
				{
					num3 = 0u;
					theApp.m_TraceLapFlag = 1;
				}
			}
			while (num2 < n);
			theApp.m_TracePointer = num3;
		}
		m = (ushort)num2;
		return num;
	}

	public int dbgApi_ReadTraceMemory(uint tp, ushort n, ref ushort m, TRMEM[] tracedat)
	{
		int num = 0;
		uint num2 = 0u;
		if (tp > theApp.m_TracePointer && theApp.m_TraceLapFlag == 0)
		{
			num = -1;
		}
		if (n == 0)
		{
			num = -1;
		}
		if (num == 0)
		{
			uint num3;
			uint num4;
			uint num5;
			if (BM.I2bool(theApp.m_TraceLapFlag))
			{
				num3 = theApp.m_TracePointer + tp;
				num4 = theApp.m_TracePointer + 262144;
				num5 = num3 + n;
			}
			else
			{
				num3 = tp;
				num4 = theApp.m_TracePointer;
				num5 = tp + n;
			}
			while (num3 < num5)
			{
				if (num3 < 262144)
				{
					tracedat[num2].pc = theApp.m_TraceMemory[num3].pc;
					tracedat[num2].psw = theApp.m_TraceMemory[num3].psw;
					tracedat[num2].ramad = theApp.m_TraceMemory[num3].ramad;
					tracedat[num2].ramdt = theApp.m_TraceMemory[num3].ramdt;
					tracedat[num2].ramdt16 = theApp.m_TraceMemory[num3].ramdt16;
					tracedat[num2].probe = theApp.m_TraceMemory[num3].probe;
					tracedat[num2].intcycle = theApp.m_TraceMemory[num3].intcycle;
					tracedat[num2].atr = theApp.m_TraceMemory[num3].atr;
				}
				else
				{
					tracedat[num2].pc = theApp.m_TraceMemory[num3 - 262144].pc;
					tracedat[num2].psw = theApp.m_TraceMemory[num3 - 262144].psw;
					tracedat[num2].ramad = theApp.m_TraceMemory[num3 - 262144].ramad;
					tracedat[num2].ramdt = theApp.m_TraceMemory[num3 - 262144].ramdt;
					tracedat[num2].ramdt16 = theApp.m_TraceMemory[num3 - 262144].ramdt16;
					tracedat[num2].probe = theApp.m_TraceMemory[num3 - 262144].probe;
					tracedat[num2].intcycle = theApp.m_TraceMemory[num3 - 262144].intcycle;
					tracedat[num2].atr = theApp.m_TraceMemory[num3 - 262144].atr;
				}
				num3++;
				num2++;
				if (num3 > num4)
				{
					break;
				}
			}
		}
		m = (ushort)num2;
		return num;
	}

	public int dbgApi_GetTracePointer(ref uint tp, ref byte buf1)
	{
		if (0 == 0)
		{
			tp = theApp.m_TracePointer;
			buf1 = theApp.m_TraceLapFlag;
		}
		return 0;
	}

	public int dbgApi_SetTraceCountBP(uint cnt)
	{
		theApp.m_TraceCountBPval = cnt;
		theApp.m_TraceCountBPinitval = cnt;
		return 0;
	}

	public int dbgApi_SetTraceCountBP_update(uint cnt)
	{
		theApp.m_TraceCountBPval = cnt;
		return 0;
	}

	public int dbgApi_GetTraceCountBP(ref uint cnt)
	{
		cnt = theApp.m_TraceCountBPval;
		return 0;
	}

	public int dbgApi_GetTraceCountBP_update(ref uint cnt)
	{
		cnt = theApp.m_TraceCountBPval;
		return 0;
	}

	public int dbgApi_SearchTraceMemory(byte sign, ushort rID, uint compdata, uint mask, uint cnt, ref uint tp)
	{
		int num = 0;
		uint num2 = 0u;
		uint num3 = 0u;
		switch (sign)
		{
		case 43:
			num2 = ((theApp.m_TracePointer != 0) ? (theApp.m_TracePointer - 1) : 262143u);
			num3 = ((theApp.m_TraceLapFlag != 0) ? theApp.m_TracePointer : 0u);
			break;
		case 45:
			num3 = ((theApp.m_TracePointer != 0) ? (theApp.m_TracePointer - 1) : 262143u);
			num2 = ((theApp.m_TraceLapFlag != 0) ? theApp.m_TracePointer : 0u);
			break;
		default:
			num = -1;
			break;
		}
		if (num == 0)
		{
			while (num == 0)
			{
				switch (rID)
				{
				case 14336:
					if (compdata == (theApp.m_TraceMemory[num2].pc & mask))
					{
						cnt--;
					}
					break;
				case 14337:
					if (compdata == (theApp.m_TraceMemory[num2].ramad & mask))
					{
						cnt--;
					}
					break;
				case 14338:
					if (compdata == (theApp.m_TraceMemory[num2].ramdt & mask))
					{
						cnt--;
					}
					break;
				case 14363:
					if (compdata == (theApp.m_TraceMemory[num2].ramdt16 & mask))
					{
						cnt--;
					}
					break;
				case 14352:
					if (compdata == (theApp.m_TraceMemory[num2].psw & mask))
					{
						cnt--;
					}
					break;
				case 14353:
					if (compdata == ((theApp.m_TraceMemory[num2].psw >> 7) & 1 & mask))
					{
						cnt--;
					}
					break;
				case 14354:
					if (compdata == ((theApp.m_TraceMemory[num2].psw >> 6) & 1 & mask))
					{
						cnt--;
					}
					break;
				case 14355:
					if (compdata == ((theApp.m_TraceMemory[num2].psw >> 5) & 1 & mask))
					{
						cnt--;
					}
					break;
				case 14356:
					if (compdata == ((theApp.m_TraceMemory[num2].psw >> 4) & 1 & mask))
					{
						cnt--;
					}
					break;
				case 14357:
					if (compdata == ((theApp.m_TraceMemory[num2].psw >> 3) & 1 & mask))
					{
						cnt--;
					}
					break;
				case 14358:
					if (compdata == ((theApp.m_TraceMemory[num2].psw >> 2) & 1 & mask))
					{
						cnt--;
					}
					break;
				case 14359:
					if (compdata == (theApp.m_TraceMemory[num2].psw & 3 & mask))
					{
						cnt--;
					}
					break;
				case 14360:
				case 14361:
				case 14362:
					num = -1;
					break;
				default:
					num = -1;
					break;
				}
				if (num == 0)
				{
					if (cnt == 0)
					{
						if (theApp.m_TraceLapFlag == 1)
						{
							if (theApp.m_TracePointer <= num2)
							{
								tp = num2 - theApp.m_TracePointer;
							}
							else
							{
								tp = 262144 - theApp.m_TracePointer + num2;
							}
						}
						else
						{
							tp = num2;
						}
						break;
					}
					if (num2 == num3)
					{
						num = -1;
					}
				}
				if (num == 0)
				{
					switch (sign)
					{
					case 43:
						num2 = ((num2 != 0) ? (num2 - 1) : 262143u);
						break;
					case 45:
						num2 = ((num2 != 262143) ? (num2 + 1) : 0u);
						break;
					}
				}
			}
		}
		return num;
	}

	public int dbgApi_ClearTracePointer()
	{
		theApp.m_TracePointer = 0u;
		theApp.m_TraceLapFlag = 0;
		return 0;
	}

	public int dbgApi_GetCycleCounter(ref ulong cnt, ref byte ovf)
	{
		if (0 == 0)
		{
			cnt = theApp.m_CycleCounter;
			ovf = theApp.m_CycleCounterOVF;
		}
		return 0;
	}

	public int dbgApi_SetCycleCounter(ulong cnt)
	{
		theApp.m_CycleCounter = cnt;
		theApp.m_CycleCounterOVF = 0;
		return 0;
	}

	public int dbgApi_IncCycleCounter(ulong val)
	{
		theApp.m_beforeCycleCounter = theApp.m_CycleCounter;
		theApp.m_CycleCounter += val;
		if (theApp.m_beforeCycleCounter > theApp.m_CycleCounter)
		{
			theApp.m_CycleCounterOVF = 1;
		}
		theApp.m_beforeCycleCounter = theApp.m_CycleCounter;
		return 0;
	}

	public int dbgApi_SetBreakStatus(uint status)
	{
		theApp.m_BreakStatus = status;
		return 0;
	}

	public int dbgApi_GetBreakStatus(ref uint status)
	{
		if (0 == 0)
		{
			status = theApp.m_BreakStatus;
		}
		return 0;
	}

	public int dbgApi_SetBreakCondition(ushort cond)
	{
		theApp.m_BreakCondition = cond;
		return 0;
	}

	public int dbgApi_GetBreakCondition(ref ushort cond)
	{
		if (0 == 0)
		{
			cond = theApp.m_BreakCondition;
		}
		return 0;
	}

	public int dbgApi_SetBreakParam(ref BRKPARAM param)
	{
		int result = 0;
		theApp.m_BreakParam.adrbrk_adrs = param.adrbrk_adrs;
		theApp.m_BreakParam.adrbrk_pcnt = param.adrbrk_pcnt;
		theApp.m_BreakParam.brkcond = param.brkcond;
		theApp.m_BreakParam.dm_pair = param.dm_pair;
		theApp.m_BreakParam.dm_pcnt = param.dm_pcnt;
		for (int i = 0; i < 4; i++)
		{
			theApp.m_BreakParam.dm_param[i].access = param.dm_param[i].access;
			theApp.m_BreakParam.dm_param[i].condition = param.dm_param[i].condition;
			theApp.m_BreakParam.dm_param[i].ramadrs = param.dm_param[i].ramadrs;
			theApp.m_BreakParam.dm_param[i].ramadrsmask = param.dm_param[i].ramadrsmask;
			theApp.m_BreakParam.dm_param[i].ramdata = param.dm_param[i].ramdata;
			theApp.m_BreakParam.dm_param[i].ramdatamask = param.dm_param[i].ramdatamask;
		}
		return result;
	}

	public int dbgApi_GetBreakParam(ref BRKPARAM param)
	{
		int num = 0;
		if (num == 0)
		{
			param.adrbrk_adrs = theApp.m_BreakParam.adrbrk_adrs;
			param.adrbrk_pcnt = theApp.m_BreakParam.adrbrk_pcnt;
			param.brkcond = theApp.m_BreakParam.brkcond;
			param.dm_pair = theApp.m_BreakParam.dm_pair;
			param.dm_pcnt = theApp.m_BreakParam.dm_pcnt;
			for (int i = 0; i < 4; i++)
			{
				param.dm_param[i].access = theApp.m_BreakParam.dm_param[i].access;
				param.dm_param[i].condition = theApp.m_BreakParam.dm_param[i].condition;
				param.dm_param[i].ramadrs = theApp.m_BreakParam.dm_param[i].ramadrs;
				param.dm_param[i].ramadrsmask = theApp.m_BreakParam.dm_param[i].ramadrsmask;
				param.dm_param[i].ramdata = theApp.m_BreakParam.dm_param[i].ramdata;
				param.dm_param[i].ramdatamask = theApp.m_BreakParam.dm_param[i].ramdatamask;
			}
		}
		return num;
	}
}
