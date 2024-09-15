namespace SimMem;

public class Mapping
{
	private byte m_Count;

	private uint[] m_StartAddress;

	private uint[] m_EndAddress;

	private uint[] m_Attribute;

	public Mapping()
	{
		m_Count = 1;
		m_StartAddress = new uint[16];
		m_EndAddress = new uint[16];
		m_Attribute = new uint[16];
		m_StartAddress[0] = 0u;
		m_EndAddress[0] = 65535u;
		m_Attribute[0] = 0u;
	}

	public void SetCount(byte n)
	{
		if (n >= 1 && n <= 16)
		{
			m_Count = n;
		}
		else
		{
			m_Count = 0;
		}
	}

	public byte GetCount()
	{
		return m_Count;
	}

	public int SetStartAddress(byte n, uint val)
	{
		int result = 0;
		if (n >= 0 && n <= 15)
		{
			m_StartAddress[n] = val;
		}
		else
		{
			result = -1;
		}
		return result;
	}

	public int GetStartAddress(byte n, ref uint val)
	{
		int result = 0;
		if (n >= 0 && n <= 15)
		{
			val = m_StartAddress[n];
		}
		else
		{
			result = -1;
		}
		return result;
	}

	public int SetEndAddress(byte n, uint val)
	{
		int result = 0;
		if (n >= 0 && n <= 15)
		{
			m_EndAddress[n] = val;
		}
		else
		{
			result = -1;
		}
		return result;
	}

	public int GetEndAddress(byte n, ref uint val)
	{
		int result = 0;
		if (n >= 0 && n <= 15)
		{
			val = m_EndAddress[n];
		}
		else
		{
			result = -1;
		}
		return result;
	}

	public int SetAttribute(byte n, byte val)
	{
		int result = 0;
		if (n >= 0 && n <= 15)
		{
			m_Attribute[n] = val;
		}
		else
		{
			result = -1;
		}
		return result;
	}

	public int GetAttribute(byte n, ref byte val)
	{
		int result = 0;
		if (n >= 0 && n <= 15)
		{
			val = (byte)m_Attribute[n];
		}
		else
		{
			result = -1;
		}
		return result;
	}
}
