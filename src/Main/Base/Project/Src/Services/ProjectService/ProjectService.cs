using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ProjectService
	{
		static Solution openSolution   = null;
		static IProject currentProject = null;
		
		public static Solution OpenSolution {
			get {
				return openSolution;
			}
		}
		
		public static IProject CurrentProject {
			get {
				return currentProject;
			}
			set {
				if (currentProject != value) {
					currentProject = value;
					OnCurrentProjectChanged(new ProjectEventArgs(currentProject));
				}
			}
		}
		
		public static IProject GetProject(string filename)
		{
			filename = Path.GetFullPath(filename).ToLower();
			foreach (IProject project in OpenSolution.Projects) {
				if (project.FileName.ToLower() == filename) {
					return project;
				}
			}
			return null;
		}
		
		static ProjectService()
		{
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += new EventHandler(ActiveWindowChanged);
			FileService.FileRenamed += new FileRenameEventHandler(FileServiceFileRenamed);
			FileService.FileRemoved += new FileEventHandler(FileServiceFileRemoved);
		}
		
		static void FileServiceFileRenamed(object sender, FileRenameEventArgs e)
		{
			if (OpenSolution == null) {
				return;
			}
			string oldName = e.SourceFile;
			string newName = e.TargetFile;
			long x = 0;
			foreach (ISolutionFolderContainer container in OpenSolution.SolutionFolderContainers) {
				foreach (SolutionItem item in container.SolutionItems.Items) {
					string oldFullName  = Path.Combine(OpenSolution.Directory, item.Name);
					++x;
					if (FileUtility.IsBaseDirectory(oldName, oldFullName)) {
						string newFullName = FileUtility.RenameBaseDirectory(oldFullName, oldName, newName);
						item.Name = item.Location = FileUtility.GetRelativePath(OpenSolution.Directory, newFullName);
					}
				}
			}
			
			long y = 0;
			foreach (IProject project in OpenSolution.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, oldName)) {
					foreach (ProjectItem item in project.Items) {
						++y;
						if (FileUtility.IsBaseDirectory(oldName, item.FileName)) {
							item.FileName = FileUtility.RenameBaseDirectory(item.FileName, oldName, newName);
						}
					}
				}
			}
		}
		
		static void FileServiceFileRemoved(object sender, FileEventArgs e)
		{
			if (OpenSolution == null) {
				return;
			}
			string fileName = e.FileName;
			
			foreach (ISolutionFolderContainer container in OpenSolution.SolutionFolderContainers) {
				for (int i = 0; i < container.SolutionItems.Items.Count;) {
					SolutionItem item = container.SolutionItems.Items[i];
					if (FileUtility.IsBaseDirectory(fileName, Path.Combine(OpenSolution.Directory, item.Name))) {
						container.SolutionItems.Items.RemoveAt(i);
					} else {
						++i;
					}
				}
			}
			
			foreach (IProject project in OpenSolution.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, fileName)) {
					for (int i = 0; i < project.Items.Count;) {
						ProjectItem item =project.Items[i];
						if (FileUtility.IsBaseDirectory(fileName, item.FileName)) {
							project.Items.RemoveAt(i);
						} else {
							++i;
						}
					}
				}
			}
		}
		
		static void ActiveWindowChanged(object sender, EventArgs e)
		{
			if (OpenSolution == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return;
			}
			string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
			if (fileName == null) {
				return;
			}
			foreach (IProject project in OpenSolution.Projects) {
				if (project.IsFileInProject(fileName)) {
					CurrentProject = project;
					break;
				}
			}
		}
		
		public static void AddReference(IProject project, ReferenceProjectItem reference)
		{
			project.Items.Add(reference);
			OnReferenceAdded(new ProjectReferenceEventArgs(project, reference));
		}
		
		public static void LoadSolution(string fileName)
		{
			openSolution = Solution.Load(fileName);
			OnSolutionLoaded(new SolutionEventArgs(openSolution));
		}
		
		public static void SaveSolution()
		{
			if (openSolution != null) {
				openSolution.Save();
				foreach (IProject project in openSolution.Projects) {
					project.Save();
				}
				OnSolutionSaved(new SolutionEventArgs(openSolution));
			}
		}
		
		public static void SaveSolutionPreferences()
		{
			// TODO:
		}
		
		public static void CloseSolution()
		{
			if (openSolution != null) {
				OnSolutionClosing(new SolutionEventArgs(openSolution));
				
				openSolution.Dispose();
				openSolution = null;
				
				OnSolutionClosed(EventArgs.Empty);
			}
		}
		
		public static void MarkFileDirty(string fileName)
		{
			if (OpenSolution != null) {
				foreach (IProject project in OpenSolution.Projects) {
					if (project.IsFileInProject(fileName)) {
						MarkProjectDirty(project);
					}
				}
			}
		}
		
		public static void MarkProjectDirty(IProject project)
		{
			project.IsDirty = true;
		}
			
		static void OnCurrentProjectChanged(ProjectEventArgs e)
		{
			if (CurrentProjectChanged != null) {
				CurrentProjectChanged(null, e);
			}
		}
		
		static void OnSolutionClosed(EventArgs e)
		{
			if (SolutionClosed != null) {
				SolutionClosed(null, e);
			}
		}
		
		static void OnSolutionClosing(SolutionEventArgs e)
		{
			if (SolutionClosing != null) {
				SolutionClosing(null, e);
			}
		}
		
		static void OnSolutionLoaded(SolutionEventArgs e)
		{
			if (SolutionLoaded != null) {
				SolutionLoaded(null, e);
			}
		}
		
		static void OnSolutionSaved(SolutionEventArgs e)
		{
			if (SolutionSaved != null) {
				SolutionSaved(null, e);
			}
		}
		
		static void OnProjectConfigurationChanged(ProjectConfigurationEventArgs e) 
		{
			if (ProjectConfigurationChanged != null) {
				ProjectConfigurationChanged(null, e);
			}
		}
		
		static void OnSolutionConfigurationChanged(SolutionConfigurationEventArgs e) 
		{
			if (SolutionConfigurationChanged != null) {
				SolutionConfigurationChanged(null, e);
			}
		}
		
		static void OnReferenceAdded(ProjectReferenceEventArgs e)
		{
			if (ReferenceAdded != null) {
				ReferenceAdded(null, e);
			}
		}
		
		static void OnStartBuild(EventArgs e)
		{
			if (StartBuild != null) {
				StartBuild(null, e);
			}
		}
		
		static void OnEndBuild(EventArgs e) 
		{
			if (EndBuild != null) {
				EndBuild(null, e);
			}
		}
		
		public static void RemoveSolutionFolder(string guid)
		{
			if (OpenSolution == null) {
				return;
			}
			foreach (ISolutionFolder folder in OpenSolution.SolutionFolders) {
				if (folder.IdGuid == guid) {
					folder.Parent.RemoveFolder(folder);
					OnSolutionFolderRemoved(new SolutionFolderEventArgs(folder));
					break;
				}
			}
		}
		
		static void OnSolutionFolderRemoved(SolutionFolderEventArgs e) 
		{
			if (SolutionFolderRemoved != null) {
				SolutionFolderRemoved(null, e);
			}
		}
		public static event SolutionFolderEventHandler SolutionFolderRemoved;
		
		public static event EventHandler StartBuild;
		public static event EventHandler EndBuild;
		
		public static event ProjectConfigurationEventHandler ProjectConfigurationChanged;
		public static event SolutionConfigurationEventHandler SolutionConfigurationChanged;
		
		public static event SolutionEventHandler SolutionLoaded;
		public static event SolutionEventHandler SolutionSaved;
		
		public static event SolutionEventHandler SolutionClosing;
		public static event EventHandler         SolutionClosed;
		
		public static event ProjectEventHandler CurrentProjectChanged;
		
		public static event ProjectReferenceEventHandler ReferenceAdded;
	}
}
