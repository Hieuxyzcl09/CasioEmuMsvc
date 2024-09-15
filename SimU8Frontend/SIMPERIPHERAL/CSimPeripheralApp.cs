using System;
using System.Reflection;

namespace SIMPERIPHERAL;

public class CSimPeripheralApp
{
	public const int SFR_NUM = 4;

	public SFR_TBL[] m_SFR_table;

	public byte m_WaitReqFlag;

	public uint m_SFR_staddr;

	public INTNUM m_IntNum;

	public tPeriIntInfo[] m_INT_INFO;

	public byte m_IntInfo_cnt;

	public byte tm0_ovf;

	public byte equ_flag;

	public byte tm01_int0;

	public string DllName_SimMem;

	public dynamic CSimMemApp;

	public const ushort SFR_TM0D = 0;

	public const ushort SFR_TM0C = 1;

	public const ushort SFR_TM0CON0 = 2;

	public const ushort SFR_TM0CON1 = 3;

	public const ushort B_T0CS0 = 1;

	public const ushort B_T0CS1 = 2;

	public const ushort B_T01M16 = 4;

	public const ushort B_T0RUN = 1;

	public const ushort B_T0STAT = 128;

	public const int rDM = 12305;

	public CSimPeripheralApp()
	{
		m_SFR_table = new SFR_TBL[4];
		m_INT_INFO = new tPeriIntInfo[128];
		tPeriIntInfo[] iNT_INFO = m_INT_INFO;
		for (int i = 0; i < iNT_INFO.Length; i++)
		{
			((tPeriIntInfo)iNT_INFO[i]).InitIntSym();
		}
		m_WaitReqFlag = 0;
		m_SFR_staddr = 0u;
		m_IntNum.int_req1 = 0u;
		m_IntNum.int_req2 = 0u;
		m_IntNum.int_req3 = 0u;
		m_IntNum.int_req4 = 0u;
		for (int j = 0; j < 128; j++)
		{
			m_INT_INFO[j].Irq_Addr = 0u;
			m_INT_INFO[j].Irq_Bit = 0;
			m_INT_INFO[j].InitIntSym();
			for (int k = 0; k < 32; k++)
			{
				m_INT_INFO[j].IntSym[k] = 0;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			switch (j)
			{
			case 0:
				m_SFR_table[j].offset = 0u;
				m_SFR_table[j].bit_rd_attr = byte.MaxValue;
				m_SFR_table[j].bit_wr_attr = byte.MaxValue;
				m_SFR_table[j].value = byte.MaxValue;
				break;
			case 1:
				m_SFR_table[j].offset = 1u;
				m_SFR_table[j].bit_rd_attr = byte.MaxValue;
				m_SFR_table[j].bit_wr_attr = byte.MaxValue;
				m_SFR_table[j].value = 0;
				break;
			case 2:
				m_SFR_table[j].offset = 2u;
				m_SFR_table[j].bit_rd_attr = byte.MaxValue;
				m_SFR_table[j].bit_wr_attr = byte.MaxValue;
				m_SFR_table[j].value = 0;
				break;
			case 3:
				m_SFR_table[j].offset = 3u;
				m_SFR_table[j].bit_rd_attr = byte.MaxValue;
				m_SFR_table[j].bit_wr_attr = byte.MaxValue;
				m_SFR_table[j].value = 0;
				break;
			}
		}
	}

	public bool InitInstance()
	{
		return true;
	}

	public int perApi_Run()
	{
		byte b = 0;
		byte b2 = 0;
		int num = 0;
		if (num == 0)
		{
			uint num2 = 2 + m_SFR_staddr;
			CSimMemApp.memApi_GetVal(12305, num2, ref b);
			if (!BM.I2bool(b & 2) || BM.I2bool(b & 1))
			{
				num2 = 3 + m_SFR_staddr;
				CSimMemApp.memApi_GetVal(12305, num2, ref b);
				if (BM.I2bool(b & 1))
				{
					num2 = 1 + m_SFR_staddr;
					CSimMemApp.memApi_GetVal(12305, num2, ref b);
					b++;
					CSimMemApp.memApi_SetVal(12305, num2, b);
					uint sFR_staddr = m_SFR_staddr;
					CSimMemApp.memApi_GetVal(12305, sFR_staddr, ref b2);
					if (b == b2)
					{
						string text = "TM0INT";
						byte b3 = 0;
						uint num3 = 0u;
						ushort num4 = 0;
						for (int i = 0; i < m_IntInfo_cnt; i++)
						{
							if (BM.BA2S(m_INT_INFO[i].IntSym) == text)
							{
								num3 = m_INT_INFO[i].Irq_Addr;
								num4 = m_INT_INFO[i].Irq_Bit;
								break;
							}
						}
						CSimMemApp.memApi_GetVal(12305, num3, ref b3);
						b3 = BM.I2B(b3 | (1 << (int)num4));
						CSimMemApp.memApi_SetVal(12305, num3, b3);
					}
					num2 = 3 + m_SFR_staddr;
					CSimMemApp.memApi_GetVal(12305, num2, ref b);
					b = BM.I2B(b | 0x80);
					CSimMemApp.memApi_SetVal(12305, num2, b);
				}
				else
				{
					num2 = 3 + m_SFR_staddr;
					CSimMemApp.memApi_GetVal(12305, num2, ref b);
					b = BM.I2B(b & -129);
					CSimMemApp.memApi_SetVal(12305, num2, b);
				}
			}
		}
		return num;
	}

	public int perApi_GetSFRInfo(byte cnt, dynamic sfr_addr)
	{
		int result = 0;
		for (int i = 0; i < cnt; i++)
		{
			sfr_addr[i] = m_SFR_table[i].offset;
		}
		return result;
	}

	public int perApi_SetSFRStartAdr(uint st_addr)
	{
		int result = 0;
		m_SFR_staddr = st_addr;
		return result;
	}

	public int perApi_SetSFR(uint addr, byte val)
	{
		int num = 0;
		int result = 0;
		for (int i = 0; i < 4; i++)
		{
			if (m_SFR_table[i].offset + m_SFR_staddr == addr)
			{
				m_SFR_table[i].value = val;
				CSimMemApp.memApi_SetVal(12305, addr, val);
				num = 1;
			}
		}
		if (num == 0)
		{
			result = -1;
		}
		return result;
	}

	public int perApi_GetSFR(uint addr, ref byte val)
	{
		int num = 0;
		int result = 0;
		for (int i = 0; i < 4; i++)
		{
			if (m_SFR_table[i].offset + m_SFR_staddr == addr)
			{
				byte value = m_SFR_table[i].value;
				CSimMemApp.memApi_GetVal(12305, addr, ref value);
				m_SFR_table[i].value = value;
				val = m_SFR_table[i].value;
				num = 1;
			}
		}
		if (num == 0)
		{
			result = -1;
		}
		return result;
	}

	public int perApi_GetIntReq(ref dynamic val)
	{
		val = m_IntNum;
		return 0;
	}

	public int perApi_SetIntInfo(int cnt, dynamic int_table)
	{
		int num = 0;
		if (num == 0)
		{
			for (int i = 0; i < cnt; i++)
			{
				Array.Copy(int_table[i].IntSym, m_INT_INFO[i].IntSym, m_INT_INFO[i].IntSym.Length);
				m_INT_INFO[i].Irq_Addr = int_table[i].Irq_Addr;
				m_INT_INFO[i].Irq_Bit = int_table[i].Irq_Bit;
			}
		}
		m_IntInfo_cnt = (byte)cnt;
		return num;
	}

	public int perApi_GetWaitReq(ref byte val)
	{
		val = m_WaitReqFlag;
		return 0;
	}

	public int perApi_Reset()
	{
		byte b = 0;
		int num = 0;
		uint num2 = 0u;
		if (num == 0)
		{
			for (int i = 0; i < 4; i++)
			{
				if (m_SFR_table[i].offset == 3)
				{
					num2 = 3 + m_SFR_staddr;
					byte value = m_SFR_table[i].value;
					CSimMemApp.memApi_GetVal(12305, num2, ref value);
					m_SFR_table[i].value = value;
					b = BM.I2B(m_SFR_table[i].value & -129);
					m_SFR_table[i].value = b;
					CSimMemApp.memApi_SetVal(12305, num2, b);
				}
				if (m_SFR_table[i].offset == 1)
				{
					num2 = 1 + m_SFR_staddr;
					m_SFR_table[i].value = 0;
					CSimMemApp.memApi_SetVal(12305, num2, 0);
				}
			}
		}
		num2 = 0u;
		ushort num3 = 0;
		bool flag = false;
		string text = "TM0INT";
		for (int i = 0; i < m_IntInfo_cnt; i++)
		{
			if (BM.BA2S(m_INT_INFO[i].IntSym) == text)
			{
				num2 = m_INT_INFO[i].Irq_Addr;
				num3 = m_INT_INFO[i].Irq_Bit;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			num = -1;
		}
		if (num == 0)
		{
			CSimMemApp.memApi_GetVal(12305, num2, ref b);
			b = BM.I2B(b & ~(1 << (int)num3));
			CSimMemApp.memApi_SetVal(12305, num2, b);
		}
		return num;
	}

	public int perApi_SetMemDllName(string dll_name)
	{
		int result = 0;
		DllName_SimMem = dll_name;
		CSimMemApp = Activator.CreateInstance(Assembly.LoadFrom(DllName_SimMem).GetType("SimMem.CSimMemApp"));
		return result;
	}
}
