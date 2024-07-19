namespace GenericAssembler.Tests;

[TestFixture]
[TestOf(typeof(SettingReader))]
public class SettingReaderTest {
	private const string PrePath = "../../../Resources/Configs/";
	
	[Test]
	public void ConfigSimple() {
		string configInput = File.ReadAllText(PrePath + "configBasic.json");
		SettingReader settingReader = new(configInput);
		(Configuration? configuration, ErrorValue ev) = settingReader.Read();

        Assert.Multiple(() => {
            Assert.That(ev.IsOkay);
            Assert.That(configuration, Is.Not.Null);
        });

        Configuration expectedConfig = new() {
	        InstructionLength = 16,
	        OpCodeLength = 4,
	        RegisterLength = 4,
	        ImmediateLength = 4,
	        AddressLength = 12,
	        ShamtLength = 0,
	        FunctLength = 0
        };

        List<Instruction> expectedInstructions = new () { 
			new ("add", InstructionFormat.R, 1, 0, 0),
			new ("sub", InstructionFormat.R, 2, 0, 0),
			new ("addi", InstructionFormat.I, 3),
			new ("jump", InstructionFormat.J, 4),
			new ("lw", InstructionFormat.IMem, 5),
			new ("slr", InstructionFormat.RShift, 6, 0, 0),
			new ("lui", InstructionFormat.ISingle, 7),
			new ("jr", InstructionFormat.RSingle, 8, 0, 0),
		};
		expectedConfig.Instructions = expectedInstructions;
		
		AreConfigurationsEqual(expectedConfig, configuration, regMap:false);
	}

	[Test]
	public void ConfigWithRegMap() {
		string configInput = File.ReadAllText(PrePath + "configRegMap.json");
		SettingReader settingReader = new(configInput);
		(Configuration? configuration, ErrorValue ev) = settingReader.Read();

		Assert.Multiple(() => {
			Assert.That(ev.IsOkay);
			Assert.That(configuration, Is.Not.Null);
		});

		Configuration expectedConfig = new() {
			InstructionLength = 16,
			OpCodeLength = 4,
			RegisterLength = 4,
			ImmediateLength = 4,
			AddressLength = 12,
			ShamtLength = 0,
			FunctLength = 0
		};

		List<Instruction> expectedInstructions = new () { 
			new ("add", InstructionFormat.R, 1, 0, 0),
			new ("sub", InstructionFormat.R, 2, 0, 0),
			new ("addi", InstructionFormat.I, 3),
			new ("jump", InstructionFormat.J, 4),
			new ("lw", InstructionFormat.IMem, 5),
			new ("slr", InstructionFormat.RShift, 6, 0, 0),
			new ("lui", InstructionFormat.ISingle, 7),
			new ("jr", InstructionFormat.RSingle, 8, 0, 0),
		};
		expectedConfig.Instructions = expectedInstructions;

		Dictionary<string, int> regMap = new() {
			{ "t0", 1 },
			{ "t1", 2 },
			{ "t2", 3 },
			{ "s0", 4 },
			{ "s1", 5 },
			{ "s2", 6 },
			{ "s3", 7 },
			{ "s4", 8 },
			{ "a1", 9 },
			{ "a2", 10 },
			{ "a3", 11 },
			{ "a4", 12 }
		};

		expectedConfig.RegisterMap = regMap;

		AreConfigurationsEqual(configuration, expectedConfig);
	}

	private static void AreConfigurationsEqual(Configuration expected, Configuration actual, bool lengths = true, bool instructions = true, bool regMap = true) {
		if (lengths) {
			Assert.Multiple(() => {
				Assert.That(expected.InstructionLength, Is.EqualTo(actual.InstructionLength));
				Assert.That(expected.OpCodeLength, Is.EqualTo(actual.OpCodeLength));
				Assert.That(expected.RegisterLength, Is.EqualTo(actual.OpCodeLength));
				Assert.That(expected.ImmediateLength, Is.EqualTo(actual.ImmediateLength));
				Assert.That(expected.AddressLength, Is.EqualTo(actual.AddressLength));
				Assert.That(expected.ShamtLength, Is.EqualTo(actual.ShamtLength));
				Assert.That(expected.FunctLength, Is.EqualTo(actual.FunctLength));
			});
		}

		if (instructions) {
			Assert.That(expected.Instructions, Is.EquivalentTo(actual.Instructions));
		}

		if (regMap) {
			Assert.That(expected.RegisterMap, Is.EquivalentTo(actual.RegisterMap));
		}
	} 
}