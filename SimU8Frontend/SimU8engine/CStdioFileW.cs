using System.IO;

namespace SimU8engine;

public class CStdioFileW
{
	private TextWriter _writer;

	public bool OpenForCreate(string filename)
	{
		try
		{
			_writer = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));
			return true;
		}
		catch (IOException)
		{
			return false;
		}
	}

	public void WriteString(string line)
	{
		_writer.Write(line);
	}

	public void Abort()
	{
	}

	public void Close()
	{
		_writer.Close();
	}

	public void Flush()
	{
		_writer.Flush();
	}
}
