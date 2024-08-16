using System.Text;
using System.Text.Json;

namespace ConsoleApp1
{
    public class Dictionary1
    {
        private const string DictionariesFolder = "Dictionaries";

        public string Name { get; set; }
        public Dictionary<string, List<string>> Words { get; set; }

        public Dictionary1(string name)
        {
            Name = name;
            Words = new Dictionary<string, List<string>>();
        }

        public void AddWord(string word, List<string> translations)
        {
            try
            {
                if (Words.ContainsKey(word))
                {
                    foreach (var translation in translations)
                    {
                        if (!Words[word].Contains(translation))
                        {
                            Words[word].Add(translation);
                        }
                    }
                }
                else
                {
                    Words[word] = new List<string>(translations);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding word: {ex.Message}");
            }
        }

        public void ReplaceWord(string oldWord, string newWord)
        {
            try
            {
                if (Words.ContainsKey(oldWord))
                {
                    List<string> translations = Words[oldWord];
                    Words.Remove(oldWord);
                    Words[newWord] = new List<string>(translations);
                }
                else
                {
                    Console.WriteLine("The specified word does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while replacing word: {ex.Message}");
            }
        }

        public void ReplaceTranslation(string word, string oldTranslation, string newTranslation)
        {
            try
            {
                if (Words.ContainsKey(word))
                {
                    var translations = Words[word];
                    if (translations.Contains(oldTranslation))
                    {
                        translations.Remove(oldTranslation);
                        translations.Add(newTranslation);
                    }
                    else
                    {
                        Console.WriteLine("The specified translation does not exist.");
                    }
                }
                else
                {
                    Console.WriteLine("The specified word does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while replacing translation: {ex.Message}");
            }
        }

        public void DeleteWord(string word)
        {
            try
            {
                if (Words.ContainsKey(word))
                {
                    Words.Remove(word);
                }
                else
                {
                    Console.WriteLine("The specified word does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting word: {ex.Message}");
            }
        }

        public void DeleteTranslation(string word, string translation)
        {
            try
            {
                if (Words.ContainsKey(word))
                {
                    var translations = Words[word];
                    if (translations.Contains(translation))
                    {
                        translations.Remove(translation);
                    }
                    else
                    {
                        Console.WriteLine("The specified translation does not exist.");
                    }
                }
                else
                {
                    Console.WriteLine("The specified word does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting translation: {ex.Message}");
            }
        }

        public void ShowAllDictionary()
        {
            try
            {
                if (Words.Count == 0)
                {
                    Console.WriteLine("Dictionary is empty");
                    return;
                }

                Console.WriteLine($"Dictionary: {Name}");
                Console.WriteLine("------------------------------");

                foreach (var word in Words)
                {
                    Console.WriteLine($"Word: {word.Key}");

                    for (int i = 0; i < word.Value.Count; i++)
                    {
                        Console.WriteLine($"\tTranslation {i + 1}: {word.Value[i]}");
                    }
                    Console.WriteLine("------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while showing dictionary: {ex.Message}");
            }
        }

        public List<string> ShowAllTranslationToWord(string word)
        {
            try
            {
                if (Words.ContainsKey(word))
                {
                    return Words[word];
                }
                else
                {
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while showing translations: {ex.Message}");
                return new List<string>();
            }
        }

        public void ExportWordToFile(string word)
        {
            try
            {
                if (Words.ContainsKey(word))
                {
                    string fileName = $"{Name.Replace(" ", "_")}_dictionary.json";
                    Dictionary<string, List<string>> exportData;

                    if (File.Exists(fileName))
                    {
                        string existingJson = File.ReadAllText(fileName);
                        exportData = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(existingJson);
                    }
                    else
                    {
                        exportData = new Dictionary<string, List<string>>();
                    }

                    if (exportData.ContainsKey(word))
                    {
                        foreach (var translation in Words[word])
                        {
                            if (!exportData[word].Contains(translation))
                            {
                                exportData[word].Add(translation);
                            }
                        }
                    }
                    else
                    {
                        exportData[word] = Words[word];
                    }

                    string json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(fileName, json);

                    Console.WriteLine($"Word '{word}' successfully exported to file {fileName}");
                }
                else
                {
                    Console.WriteLine("The specified word does not exist in the dictionary.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while exporting word: {ex.Message}");
            }
        }

        public void ExportDictionaryToFile()
        {
            try
            {
                if (!Directory.Exists(DictionariesFolder))
                {
                    Directory.CreateDirectory(DictionariesFolder);
                }

                string fileName = Path.Combine(DictionariesFolder, $"{Name.Replace(" ", "_")}_dictionary.json");
                Dictionary<string, List<string>> exportData;

                if (File.Exists(fileName))
                {
                    string existingJson = File.ReadAllText(fileName);
                    exportData = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(existingJson);
                }
                else
                {
                    exportData = new Dictionary<string, List<string>>();
                }

                foreach (var entry in Words)
                {
                    exportData[entry.Key] = entry.Value;
                }

                string json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, json);

                Console.WriteLine($"Dictionary '{Name}' successfully exported to file {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while exporting dictionary: {ex.Message}");
            }
        }
    }

    public class Menu
    {
        private Dictionary<string, Dictionary1> dictionaries = new Dictionary<string, Dictionary1>();
        private const string DictionariesFolder = "Dictionaries";

        public void Show()
        {
            try
            {
                ImportAllDictionaries();

                while (true)
                {
                    Console.WriteLine("=== DICTIONARY MENU ===");
                    Console.WriteLine("1. Create a new dictionary");
                    Console.WriteLine("2. Add a word and translation");
                    Console.WriteLine("3. Find translation of a word");
                    Console.WriteLine("4. Export word to file");
                    Console.WriteLine("5. Export entire dictionary to file");
                    Console.WriteLine("6. Show all words in dictionary");
                    Console.WriteLine("8. Delete word from dictionary");
                    Console.WriteLine("9. Delete translation from dictionary");
                    Console.WriteLine("10. Replace word in dictionary");
                    Console.WriteLine("11. Replace translation in dictionary");
                    Console.WriteLine("7. Exit");

                    Console.Write("Choose an option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            CreateDictionary();
                            break;
                        case "2":
                            Dictionary1 selectedDictionary = SelectDictionary();
                            if (selectedDictionary != null)
                            {
                                AddWord(selectedDictionary);
                            }
                            break;
                        case "3":
                            selectedDictionary = SelectDictionary();
                            if (selectedDictionary != null)
                            {
                                FindTranslation(selectedDictionary);
                            }
                            break;
                        case "4":
                            selectedDictionary = SelectDictionary();
                            if (selectedDictionary != null)
                            {
                                ExportWord(selectedDictionary);
                            }
                            break;
                        case "5":
                            selectedDictionary = SelectDictionary();
                            if (selectedDictionary != null)
                            {
                                ExportDictionary(selectedDictionary);
                            }
                            break;
                        case "6":
                            selectedDictionary = SelectDictionary();
                            if (selectedDictionary != null)
                            {
                                ShowAllWords(selectedDictionary);
                            }
                            break;
                        case "7":
                            Console.WriteLine("Exiting program.");
                            return;
                        case "8":
                            Console.WriteLine("Enter a word to delete:");
                            string word = Console.ReadLine();
                            selectedDictionary = SelectDictionary();
                            if (selectedDictionary != null && word != null)
                            {
                                selectedDictionary.DeleteWord(word);
                            }
                            break;
                        case "9":
                            selectedDictionary = SelectDictionary();
                            Console.WriteLine("Enter a word from which translation must be deleted:");
                            string word1 = Console.ReadLine();
                            Console.WriteLine("Enter a translation to delete:");
                            string word2 = Console.ReadLine();

                            if (selectedDictionary != null && selectedDictionary.Words.ContainsKey(word1) && selectedDictionary.Words[word1].Contains(word2))
                            {
                                selectedDictionary.DeleteTranslation(word1, word2);
                            }
                            else
                            {
                                Console.WriteLine("The specified word or translation does not exist in the dictionary.");
                            }
                            break;
                        case "10":
                            selectedDictionary = SelectDictionary();
                            Console.WriteLine("Enter a word to replace:");
                            string oldWord = Console.ReadLine();
                            Console.WriteLine("Enter a new word:");
                            string newWord = Console.ReadLine();

                            if (selectedDictionary != null && oldWord != null && newWord != null)
                            if (selectedDictionary != null && oldWord != null && newWord != null)
                                {
                                    selectedDictionary.ReplaceWord(oldWord, newWord);
                                }
                                else
                                {
                                    Console.WriteLine("The specified word or new word is invalid.");
                                }
                            break;
                        case "11":
                            selectedDictionary = SelectDictionary();
                            Console.WriteLine("Enter a word to modify its translation:");
                            string wordToModify = Console.ReadLine();
                            Console.WriteLine("Enter the old translation:");
                            string oldTranslation = Console.ReadLine();
                            Console.WriteLine("Enter the new translation:");
                            string newTranslation = Console.ReadLine();

                            if (selectedDictionary != null && wordToModify != null && oldTranslation != null && newTranslation != null)
                            {
                                selectedDictionary.ReplaceTranslation(wordToModify, oldTranslation, newTranslation);
                            }
                            else
                            {
                                Console.WriteLine("The specified word, old translation, or new translation is invalid.");
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please choose a valid option from the menu.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in the main menu loop: {ex.Message}");
            }
        }

        private void CreateDictionary()
        {
            try
            {
                Console.Write("Enter the name of the new dictionary: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Dictionary name cannot be empty.");
                    return;
                }

                if (!dictionaries.ContainsKey(name))
                {
                    dictionaries[name] = new Dictionary1(name);
                    Console.WriteLine($"Dictionary '{name}' created successfully.");
                }
                else
                {
                    Console.WriteLine("A dictionary with this name already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating dictionary: {ex.Message}");
            }
        }

        private void AddWord(Dictionary1 dictionary)
        {
            try
            {
                Console.Write("Enter the word to add: ");
                string word = Console.ReadLine();
                Console.WriteLine("Enter translations (comma-separated):");
                string translationsInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(word) && !string.IsNullOrWhiteSpace(translationsInput))
                {
                    List<string> translations = translationsInput.Split(',').Select(t => t.Trim()).ToList();
                    dictionary.AddWord(word, translations);
                    Console.WriteLine($"Word '{word}' with translations added successfully.");
                }
                else
                {
                    Console.WriteLine("Word or translations cannot be empty.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding word: {ex.Message}");
            }
        }

        private void FindTranslation(Dictionary1 dictionary)
        {
            try
            {
                Console.Write("Enter the word to find its translations: ");
                string word = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(word))
                {
                    Console.WriteLine("Word cannot be empty.");
                    return;
                }

                var translations = dictionary.ShowAllTranslationToWord(word);
                if (translations.Count > 0)
                {
                    Console.WriteLine($"Translations for '{word}': {string.Join(", ", translations)}");
                }
                else
                {
                    Console.WriteLine($"No translations found for the word '{word}'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while finding translation: {ex.Message}");
            }
        }

        private void ExportWord(Dictionary1 dictionary)
        {
            try
            {
                Console.Write("Enter the word to export: ");
                string word = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(word))
                {
                    Console.WriteLine("Word cannot be empty.");
                    return;
                }

                dictionary.ExportWordToFile(word);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while exporting word: {ex.Message}");
            }
        }

        private void ExportDictionary(Dictionary1 dictionary)
        {
            try
            {
                dictionary.ExportDictionaryToFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while exporting dictionary: {ex.Message}");
            }
        }

        private void ShowAllWords(Dictionary1 dictionary)
        {
            try
            {
                dictionary.ShowAllDictionary();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while showing all words: {ex.Message}");
            }
        }

        private Dictionary1 SelectDictionary()
        {
            try
            {
                if (dictionaries.Count == 0)
                {
                    Console.WriteLine("No dictionaries available. Please create a dictionary first.");
                    return null;
                }

                Console.WriteLine("Available dictionaries:");
                foreach (var dictionary in dictionaries.Keys)
                {
                    Console.WriteLine($"- {dictionary}");
                }

                Console.Write("Enter the name of the dictionary: ");
                string dictionaryName = Console.ReadLine();

                if (dictionaries.TryGetValue(dictionaryName, out Dictionary1 selectedDictionary))
                {
                    return selectedDictionary;
                }
                else
                {
                    Console.WriteLine("The specified dictionary does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while selecting dictionary: {ex.Message}");
                return null;
            }
        }

        private void ImportAllDictionaries()
        {
            try
            {
                if (Directory.Exists(DictionariesFolder))
                {
                    foreach (var file in Directory.GetFiles(DictionariesFolder, "*_dictionary.json"))
                    {
                        string dictionaryName = Path.GetFileNameWithoutExtension(file).Replace("_dictionary", "");
                        string json = File.ReadAllText(file);
                        var words = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
                        if (words != null)
                        {
                            dictionaries[dictionaryName] = new Dictionary1(dictionaryName) { Words = words };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while importing dictionaries: {ex.Message}");
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            var menu = new Menu();
            menu.Show();
        }
    }
}
