using GenericAssembler;

namespace GenericAssembler.Tests;

[TestFixture]
[TestOf(typeof(ProcessFile))]
public class ProcessFileTest {
	private const string PrePath = "../../../Resources/";

	[Test]
	public void AssembleBasic() {
		string configInput = File.ReadAllText(PrePath + "Configs/configBasic.json");
		SettingReader settingReader = new(configInput);
		(Configuration? configuration, _) = settingReader.Read();

		Assert.That(configuration, Is.Not.Null);
		
		string[] assembly = File.ReadAllLines(PrePath + "Assembly/basic.s");
		Assert.That(assembly, Is.Not.Null);
		ProcessFile processFile = new (configuration);
		(List<string> actual, ErrorValue ev ) = processFile.Run(assembly);


		List<string> expected = new() {
			"0011000000010010",
			"0011000000100011",
			"0011000001000101",
			"0001000100100011",
			"0010010000100101",
			"0100000000101000",
			"0101001000011100",
			"0111000011001111",
			"1000110000000000",
		};
		
		Assert.That(ev.IsOkay);
		
		Assert.That(actual, Is.EqualTo(expected));
	}
	
	[Test]
	public void AssembleRegMap() {
		string configInput = File.ReadAllText(PrePath + "Configs/configRegMap.json");
		SettingReader settingReader = new(configInput);
		(Configuration? configuration, _) = settingReader.Read();

		Assert.That(configuration, Is.Not.Null);
		
		string[] assembly = File.ReadAllLines(PrePath + "Assembly/regMap.s");
		Assert.That(assembly, Is.Not.Null);
		ProcessFile processFile = new (configuration);
		(List<string> actual, ErrorValue ev ) = processFile.Run(assembly);


		List<string> expected = new() {
			"0011000101010010",
			"0011000101100011",
			"0011000110000101",
			"0001100100101010",
			"0010101100101100",
			"0100000000101000",
			"0101001100101100",
			"0111000010101111",
			"1000101100000000",
		};
		
		Assert.That(ev.IsOkay);
		
		Assert.That(actual, Is.EqualTo(expected));
	}

	[Test]
	public void AssembleLabel() {
		string configInput = File.ReadAllText(PrePath + "Configs/configLabel.json");
		SettingReader settingReader = new(configInput);
		(Configuration? configuration, _) = settingReader.Read();
		
		Assert.That(configuration, Is.Not.Null);
		
		string[] assembly = File.ReadAllLines(PrePath + "Assembly/label.s");
		Assert.That(assembly, Is.Not.Null);
		ProcessFile processFile = new (configuration);
		(List<string> actual, ErrorValue ev ) = processFile.Run(assembly);

		List<string> expected = new() {
			"0011000000010001",
			"0011000000101010",
			"1001000100100010",
			"0011000100010001",
			"0100111111111101"
		};
		
		Assert.That(ev.IsOkay);
		
		Assert.That(actual, Is.EqualTo(expected));
	}
}