namespace SimMem;

public class CSimMemApp
{
	public U8Memory m_CMemory;

	public U8Memory m_DMemory;

	public U8Memory m_GMemory;

	public Mapping m_Cmapping;

	public Mapping m_Dmapping;

	public Mapping m_Gmapping;

	public byte[] m_lpMem_Cmem;

	public byte[] m_lpMem_Dmem;

	public byte[] m_lpMem_Gmem;

	public byte[] m_lpMem_SFR_flg;

	public const int rCM = 12289;

	public const int rDM = 12305;

	public const int rGM = 12321;

	public const int W_FLG = 1;

	public const int R_FLG = 2;

	public static CSimMemApp theApp = new CSimMemApp();

	public CSimMemApp()
	{
		m_CMemory = new U8Memory();
		m_DMemory = new U8Memory();
		m_GMemory = new U8Memory();
		m_Cmapping = new Mapping();
		m_Dmapping = new Mapping();
		m_Gmapping = new Mapping();
		m_lpMem_Cmem = new byte[65536];
		m_CMemory.m_MemBuf = m_lpMem_Cmem;
		m_lpMem_Dmem = new byte[65536];
		m_DMemory.m_MemBuf = m_lpMem_Dmem;
		m_lpMem_Gmem = new byte[16711680];
		m_GMemory.m_MemBuf = m_lpMem_Gmem;
		m_lpMem_SFR_flg = new byte[4096];
	}

	public int memApi_SetVal(ushort rID, uint nIndex, byte val)
	{
		int num = 0;
		switch (rID)
		{
		case 12289:
			num = theApp.m_CMemory.SetVal(nIndex, val);
			break;
		case 12305:
			num = theApp.m_DMemory.SetVal(nIndex, val);
			if (nIndex >= 61440 && nIndex < 65536)
			{
				theApp.m_lpMem_SFR_flg[nIndex - 61440] |= 1;
			}
			break;
		case 12321:
			num = theApp.m_GMemory.SetVal(nIndex, val);
			break;
		default:
			num = -1;
			break;
		}
		return num;
	}

	public int memApi_GetVal(ushort rID, uint nIndex, ref byte val)
	{
		int num = 0;
		switch (rID)
		{
		case 12289:
			num = theApp.m_CMemory.GetVal(nIndex, ref val);
			break;
		case 12305:
			num = theApp.m_DMemory.GetVal(nIndex, ref val);
			if (nIndex >= 61440 && nIndex < 65536)
			{
				theApp.m_lpMem_SFR_flg[nIndex - 61440] |= 2;
			}
			break;
		case 12321:
			num = theApp.m_GMemory.GetVal(nIndex, ref val);
			break;
		default:
			num = -1;
			break;
		}
		return num;
	}

	public int memApi_SetWordVal(ushort rID, uint nIndex, ushort val)
	{
		int num = 0;
		switch (rID)
		{
		case 12289:
			num = theApp.m_CMemory.SetWordVal(nIndex, val);
			break;
		case 12305:
			num = theApp.m_DMemory.SetWordVal(nIndex, val);
			if (nIndex >= 61440 && nIndex < 65536 && num == 0)
			{
				theApp.m_lpMem_SFR_flg[nIndex - 61440] |= 1;
				theApp.m_lpMem_SFR_flg[nIndex - 61440 + 1] |= 1;
			}
			break;
		case 12321:
			num = theApp.m_GMemory.SetWordVal(nIndex, val);
			break;
		default:
			num = -1;
			break;
		}
		return num;
	}

	public int memApi_GetWordVal(ushort rID, uint nIndex, ref ushort val)
	{
		int num = 0;
		switch (rID)
		{
		case 12289:
			num = theApp.m_CMemory.GetWordVal(nIndex, ref val);
			break;
		case 12305:
			num = theApp.m_DMemory.GetWordVal(nIndex, ref val);
			if (nIndex >= 61440 && nIndex < 65536 && num == 0)
			{
				theApp.m_lpMem_SFR_flg[nIndex - 61440] |= 2;
				theApp.m_lpMem_SFR_flg[nIndex - 61440 + 1] |= 2;
			}
			break;
		case 12321:
			num = theApp.m_GMemory.GetWordVal(nIndex, ref val);
			break;
		default:
			num = -1;
			break;
		}
		return num;
	}

	public int memApi_SetRange(ushort rID, uint sadr, uint eadr)
	{
		int num = 0;
		return rID switch
		{
			12289 => theApp.m_CMemory.SetRange(sadr, eadr), 
			12305 => theApp.m_DMemory.SetRange(sadr, eadr), 
			12321 => theApp.m_GMemory.SetRange(sadr, eadr), 
			_ => -1, 
		};
	}

