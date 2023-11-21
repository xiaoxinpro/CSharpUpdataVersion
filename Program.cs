using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpUpdataVersion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("软件版本：V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (args.Length == 0)
            {
                Console.WriteLine("Error: 需要携带参数允许。");
                return;
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("更新版本号...");
                UpdataVersionFile(args[0].Trim());
            }
            else if (args.Length == 2)
            {
                Console.WriteLine("复制版本号...");
                CopyVersionFile(args[0].Trim(), args[1].Trim());
            }

        }

        static void UpdataVersionFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine(path);
                Console.WriteLine("Error: 文件不存在。");
                return;
            }
            if (Path.GetExtension(path) == ".cs")
            {
                var lines = File.ReadAllLines(path);
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    if (lines[i].Contains("assembly: AssemblyFileVersion"))
                    {
                        UpdataVersion(ref lines[i]);
                    }
                    if (lines[i].Contains("assembly: AssemblyVersion") && !lines[i].Contains("*"))
                    {
                        UpdataVersion(ref lines[i]);
                        break;
                    }
                }
                File.WriteAllLines(path, lines);
            }
            else if (Path.GetExtension(path) == ".c" || Path.GetExtension(path) == ".h")
            {
                Encoding encoding = EncodingType.GetType(path);
                string code_data = File.ReadAllText(path, encoding);
                Regex regex_major = new Regex(@"(?<=\#define\s+((FW)|(APP))_VERSION_MAJOR\s+)\d+");
                Regex regex_minor = new Regex(@"(?<=\#define\s+((FW)|(APP))_VERSION_MINOR\s+)\d+");
                Regex regex_patch = new Regex(@"(?<=\#define\s+((FW)|(APP))_VERSION_PATCH\s+)\d+");
                Match match_major = regex_major.Match(code_data);
                Match match_minor = regex_minor.Match(code_data);
                Match match_patch = regex_patch.Match(code_data);
                if (match_major.Success && match_minor.Success && match_patch.Success)
                {
                    int[] version = { Convert.ToInt32(match_major.Value), Convert.ToInt32(match_minor.Value), Convert.ToInt32(match_patch.Value) };
                    UpdataVersion(ref version);
                    code_data = regex_major.Replace(code_data, version[0].ToString());
                    code_data = regex_minor.Replace(code_data, version[1].ToString());
                    code_data = regex_patch.Replace(code_data, version[2].ToString());
                    File.WriteAllText(path, code_data, encoding);
                }
                else
                {
                    Console.WriteLine("Error: 没有找到 FW_VERSION_ 的宏定义。");
                }
            }
            Console.WriteLine("完成");
        }

        static void UpdataVersion(ref int[] version)
        {
            int i = version.Length - 1;
            if (i > 0)
            {
                while (++version[i] > 255)
                {
                    if (i > 0)
                    {
                        version[i] = 0;
                        i--;
                    }
                    else
                    {
                        version[i] = 255;
                        break;
                    }
                }
            }
        }

        static void UpdataVersion(ref string line)
        {
            var first = line.IndexOf('"');
            var second = line.LastIndexOf('"');
            var sVersion = line.Substring(first + 1, second - first - 1);
            var arrVersion = sVersion.Split('.');
            if (arrVersion.Length < 4) return;
            var major = Convert.ToInt32(arrVersion[0]);
            var minor = Convert.ToInt32(arrVersion[1]);
            var build = Convert.ToInt32(arrVersion[2]);
            var amendment = Convert.ToInt32(arrVersion[3]);
            if (++amendment > 9999)
            {
                build++;
            }
            if (build > 99)
            {
                minor++;
            }
            if (minor > 9)
            {
                major++;
            }
            var sNewVersion = $"{major}.{minor}.{build}.{amendment}";
            line = line.Replace(sVersion, sNewVersion);
        }

        static void CopyVersionFile(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath) || !File.Exists(targetPath))
            {
                Console.WriteLine(sourcePath);
                Console.WriteLine(targetPath);
                Console.WriteLine("Error: 文件不存在。");
                return;
            }
            var sourceLines = File.ReadAllLines(sourcePath);
            string strFileVersion = null, strVersion = null;
            for (int i = sourceLines.Length - 1; i >= 0; i--)
            {
                if (sourceLines[i].Contains("assembly: AssemblyFileVersion"))
                {
                    strFileVersion = sourceLines[i];
                }
                if (sourceLines[i].Contains("assembly: AssemblyVersion") && !sourceLines[i].Contains("*"))
                {
                    strVersion = sourceLines[i];
                    break;
                }
            }
            if (string.IsNullOrEmpty(strFileVersion) || string.IsNullOrEmpty(strVersion))
            {
                Console.WriteLine("Error: AssemblyFileVersion or AssemblyVersion 不存在。");
                return;
            }
            var targetLines = File.ReadAllLines(targetPath);
            for (int i = targetLines.Length - 1; i >= 0; i--)
            {
                if (targetLines[i].Contains("assembly: AssemblyFileVersion"))
                {
                    targetLines[i] = strFileVersion;
                }
                if (targetLines[i].Contains("assembly: AssemblyVersion") && !targetLines[i].Contains("*"))
                {
                    targetLines[i] = strVersion;
                    break;
                }
            }
            File.WriteAllLines(targetPath, targetLines);
            Console.WriteLine("完成");
        }
    }
}
