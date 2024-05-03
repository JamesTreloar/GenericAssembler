namespace GenericAssembler; 

internal abstract class Program {

	public static void Main(string[] args) {
		if (args.Length != 2) {
			Console.WriteLine("Error:");
			Console.WriteLine("USAGE: ./GenericAssembler schema.json input.s");
			return;
		}
		SettingReader settingReader = new(args[0]);
		Configuration? configuration = settingReader.Read();
		if (configuration == null) {
			return;
		}

		string[] input = File.ReadAllLines(args[1]);
		ProcessFile processFile = new(configuration);
		List<string>? result = processFile.Run(input);
		if (result == null) {
			return;
		}

		Console.WriteLine("Binary representation: ");
		foreach (string s in result) {
			Console.WriteLine(s);
		}
		
		Console.WriteLine("Hex Representation: ");
		foreach (string s in result) {
			Console.WriteLine("x" + Convert.ToInt32(s, 2).ToString("X"));
		}
	}
}
