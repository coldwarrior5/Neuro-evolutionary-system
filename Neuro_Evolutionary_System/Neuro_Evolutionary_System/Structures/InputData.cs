namespace Neuro_Evolutionary_System.Structures
{
	public class InputData
	{
		public string TestDataFileName { get; private set; }
		public string SettingsFileName { get; private set; }

		public void SetTestDataFileName(string fileName)
		{
			TestDataFileName = fileName;
		}

		public void SetSettingsFileName(string fileName)
		{
			SettingsFileName = fileName;
		}
	}
}