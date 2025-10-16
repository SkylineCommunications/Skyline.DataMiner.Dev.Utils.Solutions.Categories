namespace Skyline.DataMiner.Utils.Categories.API.Tools
{
	public static class NameUtil
	{
		public static bool Validate(string name, out string error)
		{
			if (name == null)
			{
				error = "Name cannot be null.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(name))
			{
				error = "Name cannot be whitespace or empty.";
				return false;
			}

			if (name.Length < 1 || name.Length > 100)
			{
				error = "Name must be between 1 and 100 characters long.";
				return false;
			}

			if (char.IsWhiteSpace(name[0]) || char.IsWhiteSpace(name[name.Length - 1]))
			{
				error = "Name cannot start or end with whitespace.";
				return false;
			}

			error = null;
			return true;
		}
	}
}
