﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASM.net.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ASM.net.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to namespace TestProgram
        ///{
        ///	public class Test
        ///	{
        ///		mov ecx, 1000 ;freq
        ///		mov bx, 0 ;duration
        ///		
        ///		push ecx
        ///		push bx
        ///		call beep
        ///		dec ecx
        ///		inc bx
        ///		jmp 18h
        ///	}
        ///}.
        /// </summary>
        internal static string BeepExample {
            get {
                return ResourceManager.GetString("BeepExample", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap build {
            get {
                object obj = ResourceManager.GetObject("build", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to namespace TestProgram
        ///{
        ///	public class Test
        ///	{
        ///		;set video mode: 320*200 - 256 colors
        ///		MOV ah, 0
        ///		mov al, 13h
        ///		int 10
        ///		
        ///		;X/Y offset
        ///		mov cx, 10
        ///		mov dx, 10
        ///		mov bx, 0
        ///		mov al, 15
        ///		
        ///		
        ///		;draw top
        ///		mov ah, ch
        ///		int 10
        ///		inc cx
        ///		cmp cx, 35
        ///		jnz 33h
        ///		
        ///		;reset
        ///		mov cx, 10
        ///		mov dx, 30
        ///		
        ///		;draw bottom
        ///		mov ah, ch
        ///		int 10
        ///		inc cx
        ///		cmp cx, 35
        ///		jnz 5eh
        ///		
        ///		;reset
        ///		mov cx, 10
        ///		mov dx, 10
        ///		
        ///		;draw left
        ///		mov ah, ch
        ///		int 10
        ///		inc dx
        ///		cmp dx, 30
        ///		jnz 89h
        ///				
        ///		;reset
        ///		mov cx, 35
        ///		mov  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DrawBoxExample {
            get {
                return ResourceManager.GetString("DrawBoxExample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to namespace TestProgram
        ///{
        ///	public class Test
        ///	{
        ///		;reset registers
        ///		mov ecx, 0
        ///		mov eax, 0
        ///		
        ///		inc eax
        ///		cmp eax, 255
        ///		jnz 18h
        ///		cmp ecx, 255
        ///		inc ecx
        ///		jnz 12h
        ///		
        ///		;done looping 65025 times
        ///		writel &quot;Done looping&quot;
        ///	}
        ///}.
        /// </summary>
        internal static string LoopExample {
            get {
                return ResourceManager.GetString("LoopExample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to namespace TestProgram
        ///{
        ///	public class Test
        ///	{
        ///		push NULL
        ///		push &quot;title&quot;
        ///		push &quot;message&quot;
        ///		push MB_YESNO
        ///		call MessageBox
        ///	}
        ///}.
        /// </summary>
        internal static string MessageBoxExample {
            get {
                return ResourceManager.GetString("MessageBoxExample", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap play {
            get {
                object obj = ResourceManager.GetObject("play", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap save_all {
            get {
                object obj = ResourceManager.GetObject("save-all", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap saveas {
            get {
                object obj = ResourceManager.GetObject("saveas", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}