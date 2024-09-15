using System;
using System.Reflection;

namespace SIMBCD;

public class CSimPeripheralApp
{
	public string DllName_SimMem;

	public dynamic CSimMemApp;

	public const int REG_SIZE = 12;

	public const int REG_NUM = 4;

	public const int SFR_NUM = 55;

	public SFR_TBL[] m_SFR_table;

	public byte m_WaitReqFlag;

	public uint m_SFR_staddr;

	private int calc_mode;

	private int dst;

	private int src;

	private bool calc_en;

	private bool calc_en_d;

	private bool calc_en_dd;

	private bool calc_en_ddd;

	private bool wait_state;

	private bool divsn_mode;

	private bool div_mode;

	private bool mul_mode;

	private bool sft_mode;

	private uint calc_len;

	private int calc_pos;

	private uint BMC;

	private uint macro_state;

	private uint macro_cnt;

	private bool pend_BCDCMD;

	private bool pend_BCDMCR;

	private const int REG_OFFSET = 0;

	private const int REG_BCDCMD = 0;

	private const int REG_BCDCON = 2;

	private const int REG_BCDMCN = 4;

	private const int REG_BCDMCR = 5;

	private const int REG_BCDFLG = 16;

	private const int REG_BCDLLZ = 20;

	private const int REG_BCDMLZ = 21;

	public const int rDM = 12305;

	private const int CAL_NOP = 0;

	private const int CAL_ADD = 1;

	private const int CAL_SUB = 2;

	private const int CAL_SL = 8;

	private const int CAL_SR = 9;

	private const int CAL_CON = 10;

	private const int CAL_CP = 11;

	private const int CAL_SLX = 12;

	private const int CAL_SRX = 13;

	private const int NOCMD_VAL = 255;

	private const int MACRO_START = 255;

	private const int MACRO_END = 63;

	private const int STATE_MUL_INIT_0 = 24;

	private const int STATE_MUL_INIT_1 = 25;

	private const int STATE_MUL_INIT_2 = 26;

	private const int STATE_MUL_INIT_3 = 27;

	private const int STATE_MUL_INIT_4 = 28;

	private const int STATE_MUL_1 = 32;

	private const int STATE_MUL_2 = 33;

	private const int STATE_MUL_3 = 34;

	private const int STATE_MUL_4 = 35;

	private const int STATE_MUL_5 = 36;

	private const int STATE_MUL_6 = 37;

	private const int STATE_MUL_7 = 38;

	private const int STATE_MUL_8 = 39;

	private const int STATE_MUL_9 = 40;

	private const int STATE_MUL_10 = 41;

	private const int STATE_MUL_11 = 49;

	private const int STATE_MUL_12 = 50;

	private const int STATE_MUL_13 = 51;

	private const int STATE_MUL_14 = 52;

	private const int STATE_MUL_15 = 53;

	private const int STATE_MUL_16 = 54;

	private const int STATE_MUL_17 = 55;

	private const int STATE_MUL_18 = 56;

	private const int STATE_MUL_19 = 57;

	private const int STATE_DIVSN_INIT_0 = 32;

	private const int STATE_DIVSN_INIT_1 = 33;

	private const int STATE_DIVSN_INIT_2 = 34;

	private const int STATE_DIVSN_INIT_3 = 35;

	private const int STATE_DIVSN_INIT_4 = 36;

	private const int STATE_DIV_INIT_0 = 16;

	private const int STATE_DIV_INIT_1 = 17;

	private const int STATE_DIV_INIT_2 = 18;

	private const int STATE_DIV_INIT_3 = 19;

	private const int STATE_DIV_INIT_4 = 20;

	private const int STATE_DIV_0 = 24;

	private const int STATE_DIV_1 = 25;

	private const int STATE_DIV_2 = 26;

	private const int STATE_DIV_3 = 2;

	private const int STATE_DIV_4 = 1;

	private const int STATE_DIV_5 = 0;

	private const int STATE_DIV_6 = 27;

	private const int STATE_DIV_7 = 5;

	private const int STATE_DIV_8 = 4;

	private const int STATE_DIV_9 = 3;

