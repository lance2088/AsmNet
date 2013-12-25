using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using AsmEngine.Objects;
using System.Reflection;
using System.Reflection.Emit;
using AsmEngine;

namespace ASM.net.src
{
    public class AssemblerCompiler
    {
        private string code;
        public List<Byte> AssemblerBytes = new List<byte>();
        public List<src.CompileError> errors = new List<CompileError>();
        public SortedList<string, object> defines;

        public AssemblerCompiler(string Code)
        {
            this.code = Code;
            defines = new SortedList<string, object>();

            //Misc
            defines.Add("INFINITE", 0xFFFFFFFF);
            defines.Add("WAIT_ABANDONED", 0x00000080L);
            defines.Add("WAIT_OBJECT_0", 0x00000000L);
            defines.Add("WAIT_TIMEOUT", 0x00000102L);
            defines.Add("WAIT_FAILED", 0xFFFFFFFF);

            //Messagebox
            defines.Add("MB_ABORTRETRYIGNORE", 0x00000002L);
            defines.Add("MB_CANCELTRYCONTINUE", 0x00000006L);
            defines.Add("MB_HELP", 0x00004000L);
            defines.Add("MB_OK", 0);
            defines.Add("MB_OKCANCEL", 0x00000001L);
            defines.Add("MB_RETRYCANCEL", 0x00000005L);
            defines.Add("MB_YESNO", 0x00000004L);
            defines.Add("MB_YESNOCANCEL", 0x00000003L);

            defines.Add("IDOK", 1);
            defines.Add("IDCANCEL", 2);
            defines.Add("IDABORT", 3);
            defines.Add("IDRETRY", 4);
            defines.Add("IDIGNORE", 5);
            defines.Add("IDYES", 6);
            defines.Add("IDNO", 7);
            defines.Add("IDCLOSE", 8);
            defines.Add("IDHELP", 9);
            defines.Add("IDTRYAGAIN", 10);
            defines.Add("IDCONTINUE", 11);


            defines.Add("NULL", 0);
            defines.Add("true", 1);
            defines.Add("false", 0);
            defines.Add("TRUE", 1);
            defines.Add("FALSE", 0);

            //create file
            defines.Add("FILE_SHARE_DELETE", 0);
            defines.Add("FILE_SHARE_READ", 1);
            defines.Add("FILE_SHARE_WRITE", 2);
            defines.Add("CREATE_ALWAYS", 2);
            defines.Add("CREATE_NEW", 1);
            defines.Add("OPEN_ALWAYS", 4);
            defines.Add("OPEN_EXISTING", 3);
            defines.Add("TRUNCATE_EXISTING", 5);
            defines.Add("FILE_ATTRIBUTE_ARCHIVE", 0x20);
            defines.Add("FILE_ATTRIBUTE_ENCRYPTED", 0x4000);
            defines.Add("FILE_ATTRIBUTE_HIDDEN", 0x02);
            defines.Add("FILE_ATTRIBUTE_NORMAL", 0x80);
            defines.Add("FILE_ATTRIBUTE_OFFLINE", 0x1000);
            defines.Add("FILE_ATTRIBUTE_READONLY", 0x01);
            defines.Add("FILE_ATTRIBUTE_SYSTEM", 0x04);
            defines.Add("FILE_ATTRIBUTE_TEMPORARY", 0x100);
            defines.Add("FILE_FLAG_BACKUP_SEMANTICS", 0x02000000);
            defines.Add("FILE_FLAG_DELETE_ON_CLOSE", 0x04000000);
            defines.Add("FILE_FLAG_NO_BUFFERING", 0x20000000);
            defines.Add("FILE_FLAG_OPEN_NO_RECALL", 0x00100000);
            defines.Add("FILE_FLAG_OPEN_REPARSE_POINT", 0x00200000);
            defines.Add("FILE_FLAG_OVERLAPPED", 0x40000000);
            defines.Add("FILE_FLAG_POSIX_SEMANTICS", 0x0100000);
            defines.Add("FILE_FLAG_RANDOM_ACCESS", 0x10000000);
            defines.Add("FILE_FLAG_SEQUENTIAL_SCAN", 0x08000000);
            defines.Add("FILE_FLAG_WRITE_THROUGH", 0x80000000);

            //Mutex
            defines.Add("DELETE", 0x00010000L);
            defines.Add("READ_CONTROL", 0x00020000L);
            defines.Add("SYNCHRONIZE", 0x00100000L);
            defines.Add("WRITE_DAC", 0x00040000L);
            defines.Add("WRITE_OWNER", 0x00080000L);
            defines.Add("EVENT_ALL_ACCESS", 0x1F0003);
            defines.Add("EVENT_MODIFY_STATE", 0x0002);
            defines.Add("MUTEX_ALL_ACCESS", 0x1F0001);
            defines.Add("MUTEX_MODIFY_STATE", 0x0001);
            defines.Add("SEMAPHORE_ALL_ACCESS", 0x1F0003);
            defines.Add("SEMAPHORE_MODIFY_STATE", 0x0002);
            defines.Add("TIMER_ALL_ACCESS ", 0x1F0003);
            defines.Add("TIMER_MODIFY_STATE", 0x0002);
            defines.Add("TIMER_QUERY_STATE", 0x0001);

            #region "Error Codes"
            defines.Add("ERROR_SUCCESS", 0x0);
            defines.Add("ERROR_INVALID_FUNCTION", 0x1);
            defines.Add("ERROR_FILE_NOT_FOUND", 0x2);
            defines.Add("ERROR_PATH_NOT_FOUND", 0x3);
            defines.Add("ERROR_TOO_MANY_OPEN_FILES", 0x4);
            defines.Add("ERROR_ACCESS_DENIED", 0x5);
            defines.Add("ERROR_INVALID_HANDLE", 0x6);
            defines.Add("ERROR_ARENA_TRASHED", 0x7);
            defines.Add("ERROR_NOT_ENOUGH_MEMORY", 0x8);
            defines.Add("ERROR_INVALID_BLOCK", 0x9);
            defines.Add("ERROR_BAD_ENVIRONMENT", 0xA);
            defines.Add("ERROR_BAD_FORMAT", 0xB);
            defines.Add("ERROR_INVALID_ACCESS", 0xC);
            defines.Add("ERROR_INVALID_DATA", 0xD);
            defines.Add("ERROR_OUTOFMEMORY", 0xE);
            defines.Add("ERROR_INVALID_DRIVE", 0xF);
            defines.Add("ERROR_CURRENT_DIRECTORY", 0x10);
            defines.Add("ERROR_NOT_SAME_DEVICE", 0x11);
            defines.Add("ERROR_NO_MORE_FILES", 0x12);
            defines.Add("ERROR_WRITE_PROTECT", 0x13);
            defines.Add("ERROR_BAD_UNIT", 0x14);
            defines.Add("ERROR_NOT_READY", 0x15);
            defines.Add("ERROR_BAD_COMMAND", 0x16);
            defines.Add("ERROR_CRC", 0x17);
            defines.Add("ERROR_BAD_LENGTH", 0x18);
            defines.Add("ERROR_SEEK", 0x19);
            defines.Add("ERROR_NOT_DOS_DISK", 0x1A);
            defines.Add("ERROR_SECTOR_NOT_FOUND", 0x1B);
            defines.Add("ERROR_OUT_OF_PAPER", 0x1C);
            defines.Add("ERROR_WRITE_FAULT", 0x1D);
            defines.Add("ERROR_READ_FAULT", 0x1E);
            defines.Add("ERROR_GEN_FAILURE", 0x1F);
            defines.Add("ERROR_SHARING_VIOLATION", 0x20);
            defines.Add("ERROR_LOCK_VIOLATION", 0x21);
            defines.Add("ERROR_WRONG_DISK", 0x22);
            defines.Add("ERROR_SHARING_BUFFER_EXCEEDED", 0x24);
            defines.Add("ERROR_HANDLE_EOF", 0x26);
            defines.Add("ERROR_HANDLE_DISK_FULL", 0x27);
            defines.Add("ERROR_NOT_SUPPORTED", 0x32);
            defines.Add("ERROR_REM_NOT_LIST", 0x33);
            defines.Add("ERROR_DUP_NAME", 0x34);
            defines.Add("ERROR_BAD_NETPATH", 0x35);
            defines.Add("ERROR_NETWORK_BUSY", 0x36);
            defines.Add("ERROR_DEV_NOT_EXIST", 0x37);
            defines.Add("ERROR_TOO_MANY_CMDS", 0x38);
            defines.Add("ERROR_ADAP_HDW_ERR", 0x39);
            defines.Add("ERROR_BAD_NET_RESP", 0x3A);
            defines.Add("ERROR_UNEXP_NET_ERR", 0x3B);
            defines.Add("ERROR_BAD_REM_ADAP", 0x3C);
            defines.Add("ERROR_PRINTQ_FULL", 0x3D);
            defines.Add("ERROR_NO_SPOOL_SPACE", 0x3E);
            defines.Add("ERROR_PRINT_CANCELLED", 0x3F);
            defines.Add("ERROR_NETNAME_DELETED", 0x40);
            defines.Add("ERROR_NETWORK_ACCESS_DENIED", 0x41);
            defines.Add("ERROR_BAD_DEV_TYPE", 0x42);
            defines.Add("ERROR_BAD_NET_NAME", 0x43);
            defines.Add("ERROR_TOO_MANY_NAMES", 0x44);
            defines.Add("ERROR_TOO_MANY_SESS", 0x45);
            defines.Add("ERROR_SHARING_PAUSED", 0x46);
            defines.Add("ERROR_REQ_NOT_ACCEP", 0x47);
            defines.Add("ERROR_REDIR_PAUSED", 0x48);
            defines.Add("ERROR_FILE_EXISTS", 0x50);
            defines.Add("ERROR_CANNOT_MAKE", 0x52);
            defines.Add("ERROR_FAIL_I24", 0x53);
            defines.Add("ERROR_OUT_OF_STRUCTURES", 0x54);
            defines.Add("ERROR_ALREADY_ASSIGNED", 0x55);
            defines.Add("ERROR_INVALID_PASSWORD", 0x56);
            defines.Add("ERROR_INVALID_PARAMETER", 0x57);
            defines.Add("ERROR_NET_WRITE_FAULT", 0x58);
            defines.Add("ERROR_NO_PROC_SLOTS", 0x59);
            defines.Add("ERROR_TOO_MANY_SEMAPHORES", 0x64);
            defines.Add("ERROR_EXCL_SEM_ALREADY_OWNED", 0x65);
            defines.Add("ERROR_SEM_IS_SET", 0x66);
            defines.Add("ERROR_TOO_MANY_SEM_REQUESTS", 0x67);
            defines.Add("ERROR_INVALID_AT_INTERRUPT_TIME", 0x68);
            defines.Add("ERROR_SEM_OWNER_DIED", 0x69);
            defines.Add("ERROR_SEM_USER_LIMIT", 0x6A);
            defines.Add("ERROR_DISK_CHANGE", 0x6B);
            defines.Add("ERROR_DRIVE_LOCKED", 0x6C);
            defines.Add("ERROR_BROKEN_PIPE", 0x6D);
            defines.Add("ERROR_OPEN_FAILED", 0x6E);
            defines.Add("ERROR_BUFFER_OVERFLOW", 0x6F);
            defines.Add("ERROR_DISK_FULL", 0x70);
            defines.Add("ERROR_NO_MORE_SEARCH_HANDLES", 0x71);
            defines.Add("ERROR_INVALID_TARGET_HANDLE", 0x72);
            defines.Add("ERROR_INVALID_CATEGORY", 0x75);
            defines.Add("ERROR_INVALID_VERIFY_SWITCH", 0x76);
            defines.Add("ERROR_BAD_DRIVER_LEVEL", 0x77);
            defines.Add("ERROR_CALL_NOT_IMPLEMENTED", 0x78);
            defines.Add("ERROR_SEM_TIMEOUT", 0x79);
            defines.Add("ERROR_INSUFFICIENT_BUFFER", 0x7A);
            defines.Add("ERROR_INVALID_NAME", 0x7B);
            defines.Add("ERROR_INVALID_LEVEL", 0x7C);
            defines.Add("ERROR_NO_VOLUME_LABEL", 0x7D);
            defines.Add("ERROR_MOD_NOT_FOUND", 0x7E);
            defines.Add("ERROR_PROC_NOT_FOUND", 0x7F);
            defines.Add("ERROR_WAIT_NO_CHILDREN", 0x80);
            defines.Add("ERROR_CHILD_NOT_COMPLETE", 0x81);
            defines.Add("ERROR_DIRECT_ACCESS_HANDLE", 0x82);
            defines.Add("ERROR_NEGATIVE_SEEK", 0x83);
            defines.Add("ERROR_SEEK_ON_DEVICE", 0x84);
            defines.Add("ERROR_IS_JOIN_TARGET", 0x85);
            defines.Add("ERROR_IS_JOINED", 0x86);
            defines.Add("ERROR_IS_SUBSTED", 0x87);
            defines.Add("ERROR_NOT_JOINED", 0x88);
            defines.Add("ERROR_NOT_SUBSTED", 0x89);
            defines.Add("ERROR_JOIN_TO_JOIN", 0x8A);
            defines.Add("ERROR_SUBST_TO_SUBST", 0x8B);
            defines.Add("ERROR_JOIN_TO_SUBST", 0x8C);
            defines.Add("ERROR_SUBST_TO_JOIN", 0x8D);
            defines.Add("ERROR_BUSY_DRIVE", 0x8E);
            defines.Add("ERROR_SAME_DRIVE", 0x8F);
            defines.Add("ERROR_DIR_NOT_ROOT", 0x90);
            defines.Add("ERROR_DIR_NOT_EMPTY", 0x91);
            defines.Add("ERROR_IS_SUBST_PATH", 0x92);
            defines.Add("ERROR_IS_JOIN_PATH", 0x93);
            defines.Add("ERROR_PATH_BUSY", 0x94);
            defines.Add("ERROR_IS_SUBST_TARGET", 0x95);
            defines.Add("ERROR_SYSTEM_TRACE", 0x96);
            defines.Add("ERROR_INVALID_EVENT_COUNT", 0x97);
            defines.Add("ERROR_TOO_MANY_MUXWAITERS", 0x98);
            defines.Add("ERROR_INVALID_LIST_FORMAT", 0x99);
            defines.Add("ERROR_LABEL_TOO_LONG", 0x9A);
            defines.Add("ERROR_TOO_MANY_TCBS", 0x9B);
            defines.Add("ERROR_SIGNAL_REFUSED", 0x9C);
            defines.Add("ERROR_DISCARDED", 0x9D);
            defines.Add("ERROR_NOT_LOCKED", 0x9E);
            defines.Add("ERROR_BAD_THREADID_ADDR", 0x9F);
            defines.Add("ERROR_BAD_ARGUMENTS", 0xA0);
            defines.Add("ERROR_BAD_PATHNAME", 0xA1);
            defines.Add("ERROR_SIGNAL_PENDING", 0xA2);
            defines.Add("ERROR_MAX_THRDS_REACHED", 0xA4);
            defines.Add("ERROR_LOCK_FAILED", 0xA7);
            defines.Add("ERROR_BUSY", 0xAA);
            defines.Add("ERROR_CANCEL_VIOLATION", 0xAD);
            defines.Add("ERROR_ATOMIC_LOCKS_NOT_SUPPORTED", 0xAE);
            defines.Add("ERROR_INVALID_SEGMENT_NUMBER", 0xB4);
            defines.Add("ERROR_INVALID_ORDINAL", 0xB6);
            defines.Add("ERROR_ALREADY_EXISTS", 0xB7);
            #endregion
        }

