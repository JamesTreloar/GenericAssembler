namespace GenericAssembler; 

internal abstract class Program {

	public static void Main(string[] args) {
		ErrorValue ev;
		if (args.Length != 2) {
			ev = new(ErrorNumbers.InvalidOptions);
			ev.DisplayError();
			return;
		}

		string jsonStringConfig = File.ReadAllText(args[0]); 
		
		SettingReader settingReader = new(jsonStringConfig);
		
		(Configuration? configuration, ev) = settingReader.Read();
		if (!ev.IsOkay() || configuration == null) {
			ev.DisplayError();
			return;
		}

		string[] input = File.ReadAllLines(args[1]);
		ProcessFile processFile = new(configuration);
		(List<string>? result, ev) = processFile.Run(input);
		if (!ev.IsOkay() || result == null) {
			ev.DisplayError();
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
