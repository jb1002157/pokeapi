using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PokeAPIApp
{
// Classes to map the JSON structure from PokeAPI
public class PokemonSprites
{
[JsonProperty("front_default")]
public string FrontDefault { get; set; }
}

public class PokemonType
{
[JsonProperty("type")]
public TypeInfo Type { get; set; }
}

public class TypeInfo
{
[JsonProperty("name")]
public string Name { get; set; }
}

public class Pokemon
{
[JsonProperty("name")]
public string Name { get; set; }

[JsonProperty("height")]
public int Height { get; set; } // In decimeters

[JsonProperty("weight")]
public int Weight { get; set; } // In hectograms

[JsonProperty("sprites")]
public PokemonSprites Sprites { get; set; }

[JsonProperty("types")]
public List<PokemonType>
	Types { get; set; }

	// Display formatted properties
	public string DisplayHeight => $"{Height / 10.0}m";
	public string DisplayWeight => $"{Weight / 10.0}kg";
	public string DisplayTypes => string.Join(", ", Types?.ConvertAll(t => t.Type.Name) ?? new List<string>
		());
		}

		class Program
		{
		private static readonly HttpClient client = new HttpClient();
		private static List<Pokemon>
			pokemonList = new List<Pokemon>
				();

				static async Task Main(string[] args)
				{
				Console.WriteLine("PokeAPI Pokemon Finder");
				Console.WriteLine("----------------------");

				while (true)
				{
				Console.Write("\nEnter Pokemon name or ID (or 'quit' to exit): ");
				string input = Console.ReadLine()?.Trim();

				if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
				{
				break;
				}

				if (string.IsNullOrEmpty(input))
				{
				Console.WriteLine("Please enter a valid Pokemon name or ID.");
				continue;
				}

				try
				{
				var pokemon = await GetPokemonAsync(input);
				if (pokemon != null)
				{
				pokemonList.Add(pokemon);
				DisplayPokemonDetails(pokemon);
				DisplayPokemonList();
				}
				}
				catch (Exception ex)
				{
				Console.WriteLine($"Error: {ex.Message}");
				}
				}

				Console.WriteLine("\nThank you for using the PokeAPI Pokemon Finder!");
				}

				static async Task<Pokemon>
					GetPokemonAsync(string nameOrId)
					{
					try
					{
					string url = $"https://pokeapi.co/api/v2/pokemon/{nameOrId.ToLower()}";
					HttpResponseMessage response = await client.GetAsync(url);

					if (!response.IsSuccessStatusCode)
					{
					throw new Exception($"Pokemon not found: {nameOrId}. Status code: {response.StatusCode}");
					}

					string json = await response.Content.ReadAsStringAsync();
					Pokemon pokemon = JsonConvert.DeserializeObject<Pokemon>
						(json);

						return pokemon;
						}
						catch (HttpRequestException ex)
						{
						throw new Exception($"Network error: {ex.Message}");
						}
						catch (JsonException ex)
						{
						throw new Exception($"Invalid data received: {ex.Message}");
						}
						}

						static void DisplayPokemonDetails(Pokemon pokemon)
						{
						Console.WriteLine("\nPokemon Details:");
						Console.WriteLine($"Name: {pokemon.Name}");
						Console.WriteLine($"Height: {pokemon.DisplayHeight}");
						Console.WriteLine($"Weight: {pokemon.DisplayWeight}");
						Console.WriteLine($"Types: {pokemon.DisplayTypes}");
						Console.WriteLine($"Sprite URL: {pokemon.Sprites?.FrontDefault ?? "N/A"}");
						}

						static void DisplayPokemonList()
						{
						Console.WriteLine("\nPokemon You've Requested:");
						Console.WriteLine("------------------------");

						if (pokemonList.Count == 0)
						{
						Console.WriteLine("No Pokemon in your list yet.");
						return;
						}

						for (int i = 0; i < pokemonList.Count; i++)
            {
                var p = pokemonList[i];
                Console.WriteLine($"{i + 1}. {p.Name} (Height: {p.DisplayHeight}, Weight: {p.DisplayWeight}, Types: {p.DisplayTypes})");
            }
        }
    }
}