	public int memApi_GetRange(uint rID, ref uint sadr, ref uint eadr)
	{
		int result = 0;
		switch (rID)
		{
		case 12289u:
			theApp.m_CMemory.GetRange(ref sadr, ref eadr);
			break;
		case 12305u:
			theApp.m_DMemory.GetRange(ref sadr, ref eadr);
			break;
		case 12321u:
			theApp.m_GMemory.GetRange(ref sadr, ref eadr);
			break;
		default:
			result = -1;
			break;
		}
		return result;
	}

	public int memApi_SetCount(ushort rID, byte n)
	{
		int result = 0;
		switch (rID)
		{
		case 12289:
			theApp.m_Cmapping.SetCount(n);
			break;
		case 12305:
			theApp.m_Dmapping.SetCount(n);
			break;
		case 12321:
			theApp.m_Gmapping.SetCount(n);
			break;
		default:
			result = -1;
			break;
		}
		return result;
	}

	public int memApi_GetCount(ushort rID, ref byte n)
	{
		int result = 0;
		byte b = 0;
		switch (rID)
		{
		case 12289:
			b = theApp.m_Cmapping.GetCount();
			break;
		case 12305:
			b = theApp.m_Dmapping.GetCount();
			break;
		case 12321:
			b = theApp.m_Gmapping.GetCount();
			break;
		default:
			result = -1;
			break;
		}
		n = b;
		return result;
	}

	public int memApi_SetStartAddress(ushort rID, byte n, uint val)
	{
		int num = 0;
		return rID switch
		{
			12289 => theApp.m_Cmapping.SetStartAddress(n, val), 
			12305 => theApp.m_Dmapping.SetStartAddress(n, val), 
			12321 => theApp.m_Gmapping.SetStartAddress(n, val), 
			_ => -1, 
		};
	}

	public int memApi_GetStartAddress(ushort rID, byte n, ref uint val)
	{
		int num = 0;
		return rID switch
		{
			12289 => theApp.m_Cmapping.GetStartAddress(n, ref val), 
			12305 => theApp.m_Dmapping.GetStartAddress(n, ref val), 
			12321 => theApp.m_Gmapping.GetStartAddress(n, ref val), 
			_ => -1, 
		};
	}

	public int memApi_GetStartAddress(ushort rID, byte n, uint[] data, int index)
	{
		uint val = 0u;
		int num = memApi_GetStartAddress(rID, n, ref val);
		if (num == 0)
		{
			data[index] = val;
		}
		return num;
	}

	public int memApi_SetEndAddress(ushort rID, byte n, uint val)
	{
		int num = 0;
		return rID switch
		{
			12289 => theApp.m_Cmapping.SetEndAddress(n, val), 
			12305 => theApp.m_Dmapping.SetEndAddress(n, val), 
			12321 => theApp.m_Gmapping.SetEndAddress(n, val), 
			_ => -1, 
		};
	}

	public int memApi_GetEndAddress(ushort rID, byte n, ref uint val)
	{
		int num = 0;
		return rID switch
		{
			12289 => theApp.m_Cmapping.GetEndAddress(n, ref val), 
			12305 => theApp.m_Dmapping.GetEndAddress(n, ref val), 
			12321 => theApp.m_Gmapping.GetEndAddress(n, ref val), 
			_ => -1, 
		};
	}

	public int memApi_GetEndAddress(ushort rID, byte n, uint[] data, int index)
	{
		uint val = 0u;
		int num = memApi_GetEndAddress(rID, n, ref val);
		if (num == 0)
		{
			data[index] = val;
		}
		return num;
	}

	public int memApi_SetAttribute(ushort rID, byte n, byte val)
	{
		int num = 0;
		return rID switch
		{
			12289 => theApp.m_Cmapping.SetAttribute(n, val), 
			12305 => theApp.m_Dmapping.SetAttribute(n, val), 
			12321 => theApp.m_Gmapping.SetAttribute(n, val), 
			_ => -1, 
		};
	}

	public int memApi_GetAttribute(ushort rID, byte n, ref byte val)
	{
		int num = 0;
		return rID switch
		{
			12289 => theApp.m_Cmapping.GetAttribute(n, ref val), 
			12305 => theApp.m_Dmapping.GetAttribute(n, ref val), 
			12321 => theApp.m_Gmapping.GetAttribute(n, ref val), 
			_ => -1, 
		};
	}

	public int memApi_GetAttribute(ushort rID, byte n, byte[] data, int index)
	{
		byte val = 0;
		int num = memApi_GetAttribute(rID, n, ref val);
		if (num == 0)
		{
			data[index] = val;
		}
		return num;
	}

	public static int memApi_GetSFRFlg(uint index)
	{
		if (index >= 61440 && index < 65536)
		{
			return theApp.m_lpMem_SFR_flg[index - 61440];
		}
		return 0;
	}

	public static int memApi_ClearSFRFlg(uint index)
	{
		if (index >= 61440 && index < 65536)
		{
			theApp.m_lpMem_SFR_flg[index - 61440] = 0;
		}
		return 0;
	}
}