        public void Build()
        {
            AssemblerBytes = new List<byte>();

            MemoryStream stream = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(code), 0, code.Length);
            StreamReader reader = new StreamReader(stream);
            errors.Clear();
            GlobalVariables.MainForm.listView1.Items.Clear();
            SortedList<string, byte> variables = new SortedList<string, byte>();
            SortedList<string, int> JumpLabels = new SortedList<string, int>();

            string CurrentNamespace = "";
            bool BodyOpen = false;

            //Increment id's
            byte NamespaceId = 0;
            byte ClassId = 0;

            string LastKeyword = "";
            byte LastNamespaceId = 0;
            byte LastClassId = 0;
            string LastNamespaceName = "";
            string LastClassName = "";

            bool InsideBody = false;

            int CurrentLine = 0;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                line = line.Replace("\t", "");
                line = line.Trim();
                CurrentLine++;

                //remove the comment behind the ASM
                #region Remove comment from code
                int StartComment = 0;
                bool FoundComment = false;
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == ';')
                    {
                        StartComment = i;
                        FoundComment = true;
                        break;
                    }
                }
                if (FoundComment)
                {
                    line = line.Substring(0, StartComment);
                    line = line.Trim();
                }
                #endregion

                if (line.ToLower().StartsWith("namespace"))
                {
                    //00 00
                    //length - INT32
                    //string

                    try
                    {
                        LastNamespaceName = line.Substring(8).Trim();
                        LastNamespaceName = LastNamespaceName.Split(' ')[1];
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Identifier expected"));
                        continue;
                    }

                    AssemblerBytes.AddRange(new byte[] { 00, 00 });

                    NamespaceId++;
                    LastNamespaceId = NamespaceId;
                    AssemblerBytes.Add(NamespaceId);
                    CurrentNamespace = LastNamespaceName;
                    LastKeyword = "namespace";
                }
                else if (line.ToLower().Contains("class"))
                {
                    //00 01
                    //accessor, 00=private, 01=public, 02=protected
                    //name
                    //Namespace id
                    //Class id
                    try
                    {
                        AssemblerBytes.AddRange(new byte[] { 00, 01 });

                        try
                        {
                            Match accessor = Regex.Match(line, "(public|private|protected|internal).");
                            AssemblerBytes.Add((byte)Enum.Parse(typeof(Accessor), accessor.Value.Trim(), true));
                        }
                        catch
                        {
                            errors.Add(new CompileError(CurrentLine, "Unknown specified accessor, Please give one of the following accessor: private, public, protected, internal"));
                            continue;
                        }

                        AssemblerBytes.Add(NamespaceId);
                        AssemblerBytes.Add(ClassId);
                        LastClassId = ClassId;
                        ClassId++;
                        LastKeyword = "class";
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                    //un used for now
                /*else if (line.ToLower().Contains("struct"))
                {
                    LastKeyword = "struct";
                }
                else if (line.ToLower().Contains("func"))
                {
                    //00 02
                    Match accessor = Regex.Match(line, "(public|private|protected|internal).");
                    AssemblerBytes.Add((byte)Enum.Parse(typeof(Accessor), accessor.Value.Trim(), true));
                }*/
                #region Assembler
                else if (line.ToLower().StartsWith("push"))
                {
                    try
                    {
                        AssemblerBytes.AddRange(new byte[] { 01, 00 });

                        //DataType, 0 = INT32, 1 = string
                        //if(datatype == string) GiveLength
                        //value

                        Match match = Regex.Match(line.Substring(5), "\"(.*?)\"");
                        if (match.ToString() != "")
                        {
                            string RegexString = match.ToString().Substring(1, match.ToString().Length - 2);
                            RegexString = RegexString.Replace("\\r", "\r");
                            RegexString = RegexString.Replace("\\n", "\n");

                            AssemblerBytes.Add(1); //string
                            string MergedStrings = RegexString;

                            AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(MergedStrings.Length))); //length
                            AssemblerBytes.AddRange(ASCIIEncoding.ASCII.GetBytes(MergedStrings));
                            continue;
                        }

                        match = Regex.Match(line.Substring(5), "[0-9]*");
                        if (match.ToString() != "")
                        {
                            AssemblerBytes.Add(0); //0 = INT32
                            AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(match.ToString())));
                            continue;
                        }

                        //Check registers
                        if (line.Substring(5).ToLower().Contains("eax"))
                        {
                            AssemblerBytes.Add(3);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("ebx"))
                        {
                            AssemblerBytes.Add(4);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("ecx"))
                        {
                            AssemblerBytes.Add(5);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("edx"))
                        {
                            AssemblerBytes.Add(6);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("esi"))
                        {
                            AssemblerBytes.Add(7);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("edi"))
                        {
                            AssemblerBytes.Add(8);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("ebp"))
                        {
                            AssemblerBytes.Add(9);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("esp"))
                        {
                            AssemblerBytes.Add(10);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("eip"))
                        {
                            AssemblerBytes.Add(11);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("cx"))
                        {
                            AssemblerBytes.Add(12);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("dx"))
                        {
                            AssemblerBytes.Add(13);
                            continue;
                        }
                        else if (line.Substring(5).ToLower().Contains("bx"))
                        {
                            AssemblerBytes.Add(14);
                            continue;
                        }

                        if (defines.ContainsKey(line.Substring(5)))
                        {
                            if (defines[line.Substring(5)].GetType() == typeof(Int32))
                            {
                                AssemblerBytes.Add(0); //0 = INT32
                                AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(defines[line.Substring(5)])));
                            }
                            else if (defines[line.Substring(5)].GetType() == typeof(Int64))
                            {
                                AssemblerBytes.Add(2); //0 = INT64
                                AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt64(defines[line.Substring(5)])));
                            }
                            else if (defines[line.Substring(5)].GetType() == typeof(UInt32))
                            {
                                AssemblerBytes.Add(15); //UInt32
                                AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(defines[line.Substring(5)])));
                            }
                            else if (defines[line.Substring(5)].GetType() == typeof(UInt64))
                            {
                                AssemblerBytes.Add(16); //UInt32
                                AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt64(defines[line.Substring(5)])));
                            }
                        }
                        else
                        {
                            errors.Add(new CompileError(CurrentLine, "Unknown PUSH value"));
                        }
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                else if (line.ToLower().StartsWith("call"))
                {
                    //01 01
                    //Function id
                    try
                    {
                        AssemblerBytes.AddRange(new byte[] { 01, 01 });
                        switch (line.Substring(5).ToLower())
                        {
                            case "messagebox":          AssemblerBytes.Add(1); break;
                            case "createfile":          AssemblerBytes.Add(2); break;
                            case "listen":              AssemblerBytes.Add(3); break;
                            case "writefile":           AssemblerBytes.Add(4); break;
                            case "beep":                AssemblerBytes.Add(5); break;
                            case "allocconsole":        AssemblerBytes.Add(6); break;
                            case "attachconsole":       AssemblerBytes.Add(7); break;
                            case "activateactctx":      AssemblerBytes.Add(8); break;
                            case "isdebuggerpresent":   AssemblerBytes.Add(9); break;
                            case "debugactiveprocess":  AssemblerBytes.Add(10); break;
                            case "closehandle":         AssemblerBytes.Add(11); break;
                            case "copyfile":            AssemblerBytes.Add(12); break;
                            case "createconsolescreenbuffer": AssemblerBytes.Add(13); break;
                            case "createdirectory":     AssemblerBytes.Add(14); break;
                            case "createdirectoryex":   AssemblerBytes.Add(15); break;
                            case "createevent":         AssemblerBytes.Add(16); break;
                            case "createmutex":         AssemblerBytes.Add(17); break;
                            case "openmutex":           AssemblerBytes.Add(18); break;
                            case "getlasterror":        AssemblerBytes.Add(19); break;
                            case "waitforsingleobject": AssemblerBytes.Add(20); break;

                            default:
                                AssemblerBytes.Add(0);
                                errors.Add(new CompileError(CurrentLine, "Unknown function, Or not be supported yet"));
                                break;
                        }
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                else if (line.ToLower() == "nop")
                {
                    AssemblerBytes.AddRange(new byte[] { 01, 02 });
                }
                else if (line.ToLower().StartsWith("inc") || line.ToLower().StartsWith("dec"))
                {
                    try
                    {
                        if (line.ToLower().StartsWith("inc"))
                            AssemblerBytes.AddRange(new byte[] { 01, 03 });
                        else if (line.ToLower().StartsWith("dec"))
                            AssemblerBytes.AddRange(new byte[] { 01, 04 });

                        if (line.ToLower().Substring(4).StartsWith("$"))
                        {
                            if (variables.ContainsKey(line.ToLower().Substring(5)))
                            {
                                AssemblerBytes.Add(0); //variable or register
                                AssemblerBytes.Add(variables[line.ToLower().Substring(5)]);
                            }
                            else
                            {
                                try
                                {
                                    errors.Add(new CompileError(CurrentLine, "The variable " + line.ToLower().Substring(3) + " is not declared"));
                                }
                                catch
                                {
                                    errors.Add(new CompileError(CurrentLine, "The variable is not declared"));
                                }
                            }
                        }
                        else if (line.ToLower().Substring(4).StartsWith("eax"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.EAX);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("ah"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.AH);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("al"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.AL);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("ebx"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.EBX);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("ecx"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.ECX);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("edx"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.EDX);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("esi"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.ESI);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("edi"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.EDI);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("ax"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.AX);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("cx"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.CX);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("dx"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.DX);
                        }
                        else if (line.ToLower().Substring(4).StartsWith("bx"))
                        {
                            AssemblerBytes.Add(1); //not a variable
                            AssemblerBytes.Add((byte)Register.BX);
                        }
                        else
                        {
                            errors.Add(new CompileError(CurrentLine, "Invalid register"));
                            continue;
                        }
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                else if (line.ToLower().StartsWith("mov"))
                {
                    try
                    {
                        AssemblerBytes.AddRange(new byte[] { 01, 05 });

                        bool isRegister = false;

                        //Read what we are going to modify
                        if (line.ToLower().Substring(4).StartsWith("ah"))
                        {
                            AssemblerBytes.Add(0);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("eax"))
                        {
                            AssemblerBytes.Add(1);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("al"))
                        {
                            AssemblerBytes.Add(2);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("ebx"))
                        {
                            AssemblerBytes.Add(3);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("ecx"))
                        {
                            AssemblerBytes.Add(4);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("edx"))
                        {
                            AssemblerBytes.Add(5);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("esi"))
                        {
                            AssemblerBytes.Add(6);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("edi"))
                        {
                            AssemblerBytes.Add(7);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("ax"))
                        {
                            AssemblerBytes.Add(8);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("cx"))
                        {
                            AssemblerBytes.Add(9);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("dx"))
                        {
                            AssemblerBytes.Add(10);
                            isRegister = true;
                        }
                        else if (line.ToLower().Substring(4).StartsWith("bx"))
                        {
                            AssemblerBytes.Add(11);
                            isRegister = true;
                        }
                        else
                        {
                            errors.Add(new CompileError(CurrentLine, "Invalid register"));
                            continue;
                        }

                        //Value
                        Match match = Regex.Match(line.ToLower().Split(',')[1].Trim(), "[0-9]*");

                        if (line.ToLower().Split(',')[1].Trim().EndsWith("h"))
                        {
                            string ToMatch = line.ToLower().Substring(4).Split(',')[1].Trim();
                            ToMatch = ToMatch.Substring(0, ToMatch.Length - 1);
                            match = Regex.Match(ToMatch, @"^[A-Fa-f0-9]*$");
                        }
                        else
                            match = Regex.Match(line.ToLower().Split(',')[1].Trim(), "[0-9]*");


                        Match Expres1Match = Regex.Match(line.ToLower().Split(',')[1].Trim(), "(eax|ah|al|ecx|cx|dx)");
                        if (Expres1Match.ToString() != "")
                        {
                            AssemblerBytes.Add(1);
                            if (Expres1Match.ToString() == "eax")
                                AssemblerBytes.Add((byte)Register.EAX);
                            else if (Expres1Match.ToString() == "ah")
                                AssemblerBytes.Add((byte)Register.AH);
                            else if (Expres1Match.ToString() == "al")
                                AssemblerBytes.Add((byte)Register.AL);
                            else if (Expres1Match.ToString() == "ecx")
                                AssemblerBytes.Add((byte)Register.ECX);
                            else if (Expres1Match.ToString() == "cx")
                                AssemblerBytes.Add((byte)Register.CX);
                            else if (Expres1Match.ToString() == "dx")
                                AssemblerBytes.Add((byte)Register.DX);
                            else if (Expres1Match.ToString() == "bx")
                                AssemblerBytes.Add((byte)Register.DX);
                            else if (Expres1Match.ToString() == "ebx")
                                AssemblerBytes.Add((byte)Register.EDX);
                            else if (Expres1Match.ToString() == "edx")
                                AssemblerBytes.Add((byte)Register.EDX);
                        }
                        else if (match.ToString() != "" && isRegister == true)
                        {
                            if (line.ToLower().Trim().EndsWith("h"))
                            {
                                short HexValue = short.Parse(match.ToString(), System.Globalization.NumberStyles.HexNumber);
                                AssemblerBytes.Add(0);
                                AssemblerBytes.AddRange(BitConverter.GetBytes((short)HexValue));
                            }
                            else
                            {
                                AssemblerBytes.Add(0);
                                AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(match.ToString())));
                            }
                        }
                        else
                        {
                            errors.Add(new CompileError(CurrentLine, "Invalid register or variable"));
                            continue;
                        }
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                else if (line.ToLower().StartsWith("int"))
                {
                    try
                    {
                        AssemblerBytes.AddRange(new byte[] { 01, 06 });
                        Match match = Regex.Match(line.ToLower().Substring(4), "[0-9]*");
                        if (match.ToString() != "")
                            AssemblerBytes.Add(BitConverter.GetBytes(Convert.ToByte(match.ToString()))[0]);
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                /*else if (line.ToLower().StartsWith(".while"))
                {
                    AssemblerBytes.AddRange(new byte[] { 01, 07 });
                    AssemblerBytes.Add(0);
                }*/
                else if (line.ToLower().StartsWith("jmp") || line.ToLower().StartsWith("jz") ||
                         line.ToLower().StartsWith("jnz") || line.ToLower().StartsWith("jne") ||
                         line.ToLower().StartsWith("je"))
                {
                    try
                    {
                        AssemblerBytes.AddRange(new byte[] { 01, 08 });

                        byte offset = 0;
                        if (line.ToLower().StartsWith("jmp"))
                        {
                            AssemblerBytes.Add(0);
                            offset = 4;
                        }
                        else if (line.ToLower().StartsWith("jz"))
                        {
                            AssemblerBytes.Add(1);
                            offset = 3;
                        }
                        else if (line.ToLower().StartsWith("jnz"))
                        {
                            AssemblerBytes.Add(2);
                            offset = 4;
                        }
                        else if (line.ToLower().StartsWith("jne"))
                        {
                            AssemblerBytes.Add(3);
                            offset = 4;
                        }
                        else if (line.ToLower().StartsWith("je"))
                        {
                            AssemblerBytes.Add(4);
                            offset = 3;
                        }

                        Match match = null;
                        if (line.ToLower().Trim().EndsWith("h"))
                        {
                            string ToMatch = line.ToLower().Substring(offset);
                            ToMatch = ToMatch.Substring(0, ToMatch.Length - 1);
                            match = Regex.Match(ToMatch, @"^[A-Fa-f0-9]*$");
                        }
                        else
                            match = Regex.Match(line.ToLower().Substring(4), "[0-9]*");

                        if (match.ToString() != "")
                        {
                            if (line.ToLower().Trim().EndsWith("h"))
                            {
                                short HexValue = short.Parse(match.ToString(), System.Globalization.NumberStyles.HexNumber);
                                AssemblerBytes.AddRange(BitConverter.GetBytes((short)HexValue));
                            }
                            else
                            {
                                AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(match.ToString())));
                            }
                        }
                        else
                            errors.Add(new CompileError(CurrentLine, "Unknown jump address, hex decimal ?"));
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                else if (line.ToLower().StartsWith("cmp"))
                {
                    try
                    {
                        AssemblerBytes.AddRange(new byte[] { 01, 09 });
                        string Expression1 = line.ToLower().Substring(4).Split(',')[0];
                        string Expression2 = line.ToLower().Split(',')[1];

                        Match Expres1Match = Regex.Match(Expression1, "(eax|ah|al|ecx|cx|dx|bx)");
                        if (Expres1Match.ToString() != "")
                        {
                            if (Expres1Match.ToString() == "eax")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.EAX });
                            else if (Expres1Match.ToString() == "ah")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.AH });
                            else if (Expres1Match.ToString() == "al")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.AL });
                            else if (Expres1Match.ToString() == "ecx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.ECX });
                            else if (Expres1Match.ToString() == "cx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.CX });
                            else if (Expres1Match.ToString() == "dx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.DX });
                            else if (Expres1Match.ToString() == "bx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.BX });
                            else if (Expres1Match.ToString() == "ebx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.EBX });
                            else if (Expres1Match.ToString() == "edx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.EDX });
                        }

                        Match Expres2Match = Regex.Match(Expression2, "(eax|ah|al|ecx|cx|dx|bx)");
                        if (Expres2Match.ToString() != "")
                        {
                            if (Expres2Match.ToString() == "eax")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.EAX });
                            else if (Expres2Match.ToString() == "ah")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.AH });
                            else if (Expres2Match.ToString() == "al")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.AL });
                            else if (Expres2Match.ToString() == "ecx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.ECX });
                            else if (Expres2Match.ToString() == "cx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.CX });
                            else if (Expres2Match.ToString() == "dx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.DX });
                            else if (Expres2Match.ToString() == "bx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.BX });
                            else if (Expres2Match.ToString() == "ebx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.EBX });
                            else if (Expres2Match.ToString() == "edx")
                                AssemblerBytes.AddRange(new byte[] { 00, (byte)Register.EDX });
                        }
                        else if (Expres2Match.ToString() == "")
                        {
                            Expres2Match = Regex.Match(Expression2.Trim(), @"[0-9]*");
                            if (Expres2Match.ToString() != "")
                            {
                                AssemblerBytes.Add(0x01);
                                AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt64(Expres2Match.ToString())));
                            }
                            else
                            {
                                if (defines.ContainsKey(line.Split(',')[1].Trim()))
                                {
                                    AssemblerBytes.Add(0x01);
                                    AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt64(defines[line.Split(',')[1].Trim()])));
                                }
                            }
                        }
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                #endregion

                else if (line.ToLower().StartsWith("$")) //variable
                {
                    try
                    {
                        AssemblerBytes.AddRange(new byte[] { 03, 00 });
                        byte VariableId = (byte)variables.Count;

                        string VariableName = line.ToLower().Substring(1);
                        if (VariableName.Contains(" "))
                            VariableName = VariableName.Split(' ')[0];

                        if (!variables.ContainsKey(VariableName))
                            variables.Add(VariableName, VariableId);
                        else
                        {
                            errors.Add(new CompileError(CurrentLine, "Variable '" + VariableName + "' is already defined"));
                            continue;
                        }

                        Match match = Regex.Match(line.Split('=')[1], "\"(.*?)\"");
                        if (match.ToString() != "")
                        {
                            string RegexString = match.ToString().Substring(1, match.ToString().Length - 2);
                            AssemblerBytes.Add(0x00); //string
                            AssemblerBytes.Add(VariableId);
                            AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(RegexString.Length))); //length
                            AssemblerBytes.AddRange(ASCIIEncoding.ASCII.GetBytes(RegexString));
                        }
                        else if (match.ToString() == "")
                        {
                            match = Regex.Match(line.Split('=')[1].Trim(), "[0-9]*");
                            if (match.ToString() != "")
                            {
                                AssemblerBytes.Add(0x01); //0 = INT32
                                AssemblerBytes.Add(VariableId);
                                AssemblerBytes.AddRange(BitConverter.GetBytes((int)Convert.ToInt32(match.ToString())));
                            }
                        }
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }

                //special
                else if (line.ToLower().StartsWith("bin"))
                {
                    try
                    {
                        line = line.Substring(4);
                        string[] BinaryStrings = line.Split(new char[] { ' ' });

                        for (int i = 0; i < BinaryStrings.Length; i += 2)
                            AssemblerBytes.Add(Convert.ToByte(BinaryStrings[i] + BinaryStrings[i + 1], 2));
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                else if (line.ToLower().StartsWith("write"))
                {
                    //02 00
                    //length - INT32
                    //string

                    try
                    {
                        Match match = Regex.Match(line.Substring(6), "\"(.*?)\"");
                        if (match.ToString() != "")
                        {
                            string RegexString = match.ToString().Substring(1, match.ToString().Length - 2);

                            if (line.ToLower().StartsWith("writel"))
                                RegexString += "\r\n";

                            AssemblerBytes.AddRange(new byte[] { 02, 00 });
                            AssemblerBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(RegexString.Length))); //length
                            AssemblerBytes.AddRange(ASCIIEncoding.ASCII.GetBytes(RegexString));
                        }
                    }
                    catch
                    {
                        errors.Add(new CompileError(CurrentLine, "Compile error at line " + CurrentLine + ", please recheck your code"));
                        continue;
                    }
                }
                else if (line.StartsWith("{"))
                {
                    BodyOpen = true;
                    if (LastClassName != "" || LastNamespaceName != "")
                        InsideBody = true;
                    AssemblerBytes.AddRange(new byte[] { 5, 0 });
                }
                else if (line.StartsWith("}"))
                {
                    BodyOpen = false;
                    if (LastClassName != "" || LastNamespaceName != "")
                        InsideBody = false;
                    AssemblerBytes.AddRange(new byte[] { 5, 1 });
                }
                else if (line == "") { } //empty line
                else if (line.ToLower().Trim().EndsWith(":"))
                {
                    //LABEL
                }
                else if (line.ToLower().Trim().StartsWith(";")) { /*comment*/ }

                else
                {
                    errors.Add(new CompileError(CurrentLine, "Unknown keyword '" + line + "'"));
                }
            }

            if (errors.Count > 0)
            {
                GlobalVariables.MainForm.listView1.Items.Clear();
                for(int i = 0; i < errors.Count; i++)
                    GlobalVariables.MainForm.listView1.Items.Add(errors[i].item);
                return;
            }
        }
    }
}