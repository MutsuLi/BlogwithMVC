using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Blog.Common
{
    //
    // 摘要:
    //     调用cmd
    public class CmdTo
    {
        //
        // 摘要:
        //     返回结果
        public class BashResult
        {
            //
            // 摘要:
            //     The command's standard output as a string. (if redirected)
            public string Output
            {
                get;
                private set;
            }

            //
            // 摘要:
            //     The command's error output as a string. (if redirected)
            public string ErrorMsg
            {
                get;
                private set;
            }

            //
            // 摘要:
            //     The command's exit code as an integer.
            public int ExitCode
            {
                get;
                private set;
            }

            //
            // 摘要:
            //     An array of the command's output split by newline characters. (if redirected)
            public string[] Lines => Output?.Split(Environment.NewLine.ToCharArray());

            internal BashResult(string output, string errorMsg, int exitCode)
            {
                Output = output?.TrimEnd(Environment.NewLine.ToCharArray());
                ErrorMsg = errorMsg?.TrimEnd(Environment.NewLine.ToCharArray());
                ExitCode = exitCode;
            }
        }

        //
        // 摘要:
        //     执行
        public class Bash
        {
            private static bool Plinux
            {
                get;
            }

            private static bool Pmac
            {
                get;
            }

            private static bool Pwindows
            {
                get;
            }

            private static string PbashPath
            {
                get;
            }

            //
            // 摘要:
            //     Determines whether bash is running in a native OS (Linux/MacOS).
            //
            // 返回结果:
            //     True if in *nix, else false.
            public static bool Native
            {
                get;
            }

            //
            // 摘要:
            //     Determines if using Windows and if Linux subsystem is installed.
            //
            // 返回结果:
            //     True if in Windows and bash detected.
            public static bool Subsystem => Pwindows && File.Exists("C:\\Windows\\System32\\bash.exe");

            //
            // 摘要:
            //     Stores output of the previous command if redirected.
            public string Output
            {
                get;
                private set;
            }

            //
            // 摘要:
            //     Gets an array of the command output split by newline characters if redirected.
            public string[] Lines => Output?.Split(Environment.NewLine.ToCharArray());

            //
            // 摘要:
            //     Stores the exit code of the previous command.
            public int ExitCode
            {
                get;
                private set;
            }

            //
            // 摘要:
            //     Stores the error message of the previous command if redirected.
            public string ErrorMsg
            {
                get;
                private set;
            }

            static Bash()
            {
                Plinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                Pmac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                Pwindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                Native = ((Plinux || Pmac) ? true : false);
                PbashPath = (Native ? "bash" : "bash.exe");
            }

            //
            // 摘要:
            //     Execute a new Bash command.
            //
            // 参数:
            //   input:
            //     The command to execute.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Command(string input, bool redirect = true)
            {
                if (!Native && !Subsystem)
                {
                    throw new PlatformNotSupportedException();
                }
                using (Process process = new Process
                {
                    StartInfo = BashInfo(input, redirect)
                })
                {
                    process.Start();
                    if (redirect)
                    {
                        Output = process.StandardOutput.ReadToEnd().TrimEnd(Environment.NewLine.ToCharArray());
                        ErrorMsg = process.StandardError.ReadToEnd().TrimEnd(Environment.NewLine.ToCharArray());
                    }
                    else
                    {
                        Output = null;
                        ErrorMsg = null;
                    }
                    process.WaitForExit();
                    ExitCode = process.ExitCode;
                    process.Close();
                }
                if (redirect)
                {
                    return new BashResult(Output, ErrorMsg, ExitCode);
                }
                return new BashResult(null, null, ExitCode);
            }

            private ProcessStartInfo BashInfo(string input, bool redirectOutput)
            {
                return new ProcessStartInfo
                {
                    FileName = PbashPath,
                    Arguments = "-c \"" + input + "\"",
                    RedirectStandardInput = false,
                    RedirectStandardOutput = redirectOutput,
                    RedirectStandardError = redirectOutput,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false
                };
            }

            //
            // 摘要:
            //     Echo the given string to standard output.
            //
            // 参数:
            //   input:
            //     The string to print.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Echo(string input, bool redirect = false)
            {
                return Command("echo " + input, redirect);
            }

            //
            // 摘要:
            //     Echo the given string to standard output.
            //
            // 参数:
            //   input:
            //     The string to print.
            //
            //   flags:
            //     Optional `echo` arguments.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Echo(string input, string flags, bool redirect = false)
            {
                return Command("echo " + flags + " " + input, redirect);
            }

            //
            // 摘要:
            //     Echo the given string to standard output.
            //
            // 参数:
            //   input:
            //     The string to print.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Echo(object input, bool redirect = false)
            {
                return Command($"echo {input}", redirect);
            }

            //
            // 摘要:
            //     Echo the given string to standard output.
            //
            // 参数:
            //   input:
            //     The string to print.
            //
            //   flags:
            //     Optional `echo` arguments.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Echo(object input, string flags, bool redirect = false)
            {
                return Command($"echo {flags} {input}", redirect);
            }

            //
            // 摘要:
            //     Search for `pattern` in each file in `location`.
            //
            // 参数:
            //   pattern:
            //     The pattern to match.
            //
            //   location:
            //     The files or directory to search.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Grep(string pattern, string location, bool redirect = true)
            {
                return Command("grep " + pattern + " " + location, redirect);
            }

            //
            // 摘要:
            //     Search for `pattern` in each file in `location`.
            //
            // 参数:
            //   pattern:
            //     The pattern to match.
            //
            //   location:
            //     The files or directory to search.
            //
            //   flags:
            //     Optional `grep` arguments.
            //
            //   redirect:
            //     Print output to terminal if false.
            public BashResult Grep(string pattern, string location, string flags, bool redirect = true)
            {
                return Command("grep " + pattern + " " + flags + " " + location, redirect);
            }

            //
            // 摘要:
            //     List information about files in the current directory.
            //
            // 参数:
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Ls(bool redirect = true)
            {
                return Command("ls", redirect);
            }

            //
            // 摘要:
            //     List information about files in the current directory.
            //
            // 参数:
            //   flags:
            //     Optional `ls` arguments.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Ls(string flags, bool redirect = true)
            {
                return Command("ls " + flags, redirect);
            }

            //
            // 摘要:
            //     List information about the given files.
            //
            // 参数:
            //   flags:
            //     Optional `ls` arguments.
            //
            //   files:
            //     Files or directory to search.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Ls(string flags, string files, bool redirect = true)
            {
                return Command("ls " + flags + " " + files, redirect);
            }

            //
            // 摘要:
            //     Move `source` to `directory`.
            //
            // 参数:
            //   source:
            //     The file to be moved.
            //
            //   directory:
            //     The destination directory.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Mv(string source, string directory, bool redirect = true)
            {
                return Command("mv " + source + " " + directory, redirect);
            }

            //
            // 摘要:
            //     Move `source` to `directory`.
            //
            // 参数:
            //   source:
            //     The file to be moved.
            //
            //   directory:
            //     The destination directory.
            //
            //   flags:
            //     Optional `mv` arguments.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Mv(string source, string directory, string flags, bool redirect = true)
            {
                return Command("mv " + flags + " " + source + " " + directory, redirect);
            }

            //
            // 摘要:
            //     Copy `source` to `directory`.
            //
            // 参数:
            //   source:
            //     The file to be copied.
            //
            //   directory:
            //     The destination directory.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Cp(string source, string directory, bool redirect = true)
            {
                return Command("cp " + source + " " + directory, redirect);
            }

            //
            // 摘要:
            //     Copy `source` to `directory`.
            //
            // 参数:
            //   source:
            //     The file to be copied.
            //
            //   directory:
            //     The destination directory.
            //
            //   flags:
            //     Optional `cp` arguments.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Cp(string source, string directory, string flags, bool redirect = true)
            {
                return Command("cp " + flags + " " + source + " " + directory, redirect);
            }

            //
            // 摘要:
            //     Remove or unlink the given file.
            //
            // 参数:
            //   file:
            //     The file(s) to be removed.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Rm(string file, bool redirect = true)
            {
                return Command("rm " + file, redirect);
            }

            //
            // 摘要:
            //     Remove or unlink the given file.
            //
            // 参数:
            //   file:
            //     The file(s) to be removed.
            //
            //   flags:
            //     Optional `rm` arguments.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Rm(string file, string flags, bool redirect = true)
            {
                return Command("rm " + flags + " " + file, redirect);
            }

            //
            // 摘要:
            //     Concatenate `file` to standard input.
            //
            // 参数:
            //   file:
            //     The source file.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Cat(string file, bool redirect = true)
            {
                return Command("cat " + file, redirect);
            }

            //
            // 摘要:
            //     Concatenate `file` to standard input.
            //
            // 参数:
            //   file:
            //     The source file.
            //
            //   flags:
            //     Optional `cat` arguments.
            //
            //   redirect:
            //     Print output to terminal if false.
            //
            // 返回结果:
            //     A `BashResult` containing the command's output information.
            public BashResult Cat(string file, string flags, bool redirect = true)
            {
                return Command("cat " + flags + " " + file, redirect);
            }
        }

        //
        // 摘要:
        //     Windows操作系统，执行cmd命令 多命令请使用批处理命令连接符：
        public static string Run(string cmdText, string cmdPath = "cmd.exe")
        {
            if (cmdPath == "cmd.exe")
            {
                cmdPath = Environment.SystemDirectory + "\\" + cmdPath;
            }
            string result = "";
            string value = cmdText + " &exit";
            using (Process process = new Process())
            {
                process.StartInfo.FileName = cmdPath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.WriteLine(value);
                process.StandardInput.AutoFlush = true;
                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }
            return result;
        }

        //
        // 摘要:
        //     Linux操作系统，执行Shell 【 using https://github.com/phil-harmoniq/Shell.NET 】
        //
        // 参数:
        //   cmd:
        public static BashResult Shell(string cmd)
        {
            return new Bash().Command(cmd);
        }
    }
}
