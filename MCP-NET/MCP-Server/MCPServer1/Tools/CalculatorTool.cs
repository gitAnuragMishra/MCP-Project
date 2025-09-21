using MCPSharp;

namespace Tools
{
    public class CalculatorTool
    {
        [McpTool(name: "addition", Description = "This function will add two numbers.")]
        public static int Addition(
            [McpParameter(required: true, description: "First number")] int firstNumber,
            [McpParameter(required: true, description: "Second number")] int secondNumber)
        {
            return firstNumber + secondNumber;
        }

        [McpTool(name: "subtraction", Description = "This function will subtract three numbers.")]
        public static int Subtraction(
            [McpParameter(required: true, description: "First number")] int firstNumber,
            [McpParameter(required: true, description: "Second number")] int secondNumber,
            [McpParameter(required: true, description: "Third number")] int thirdNumber)
        {
            return firstNumber - secondNumber - thirdNumber;
        }
    }
}