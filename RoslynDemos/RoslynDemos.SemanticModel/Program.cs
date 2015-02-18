﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RoslynDemos.SemanticModel
{
	class Program
	{
		static void Main(string[] args)
		{
			var greeting = "Hello World!" + 5;

			// Let's self-inspect this program and find out the type of 'greeting'
			var sourcePath = Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				"..", // debug
				"..", // bin
				"Program.cs");

			// Get the syntax tree from this source file
			var syntaxTree = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(sourcePath));

			// Run the compiler and get the semantic model
			var semanticModel = CSharpCompilation.Create("selfInspection")
				.AddSyntaxTrees(syntaxTree)
				.AddReferences(	// Add reference to mscorlib
					MetadataReference.CreateFromAssembly(typeof(object).Assembly))
				.GetSemanticModel(syntaxTree);

			// Look for variable declarator of 'greeting'
			var varNode = syntaxTree.GetRoot()
				.DescendantNodes()
				.OfType<LocalDeclarationStatementSyntax>()
				.Select(d => d.Declaration)
				.First(d => d.ChildNodes()
					.OfType<VariableDeclaratorSyntax>()
					.First()
					.Identifier
					.ValueText == "greeting")
				.Type;

			// Get the semantic information for variable declarator
			var semanticInfo = semanticModel.GetTypeInfo(varNode);

			Console.WriteLine(semanticInfo.Type.Name);
		}
	}
}
