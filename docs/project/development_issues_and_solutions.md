# Issues and Solutions
This document intends to outline some common errors you may run into while developing your own Type Provider. This document is not exhaustive and only contains errors with known causes or solutions. 

The issues covered in this document include:

* **FS3033**:
	* The type provider .. reported an error in the context of provided type .., member .. The error: The design-time type .. utilized by a type provider was not found in the target reference assembly set ..
* **MSB3026**/**MSB3027**:
	* Could not copy .. to .. . Exceeded retry count of 10. Failed. The file is locked by: "Microsoft Visual Studio 2019 (24320)" RunTime C:\Program Files (x86)\MicrosoftVisualStudio\2019\Community\MSBuild\Current\Bin\Microsoft.Common.CurrentVersion.targets	4678	


## FS3033
### The type provider .. reported an error in the context of provided type .., member .. The error: The design-time type .. utilized by a type provider was not found in the target reference assembly set ..

**Example:**

	ProviderTests.fsx(8,1): error FS3033: The type provider 'BioProviders.DesignTime.GenBankProvider' reported an error in the context of provided type 'GenBank.AssemblyProvider,Taxon="bacteria",Species="Staphylococcus_borealis",Assembly="GCA_003042555.1_ASM304255v1"', member 'LoadGBFF'. The error: The design-time type 'BioProviders.DesignTime.TypeGenerator' utilized by a type provider was not found in the target reference assembly set '[
		 tgt assembly FSharp.Core, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;
		 tgt assembly System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;
		 tgt assembly System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;
		 tgt assembly System.Runtime.Remoting, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;
		 tgt assembly System.Runtime.Serialization.Formatters.Soap, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;
		 tgt assembly System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;
		 tgt assembly System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly FSharp.Compiler.Interactive.Settings, Version=11.4.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.ValueTuple, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51;
		 tgt assembly netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51;
		 tgt assembly System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Reflection, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Linq.Expressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Threading.Tasks, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.IO, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Net.Requests, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Collections, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Runtime.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Threading, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;
		 tgt assembly System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;
		 tgt assembly System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;
		 tgt assembly FSharp.Data.BioProviders, Version=1.0.0.0, Culture=neutral]'. 
	 You may be referencing a profile which contains fewer types than those needed by the type provider you are using.
	 
**Explanation:**
This  error occurs when the contents of your Type Provider quotations cannot be found within the RunTime component. In the above example, the error is thrown due to the following code:

	module internal TypeGenerator =
		...
		let load = 1
		let loadGBFF = ProvidedMethod(..., invokeCode = (fun _ -> <@@ load @@>),...)
		providerType.AddMember loadGBFF
		...

Here, the module `TypeGenerator` is within the DesignTime component of a Type Provider with a split DesignTime-RunTime design. As `let load = 1` is part of the `TypeGenerator` module, it is not found in the RunTime component, causing the **FS3033** error.

**Solution:**
The contents of your Type Provider [quotations](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/code-quotations) should be implemented in your RunTime component. For example, the `load` function from the quotation `<@@ loadFile @@>` should be moved to the RunTime component.

This error may become prevalent if your attempt to return your own data types from a Type Provider. In this case, please read the documentation [here](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/creating-a-type-provider#choosing-representations-for-erased-provided-types).

## MSB3026/MSB3027
### Could not copy .. to .. . Exceeded retry count of 10. Failed. The file is locked by: "Microsoft Visual Studio 2019 (24320)" RunTime C:\Program Files (x86)\MicrosoftVisualStudio\2019\Community\MSBuild\Current\Bin\Microsoft.Common.CurrentVersion.targets	4678	

**Example:**

	Could not copy "D:\Work\Storage\Bio Type Providers\Production\Code\BioProviders\bin\Debug\typeproviders\fsharp5\netstandard2.0\FSharp.Data.BioProviders.DesignTime.dll" 
	to "..\..\bin\Debug\netstandard2.0\FSharp.Data.BioProviders.DesignTime.dll".
	Exceeded retry count of 10. Failed. 
	The file is locked by: "Microsoft Visual Studio 2019 (24320)"	RunTime 
	C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\Microsoft.Common.CurrentVersion.targets	4678	

**Explanation:**
This error occurs when building a solution, if the specified `.dll` is locked. In the above example, `..\..\bin\Debug\netstandard2.0\FSharp.Data.BioProviders.DesignTime.dll` is locked during the build process.

This is particularly common in Type Providers as explained by [Sergey Tihon](https://sergeytihon.com/tag/type-providers/) ".. when you reference the Type Provider dll, IDE/Intellisense loads this assembly and locks the file. After that, you will not be able to rebuild your Type Provider anymore until you close IDE."

**Solution:**
The simplest solution to this issue is to close and reopen your IDE. Other solutions can be found [here](https://sergeytihon.com/tag/type-providers/).