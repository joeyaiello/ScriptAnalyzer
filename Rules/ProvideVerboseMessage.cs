﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation.Language;
using Microsoft.Windows.Powershell.ScriptAnalyzer.Generic;
using System.ComponentModel.Composition;
using System.Resources;
using System.Globalization;
using System.Threading;
using System.Reflection;

namespace Microsoft.Windows.Powershell.ScriptAnalyzer.BuiltinRules
{
    /// <summary>
    /// ProvideVerboseMessage: Analyzes the ast to check that Write-Verbose is called at least once in every cmdlet or script.
    /// </summary>
    [Export(typeof(IScriptRule))]
    public class ProvideVerboseMessage : SkipNamedBlock, IScriptRule
    {
        /// <summary>
        /// AnalyzeScript: Analyzes the ast to check that Write-Verbose is called at least once in every cmdlet or script.
        /// <param name="ast">The script's ast</param>
        /// <param name="fileName">The script's file name</param>
        /// </summary>
        public IEnumerable<DiagnosticRecord> AnalyzeScript(Ast ast, string fileName)
        {
            if (ast == null) throw new ArgumentNullException(Strings.NullAstErrorMessage);
            
            ClearList();
            this.AddNames(new List<string>() { "Configuration", "Workflow" });
            DiagnosticRecords.Clear();
            
            this.fileName = fileName;
            //We only check that advanced functions should have Write-Verbose
            ast.Visit(this);

            return DiagnosticRecords;
        }

        /// <summary>
        /// Visit function and checks that it has write verbose
        /// </summary>
        /// <param name="funcAst"></param>
        /// <returns></returns>
        public override AstVisitAction VisitFunctionDefinition(FunctionDefinitionAst funcAst)
        {
            if (funcAst == null)
            {
                return AstVisitAction.SkipChildren;
            }

            var commandAsts = funcAst.Body.FindAll(testAst => testAst is CommandAst, false);
            bool hasVerbose = false;

            if (commandAsts != null)
            {
                foreach (CommandAst commandAst in commandAsts)
                {
                    hasVerbose |= String.Equals(commandAst.GetCommandName(), "Write-Verbose", StringComparison.OrdinalIgnoreCase);
                }
            }

            if (!hasVerbose)
            {
                DiagnosticRecords.Add(new DiagnosticRecord(string.Format(CultureInfo.CurrentCulture, Strings.ProvideVerboseMessageErrorFunction, funcAst.Name),
                    funcAst.Extent, GetName(), DiagnosticSeverity.Strict, fileName));
            }

            return AstVisitAction.Continue;
        }

        /// <summary>
        /// Method: Retrieves the name of this rule.
        /// </summary>
        public string GetName()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.NameSpaceFormat, GetSourceName(), Strings.ProvideVerboseMessageName);
        }

        /// <summary>
        /// GetCommonName: Retrieves the common name of this rule.
        /// </summary>
        /// <returns>The common name of this rule</returns>
        public string GetCommonName()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.ProvideVerboseMessageCommonName);
        }

        /// <summary>
        /// Method: Retrieves the description of this rule.
        /// </summary>
        /// <returns>The description of this rule</returns>
        public string GetDescription()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.ProvideVerboseMessageDescription);
        }

        /// <summary>
        /// Method: Retrieves the type of the rule: builtin, managed or module.
        /// </summary>
        public SourceType GetSourceType()
        {
            return SourceType.Builtin;
        }

        /// <summary>
        /// Method: Retrieves the module/assembly name the rule is from.
        /// </summary>
        public string GetSourceName()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.SourceName);
        }
    }
}
