namespace SimCopro;

public class CSimCoproApp
{
	private byte[] m_CR;

	private byte[] tmp_cr;

	private bool cal_start;

	private readonly bool cal_en;

	private byte st_count;

	private bool mul_flag;

	private bool div_flag;

	private bool div32_flag;

	private bool som_flag;

	private bool soms_flag;

	private byte m_WaitReqFlag;

	public const byte B_CLMOD0 = 1;

	public const byte B_CLMOD1 = 2;

	public const byte B_SIGN = 16;

	public const byte B_CLEN = 128;

	public const byte B_USE = 1;

	public const byte B_Q = 8;

	public const byte B_OV = 16;

	public const byte B_S = 32;

	public const byte B_Z = 64;

	public const byte B_C = 128;

	public static CSimCoproApp theApp = new CSimCoproApp();

	public CSimCoproApp()
	{
		m_CR = new byte[16];
		tmp_cr = new byte[16];
		for (int i = 0; i < 16; i++)
		{
			m_CR[i] = 0;
			tmp_cr[i] = 0;
		}
		m_WaitReqFlag = 0;
		cal_start = false;
		st_count = 0;
		mul_flag = false;
		div_flag = false;
		som_flag = false;
		soms_flag = false;
		div32_flag = false;
	}

	public bool InitInstance()
	{
		return true;
	}

	public int mul_func()
	{
		ushort num = 0;
		ushort num2 = 0;
		uint num3 = 0u;
		bool sign = false;
		num = BM.MAKEWORD(tmp_cr[4], tmp_cr[5]);
		num2 = BM.MAKEWORD(tmp_cr[6], tmp_cr[7]);
		if ((tmp_cr[8] & 0x10u) != 0)
		{
			sign = true;
		}
		if ((tmp_cr[8] & 0x80u) != 0)
		{
			tmp_cr[9] = BM.I2B(tmp_cr[9] | 1);
			st_count++;
			if (st_count == 4)
			{
				byte st = tmp_cr[9];
				num3 = mul_subfn(num, num2, sign, ref st);
				tmp_cr[9] = st;
				m_CR[3] = BM.UI2B((num3 >> 24) & 0xFFu);
				m_CR[2] = BM.UI2B((num3 >> 16) & 0xFFu);
				m_CR[1] = BM.UI2B((num3 >> 8) & 0xFFu);
				m_CR[0] = BM.UI2B(num3 & 0xFFu);
				tmp_cr[9] = BM.I2B(tmp_cr[9] & -2);
				m_CR[9] = tmp_cr[9];
				mul_flag = false;
				st_count = 0;
			}
		}
		return 0;
	}

	public uint mul_subfn(ushort a, ushort b, bool sign, ref byte st)
	{
		ushort num = a;
		ushort num2 = b;
		uint num3 = 0u;
		byte b2 = st;
		if (num == 0 || num2 == 0)
		{
			num3 = 0u;
			b2 = BM.I2B(BM.I2B(BM.I2B(b2 | 0x40) & -33) & -129);
		}
		else if (sign)
		{
			bool flag = false;
			bool flag2 = false;
			if ((num & 0x8000u) != 0)
			{
				flag = true;
				num = BM.I2W((~num + 1) & 0xFFFF);
			}
			if ((num2 & 0x8000u) != 0)
			{
				flag2 = true;
				num2 = BM.I2W((~num2 + 1) & 0xFFFF);
			}
			num3 = BM.L2UI((num * num2) & 0xFFFFFFFFu);
			if (flag ^ flag2)
			{
				num3 = ~num3 + 1;
				b2 = BM.I2B(b2 | 0x20);
			}
			else
			{
				b2 = BM.I2B(b2 & -33);
			}
			b2 = BM.I2B(BM.I2B(b2 & -65) & -129);
		}
		else
		{
			num3 = BM.L2UI((a * b) & 0xFFFFFFFFu);
			b2 = BM.I2B(BM.I2B(BM.I2B(b2 & -33) & -65) & -129);
		}
		st = b2;
		return num3;
	}

