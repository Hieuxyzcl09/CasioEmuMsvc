using System;
using System.Runtime.InteropServices;
using System.Threading;
using SimDbg;
using SimU8engine;

namespace SimU8;

public class CSimU8App
{
	public const int TIMEOUT_VALUE = 3000;

	public const int WAIT_TIMEOUT_VALUE = 30000;

	public const int ERR_METEREDSECTION_TIMEOUT = 15;

	public const int ERR_SIMU8_FATAL_ERROR = 16;

	public const int MODE_STOP = 0;

	public const int MODE_RUN = 1;

	public const int MODE_STOPRQ = 2;

	public const int SIMU8_ER_API_BUSY = 1;

	public const int SIMU8_ER_ADRS_INVALID = 2;

	public const int SIMU8_ER_ADRS_TOO_LONG = 3;

	public const int SIMU8_ER_NO_SUCH_REGISTER = 4;

	public const int SIMU8_ER_MEMORY_INVALID = 5;

	public const int SIMU8_ER_P_FILE_OPEN_FAILED = 7;

	public const int SIMU8_ER_MUTEX_TIMEOUT = 15;

	public const int SIMU8_ER_SIMU8_FATAL_ERROR = 16;

	public const int SIMU8_ER_INTERRUPT_SIZE_TO_BIG = 17;

	public const int SIMU8_ER_VECTOR_ADRS_INVALID = 18;

	public const int SIMU8_ER_IE_ADRS_INVALID = 19;

	public const int SIMU8_ER_IRQ_ADRS_INVALID = 20;

	public const int SIMU8_ER_IE_BIT_INVALID = 21;

	public const int SIMU8_ER_IRQ_BIT_INVALID = 22;

	public const int SIMU8_ER_SYMBOL_TOO_LONG = 23;

	public const int SIMU8_ER_IOFILE_NOT_EXIST = 24;

	public const int SIMU8_ER_CORE_MODE_INVALID = 25;

	public const int SIMU8_ER_MEM_DLL_TOO_LONG = 26;

	public const int SIMU8_ER_MEM_DLL_NOT_EXIST = 27;

	public const int SIMU8_ER_MEM_WAIT_INVALID = 28;

	public const int SIMU8_ER_DBG_DLL_TOO_LONG = 29;

	public const int SIMU8_ER_DBG_NOT_EXIST = 30;

	public const int SIMU8_ER_COP_DLL_TOO_LONG = 31;

	public const int SIMU8_ER_COP_NOT_EXIST = 32;

	public const int SIMU8_ER_COP_IF_TOO_LONG = 33;

	public const int SIMU8_ER_PERI_DLL_TOO_LONG = 34;

	public const int SIMU8_ER_PERI_NUM_INVALID = 35;

	public const int SIMU8_ER_PERI_ADR_INVALID = 36;

	public const int SIMU8_ER_PERI_INT_NUM_INVALID = 37;

	public const int SIMU8_ER_PERI_INT_VECTOR_INVALID = 38;

	public const int SIMU8_ER_PERI_IE_ADR_INVALID = 39;

	public const int SIMU8_ER_PERI_IE_BIT_INVALID = 40;

	public const int SIMU8_ER_PERI_IRQ_ADR_INVALID = 41;

	public const int SIMU8_ER_PERI_IRQ_BIT_INVALID = 42;

	public const int SIMU8_ER_PERI_SYMBOL_NON = 43;

	public const int SIMU8_ER_STOP_HALT_INFO_ILLEGAL = 44;

	public const int SIMU8_ER_EXCEPTION_ERROR = -1;

	public const int SIMU8_ER_DLL_NOT_EXIST = -2;

	public const int SIMU8_ER_SEG_SIZE_TO_BIG = 8;

	public const int SIMU8_ER_ILLEGAL_RECORD = 9;

	public const int SIMU8_ER_CHECKSUM = 10;

	public const int SIMU8_ER_INVALID_END_RECORD = 11;

	public const int SIMU8_ER_NO_END_RECORD = 12;

	public const int SIMU8_ER_NO_VALID_RECORD = 13;

	public const int SIMU8_ER_ILLEGAL_POS_END_RECORD = 14;

	public CSimU8main simu8 = new CSimU8main();

	private byte m_WatchSimCondition;

	private Thread m_pWatchSimThread;

	private lpFunc lpU8Func;

