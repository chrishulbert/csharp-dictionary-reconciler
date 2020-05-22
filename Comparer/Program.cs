using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Comparer
{
  class Program
  {
    static void Main(string[] args)
    {
      // This compares the contents of two files to find any records 
      // that are in one file but not in the other file.
      // Note that the order of the records in the files is immaterial.

      // A positive value in the integer part of the dictionary
      // signifies that the record is found in file A
      // A negative value means file B
      Dictionary<string, int> Comparer = new Dictionary<string, int>();
      string line;
      int records=0; // only used for progress reporting

      // Load the first file into the dictionary
      Console.WriteLine("Loading file A");
      using (StreamReader sr = new StreamReader("SmallFileA.txt"))
      {
        while (sr.Peek() >= 0)
        {
          // Progress reporting
          if (++records % 10000 == 0)
            Console.Write("{0}%    \r", sr.BaseStream.Position * 100 / sr.BaseStream.Length);

          line = sr.ReadLine();
          if (Comparer.ContainsKey(line))
            Comparer[line]++;
          else
            Comparer[line] = 1;
        }
      }
      Console.WriteLine("Loaded");

      // Load the second file, hopefully zeroing out the dictionary values
      Console.WriteLine("Loading file B");
      using (StreamReader sr = new StreamReader("SmallFileB.txt"))
      {
        while (sr.Peek() >= 0)
        {
          // Progress reporting
          if (++records % 10000 == 0)
            Console.Write("{0}%    \r", sr.BaseStream.Position * 100 / sr.BaseStream.Length);

          line = sr.ReadLine();
          if (Comparer.ContainsKey(line))
            Comparer[line]--;
          else
            Comparer[line] = -1;
        }
      }
      Console.WriteLine("Loaded");

      // List any mismatches
      int mismatches = 0;
      foreach (KeyValuePair<string, int> kvp in Comparer)
      {
        if (kvp.Value != 0)
        {
          mismatches++;
          string InWhich = kvp.Value > 0 ? "A" : "B";
          Console.Write("Extra value '{0}' found in file {1}", kvp.Key, InWhich);
          if (Math.Abs(kvp.Value) != 1) Console.Write(" ({0} times)", Math.Abs(kvp.Value));
          Console.WriteLine();
        }
      }
      if (mismatches == 0)
        Console.WriteLine("No mismatches found");

      // How much ram did this use?
      Console.WriteLine("Used {0} MB of memory (private bytes) to compare {1} records",
        System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64/1024/1024,
        records);

      // Free the memory to the GC explicitly in case you use this in other code
      // This isn't essential, it just returns the memory faster in my tests.
      Comparer.Clear();
      Comparer = null;
    }
  }
}
