// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
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
            this.Write("\r\n\t{\r\n        private bool isEmbedded = ");
            
            #line 39 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetIsEmbedded()));
            
            #line default
            #line hidden
            this.Write(@";

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
            
            #line 66 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetEmbedFileName()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n\t\t\tif (!File.Exists(fileName))\r\n\t\t\t{\r\n\t\t\t\treturn null;\r\n\t\t\t}\r\n\r\n\t\t\tList<Sce" +
                    "nario> embeddedScenarios = new List<Scenario>();\r\n\r\n\t\t\tstring line;\r\n\t\t\tusing (S" +
                    "ystem.IO.StreamReader fileReader = new System.IO.StreamReader(fileName))\r\n\t\t\t{\r\n" +
                    "\t\t\t\twhile ((line = fileReader.ReadLine()) != null)\r\n\t\t\t\t{\r\n\t\t\t\t\tvar json = TestF" +
                    "lask.Models.Utils.CompressUtil.DecompressString(line);\r\n\t\t\t\t\tvar scenario = Json" +
                    "Convert.DeserializeObject<Scenario>(json);\r\n\t\t\t\t\tembeddedScenarios.Add(scenario)" +
                    ";\r\n\t\t\t\t}\r\n\t\t\t}\r\n\r\n\t\t\treturn embeddedScenarios;\r\n\t\t}\r\n\r\n\t\tprivate void ProvideTes" +
                    "tFlaskHttpContext(Step step, TestModes testMode)\r\n\t\t{\r\n\t\t\tHttpContext.Current = " +
                    "new HttpContext(\r\n\t\t\t\tnew HttpRequest(\"\", \"http://tempuri.org\", \"\"),\r\n\t\t\t\tnew Ht" +
                    "tpResponse(new StringWriter())\r\n\t\t\t\t);\r\n\r\n\t\t\tvar invocation = step.GetRootInvoca" +
                    "tion();\r\n\r\n\t\t\t// In order to by pass Platform not supported exception\r\n\t\t\t// htt" +
                    "p://bigjimindc.blogspot.com.tr/2007/07/ms-kb928365-aspnet-requestheadersadd.html" +
                    "\r\n\t\t\tAddHeaderToRequest(HttpContext.Current.Request, ContextKeys.ProjectKey, inv" +
                    "ocation.ProjectKey);\r\n\t\t\tAddHeaderToRequest(HttpContext.Current.Request, Context" +
                    "Keys.ScenarioNo, invocation.ScenarioNo.ToString());\r\n\t\t\tAddHeaderToRequest(HttpC" +
                    "ontext.Current.Request, ContextKeys.StepNo, invocation.StepNo.ToString());\r\n\t\t\tA" +
                    "ddHeaderToRequest(HttpContext.Current.Request, ContextKeys.TestMode, testMode.To" +
                    "String());\r\n\r\n            TestFlaskContext.LoadedStep = step;\r\n\t\t}\r\n\r\n\t\tprivate " +
                    "void AddHeaderToRequest(HttpRequest request, string key, string value)\r\n\t\t{\r\n\t\t\t" +
                    "NameValueCollection headers = request.Headers;\r\n\r\n\t\t\tType t = headers.GetType();" +
                    "\r\n\t\t\tArrayList item = new ArrayList();\r\n\r\n\t\t\t// Remove read-only limitation on h" +
                    "eaders\r\n\t\t\tt.InvokeMember(\"MakeReadWrite\", BindingFlags.InvokeMethod | BindingFl" +
                    "ags.NonPublic | BindingFlags.Instance, null, headers, null);\r\n\t\t\tt.InvokeMember(" +
                    "\"InvalidateCachedArrays\", BindingFlags.InvokeMethod | BindingFlags.NonPublic | B" +
                    "indingFlags.Instance, null, headers, null);\r\n\t\t\titem.Add(value);\r\n\t\t\tt.InvokeMem" +
                    "ber(\"BaseAdd\", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags" +
                    ".Instance, null, headers, new object[] { key, item });\r\n\t\t\tt.InvokeMember(\"MakeR" +
                    "eadOnly\", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Inst" +
                    "ance, null, headers, null);\r\n\t\t}\r\n\r\n        private Step GetLoadedStep(long step" +
                    "No, bool isEmbedded)\r\n        {\r\n            Step step = null;\r\n\r\n            if" +
                    " (isEmbedded)\r\n            {\r\n                step = embeddedScenarios?.SelectMa" +
                    "ny(sc => sc.Steps).SingleOrDefault(st => st.StepNo == stepNo);\r\n            }\r\n\r" +
                    "\n            if (step == null)\r\n            {\r\n                TestFlaskApi api " +
                    "= new TestFlaskApi();\r\n                step = api.LoadStep(stepNo);\r\n           " +
                    " }\r\n\r\n            return step;\r\n        }\r\n\r\n        private void HandleAssertio" +
                    "n(Invocation rootInvocation, object responseObject, Exception exception, Action " +
                    "stepAssertion)\r\n        {\r\n            if ((!rootInvocation.IsFaulted && excepti" +
                    "on == null) || (rootInvocation.IsFaulted && exception != null))\r\n            {\r\n" +
                    "                stepAssertion();\r\n            }\r\n            else if (exception " +
                    "!= null)\r\n            {\r\n                string exceptionStr = JToken.Parse(Json" +
                    "Convert.SerializeObject(exception, jsonSerializerSettings)).ToString(Formatting." +
                    "Indented);\r\n                Assert.Fail($\"Expected proper response of type {root" +
                    "Invocation.ResponseType} but got exception =>{Environment.NewLine}{exceptionStr}" +
                    "{Environment.NewLine}{GetExceptionStackOutput()}\");\r\n            }\r\n            " +
                    "else\r\n            {\r\n                string responseStr = JToken.Parse(JsonConve" +
                    "rt.SerializeObject(responseObject, jsonSerializerSettings)).ToString(Formatting." +
                    "Indented);\r\n                Assert.Fail($\"Expected exception of type {rootInvoca" +
                    "tion.ExceptionType} but got response =>{Environment.NewLine}{responseStr}\");\r\n  " +
                    "          }\r\n        }\r\n\r\n        private string GetExceptionStackOutput()\r\n    " +
                    "    {\r\n            StringBuilder strBuilder = new StringBuilder();\r\n            " +
                    "IEnumerable<Invocation> exceptionalInvocations = TestFlaskContext.InvocationStac" +
                    "k.ExceptionStack;\r\n\r\n            strBuilder.AppendLine(\"**** TestFlask Exception" +
                    " Stack Snapshot ****\");\r\n            foreach (var excInv in exceptionalInvocatio" +
                    "ns)\r\n            {\r\n                strBuilder.AppendLine(\"\\t**** Faulty Invocat" +
                    "ion ****\");\r\n                strBuilder.AppendLine($\"\\t\\tMethod => {excInv.Invoc" +
                    "ationSignature}\");\r\n                strBuilder.AppendLine($\"\\t\\tInvocation Mode " +
                    "=> {excInv.InvocationMode}\");\r\n                if (!string.IsNullOrWhiteSpace(ex" +
                    "cInv.RequestDisplayInfo))\r\n                {\r\n                    strBuilder.App" +
                    "endLine($\"\\t\\tRequest Info => {excInv.RequestDisplayInfo}\");\r\n                }\r" +
                    "\n                strBuilder.AppendLine($\"\\t\\tRequest => \");\r\n                str" +
                    "Builder.AppendLine(JToken.Parse(excInv.Request).ToString(Formatting.Indented));\r" +
                    "\n                strBuilder.AppendLine($\"\\t\\tExceptionType => {excInv.ExceptionT" +
                    "ype}\");\r\n                strBuilder.AppendLine($\"\\t\\tException => \");\r\n         " +
                    "       strBuilder.AppendLine(JToken.Parse(excInv.Exception).ToString(Formatting." +
                    "Indented));\r\n            }\r\n\r\n            return strBuilder.ToString();\r\n       " +
                    " }\r\n\r\n        private Invocation PrepareStep(long stepNo, TestModes testMode, bo" +
                    "ol isEmbedded) \r\n        {\r\n            Step loadedStep = GetLoadedStep(stepNo, " +
                    "isEmbedded);\r\n\t\t\tvar rootInvocation = loadedStep.GetRootInvocation();\r\n\t\t\tProvid" +
                    "eTestFlaskHttpContext(loadedStep, testMode);\r\n\t\t\tProvideOperationContext(rootInv" +
                    "ocation);\r\n            return rootInvocation;\r\n        }\r\n\r\n\t\t#endregion\r\n\t\t\r\n");
            
            #line 195 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

