namespace SimU8engine;

public struct SIMU8_INTERRUPT_TABLE
{
	public uint vector_adrs;

	public uint ie_adrs;

	public byte ie_bit;

	public uint irq_adrs;

	public byte irq_bit;

	public byte[] intname;

	public void InitIntname()
	{
		intname = new byte[20];
	}
}
