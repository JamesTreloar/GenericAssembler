using System.Text.Json;
using System.Text.Json.Nodes;

namespace GenericAssembler; 

public class SettingReader(string fileName) {
	public Configuration? Read() {
		string jsonString = File.ReadAllText(fileName);
		JsonNode node;
		Configuration? config;
		try {
			node = JsonNode.Parse(jsonString)!;

			config = JsonSerializer.Deserialize<Configuration>(jsonString);
		} catch (Exception e) {
			Console.WriteLine("ERROR");
			Console.WriteLine("Invalid json provided");
			return null;
		}

		if (config == null) {
			Console.WriteLine("ERROR");
			Console.WriteLine("Invalid json provided");
			return null;
		} 

		JsonArray instructions = node!["Instructions"]!.AsArray();
		if (instructions.Count < 1) {
			Console.WriteLine("ERROR");
			Console.WriteLine("No instructions were provided in the json input.");
			return null;
		}

		int instructionLen = config.InstructionLength;
		int rSum = config.OpCodeLength + config.RegisterLength * 3 + config.FunctLength + config.ShamtLength;
		if (rSum != instructionLen) {
			Console.WriteLine("ERROR");
			Console.WriteLine("Invalid lengths for r-type instructions.");
			Console.WriteLine("Instruction length needs to be equal to length for opcode, 3 registers, shamt, and funct");
			Console.WriteLine($"Currently instruction length is {instructionLen} and calculated length is {rSum}");
			return null;
		}

		int iSum = config.OpCodeLength + config.RegisterLength * 2 + config.ImmediateLength;
		if (iSum != config.InstructionLength) {
			Console.WriteLine("ERROR");
			Console.WriteLine("Invalid lengths for i-type instructions.");
			Console.WriteLine("Instruction length needs to be equal to length for opcode, 2 registers, and immediate value");
			Console.WriteLine($"Currently instruction length is {instructionLen} and calculated length is {iSum}");
			return null;
		}

		int jSum = config.OpCodeLength + config.AddressLength;
		if (jSum != config.InstructionLength) {
			Console.WriteLine("ERROR");
			Console.WriteLine("Invalid lengths for j-type instructions.");
			Console.WriteLine("Instruction length needs to be equal to length for opcode, and address");
			Console.WriteLine($"Currently instruction length is {instructionLen} and calculated length is {jSum}");
			return null;
		}
		
		foreach (JsonNode jsonNode in instructions) {
			Instruction add;
			switch (jsonNode!["Format"]!.ToString()) {
				case "R":
					add = new(jsonNode!["Nemonic"]!.ToString(), InstructionFormat.R,
						(int)jsonNode!["OpCode"]!, (int)jsonNode!["Shamt"]!, 
						(int)jsonNode!["Funct"]!);
					config.Instructions.Add(add);
					break;
				case "RShift":
					add = new(jsonNode!["Nemonic"]!.ToString(), InstructionFormat.RShift,
						// the instruction will have a value for shamt which is used over the 0 here
						(int)jsonNode!["OpCode"]!, 0, 
						(int)jsonNode!["Funct"]!);
					config.Instructions.Add(add);
					break;
				case "RSingle":
					add = new(jsonNode!["Nemonic"]!.ToString(), InstructionFormat.RSingle,
						(int)jsonNode!["OpCode"]!, (int)jsonNode!["Shamt"]!, 
						(int)jsonNode!["Funct"]!);
					config.Instructions.Add(add);
					break;
				case "I":
					add = new(jsonNode!["Nemonic"]!.ToString(), InstructionFormat.I,
						(int)jsonNode!["OpCode"]!);
					config.Instructions.Add(add);
					break;
				case "IMem":
					add = new(jsonNode!["Nemonic"]!.ToString(), InstructionFormat.IMem,
						(int)jsonNode!["OpCode"]!);
					config.Instructions.Add(add);
					break;
				case "ISingle":
					add = new(jsonNode!["Nemonic"]!.ToString(), InstructionFormat.ISingle,
						(int)jsonNode!["OpCode"]!);
					config.Instructions.Add(add);
					break;
				case "J":
					add = new(jsonNode!["Nemonic"]!.ToString(), InstructionFormat.J,
						(int)jsonNode!["OpCode"]!);
					config.Instructions.Add(add);
					break;
				default:
					Console.WriteLine("ERROR");
					Console.WriteLine($"Invalid instruction format ({jsonNode!["Format"]!}) " +
					          $"encountered on the {jsonNode.GetElementIndex()}th instruction definition.");
					return null;
			}
		}
		
		return config;
	}
}