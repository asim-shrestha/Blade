using UnityEditor;

using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;


//#if ENABLE_VSTU //not defined in VS for me for some reason
using SyntaxTree.VisualStudio.Unity.Bridge;

[InitializeOnLoad]
public class ProjectFileHook {
	private const string SCHEMA = @"http://schemas.microsoft.com/developer/msbuild/2003";
	private const string ASSEMBLY_NAME = "AssemblyName";//XML category for assembly
	private const string ASSEMBLY_CSHARP_RUNTIME = "Assembly-CSharp";//value for assembly name
	private const string TARGET_FRAMEWORK_VERSION = "TargetFrameworkVersion";//XML category for .net framework
	private const string FRAMEWORK_EXPECTED = "v4.7.1";//old value for framework
	private const string FRAMEWORK_DESIRED = "v4.6.1";//new value for framework

	class Utf8StringWriter : StringWriter {//to allow UTF8 saving
		public override Encoding Encoding {
			get { return Encoding.UTF8; }
		}
	}

	static ProjectFileHook() {
		ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) => {
			var document = XDocument.Parse(content);
			var assemblyName = document.Descendants(XName.Get(ASSEMBLY_NAME, SCHEMA)).FirstOrDefault();
			if (null != assemblyName && assemblyName.Value.Contains(ASSEMBLY_CSHARP_RUNTIME)) {
				var target = document.Descendants(XName.Get(TARGET_FRAMEWORK_VERSION, SCHEMA)).FirstOrDefault();
				if (null != target && target.Value.Contains(FRAMEWORK_EXPECTED)) {
					target.SetValue(FRAMEWORK_DESIRED);
				}
			}
			var str = new Utf8StringWriter();
			document.Save(str);

			return str.ToString();
		};
	}
}
//#endif