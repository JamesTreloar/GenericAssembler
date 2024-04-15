using System.Text.Json;
using System.Text.Json.Nodes;

namespace GenericAssembler; 

public class SettingReader(string fileName) {
	public Configuration Read() {
		string jsonString = File.ReadAllText(fileName);
		JsonNode node = JsonNode.Parse(jsonString)!;

		Configuration? config = JsonSerializer.Deserialize<Configuration>(jsonString);
		
		if (node == null || config == null) {
			throw new JsonException("Invalid json provided.");
		}

		JsonArray instructions = node!["Instructions"]!.AsArray();
		if (instructions.Count < 1) {
			throw new("No instructions were provided in the json input.");
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
					throw new($"Invalid instruction format ({jsonNode!["Format"]!}) " +
					          $"encountered on the {jsonNode.GetElementIndex()}th instruction definition.");
			}
		}
		
		return config;
	}
}