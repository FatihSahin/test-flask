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
            
            #line 9 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DateTime.UtcNow.ToString()));
            
            #line default
            #line hidden
            this.Write("\t*\r\n*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t \t*\r\n*****************************************************" +
                    "***********************/\r\nusing System;\r\nusing System.Collections;\r\nusing System" +
                    ".Collections.Generic;\r\nusing System.Collections.Specialized;\r\nusing System.IO;\r\n" +
                    "using System.Linq;\r\nusing System.Reflection;\r\nusing System.Text;\r\nusing System.W" +
                    "eb;\r\nusing Microsoft.VisualStudio.TestTools.UnitTesting;\r\nusing Newtonsoft.Json;" +
                    "\r\nusing TestFlask.Aspects.ApiClient;\r\nusing TestFlask.Aspects.Enums;\r\nusing Test" +
                    "Flask.Models.Context;\r\nusing TestFlask.Models.Entity;\r\n\r\nnamespace Payments.Orde" +
                    "r.ManagementWebService.Test\r\n{\r\n\tpublic partial class TestFlaskTests\r\n\t{\r\n\t\t#reg" +
                    "ion ConventionalAutos\r\n\r\n\t\tJsonSerializerSettings jsonSerializerSettings = new J" +
                    "sonSerializerSettings\r\n\t\t{\r\n\t\t\tTypeNameHandling = TypeNameHandling.All,\r\n\t\t\tType" +
                    "NameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple\r\n\t\t};\r\n\r\n\t\tpr" +
                    "ivate void ProvideTestFlaskHttpContext(Invocation invocation)\r\n\t\t{\r\n\t\t\tHttpConte" +
                    "xt.Current = new HttpContext(\r\n\t\t\t\tnew HttpRequest(\"\", \"http://tempuri.org\", \"\")" +
                    ",\r\n\t\t\t\tnew HttpResponse(new StringWriter())\r\n\t\t\t\t);\r\n\r\n\t\t\t//In order to by pass " +
                    "Platform not supported exception\r\n\t\t\t//http://bigjimindc.blogspot.com.tr/2007/07" +
                    "/ms-kb928365-aspnet-requestheadersadd.html\r\n\t\t\tAddHeaderToRequest(HttpContext.Cu" +
                    "rrent.Request, ContextKeys.ProjectKey, invocation.ProjectKey);\r\n\t\t\tAddHeaderToRe" +
                    "quest(HttpContext.Current.Request, ContextKeys.ScenarioNo, invocation.ScenarioNo" +
                    ".ToString());\r\n\t\t\tAddHeaderToRequest(HttpContext.Current.Request, ContextKeys.St" +
                    "epNo, invocation.StepNo.ToString());\r\n\t\t\tAddHeaderToRequest(HttpContext.Current." +
                    "Request, ContextKeys.TestMode, TestModes.Assert.ToString());\r\n\t\t}\r\n\r\n\t\tprivate v" +
                    "oid AddHeaderToRequest(HttpRequest request, string key, string value)\r\n\t\t{\r\n\t\t\tN" +
                    "ameValueCollection headers = request.Headers;\r\n\r\n\t\t\tType t = headers.GetType();\r" +
                    "\n\t\t\tArrayList item = new ArrayList();\r\n\r\n\t\t\t// Remove read-only limitation on he" +
                    "aders\r\n\t\t\tt.InvokeMember(\"MakeReadWrite\", BindingFlags.InvokeMethod | BindingFla" +
                    "gs.NonPublic | BindingFlags.Instance, null, headers, null);\r\n\t\t\tt.InvokeMember(\"" +
                    "InvalidateCachedArrays\", BindingFlags.InvokeMethod | BindingFlags.NonPublic | Bi" +
                    "ndingFlags.Instance, null, headers, null);\r\n\t\t\titem.Add(value);\r\n\t\t\tt.InvokeMemb" +
                    "er(\"BaseAdd\", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags." +
                    "Instance, null, headers, new object[] { key, item });\r\n\t\t\tt.InvokeMember(\"MakeRe" +
                    "adOnly\", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Insta" +
                    "nce, null, headers, null);\r\n\t\t}\r\n\r\n\t\t//will be fully prepared on template\r\n\t\tpri" +
                    "vate Invocation GetRootInvocationFromApi(long stepNo)\r\n\t\t{\r\n\t\t\tTestFlaskApi api " +
                    "= new TestFlaskApi();\r\n\t\t\tStep step = api.LoadStep(stepNo);\r\n\t\t\treturn step.Invo" +
                    "cations.SingleOrDefault(inv => inv.Depth == 1);\r\n\t\t}\r\n\r\n\t\tprivate void HandleAss" +
                    "ertion(Invocation rootInvocation, object responseObject, Exception exception, Ac" +
                    "tion stepAssertion)\r\n\t\t{\r\n\t\t\tif ((!rootInvocation.IsFaulted && exception == null" +
                    ") || (rootInvocation.IsFaulted && exception != null))\r\n\t\t\t{\r\n\t\t\t\tstepAssertion()" +
                    ";\r\n\t\t\t}\r\n\t\t\telse if (exception != null)\r\n\t\t\t{\r\n\t\t\t\tstring exceptionStr = JsonCon" +
                    "vert.SerializeObject(exception, jsonSerializerSettings);\r\n\t\t\t\tAssert.Fail($\"Expe" +
                    "cted proper response of type {rootInvocation.ResponseType} but got exception =>{" +
                    "Environment.NewLine}{exceptionStr}\");\r\n\t\t\t}\r\n\t\t\telse\r\n\t\t\t{\r\n\t\t\t\tstring responseS" +
                    "tr = JsonConvert.SerializeObject(responseObject, jsonSerializerSettings);\r\n\t\t\t\tA" +
                    "ssert.Fail($\"Expected exception of type {rootInvocation.ExceptionType} but got r" +
                    "esponse =>{Environment.NewLine}{responseStr}\");\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\t#endregion\r\n\r\n\t\t" +
                    "#region Scenarios\r\n\t\t\r\n");
            
            #line 100 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

