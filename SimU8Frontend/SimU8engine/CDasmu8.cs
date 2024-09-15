namespace SimU8engine;

public class CDasmu8
{
	public const int OPERAND_MAX = 3;

	public const int MAXINSTLEN = 9;

	public const int U8_I_IMM8 = 100;

	public const int U8_I_RNRM = 101;

	public const int U8_I_ALU = 102;

	public const int U8_I_EXTBW = 103;

	public const int U8_I_RN = 104;

	public const int U8_I_ERN = 105;

	public const int U8_I_XRN = 106;

	public const int U8_I_QRN = 107;

	public const int U8_I_RN_D16 = 108;

	public const int U8_I_SHIFT = 109;

	public const int U8_I_BIT = 110;

	public const int U8_I_CTRREG = 111;

	public const int U8_I_ERN_D16 = 112;

	public const int U8_I_SP = 113;

	public const int U8_I_CTRREG2 = 114;

	public const int U8_I_ERN_D6 = 115;

	public const int U8_I_CJUMP = 116;

	public const int U8_I_RN_D6 = 117;

	public const int U8_I_I7 = 118;

	public const int U8_I_SIGNED8 = 119;

	public const int U8_I_UNSIGNED8 = 120;

	public const int U8_I_PSW = 121;

	public const int U8_I_SWI = 122;

	public const int U8_I_JUMP = 123;

	public const int U8_I_MULDIV = 124;

	public const int U8_I_ERNERM = 125;

	public const int U8_I_LEA = 126;

	public const int U8_I_COPRO = 127;

	public const int U8_I_PUSHPOP = 128;

	public const int U8_I_RT_INC = 129;

	public const int U8_I_DSRRD = 130;

	public const int U8_I_DSRPEG = 131;

	public const int U8_I_DSRDSR = 132;

	public const int U8_I_ICESWI = 133;

	public const int U8_I_BRK = 134;

	private readonly ushort[] m_dasmCode;

	private ushort m_dasmNum;

	private int m_ins_type;

	private DasmU8ErrCodeNumber m_errCode;

	private byte m_op;

	private byte m_op2;

	private byte m_op3;

	private byte m_n;

	private byte m_m;

	private byte m_w;

	private byte m_b;

	private byte m_imm8;

	private byte m_imm7;

	private byte m_signed8;

	private byte m_unsigned8;

	private byte m_snum;

	private byte m_g;

	private byte m_lepa;

	private byte m_disp6;

	private byte m_radr;

	private ushort m_dadr;

	private ushort m_disp16;

	private ushort m_dbitadr;

	private ushort m_cadr;

	private DasmU8ErrCodeNumber GetErrCode()
	{
		return m_errCode;
	}

	public int GetInsType()
	{
		return m_ins_type;
	}

	private byte Get_op()
	{
		return m_op;
	}

	private byte Get_op2()
	{
		return m_op2;
	}

	private byte Get_op3()
	{
		return m_op3;
	}

	private byte Get_n()
	{
		return m_n;
	}

	private byte Get_m()
	{
		return m_m;
	}

	private byte Get_w()
	{
		return m_w;
	}

	private byte Get_b()
	{
		return m_b;
	}

	private byte Get_imm8()
	{
		return m_imm8;
	}

	private byte Get_imm7()
	{
		return m_imm7;
	}

	private byte Get_signed8()
	{
		return m_signed8;
	}

	private byte Get_unsigned8()
	{
		return m_unsigned8;
	}

	private byte Get_snum()
	{
		return m_snum;
	}

	private byte Get_g()
	{
		return m_g;
	}

	private byte Get_lepa()
	{
		return m_lepa;
	}

	private byte Get_disp6()
	{
		return m_disp6;
	}

	private byte Get_radr()
	{
		return m_radr;
	}

	private ushort Get_dadr()
	{
		return m_dadr;
	}

	private ushort Get_disp16()
	{
		return m_disp16;
	}

	private ushort Get_dbitadr()
	{
		return m_dbitadr;
	}

	private ushort Get_cadr()
	{
		return m_cadr;
	}

	public CDasmu8()
	{
		m_dasmCode = new ushort[2];
	}

