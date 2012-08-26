﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementFileService
	{
		void RemoveFile(string path);
		void RemoveDirectory(string path);
		bool IsOpen(string fileName);
		void OpenFile(string fileName);
		void CopyFile(string oldFileName, string newFileName);
		bool FileExists(string fileName);
		string[] GetFiles(string path);
		string[] GetDirectories(string path);
		
		void ParseFile(string fileName);
		ICompilationUnit GetCompilationUnit(string fileName);
	}
}
