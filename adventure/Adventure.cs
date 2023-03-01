using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
//using System.Media;

namespace adventure
{
    public class SaveData
    {
        public string curHeader { get; set; }
        public string gameFile { get; set; }
        public List<Variable> vars { get; set; }

        public SaveData()
        {
            curHeader = "";
            gameFile = "game.gam";
            vars = new List<Variable>();
        }
    }

    class Global
    {
        public static Parser curParser;
        public static bool stop = false;
        public static string saveFile = "save.dat";
        public static SaveData data = new SaveData();
    }

    class Adventure
    {
        static void Main(string[] args)
        {
            Console.WriteLine("dotAdventure - Main Menu");
            Console.WriteLine("Write down the name of the game file or your save file to begin!");
            do
            {
                string fileName = Console.ReadLine();
                LoadProgress(fileName);

            } while (Global.curParser == null);
            Console.Clear();
            Game();
            Console.ReadKey();
            Console.WriteLine("-- End of File --");
            Console.ReadKey();
        }

        static void Game()
        {
            do
            {
                Global.curParser.ParseLines();
                if (Global.curParser.curLine < Global.curParser.lines.Length - 1)
                    Global.curParser.curLine++;
                else
                    Global.stop = true;
            } while (!Global.stop);
        }

        public static void SaveProgress()
        {
            Global.saveFile = Global.data.gameFile.Replace(".dat","") + ".save.dat";
            string gamedir = Directory.GetCurrentDirectory();
            string savFile = gamedir + "/" + Global.saveFile;

            string json = JsonSerializer.Serialize(Global.data);
            File.WriteAllText(savFile, json);
        }

        public static void LoadProgress(string file)
        {
            string gamedir = Directory.GetCurrentDirectory();
            string savFile = gamedir + "/" + file;
            if (!File.Exists(savFile)) return;
            string[] line = File.ReadAllLines(savFile);
            if(line.Length > 1)
            {
                Global.data.gameFile = file;
                Global.curParser = new Parser(Global.data.gameFile);
            }
            else
            {
                SaveData data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(savFile));
                Global.data = data;
                Global.curParser = new Parser(Global.data.gameFile);
                Global.curParser.JumpToHeader();
            }
        }
    }
}
