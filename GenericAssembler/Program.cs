using CommandLine;

namespace GenericAssembler;

internal abstract class Program {
	public class Options {
		[Option(shortName: 'a', longName: "assembly", Required = false, HelpText = "Path to assembly file")]
		public string? AssemblyPath { get; init; }

		[Option(shortName: 's', longName: "specification", Required = true, HelpText = "Path to specification file")]
		public required string SpecificationPath { get; init; }

		[Option(shortName: 'o', longName: "output", Required = false, HelpText = "Path to output file")]
		public string? OutputPath { get; init; }

		[Option(shortName: 'f', longName: "format", Required = false, HelpText = "Output format, b=1; h=2")]
		public int Format { get; init; }
	}

	public static int Main(string[] args) {
		Options? options = null;
		Parser.Default.ParseArguments<Options>(args)
			.WithParsed(
				o => options = o
			).WithNotParsed(err => {
					foreach (Error error in err) {
						Console.WriteLine(error.ToString());
					}
				}
			);

		if (options == null) {
			return -1;
		}

		ErrorValue ev;

		string jsonStringConfig = File.ReadAllText(options.SpecificationPath);

		SettingReader settingReader = new(jsonStringConfig);

		(Configuration? configuration, ev) = settingReader.Read();
		if (!ev.IsOkay() || configuration == null) {
			ev.DisplayError();
			return 1;
		}

		if (options.AssemblyPath == null) {
			Console.WriteLine("Specification file is correctly formed");
			return 0;
		}

		string[] input = File.ReadAllLines(options.AssemblyPath);
		ProcessFile processFile = new(configuration);
		(List<string>? result, ev) = processFile.Run(input);
		if (!ev.IsOkay() || result == null) {
			ev.DisplayError();
			return 1;
		}

		int format = options.Format;
		if (options.OutputPath != null) {
			if ((format & 1) == 1) {
				File.WriteAllLines(options.OutputPath, result);
			}

			if ((format & 2) == 2) {
				File.WriteAllLines(options.OutputPath, result.Select(x => "x" + Convert.ToInt32(x, 2).ToString("X")));
			}

			return 0;
		}

		if ((format & 1) == 1) {
			foreach (string s in result) {
				Console.WriteLine(s);
			}
		}

		if ((format & 2) == 2) {
			foreach (string s in result) {
				Console.WriteLine("x" + Convert.ToInt32(s, 2).ToString("X"));
			}
		}

		return 0;
	}
}