﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveSysModuleTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("sys", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"import sys\r\n" +
				"sys\r\n" +
				"\r\n";
		}
		
		[Test]
		public void CompilationUnitHasSysModuleInUsingsCollection()
		{
			Assert.AreEqual("sys", compilationUnit.UsingScope.Usings[0].Usings[0]);
		}
		
		[Test]
		public void ResolveResultContainsExitMethod()
		{
			List<ICompletionEntry> items = GetCompletionItems();
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("exit", items);
			Assert.IsNotNull(method);
		}
		
		List<ICompletionEntry> GetCompletionItems()
		{
			return resolveResult.GetCompletionData(projectContent);
		}
		
		[Test]
		public void MathModuleExpressionShouldNotHaveAnyCompletionItemsSinceMathModuleIsNotImported()
		{
			ExpressionResult result = new ExpressionResult("math", ExpressionContext.Default);
			resolveResult = resolver.Resolve(result, parseInfo, GetPythonScript());
			
			Assert.IsNull(resolveResult);
		}
	}
}