	private const int STATE_DIV_10 = 9;

	private const int STATE_DIV_11 = 8;

	private const int STATE_DIV_12 = 7;

	private const int STATE_DIV_13 = 6;

	public CSimPeripheralApp()
	{
		m_SFR_table = new SFR_TBL[55];
		m_WaitReqFlag = 0;
		m_SFR_staddr = 0u;
		m_SFR_table[0].offset = 0u;
		m_SFR_table[0].bit_rd_attr = byte.MaxValue;
		m_SFR_table[0].bit_wr_attr = byte.MaxValue;
		m_SFR_table[0].value = byte.MaxValue;
		m_SFR_table[1].offset = 2u;
		m_SFR_table[1].bit_rd_attr = 15;
		m_SFR_table[1].bit_wr_attr = 15;
		m_SFR_table[1].value = 6;
		m_SFR_table[2].offset = 4u;
		m_SFR_table[2].bit_rd_attr = 63;
		m_SFR_table[2].bit_wr_attr = 63;
		m_SFR_table[2].value = 0;
		m_SFR_table[3].offset = 5u;
		m_SFR_table[3].bit_rd_attr = 128;
		m_SFR_table[3].bit_wr_attr = byte.MaxValue;
		m_SFR_table[3].value = 0;
		m_SFR_table[4].offset = 16u;
		m_SFR_table[4].bit_rd_attr = 192;
		m_SFR_table[4].bit_wr_attr = 192;
		m_SFR_table[4].value = 0;
		m_SFR_table[5].offset = 20u;
		m_SFR_table[5].bit_rd_attr = byte.MaxValue;
		m_SFR_table[5].bit_wr_attr = 0;
		m_SFR_table[5].value = 0;
		m_SFR_table[6].offset = 21u;
		m_SFR_table[6].bit_rd_attr = byte.MaxValue;
		m_SFR_table[6].bit_wr_attr = 0;
		m_SFR_table[6].value = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 12; j++)
			{
				m_SFR_table[7 + i * 12 + j].offset = BM.I2W(128 + i * 32 + j);
				m_SFR_table[7 + i * 12 + j].bit_rd_attr = byte.MaxValue;
				m_SFR_table[7 + i * 12 + j].bit_wr_attr = byte.MaxValue;
				m_SFR_table[7 + i * 12 + j].value = 0;
			}
		}
	}

	public bool InitInstance()
	{
		return true;
	}

	private uint RegAdr(int reg_num, int reg_pos)
	{
		return (uint)(m_SFR_staddr + 128 + reg_num * 32 + reg_pos);
	}

	private int RegPrev(int reg_num)
	{
		return (reg_num - 1 + 4) % 4;
	}

	private int RegNext(int reg_num)
	{
		return (reg_num + 1) % 4;
	}

	private uint SFRAdr(int reg_offset)
	{
		return (uint)(m_SFR_staddr + reg_offset);
	}

	private uint abcd44(bool m, uint a, uint b, uint ci)
	{
		ci = (m ? (ci ^ 1u) : ci) & 1u;
		uint num = 0u;
		for (int i = 0; i < 4; i++)
		{
			uint num2 = (a >> i * 4) & 0xF;
			uint num3 = (b >> i * 4) & 0xFu;
			if (m)
			{
				num3 = (9 - num3) & 0xFu;
			}
			uint num4 = num2 + num3 + ci;
			ci = ((num4 >= 10) ? 1u : 0u);
			num |= (uint)(((int)(num4 - ((ci != 0) ? 10 : 0)) & 0xF) << i * 4);
		}
		ci = (m ? (ci ^ 1u) : ci);
		return (ci << 16) + num;
	}

	private void calc_sl(bool ex)
	{
		byte b = 0;
		byte b2 = 0;
		byte b3 = 0;
		switch (src)
		{
		case 0:
		{
			for (int i = 0; i < 11; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, 11 - i), ref b2);
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, 10 - i), ref b3);
				b = (byte)((b2 << 4) | (b3 >> 4));
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 11 - i), b);
			}
			CSimMemApp.memApi_GetVal(12305, RegAdr(dst, 0), ref b2);
			if (ex)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(RegPrev(dst), 11), ref b3);
			}
			else
			{
				b3 = 0;
			}
			b = (byte)((b2 << 4) | (b3 >> 4));
			CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 0), b);
			break;
		}
		case 1:
		{
			for (int i = 0; i < 11; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, 10 - i), ref b);
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 11 - i), b);
			}
			if (ex)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(RegPrev(dst), 11), ref b);
			}
			else
			{
				b = 0;
			}
			CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 0), b);
			break;
		}
		case 2:
		{
			for (int i = 0; i < 10; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, 9 - i), ref b);
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 11 - i), b);
			}
			for (int i = 0; i < 2; i++)
			{
				if (ex)
				{
					CSimMemApp.memApi_GetVal(12305, RegAdr(RegPrev(dst), 10 + i), ref b);
				}
				else
				{
					b = 0;
				}
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			break;
		}
		case 3:
		{
			for (int i = 0; i < 8; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, 7 - i), ref b);
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 11 - i), b);
			}
			for (int i = 0; i < 4; i++)
			{
				if (ex)
				{
					CSimMemApp.memApi_GetVal(12305, RegAdr(RegPrev(dst), 8 + i), ref b);
				}
				else
				{
					b = 0;
				}
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			break;
		}
		}
	}

	private void calc_sr(bool ex)
	{
		byte b = 0;
		byte b2 = 0;
		byte b3 = 0;
		switch (src)
		{
		case 0:
		{
			for (int i = 0; i < 11; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, i), ref b2);
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, i + 1), ref b3);
				b = (byte)((b2 >> 4) | (b3 << 4));
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			CSimMemApp.memApi_GetVal(12305, RegAdr(dst, 11), ref b2);
			if (ex)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(RegNext(dst), 0), ref b3);
			}
			else
			{
				b3 = 0;
			}
			b = (byte)((b2 >> 4) | (b3 << 4));
			CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 11), b);
			break;
		}
		case 1:
		{
			for (int i = 0; i < 11; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, i + 1), ref b);
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			if (ex)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(RegNext(dst), 0), ref b);
			}
			else
			{
				b = 0;
			}
			CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 11), b);
			break;
		}
		case 2:
		{
			int i;
			for (i = 0; i < 10; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, i + 2), ref b);
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			for (; i < 12; i++)
			{
				if (ex)
				{
					CSimMemApp.memApi_GetVal(12305, RegAdr(RegNext(dst), i - 12 + 2), ref b);
				}
				else
				{
					b = 0;
				}
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			break;
		}
		case 3:
		{
			int i;
			for (i = 0; i < 8; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(dst, i + 4), ref b);
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			for (; i < 12; i++)
			{
				if (ex)
				{
					CSimMemApp.memApi_GetVal(12305, RegAdr(RegNext(dst), i - 12 + 4), ref b);
				}
				else
				{
					b = 0;
				}
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			break;
		}
		}
	}

	private void exec_calc()
	{
		exec_Add_Sub();
		exec_Sft_Con_Cp();
		update_LLZ_MLZ();
		check_calc_end();
	}

	private void exec_Add_Sub()
	{
		ushort a = 0;
		ushort b = 0;
		uint num = 0u;
		if (calc_en && calc_pos == 0)
		{
			uint num2 = 1u;
			uint num3 = 0u;
			bool flag = calc_mode == 1 || calc_mode == 2;
			for (int i = 0; i < calc_len; i += 2)
			{
				CSimMemApp.memApi_GetWordVal(12305, RegAdr(dst, i * 2), ref a);
				CSimMemApp.memApi_GetWordVal(12305, RegAdr(src, i * 2), ref b);
				num = abcd44(calc_mode == 2, a, b, num3);
				num3 = (num >> 16) & 1u;
				num2 = (((num & 0xFFFF) == 0 && num2 != 0) ? 1u : 0u);
				if (flag)
				{
					CSimMemApp.memApi_SetWordVal(12305, RegAdr(dst, i * 2), (ushort)num);
				}
				CSimMemApp.memApi_GetWordVal(12305, RegAdr(dst, i * 2 + 2), ref a);
				CSimMemApp.memApi_GetWordVal(12305, RegAdr(src, i * 2 + 2), ref b);
				num = abcd44(calc_mode == 2, a, b, num3);
				if (i + 1 != calc_len)
				{
					num3 = (num >> 16) & 1u;
				}
				if (i + 1 != calc_len)
				{
					num2 = (((num & 0xFFFF) == 0 && num2 != 0) ? 1u : 0u);
				}
				CSimMemApp.memApi_SetVal(12305, SFRAdr(16), (byte)((num3 << 7) | (num2 << 6)));
				if (flag)
				{
					if (i + 1 == calc_len)
					{
						num = 0u;
					}
					CSimMemApp.memApi_SetWordVal(12305, RegAdr(dst, i * 2 + 2), (ushort)num);
				}
			}
		}
		if ((calc_mode == 1 || calc_mode == 2) && (calc_en_d || calc_en_dd))
		{
			calc_pos += 2;
			if (calc_pos >= calc_len)
			{
				calc_en = false;
			}
		}
	}

	private void exec_Sft_Con_Cp()
	{
		byte b = 0;
		switch (calc_mode & (calc_en ? 15 : 0))
		{
		case 8:
			calc_sl(ex: false);
			break;
		case 9:
			calc_sr(ex: false);
			break;
		case 12:
			calc_sl(ex: true);
			break;
		case 13:
			calc_sr(ex: true);
			break;
		case 10:
		{
			for (int i = 1; i < 12; i++)
			{
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), 0);
			}
			CSimMemApp.memApi_SetVal(12305, RegAdr(dst, 0), (byte)((src == 3) ? 5u : ((uint)src)));
			break;
		}
		case 11:
		{
			for (int i = 0; i < 12; i++)
			{
				CSimMemApp.memApi_GetVal(12305, RegAdr(src, i), ref b);
				CSimMemApp.memApi_SetVal(12305, RegAdr(dst, i), b);
			}
			break;
		}
		}
	}

	private void update_LLZ_MLZ()
	{
		byte b = 0;
		CSimMemApp.memApi_GetVal(12305, SFRAdr(0), ref b);
		if ((!calc_en_dd || calc_en_d) && (b & 0xF0u) != 0)
		{
			return;
		}
		int num = 11;
		byte b2 = 0;
		while (num >= 0)
		{
			CSimMemApp.memApi_GetVal(12305, RegAdr(dst, num), ref b);
			if (num < calc_len * 2 && (b & 0xF0u) != 0)
			{
				break;
			}
			b2++;
			if (num < calc_len * 2 && (b & 0xFu) != 0)
			{
				break;
			}
			b2++;
			num--;
		}
		num = 0;
		byte b3 = 0;
		for (; num < 12; num++)
		{
			CSimMemApp.memApi_GetVal(12305, RegAdr(dst, num), ref b);
			if (num < calc_len * 2 && (b & 0xFu) != 0)
			{
				break;
			}
			b3++;
			if (num < calc_len * 2 && (b & 0xF0u) != 0)
			{
				break;
			}
			b3++;
		}
		CSimMemApp.memApi_SetVal(12305, SFRAdr(20), b3);
		CSimMemApp.memApi_SetVal(12305, SFRAdr(21), b2);
	}

	private void check_calc_end()
	{
		if (((uint)calc_mode & 8u) != 0 || calc_mode == 0)
		{
			calc_en = false;
			calc_pos = 6;
		}
	}

	private void check_BCD_Register()
	{
		wait_state = calc_en || (mul_mode | div_mode | divsn_mode | sft_mode);
		check_BCDCMD();
		write_BCDCON();
		write_BCDMCN();
		check_BCDMCR();
	}

	private void check_BCDCMD()
	{
		byte b = 0;
		CSimMemApp.memApi_GetVal(12305, SFRAdr(0), ref b);
		if (b == byte.MaxValue)
		{
			return;
		}
		if (wait_state)
		{
			pend_BCDCMD = true;
			return;
		}
		pend_BCDCMD = false;
		calc_mode = (b >> 4) & 0xF;
		src = (b >> 2) & 3;
		dst = b & 3;
		calc_pos = 0;
		if (calc_mode != 0)
		{
			calc_en = true;
		}
		else
		{
			calc_en = false;
			calc_en_d = true;
		}
		CSimMemApp.memApi_SetVal(12305, SFRAdr(0), 255);
	}

	private void write_BCDCON()
	{
		byte b = 0;
		CSimMemApp.memApi_GetVal(12305, SFRAdr(2), ref b);
		calc_len = b & 0xFu;
		calc_len = ((calc_len > 6) ? 6u : ((calc_len == 0) ? 1u : calc_len));
		CSimMemApp.memApi_SetVal(12305, SFRAdr(2), (byte)calc_len);
	}

	private void write_BCDMCN()
	{
		byte b = 0;
		CSimMemApp.memApi_GetVal(12305, SFRAdr(4), ref b);
		CSimMemApp.memApi_SetVal(12305, SFRAdr(4), (byte)(b & 0x1Fu));
	}

	private void check_BCDMCR()
	{
		byte b = 0;
		CSimMemApp.memApi_GetVal(12305, SFRAdr(5), ref b);
		if ((b & 0x7Fu) != 0)
		{
			if (wait_state)
			{
				pend_BCDMCR = true;
				return;
			}
			pend_BCDMCR = false;
			BMC = b;
			CSimMemApp.memApi_GetVal(12305, SFRAdr(4), ref b);
			macro_cnt = b;
			CSimMemApp.memApi_SetVal(12305, SFRAdr(5), (byte)0);
			macro_state = 255u;
		}
	}

	private void state_manage()
	{
		byte b = 0;
		if (macro_state == 255 && !calc_en)
		{
			state_manage_init();
		}
		else if ((mul_mode | div_mode | divsn_mode | sft_mode) && !calc_en)
		{
			if (mul_mode)
			{
				state_manage_mul();
			}
			else if (div_mode | divsn_mode)
			{
				state_manage_div_divsn();
			}
			else if (sft_mode)
			{
				state_manage_sft();
			}
			if (mul_mode || div_mode || divsn_mode)
			{
				calc_en = (calc_mode | src | dst) != 0;
			}
			calc_pos = 0;
		}
		CSimMemApp.memApi_GetVal(12305, SFRAdr(5), ref b);
		int num = b & 0x7F;
		int num2 = ((mul_mode | div_mode | divsn_mode | sft_mode) ? 128 : 0);
		CSimMemApp.memApi_SetVal(12305, SFRAdr(5), (byte)(num | num2));
		calc_en_ddd = calc_en_dd;
		calc_en_dd = calc_en_d;
		calc_en_d = calc_en;
		m_WaitReqFlag = (byte)((pend_BCDCMD | pend_BCDMCR) ? 1u : 0u);
	}

	private void state_set(int s_mode, int s_src, int s_dst, uint s_state)
	{
		calc_mode = s_mode;
		src = s_src;
		dst = s_dst;
		macro_state = s_state;
	}

	private void state_manage_init()
	{
		byte b = 0;
		if (mul_mode)
		{
			CSimMemApp.memApi_GetVal(12305, RegAdr(0, 0), ref b);
			state_set(13, 0, 0, (uint)(32 + (b & 0xF)));
		}
		else if (div_mode)
		{
			state_set(8, 0, 1, 24u);
		}
		else if (divsn_mode)
		{
			state_set(12, 0, 1, 24u);
		}
		else
		{
			switch ((BMC >> 1) & 0xF)
			{
			case 1u:
				if ((BMC & (true ? 1u : 0u)) != 0)
				{
					state_set(13, 0, 0, (uint)(32 + (b & 0xF)));
				}
				else
				{
					state_set(11, 1, 3, 24u);
				}
				mul_mode = true;
				break;
			case 2u:
				if ((BMC & (true ? 1u : 0u)) != 0)
				{
					state_set(8, 0, 1, 24u);
				}
				else
				{
					state_set(11, 1, 3, 16u);
				}
				div_mode = true;
				break;
			case 3u:
				if ((BMC & (true ? 1u : 0u)) != 0)
				{
					state_set(12, 0, 1, 24u);
				}
				else
				{
					state_set(11, 1, 3, 32u);
				}
				divsn_mode = true;
				break;
			case 4u:
			case 5u:
			case 6u:
			case 7u:
				macro_cnt++;
				state_set(((BMC & 0xC) == 8) ? 8 : 9, (macro_cnt >= 8) ? 3 : ((macro_cnt >= 4) ? 2 : ((macro_cnt >= 2) ? 1 : 0)), (int)(BMC & 3), 0u);
				macro_cnt -= (uint)(1 << src);
				sft_mode = macro_cnt != 0;
				break;
			default:
				state_set(0, 0, 0, macro_state);
				break;
			}
		}
		calc_pos = 0;
		calc_en = true;
		BMC = 0u;
	}

	private void state_manage_mul()
	{
		byte b = 0;
		switch (macro_state)
		{
		case 24u:
			state_set(11, 1, 2, 25u);
			break;
		case 25u:
			state_set(1, 2, 2, 26u);
			break;
		case 26u:
			state_set(1, 2, 2, 27u);
			break;
		case 27u:
			state_set(10, 0, 1, 28u);
			break;
		case 28u:
			CSimMemApp.memApi_GetVal(12305, RegAdr(0, 0), ref b);
			state_set(13, 0, 0, (uint)(32 + (b & 0xF)));
			break;
		case 32u:
			state_set(9, 0, 1, 63u);
			break;
		case 33u:
		case 34u:
		case 35u:
		case 36u:
		case 37u:
		case 38u:
		case 39u:
		case 40u:
		case 41u:
			state_set(9, 0, 1, macro_state + 16);
			break;
		case 49u:
			state_set(1, 3, 1, 63u);
			break;
		case 50u:
			state_set(1, 3, 1, 49u);
			break;
		case 51u:
			state_set(2, 3, 1, 52u);
			break;
		case 52u:
			state_set(1, 2, 1, 63u);
			break;
		case 53u:
			state_set(1, 2, 1, 49u);
			break;
		case 54u:
			state_set(1, 2, 1, 50u);
			break;
		case 55u:
			state_set(1, 2, 1, 51u);
			break;
		case 56u:
			state_set(1, 2, 1, 52u);
			break;
		case 57u:
			state_set(1, 2, 1, 53u);
			break;
		default:
			state_set(0, 0, 0, 63u);
			mul_mode = macro_cnt != 0;
			break;
		}
		if (macro_state == 63 && macro_cnt != 0)
		{
			macro_cnt--;
			macro_state = 255u;
		}
	}

	private void state_manage_div_divsn()
	{
		byte b = 0;
		CSimMemApp.memApi_GetVal(12305, SFRAdr(16), ref b);
		uint num = (((b & 0x80u) != 0) ? 1u : 0u);
		uint num2 = macro_state;
		switch (macro_state)
		{
		case 32u:
			state_set(11, 1, 2, 33u);
			break;
		case 33u:
			state_set(1, 1, 2, 34u);
			break;
		case 34u:
			state_set(1, 1, 2, 35u);
			break;
		case 35u:
			state_set(12, 3, 1, 36u);
			break;
		case 36u:
			state_set(8, 3, 0, 25u);
			break;
		case 16u:
			state_set(11, 1, 2, 17u);
			break;
		case 17u:
			state_set(1, 1, 2, 18u);
			break;
		case 18u:
			state_set(1, 1, 2, 19u);
			break;
		case 19u:
			state_set(11, 0, 1, 20u);
			break;
		case 20u:
			state_set(10, 0, 0, 24u);
			break;
		case 24u:
			state_set(8, 0, 0, 25u);
			break;
		case 25u:
			state_set(2, 2, 1, 26u);
			break;
		case 26u:
			if (num != 0)
			{
				state_set(1, 3, 1, 2u);
			}
			else
			{
				state_set(2, 2, 1, 27u);
			}
			break;
		case 2u:
			if (num != 0)
			{
				state_set(0, 0, 0, 63u);
			}
			else
			{
				state_set(1, 3, 1, 1u);
			}
			break;
		case 1u:
			if (num != 0)
			{
				state_set(0, 0, 0, 63u);
			}
			else
			{
				state_set(1, 3, 1, 0u);
			}
			break;
		case 0u:
			state_set(0, 0, 0, 63u);
			break;
		case 27u:
			if (num != 0)
			{
				state_set(1, 3, 1, 5u);
			}
			else
			{
				state_set(2, 2, 1, 9u);
			}
			break;
		case 5u:
			if (num != 0)
			{
				state_set(0, 0, 0, 63u);
			}
			else
			{
				state_set(1, 3, 1, 4u);
			}
			break;
		case 4u:
			if (num != 0)
			{
				state_set(0, 0, 0, 63u);
			}
			else
			{
				state_set(1, 3, 1, 3u);
			}
			break;
		case 3u:
			state_set(0, 0, 0, 63u);
			break;
		case 9u:
			if (num != 0)
			{
				state_set(1, 3, 1, 8u);
			}
			else
			{
				state_set(0, 0, 0, 63u);
			}
			break;
		case 8u:
			if (num != 0)
			{
				state_set(0, 0, 0, 63u);
			}
			else
			{
				state_set(1, 3, 1, 7u);
			}
			break;
		case 7u:
			if (num != 0)
			{
				state_set(0, 0, 0, 63u);
			}
			else
			{
				state_set(1, 3, 1, 6u);
			}
			break;
		case 6u:
			state_set(0, 0, 0, 63u);
			break;
		}
		if (macro_state != 63)
		{
			return;
		}
		CSimMemApp.memApi_GetVal(12305, RegAdr(0, 0), ref b);
		b = (byte)((b & 0xF0u) | (num2 & 0xFu));
		CSimMemApp.memApi_SetVal(12305, RegAdr(0, 0), b);
		if (macro_cnt != 0)
		{
			macro_cnt--;
			if (div_mode)
			{
				state_set(8, 0, 1, 24u);
			}
			else if (divsn_mode)
			{
				state_set(12, 0, 1, 24u);
			}
		}
		else
		{
			div_mode = false;
			divsn_mode = false;
		}
	}

	private void state_manage_sft()
	{
		src = ((macro_cnt >= 8) ? 3 : ((macro_cnt >= 4) ? 2 : ((macro_cnt >= 2) ? 1 : 0)));
		macro_cnt -= (uint)(1 << src);
		if (macro_cnt == 0)
		{
			sft_mode = false;
		}
		calc_en = true;
		macro_state = 0u;
	}

	public int perApi_Run()
	{
		exec_calc();
		check_BCD_Register();
		state_manage();
		return 0;
	}

	public int perApi_SetSFRStartAdr(uint st_addr)
	{
		int result = 0;
		m_SFR_staddr = st_addr;
		return result;
	}

	public int perApi_SetIntInfo(int cnt, dynamic int_table)
	{
		return 0;
	}

	public int perApi_GetWaitReq(ref byte val)
	{
		val = m_WaitReqFlag;
		return 0;
	}

	public int perApi_Reset()
	{
		int result = 0;
		Random random = new Random();
		m_SFR_table[0].value = byte.MaxValue;
		m_SFR_table[1].value = 6;
		m_SFR_table[2].value = 0;
		m_SFR_table[3].value = 0;
		m_SFR_table[4].value = 0;
		m_SFR_table[5].value = 0;
		m_SFR_table[6].value = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 12; j++)
			{
				m_SFR_table[7 + i * 12 + j].value = BM.I2B(random.Next(0, 256));
			}
		}
		for (int i = 0; i < 55; i++)
		{
			CSimMemApp.memApi_SetVal(12305, m_SFR_staddr + m_SFR_table[i].offset, m_SFR_table[i].value);
		}
		calc_len = 6u;
		calc_pos = 0;
		return result;
	}

	public int perApi_SetMemDllName(string dll_name)
	{
		int result = 0;
		DllName_SimMem = dll_name;
		CSimMemApp = Activator.CreateInstance(Assembly.LoadFrom(DllName_SimMem).GetType("SimMem.CSimMemApp"));
		return result;
	}
}
