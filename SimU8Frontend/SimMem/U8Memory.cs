using System;

namespace SimMem;

public class U8Memory
{
	public byte[] m_MemBuf = new byte[1];

	private uint m_iStartAdr;

	private uint m_iEndAdr;

	public U8Memory()
	{
		m_iStartAdr = 65536u;
		m_iEndAdr = 0u;
		SetSize(65536u);
	}

	public void SetSize(uint size)
	{
		if (m_MemBuf == null)
		{
			m_MemBuf = new byte[size];
		}
		else if (m_MemBuf.Length < size)
		{
			byte[] array = new byte[size];
			Array.Copy(m_MemBuf, array, m_MemBuf.Length);
			m_MemBuf = array;
		}
	}

	public int SetVal(uint nIndex, byte val)
	{
		if (nIndex < m_iStartAdr || nIndex > m_iEndAdr)
		{
			return -1;
		}
		m_MemBuf[nIndex] = val;
		return 0;
	}

	public int GetVal(uint nIndex, ref byte val)
	{
		if (nIndex < m_iStartAdr || nIndex > m_iEndAdr)
		{
			return -1;
		}
		val = m_MemBuf[nIndex];
		return 0;
	}

	public int SetWordVal(uint nIndex, ushort val)
	{
		if (nIndex < m_iStartAdr || nIndex + 1 > m_iEndAdr)
		{
			return -1;
		}
		m_MemBuf[nIndex] = BM.LOBYTE(val);
		m_MemBuf[nIndex + 1] = BM.HIBYTE(val);
		return 0;
	}

	public int GetWordVal(uint nIndex, ref ushort val)
	{
		if (nIndex < m_iStartAdr || nIndex + 1 > m_iEndAdr)
		{
			return -1;
		}
		val = BM.MAKEWORD(m_MemBuf[nIndex], m_MemBuf[nIndex + 1]);
		return 0;
	}

	public int SetRange(uint sadr, uint eadr)
	{
		m_iStartAdr = sadr;
		m_iEndAdr = eadr;
		if (m_iStartAdr == 0 && m_iEndAdr == 0)
		{
			SetSize(0u);
		}
		else
		{
			SetSize(m_iEndAdr + 1);
		}
		return 0;
	}

	public void GetRange(ref uint sadr, ref uint eadr)
	{
		sadr = m_iStartAdr;
		eadr = m_iEndAdr;
	}
}