	private uint lU8Param;

	private int m_Waittime;

	private int run_mode;

	public object m_lpMeteredSection = new object();

	public static CSimU8App theApp = new CSimU8App();

	public CSimU8App()
	{
		lpU8Func = null;
		m_Waittime = 0;
	}

	public static int simThreadStart()
	{
		if (theApp.m_pWatchSimThread != null)
		{
			theApp.m_pWatchSimThread.Join();
		}
		theApp.m_pWatchSimThread = new Thread(WatchSimThread);
		theApp.m_WatchSimCondition = 1;
		theApp.m_pWatchSimThread.Start(theApp);
		return 0;
	}

	public static int SetCodeMemorySize(uint startadrs, uint endadrs)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					return 1;
				}
				return theApp.simu8.m_SetCodeMemorySize(startadrs, endadrs);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetDataMemorySize([MarshalAs(UnmanagedType.U4)] uint startadrs, [MarshalAs(UnmanagedType.U4)] uint endadrs)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					return 1;
				}
				return theApp.simu8.m_SetDataMemorySize(startadrs, endadrs);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetCallbackFunc([MarshalAs(UnmanagedType.FunctionPtr)] lpFunc lpFunc, [MarshalAs(UnmanagedType.U4)] uint lParam)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					result = 1;
				}
				else
				{
					theApp.lpU8Func = lpFunc;
					theApp.lU8Param = lParam;
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetWait([MarshalAs(UnmanagedType.U2)] ushort dwMilliseconds)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					result = 1;
				}
				else
				{
					theApp.m_Waittime = dwMilliseconds;
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetRomWindowSize([MarshalAs(UnmanagedType.U4)] uint startadrs, [MarshalAs(UnmanagedType.U4)] uint endadrs)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					return 1;
				}
				return theApp.simu8.m_SetRomWindowSize(startadrs, endadrs);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetCodeMemoryDefaultCode([MarshalAs(UnmanagedType.U2)] ushort val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					return 1;
				}
				return theApp.simu8.m_SetCodeMemoryDefaultCode(val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int LoadHexFile([MarshalAs(UnmanagedType.LPTStr)] string filename)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					return 1;
				}
				return theApp.simu8.m_LoadHexFile(filename);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SimReset()
	{
		int result;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				int num = theApp.run_mode;
				result = theApp.simu8.m_SimReset(num);
				if (num == 1)
				{
					BRKPARAM param = default(BRKPARAM);
					param.InitDMParam();
					param.adrbrk_adrs = 0u;
					param.adrbrk_pcnt = 0;
					param.brkcond = 0;
					param.dm_pair = 0;
					param.dm_pcnt = 0;
					for (int i = 0; i < 4; i++)
					{
						param.dm_param[i].access = 0;
						param.dm_param[i].condition = 0;
						param.dm_param[i].ramadrs = 0u;
						param.dm_param[i].ramadrsmask = 0u;
						param.dm_param[i].ramdata = 0;
						param.dm_param[i].ramdatamask = 0;
					}
					result = theApp.simu8.m_SetBreakCondition(0);
					result = theApp.simu8.m_SimStart_Break(ref param);
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SimStart()
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					result = 1;
				}
				else
				{
					BRKPARAM param = default(BRKPARAM);
					param.InitDMParam();
					param.adrbrk_adrs = 0u;
					param.adrbrk_pcnt = 0;
					param.brkcond = 0;
					param.dm_pair = 0;
					param.dm_pcnt = 0;
					for (int i = 0; i < 4; i++)
					{
						param.dm_param[i].access = 0;
						param.dm_param[i].condition = 0;
						param.dm_param[i].ramadrs = 0u;
						param.dm_param[i].ramadrsmask = 0u;
						param.dm_param[i].ramdata = 0;
						param.dm_param[i].ramdatamask = 0;
					}
					result = theApp.simu8.m_SetBreakCondition(0);
					result = theApp.simu8.m_SimStart_Break(ref param);
					theApp.run_mode = 1;
					simThreadStart();
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SimStop()
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition == 1)
				{
					theApp.run_mode = 0;
					result = theApp.simu8.m_SimStop();
				}
				Thread.Sleep(230);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Message : {0}", ex.Message);
				Console.WriteLine("Message : {0}", ex.StackTrace);
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int WriteCodeMemory([MarshalAs(UnmanagedType.U4)] uint adrs, [MarshalAs(UnmanagedType.U4)] uint len, [MarshalAs(UnmanagedType.U4)] byte[] val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					return 1;
				}
				return theApp.simu8.m_WriteCodeMemory(adrs, len, val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int ReadDataMemory([MarshalAs(UnmanagedType.U4)] uint adrs, [MarshalAs(UnmanagedType.U4)] uint len, [MarshalAs(UnmanagedType.LPArray)] byte[] val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_ReadDataMemory(adrs, len, val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int WriteDataMemory([MarshalAs(UnmanagedType.U4)] uint adrs, [MarshalAs(UnmanagedType.U4)] uint len, [MarshalAs(UnmanagedType.LPArray)] byte[] val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_WriteDataMemory(adrs, len, val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int WriteBitDataMemory([MarshalAs(UnmanagedType.U4)] uint adrs, [MarshalAs(UnmanagedType.U1)] byte n, byte val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_WriteBitDataMemory(adrs, n, val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int ReadReg([MarshalAs(UnmanagedType.U1)] byte regtype, [MarshalAs(UnmanagedType.SysUInt)] ref uint val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_ReadReg(regtype, ref val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int WriteReg([MarshalAs(UnmanagedType.U1)] byte regtype, [MarshalAs(UnmanagedType.U4)] uint val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_WriteReg(regtype, val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int LogStart()
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			Console.WriteLine("LogStart Monitor.TryEnter");
			try
			{
				result = theApp.simu8.m_LogStart();
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
			Console.WriteLine("LogStart Monitor.Exit");
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int LogStart2([MarshalAs(UnmanagedType.LPStr)] string fname)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_LogStart2(fname);
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int LogStop()
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_LogStop();
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int ReadCodeMemory([MarshalAs(UnmanagedType.U4)] uint adrs, [MarshalAs(UnmanagedType.U4)] uint len, byte[] val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_ReadCodeMemory(adrs, len, val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetCount([MarshalAs(UnmanagedType.U4)] ref uint cnt)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_GetCount(ref cnt);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetMappingMIO([MarshalAs(UnmanagedType.U2)] ushort n, [MarshalAs(UnmanagedType.LPArray)] uint[] startadrs, [MarshalAs(UnmanagedType.LPArray)] uint[] endadrs, [MarshalAs(UnmanagedType.LPArray)] byte[] atrb)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetMappingMIO(n, startadrs, endadrs, atrb));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetMappingMIO([MarshalAs(UnmanagedType.U2)] ref ushort n, [MarshalAs(UnmanagedType.LPArray)] uint[] startadrs, [MarshalAs(UnmanagedType.LPArray)] uint[] endadrs, [MarshalAs(UnmanagedType.LPArray)] byte[] atrb)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_GetMappingMIO(ref n, startadrs, endadrs, atrb);
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetMappingPIO([MarshalAs(UnmanagedType.U2)] ushort n, [MarshalAs(UnmanagedType.LPArray)] uint[] startadrs, [MarshalAs(UnmanagedType.LPArray)] uint[] endadrs, [MarshalAs(UnmanagedType.LPArray)] byte[] atrb)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetMappingPIO(n, startadrs, endadrs, atrb));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetMappingPIO([MarshalAs(UnmanagedType.U2)] ref ushort n, [MarshalAs(UnmanagedType.LPArray)] uint[] startadrs, [MarshalAs(UnmanagedType.LPArray)] uint[] endadrs, [MarshalAs(UnmanagedType.LPArray)] byte[] atrb)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_GetMappingPIO(ref n, startadrs, endadrs, atrb);
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetMappingGIO([MarshalAs(UnmanagedType.U2)] ushort n, [MarshalAs(UnmanagedType.LPArray)] uint[] startadrs, [MarshalAs(UnmanagedType.LPArray)] uint[] endadrs, [MarshalAs(UnmanagedType.LPArray)] byte[] atrb)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetMappingGIO(n, startadrs, endadrs, atrb));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetMappingGIO([MarshalAs(UnmanagedType.U2)] ref ushort n, [MarshalAs(UnmanagedType.LPArray)] uint[] startadrs, [MarshalAs(UnmanagedType.LPArray)] uint[] endadrs, [MarshalAs(UnmanagedType.LPArray)] byte[] atrb)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_GetMappingGIO(ref n, startadrs, endadrs, atrb);
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int FillMemoryU8(ushort rID, uint startadrs, uint endadrs, ushort size, uint val)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_FillMemoryU8(rID, startadrs, endadrs, size, val));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetBreakPoint([MarshalAs(UnmanagedType.U2)] ushort n, [MarshalAs(UnmanagedType.LPArray)] uint[] adrs, [MarshalAs(UnmanagedType.U2)] ref ushort num, [MarshalAs(UnmanagedType.LPArray)] ushort[] code)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetBreakPoint(n, adrs, ref num, code));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int ClearBreakPoint([MarshalAs(UnmanagedType.U2)] ushort n, [MarshalAs(UnmanagedType.LPArray)] uint[] adrs, [MarshalAs(UnmanagedType.LPArray)] ushort[] code)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_ClearBreakPoint(n, adrs, code));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetBreakStatus(ref uint status)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_GetBreakStatus(ref status);
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetBreakCondition(ushort cond)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetBreakCondition(cond));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetBreakCondition(ref ushort cond)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_GetBreakCondition(ref cond);
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SimStart_Break(ref BRKPARAM param)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					result = 1;
				}
				else
				{
					result = theApp.simu8.m_SimStart_Break(ref param);
					simThreadStart();
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SimStart_Restart(ushort code, ref BRKPARAM param)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					result = 1;
				}
				else
				{
					result = theApp.simu8.m_SimStart_Restart(code, param);
					simThreadStart();
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int StepIn()
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					result = 1;
				}
				else
				{
					result = theApp.simu8.m_StepIn();
					simThreadStart();
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int StepOver()
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				if (theApp.m_WatchSimCondition != 0)
				{
					result = 1;
				}
				else
				{
					result = theApp.simu8.m_StepOver();
					simThreadStart();
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int CheckBreakGo(ref byte status, ref uint nextpc, ref uint breakpc, ref uint brkstatus, ref ushort adr_brk_passcnt, ref ushort rammatch_brk_passcnt)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_CheckBreakGo(ref status, ref nextpc, ref breakpc, ref brkstatus, ref adr_brk_passcnt, ref rammatch_brk_passcnt);
				if ((theApp.m_WatchSimCondition != 1 && status == 0) || (theApp.m_WatchSimCondition != 0 && status == 1))
				{
					result = 1;
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int CheckBreakStep(ref byte status, ref uint nextpc, ref uint breakpc, ref uint brkstatus)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_CheckBreakStep(ref status, ref nextpc, ref breakpc, ref brkstatus);
				if ((theApp.m_WatchSimCondition != 1 && status == 0) || (theApp.m_WatchSimCondition != 0 && status == 1))
				{
					result = 1;
				}
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int ReadTraceMemory([MarshalAs(UnmanagedType.U4)] uint tp, [MarshalAs(UnmanagedType.U2)] ushort n, [MarshalAs(UnmanagedType.U2)] ref ushort m, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] TRMEM[] tracedat)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_ReadTraceMemory(tp, n, ref m, ref tracedat));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetTracePointer(ref uint tp, ref byte buf1)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_GetTracePointer(ref tp, ref buf1));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetTraceCountBP(uint cnt)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetTraceCountBP(cnt));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetTraceCountBP(ref uint cnt)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_GetTraceCountBP(ref cnt));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SearchTraceMemory(byte sign, ushort rID, uint compdata, uint mask, uint cnt, ref uint tp)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SearchTraceMemory(sign, rID, compdata, mask, cnt, ref tp));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int ClearTracePointer()
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_ClearTracePointer());
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetCycleCounter(ref ulong cnt, ref byte ovf)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_GetCycleCounter(ref cnt, ref ovf);
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetCycleCounter(ulong cnt)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetCycleCounter(cnt));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int ReadIEMemory([MarshalAs(UnmanagedType.U4)] uint adrs, [MarshalAs(UnmanagedType.U4)] uint len, [MarshalAs(UnmanagedType.LPArray)] byte[] val)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_ReadIEMemory(adrs, len, val));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int ReadRegAll([MarshalAs(UnmanagedType.LPArray)] uint[] val)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_ReadRegAll(val);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetIEIncCount(ref uint cnt)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_GetIEIncCount(ref cnt);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetIECntAddress(uint start, uint end, ref uint cnt)
	{
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				return theApp.simu8.m_SetIECntAddress(start, end, ref cnt);
			}
			catch (Exception)
			{
				return -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		return 15;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetCoreRevision(string pRev)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetCoreRevision(pRev));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetInterruptSetting([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] INTERRUPTSETTING[] intsetting)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetInterruptSetting(intsetting));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int GetInterruptSetting([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] INTERRUPTSETTING[] intsetting)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = theApp.simu8.m_GetInterruptSetting(intsetting);
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetTargetName(string tname)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetTargetName(tname));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetInterruptInfo([MarshalAs(UnmanagedType.U1)] byte intnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] INTERRUPTTABLE[] intinfo)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetInterruptInfo(intnum, ref intinfo));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetInterruptTable([MarshalAs(UnmanagedType.U1)] byte intnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] SIMU8_INTERRUPT_TABLE[] intinfo)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetInterruptTable(intnum, ref intinfo));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.I4)]
	public static int SetMemoryModel(byte model)
	{
		int result = 0;
		if (Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
		{
			try
			{
				result = ((theApp.m_WatchSimCondition != 0) ? 1 : theApp.simu8.m_SetMemoryModel(model));
			}
			catch (Exception)
			{
				result = -1;
			}
			finally
			{
				Monitor.Exit(theApp.m_lpMeteredSection);
			}
		}
		else
		{
			result = 15;
		}
		return result;
	}

	public static void WatchSimThread(object pParam)
	{
		int num = 0;
		int num2 = 0;
		theApp.simu8.m_CycleCountEnable = 0;
		if (theApp.simu8.m_LogState() != 0)
		{
			if (theApp.lpU8Func != null)
			{
				new COutMod().W(" Callback Func: ").W(theApp.lpU8Func.ToString()).endl();
			}
			else
			{
				new COutMod().W(" Callback Func: NULL").endl();
			}
			new COutMod().W(" Wait Time: ").W(theApp.m_Waittime.ToString()).endl();
		}
		while (true)
		{
			num2++;
			if (!Monitor.TryEnter(theApp.m_lpMeteredSection, 3000))
			{
				break;
			}
			if (theApp.m_WatchSimCondition == 1)
			{
				if (theApp.simu8.m_CycleCountEnable < 3)
				{
					theApp.simu8.m_CycleCountEnable++;
				}
				theApp.simu8.m_GetExternalWait();
				if (theApp.simu8.m_CycleCountEnable >= 3)
				{
					theApp.simu8.m_ExternalRun();
				}
				byte simRun = theApp.simu8.GetSimRun();
				if (simRun == 0 || simRun == 3 || simRun == 15 || simRun == 16)
				{
					if (num == 0)
					{
						if (theApp.simu8.m_CycleCountEnable >= 3)
						{
							theApp.simu8.m_InternalRun();
						}
						int num3 = theApp.simu8.m_Execute();
						simRun = theApp.simu8.GetSimRun();
						if (simRun == 0 && num3 != 0)
						{
							simRun = 4;
						}
						num = theApp.m_Waittime;
						Monitor.Exit(theApp.m_lpMeteredSection);
						if ((simRun >= 5 && simRun <= 11) || simRun == 13 || simRun == 14)
						{
							theApp.m_WatchSimCondition = 2;
							if (theApp.lpU8Func != null)
							{
								theApp.lpU8Func(simRun, theApp.lU8Param);
							}
						}
						else if (simRun != 0 && theApp.lpU8Func != null)
						{
							theApp.lpU8Func(simRun, theApp.lU8Param);
						}
						if (simRun == 12)
						{
							theApp.m_WatchSimCondition = 2;
						}
						if (num2 % 100 == 0)
						{
							Thread.Sleep(0);
						}
					}
					else
					{
						Monitor.Exit(theApp.m_lpMeteredSection);
						if (num > 100)
						{
							Thread.Sleep(100);
							num -= 100;
						}
						else
						{
							Thread.Sleep(num);
							num = 0;
						}
					}
				}
				else
				{
					int num3 = theApp.simu8.m_CheckInterruptStopHalt();
					Monitor.Exit(theApp.m_lpMeteredSection);
				}
				continue;
			}
			theApp.m_WatchSimCondition = 0;
			Monitor.Exit(theApp.m_lpMeteredSection);
			(new byte[1])[0] &= 252;
			break;
		}
	}

	private int ExitInstance()
	{
		return 0;
	}
}
