namespace SimU8engine;

public class CRegister
{
	public const int R0 = 0;

	public const int R1 = 1;

	public const int R2 = 2;

	public const int R3 = 3;

	public const int R4 = 4;

	public const int R5 = 5;

	public const int R6 = 6;

	public const int R7 = 7;

	public const int R8 = 8;

	public const int R9 = 9;

	public const int R10 = 10;

	public const int R11 = 11;

	public const int R12 = 12;

	public const int R13 = 13;

	public const int R14 = 14;

	public const int R15 = 15;

	public const int PC = 16;

	public const int LR = 17;

	public const int ELR1 = 18;

	public const int ELR2 = 19;

	public const int ELR3 = 20;

	public const int CSR = 21;

	public const int LCSR = 22;

	public const int ECSR1 = 23;

	public const int ECSR2 = 24;

	public const int ECSR3 = 25;

	public const int PSW = 26;

	public const int EPSW1 = 27;

	public const int EPSW2 = 28;

	public const int EPSW3 = 29;

	public const int SP = 30;

	public const int EA = 31;

	public const int CR0 = 32;

	public const int CR1 = 33;

	public const int CR2 = 34;

	public const int CR3 = 35;

	public const int CR4 = 36;

	public const int CR5 = 37;

	public const int CR6 = 38;

	public const int CR7 = 39;

	public const int CR8 = 40;

	public const int CR9 = 41;

	public const int CR10 = 42;

	public const int CR11 = 43;

	public const int CR12 = 44;

	public const int CR13 = 45;

	public const int CR14 = 46;

	public const int CR15 = 47;

	public const int C = 48;

	public const int Z = 49;

	public const int S = 50;

	public const int OV = 51;

	public const int MIE = 52;

	public const int HC = 53;

	public const int ELEVEL = 54;

	public const int PSWEND = 55;

	public const int STPACP = 61448;

	public const int SBYCON = 61449;

	public const int IE0 = 61456;

	public const int IE1 = 61457;

	public const int IRQ0 = 61460;

	public const int IRQ1 = 61461;

	private readonly byte[] m_R;

	private ushort m_PC;

	private readonly ushort[] m_LR;

	private byte m_CSR;

	private readonly byte[] m_LCSR;

	private byte m_PSW;

	private readonly byte[] m_EPSW;

	private ushort m_SP;

	private ushort m_EA;

	public CRegister()
	{
		m_R = new byte[16];
		m_LR = new ushort[4];
		m_LCSR = new byte[4];
		m_EPSW = new byte[4];
		Reset();
	}

	public void Reset()
	{
		m_PC = 0;
		m_SP = 0;
		for (int i = 0; i < 16; i++)
		{
			m_R[i] = 0;
		}
		for (int j = 0; j < 4; j++)
		{
			m_LR[j] = 0;
			m_LCSR[j] = 0;
			m_EPSW[j] = 0;
		}
		m_CSR = 0;
		m_PSW = 0;
		m_EA = 0;
	}

	public int SetReg(byte type, uint val)
	{
		int result = 0;
		if (type >= 0 && type <= 15)
		{
			m_R[type] = BM.UI2B(val & 0xFFu);
		}
		else
		{
			switch (type)
			{
			case 16:
				m_PC = BM.UI2W(val & 0xFFFFu);
				break;
			case 17:
			case 18:
			case 19:
			case 20:
				m_LR[type - 17] = BM.UI2W(val & 0xFFFFu);
				break;
			case 21:
				m_CSR = BM.UI2B(val & 0xFu);
				break;
			case 22:
			case 23:
			case 24:
			case 25:
				m_LCSR[type - 22] = BM.UI2B(val & 0xFu);
				break;
			case 26:
				m_PSW = BM.UI2B(val & 0xFFu);
				break;
			case 27:
			case 28:
			case 29:
				m_EPSW[type - 26] = BM.UI2B(val & 0xFFu);
				break;
			case 30:
				m_SP = BM.UI2W(val & 0xFFFFu);
				break;
			case 31:
				m_EA = BM.UI2W(val & 0xFFFFu);
				break;
			default:
				result = -1;
				break;
			}
		}
		return result;
	}

	public int GetReg(byte type)
	{
		if (type >= 0 && type <= 15)
		{
			return m_R[type];
		}
		switch (type)
		{
		case 16:
			return m_PC;
		case 17:
		case 18:
		case 19:
		case 20:
			return m_LR[type - 17];
		case 21:
			return m_CSR;
		case 22:
		case 23:
		case 24:
		case 25:
			return m_LCSR[type - 22];
		case 26:
			return m_PSW;
		case 27:
		case 28:
		case 29:
			return m_EPSW[type - 26];
		case 30:
			return m_SP;
		case 31:
			return m_EA;
		default:
			return -1;
		}
	}

	public void SetPSW(byte type, byte val)
	{
		byte b = BM.I2B(55 - type);
		switch (b)
		{
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		{
			byte b2 = BM.I2B(1 << (int)b);
			if ((val & 1) == 1)
			{
				m_PSW |= b2;
			}
			else
			{
				m_PSW &= BM.I2B(~b2);
			}
			break;
		}
		case 0:
		case 1:
			m_PSW = BM.I2B((m_PSW & 0xFC) | (val & 3));
			break;
		}
	}

	public byte GetPSW(byte type)
	{
		byte b = BM.I2B(55 - type);
		switch (b)
		{
		default:
			return 0;
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			return BM.I2B((m_PSW >> (int)b) & 1);
		case 0:
		case 1:
			return BM.I2B(m_PSW & 3);
		}
	}
}
