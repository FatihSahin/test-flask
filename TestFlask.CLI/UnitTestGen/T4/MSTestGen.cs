// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 14.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace TestFlask.CLI.UnitTestGen.T4
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using TestFlask.Models.Entity;
    using TestFlask.CLI.Options;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public partial class MSTestGen : MSTestGenBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("/****************************************************************************\r\n*\t" +
                    "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t \t*\r\n*\tThis class is auto generated by TestFlask CLI on ");
            
            #line 10 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DateTime.Now.ToString()));
            
            #line default
            #line hidden
            this.Write(@"	    *
*	https://github.com/FatihSahin/test-flask                                *
*	Implement provider methods and step assertions inside another file.		*
*																		 	*
****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestFlask.Aspects.ApiClient;
using TestFlask.Aspects.Enums;
using TestFlask.Models.Context;
using TestFlask.Models.Entity;
using TestFlask.Aspects.Context;

namespace ");
            
            #line 34 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(options.Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [TestClass]\r\n\tpublic partial class ");
            
            #line 37 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(options.ClassName));
            
            #line default
            #line hidden
            this.Write(@"
	{
		#region Conventional

		private static IEnumerable<Scenario> embeddedScenarios { get; set; }

		private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
		};

		[ClassInitialize]
		public static void ClassSetUp(TestContext context)
		{ 
			embeddedScenarios = ReadEmbeddedScenarios();
			DoClassSetUp(context);
		}
		
		[ClassCleanup]
		public static void ClassTearDown() {
			embeddedScenarios = null;
			DoClassTearDown();
		}

		private static IEnumerable<Scenario> ReadEmbeddedScenarios()
		{
			string fileName = """);
            
            #line 64 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetEmbedFileName()));
            
            #line default
            #line hidden
            this.Write(@""";

			if (!File.Exists(fileName))
			{
				return null;
			}

			List<Scenario> embeddedScenarios = new List<Scenario>();

			string line;
			using (System.IO.StreamReader fileReader = new System.IO.StreamReader(fileName))
			{
				while ((line = fileReader.ReadLine()) != null)
				{
					var json = TestFlask.Models.Utils.CompressUtil.DecompressString(line);
					var scenario = JsonConvert.DeserializeObject<Scenario>(json);
					embeddedScenarios.Add(scenario);
				}
			}

			return embeddedScenarios;
		}

		private void ProvideTestFlaskHttpContext(Step step)
		{
			HttpContext.Current = new HttpContext(
				new HttpRequest("""", ""http://tempuri.org"", """"),
				new HttpResponse(new StringWriter())
				);

			var invocation = step.GetRootInvocation();

			// In order to by pass Platform not supported exception
			// http://bigjimindc.blogspot.com.tr/2007/07/ms-kb928365-aspnet-requestheadersadd.html
			AddHeaderToRequest(HttpContext.Current.Request, ContextKeys.ProjectKey, invocation.ProjectKey);
			AddHeaderToRequest(HttpContext.Current.Request, ContextKeys.ScenarioNo, invocation.ScenarioNo.ToString());
			AddHeaderToRequest(HttpContext.Current.Request, ContextKeys.StepNo, invocation.StepNo.ToString());
			AddHeaderToRequest(HttpContext.Current.Request, ContextKeys.TestMode, ");
            
            #line 101 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTestMode()));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n            TestFlaskContext.LoadedStep = step;\r\n\t\t}\r\n\r\n\t\tprivate void AddH" +
                    "eaderToRequest(HttpRequest request, string key, string value)\r\n\t\t{\r\n\t\t\tNameValue" +
                    "Collection headers = request.Headers;\r\n\r\n\t\t\tType t = headers.GetType();\r\n\t\t\tArra" +
                    "yList item = new ArrayList();\r\n\r\n\t\t\t// Remove read-only limitation on headers\r\n\t" +
                    "\t\tt.InvokeMember(\"MakeReadWrite\", BindingFlags.InvokeMethod | BindingFlags.NonPu" +
                    "blic | BindingFlags.Instance, null, headers, null);\r\n\t\t\tt.InvokeMember(\"Invalida" +
                    "teCachedArrays\", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFla" +
                    "gs.Instance, null, headers, null);\r\n\t\t\titem.Add(value);\r\n\t\t\tt.InvokeMember(\"Base" +
                    "Add\", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance" +
                    ", null, headers, new object[] { key, item });\r\n\t\t\tt.InvokeMember(\"MakeReadOnly\"," +
                    " BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, nul" +
                    "l, headers, null);\r\n\t\t}\r\n\r\n\t\tprivate Step GetLoadedStep(long stepNo)\r\n\t\t{\r\n\t\t\tSt" +
                    "ep step = embeddedScenarios?.SelectMany(sc => sc.Steps).SingleOrDefault(st => st" +
                    ".StepNo == stepNo);\r\n\r\n\t\t\tif (step == null)\r\n\t\t\t{\r\n\t\t\t\tTestFlaskApi api = new Te" +
                    "stFlaskApi();\r\n\t\t\t\tstep = api.LoadStep(stepNo);\r\n\t\t\t}\r\n\r\n\t\t\treturn step;\r\n\t\t}\r\n\r" +
                    "\n        private void HandleAssertion(Invocation rootInvocation, object response" +
                    "Object, Exception exception, Action stepAssertion)\r\n        {\r\n            if ((" +
                    "!rootInvocation.IsFaulted && exception == null) || (rootInvocation.IsFaulted && " +
                    "exception != null))\r\n            {\r\n                stepAssertion();\r\n          " +
                    "  }\r\n            else if (exception != null)\r\n            {\r\n                str" +
                    "ing exceptionStr = JToken.Parse(JsonConvert.SerializeObject(exception, jsonSeria" +
                    "lizerSettings)).ToString(Formatting.Indented);\r\n                Assert.Fail($\"Ex" +
                    "pected proper response of type {rootInvocation.ResponseType} but got exception =" +
                    ">{Environment.NewLine}{exceptionStr}{Environment.NewLine}{GetExceptionStackOutpu" +
                    "t()}\");\r\n            }\r\n            else\r\n            {\r\n                string " +
                    "responseStr = JToken.Parse(JsonConvert.SerializeObject(responseObject, jsonSeria" +
                    "lizerSettings)).ToString(Formatting.Indented);\r\n                Assert.Fail($\"Ex" +
                    "pected exception of type {rootInvocation.ExceptionType} but got response =>{Envi" +
                    "ronment.NewLine}{responseStr}\");\r\n            }\r\n        }\r\n\r\n        private st" +
                    "ring GetExceptionStackOutput()\r\n        {\r\n            StringBuilder strBuilder " +
                    "= new StringBuilder();\r\n            IEnumerable<Invocation> exceptionalInvocatio" +
                    "ns = TestFlaskContext.InvocationStack.ExceptionStack;\r\n\r\n            strBuilder." +
                    "AppendLine(\"**** TestFlask Exception Stack Snapshot ****\");\r\n            foreach" +
                    " (var excInv in exceptionalInvocations)\r\n            {\r\n                strBuild" +
                    "er.AppendLine(\"\\t**** Faulty Invocation ****\");\r\n                strBuilder.Appe" +
                    "ndLine($\"\\t\\tMethod => {excInv.InvocationSignature}\");\r\n                strBuild" +
                    "er.AppendLine($\"\\t\\tInvocation Mode => {excInv.InvocationMode}\");\r\n             " +
                    "   if (!string.IsNullOrWhiteSpace(excInv.RequestDisplayInfo))\r\n                {" +
                    "\r\n                    strBuilder.AppendLine($\"\\t\\tRequest Info => {excInv.Reques" +
                    "tDisplayInfo}\");\r\n                }\r\n                strBuilder.AppendLine($\"\\t\\" +
                    "tRequest => \");\r\n                strBuilder.AppendLine(JToken.Parse(excInv.Reque" +
                    "st).ToString(Formatting.Indented));\r\n                strBuilder.AppendLine($\"\\t\\" +
                    "tExceptionType => {excInv.ExceptionType}\");\r\n                strBuilder.AppendLi" +
                    "ne($\"\\t\\tException => \");\r\n                strBuilder.AppendLine(JToken.Parse(ex" +
                    "cInv.Exception).ToString(Formatting.Indented));\r\n            }\r\n\r\n            re" +
                    "turn strBuilder.ToString();\r\n        }\r\n\r\n\t\t#endregion\r\n\r\n\t\t#region Scenarios\r\n\t" +
                    "\t\r\n");
            
            #line 181 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

foreach (Scenario scenario in Scenarios) {
	Scenario deepScenario = LoadScenario(scenario.ScenarioNo);

            
            #line default
            #line hidden
            this.Write("\t\t[TestMethod]\r\n\t\t[TestCategory(\"TestFlask\")]\r\n\t\tpublic void ");
            
            #line 187 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetScenarioTestMethodName(deepScenario)));
            
            #line default
            #line hidden
            this.Write("() {\r\n");
            
            #line 188 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	foreach (Step step in deepScenario.Steps) {

            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 191 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetStepAssertMethodName(deepScenario, step)));
            
            #line default
            #line hidden
            this.Write("();\r\n");
            
            #line 192 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	}

            
            #line default
            #line hidden
            this.Write("\t\t} \r\n\r\n");
            
            #line 197 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	foreach (Step step in deepScenario.Steps) {
		var rootInvocation = step.GetRootInvocation();
		signatureMatch = signatureRegex.Match(rootInvocation.InvocationSignature);

            
            #line default
            #line hidden
            this.Write("\t\tprivate void ");
            
            #line 202 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetStepAssertMethodName(deepScenario, step)));
            
            #line default
            #line hidden
            this.Write("() {\r\n\t\t\t\r\n\t\t\tlong stepNo = ");
            
            #line 204 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(step.StepNo));
            
            #line default
            #line hidden
            this.Write(@";
			Step loadedStep = GetLoadedStep(stepNo);

			var rootInvocation = loadedStep.GetRootInvocation();

			ProvideTestFlaskHttpContext(loadedStep);
			ProvideOperationContext(rootInvocation);

			var requestObject = JsonConvert.DeserializeObject<object[]>(rootInvocation.Request, jsonSerializerSettings).First()
				as ");
            
            #line 213 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRequestTypeName()));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n\t\t\t");
            
            #line 215 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetResponseTypeName()));
            
            #line default
            #line hidden
            this.Write(" responseObject = null;\r\n\t\t\tException exception = null;\r\n\t\t\t\r\n\t\t\ttry\r\n\t\t\t{\r\n\t\t\t\tr" +
                    "esponseObject = subject");
            
            #line 220 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetSubjectValue(step)));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 220 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRootMethodName()));
            
            #line default
            #line hidden
            this.Write("(requestObject);\r\n\t\t\t}\r\n\t\t\tcatch (Exception ex)\r\n\t\t\t{\r\n\t\t\t\texception = ex;\r\n\t\t\t}\r" +
                    "\n\r\n\t\t\tHandleAssertion(rootInvocation, responseObject, exception, () => Assert");
            
            #line 227 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetStepAssertMethodName(deepScenario, step)));
            
            #line default
            #line hidden
            this.Write("(responseObject, exception));\r\n\t\t}\r\n\r\n");
            
            #line 230 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	}

            
            #line default
            #line hidden
            
            #line 233 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
 
} 

	foreach (var subject in Subjects) { 
            
            #line default
            #line hidden
            this.Write("\t\tprivate ");
            
            #line 237 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subject.Key));
            
            #line default
            #line hidden
            this.Write(" subject");
            
            #line 237 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subject.Value));
            
            #line default
            #line hidden
            this.Write(";\t\r\n");
            
            #line 238 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	}

            
            #line default
            #line hidden
            this.Write("\r\n\t\t#endregion\r\n\r\n\t}\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public class MSTestGenBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
