using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using System.IO;
using Mono.Cecil.Cil;

namespace ASM.net.src.NetFuscator
{
    public class NetFuscator
    {
        public NetFuscator()
        {

        }

        public void Obfuscate(string FilePath)
        {
            AssemblyDefinition assembly = AssemblyFactory.GetAssembly(FilePath);
            SortedList<string, string> Namespaces = new SortedList<string, string>();

            int ObfuscatedTypes = 0;
            int ObfuscatedExternTypes = 0;

            int RenamedVariables = 0;
            int RenamedFields = 0;
            int RenamedProperty = 0;

            //load the modules info
            foreach (ModuleDefinition module in assembly.Modules)
            {
                module.FullLoad();

                //damage headers
                module.Image.CLIHeader.Cb = 0;
                module.Image.CLIHeader.EntryPointToken = 0;
                module.Image.CLIHeader.ImageHash = new byte[] { };
                module.Image.CLIHeader.ManagedNativeHeader.Size = 0;
                module.Image.CLIHeader.MajorRuntimeVersion = 0;
                module.Image.CLIHeader.MinorRuntimeVersion = 0;


                foreach (TypeDefinition type in module.Types)
                {
                    GlobalVariables.MainForm.textBox1.Text += "MetaData, " + type.Scope.Name + " -> " + type.Scope.Name + "-M\r\n";
                    type.Scope.Name += "-M";

                    GlobalVariables.MainForm.textBox1.Text += "Obfuscating - type: " + type.Name + "\r\n";
                    //type.Name = Char.ConvertFromUtf32(0); //for fun
                    type.Name += "-T";
                    ObfuscatedTypes++;

                    foreach (FieldDefinition field in type.Fields)
                    {
                        GlobalVariables.MainForm.textBox1.Text += "Renaming - field: " + field.Name + "\r\n";
                        field.Name += "-F";
                        RenamedFields++;
                    }

                    foreach (PropertyDefinition property in type.Properties)
                    {
                        GlobalVariables.MainForm.textBox1.Text += "Renaming - property: " + property.Name + "\r\n";
                        property.Name += "-F";
                        RenamedProperty++;
                    }

                    foreach (MethodDefinition method in type.Methods)
                    {
                        foreach(VariableDefinition variable in method.Body.Variables)
                        {
                            GlobalVariables.MainForm.textBox1.Text += "Renaming - variable: " + variable.Name + "\r\n";
                            variable.Name += "-V";
                            RenamedVariables++;
                        }
                    }
                }

                foreach (TypeReference type in module.ExternTypes)
                {
                    GlobalVariables.MainForm.textBox1.Text += "Obfuscating - extern type: " + type.Name + "\r\n";
                    type.Name = "-externT";
                    ObfuscatedExternTypes++;
                }

                for (int i = 0; i < assembly.EntryPoint.Body.Variables.Count; i++)
                {
                    GlobalVariables.MainForm.textBox1.Text += "Renaming - variable: " + assembly.EntryPoint.Body.Variables[i].Name + "\r\n";
                    assembly.EntryPoint.Body.Variables[i].Name += "-V";
                    RenamedVariables++;
                }
            }

            FileInfo file = new FileInfo(FilePath);
            String output = file.FullName;
            output = output.Substring(0, output.Length - file.Name.Length);
            output += file.Name + "-NetFuscator.exe";

            GlobalVariables.MainForm.textBox1.Text += "Obfuscated types: " + ObfuscatedTypes + "\r\n";
            GlobalVariables.MainForm.textBox1.Text += "Obfuscated extern types: " + ObfuscatedExternTypes + "\r\n";
            GlobalVariables.MainForm.textBox1.Text += "Renamed variables: " + RenamedVariables + "\r\n";
            GlobalVariables.MainForm.textBox1.Text += "Renamed fields: " + RenamedFields + "\r\n";
            GlobalVariables.MainForm.textBox1.Text += "Renamed properties: " + RenamedProperty + "\r\n";

            GlobalVariables.MainForm.textBox1.Text += "Sucessfully obfuscated\r\n";
            AssemblyFactory.SaveAssembly(assembly, output);
        }
    }
}