using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TCS_OpenSource
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
			              "\\Saved Games\\Frontier Developments\\Elite Dangerous\\";

			if (!Directory.Exists(path))
			{
				Console.WriteLine("It appears you have no Elite: Dangerous log files.");
				return;
			}
			
			Dictionary<ThargoidType, int> kills = new Dictionary<ThargoidType, int>();


			ThargoidType[] thargoidTypes = (ThargoidType[]) Enum.GetValues(typeof(ThargoidType));

			foreach (var type in thargoidTypes)
				kills.Add(type, 0);

			bool currentSessionFailure = false;

			foreach (string file in Directory.EnumerateFiles(path, "*.log", SearchOption.AllDirectories))
			{
				string line;
				StreamReader reader;
				try
				{
					reader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read), Encoding.UTF8);
				}
				catch (IOException e)
				{
					//Console.WriteLine($"{file} in use.");
					currentSessionFailure = true;
					continue;
				}

				while ((line = reader.ReadLine()) != null)
				{
					
					foreach (var type in thargoidTypes)
					{
						if (line.Contains("\"") && line.Contains("Reward") && line.Contains("\"") &&
						    line.Contains($":{(int) type},") && line.Contains("AwardingFaction"))
						{
							kills[type]++;
						}
					}
					
				}
			}

			if (currentSessionFailure)
			{
				Console.ForegroundColor = ConsoleColor.Red;

				Console.WriteLine($"Kills from current play session may not appear.");
				Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine();
			}

			Console.ForegroundColor = ConsoleColor.Blue;
			
			foreach (var pair in kills)
			{
				Console.WriteLine(pair.Key + ": " + pair.Value);
			}
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"Total Kills: {kills.Values.Aggregate(0, (total, typeKills) => total + typeKills)}");
			
			Console.WriteLine("Press any key to continue.");
			Console.ReadKey();
		}
	}


	public enum ThargoidType
	{
		Scout = 10000,
		Cyclops = 2000000,
		Basillisk = 6000000,
		Medusa = 10000000,
		Hydra = 15000000
	}
}