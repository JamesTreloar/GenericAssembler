using System.Globalization;

namespace GenericAssembler;

public class Utils {
	public static bool TryIntParse(string s, out int result) {
		bool success;
		if (s[0] == '0' && s.Length > 1 && s[1] is 'x' or 'b') {
			if (s[1] == 'x') {
				success = int.TryParse(s[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
				return success;
			} 
			if (s[1] == 'b') {
				success = int.TryParse(s[2..], NumberStyles.BinaryNumber, CultureInfo.InvariantCulture, out result);
				return success;
			}
		}

		success = int.TryParse(s, out result);
		return success;
	}

	public static string BinaryStringConvert(int num, int length) {
		if (num >= 0) {
			return Convert.ToString(num, 2).PadLeft(length, '0');
		}

		int start = 1 << length;
		start += num;
		return Convert.ToString(start, 2).PadLeft(length, '0');
	}
}