	public bool Disassemble(ushort code0, ushort code1)
	{
		m_dasmCode[0] = code0;
		m_dasmCode[1] = code1;
		m_dasmNum = 1;
		m_errCode = DasmU8ErrCodeNumber.DasmU8_no_err;
		Disasmu8();
		if (m_errCode != 0)
		{
			m_dasmNum = 1;
			return false;
		}
		return true;
	}

	public ushort GetUsedCodeNum()
	{
		return m_dasmNum;
	}

	public void Disasmu8()
	{
		byte b = BM.I2B((m_dasmCode[0] & 0xF000) >> 12);
		byte b2 = BM.I2B(m_dasmCode[0] & 0xF);
		byte b3 = BM.I2B((m_dasmCode[0] & 0xF0) >> 4);
		byte b4 = BM.I2B((m_dasmCode[0] & 0xF00) >> 8);
		m_ins_type = -1;
		if (b < 8)
		{
			m_ins_type = 100;
			m_op = b;
			m_n = b4;
			m_imm8 = BM.I2B(m_dasmCode[0] & 0xFF);
			return;
		}
		switch (b)
		{
		case 8:
			if (b2 != 15)
			{
				m_ins_type = 101;
				m_op = b2;
				m_n = b4;
				m_m = b3;
			}
			else if ((b3 & 1) == 1)
			{
				m_ins_type = 102;
				m_op = b3;
				m_n = b4;
			}
			else
			{
				m_ins_type = 103;
				m_n = b3;
			}
			break;
		case 9:
			switch (b2)
			{
			case 0:
			case 1:
				m_ins_type = 104;
				m_op = b2;
				switch (b3)
				{
				case 1:
					m_op2 = 6;
					break;
				case 3:
					m_op2 = 0;
					break;
				case 5:
					m_op2 = 1;
					break;
				default:
					m_op2 = 2;
					break;
				}
				m_op3 = 0;
				m_n = b4;
				m_m = b3;
				if (b3 == 1)
				{
					m_dadr = m_dasmCode[1];
					m_dasmNum = 2;
				}
				break;
			case 2:
			case 3:
				m_ins_type = 104;
				m_op = BM.I2B(b2 & 1);
				switch (b3)
				{
				case 1:
					m_op2 = 6;
					break;
				case 3:
					m_op2 = 0;
					break;
				case 5:
					m_op2 = 1;
					break;
				default:
					m_op2 = 2;
					break;
				}
				m_op3 = 1;
				m_n = b4;
				m_m = b3;
				if (b3 == 1)
				{
					m_dadr = m_dasmCode[1];
					m_dasmNum = 2;
				}
				break;
			case 4:
			case 5:
				m_ins_type = 104;
				m_op = BM.I2B(b2 & 1);
				switch (b3)
				{
				case 3:
					m_op2 = 0;
					break;
				case 5:
					m_op2 = 1;
					break;
				}
				m_op3 = 2;
				m_n = b4;
				break;
			case 6:
			case 7:
				m_ins_type = 104;
				m_op = BM.I2B(b2 & 1);
				switch (b3)
				{
				case 3:
					m_op2 = 0;
					break;
				case 5:
					m_op2 = 1;
					break;
				}
				m_op3 = 3;
				m_n = b4;
				break;
			case 8:
			case 9:
				m_ins_type = 104;
				m_op = BM.I2B(b2 - 8);
				m_op2 = 3;
				m_op3 = 0;
				m_n = b4;
				m_m = b3;
				m_disp16 = m_dasmCode[1];
				m_dasmNum = 2;
				break;
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
				m_ins_type = 109;
				m_op = b2;
				m_n = b4;
				m_w = b3;
				break;
			case 15:
				m_ins_type = 130;
				m_n = b3;
				break;
			}
			break;
		case 10:
			switch (b2)
			{
			case 0:
			case 1:
			case 2:
				m_ins_type = 110;
				m_op = b2;
				m_b = b3;
				if (b3 < 8)
				{
					m_n = b4;
					break;
				}
				m_dbitadr = m_dasmCode[1];
				m_dasmNum = 2;
				break;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
				m_ins_type = 111;
				m_op = b2;
				m_n = b4;
				m_m = b3;
				break;
			case 8:
			case 9:
				m_ins_type = 104;
				m_op = BM.I2B(b2 - 8);
				m_op2 = 3;
				m_op3 = 1;
				m_n = b4;
				m_m = b3;
				m_disp16 = m_dasmCode[1];
				m_dasmNum = 2;
				break;
			case 10:
				m_ins_type = 113;
				m_n = b4;
				m_m = b3;
				break;
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
				m_ins_type = 114;
				m_op = b2;
				if (b2 == 13)
				{
					m_m = b4;
					break;
				}
				m_n = b4;
				m_m = b3;
				break;
			}
			break;
		case 11:
			m_ins_type = 104;
			m_op = BM.I2B(b3 >> 3);
			m_op2 = BM.I2B(((b3 & 4) == 0) ? 4 : 5);
			m_op3 = 1;
			m_n = b4;
			m_disp6 = BM.I2B(m_dasmCode[0] & 0x3F);
			break;
		case 12:
			m_ins_type = 116;
			m_op = b4;
			m_radr = BM.I2B(m_dasmCode[0] & 0xFF);
			break;
		case 13:
			m_ins_type = 104;
			m_op = BM.I2B(b3 >> 3);
			m_op2 = BM.I2B(((b3 & 4) == 0) ? 4 : 5);
			m_op3 = 0;
			m_n = b4;
			m_disp6 = BM.I2B(m_dasmCode[0] & 0x3F);
			break;
		case 14:
			if ((b4 & 1) == 0)
			{
				m_ins_type = 118;
				m_op = BM.I2B(b3 >> 3);
				m_n = b4;
				m_imm7 = BM.I2B(m_dasmCode[0] & 0x7F);
				break;
			}
			switch (b4)
			{
			case 1:
				m_ins_type = 119;
				m_signed8 = BM.I2B(m_dasmCode[0] & 0xFF);
				break;
			case 9:
				m_ins_type = 120;
				m_unsigned8 = BM.I2B(m_dasmCode[0] & 0xFF);
				break;
			case 11:
			case 13:
				m_ins_type = 121;
				m_op = b2;
				break;
			case 5:
				m_ins_type = 122;
				m_snum = BM.I2B(m_dasmCode[0] & 0x3F);
				break;
			case 3:
				m_ins_type = 131;
				m_imm8 = BM.I2B(m_dasmCode[0] & 0xFF);
				break;
			default:
				m_errCode = DasmU8ErrCodeNumber.DasmU8_syntax_err;
				break;
			}
			break;
		case 15:
			switch (b2)
			{
			case 0:
			case 1:
			case 2:
			case 3:
				m_ins_type = 123;
				m_op = b2;
				if (b2 < 2)
				{
					m_g = b4;
					m_cadr = m_dasmCode[1];
					m_dasmNum = 2;
				}
				else
				{
					m_n = b3;
				}
				break;
			case 4:
			case 9:
				m_ins_type = 124;
				m_op = b2;
				m_n = b4;
				m_m = b3;
				break;
			case 5:
			case 6:
			case 7:
				m_ins_type = 125;
				m_op = b2;
				m_n = b4;
				m_m = b3;
				break;
			case 10:
			case 11:
			case 12:
				m_ins_type = 126;
				m_op = b2;
				m_m = b3;
				switch (b2)
				{
				case 11:
					m_disp16 = m_dasmCode[1];
					m_dasmNum = 2;
					break;
				case 12:
					m_dadr = m_dasmCode[1];
					m_dasmNum = 2;
					break;
				}
				break;
			case 13:
				m_ins_type = 127;
				m_op = b3;
				if (b3 < 8)
				{
					m_n = b4;
				}
				else
				{
					m_m = b4;
				}
				break;
			case 14:
				m_ins_type = 128;
				m_op = b3;
				if (b3 < 8)
				{
					m_n = b4;
				}
				else
				{
					m_lepa = b4;
				}
				break;
			case 15:
				switch (b4)
				{
				case 14:
					switch (b3)
					{
					case 9:
						m_ins_type = 132;
						break;
					case 15:
						m_ins_type = 133;
						break;
					default:
						m_ins_type = 129;
						m_op = b3;
						break;
					}
					break;
				case 15:
					if (b3 == 15)
					{
						m_ins_type = 134;
					}
					break;
				}
				break;
			case 8:
				break;
			}
			break;
		}
	}
}
