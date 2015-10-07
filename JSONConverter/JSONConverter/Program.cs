using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JSONConverter {
    class Program {
        static void Main(string[] args) {
            int[] initialArray = null;
            int[][] result = null;
            int columns = 0;
            int rows = 0;
            Console.WriteLine("Please enter file location: ");
            string loadPath = Console.ReadLine();
            if (File.Exists(loadPath)) {
                Console.WriteLine("Loading data...");
                using (TextReader reader = File.OpenText(loadPath)) {
                    Console.Write("What is the width of your array? ");
                    columns = System.Convert.ToInt32(Console.ReadLine());
                    Console.Write("What is the height of your array? ");
                    rows = System.Convert.ToInt32(Console.ReadLine());

                    initialArray = new int[rows*columns];
                    string content = reader.ReadLine();
                    do {
                        string[] contents = content.Split(',');
                        for (int i = 0; i < contents.Length; i++) {
                            initialArray[i] = System.Convert.ToInt32(contents[i]);
                        }
                        content = reader.ReadLine();
                    } while (content != null);
                    Console.WriteLine("Copying of array completed");

                    Console.WriteLine("Initializing 2D Array...");
                    result = new int[rows][];
                    for (int i = 0; i < result.Length; i++) {
                        result[i] = new int[columns];
                    }

                    Console.Write("Assigning 2D Array...");
                    for (int y = 0; y < result.Length; y++) {
                        for (int x = 0; x < result[y].Length; x++) {
                            result[y][x] = initialArray[y * columns + x];
                        }
                    }

                    Console.WriteLine("\nWhere would you like to save the file?");
                    string savePath = Console.ReadLine();
                    using(TextWriter writer = File.CreateText(savePath)) {
                        Console.WriteLine("Writing data...");
                        for (int y = 0; y < result.Length; y++) {
                            for(int x = 0; x < result[y].Length; x++) {
                                writer.Write(result[y][x]);
                                if (x != result[y].Length - 1) {
                                    writer.Write(",");
                                }
                            }
                            writer.Write(writer.NewLine);

                        }
                        Console.Write("Finished saving data...");
                    }
                    Console.WriteLine("\nPress enter to exit");
                    Console.ReadLine();
                }
            }
            else {
                Console.WriteLine("File not found!");
                Console.ReadLine();
            }
        }
    }
}
