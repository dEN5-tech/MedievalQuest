using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MedievalQuestGenerator
{
    [Generator]
    public class QuestGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Example generator that creates a simple Quest class
            string sourceCode = @"
using System;

namespace MedievalQuest
{
    public class GodotEngineUIXML
    {

    }
}";
            context.AddSource("GeneratedQuest.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }
} 