	public int div_func()
	{
		ushort num = 0;
		ushort num2 = 0;
		ulong num3 = 0uL;
		ulong num4 = 0uL;
		ushort num5 = 0;
		ulong num6 = 0uL;
		ushort num7 = 0;
		num = BM.MAKEWORD(tmp_cr[0], tmp_cr[1]);
		ushort num8 = BM.MAKEWORD(tmp_cr[2], tmp_cr[3]);
		BM.MAKEWORD(tmp_cr[4], tmp_cr[5]);
		num2 = BM.MAKEWORD(tmp_cr[6], tmp_cr[7]);
		num3 = (((ulong)num8 << 16) & 0xFFFF0000u) + ((ulong)num & 0xFFFFuL);
		if ((tmp_cr[8] & 0x80u) != 0)
		{
			tmp_cr[9] = BM.I2B(tmp_cr[9] | 1);
			st_count++;
			if (st_count == 8)
			{
				if ((tmp_cr[8] & 0x10u) != 0)
				{
					if (num2 == 0)
					{
						if ((num3 & 0x80000000u) == 0L)
						{
							num4 = 2147483647uL;
							num5 = BM.I2W(num & 0x7FFF);
							tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
						}
						else
						{
							num4 = 2147483648uL;
							num5 = BM.I2W((num & 0x7FFF) | 0x8000);
							tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x20);
						}
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x80 | 0x40);
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						num6 = num3;
						num7 = num2;
						if ((num3 & 0x80000000u) != 0L)
						{
							flag = true;
							num6 = (~num3 + 1) & 0xFFFFFFFFu;
						}
						if ((num2 & 0x8000u) != 0)
						{
							flag2 = true;
							num7 = BM.I2W((~num2 + 1) & 0xFFFF);
						}
						num4 = num6 / num7;
						num5 = BM.UL2W((num6 % num7) & 0xFFFF);
						if (flag ^ flag2)
						{
							num4 = ~num4 + 1;
							tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x20);
						}
						else
						{
							tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
						}
						if (flag)
						{
							num5 = BM.I2W(~num5 + 1);
						}
						if (num4 == 0L)
						{
							tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x40);
						}
						else
						{
							tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
						}
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -129);
					}
				}
				else if (num2 == 0)
				{
					num4 = 4294967295uL;
					num5 = BM.I2W(num & 0xFFFF);
					tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x80);
					tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
					tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
				}
				else
				{
					num6 = num3;
					num7 = num2;
					num4 = num6 / num7;
					num5 = BM.UL2W((num6 % num7) & 0xFFFF);
					tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
					if (num4 == 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x40);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
					}
					tmp_cr[9] = BM.I2B(tmp_cr[9] & -129);
				}
				m_CR[5] = BM.I2B((num5 >> 8) & 0xFF);
				m_CR[4] = BM.I2B(num5 & 0xFF);
				m_CR[3] = BM.UL2B((num4 >> 24) & 0xFF);
				m_CR[2] = BM.UL2B((num4 >> 16) & 0xFF);
				m_CR[1] = BM.UL2B((num4 >> 8) & 0xFF);
				m_CR[0] = BM.UL2B(num4 & 0xFF);
				tmp_cr[9] = BM.I2B(tmp_cr[9] & -2);
				m_CR[9] = tmp_cr[9];
				div_flag = false;
				st_count = 0;
			}
		}
		return 0;
	}

	public int div32_func()
	{
		ushort num = 0;
		ushort num2 = 0;
		ushort num3 = 0;
		ulong num4 = 0uL;
		ulong num5 = 0uL;
		ulong num6 = 0uL;
		ulong num7 = 0uL;
		ulong num8 = 0uL;
		ulong num9 = 0uL;
		num = BM.MAKEWORD(tmp_cr[0], tmp_cr[1]);
		num2 = BM.MAKEWORD(tmp_cr[2], tmp_cr[3]);
		num3 = BM.MAKEWORD(tmp_cr[4], tmp_cr[5]);
		ushort num10 = BM.MAKEWORD(tmp_cr[6], tmp_cr[7]);
		num4 = (((ulong)num2 << 16) & 0xFFFF0000u) + ((ulong)num & 0xFFFFuL);
		num5 = (((ulong)num10 << 16) & 0xFFFF0000u) + ((ulong)num3 & 0xFFFFuL);
		if ((tmp_cr[8] & 0x80u) != 0)
		{
			tmp_cr[9] = BM.I2B(tmp_cr[9] | 1);
			st_count++;
			if (st_count == 16)
			{
				if ((tmp_cr[8] & 0x10u) != 0)
				{
					if (num5 == 0L)
					{
						if ((num4 & 0x80000000u) == 0L)
						{
							num6 = 2147483647uL;
							num7 = num4 & 0x7FFFFFFF;
							tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
						}
						else
						{
							num6 = 2147483648uL;
							num7 = (num4 & 0x7FFFFFFF) | 0x80000000u;
							tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x20);
						}
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x80 | 0x40);
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						num8 = num4;
						num9 = num5;
						if ((num4 & 0x80000000u) != 0L)
						{
							flag = true;
							num8 = (~num4 + 1) & 0xFFFFFFFFu;
						}
						if ((num5 & 0x80000000u) != 0L)
						{
							flag2 = true;
							num9 = (~num5 + 1) & 0xFFFFFFFFu;
						}
						num6 = num8 / num9;
						num7 = num8 % num9;
						if (flag ^ flag2)
						{
							num6 = ~num6 + 1;
							tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x20);
						}
						else
						{
							tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
						}
						if (flag)
						{
							num7 = ~num7 + 1;
						}
						if (num6 == 0L)
						{
							tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x40);
						}
						else
						{
							tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
						}
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -129);
					}
				}
				else if (num5 == 0L)
				{
					num6 = 4294967295uL;
					num7 = num4 & 0xFFFFFFFFu;
					tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x80);
					tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
					tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
				}
				else
				{
					num8 = num4;
					num9 = num5;
					num6 = num8 / num9;
					num7 = num8 % num9;
					tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
					if (num6 == 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x40);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
					}
					tmp_cr[9] = BM.I2B(tmp_cr[9] & -129);
				}
				m_CR[7] = BM.UL2B((num7 >> 24) & 0xFF);
				m_CR[6] = BM.UL2B((num7 >> 16) & 0xFF);
				m_CR[5] = BM.UL2B((num7 >> 8) & 0xFF);
				m_CR[4] = BM.UL2B(num7 & 0xFF);
				m_CR[3] = BM.UL2B((num6 >> 24) & 0xFF);
				m_CR[2] = BM.UL2B((num6 >> 16) & 0xFF);
				m_CR[1] = BM.UL2B((num6 >> 8) & 0xFF);
				m_CR[0] = BM.UL2B(num6 & 0xFF);
				tmp_cr[9] = BM.I2B(tmp_cr[9] & -2);
				m_CR[9] = tmp_cr[9];
				div32_flag = false;
				st_count = 0;
			}
		}
		return 0;
	}

	public int som_func()
	{
		ushort num = 0;
		ushort num2 = 0;
		ushort num3 = 0;
		ulong num4 = 0uL;
		uint num5 = 0u;
		ulong num6 = 0uL;
		bool flag = false;
		num = BM.MAKEWORD(tmp_cr[0], tmp_cr[1]);
		ushort num7 = BM.MAKEWORD(tmp_cr[2], tmp_cr[3]);
		num2 = BM.MAKEWORD(tmp_cr[4], tmp_cr[5]);
		num3 = BM.MAKEWORD(tmp_cr[6], tmp_cr[7]);
		num4 = (((ulong)num7 << 16) & 0xFFFF0000u) + ((ulong)num & 0xFFFFuL);
		if ((tmp_cr[8] & 0x10u) != 0)
		{
			flag = true;
		}
		if ((tmp_cr[8] & 0x80u) != 0)
		{
			tmp_cr[9] = BM.I2B(tmp_cr[9] | 1);
			st_count++;
			if (st_count == 4)
			{
				byte st = tmp_cr[9];
				num5 = mul_subfn(num2, num3, flag, ref st);
				tmp_cr[9] = st;
				if (flag)
				{
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					ulong num8 = 0uL;
					ulong num9 = 0uL;
					if ((num4 & 0x80000000u) != 0L)
					{
						num8 = num4 | 0xFFFFFFFF00000000uL;
						flag2 = true;
					}
					else
					{
						num8 = num4;
						flag2 = false;
					}
					if ((num5 & 0x80000000u) != 0)
					{
						num9 = num5 | 0xFFFFFFFF00000000uL;
						flag3 = true;
					}
					else
					{
						num9 = num5;
						flag3 = false;
					}
					num6 = num9 + num8;
					if (num5 + num4 > uint.MaxValue)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x80);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -129);
					}
					if ((num6 & 0x80000000u) != 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x20);
						flag4 = true;
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
						flag4 = false;
					}
					if ((num6 & 0xFFFFFFFFu) == 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x40);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
					}
					if (flag2 && flag3 && !flag4)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x10);
					}
					else if (!flag2 && !flag3 && flag4)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x10);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -17);
					}
				}
				else
				{
					num6 = num5 + num4;
					if (num6 > uint.MaxValue)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x80);
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x10);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -129);
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -17);
					}
					if ((num6 & 0x80000000u) != 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x20);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
					}
					if ((num6 & 0xFFFFFFFFu) == 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x40);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
					}
				}
				m_CR[3] = BM.UL2B((num6 >> 24) & 0xFF);
				m_CR[2] = BM.UL2B((num6 >> 16) & 0xFF);
				m_CR[1] = BM.UL2B((num6 >> 8) & 0xFF);
				m_CR[0] = BM.UL2B(num6 & 0xFF);
				tmp_cr[9] = BM.I2B(tmp_cr[9] & -2);
				m_CR[9] = tmp_cr[9];
				som_flag = false;
				st_count = 0;
			}
		}
		return 0;
	}

	public int soms_func()
	{
		ushort num = 0;
		ushort num2 = 0;
		ushort num3 = 0;
		ulong num4 = 0uL;
		uint num5 = 0u;
		ulong num6 = 0uL;
		bool flag = false;
		num = BM.MAKEWORD(tmp_cr[0], tmp_cr[1]);
		ushort num7 = BM.MAKEWORD(tmp_cr[2], tmp_cr[3]);
		num2 = BM.MAKEWORD(tmp_cr[4], tmp_cr[5]);
		num3 = BM.MAKEWORD(tmp_cr[6], tmp_cr[7]);
		num4 = (((ulong)num7 << 16) & 0xFFFF0000u) + ((ulong)num & 0xFFFFuL);
		if ((tmp_cr[8] & 0x10u) != 0)
		{
			flag = true;
		}
		if ((tmp_cr[8] & 0x80u) != 0)
		{
			tmp_cr[9] = BM.I2B(tmp_cr[9] | 1);
			st_count++;
			if (st_count == 4)
			{
				byte st = tmp_cr[9];
				num5 = mul_subfn(num2, num3, flag, ref st);
				tmp_cr[9] = st;
				if (flag)
				{
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					ulong num8 = 0uL;
					ulong num9 = 0uL;
					if ((num4 & 0x80000000u) != 0L)
					{
						num8 = num4 | 0xFFFFFFFF00000000uL;
						flag2 = true;
					}
					else
					{
						num8 = num4;
						flag2 = false;
					}
					if ((num5 & 0x80000000u) != 0)
					{
						num9 = num5 | 0xFFFFFFFF00000000uL;
						flag3 = true;
					}
					else
					{
						num9 = num5;
						flag3 = false;
					}
					num6 = num9 + num8;
					if (num5 + num4 > uint.MaxValue)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x80);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -129);
					}
					if ((num6 & 0x80000000u) != 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x20);
						flag4 = true;
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
						flag4 = false;
					}
					if ((num6 & 0xFFFFFFFFu) == 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x40);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
					}
					if (flag2 && flag3 && !flag4)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x10);
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 8);
						num6 = 18446744071562067968uL;
					}
					else if (!flag2 && !flag3 && flag4)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x10);
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 8);
						num6 = 2147483647uL;
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -17);
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -9);
					}
				}
				else
				{
					num6 = num5 + num4;
					if (num6 > uint.MaxValue)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x80);
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x10);
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 8);
						num6 = ulong.MaxValue;
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -129);
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -17);
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -9);
					}
					if ((num6 & 0x80000000u) != 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x20);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -33);
					}
					if ((num6 & 0xFFFFFFFFu) == 0L)
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] | 0x40);
					}
					else
					{
						tmp_cr[9] = BM.I2B(tmp_cr[9] & -65);
					}
				}
				m_CR[3] = BM.UL2B((num6 >> 24) & 0xFF);
				m_CR[2] = BM.UL2B((num6 >> 16) & 0xFF);
				m_CR[1] = BM.UL2B((num6 >> 8) & 0xFF);
				m_CR[0] = BM.UL2B(num6 & 0xFF);
				tmp_cr[9] = BM.I2B(tmp_cr[9] & -2);
				m_CR[9] = tmp_cr[9];
				soms_flag = false;
				st_count = 0;
			}
		}
		return 0;
	}

	public int cpApi_Run()
	{
		int result = 0;
		if (theApp.cal_start)
		{
			switch (theApp.tmp_cr[8] & 7)
			{
			case 0:
				theApp.mul_flag = true;
				theApp.div_flag = false;
				theApp.som_flag = false;
				theApp.soms_flag = false;
				theApp.div32_flag = false;
				break;
			case 1:
				theApp.mul_flag = false;
				theApp.div_flag = true;
				theApp.som_flag = false;
				theApp.soms_flag = false;
				theApp.div32_flag = false;
				break;
			case 2:
				theApp.mul_flag = false;
				theApp.div_flag = false;
				theApp.som_flag = true;
				theApp.soms_flag = false;
				theApp.div32_flag = false;
				break;
			case 3:
				theApp.mul_flag = false;
				theApp.div_flag = false;
				theApp.som_flag = false;
				theApp.soms_flag = true;
				theApp.div32_flag = false;
				break;
			case 5:
				if ((theApp.tmp_cr[15] & 0x80u) != 0)
				{
					theApp.mul_flag = false;
					theApp.div_flag = false;
					theApp.som_flag = false;
					theApp.soms_flag = false;
					theApp.div32_flag = true;
				}
				else
				{
					theApp.mul_flag = false;
					theApp.div_flag = false;
					theApp.som_flag = false;
					theApp.soms_flag = false;
					theApp.div32_flag = false;
				}
				break;
			default:
				theApp.mul_flag = false;
				theApp.div_flag = false;
				theApp.som_flag = false;
				theApp.soms_flag = false;
				theApp.div32_flag = false;
				break;
			}
			theApp.cal_start = false;
		}
		if (theApp.mul_flag)
		{
			result = theApp.mul_func();
		}
		else if (theApp.div_flag)
		{
			result = theApp.div_func();
		}
		else if (theApp.som_flag)
		{
			result = theApp.som_func();
		}
		else if (theApp.soms_flag)
		{
			result = theApp.soms_func();
		}
		else if (theApp.div32_flag)
		{
			result = theApp.div32_func();
		}
		return result;
	}

	public int cpApi_SetReg(byte regNo, uint val)
	{
		int num = 0;
		if (regNo < 0 || 15 < regNo)
		{
			num = 4;
		}
		if (num == 0 && regNo != 15)
		{
			theApp.m_CR[regNo] = (byte)val;
		}
		if (regNo == 7 && (theApp.m_CR[8] & 0x80) == 128)
		{
			theApp.cal_start = true;
			theApp.st_count = 0;
			for (int i = 0; i < 16; i++)
			{
				theApp.tmp_cr[i] = theApp.m_CR[i];
			}
		}
		return num;
	}

	public int cpApi_GetReg(byte regNo, ref uint val)
	{
		int num = 0;
		if (regNo < 0 || 15 < regNo)
		{
			num = 4;
		}
		if (num == 0)
		{
			val = theApp.m_CR[regNo];
		}
		return num;
	}

	public int cpApi_GetWaitReq(ref byte val)
	{
		val = theApp.m_WaitReqFlag;
		return 0;
	}

	public int cpApi_Reset()
	{
		int result = 0;
		for (int i = 0; i < 16; i++)
		{
			if (i != 15)
			{
				theApp.m_CR[i] = 0;
			}
			theApp.tmp_cr[i] = 0;
		}
		theApp.cal_start = false;
		theApp.st_count = 0;
		theApp.mul_flag = false;
		theApp.div_flag = false;
		theApp.som_flag = false;
		theApp.soms_flag = false;
		theApp.div32_flag = false;
		return result;
	}

	public int cpApi_SetCoproID(byte id)
	{
		theApp.m_CR[15] = id;
		return 0;
	}
}
