using System.IO;

namespace SimU8engine;

public class CStdioFileR
{
	private string _filename;

	private TextReader _reader;

	public bool OpenForRead(string filename)
	{
		try
		{
			_filename = filename;
			_reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
			return true;
		}
		catch (IOException)
		{
			return false;
		}
	}

	public bool ReadString(ref string line)
	{
		line = _reader.ReadLine();
		return line != null;
	}

	public void Close()
	{
		_reader.Close();
	}

	public int GetLength()
	{
		return (int)new FileInfo(_filename).Length;
	}
}