foreach (Scenario scenario in Scenarios) {
	Scenario deepScenario = Api.GetScenarioDeep(scenario.ScenarioNo);

            
            #line default
            #line hidden
            this.Write("\t\t[TestMethod]\r\n\t\t[TestCategory(\"TestFlask\")]\r\n\t\tprivate void Scenario");
            
            #line 106 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(deepScenario.ScenarioNo.ToString()));
            
            #line default
            #line hidden
            this.Write("_");
            
            #line 106 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(deepScenario.ScenarioName.Replace(" ", string.Empty)));
            
            #line default
            #line hidden
            this.Write("() {\r\n");
            
            #line 107 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	foreach (Step step in deepScenario.Steps) {

            
            #line default
            #line hidden
            this.Write("\t\t\tScenario");
            
            #line 110 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(deepScenario.ScenarioNo.ToString()));
            
            #line default
            #line hidden
            this.Write("_");
            
            #line 110 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(deepScenario.ScenarioName.Replace(" ", string.Empty)));
            
            #line default
            #line hidden
            this.Write("_Step");
            
            #line 110 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(step.StepNo.ToString()));
            
            #line default
            #line hidden
            this.Write("_");
            
            #line 110 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(step.StepName.Replace(" ", string.Empty)));
            
            #line default
            #line hidden
            this.Write("();\r\n");
            
            #line 111 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	}

            
            #line default
            #line hidden
            this.Write("\t\t} \r\n\r\n\t\t// Buraya step testler gelecek\r\n");
            
            #line 117 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
 
} 

	foreach (var subject in Subjects) { 
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 121 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subject.Key));
            
            #line default
            #line hidden
            this.Write(" subject");
            
            #line 121 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subject.Value));
            
            #line default
            #line hidden
            this.Write(";\t\r\n");
            
            #line 122 "D:\github\test-flask\TestFlask.CLI\UnitTestGen\T4\MSTestGen.tt"

	}

            
            #line default
            #line hidden
            this.Write(@"		Payments.Order.ManagementWebService.Interfaces.IPaymentOrderManagementService subjectIPaymentOrderManagementService;

		[TestMethod]
		[TestCategory(""TestFlask"")]
		public void Scenario59_EmptyFee()
		{
			Scenario59_EmptyFee_Step187_createRemOrderWithEmptyFee();
		}

		private void Scenario59_EmptyFee_Step187_createRemOrderWithEmptyFee()
		{
			long stepNo = 187;
			Invocation rootInvocation = GetRootInvocationFromApi(stepNo);

			ProvideTestFlaskHttpContext(rootInvocation);
			ProvideOperationContext(rootInvocation);

			//Direct casting of first elemnt of object array to first method arg.. need to improve here to handle multi args with dynamic type matching
			var requestObject = JsonConvert.DeserializeObject<object[]>(rootInvocation.Request, jsonSerializerSettings).First()
				as Payments.Order.ManagementWebService.ServiceContracts.CreatePaymentOrderRequest;

			Payments.Order.ManagementWebService.ServiceContracts.CreatePaymentOrderResponse responseObject = null;
			Exception exception = null;

			try
			{
				responseObject = subjectIPaymentOrderManagementService.createPaymentOrder(requestObject);
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			HandleAssertion(rootInvocation, responseObject, exception, () => AssertScenario59_Step187_createRemOrderWithEmptyFee(responseObject, exception));
		   
		}

		#endregion
	}
}
");
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