foreach (Scenario scenario in Scenarios) {
	Scenario deepScenario = LoadScenario(scenario.ScenarioNo);

            
            #line default
            #line hidden
            this.Write("        #region ");
            
            #line 199 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetScenarioTestMethodName(deepScenario)));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n\t\t[TestMethod]\r\n\t\t[TestCategory(\"TestFlask\")]\r\n\t\tpublic void ");
            
            #line 203 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetScenarioTestMethodName(deepScenario)));
            
            #line default
            #line hidden
            this.Write("() {\r\n");
            
            #line 204 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	foreach (Step step in deepScenario.Steps) {

            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 207 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetStepAssertMethodName(deepScenario, step)));
            
            #line default
            #line hidden
            this.Write("();\r\n");
            
            #line 208 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	}

            
            #line default
            #line hidden
            this.Write("\t\t} \r\n\r\n");
            
            #line 213 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	foreach (Step step in deepScenario.Steps) {
		var rootInvocation = step.GetRootInvocation();
		signatureMatch = signatureRegex.Match(rootInvocation.InvocationSignature);

            
            #line default
            #line hidden
            this.Write("\t\tprivate void ");
            
            #line 218 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetStepAssertMethodName(deepScenario, step)));
            
            #line default
            #line hidden
            this.Write("() \r\n        {\t\t\t\r\n            var rootInvocation = PrepareStep(");
            
            #line 220 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(step.StepNo));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 220 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTestMode()));
            
            #line default
            #line hidden
            this.Write(", isEmbedded);\t\r\n\t\t\tvar requestObject = JsonConvert.DeserializeObject<object[]>(r" +
                    "ootInvocation.Request, jsonSerializerSettings).First() as ");
            
            #line 221 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRequestTypeName()));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n            //Set up additional behaviour for method args\r\n            SetUp" +
                    "_");
            
            #line 224 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetStepAssertMethodName(deepScenario, step)));
            
            #line default
            #line hidden
            this.Write("(requestObject);\r\n\r\n\t\t\t");
            
            #line 226 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetResponseTypeName()));
            
            #line default
            #line hidden
            this.Write(" responseObject = null;\r\n\t\t\tException exception = null;\r\n\t\t\t\r\n\t\t\ttry { responseOb" +
                    "ject = subject");
            
            #line 229 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetSubjectValue(step)));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 229 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRootMethodName()));
            
            #line default
            #line hidden
            this.Write("(requestObject); }\r\n\t\t\tcatch (Exception ex) { exception = ex; }\r\n\r\n\t\t\tHandleAsser" +
                    "tion(rootInvocation, responseObject, exception, \r\n                () => Assert_");
            
            #line 233 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetStepAssertMethodName(deepScenario, step)));
            
            #line default
            #line hidden
            this.Write("(responseObject, exception));\r\n\t\t}\r\n\r\n");
            
            #line 236 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	}

            
            #line default
            #line hidden
            this.Write("        #endregion\r\n\r\n");
            
            #line 241 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
 
} 

	foreach (var subject in Subjects) { 
            
            #line default
            #line hidden
            this.Write("\t\tprivate ");
            
            #line 245 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subject.Key));
            
            #line default
            #line hidden
            this.Write(" subject");
            
            #line 245 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subject.Value));
            
            #line default
            #line hidden
            this.Write(";\t\r\n");
            
            #line 246 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	}

            
            #line default
            #line hidden
            this.Write("\t}\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
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
