using System.Collections.Generic;
using System.IO;

namespace SIMBCD;

public class ProfileStringReader
{
	public class Section
	{
		public string name;

		public Dictionary<string, string> data;

		public Section(string name_)
		{
			name = name_;
			data = new Dictionary<string, string>();
		}
	}

	private List<Section> _sections;

	public ProfileStringReader(string filename)
	{
		_sections = new List<Section>();
		Section section = new Section("");
		StreamReader streamReader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
		for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
		{
			if (IsKeyValueLine(text, out var key, out var value))
			{
				section.data.Add(key, value);
			}
			else if (IsSectionStart(text, out value))
			{
				_sections.Add(section);
				section = new Section(value);
			}
		}
		streamReader.Close();
		_sections.Add(section);
	}

	private static bool IsKeyValueLine(string line, out string key, out string value)
	{
		int num = line.IndexOf('=');
		if (num > 0)
		{
			key = line.Substring(0, num).Trim();
			value = line.Substring(num + 1).Trim();
			return true;
		}
		key = (value = null);
		return false;
	}

	private static bool IsSectionStart(string line, out string name)
	{
		int num = line.IndexOf('[');
		int num2 = line.IndexOf(']');
		if (num >= 0 && num2 > num)
		{
			name = line.Substring(num + 1, num2 - num - 1);
			return true;
		}
		name = null;
		return false;
	}

	public void GetPrivateProfileString(string secname, string key, string defval, out string result)
	{
		Section section = FindSection(secname);
		if (section == null || !section.data.TryGetValue(key, out result))
		{
			result = defval;
		}
	}

	private Section FindSection(string secname)
	{
		foreach (Section section in _sections)
		{
			if (section.name == secname)
			{
				return section;
			}
		}
		return null;
	}

	public uint GetPrivateProfileInt(string secname, string key, uint defval)
	{
		GetPrivateProfileString(secname, key, "", out var result);
		if (!string.IsNullOrEmpty(result) && uint.TryParse(result, out var result2))
		{
			return result2;
		}
		return defval;
	}
}
