﻿Import-Module ScriptAnalyzer

$violationMessage = [regex]::Escape('Parameter $passphrases should use SecureString, otherwise this will expose sensitive information. See ConvertTo-SecureString for more information.')
$violationName = "PSAvoidUsingPlainTextForPassword"
$directory = Split-Path -Parent $MyInvocation.MyCommand.Path
$violations = Invoke-ScriptAnalyzer $directory\AvoidUsingPlainTextForPassword.ps1 | Where-Object {$_.RuleName -eq $violationName}
$noViolations = Invoke-ScriptAnalyzer $directory\AvoidUsingPlainTextForPasswordNoViolations.ps1 | Where-Object {$_.RuleName -eq $violationName}

Describe "AvoidUsingPlainTextForPassword" {
    Context "When there are violations" {
        It "has 3 avoid using plain text for password violations" {
            $violations.Count | Should Be 3
        }

        It "has the correct violation message" {
            $violations[0].Message | Should Match $violationMessage
        }
    }

    Context "When there are no violations" {
        It "returns no violations" {
            $noViolations.Count | Should Be 0
        }
    }
}