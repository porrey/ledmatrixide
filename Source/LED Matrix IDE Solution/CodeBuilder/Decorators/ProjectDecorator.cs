namespace CodeBuilder.Decorators
{
	public static class ProjectDecorator
	{
		public static string CppFileName(this IBuildProject project) => $"{project.Name}-image.cpp";
		public static string HeaderFileName(this IBuildProject project) => $"{project.Name}-image.h";
		public static string MakeFileName(this IBuildProject project) => $"{project.Name}-make.txt";
		public static string InstructionsFileName(this IBuildProject project) => $"{project.Name}-instructions.txt";
	}
}
